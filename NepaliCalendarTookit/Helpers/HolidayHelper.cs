using System;
using System.Collections.Generic;
using System.Linq;

public static class HolidayHelper
{
    internal static List<HolidayInfo> GetHolidaysAndWeekends(int year, int? month = null,
        HolidayOrWeekendEnum returnType = HolidayOrWeekendEnum.Both)
    {
        var results = new List<HolidayInfo>();
        var holidays = Holidays.HolidayDates;

        // Determine the year to check for holidays
        if (returnType == HolidayOrWeekendEnum.Holidays || returnType == HolidayOrWeekendEnum.Both)
            if (holidays.ContainsKey(year))
            {
                var holidayList = holidays[year];
                foreach (var holiday in holidayList)
                    if (month == null || holiday.Month == month.Value)
                    {
                        var nepaliDate = new NepaliDate
                            { Year = year, Month = holiday.Month, Day = holiday.Day };
                        var adDate = NepaliCalendarConverter.ConvertToAD(nepaliDate);
                        var dayName = adDate.DayOfWeek.ToString(); // Get the day name from the AD date
                        results.Add(new HolidayInfo(
                            dayName, // Use the calculated day name
                            holiday.Name,
                            adDate,
                            $"{nepaliDate.Year}-{nepaliDate.Month:D2}-{nepaliDate.Day:D2}" // Format BS date
                        ));
                    }
            }

        // Add weekends if requested
        if (returnType == HolidayOrWeekendEnum.Weekends || returnType == HolidayOrWeekendEnum.Both)
        {
            if (month.HasValue)
            {
                // Get the start and end date of the Nepali month in AD
                var (startDate, endDate) = NepaliCalendarConverter.GetStartAndEndDateInAD(year, month.Value);

                // Loop through all days in the month
                for (var date = startDate; date <= endDate; date = date.AddDays(1))
                    if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                    {
                        var nepaliDate = NepaliCalendarConverter.ConvertToNepali(date);
                        var weekendType = date.DayOfWeek == DayOfWeek.Saturday ? "Saturday" : "Sunday";

                        results.Add(new HolidayInfo(
                            weekendType,
                            string.Empty,
                            date,
                            $"{nepaliDate.Year}-{nepaliDate.Month:D2}-{nepaliDate.Day:D2}" // Format BS date
                        ));
                    }
            }
            else // Full year, all months
            {
                // Loop through all 12 months
                for (var m = 1; m <= 12; m++)
                {
                    var (startDate, endDate) = NepaliCalendarConverter.GetStartAndEndDateInAD(year, m);

                    for (var date = startDate; date <= endDate; date = date.AddDays(1))
                        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                        {
                            var nepaliDate = NepaliCalendarConverter.ConvertToNepali(date);
                            var weekendType = date.DayOfWeek == DayOfWeek.Saturday ? "Saturday" : "Sunday";
                            results.Add(new HolidayInfo(
                                weekendType,
                                string.Empty,
                                date,
                                $"{nepaliDate.Year}-{nepaliDate.Month:D2}-{nepaliDate.Day:D2}" // Format BS date
                            ));
                        }
                }
            }
        }

        // Sort results by date
        var sortedResults = results.OrderBy(r => r.AdDate).ToList();

        return sortedResults;
    }
}