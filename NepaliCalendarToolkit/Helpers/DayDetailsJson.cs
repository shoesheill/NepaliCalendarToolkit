using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NepaliCalendarToolkit.Helpers
{
    internal static class DayDetailsJson
    {
        public static List<DayDetailsData> GetDayDetails(int year)
        {
            var path = $"DayDetails/{year}.json";
            return DataProvider.GetData<List<DayDetailsData>>(
                       path,
                       DataProvider.GetEmbedded<List<DayDetailsData>>(path))
                   ?? new List<DayDetailsData>();
        }

        internal class DayDetailsData
        {
            [JsonPropertyName("adDate")] public string AdDate { get; set; }
            [JsonPropertyName("bsMonth")] public int BsMonth { get; set; }
            [JsonPropertyName("bsDay")] public int BsDay { get; set; }
            [JsonPropertyName("tithi")] public int? Tithi { get; set; }
            [JsonPropertyName("chandrama")] public int? Chandrama { get; set; }
            [JsonPropertyName("nsMonth")] public string NsMonth { get; set; }
            [JsonPropertyName("nsYear")] public int? NsYear { get; set; }
            [JsonPropertyName("isVerified")] public bool IsVerified { get; set; }
        }
    }
}