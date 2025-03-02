using System;
using System.Globalization;
using System.Linq;

public class CalendarUtilities
{
    internal static int GetYear(DateTime date)
    {
        var localDate = DateHelper.LocaliseDate(date);
        var npYear = -1;
        var lowerLimit =
            DateTime.ParseExact(YearStart.Dates.Values.Min(), "yyyy-MM-dd", CultureInfo.InvariantCulture);

        foreach (var key in YearStart.Dates.Keys.OrderBy(k => k))
        {
            var yearStart = DateTime.ParseExact(YearStart.Dates[key], "yyyy-MM-dd", CultureInfo.InvariantCulture);
            if (DateTime.ParseExact(localDate, "yyyy-MM-dd", CultureInfo.InvariantCulture) >= lowerLimit &&
                DateTime.ParseExact(localDate, "yyyy-MM-dd", CultureInfo.InvariantCulture) < yearStart)
            {
                npYear = key - 1;
                return npYear;
            }
        }

        // Check if date falls in the last supported year
        var lastYear = YearStart.Dates.Keys.Max();
        var lastYearStart =
            DateTime.ParseExact(YearStart.Dates[lastYear], "yyyy-MM-dd", CultureInfo.InvariantCulture);

        if (DateTime.ParseExact(localDate, "yyyy-MM-dd", CultureInfo.InvariantCulture) >= lastYearStart)
        {
            // Check if the date is within the limits of the last year
            var nextYearApproximateStart = lastYearStart.AddDays(365);
            if (DateTime.ParseExact(localDate, "yyyy-MM-dd", CultureInfo.InvariantCulture) <
                nextYearApproximateStart)
                return lastYear;
        }

        return npYear;
    }

    internal static (int Month, int Day) GetMonthAndDate(int year, DateTime date)
    {
        var daysPassed = GetDaysPassed(year, date);
        var monthIndex = -1;
        var dayOfMonth = -1;

        for (var i = 0; i < 12; i++)
        {
            if (daysPassed <= MonthLengths.Lengths[year][i])
            {
                monthIndex = i;
                dayOfMonth = daysPassed;
                break;
            }

            daysPassed -= MonthLengths.Lengths[year][i];
        }

        return (monthIndex + 1, dayOfMonth);
    }

    internal static int GetDaysPassed(int year, DateTime date)
    {
        var localDate = DateHelper.LocaliseDate(date);
        var givenDate = DateTime.ParseExact(localDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        var startDate = DateTime.ParseExact(YearStart.Dates[year], "yyyy-MM-dd", CultureInfo.InvariantCulture);

        var daysPassed = (givenDate - startDate).TotalDays;
        return (int)Math.Round(daysPassed + 1);
    }

    internal static bool IsValidNepaliDate(NepaliDate date)
    {
        if (!MonthLengths.Lengths.ContainsKey(date.Year))
            return false;

        if (date.Month < 1 || date.Month > 12)
            return false;

        if (date.Day < 1 || date.Day > MonthLengths.Lengths[date.Year][date.Month - 1])
            return false;

        return true;
    }

    internal static bool IsValidNepaliMonth(int year, int month)
    {
        if (!MonthLengths.Lengths.ContainsKey(year))
            return false;

        if (month < 1 || month > 12)
            return false;

        return true;
    }
}