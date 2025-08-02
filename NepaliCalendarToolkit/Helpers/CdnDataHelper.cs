using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace NepaliCalendarToolkit
{
    public static class CdnDataHelper
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        private static readonly string BaseUrl = "https://cdn.jsdelivr.net/gh/shoesheill/shiranai-deto@master/";

        static CdnDataHelper()
        {
            // Set timeout and user agent
            HttpClient.Timeout = TimeSpan.FromSeconds(30);
            HttpClient.DefaultRequestHeaders.Add("User-Agent", "NepaliCalendarToolkit/1.0");
        }

        /// <summary>
        /// Fetches JSON data from the CDN
        /// </summary>
        /// <typeparam name="T">Type to deserialize the JSON into</typeparam>
        /// <param name="path">Path relative to the CDN base URL</param>
        /// <returns>Deserialized object or null if failed</returns>
        public static async Task<T> FetchJsonAsync<T>(string path)
        {
            try
            {
                var url = BaseUrl + path;
                var response = await HttpClient.GetStringAsync(url);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                return JsonSerializer.Deserialize<T>(response, options);
            }
            catch (Exception)
            {
                // Return null if fetching fails
                return default(T);
            }
        }

        /// <summary>
        /// Fetches JSON data from the CDN synchronously (for backward compatibility)
        /// </summary>
        /// <typeparam name="T">Type to deserialize the JSON into</typeparam>
        /// <param name="path">Path relative to the CDN base URL</param>
        /// <returns>Deserialized object or null if failed</returns>
        public static T FetchJson<T>(string path)
        {
            try
            {
                var url = BaseUrl + path;
                var response = HttpClient.GetStringAsync(url).GetAwaiter().GetResult();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                return JsonSerializer.Deserialize<T>(response, options);
            }
            catch (Exception)
            {
                // Return null if fetching fails
                return default(T);
            }
        }

        /// <summary>
        /// Gets a list of available holiday years by checking the CDN
        /// </summary>
        /// <returns>List of available years</returns>
        public static List<int> GetAvailableHolidayYears()
        {
            var years = new List<int>();
            for (int year = 2065; year <= 2083; year++)
            {
                try
                {
                    var url = BaseUrl + $"Holidays/{year}.json";
                    var response = HttpClient.GetAsync(url).GetAwaiter().GetResult();
                    if (response.IsSuccessStatusCode)
                    {
                        years.Add(year);
                    }
                }
                catch
                {
                    // Continue checking other years
                }
            }

            return years;
        }
    }
}