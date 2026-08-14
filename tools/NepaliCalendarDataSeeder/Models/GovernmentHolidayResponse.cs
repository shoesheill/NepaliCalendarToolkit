using System.Text.Json.Serialization;

namespace NepaliCalendarDataSeeder.Models
{
    /// <summary>
    ///     Response model for GET /goverment-holidays/{year}.
    ///     Note: there are NO bs_month / bs_day fields in this response. The BS date is
    ///     embedded in the <see cref="Bs"/> string with format "dd.mm.yyyy".
    /// </summary>
    public class GovernmentHolidayResponse
    {
        [JsonPropertyName("event_id")] public string EventId { get; set; }
        [JsonPropertyName("event_date")] public string EventDate { get; set; }
        [JsonPropertyName("ad")] public string Ad { get; set; }
        [JsonPropertyName("bs")] public string Bs { get; set; }
        [JsonPropertyName("ns_year")] public int NsYear { get; set; }
        [JsonPropertyName("ns_month")] public string NsMonth { get; set; }
        [JsonPropertyName("id")] public string Id { get; set; }
        [JsonPropertyName("title")] public string Title { get; set; }

        /// <summary>JSON-encoded string containing "en"/"ne" descriptions.</summary>
        [JsonPropertyName("description")] public string Description { get; set; }

        [JsonPropertyName("based_on")] public string BasedOn { get; set; }
    }
}
