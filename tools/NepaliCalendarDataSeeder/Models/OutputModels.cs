using System.Collections.Generic;

namespace NepaliCalendarDataSeeder.Models
{
    /// <summary>
    ///     The exact holiday shape written to Data/Holidays/{year}.json, matching the
    ///     NepaliCalendarToolkit.HolidayJson.HolidayData model.
    /// </summary>
    public class HolidayOutput
    {
        public int month { get; set; }
        public int day { get; set; }
        public string date { get; set; }
        public string name { get; set; }
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

    /// <summary>Result of a single year's seeding (used for logging + SeedInfo.json).</summary>
    public class SeedRecord
    {
        public int Year { get; set; }
        public bool MonthLengths { get; set; }
        public bool YearStart { get; set; }
        public int HolidayCount { get; set; }
        public bool NepaliSambat { get; set; }
    }
}
