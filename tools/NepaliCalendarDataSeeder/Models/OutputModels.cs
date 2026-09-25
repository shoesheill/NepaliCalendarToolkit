using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NepaliCalendarDataSeeder.Models
{
    /// <summary>
    ///     Rich event occurrence written to Data/Events/{year}.json. The endpoint is named
    ///     goverment-holidays, but its response contains festivals, observances and regional
    ///     events as well as government holidays, so EventOutput is the canonical shape.
    /// </summary>
    public class EventOutput
    {
        public string adDate { get; set; }
        public int bsMonth { get; set; }
        public int bsDay { get; set; }
        public int nsYear { get; set; }
        public string nsMonth { get; set; }
        public string nameEn { get; set; }
        public string nameNe { get; set; }
        public string holidayType { get; set; }
        public string category { get; set; }
        public string basedOn { get; set; }
        public bool isGovernmentHoliday { get; set; }
        public bool isImportant { get; set; }
    }

    /// <summary>
    ///     Daily date-conversion details written to Data/DayDetails/{year}.json.
    /// </summary>
    public class DayDetailsOutput
    {
        public string adDate { get; set; }
        public int bsMonth { get; set; }
        public int bsDay { get; set; }
        public int? tithi { get; set; }
        public int? chandrama { get; set; }
        public string nsMonth { get; set; }
        public int? nsYear { get; set; }
        public bool isVerified { get; set; }
    }

    /// <summary>
    ///     Nepal Sambat reference point captured at the start (Baisakh 1) of a BS year.
    /// </summary>
    public class NepaliSambatYear
    {
        public int NsYear { get; set; }
        public string NsMonth { get; set; }
        public string AdStart { get; set; }
    }

    /// <summary>
    ///     Provenance for one BS year, written to Data/month-meta.json.
    ///
    ///     A year can be present but not final: the Nepal Patro API publishes predictions
    ///     ahead of the official gazette (is_verified = 0), and month lengths arrive a month
    ///     at a time. Consumers that merely display a date are happy with a prediction; the
    ///     Nepal IRD registers are not, because a filing must not carry a date that a later
    ///     revision changes. Recording this is what lets each caller decide.
    /// </summary>
    public class YearMeta
    {
        /// <summary>False when any conversion behind this year came back is_verified = 0.</summary>
        [JsonPropertyName("verified")]
        public bool Verified { get; set; }

        /// <summary>How many leading months of the year are known (12 = complete).</summary>
        [JsonPropertyName("knownMonths")]
        public int KnownMonths { get; set; }

        /// <summary>Whether Baisakh 1 of this year is itself verified.</summary>
        [JsonPropertyName("yearStartVerified")]
        public bool YearStartVerified { get; set; }

        /// <summary>UTC timestamp of the run that last wrote this year.</summary>
        [JsonPropertyName("seededAt")]
        public string SeededAt { get; set; }
    }

    /// <summary>Result of a single year's seeding (used for logging + SeedInfo.json).</summary>
    public class SeedRecord
    {
        public int Year { get; set; }
        public bool MonthLengths { get; set; }

        /// <summary>Leading months resolved this run; 12 means the year closed.</summary>
        public int KnownMonths { get; set; }

        /// <summary>False when any part of the year rests on an unverified prediction.</summary>
        public bool Verified { get; set; }
        public bool YearStart { get; set; }
        public int HolidayCount { get; set; }
        public int EventCount { get; set; }
        public int DayDetailsCount { get; set; }
        public bool NepaliSambat { get; set; }
    }
}
