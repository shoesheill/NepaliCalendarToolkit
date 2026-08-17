using System.Text.Json.Serialization;

namespace NepaliCalendarDataSeeder.Models
{
    /// <summary>
    ///     Response model for POST /calendars/dateConvert.
    ///     The endpoint accepts application/x-www-form-urlencoded requests only.
    ///     Numeric fields are nullable because the API returns null / non-numeric values for
    ///     dates that are not yet verified (e.g. a future BS year), which would otherwise
    ///     throw during JSON deserialization.
    /// </summary>
    public class DateConvertResponse
    {
        [JsonPropertyName("ad")] public string Ad { get; set; }
        [JsonPropertyName("bs")] public string Bs { get; set; }
        [JsonPropertyName("bs_day")] public int? BsDay { get; set; }
        [JsonPropertyName("bs_month")] public int? BsMonth { get; set; }
        [JsonPropertyName("bs_year")] public int? BsYear { get; set; }
        [JsonPropertyName("tithi")] public int? Tithi { get; set; }
        [JsonPropertyName("ns_month")] public string NsMonth { get; set; }
        [JsonPropertyName("ns_year")] public int? NsYear { get; set; }
        [JsonPropertyName("is_verified")] public int? IsVerified { get; set; }
        [JsonPropertyName("remarks")] public string Remarks { get; set; }
        [JsonPropertyName("updated_at")] public string UpdatedAt { get; set; }
    }
}
