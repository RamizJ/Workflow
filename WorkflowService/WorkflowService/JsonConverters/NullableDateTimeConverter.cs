using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WorkflowService.JsonConverters
{
    internal class NullableDateTimeConverter : JsonConverter<DateTime?>
    {
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Debug.Assert(typeToConvert == typeof(DateTime));
            if (DateTime.TryParse(reader.GetString(), out var dateTime))
                return dateTime;

            return null;
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value?.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ssZ"));
        }
    }
}