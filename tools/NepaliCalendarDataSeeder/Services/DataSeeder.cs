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
    ///     Incrementally seeds calendar data (month lengths, year starts, daily details, events
    ///     and a Nepal Sambat reference) from the Nepal Patro API.
    /// </summary>
    public class DataSeeder
    {
        private readonly ApiClient _api;
        private readonly string _dataDir;

        // Existing data loaded from disk, so we only fill the missing gap.
        private Dictionary<int, int[]> _monthLengths;
        private Dictionary<int, string> _yearStarts;
        private Dictionary<int, NepaliSambatYear> _nepaliSambat;
        private HashSet<int> _eventYears;
        private Dictionary<int, YearMeta> _monthMeta;

        public DataSeeder(ApiClient api, string dataDir)
        {
            _api = api;
            _dataDir = dataDir;
        }

        public async Task<int> RunAsync(
            DateTime adDate,
            int targetOffset,
            int? maxYear,
            int? detailsYear = null,
            int? detailsMonth = null)
        {
            LoadExisting();

            if (detailsYear.HasValue)
            {
                if (detailsMonth.HasValue && (detailsMonth.Value < 1 || detailsMonth.Value > 12))
                    throw new ArgumentOutOfRangeException(nameof(detailsMonth), "Details month must be between 1 and 12.");

                var detailRecord = await RefreshYearDetailsAsync(detailsYear.Value, detailsMonth);
                WriteSeedInfo(new List<SeedRecord> { detailRecord }, adDate, ComputeCurrentBsYear(adDate), detailsYear.Value);
                return 1;
            }

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

            if (detailsYear.HasValue)
            {
                // An explicit detail refresh is intentionally isolated. Do not spend hours
                // refreshing unrelated provisional years before the requested daily file.
                refreshable = new List<int>();
            }
            else if (refreshable.Count > 0)
            {
                Console.WriteLine($"Re-checking provisional/partial years   : {string.Join(", ", refreshable)}");
            }

            var targets = detailsYear.HasValue
                ? new List<int> { detailsYear.Value }
                : refreshable
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
                    $"year-start={(record.YearStart ? "Y" : "n")} events={record.EventCount} " +
                    $"details={record.DayDetailsCount} ns={(record.NepaliSambat ? "Y" : "n")}");
            }

            if (detailsYear.HasValue && !targets.Contains(detailsYear.Value))
            {
                var record = await RefreshYearDetailsAsync(detailsYear.Value, detailsMonth);
                records.Add(record);
                Console.WriteLine(
                    $"Year {record.Year}: events={record.EventCount} details={record.DayDetailsCount} (forced refresh)");
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

                // 3) All event occurrences returned by the endpoint. The endpoint name says
                // "government holidays", but the payload also contains festivals and regional
                // or international observances.
                var events = await _api.GetGovernmentHolidaysAsync(year);
                if (events == null)
                {
                    Console.WriteLine($"Year {year}: no event data yet, events skipped.");
                }
                else
                {
                    var outputs = MapEvents(events);
                    WriteJson(Path.Combine(_dataDir, "Events", $"{year}.json"), outputs);
                    _eventYears.Add(year);
                    record.EventCount = outputs.Count;
                    Console.WriteLine($"Year {year}: events written ({outputs.Count}).");
                }

                var dayDetails = await FetchDayDetailsAsync(year);
                // Mirror the detail-refresh guard: a transient API gap can return fewer days than
                // the published month lengths, and that partial list must neither overwrite a
                // complete DayDetails file nor become the file of record.
                var expectedDays = _monthLengths.TryGetValue(year, out var knownLengths) && knownLengths != null
                    ? knownLengths.Sum()
                    : 0;
                if (dayDetails.Count > 0 && dayDetails.Count == expectedDays)
                {
                    WriteJson(Path.Combine(_dataDir, "DayDetails", $"{year}.json"), dayDetails);
                    record.DayDetailsCount = dayDetails.Count;
                    Console.WriteLine($"Year {year}: day details written ({dayDetails.Count}).");
                }
                else if (dayDetails.Count > 0)
                {
                    Console.WriteLine(
                        $"Year {year}: day details incomplete ({dayDetails.Count}/{expectedDays}), existing file kept.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Year {year}: ERROR - {ex.Message}");
            }

            return record;
        }

        private async Task<SeedRecord> RefreshYearDetailsAsync(int year, int? month)
        {
            var record = new SeedRecord { Year = year };
            try
            {
                var events = await _api.GetGovernmentHolidaysAsync(year);
                if (events != null)
                {
                    var outputs = MapEvents(events);
                    WriteJson(Path.Combine(_dataDir, "Events", $"{year}.json"), outputs);
                    _eventYears.Add(year);
                    record.EventCount = outputs.Count;
                    Console.WriteLine($"Year {year}: events written ({outputs.Count}).");
                }
                else
                {
                    Console.WriteLine($"Year {year}: no event data yet, events skipped.");
                }

                var partialPath = Path.Combine(_dataDir, "DayDetails", $".partial-{year}.json");
                var existing = ReadJson<List<DayDetailsOutput>>(partialPath) ?? new List<DayDetailsOutput>();
                var details = await FetchDayDetailsAsync(year, month, existing);

                if (!_monthLengths.TryGetValue(year, out var lengths) || lengths == null || lengths.Length != 12)
                {
                    Console.WriteLine($"Year {year}: complete month lengths unavailable; partial day details retained.");
                    WriteJson(partialPath, details);
                    record.DayDetailsCount = details.Count;
                }
                else if (details.Count == lengths.Sum())
                {
                    WriteJson(Path.Combine(_dataDir, "DayDetails", $"{year}.json"),
                        details.OrderBy(x => x.adDate, StringComparer.Ordinal).ToList());
                    File.Delete(partialPath);
                    record.DayDetailsCount = details.Count;
                    Console.WriteLine($"Year {year}: complete day details written ({details.Count}).");
                }
                else
                {
                    WriteJson(partialPath, details);
                    record.DayDetailsCount = details.Count;
                    Console.WriteLine($"Year {year}: partial day details retained ({details.Count}/{lengths.Sum()}).");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Year {year}: detail refresh ERROR - {ex.Message}");
            }

            return record;
        }

        private async Task<List<DayDetailsOutput>> FetchDayDetailsAsync(
            int year,
            int? requestedMonth = null,
            List<DayDetailsOutput> existing = null)
        {
            var results = existing ?? new List<DayDetailsOutput>();
            if (!_monthLengths.TryGetValue(year, out var lengths) || lengths == null || lengths.Length == 0)
            {
                Console.WriteLine($"Year {year}: no month lengths, day details skipped.");
                return results;
            }

            // A prefix of month lengths is enough for date conversion, but it is not a
            // complete calendar year. Never publish a partial DayDetails file.
            if (lengths.Length != 12)
            {
                Console.WriteLine($"Year {year}: month lengths are incomplete ({lengths.Length}/12), day details skipped.");
                return results;
            }

            var firstMonth = requestedMonth ?? 1;
            var lastMonth = requestedMonth ?? lengths.Length;

            for (var month = firstMonth; month <= lastMonth; month++)
            {
                results.RemoveAll(x => x.bsMonth == month);
                for (var day = 1; day <= lengths[month - 1]; day++)
                {
                    var response = await _api.ConvertBsAsync(year, month, day);
                    if (response == null || string.IsNullOrWhiteSpace(response.Ad))
                        continue;

                    results.Add(new DayDetailsOutput
                    {
                        adDate = response.Ad,
                        bsMonth = response.BsMonth ?? month,
                        bsDay = response.BsDay ?? day,
                        tithi = response.Tithi,
                        chandrama = response.Chandrama,
                        nsMonth = response.NsMonth,
                        nsYear = response.NsYear,
                        isVerified = response.IsVerified == 1
                    });
                }

                Console.WriteLine($"Year {year}: day details month {month}/{lengths.Length} complete ({results.Count} day(s)).");
            }

            return results.OrderBy(x => x.adDate, StringComparer.Ordinal).ToList();
        }

        private List<EventOutput> MapEvents(List<GovernmentHolidayResponse> events)
        {
            var results = new List<EventOutput>();

            foreach (var h in events)
            {
                // Parse BS "dd.mm.yyyy" -> month, day.
                var parts = (h.Bs ?? string.Empty).Split('.');
                if (parts.Length != 3)
                    continue;

                if (!int.TryParse(parts[1], out var month) || !int.TryParse(parts[0], out var day))
                    continue;

                var metadata = ParseDescription(h.Description);
                var englishName = metadata.En;
                var nepaliName = metadata.Ne;
                if (string.IsNullOrWhiteSpace(englishName)) englishName = h.Title;

                var adDate = string.IsNullOrWhiteSpace(h.Ad) ? h.EventDate : h.Ad;
                results.Add(new EventOutput
                {
                    adDate = adDate,
                    bsMonth = month,
                    bsDay = day,
                    nsYear = h.NsYear,
                    nsMonth = h.NsMonth,
                    nameEn = englishName,
                    nameNe = nepaliName,
                    holidayType = h.HolidayType,
                    category = metadata.Category,
                    basedOn = h.BasedOn,
                    isGovernmentHoliday = IsTrue(metadata.Gh),
                    isImportant = IsTrue(metadata.ImportantEvent)
                });
            }

            // Keep deterministic ordering: prefer AD date, then month/day.
            return results
                .OrderBy(x => x.adDate, StringComparer.Ordinal)
                .ThenBy(x => x.bsMonth)
                .ThenBy(x => x.bsDay)
                .ThenBy(x => x.nameEn, StringComparer.Ordinal)
                .ToList();
        }

        private static bool IsTrue(string value) => value == "1" ||
            string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);

        private sealed class DescriptionMetadata
        {
            public string En { get; set; }
            public string Ne { get; set; }
            public string Category { get; set; }
            public string Gh { get; set; }
            public string ImportantEvent { get; set; }
        }

        private static DescriptionMetadata ParseDescription(string descriptionJson)
        {
            var metadata = new DescriptionMetadata();
            if (string.IsNullOrWhiteSpace(descriptionJson)) return metadata;
            try
            {
                using var doc = JsonDocument.Parse(descriptionJson);
                metadata.En = GetString(doc.RootElement, "en");
                metadata.Ne = GetString(doc.RootElement, "ne");
                metadata.Category = GetString(doc.RootElement, "category");
                metadata.Gh = GetString(doc.RootElement, "gh");
                metadata.ImportantEvent = GetString(doc.RootElement, "important_event");
            }
            catch
            {
                metadata.En = descriptionJson.Replace("\\", "").Trim();
            }

            return metadata;
        }

        private static string GetString(JsonElement root, string property)
        {
            if (!root.TryGetProperty(property, out var value)) return null;
            return value.ValueKind == JsonValueKind.String
                ? value.GetString()?.Replace("\\", "").Trim()
                : value.ToString();
        }

        private void LoadExisting()
        {
            _monthLengths = ReadJson<Dictionary<int, int[]>>("month-lengths.json") ?? new Dictionary<int, int[]>();
            _yearStarts = ReadJson<Dictionary<int, string>>("year-start.json") ?? new Dictionary<int, string>();
            _monthMeta = ReadJson<Dictionary<int, YearMeta>>("month-meta.json") ?? new Dictionary<int, YearMeta>();
            _nepaliSambat = ReadJson<Dictionary<int, NepaliSambatYear>>("nepali-sambat.json") ?? new Dictionary<int, NepaliSambatYear>();

            _eventYears = new HashSet<int>();
            var eventsDir = Path.Combine(_dataDir, "Events");
            if (Directory.Exists(eventsDir))
            {
                foreach (var file in Directory.GetFiles(eventsDir, "*.json"))
                {
                    var name = Path.GetFileNameWithoutExtension(file);
                    if (int.TryParse(name, out var yr)) _eventYears.Add(yr);
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
