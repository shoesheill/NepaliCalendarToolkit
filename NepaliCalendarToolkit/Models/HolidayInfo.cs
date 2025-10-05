using System;

namespace NepaliCalendarToolkit.Models
{
    public class HolidayInfo
    {
        public HolidayInfo(string dayName, string holidayName, DateTime adDate, string bsDate)
        {
            DayName = dayName;
            HolidayName = holidayName;
            AdDate = adDate;
            BsDate = bsDate;
        }

        private string DayName { get; }
        private string HolidayName { get; }
        private DateTime AdDate { get; }
        private string BsDate { get; }
        public DateTime GetAdDate => AdDate;
    }
}