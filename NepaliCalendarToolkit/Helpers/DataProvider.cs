using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;

namespace NepaliCalendarToolkit.Helpers
{
    /// <summary>
    ///     Tiered data provider that resolves toolkit data (month lengths, year starts and
    ///     holidays) in this order: <b>live CDN</b> → <b>persistent disk cache</b> →
    ///     <b>embedded baseline</b>. This keeps the data fresh whenever the device is online,
    ///     and fully functional when it is offline.
    /// </summary>
    public static class DataProvider
    {
        private static readonly HttpClient HttpClient;
        private static readonly string CacheDirectory;

        /// <summary>
        ///     Root URL of the data repository (a jsDelivr or plain HTTP host). This is
        ///     intentionally <b>not hard-coded</b>: it is read from the
        ///     <c>DATA_URL</c> environment variable at startup, or set
        ///     explicitly via <see cref="Configure"/>. Remains empty when not configured.
        /// </summary>
        public static string BaseUrl { get; private set; }

        /// <summary>
        ///     How often (in hours) the CDN is re-checked when a cached copy already exists.
        ///     A lower value keeps data fresher at the cost of more network calls; a higher
        ///     value favours speed and offline resilience. Defaults to 12 hours.
        /// </summary>
        public static int CacheTtlHours { get; set; } = 12;

        static DataProvider()
        {
            HttpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
            HttpClient.DefaultRequestHeaders.Add("User-Agent", "NepaliCalendarToolkit/1.0");

            // Load the data source URL from configuration, never from hard-coded code.
            var envUrl = Environment.GetEnvironmentVariable("DATA_URL");
            if (!string.IsNullOrWhiteSpace(envUrl))
                BaseUrl = NormalizeUrl(envUrl);

            try
            {
                var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                CacheDirectory = Path.Combine(localAppData, "NepaliCalendarToolkit", "Cache");
            }
            catch
            {
                CacheDirectory = Path.Combine(Path.GetTempPath(), "NepaliCalendarToolkit", "Cache");
            }
        }

        private static string NormalizeUrl(string url)
        {
            return url.EndsWith("/", StringComparison.Ordinal) ? url : url + "/";
        }
        /// <summary>
        ///     Configures the data source at startup (typically from appsettings). Call this
        ///     once before any conversion or holiday call.
        /// </summary>
        /// <param name="baseUrl">Optional root URL of a compatible data repository; a trailing slash is added automatically.</param>
        /// <param name="cacheTtlHours">Optional cache time-to-live in hours.</param>
        public static void Configure(string baseUrl = null, int? cacheTtlHours = null)
        {
            if (!string.IsNullOrWhiteSpace(baseUrl))
                BaseUrl = NormalizeUrl(baseUrl);

            if (cacheTtlHours.HasValue && cacheTtlHours.Value > 0)
                CacheTtlHours = cacheTtlHours.Value;
        }
        /// <summary>
        ///     Resolves data for a CDN-relative path using the CDN → disk cache → fallback
        ///     strategy described in the class summary.
        /// </summary>
        /// <typeparam name="T">Type to deserialize the JSON into.</typeparam>
        /// <param name="path">Path relative to the base URL (e.g. "month-lengths.json" or "Holidays/2083.json").</param>
        /// <param name="fallback">Value returned when the CDN and cache are both unavailable (default <c>null</c>).</param>
        /// <returns>Deserialized data, or <paramref name="fallback"/> on failure.</returns>
        public static T GetData<T>(string path, T fallback = default)
        {
            var cacheFile = GetCacheFilePath(path);

            // 1. Serve from the disk cache when it is fresh enough (avoids an unnecessary
            //    network call and guarantees fast, offline-friendly operation).
            if (cacheFile != null && File.Exists(cacheFile) && IsCacheFresh(cacheFile))
            {
                var cached = TryDeserialize<T>(ReadFile(cacheFile));
                if (cached != null) return cached;
            }

            // 2. Try the live CDN when online (this is where new data arrives without a version bump).
            try
            {
                var raw = HttpClient.GetStringAsync(BaseUrl + path).GetAwaiter().GetResult();
                var data = TryDeserialize<T>(raw);
                if (data != null)
                {
                    WriteCache(cacheFile, raw);
                    return data;
                }
            }
            catch
            {
                // CDN unavailable - fall through to the cache/baseline below.
            }

            // 3. Fall back to the (possibly stale) disk cache so we keep working offline.
            if (cacheFile != null && File.Exists(cacheFile))
            {
                var cached = TryDeserialize<T>(ReadFile(cacheFile));
                if (cached != null) return cached;
            }

            // 4. Last resort: return the caller-provided baseline.
            return fallback;
        }
        /// <summary>
        ///     Loads a bundled offline baseline (embedded as a managed resource in the assembly),
        ///     used on a true first run with no network and no cache.
        /// </summary>
        /// <typeparam name="T">Type to deserialize the JSON into.</typeparam>
        /// <param name="resourceKey">Suffix of the embedded resource name, e.g. "month-lengths.json" or "Holidays/2073.json".</param>
        /// <returns>Deserialized data, or <c>default</c> if the resource is missing or unreadable.</returns>
        public static T GetEmbedded<T>(string resourceKey)
        {
            try
            {
                var assembly = typeof(DataProvider).Assembly;
                // Embedded resources replace directory separators with dots
                // (e.g. "Holidays/2083.json" -> "...Holidays.2083.json"), so normalise the key.
                var normalizedKey = resourceKey.Replace('/', '.').TrimStart('.');

                var fullName = assembly.GetManifestResourceNames()
                    .FirstOrDefault(n => n.EndsWith(normalizedKey, StringComparison.OrdinalIgnoreCase));

                if (fullName == null) return default;

                using (var stream = assembly.GetManifestResourceStream(fullName))
                {
                    if (stream == null) return default;
                    using (var reader = new StreamReader(stream))
                    {
                        return JsonSerializer.Deserialize<T>(reader.ReadToEnd(), new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                    }
                }
            }
            catch
            {
                return default;
            }
        }

        private static string GetCacheFilePath(string path)
        {
            if (string.IsNullOrEmpty(CacheDirectory)) return null;
            try
            {
                return Path.Combine(CacheDirectory, path.Replace('/', '_').Replace('.', '_') + ".json");
            }
            catch
            {
                return null;
            }
        }

        private static bool IsCacheFresh(string cacheFile)
        {
            try
            {
                var age = DateTime.UtcNow - File.GetLastWriteTimeUtc(cacheFile);
                return age.TotalHours <= CacheTtlHours;
            }
            catch
            {
                return false;
            }
        }

        private static string ReadFile(string path)
        {
            try
            {
                return File.Exists(path) ? File.ReadAllText(path) : null;
            }
            catch
            {
                return null;
            }
        }

        private static void WriteCache(string path, string content)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(content)) return;
            try
            {
                var dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
                File.WriteAllText(path, content);
            }
            catch
            {
                // Caching is best-effort; a write failure must never break the caller.
            }
        }

        private static T TryDeserialize<T>(string json)
        {
            if (string.IsNullOrEmpty(json)) return default;
            try
            {
                return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch
            {
                return default;
            }
        }
    }
}
