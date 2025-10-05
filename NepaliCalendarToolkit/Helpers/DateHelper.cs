using System;
using System.Runtime.InteropServices;
using NepaliCalendarToolkit.Models;

namespace NepaliCalendarToolkit.Helpers
{
    public static class DateHelper
    {
        public static string LocaliseDate(DateTime date)
        {
            var timeZoneId = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? "Nepal Standard Time"
                : "Asia/Kathmandu";
            var kathmanduTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            var localDate = TimeZoneInfo.ConvertTime(date, kathmanduTimeZone);
            return localDate.ToString("yyyy-MM-dd");
        }

        public static bool IsWeekend(DateTime date)
        {
            return WeekendConfiguration.IsWeekend(date.DayOfWeek);
        }
    }
}