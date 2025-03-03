using System;
using System.Collections.Generic;
using System.Globalization;

public static class NepaliCalendarConverter
{
    public static NepaliDate ConvertToNepali(DateTime ad)
    {
        var npYear = CalendarUtilities.GetYear(ad);
        if (npYear == -1) throw new ArgumentOutOfRangeException("Date is outside the supported range.");

        var npMonthAndDate = CalendarUtilities.GetMonthAndDate(npYear, ad);
        return new NepaliDate(npYear, npMonthAndDate.Month, npMonthAndDate.Day);
    }

    public static DateTime ConvertToAD(NepaliDate nepaliDate)
    {
        if (!CalendarUtilities.IsValidNepaliDate(nepaliDate))
            throw new ArgumentException("Invalid Nepali date");

        // Get the start date of the Nepali year
        if (!YearStart.Dates.ContainsKey(nepaliDate.GetYear))
            throw new ArgumentOutOfRangeException("Year is outside the supported range");

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

    public static (string StartDate, string EndDate) GetStartAndEndDateInAD(int yearBs, int monthBs)
    {
        if (!CalendarUtilities.IsValidNepaliMonth(yearBs, monthBs))
            throw new ArgumentException("Invalid year or month");

        var startNepaliDate = new NepaliDate(yearBs, monthBs, 1);
        var endNepaliDate = new NepaliDate(yearBs, monthBs, MonthLengths.Lengths[yearBs][monthBs - 1]);
        var startDate = ConvertToAD(startNepaliDate);
        var endDate = ConvertToAD(endNepaliDate);
        return (startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
    }

    public static (string StartDate, string EndDate) GetQuarterDateRangeInAD(int yearBs, int quarter)
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

    public static (string StartDate, string EndDate) GetMonthRangeDateInAD(int yearBs, int startMonth,
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

    public static List<HolidayInfo> GetHolidaysAndWeekends(int year, int? month = null,
        HolidayOrWeekendEnum returnType = HolidayOrWeekendEnum.Both)
    {
        return HolidayHelper.GetHolidaysAndWeekends(year, month, returnType);
    }
}