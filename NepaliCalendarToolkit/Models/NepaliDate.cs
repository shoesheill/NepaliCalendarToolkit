namespace NepaliCalendarToolkit.Models
{
    public class NepaliDate
    {
        public NepaliDate(int year, int month, int day)
        {
            Year = year;
            Month = month;
            Day = day;
        }

        private int Year { get; }

        private int Month { get; }
        private int Day { get; }
        public int GetYear => Year;
        public int GetMonth => Month;
        public int GetDay => Day;

        public override string ToString()
        {
            return $"{Year}-{Month:D2}-{Day:D2}";
        }
    }
}