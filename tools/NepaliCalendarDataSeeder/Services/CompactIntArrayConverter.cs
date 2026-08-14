using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NepaliCalendarDataSeeder.Services
{
    /// <summary>
    ///     Serializes int[] arrays inline (e.g. [31, 32, 31]) instead of expanding
    ///     every element onto its own line when WriteIndented is enabled. This keeps
    ///     the month-lengths dictionary compact: one year (row) per line.
    /// </summary>
    public sealed class CompactIntArrayConverter : JsonConverter<int[]>
    {
        public override int[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => throw new NotSupportedException("Deserialization is not supported by this converter.");

        public override void Write(Utf8JsonWriter writer, int[] value, JsonSerializerOptions options)
        {
            // Emit the array as a single inline JSON fragment so the surrounding
            // indented object keeps one property per line, e.g.
            // "2065": [31, 32, 31, 32, 31, 30, 30, 30, 29, 29, 30, 31].
            // string.Join handles a null array gracefully (returns "").
            var inline = "[" + string.Join(", ", value) + "]";
            writer.WriteRawValue(inline);
        }
    }
}
