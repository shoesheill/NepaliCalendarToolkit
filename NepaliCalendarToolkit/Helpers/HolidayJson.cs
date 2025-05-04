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

        /// <summary>
        ///     Gets a list of all years for which holiday data is available
        /// </summary>
        /// <returns>List of years (BS) with available holiday data</returns>
        public static List<int> GetAvailableYears()
        {
            // Create directory if it doesn't exist
            if (!Directory.Exists(HolidayDirectoryPath))
            {
                try
                {
                    // Try alternative paths if standard path doesn't exist
                    var alternativePaths = GetAlternativeHolidayPaths();
                    foreach (var path in alternativePaths)
                    {
                        if (Directory.Exists(path))
                        {
                            return GetYearsFromDirectory(path);
                        }
                    }

                    // No valid directory found
                    return new List<int>();
                }
                catch (Exception)
                {
                    // Handle any exceptions gracefully
                    return new List<int>();
                }
            }

            return GetYearsFromDirectory(HolidayDirectoryPath);
        }

        /// <summary>
        ///     Extract years from JSON filenames in a directory
        /// </summary>
        private static List<int> GetYearsFromDirectory(string directoryPath)
        {
            var result = new List<int>();

            // Get all JSON files in the directory
            var jsonFiles = Directory.GetFiles(directoryPath, "*.json");

            // Extract years from filenames
            foreach (var file in jsonFiles)
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                if (int.TryParse(fileName, out var year))
                {
                    result.Add(year);
                }
            }

            return result;
        }

        /// <summary>
        ///     Get alternative possible paths for holiday files
        /// </summary>
        private static List<string> GetAlternativeHolidayPaths()
        {
            var paths = new List<string>();

            try
            {
                // Try to look in the executing assembly location
                var assemblyPath = Path.GetDirectoryName(typeof(HolidayJson).Assembly.Location);
                if (!string.IsNullOrEmpty(assemblyPath))
                {
                    paths.Add(Path.Combine(assemblyPath, "Data", "Holidays"));
                }

                // Try current directory
                var currentDir = Directory.GetCurrentDirectory();
                paths.Add(Path.Combine(currentDir, "Data", "Holidays"));

                // Try parent directories
                var dirInfo = Directory.GetParent(currentDir);
                if (dirInfo != null)
                {
                    paths.Add(Path.Combine(dirInfo.FullName, "Data", "Holidays"));
                    paths.Add(Path.Combine(dirInfo.FullName, "NepaliCalendarToolkit", "Data", "Holidays"));

                    // Try one more level up
                    var parentDirInfo = dirInfo.Parent;
                    if (parentDirInfo != null)
                    {
                        paths.Add(Path.Combine(parentDirInfo.FullName, "NepaliCalendarToolkit", "Data", "Holidays"));
                    }
                }
            }
            catch
            {
                // Ignore errors in path resolution
            }

            return paths;
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