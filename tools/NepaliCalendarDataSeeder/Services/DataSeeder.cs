using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using NepaliCalendarDataSeeder.Models;

namespace NepaliCalendarDataSeeder.Services
{
    /// <summary>
    ///     Incrementally seeds calendar data (month lengths, year starts, holidays and a
    ///     Nepal Sambat reference) from the Nepal Patro API, writing the same JSON shapes
    ///     used by the NepaliCalendarToolkit library.
    /// </summary>
    public class DataSeeder
    {
        private readonly ApiClient _api;
        private readonly string _dataDir;

        // Existing data loaded from disk, so we only fill the missing gap.
        private Dictionary<int, int[]> _monthLengths;
        private Dictionary<int, string> _yearStarts;
        private Dictionary<int, NepaliSambatYear> _nepaliSambat;
        private HashSet<int> _holidayYears;

        public DataSeeder(ApiClient api, string dataDir)
        {
            _api = api;
            _dataDir = dataDir;
        }

        public async Task<int> RunAsync(DateTime adDate, int targetOffset, int? maxYear)
        {
            LoadExisting();

            var currentBsYear = ComputeCurrentBsYear(adDate);
            var targetBsYear = maxYear ?? (currentBsYear + targetOffset);

            // Highest *completely* seeded year. month-lengths is the bottleneck: it can
            // only be finished once the NEXT year's Baisakh 1 is published, so holiday and
            // year-start files legitimately get ahead of it (e.g. a future year may have
            // holidays but not month-lengths yet). Basing maxExisting on month-lengths alone
            // ensures a later re-run re-seeds those in-between years and fills the gap
            // instead of wrongly reporting "already up to date".
            var maxExisting = _monthLengths.Keys
                .DefaultIfEmpty(2064)
                .Max();

            // The library's minimum supported year is 2065 BS. If no (or only newer)
            // data exists, seed from 2065 so the whole supported range is covered.
            var startYear = Math.Max(2065, maxExisting + 1);
            Console.WriteLine($"Current BS year (as of {adDate:yyyy-MM-dd}) : {currentBsYear}");
            Console.WriteLine($"Target BS year                        : {targetBsYear}");
            Console.WriteLine($"Highest year already present           : {maxExisting}");
            Console.WriteLine($"Seeding range                          : {startYear}..{targetBsYear}");
            Console.WriteLine(new string('-', 60));

            var records = new List<SeedRecord>();

            for (var year = startYear; year <= targetBsYear; year++)
            {
                var record = await SeedYearAsync(year);
                records.Add(record);
                Console.WriteLine(
                    $"Year {year}: month-lengths={(record.MonthLengths ? "Y" : "n")} " +
                    $"year-start={(record.YearStart ? "Y" : "n")} holidays={record.HolidayCount} ns={(record.NepaliSambat ? "Y" : "n")}");
            }

            if (records.Count == 0)
            {
                Console.WriteLine("No new years to seed. Data is already up to date.");
            }
            else
            {
                // Persist the aggregate data files (in-memory dictionaries are updated
                // during seeding; write them all out once the loop finishes).
                WriteJson(Path.Combine(_dataDir, "month-lengths.json"), _monthLengths);
                WriteJson(Path.Combine(_dataDir, "year-start.json"), _yearStarts);
                // WriteJson(Path.Combine(_dataDir, "nepali-sambat.json"), _nepaliSambat);

                WriteSeedInfo(records, adDate, currentBsYear, targetBsYear);
                Console.WriteLine(new string('-', 60));
                Console.WriteLine($"Done. Seeded {records.Count} year(s). Wrote data under: {_dataDir}");
            }

            return records.Count;
        }

        private async Task<SeedRecord> SeedYearAsync(int year)
        {
            var record = new SeedRecord { Year = year };

            try
            {
                // 1) Year start + Nepal Sambat reference from Baisakh 1.
                var start = await _api.ConvertBsAsync(year, 1, 1);
                if (start == null || string.IsNullOrWhiteSpace(start.Ad))
                {
                    Console.WriteLine($"Year {year}: dateConvert failed, skipping (data not available yet).");
                    return record;
                }

                _yearStarts[year] = start.Ad;
                record.YearStart = true;

                // Nepal Sambat reference at the BS year start.
                _nepaliSambat[year] = new NepaliSambatYear
                {
                    NsYear = start.NsYear ?? 0,
                    NsMonth = start.NsMonth,
                    AdStart = start.Ad
                };
                record.NepaliSambat = true;

                if (start.IsVerified == 0)
                    Console.WriteLine($"Year {year}: warning - Baisakh 1 is_verified=0 (unverified prediction).");

                // 2) Month lengths = difference between consecutive month starts.
                var lengths = new int[12];
                var previous = ParseAd(start.Ad);
                for (var m = 1; m <= 12; m++)
                {
                    var nextYear = m == 12 ? year + 1 : year;
                    var nextMonth = m == 12 ? 1 : m + 1;
                    var next = await _api.ConvertBsAsync(nextYear, nextMonth, 1);

                    if (next == null || string.IsNullOrWhiteSpace(next.Ad))
                    {
                        Console.WriteLine($"Year {year}: month {m} next-start unavailable, month-lengths skipped.");
                        record.MonthLengths = false;
                        return record;
                    }

                    var nextAd = ParseAd(next.Ad);
                    lengths[m - 1] = (int)(nextAd - previous).TotalDays;
                    previous = nextAd;
                }
                _monthLengths[year] = lengths;
                record.MonthLengths = true;

                // 3) Government holidays.
                var holidays = await _api.GetGovernmentHolidaysAsync(year);
                if (holidays == null || holidays.Count == 0)
                {
                    Console.WriteLine($"Year {year}: no government holiday data yet, holidays skipped.");
                }
                else
                {
                    var outputs = MapHolidays(holidays);
                    WriteJson(Path.Combine(_dataDir, "Holidays", $"{year}.json"), outputs);
                    _holidayYears.Add(year);
                    record.HolidayCount = outputs.Count;
                    Console.WriteLine($"Year {year}: holidays written ({outputs.Count}).");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Year {year}: ERROR - {ex.Message}");
            }

            return record;
        }

        private List<HolidayOutput> MapHolidays(List<GovernmentHolidayResponse> holidays)
        {
            var results = new List<HolidayOutput>();

            foreach (var h in holidays)
            {
                // Parse BS "dd.mm.yyyy" -> month, day.
                var parts = (h.Bs ?? string.Empty).Split('.');
                if (parts.Length != 3)
                    continue;

                if (!int.TryParse(parts[1], out var month) || !int.TryParse(parts[0], out var day))
                    continue;

                var englishName = ExtractEnglishName(h.Description);
                if (string.IsNullOrWhiteSpace(englishName))
                    englishName = h.Title; // fall back to (Nepali) title

                results.Add(new HolidayOutput
                {
                    month = month,
                    day = day,
                    date = h.Ad,
                    name = englishName
                });
            }

            // Keep deterministic ordering: prefer AD date, then month/day.
            return results
                .OrderBy(x => x.date, StringComparer.Ordinal)
                .ThenBy(x => x.month)
                .ThenBy(x => x.day)
                .ToList();
        }

        private static string ExtractEnglishName(string descriptionJson)
        {
            if (string.IsNullOrWhiteSpace(descriptionJson)) return null;
            try
            {
                using var doc = JsonDocument.Parse(descriptionJson);
                if (doc.RootElement.TryGetProperty("en", out var en))
                {
                    var value = en.GetString();
                    if (string.IsNullOrWhiteSpace(value)) return null;
                    // Remove backslashes that appear in some API descriptions.
                    return value.Replace("\\", "").Trim();
                }
            }
            catch
            {
                // Not JSON -> treat the raw string as the name.
            }

            var raw = descriptionJson.Replace("\\", "").Trim();
            return raw;
        }

        private void LoadExisting()
        {
            _monthLengths = ReadJson<Dictionary<int, int[]>>("month-lengths.json") ?? new Dictionary<int, int[]>();
            _yearStarts = ReadJson<Dictionary<int, string>>("year-start.json") ?? new Dictionary<int, string>();
            _nepaliSambat = ReadJson<Dictionary<int, NepaliSambatYear>>("nepali-sambat.json") ?? new Dictionary<int, NepaliSambatYear>();

            _holidayYears = new HashSet<int>();
            var holidaysDir = Path.Combine(_dataDir, "Holidays");
            if (Directory.Exists(holidaysDir))
            {
                foreach (var file in Directory.GetFiles(holidaysDir, "*.json"))
                {
                    var name = Path.GetFileNameWithoutExtension(file);
                    if (int.TryParse(name, out var yr)) _holidayYears.Add(yr);
                }
            }
        }

        private T ReadJson<T>(string fileName)
        {
            var path = Path.Combine(_dataDir, fileName);
            if (!File.Exists(path)) return default;
            try
            {
                return JsonSerializer.Deserialize<T>(File.ReadAllText(path));
            }
            catch
            {
                return default;
            }
        }

        private void WriteSeedInfo(List<SeedRecord> records, DateTime adDate, int currentBsYear, int targetBsYear)
        {
            var info = new
            {
                generated_at_utc = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                run_date = adDate.ToString("yyyy-MM-dd"),
                current_bs_year = currentBsYear,
                target_bs_year = targetBsYear,
                years_seeded = records.Select(r => r.Year).ToList(),
                // api_base_url = _api.BaseUrlForLogging
            };
            // Metadata goes OUTSIDE the embedded Data folder (repo root) so it is never
            // packed into the NuGet package as calendar data.
            WriteJson(Path.Combine(Environment.CurrentDirectory, "SeedInfo.json"), info);
        }

        private void WriteJson<T>(string path, T data)
        {
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
            var options = new JsonSerializerOptions { WriteIndented = true };
            // Render int[] arrays (month lengths) inline so each year stays on its own line.
            options.Converters.Add(new CompactIntArrayConverter());
            File.WriteAllText(path, JsonSerializer.Serialize(data, options));
        }

        private static DateTime ParseAd(string ad)
        {
            return DateTime.ParseExact(ad, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     Converts an AD date to a BS year. The BS new year (Baisakh 1) begins around
        ///     April 14, so dates on/after that point roll over to the next BS year while
        ///     earlier dates remain in the "AD + 56" year.
        /// </summary>
        public static int ComputeCurrentBsYear(DateTime ad)
        {
            var bs = ad.Year + 56;
            var boundary = new DateTime(ad.Year, 4, 1);
            if (ad >= boundary) bs++;
            return bs;
        }
    }
}
