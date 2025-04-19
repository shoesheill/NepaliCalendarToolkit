namespace NepaliCalendarToolkit.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public static class HolidayJson
    {
        private static readonly string HolidayDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Holidays");

        // Holiday model that matches the JSON structure
        public class HolidayData
        {
            [JsonPropertyName("month")]
            public int Month { get; set; }

            [JsonPropertyName("day")]
            public int Day { get; set; }

            [JsonPropertyName("date")]
            public string Date { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; }
        }

        // Get holidays for a specific year
        public static List<HolidayData> GetHolidays(int year)
        {
            string filePath = GetHolidayFilePath(year);

            // Check if the file exists
            if (!File.Exists(filePath))
            {
                return new List<HolidayData>();
            }

            string jsonContent = File.ReadAllText(filePath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<List<HolidayData>>(jsonContent, options);
        }

        // Get holiday file path
        private static string GetHolidayFilePath(int year)
        {
            return Path.Combine(HolidayDirectoryPath, $"{year}.json");
        }
    }
}