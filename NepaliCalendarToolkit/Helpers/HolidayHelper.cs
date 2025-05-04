using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NepaliCalendarToolkit;

public static class HolidayHelper
{
    internal static List<HolidayInfo> GetHolidaysAndWeekends(int year, int? month = null,
        HolidayOrWeekendEnum returnType = HolidayOrWeekendEnum.Both)
    {
        // Validate that the year exists in MonthLengths
        if (!MonthLengths.Lengths.ContainsKey(year))
            throw new ArgumentException(
                $"Year {year} is not supported. Supported years are: {string.Join(", ", MonthLengths.Lengths.Keys.OrderBy(k => k))}");

        var results = new List<HolidayInfo>();

        // Get holidays from JSON files if requested
        if (returnType == HolidayOrWeekendEnum.Holidays || returnType == HolidayOrWeekendEnum.Both)
        {
            // Load holidays from JSON file
            var holidayList = HolidayJson.GetHolidays(year);

            // If JSON file doesn't exist, generate it from in-memory data
            if (!holidayList.Any())
                // Generate JSON file for this year
                // HolidayJson.GenerateHolidayJsonFiles();
                // Try loading the JSON file again
                holidayList = HolidayJson.GetHolidays(year);

            // Log a warning if no holiday data is found for the year
            if (!holidayList.Any())
                // You might want to log this situation or handle it as appropriate
                Debug.WriteLine($"No holiday data found for year {year}");

            foreach (var holiday in holidayList)
                if (month == null || holiday.Month == month.Value)
                {
                    var nepaliDate = new NepaliDate(year, holiday.Month, holiday.Day);
                    var adDate = NepaliCalendarConverter.ConvertToAd(nepaliDate);
                    var dayName = adDate.DayOfWeek.ToString(); // Get the day name from the AD date

                    results.Add(new HolidayInfo(
                        dayName, // Use the calculated day name
                        holiday.Name,
                        adDate,
                        nepaliDate.ToString() // Format BS date
                    ));
                }
        }

        // Add weekends if requested
        if (returnType == HolidayOrWeekendEnum.Weekends || returnType == HolidayOrWeekendEnum.Both)
        {
            if (month.HasValue)
            {
                // Get the start and end date of the Nepali month in AD
                var (startDateString, endDateString) =
                    NepaliCalendarConverter.GetMonthDateInAd(year, month.Value);
                DateTime.TryParse(startDateString, out var startDate);
                DateTime.TryParse(endDateString, out var endDate);
                // Loop through all days in the month
                for (var date = startDate; date <= endDate; date = date.AddDays(1))
                    if (WeekendConfiguration.IsWeekend(date.DayOfWeek))
                    {
                        var nepaliDate = NepaliCalendarConverter.ConvertToNepali(date);
                        var weekendType = date.DayOfWeek.ToString();

                        results.Add(new HolidayInfo(
                            weekendType,
                            string.Empty,
                            date,
                            nepaliDate.ToString() // Format BS date
                        ));
                    }
            }
            else // Full year, all months
            {
                // Loop through all 12 months
                for (var m = 1; m <= 12; m++)
                {
                    var (startDateString, endDateString) = NepaliCalendarConverter.GetMonthDateInAd(year, m);
                    DateTime.TryParse(startDateString, out var startDate);
                    DateTime.TryParse(endDateString, out var endDate);
                    for (var date = startDate; date <= endDate; date = date.AddDays(1))
                        if (WeekendConfiguration.IsWeekend(date.DayOfWeek))
                        {
                            var nepaliDate = NepaliCalendarConverter.ConvertToNepali(date);
                            var weekendType = date.DayOfWeek.ToString();
                            results.Add(new HolidayInfo(
                                weekendType,
                                string.Empty,
                                date,
                                nepaliDate.ToString() // Format BS date
                            ));
                        }
                }
            }
        }

        // Sort results by date
        return results.OrderBy(r => r.GetAdDate).ToList();
    }

    /// <summary>
    ///     Gets holidays and weekends for a range of Nepali dates
    /// </summary>
    /// <param name="startYear">Start year in BS</param>
    /// <param name="startMonth">Start month in BS</param>
    /// <param name="startDay">Start day in BS</param>
    /// <param name="endYear">End year in BS</param>
    /// <param name="endMonth">End month in BS</param>
    /// <param name="endDay">End day in BS</param>
    /// <param name="returnType">Type of days to return (holidays, weekends, or both)</param>
    /// <returns>List of holidays and/or weekends in the specified date range</returns>
    internal static List<HolidayInfo> GetHolidaysAndWeekends(
        int startYear, int startMonth, int startDay,
        int endYear, int endMonth, int endDay,
        HolidayOrWeekendEnum returnType = HolidayOrWeekendEnum.Both)
    {
        // Validate inputs
        if (!MonthLengths.Lengths.ContainsKey(startYear) || !MonthLengths.Lengths.ContainsKey(endYear))
        {
            var supportedYears = string.Join(", ", MonthLengths.Lengths.Keys.OrderBy(k => k));
            throw new ArgumentException($"One of the years is not supported. Supported years are: {supportedYears}");
        }

        var results = new List<HolidayInfo>();
        var startDate = new NepaliDate(startYear, startMonth, startDay);
        var endDate = new NepaliDate(endYear, endMonth, endDay);

        // Convert Nepali dates to AD for iterating
        var startDateAD = NepaliCalendarConverter.ConvertToAd(startDate);
        var endDateAD = NepaliCalendarConverter.ConvertToAd(endDate);

        // Iterate through each day in the range
        for (var date = startDateAD; date <= endDateAD; date = date.AddDays(1))
        {
            var nepaliDate = NepaliCalendarConverter.ConvertToNepali(date);

            // Skip if the year isn't supported
            if (!MonthLengths.Lengths.ContainsKey(nepaliDate.GetYear))
                continue;

            // Add holidays if requested
            if (returnType == HolidayOrWeekendEnum.Holidays || returnType == HolidayOrWeekendEnum.Both)
            {
                var holidayList = HolidayJson.GetHolidays(nepaliDate.GetYear);
                var holiday =
                    holidayList.FirstOrDefault(h => h.Month == nepaliDate.GetMonth && h.Day == nepaliDate.GetDay);

                if (holiday != null)
                    results.Add(new HolidayInfo(
                        date.DayOfWeek.ToString(),
                        holiday.Name,
                        date,
                        nepaliDate.ToString()
                    ));
            }

            // Add weekends if requested
            if ((returnType == HolidayOrWeekendEnum.Weekends || returnType == HolidayOrWeekendEnum.Both) &&
                WeekendConfiguration.IsWeekend(date.DayOfWeek))
            {
                var weekendType = date.DayOfWeek.ToString();

                // Check if we already added this date as a holiday
                if (!results.Any(r => r.GetAdDate == date))
                    results.Add(new HolidayInfo(
                        weekendType,
                        string.Empty,
                        date,
                        nepaliDate.ToString()
                    ));
            }
        }

        // Sort results by date
        return results.OrderBy(r => r.GetAdDate).ToList();
    }
}