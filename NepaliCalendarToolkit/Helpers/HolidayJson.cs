using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace NepaliCalendarToolkit.Helpers
{
    /// <summary>
    ///     Loads Nepali public holidays from the data repository. Holiday files are fetched
    ///     from the live CDN and cached to disk, so holiday lookups keep working offline once
    ///     the data has been fetched at least once.
    /// </summary>
    public static class HolidayJson
    {
        /// <summary>
        ///     Gets all BS years for which holiday data is considered available. Years are
        ///     derived from the calendar year range (which is itself refreshed from the CDN),
        ///     rather than a hard-coded list, so newly added years appear without a version bump.
        /// </summary>
        /// <returns>List of years (BS) with available holiday data.</returns>
        public static List<int> GetAvailableYears()
        {
            return MonthLengths.Lengths.Keys.OrderBy(k => k).ToList();
        }

        /// <summary>
        ///     Gets the holidays for a specific BS year. The data is fetched from the live CDN
        ///     (cached to disk), and falls back to the bundled baseline (Data/Holidays/{year}.json)
        ///     when offline with no prior cache.
        /// </summary>
        /// <param name="year">BS year to load holidays for.</param>
        /// <returns>List of holidays, or an empty list if none are available.</returns>
        public static List<HolidayData> GetHolidays(int year)
        {
            var path = $"Holidays/{year}.json";
            return DataProvider.GetData<List<HolidayData>>(path, DataProvider.GetEmbedded<List<HolidayData>>(path))
                   ?? new List<HolidayData>();
        }

        /// <summary>
        ///     Holiday model that matches the JSON structure of the data repository
        ///     (e.g. "Holidays/2083.json").
        /// </summary>
        public class HolidayData
        {
            [JsonPropertyName("month")] public int Month { get; set; }

            [JsonPropertyName("day")] public int Day { get; set; }

            [JsonPropertyName("date")] public string Date { get; set; }

            [JsonPropertyName("name")] public string Name { get; set; }
        }
    }
}