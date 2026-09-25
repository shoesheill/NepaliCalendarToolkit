using System;

namespace NepaliCalendarToolkit.Models
{
    /// <summary>
    ///     One event occurrence returned by the Nepal Patro government-holidays endpoint.
    ///     The endpoint also returns festivals and observances, so this model intentionally
    ///     does not call every record a holiday.
    /// </summary>
    public class CalendarEvent
    {
        public DateTime AdDate { get; internal set; }
        public NepaliDate BsDate { get; internal set; }
        public int NepaliSambatYear { get; internal set; }
        public string NepaliSambatMonthCode { get; internal set; }
        public string NameEn { get; internal set; }
        public string NameNe { get; internal set; }
        public string HolidayType { get; internal set; }
        public string Category { get; internal set; }
        public string BasedOn { get; internal set; }
        public bool IsGovernmentHoliday { get; internal set; }
        public bool IsImportant { get; internal set; }
    }
}