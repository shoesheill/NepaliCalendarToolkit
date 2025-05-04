using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NepaliCalendarToolkit;

public static class NepaliCalendarConverter
{
    /// <summary>
    ///     Configure which days of the week are considered weekends
    /// </summary>
    /// <param name="weekendDays">Array of days to be considered as weekends</param>
    public static void ConfigureWeekendDays(params DayOfWeek[] weekendDays)
    {
        WeekendConfiguration.SetWeekendDays(weekendDays);
    }

    /// <summary>
    ///     Gets the currently configured weekend days
    /// </summary>
    /// <returns>Array of days that are configured as weekends</returns>
    public static DayOfWeek[] GetConfiguredWeekendDays()
    {
        return WeekendConfiguration.GetWeekendDays();
    }

    public static NepaliDate ConvertToNepali(DateTime ad)
    {
        var npYear = CalendarUtilities.GetYear(ad);
        if (npYear == -1) throw new ArgumentOutOfRangeException(nameof(ad), "Date is outside the supported range.");

        // Check if year is in the supported range before proceeding
        if (!MonthLengths.Lengths.ContainsKey(npYear))
        {
            var supportedYears = string.Join(", ", MonthLengths.Lengths.Keys.OrderBy(k => k));
            throw new ArgumentOutOfRangeException(
                nameof(ad),
                $"Year {npYear} is outside the supported range. Supported years are: {supportedYears}");
        }

        var npMonthAndDate = CalendarUtilities.GetMonthAndDate(npYear, ad);
        return new NepaliDate(npYear, npMonthAndDate.Month, npMonthAndDate.Day);
    }

    public static DateTime ConvertToAd(NepaliDate nepaliDate)
    {
        // Check if year is in the supported range
        if (!MonthLengths.Lengths.ContainsKey(nepaliDate.GetYear))
        {
            var supportedYears = string.Join(", ", MonthLengths.Lengths.Keys.OrderBy(k => k));
            throw new ArgumentOutOfRangeException(
                nameof(nepaliDate),
                $"Year {nepaliDate.GetYear} is outside the supported range. Supported years are: {supportedYears}");
        }

        if (!CalendarUtilities.IsValidNepaliDate(nepaliDate))
            throw new ArgumentException("Invalid Nepali date");

        // Get the start date of the Nepali year
        if (!YearStart.Dates.ContainsKey(nepaliDate.GetYear))
            throw new ArgumentOutOfRangeException(
                nameof(nepaliDate),
                $"Year {nepaliDate.GetYear} is outside the supported range");

        var startDate = DateTime.ParseExact(YearStart.Dates[nepaliDate.GetYear], "yyyy-MM-dd",
            CultureInfo.InvariantCulture);

        // Calculate days to add based on months and days
        var daysToAdd = 0;

        // Add days for full months
        for (var i = 0; i < nepaliDate.GetMonth - 1; i++) daysToAdd += MonthLengths.Lengths[nepaliDate.GetYear][i];

        // Add remaining days
        daysToAdd += nepaliDate.GetDay - 1;

        return startDate.AddDays(daysToAdd);
    }

    public static (string StartDate, string EndDate) GetMonthDateInAd(int yearBs, int monthBs)
    {
        if (!CalendarUtilities.IsValidNepaliMonth(yearBs, monthBs))
            throw new ArgumentException("Invalid year or month");

        var startNepaliDate = new NepaliDate(yearBs, monthBs, 1);
        var endNepaliDate = new NepaliDate(yearBs, monthBs, MonthLengths.Lengths[yearBs][monthBs - 1]);
        var startDate = ConvertToAd(startNepaliDate);
        var endDate = ConvertToAd(endNepaliDate);
        return (startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
    }

    /// <summary>
    ///     Gets the date range for a specific week of a Nepali month in AD format
    /// </summary>
    /// <param name="yearBs">Nepali year in BS</param>
    /// <param name="monthBs">Nepali month (1-12)</param>
    /// <param name="weekNumber">Week number (1-5, where 1 is the first week of the month)</param>
    /// <returns>Tuple containing start date and end date of the week in AD format (yyyy-MM-dd)</returns>
    public static (string StartDate, string EndDate) GetWeekDateInAd(int yearBs, int monthBs, int weekNumber)
    {
        // Validate inputs
        if (!CalendarUtilities.IsValidNepaliMonth(yearBs, monthBs))
            throw new ArgumentException("Invalid year or month");

        if (weekNumber < 1 || weekNumber > 5)
            throw new ArgumentException("Week number must be between 1 and 5");

        // Get the first day of the month
        var firstDayOfMonth = new NepaliDate(yearBs, monthBs, 1);
        var firstDayOfMonthAd = ConvertToAd(firstDayOfMonth);

        // Get the month length
        var monthLength = MonthLengths.Lengths[yearBs][monthBs - 1];

        // Calculate first day of the week
        // Week 1 is days 1-7, Week 2 is days 8-14, etc.
        var firstDayOfWeek = (weekNumber - 1) * 7 + 1;

        // If the first day of week is past the month length, return empty dates
        if (firstDayOfWeek > monthLength)
            return ("", "");

        // Calculate the last day of the week (either 7 days later or the end of the month)
        var lastDayOfWeek = Math.Min(firstDayOfWeek + 6, monthLength);

        // Create Nepali dates for start and end of the week
        var startNepaliDate = new NepaliDate(yearBs, monthBs, firstDayOfWeek);
        var endNepaliDate = new NepaliDate(yearBs, monthBs, lastDayOfWeek);

        // Convert to AD dates
        var startDate = ConvertToAd(startNepaliDate);
        var endDate = ConvertToAd(endNepaliDate);

        return (startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
    }

    public static (string StartDate, string EndDate) GetMonthRangeDateInAd(int yearBs, int startMonth,
        int endMonth)
    {
        if (!CalendarUtilities.IsValidNepaliMonth(yearBs, startMonth) ||
            !CalendarUtilities.IsValidNepaliMonth(yearBs, endMonth))
            throw new ArgumentException("Invalid year or month");

        if (startMonth > endMonth)
            throw new ArgumentException("Start month cannot be greater than end month");

        var startNepaliDate = new NepaliDate(yearBs, startMonth, 1);
        var endNepaliDate = new NepaliDate(yearBs, endMonth, MonthLengths.Lengths[yearBs][endMonth - 1]);
        var startDate = ConvertToAd(startNepaliDate);
        var endDate = ConvertToAd(endNepaliDate);
        return (startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
    }

    public static (string StartDate, string EndDate) GetQuarterDateRangeInAd(int yearBs, int quarter)
    {
        if (quarter < 1 || quarter > 4)
            throw new ArgumentException("Quarter must be between 1 and 4");

        int startMonth, endMonth;

        switch (quarter)
        {
            case 1: // Shrawan, Bhadra, Aswin
                startMonth = 4;
                endMonth = 6;
                break;
            case 2: // Kartik, Mangsir, Push
                startMonth = 7;
                endMonth = 9;
                break;
            case 3: // Magh, Falgun, Chaitra
                startMonth = 10;
                endMonth = 12;
                break;
            case 4: // Baisakh, Jest, Ashar
                startMonth = 1;
                endMonth = 3;
                break;
            default:
                throw new ArgumentException("Invalid quarter");
        }

        var startNepaliDate = new NepaliDate(yearBs, startMonth, 1);
        var endNepaliDate = new NepaliDate(yearBs, endMonth, MonthLengths.Lengths[yearBs][endMonth - 1]);
        var startDate = ConvertToAd(startNepaliDate);
        var endDate = ConvertToAd(endNepaliDate);
        return (startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
    }


    /// <summary>
    ///     Gets the date range for a Nepali fiscal year in AD format
    /// </summary>
    /// <param name="fiscalYear">Starting year of the fiscal year (e.g., for fiscal year 2062-63, use 2062)</param>
    /// <returns>Tuple containing start date and end date of the fiscal year in AD format (yyyy-MM-dd)</returns>
    public static (string StartDate, string EndDate) GetFiscalYearDateRangeInAd(int fiscalYear)
    {
        // Validate input
        if (!MonthLengths.Lengths.ContainsKey(fiscalYear) || !MonthLengths.Lengths.ContainsKey(fiscalYear + 1))
        {
            var supportedYears = string.Join(", ", MonthLengths.Lengths.Keys.OrderBy(k => k));
            throw new ArgumentException(
                $"Fiscal year {fiscalYear}-{(fiscalYear + 1) % 100} is outside the supported range. Supported years are: {supportedYears}");
        }

        // In Nepal, fiscal year starts from 1st of Shrawan (month 4) and ends on last day of Ashar (month 3) of the next year
        var startNepaliDate = new NepaliDate(fiscalYear, 4, 1); // 1st Shrawan
        var endNepaliDate =
            new NepaliDate(fiscalYear + 1, 3,
                MonthLengths.Lengths[fiscalYear + 1][3 - 1]); // Last day of Ashar of next year

        var startDate = ConvertToAd(startNepaliDate);
        var endDate = ConvertToAd(endNepaliDate);

        return (startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
    }

    public static List<HolidayInfo> GetHolidaysAndWeekends(int year, int? month = null,
        HolidayOrWeekendEnum returnType = HolidayOrWeekendEnum.Both)
    {
        return HolidayHelper.GetHolidaysAndWeekends(year, month, returnType);
    }


    /// <summary>
    ///     Gets holidays and weekends for a specific fiscal year
    /// </summary>
    /// <param name="fiscalYear">Starting year of the fiscal year (e.g., for fiscal year 2062-63, use 2062)</param>
    /// <param name="returnType">Type of days to return (holidays, weekends, or both)</param>
    /// <returns>List of holidays and/or weekends for the specified fiscal year</returns>
    public static List<HolidayInfo> GetHolidaysAndWeekendsForFiscalYear(int fiscalYear,
        HolidayOrWeekendEnum returnType = HolidayOrWeekendEnum.Both)
    {
        // Validate input
        if (!MonthLengths.Lengths.ContainsKey(fiscalYear) || !MonthLengths.Lengths.ContainsKey(fiscalYear + 1))
        {
            var supportedYears = string.Join(", ", MonthLengths.Lengths.Keys.OrderBy(k => k));
            throw new ArgumentException(
                $"Fiscal year {fiscalYear}-{(fiscalYear + 1) % 100} is outside the supported range. Supported years are: {supportedYears}");
        }

        // In Nepal, fiscal year starts from 1st of Shrawan (month 4) and ends on last day of Ashar (month 3) of the next year
        return HolidayHelper.GetHolidaysAndWeekends(
            fiscalYear, 4, 1,
            fiscalYear + 1, 3, MonthLengths.Lengths[fiscalYear + 1][3 - 1],
            returnType);
    }

    /// <summary>
    ///     Gets the available Bikram Sambat (BS) year range for which holiday data is available
    /// </summary>
    /// <returns>A tuple containing the minimum and maximum supported BS years for holidays</returns>
    public static (int MinYear, int MaxYear) GetAvailableHolidayYearsBs()
    {
        var years = HolidayJson.GetAvailableYears();

        if (years.Count == 0)
            return (0, 0);

        return (years.Min(), years.Max());
    }

    /// <summary>
    ///     Gets the available Bikram Sambat (BS) year range supported by the calendar converter
    /// </summary>
    /// <returns>A tuple containing the minimum and maximum supported BS years</returns>
    public static (int MinYear, int MaxYear) GetAvailableCalendarYearsBs()
    {
        var years = MonthLengths.Lengths.Keys.ToList();

        if (years.Count == 0)
            return (0, 0);

        return (years.Min(), years.Max());
    }
}