using System;
using System.Collections.Generic;

namespace NepaliCalendarToolkit
{
    public static class WeekendConfiguration
    {
        // Default weekend days (Saturday and Sunday)
        private static HashSet<DayOfWeek> _weekendDays = new HashSet<DayOfWeek>
        {
            DayOfWeek.Saturday,
            DayOfWeek.Sunday
        };

        /// <summary>
        ///     Sets which days of the week are considered weekends
        /// </summary>
        /// <param name="weekendDays">Array of days to be considered weekends</param>
        public static void SetWeekendDays(params DayOfWeek[] weekendDays)
        {
            _weekendDays = new HashSet<DayOfWeek>(weekendDays);
        }

        /// <summary>
        ///     Checks if the given day is a weekend according to the current configuration
        /// </summary>
        /// <param name="dayOfWeek">The day to check</param>
        /// <returns>True if the day is configured as a weekend day</returns>
        public static bool IsWeekend(DayOfWeek dayOfWeek)
        {
            return _weekendDays.Contains(dayOfWeek);
        }

        /// <summary>
        ///     Gets all currently configured weekend days
        /// </summary>
        /// <returns>Array of configured weekend days</returns>
        public static DayOfWeek[] GetWeekendDays()
        {
            DayOfWeek[] result = new DayOfWeek[_weekendDays.Count];
            _weekendDays.CopyTo(result);
            return result;
        }
    }
}