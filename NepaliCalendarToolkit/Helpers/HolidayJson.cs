using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NepaliCalendarToolkit
{
    public static class HolidayJson
    {
        /// <summary>
        ///     Gets a list of all years for which holiday data is available
        /// </summary>
        /// <returns>List of years (BS) with available holiday data</returns>
        public static List<int> GetAvailableYears()
        {
            return CdnDataHelper.GetAvailableHolidayYears();
        }

        // Get holidays for a specific year
        public static List<HolidayData> GetHolidays(int year)
        {
            var holidays = CdnDataHelper.FetchJson<List<HolidayData>>($"Holidays/{year}.json");
            return holidays ?? new List<HolidayData>();
        }

        // Holiday model that matches the JSON structure
        public class HolidayData
        {
            [JsonPropertyName("month")] public int Month { get; set; }

            [JsonPropertyName("day")] public int Day { get; set; }

            [JsonPropertyName("date")] public string Date { get; set; }

            [JsonPropertyName("name")] public string Name { get; set; }
        }
    }
}