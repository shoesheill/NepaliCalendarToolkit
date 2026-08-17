using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NepaliCalendarDataSeeder.Services
{
    /// <summary>
    ///     Lenient converter for optional integer fields in the Nepal Patro API responses.
    ///     For unverified / future dates the API occasionally returns these numeric fields as
    ///     a string or an empty string (e.g. "chandrama": "" ) instead of a JSON number, which
    ///     makes the default int? deserializer throw. This converter accepts a JSON number, a
    ///     numeric string, a blank string, or null and falls back to null otherwise, so a single
    ///     quirky field can never abort the seeding of an entire year.
    /// </summary>
    public class LenientNullableIntConverter : JsonConverter<int?>
    {
        public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    return null;
                case JsonTokenType.Number:
                    return reader.GetInt32();
                case JsonTokenType.String:
                    var s = reader.GetString();
                    if (string.IsNullOrWhiteSpace(s))
                        return null;
                    if (int.TryParse(s.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
                        return parsed;
                    return null;
                default:
                    return null;
            }
        }

        public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
                writer.WriteNumberValue(value.Value);
            else
                writer.WriteNullValue();
        }
    }
}
