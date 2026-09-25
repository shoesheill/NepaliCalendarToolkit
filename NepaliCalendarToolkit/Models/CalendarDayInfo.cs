using System;

namespace NepaliCalendarToolkit.Models
{
    /// <summary>
    ///     Daily Nepali calendar details sourced from dateConvert and enriched with local
    ///     names for tithi, lunar fortnight/month and Nepal Sambat month.
    /// </summary>
    public class CalendarDayInfo
    {
        public CalendarDayInfo(
            DateTime adDate,
            NepaliDate bsDate,
            string bsMonthName,
            string bsMonthNameEn,
            DayOfWeek dayOfWeek,
            int? tithiNumber,
            string tithiName,
            string tithiNameEn,
            int? chandramaNumber,
            string lunarMonthName,
            string lunarMonthNameEn,
            string paksha,
            string pakshaEn,
            string nepaliSambatMonthCode,
            string nepaliSambatMonthName,
            int? nepaliSambatYear,
            bool isVerified)
        {
            AdDate = adDate;
            BsDate = bsDate;
            BsMonthName = bsMonthName;
            BsMonthNameEn = bsMonthNameEn;
            DayOfWeek = dayOfWeek;
            DayName = dayOfWeek.ToString();
            TithiNumber = tithiNumber;
            TithiName = tithiName;
            TithiNameEn = tithiNameEn;
            ChandramaNumber = chandramaNumber;
            LunarMonthName = lunarMonthName;
            LunarMonthNameEn = lunarMonthNameEn;
            Paksha = paksha;
            PakshaEn = pakshaEn;
            NepaliSambatMonthCode = nepaliSambatMonthCode;
            NepaliSambatMonthName = nepaliSambatMonthName;
            NepaliSambatYear = nepaliSambatYear;
            IsVerified = isVerified;
        }

        public DateTime AdDate { get; }
        public NepaliDate BsDate { get; }
        public string BsMonthName { get; }
        public string BsMonthNameEn { get; }
        public DayOfWeek DayOfWeek { get; }
        public string DayName { get; }
        public int? TithiNumber { get; }
        public string TithiName { get; }
        public string TithiNameEn { get; }
        public int? ChandramaNumber { get; }
        public string LunarMonthName { get; }
        public string LunarMonthNameEn { get; }
        public string Paksha { get; }
        public string PakshaEn { get; }
        public string NepaliSambatMonthCode { get; }
        public string NepaliSambatMonthName { get; }
        public int? NepaliSambatYear { get; }
        public bool IsVerified { get; }
    }
}