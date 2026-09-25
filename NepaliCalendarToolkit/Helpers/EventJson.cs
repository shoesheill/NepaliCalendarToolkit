using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace NepaliCalendarToolkit.Helpers
{
    internal static class EventJson
    {
        public static List<EventData> GetEvents(int year)
        {
            var path = $"Events/{year}.json";
            return DataProvider.GetData<List<EventData>>(
                       path,
                       DataProvider.GetEmbedded<List<EventData>>(path))
                   ?? new List<EventData>();
        }

        internal class EventData
        {
            [JsonPropertyName("adDate")] public string AdDate { get; set; }
            [JsonPropertyName("bsMonth")] public int BsMonth { get; set; }
            [JsonPropertyName("bsDay")] public int BsDay { get; set; }
            [JsonPropertyName("nsYear")] public int NsYear { get; set; }
            [JsonPropertyName("nsMonth")] public string NsMonth { get; set; }
            [JsonPropertyName("nameEn")] public string NameEn { get; set; }
            [JsonPropertyName("nameNe")] public string NameNe { get; set; }
            [JsonPropertyName("holidayType")] public string HolidayType { get; set; }
            [JsonPropertyName("category")] public string Category { get; set; }
            [JsonPropertyName("basedOn")] public string BasedOn { get; set; }
            [JsonPropertyName("isGovernmentHoliday")] public bool IsGovernmentHoliday { get; set; }
            [JsonPropertyName("isImportant")] public bool IsImportant { get; set; }
        }
    }
}