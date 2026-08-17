using System;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using NepaliCalendarDataSeeder.Services;

namespace NepaliCalendarDataSeeder
{
    /// <summary>
    ///     Entry point for the data seeder tool. Reads the Nepal Patro API URL from
    ///     appsettings.json (ApiUrl) or the APIURL environment variable, then incrementally
    ///     seeds calendar data into NepaliCalendarToolkit/Data.
    ///
    ///     Optional arguments:
    ///       --date      yyyy-MM-dd   reference AD date (default: today)
    ///       --offset    N            seed N BS years beyond the current BS year (default: 1)
    ///       --max-year  N            seed up to the given BS year (overrides offset)
    /// </summary>
    public static class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var seederDir = FindSeederProjectDirectory();

            var apiUrl = ResolveApiUrl(seederDir);
            if (string.IsNullOrWhiteSpace(apiUrl))
            {
                Console.Error.WriteLine("API URL is not configured. Set 'API_URL' in appsettings.json or the API_URL env var.");
                return 1;
            }

            var date = DateTime.Today;
            // year-start only needs Baisakh 1 of the target year, so it can legitimately
            // run ahead of month-lengths/holidays (which need the year after that). Default
            // to +1 so year-start opportunistically grabs next year's start as soon as the
            // API publishes it, instead of waiting for the scheduled run after year-end.
            var offset = 0;
            int? maxYear = null;

            for (var i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--date":
                        date = DateTime.ParseExact(
                            args[++i], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        break;
                    case "--offset":
                        offset = int.Parse(args[++i], CultureInfo.InvariantCulture);
                        break;
                    case "--max-year":
                        maxYear = int.Parse(args[++i], CultureInfo.InvariantCulture);
                        break;
                    default:
                        Console.WriteLine($"Unknown argument ignored: {args[i]}");
                        break;
                }
            }

            // Seeder lives at <repo>/tools/NepaliCalendarDataSeeder; the toolkit data folder
            // is <repo>/NepaliCalendarToolkit/Data (embedded by the toolkit's csproj).
            var repoRoot = Path.GetFullPath(Path.Combine(seederDir, "..", ".."));
            var dataDir = Path.Combine(repoRoot, "Data");

            var api = new ApiClient(apiUrl);
            var seeder = new DataSeeder(api, dataDir);

            try
            {
                await seeder.RunAsync(date, offset, maxYear);
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Seeding failed: {ex.Message}");
                return 2;
            }
        }

        /// <summary>
        ///     Locates the seeder project directory by walking up from the running assembly
        ///     until it finds NepaliCalendarDataSeeder.csproj. This is robust across bin/Debug
        ///     vs bin/Release and any target-framework sub-folder depth.
        /// </summary>
        private static string FindSeederProjectDirectory()
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null)
            {
                if (File.Exists(Path.Combine(dir.FullName, "NepaliCalendarDataSeeder.csproj")))
                    return dir.FullName;
                dir = dir.Parent;
            }

            // Fall back to the current working directory as a last resort.
            return Environment.CurrentDirectory;
        }

        private static string ResolveApiUrl(string seederDir)
        {
            var env = Environment.GetEnvironmentVariable("API_URL");
            if (!string.IsNullOrWhiteSpace(env))
                return env.Trim();

            // Prefer appsettings.json in the seeder project dir, then the toolkit's copy.
            var candidates = new[]
            {
                Path.Combine(seederDir, "appsettings.json"),
                Path.Combine(seederDir, "..", "..", "NepaliCalendarToolkit", "appsettings.json")
            };

            foreach (var path in candidates)
            {
                if (!File.Exists(path))
                    continue;

                try
                {
                    using var doc = JsonDocument.Parse(File.ReadAllText(path));
                    if (doc.RootElement.TryGetProperty("API_URL", out var apiUrl))
                    {
                        var value = apiUrl.GetString()?.Trim();
                        if (!string.IsNullOrWhiteSpace(value))
                            return value;
                    }
                }
                catch
                {
                    // Try the next candidate if this file cannot be parsed.
                }
            }

            return null;
        }
    }
}
