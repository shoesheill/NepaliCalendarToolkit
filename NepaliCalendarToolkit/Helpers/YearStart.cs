using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Reflection;

namespace NepaliCalendarToolkit.Data
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
                }
            }
        }
    }
}
