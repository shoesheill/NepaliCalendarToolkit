using System;

namespace NepaliCalendarToolkit
{
    /// <summary>
    ///     Class representing comprehensive information about a date in the Nepali calendar
    /// </summary>
    public class CurrentDateInfo
    {
        /// <summary>
        ///     The Nepali date object
        /// </summary>
        public NepaliDate Date { get; set; }

        /// <summary>
        ///     The equivalent AD date
        /// </summary>
        public DateTime AdDate { get; set; }

        /// <summary>
        ///     The year in Bikram Sambat (BS)
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        ///     The month (1-12)
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        ///     The day of the month
        /// </summary>
        public int Day { get; set; }

        /// <summary>
        ///     The quarter (1-4)
        /// </summary>
        public int Quarter { get; set; }

        /// <summary>
        ///     The week of the month (1-5)
        /// </summary>
        public int WeekOfMonth { get; set; }

        /// <summary>
        ///     The fiscal year (relevant when using fiscal year context)
        /// </summary>
        public int FiscalYear { get; set; }

        /// <summary>
        ///     The day of the week
        /// </summary>
        public DayOfWeek DayOfWeek { get; set; }

        /// <summary>
        ///     Whether this date is a weekend
        /// </summary>
        public bool IsWeekend { get; set; }

        /// <summary>
        ///     Gets the name of the Nepali month
        /// </summary>
        public string MonthName => GetNepaliMonthName(Month);

        /// <summary>
        ///     Gets the name of the Nepali month in Nepali language
        /// </summary>
        public string NepaliMonthName => GetNepaliMonthNameInNepali(Month);

        /// <summary>
        ///     Gets the Nepali month name in English
        /// </summary>
        /// <param name="month">Month number (1-12)</param>
        /// <returns>English name of the Nepali month</returns>
        private string GetNepaliMonthName(int month)
        {
            switch (month)
            {
                case 1: return "Baisakh";
                case 2: return "Jestha";
                case 3: return "Ashadh";
                case 4: return "Shrawan";
                case 5: return "Bhadra";
                case 6: return "Ashwin";
                case 7: return "Kartik";
                case 8: return "Mangsir";
                case 9: return "Poush";
                case 10: return "Magh";
                case 11: return "Falgun";
                case 12: return "Chaitra";
                default: throw new ArgumentOutOfRangeException(nameof(month), "Month must be between 1 and 12");
            }
        }

        /// <summary>
        ///     Gets the Nepali month name in Nepali language
        /// </summary>
        /// <param name="month">Month number (1-12)</param>
        /// <returns>Nepali name of the month in Nepali script</returns>
        private string GetNepaliMonthNameInNepali(int month)
        {
            switch (month)
            {
                case 1: return "बैशाख";
                case 2: return "जेठ";
                case 3: return "असार";
                case 4: return "श्रावण";
                case 5: return "भाद्र";
                case 6: return "आश्विन";
                case 7: return "कार्तिक";
                case 8: return "मंसिर";
                case 9: return "पौष";
                case 10: return "माघ";
                case 11: return "फाल्गुन";
                case 12: return "चैत्र";
                default: throw new ArgumentOutOfRangeException(nameof(month), "Month must be between 1 and 12");
            }
        }

        /// <summary>
        ///     Returns a comprehensive string representation of the date information
        /// </summary>
        public override string ToString()
        {
            return $"{Date} (AD: {AdDate:yyyy-MM-dd}), " +
                   $"BS Year: {Year}, Month: {Month} ({MonthName}), Day: {Day}, " +
                   $"Quarter: {Quarter}, Week: {WeekOfMonth}, " +
                   $"Fiscal Year: {FiscalYear}, {DayOfWeek}" +
                   (IsWeekend ? " (Weekend)" : "");
        }
    }
}