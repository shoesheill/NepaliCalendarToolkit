using System.Collections.Generic;

namespace NepaliCalendarToolkit.Helpers
{
    public static class MonthLengths
    {
        public static readonly Dictionary<int, int[]> Lengths;

        static MonthLengths()
        {
            // Try to fetch from CDN first
            var cdnData = CdnDataHelper.FetchJson<Dictionary<int, int[]>>("month-lengths.json");

            if (cdnData != null)
                Lengths = cdnData;
            else
                // Fallback to empty dictionary if CDN fetch fails
                Lengths = new Dictionary<int, int[]>();
        }
    }
}