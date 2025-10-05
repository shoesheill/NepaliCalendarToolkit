using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NepaliCalendarToolkit.Enum;
using NepaliCalendarToolkit.Helpers;
using NepaliCalendarToolkit.Models;
using NepaliCalendarToolkit.Utilities;

namespace NepaliCalendarToolkit.Converters
{
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

        /// <summary>
        ///     Gets the date range for a specific month in AD format
        /// </summary>
        /// <param name="yearBs">Nepali year in BS</param>
        /// <param name="monthBs">Nepali month (1-12)</param>
        /// <param name="isFiscalYear">Whether to interpret the year as a fiscal year</param>
        /// <returns>Tuple containing start date and end date of the month in AD format (yyyy-MM-dd)</returns>
        public static (string StartDate, string EndDate) GetMonthDateInAd(int yearBs, int monthBs,
            bool isFiscalYear = false)
        {
            if (monthBs < 1 || monthBs > 12)
                throw new ArgumentException("Month must be between 1 and 12");

            // If fiscal year, adjust the year based on month
            if (isFiscalYear)
                // For fiscal year, months 4-12 (Shrawan to Chaitra) are in the first year,
                // months 1-3 (Baisakh to Ashar) are in the second year
                yearBs = monthBs >= 4 ? yearBs : yearBs + 1;

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
        /// <param name="isFiscalYear">Whether to interpret the year as a fiscal year</param>
        /// <returns>Tuple containing start date and end date of the week in AD format (yyyy-MM-dd)</returns>
        public static (string StartDate, string EndDate) GetWeekDateInAd(int yearBs, int monthBs, int weekNumber,
            bool isFiscalYear = false)
        {
            // Validate inputs
            if (monthBs < 1 || monthBs > 12)
                throw new ArgumentException("Month must be between 1 and 12");

            if (weekNumber < 1 || weekNumber > 5)
                throw new ArgumentException("Week number must be between 1 and 5");

            // If fiscal year, adjust the year based on month
            if (isFiscalYear) yearBs = monthBs >= 4 ? yearBs : yearBs + 1;

            // Now validate the year and month combination
            if (!CalendarUtilities.IsValidNepaliMonth(yearBs, monthBs))
                throw new ArgumentException("Invalid year or month");

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

        /// <summary>
        ///     Gets the date range for a specific range of months in AD format
        /// </summary>
        /// <param name="yearBs">Nepali year in BS</param>
        /// <param name="startMonth">Starting Nepali month (1-12)</param>
        /// <param name="endMonth">Ending Nepali month (1-12)</param>
        /// <param name="isFiscalYear">Whether to interpret the year as a fiscal year</param>
        /// <returns>Tuple containing start date and end date of the month range in AD format (yyyy-MM-dd)</returns>
        public static (string StartDate, string EndDate) GetMonthRangeDateInAd(int yearBs, int startMonth,
            int endMonth, bool isFiscalYear = false)
        {
            if (startMonth < 1 || startMonth > 12 || endMonth < 1 || endMonth > 12)
                throw new ArgumentException("Month must be between 1 and 12");

            if (startMonth > endMonth)
                throw new ArgumentException("Start month cannot be greater than end month");

            var startYearBs = yearBs;
            var endYearBs = yearBs;

            if (isFiscalYear)
            {
                // When dealing with fiscal years, we need to ensure that months map to appropriate BS years
                startYearBs = startMonth >= 4 ? yearBs : yearBs + 1;
                endYearBs = endMonth >= 4 ? yearBs : yearBs + 1;

                // Validate that the range is within a single fiscal year
                if ((startMonth < 4 && endMonth >= 4) || (startMonth >= 4 && endMonth < 4 && endMonth > 0))
                    throw new ArgumentException("Month range must be within the same fiscal year");
            }

            // Now validate the year and month combinations
            if (!CalendarUtilities.IsValidNepaliMonth(startYearBs, startMonth) ||
                !CalendarUtilities.IsValidNepaliMonth(endYearBs, endMonth))
                throw new ArgumentException("Invalid year or month");

            var startNepaliDate = new NepaliDate(startYearBs, startMonth, 1);
            var endNepaliDate = new NepaliDate(endYearBs, endMonth, MonthLengths.Lengths[endYearBs][endMonth - 1]);
            var startDate = ConvertToAd(startNepaliDate);
            var endDate = ConvertToAd(endNepaliDate);
            return (startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
        }

        /// <summary>
        ///     Gets the date range for a quarter in AD format
        /// </summary>
        /// <param name="yearBs">Nepali year in BS</param>
        /// <param name="quarter">Quarter number (1-4)</param>
        /// <param name="isFiscalYear">Whether to interpret the year as a fiscal year</param>
        /// <returns>Tuple containing start date and end date of the quarter in AD format (yyyy-MM-dd)</returns>
        public static (string StartDate, string EndDate) GetQuarterDateRangeInAd(int yearBs, int quarter,
            bool isFiscalYear = false)
        {
            if (quarter < 1 || quarter > 4)
                throw new ArgumentException("Quarter must be between 1 and 4");

            int startMonth, endMonth;
            var calculatedYearBs = yearBs;

            if (isFiscalYear)
                // Fiscal year quarters
                switch (quarter)
                {
                    case 1: // First quarter: Shrawan, Bhadra, Aswin (4-6) of fiscal year
                        startMonth = 4;
                        endMonth = 6;
                        calculatedYearBs = yearBs;
                        break;
                    case 2: // Second quarter: Kartik, Mangsir, Push (7-9) of fiscal year
                        startMonth = 7;
                        endMonth = 9;
                        calculatedYearBs = yearBs;
                        break;
                    case 3: // Third quarter: Magh, Falgun, Chaitra (10-12) of fiscal year
                        startMonth = 10;
                        endMonth = 12;
                        calculatedYearBs = yearBs;
                        break;
                    case 4: // Fourth quarter: Baisakh, Jest, Ashar (1-3) of next year
                        startMonth = 1;
                        endMonth = 3;
                        calculatedYearBs = yearBs + 1; // Next year for 4th quarter
                        break;
                    default:
                        throw new ArgumentException("Invalid quarter");
                }
            else
                // Regular year quarters
                switch (quarter)
                {
                    case 1: // Baisakh, Jest, Ashar
                        startMonth = 1;
                        endMonth = 3;
                        break;
                    case 2: // Shrawan, Bhadra, Aswin
                        startMonth = 4;
                        endMonth = 6;
                        break;
                    case 3: // Kartik, Mangsir, Push
                        startMonth = 7;
                        endMonth = 9;
                        break;
                    case 4: // Magh, Falgun, Chaitra
                        startMonth = 10;
                        endMonth = 12;
                        break;
                    default:
                        throw new ArgumentException("Invalid quarter");
                }

            var startNepaliDate = new NepaliDate(calculatedYearBs, startMonth, 1);
            var endNepaliDate =
                new NepaliDate(calculatedYearBs, endMonth, MonthLengths.Lengths[calculatedYearBs][endMonth - 1]);
            var startDate = ConvertToAd(startNepaliDate);
            var endDate = ConvertToAd(endNepaliDate);
            return (startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
        }

        /// <summary>
        ///     Gets the date range for a Nepali year or fiscal year in AD format
        /// </summary>
        /// <param name="yearBs">Nepali year in BS</param>
        /// <param name="isFiscalYear">Whether to interpret the year as a fiscal year</param>
        /// <returns>Tuple containing start date and end date of the year in AD format (yyyy-MM-dd)</returns>
        public static (string StartDate, string EndDate) GetYearDateRangeInAd(int yearBs, bool isFiscalYear = false)
        {
            if (isFiscalYear)
            {
                // Validate input for fiscal year
                if (!MonthLengths.Lengths.ContainsKey(yearBs) || !MonthLengths.Lengths.ContainsKey(yearBs + 1))
                {
                    var supportedYears = string.Join(", ", MonthLengths.Lengths.Keys.OrderBy(k => k));
                    throw new ArgumentException(
                        $"Fiscal year {yearBs}-{(yearBs + 1) % 100} is outside the supported range. Supported years are: {supportedYears}");
                }

                // In Nepal, fiscal year starts from 1st of Shrawan (month 4) and ends on last day of Ashar (month 3) of the next year
                var startNepaliDate = new NepaliDate(yearBs, 4, 1); // 1st Shrawan
                var endNepaliDate =
                    new NepaliDate(yearBs + 1, 3,
                        MonthLengths.Lengths[yearBs + 1][3 - 1]); // Last day of Ashar of next year

                var startDate = ConvertToAd(startNepaliDate);
                var endDate = ConvertToAd(endNepaliDate);

                return (startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
            }
            else
            {
                // Validate input for regular year
                if (!MonthLengths.Lengths.ContainsKey(yearBs))
                {
                    var supportedYears = string.Join(", ", MonthLengths.Lengths.Keys.OrderBy(k => k));
                    throw new ArgumentException(
                        $"Year {yearBs} is outside the supported range. Supported years are: {supportedYears}");
                }

                // Regular Nepali year starts from 1st of Baisakh (month 1) and ends on last day of Chaitra (month 12)
                var startNepaliDate = new NepaliDate(yearBs, 1, 1); // 1st Baisakh
                var endNepaliDate =
                    new NepaliDate(yearBs, 12, MonthLengths.Lengths[yearBs][12 - 1]); // Last day of Chaitra

                var startDate = ConvertToAd(startNepaliDate);
                var endDate = ConvertToAd(endNepaliDate);

                return (startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
            }
        }

        /// <summary>
        ///     Gets holidays and weekends for a specific year or fiscal year
        /// </summary>
        /// <param name="yearBs">Nepali year in BS</param>
        /// <param name="month">Optional month to filter (1-12)</param>
        /// <param name="returnType">Type of days to return (holidays, weekends, or both)</param>
        /// <param name="isFiscalYear">Whether to interpret the year as a fiscal year</param>
        /// <returns>List of holidays and/or weekends for the specified year/month</returns>
        public static List<HolidayInfo> GetHolidaysAndWeekends(int yearBs, int? month = null,
            HolidayOrWeekendEnum returnType = HolidayOrWeekendEnum.Both, bool isFiscalYear = false)
        {
            if (isFiscalYear)
            {
                // Validate input for fiscal year
                if (!MonthLengths.Lengths.ContainsKey(yearBs) || !MonthLengths.Lengths.ContainsKey(yearBs + 1))
                {
                    var supportedYears = string.Join(", ", MonthLengths.Lengths.Keys.OrderBy(k => k));
                    throw new ArgumentException(
                        $"Fiscal year {yearBs}-{(yearBs + 1) % 100} is outside the supported range. Supported years are: {supportedYears}");
                }

                if (month.HasValue)
                {
                    // For a specific month in a fiscal year
                    if (month.Value < 1 || month.Value > 12)
                        throw new ArgumentException("Month must be between 1 and 12");

                    // Determine which year the month belongs to in the fiscal year
                    var actualYear = month.Value >= 4 ? yearBs : yearBs + 1;
                    return HolidayHelper.GetHolidaysAndWeekends(actualYear, month, returnType);
                }

                // For the entire fiscal year
                return HolidayHelper.GetHolidaysAndWeekends(
                    yearBs, 4, 1,
                    yearBs + 1, 3, MonthLengths.Lengths[yearBs + 1][3 - 1],
                    returnType);
            }

            // For regular year
            return HolidayHelper.GetHolidaysAndWeekends(yearBs, month, returnType);
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

        /// <summary>
        ///     Gets comprehensive date information for the current date
        /// </summary>
        /// <param name="isFiscalYear">Whether to interpret dates in fiscal year context</param>
        /// <returns>Object containing current date information in Nepali calendar</returns>
        public static CurrentDateInfo GetCurrentDateInfo(bool isFiscalYear = false)
        {
            // Get current date in AD
            var today = DateTime.Today;

            // Convert to Nepali date
            var nepaliDate = ConvertToNepali(today);
            var year = nepaliDate.GetYear;
            var month = nepaliDate.GetMonth;
            var day = nepaliDate.GetDay;

            // Calculate week of month (1-based)
            var weekOfMonth = (day - 1) / 7 + 1;

            // Calculate quarter
            int quarter;
            if (isFiscalYear)
                // Fiscal year quarters
                // Q1: Shrawan-Aswin (4-6)
                // Q2: Kartik-Push (7-9)
                // Q3: Magh-Chaitra (10-12)
                // Q4: Baisakh-Ashar (1-3)
                switch (month)
                {
                    case int m when m >= 4 && m <= 6:
                        quarter = 1;
                        break;
                    case int m when m >= 7 && m <= 9:
                        quarter = 2;
                        break;
                    case int m when m >= 10 && m <= 12:
                        quarter = 3;
                        break;
                    case int m when m >= 1 && m <= 3:
                        quarter = 4;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            else
                // Regular year quarters
                // Q1: Baisakh-Ashar (1-3)
                // Q2: Shrawan-Aswin (4-6)
                // Q3: Kartik-Push (7-9)
                // Q4: Magh-Chaitra (10-12)
                quarter = (month - 1) / 3 + 1;

            // Calculate fiscal year if needed
            var fiscalYear = year;
            if (isFiscalYear)
                // If we're in months 1-3, we're in the second year of the fiscal year
                fiscalYear = month >= 1 && month <= 3 ? year - 1 : year;

            return new CurrentDateInfo
            {
                Date = nepaliDate,
                AdDate = today,
                Year = year,
                Month = month,
                Day = day,
                Quarter = quarter,
                WeekOfMonth = weekOfMonth,
                FiscalYear = fiscalYear,
                DayOfWeek = today.DayOfWeek,
                IsWeekend = WeekendConfiguration.IsWeekend(today.DayOfWeek)
            };
        }
    }
}