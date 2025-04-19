using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Reflection;

public static class MonthLengths
{
    public static readonly Dictionary<int, int[]> Lengths;

    static MonthLengths()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourcePath = "NepaliCalendarToolkit.Data.month-lengths.json";

        using (var stream = assembly.GetManifestResourceStream(resourcePath))
        {
            if (stream != null)
            {
                // Read from embedded resource
                using (var reader = new StreamReader(stream))
                {
                    var json = reader.ReadToEnd();
                    Lengths = JsonSerializer.Deserialize<Dictionary<int, int[]>>(json);
                }
            }
            else
            {
                // Fallback to file system if not embedded
                var filePath = Path.Combine(Path.GetDirectoryName(assembly.Location), "Data", "month-lengths.json");
                if (File.Exists(filePath))
                {
                    var json = File.ReadAllText(filePath);
                    Lengths = JsonSerializer.Deserialize<Dictionary<int, int[]>>(json);
                }
            }
        }
    }
}