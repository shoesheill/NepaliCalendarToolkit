using System.Collections.Generic;

namespace NepaliCalendarToolkit.Helpers
{
    /// <summary>
    ///     Loads the AD start date for each supported Nepali (BS) year. Data comes from the
    ///     live CDN with a persistent offline cache, and falls back to the bundled baseline
    ///     (Data/year-start.json) when neither is available, so the library always works offline.
    /// </summary>
    public static class YearStart
    {
        public static readonly Dictionary<int, string> Dates;

        static YearStart()
        {
            Dates = DataProvider.GetData(
                "year-start.json",
                DataProvider.GetEmbedded<Dictionary<int, string>>("year-start.json"))
                ?? new Dictionary<int, string>();
        }
    }
}
