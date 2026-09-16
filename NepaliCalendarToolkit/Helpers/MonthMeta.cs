using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NepaliCalendarToolkit.Helpers
{
    /// <summary>
    ///     Provenance for one BS year (Data/month-meta.json).
    ///
    ///     A year can be present without being final: Nepal Patro publishes predictions ahead
    ///     of the official gazette, and month lengths arrive a month at a time. A caller that
    ///     merely displays a period is happy with a prediction; one that writes a date onto a
    ///     tax filing is not, because a revision would move a date already submitted.
    /// </summary>
    public class YearMeta
    {
        [JsonPropertyName("verified")]
        public bool Verified { get; set; }

        [JsonPropertyName("knownMonths")]
        public int KnownMonths { get; set; }

        [JsonPropertyName("yearStartVerified")]
        public bool YearStartVerified { get; set; }

        [JsonPropertyName("seededAt")]
        public string SeededAt { get; set; }
    }

    /// <summary>
    ///     Loads per-year provenance, mirroring <see cref="MonthLengths" />. Absent for data
    ///     seeded before month-meta.json existed, which is treated as "final if complete".
    /// </summary>
    public static class MonthMeta
    {
        public static readonly Dictionary<int, YearMeta> Meta;

        static MonthMeta()
        {
            Meta = DataProvider.GetData(
                "month-meta.json",
                DataProvider.GetEmbedded<Dictionary<int, YearMeta>>("month-meta.json"))
                ?? new Dictionary<int, YearMeta>();
        }
    }
}
