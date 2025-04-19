using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NepaliCalendarToolkit.Data;

public static class NepaliCalendarConverter
{
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

    public static DateTime ConvertToAD(NepaliDate nepaliDate)
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

    public static (string StartDate, string EndDate) GetStartAndEndDateInAd(int yearBs, int monthBs)
    {
        if (!CalendarUtilities.IsValidNepaliMonth(yearBs, monthBs))
            throw new ArgumentException("Invalid year or month");

        var startNepaliDate = new NepaliDate(yearBs, monthBs, 1);
        var endNepaliDate = new NepaliDate(yearBs, monthBs, MonthLengths.Lengths[yearBs][monthBs - 1]);
        var startDate = ConvertToAD(startNepaliDate);
        var endDate = ConvertToAD(endNepaliDate);
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
        var startDate = ConvertToAD(startNepaliDate);
        var endDate = ConvertToAD(endNepaliDate);
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
        var startDate = ConvertToAD(startNepaliDate);
        var endDate = ConvertToAD(endNepaliDate);
        return (startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
    }

    
    /// <summary>
    /// Gets the date range for a Nepali fiscal year in AD format
    /// </summary>
    /// <param name="fiscalYear">Starting year of the fiscal year (e.g., for fiscal year 2062-63, use 2062)</param>
    /// <returns>Tuple containing start date and end date of the fiscal year in AD format (yyyy-MM-dd)</returns>
    public static (string StartDate, string EndDate) GetFiscalYearDateRangeInAd(int fiscalYear)
    {
        // Validate input
        if (!MonthLengths.Lengths.ContainsKey(fiscalYear) || !MonthLengths.Lengths.ContainsKey(fiscalYear + 1))
        {
            var supportedYears = string.Join(", ", MonthLengths.Lengths.Keys.OrderBy(k => k));
            throw new ArgumentException($"Fiscal year {fiscalYear}-{(fiscalYear + 1) % 100} is outside the supported range. Supported years are: {supportedYears}");
        }

        // In Nepal, fiscal year starts from 1st of Shrawan (month 4) and ends on last day of Ashar (month 3) of the next year
        var startNepaliDate = new NepaliDate(fiscalYear, 4, 1); // 1st Shrawan
        var endNepaliDate = new NepaliDate(fiscalYear + 1, 3, MonthLengths.Lengths[fiscalYear + 1][3 - 1]); // Last day of Ashar of next year

        var startDate = ConvertToAD(startNepaliDate);
        var endDate = ConvertToAD(endNepaliDate);

        return (startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
    }

    public static List<HolidayInfo> GetHolidaysAndWeekends(int year, int? month = null,
        HolidayOrWeekendEnum returnType = HolidayOrWeekendEnum.Both)
    {
        return HolidayHelper.GetHolidaysAndWeekends(year, month, returnType);
    }

    

    /// <summary>
    /// Gets holidays and weekends for a specific fiscal year
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
            throw new ArgumentException($"Fiscal year {fiscalYear}-{(fiscalYear + 1) % 100} is outside the supported range. Supported years are: {supportedYears}");
        }

        // In Nepal, fiscal year starts from 1st of Shrawan (month 4) and ends on last day of Ashar (month 3) of the next year
        return HolidayHelper.GetHolidaysAndWeekends(
            fiscalYear, 4, 1,
            fiscalYear + 1, 3, MonthLengths.Lengths[fiscalYear + 1][3 - 1],
            returnType);
    }
}