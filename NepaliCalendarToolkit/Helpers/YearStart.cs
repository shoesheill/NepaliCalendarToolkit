using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace NepaliCalendarToolkit
{
    public static class YearStart
    {
        public static readonly Dictionary<int, string> Dates;

        static YearStart()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourcePath = "NepaliCalendarToolkit.Data.year-start.json";

            using (var stream = assembly.GetManifestResourceStream(resourcePath))
            {
                if (stream != null)
                {
                    // Read from embedded resource
                    using (var reader = new StreamReader(stream))
                    {
                        var json = reader.ReadToEnd();
                        Dates = JsonSerializer.Deserialize<Dictionary<int, string>>(json);
                    }
                }
                else
                {
                    // Fallback to file system if not embedded
                    var filePath = Path.Combine(Path.GetDirectoryName(assembly.Location), "Data", "year-start.json");
                    if (File.Exists(filePath))
                    {
                        var json = File.ReadAllText(filePath);
                        Dates = JsonSerializer.Deserialize<Dictionary<int, string>>(json);
                    }
                    else
                    {
                        // Fallback to hardcoded values if file not found
                        Dates = new Dictionary<int, string>
                        {
                            { 2065, "2008-04-14" },
                            { 2066, "2009-04-14" },
                            { 2067, "2010-04-14" },
                            { 2068, "2011-04-14" },
                            { 2069, "2012-04-13" },
                            { 2070, "2013-04-14" },
                            { 2071, "2014-04-14" },
                            { 2072, "2015-04-14" },
                            { 2073, "2016-04-13" },
                            { 2074, "2017-04-14" },
                            { 2075, "2018-04-14" },
                            { 2076, "2019-04-14" },
                            { 2077, "2020-04-13" },
                            { 2078, "2021-04-14" },
                            { 2079, "2022-04-14" },
                            { 2080, "2023-04-14" },
                            { 2081, "2024-04-13" },
                            { 2082, "2025-04-14" },
                            { 2083, "2026-04-14" }
                        };
                    }
                }
            }
        }
    }
}