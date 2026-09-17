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
        private Dictionary<int, YearMeta> _monthMeta;

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

            // Highest fully-contiguous seeded year. month-lengths is the bottleneck: it can
            // only be finished once the NEXT year's Baisakh 1 is published, so holiday and
            // year-start files legitimately get ahead of it (e.g. a future year may have
            // holidays but not month-lengths yet). Basing maxExisting on month-lengths alone
            // ensures a later re-run re-seeds those in-between years and fills the gap
            // instead of wrongly reporting "already up to date".
            //
            // Use the highest CONTIGUOUS year (starting from the library minimum, 2065)
            // rather than a bare Max(). A stray out-of-order entry -- e.g. month-lengths
            // seeded for 2084 while 2083 was skipped because Baisakh 1 of 2084 was not yet
            // published at the moment 2083 was processed -- would otherwise be treated as
            // "already fully seeded", push startYear past the target, and both block a
            // default re-run AND leave the missing 2083 gap permanently unfilled.
            const int minYear = 2065;
            var maxExisting = minYear - 1;
            // A PARTIAL year does not count as present: month lengths are now stored as a
            // prefix, so "the key exists" no longer means "the year is closed". Requiring a
            // full twelve keeps re-runs coming back to finish it.
            while (KnownMonthsOf(maxExisting + 1) == 12)
                maxExisting++;

            // The library's minimum supported year is 2065 BS. If no (or only newer)
            // data exists, seed from 2065 so the whole supported range is covered.
            var startYear = Math.Max(2065, maxExisting + 1);
            Console.WriteLine($"Current BS year (as of {adDate:yyyy-MM-dd}) : {currentBsYear}");
            Console.WriteLine($"Target BS year                        : {targetBsYear}");
            Console.WriteLine($"Highest year already present           : {maxExisting}");
            Console.WriteLine($"Seeding range                          : {startYear}..{targetBsYear}");
            Console.WriteLine(new string('-', 60));

            // Years to touch: the new range, plus any year still incomplete or resting on a
            // prediction. Without the second group a year seeded early — when the API could
            // only predict it — would stay provisional forever, because the contiguous-max
            // scan would have moved past it.
            //
            // Bounded below by the current BS year. A year that has already ended is history
            // and will not be revised by a nightly job; if its data ever needs correcting
            // that is a deliberate --max-year run. Without this the oldest year re-fetched
            // every single night forever: the API reports BS 2065 (AD 2008) as
            // is_verified = 0, presumably predating its gazette records, so it can never
            // flip to verified and the retry could never succeed.
            var refreshable = _monthLengths.Keys
                .Concat(_yearStarts.Keys)
                .Where(y => y >= minYear && y >= (currentBsYear - 1) && y < startYear && NeedsRefresh(y))
                .Distinct()
                .OrderBy(y => y)
                .ToList();

            if (refreshable.Count > 0)
                Console.WriteLine($"Re-checking provisional/partial years   : {string.Join(", ", refreshable)}");

            var targets = refreshable
                .Concat(Enumerable.Range(startYear, Math.Max(0, targetBsYear - startYear + 1)))
                .Distinct()
                .OrderBy(y => y)
                .ToList();

            var records = new List<SeedRecord>();

            foreach (var year in targets)
            {
                var record = await SeedYearAsync(year);
                records.Add(record);
                Console.WriteLine(
                    $"Year {year}: months={record.KnownMonths}/12 " +
                    $"verified={(record.Verified ? "Y" : "n")} " +
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
                WriteJson(Path.Combine(_dataDir, "month-meta.json"), _monthMeta);
                // WriteJson(Path.Combine(_dataDir, "nepali-sambat.json"), _nepaliSambat);

                WriteSeedInfo(records, adDate, currentBsYear, targetBsYear);
                Console.WriteLine(new string('-', 60));
                Console.WriteLine($"Done. Seeded {records.Count} year(s). Wrote data under: {_dataDir}");
            }

            return records.Count;
        }

        /// <summary>Leading months currently stored for a year (0 when absent).</summary>
        private int KnownMonthsOf(int year) =>
            _monthLengths.TryGetValue(year, out var lengths) ? lengths.Length : 0;

        /// <summary>True when the stored year is complete AND officially confirmed.</summary>
        private bool IsVerified(int year) =>
            _monthMeta.TryGetValue(year, out var meta) && meta.Verified && meta.KnownMonths == 12;

        /// <summary>A year still worth re-fetching: incomplete, or resting on a prediction.</summary>
        private bool NeedsRefresh(int year) => !IsVerified(year);

        /// <summary>
        ///     Records only that Baisakh 1 of a year is known. Deliberately does NOT touch
        ///     `verified`: confirming a year's first day says nothing about whether its month
        ///     lengths are gazetted, and conflating the two marked a fully-predicted year as
        ///     final.
        /// </summary>
        private void UpsertYearStartMeta(int year, bool yearStartVerified)
        {
            _monthMeta.TryGetValue(year, out var existing);
            _monthMeta[year] = new YearMeta
            {
                Verified = existing?.Verified ?? false,
                KnownMonths = existing?.KnownMonths ?? KnownMonthsOf(year),
                YearStartVerified = (existing?.YearStartVerified ?? false) || yearStartVerified,
                SeededAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
            };
        }

        private void UpsertMeta(int year, bool verified, int knownMonths, bool yearStartVerified)
        {
            if (_monthMeta.TryGetValue(year, out var existing))
            {
                // Monotonic: a year never loses confirmation or months it already had.
                verified = existing.Verified || verified;
                knownMonths = Math.Max(existing.KnownMonths, knownMonths);
                yearStartVerified = existing.YearStartVerified || yearStartVerified;
            }

            _monthMeta[year] = new YearMeta
            {
                Verified = verified && knownMonths == 12,
                KnownMonths = knownMonths,
                YearStartVerified = yearStartVerified,
                SeededAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
            };
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

                var yearStartVerified = start.IsVerified != 0;
                if (!yearStartVerified)
                    Console.WriteLine($"Year {year}: warning - Baisakh 1 is_verified=0 (unverified prediction).");

                // Never let a prediction overwrite data that was already confirmed. The API
                // can serve an unverified value for a year it previously gazetted, and
                // silently downgrading would make a settled date move again.
                if (IsVerified(year) && !yearStartVerified)
                {
                    Console.WriteLine($"Year {year}: already verified, refusing to overwrite with a prediction.");
                    record.YearStart = true;
                    record.MonthLengths = _monthLengths.ContainsKey(year);
                    record.KnownMonths = _monthLengths.TryGetValue(year, out var kept) ? kept.Length : 0;
                    record.Verified = true;
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

                // 2) Month lengths = difference between consecutive month starts.
                //
                // Kept as a PREFIX rather than all-or-nothing. Months publish one at a time,
                // and the twelfth needs the FOLLOWING year's Baisakh 1 — so demanding all
                // twelve threw away months that were already known. A fiscal year only needs
                // the first three months of its closing year, so a prefix of 3 is enough to
                // resolve it roughly nine months earlier than before.
                var lengths = new List<int>(12);
                var verified = yearStartVerified;
                var previous = ParseAd(start.Ad);
                for (var m = 1; m <= 12; m++)
                {
                    var nextYear = m == 12 ? year + 1 : year;
                    var nextMonth = m == 12 ? 1 : m + 1;
                    var next = await _api.ConvertBsAsync(nextYear, nextMonth, 1);

                    if (next == null || string.IsNullOrWhiteSpace(next.Ad))
                    {
                        Console.WriteLine(
                            $"Year {year}: month {m} next-start unavailable, keeping {lengths.Count} known month(s).");
                        break;
                    }

                    if (next.IsVerified == 0) verified = false;

                    var nextAd = ParseAd(next.Ad);
                    lengths.Add((int)(nextAd - previous).TotalDays);
                    previous = nextAd;

                    // The month-12 boundary IS Baisakh 1 of the next year. It was already
                    // being fetched and thrown away, so the next year's start came for free
                    // and was simply never written.
                    if (m == 12)
                    {
                        _yearStarts[year + 1] = next.Ad;
                        _nepaliSambat[year + 1] = new NepaliSambatYear
                        {
                            NsYear = next.NsYear ?? 0,
                            NsMonth = next.NsMonth,
                            AdStart = next.Ad
                        };
                        UpsertYearStartMeta(year + 1, next.IsVerified != 0);
                    }
                }

                // Only widen. A run that resolved fewer months than are already stored (a
                // transient API gap) must not shrink the file.
                if (lengths.Count > 0 && lengths.Count >= KnownMonthsOf(year))
                    _monthLengths[year] = lengths.ToArray();

                record.KnownMonths = KnownMonthsOf(year);
                record.MonthLengths = record.KnownMonths == 12;
                record.Verified = verified;
                UpsertMeta(year, verified, record.KnownMonths, yearStartVerified);

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
            _monthMeta = ReadJson<Dictionary<int, YearMeta>>("month-meta.json") ?? new Dictionary<int, YearMeta>();
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
