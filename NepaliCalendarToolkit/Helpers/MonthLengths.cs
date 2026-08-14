using System.Collections.Generic;

namespace NepaliCalendarToolkit.Helpers
{
    public static class MonthLengths
    {
        public static readonly Dictionary<int, int[]> Lengths;

        static MonthLengths()
        {
            Lengths = DataProvider.GetData("month-lengths.json", DataProvider.GetEmbedded<Dictionary<int, int[]>>("month-lengths.json"))
                ?? new Dictionary<int, int[]>();
        }
    }
}

