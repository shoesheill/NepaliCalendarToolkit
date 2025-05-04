using System;
using NepaliCalendarToolkit;

public static class DateHelper
{
    public static string LocaliseDate(DateTime date)
    {
        var kathmanduTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Kathmandu");
        var localDate = TimeZoneInfo.ConvertTime(date, kathmanduTimeZone);
        return localDate.ToString("yyyy-MM-dd");
    }

    public static bool IsWeekend(DateTime date)
    {
        return WeekendConfiguration.IsWeekend(date.DayOfWeek);
    }
}