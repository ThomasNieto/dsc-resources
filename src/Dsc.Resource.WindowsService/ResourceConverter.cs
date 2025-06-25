// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dsc.Resource.WindowsService;

public class ResourceConverter : JsonConverter<IDscResource<Schema>>
{
    public override IDscResource<Schema>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotSupportedException("Deserialization is not supported.");
    }

    public override void Write(Utf8JsonWriter writer, IDscResource<Schema> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("$schema", value.ManifestSchema);
        writer.WriteString("type", value.Type);
        writer.WriteString("description", value.Description);
        writer.WriteString("version", value.Version.ToString());
        writer.WritePropertyName("tags");
        writer.WriteStartArray();
        foreach (var tag in value.Tags)
        {
            writer.WriteStringValue(tag);
        }
        writer.WriteEndArray();

        writer.WritePropertyName("exitCodes");
        writer.WriteStartObject();

        foreach (var kvp in value.ExitCodes)
        {
            writer.WriteString(kvp.Key.ToString(), kvp.Value.Description);
        }

        writer.WriteEndObject();

        writer.WritePropertyName("schema");
        writer.WriteStartObject();
        writer.WritePropertyName("command");
        writer.WriteStartObject();
        writer.WriteString("executable", value.FileName);
        writer.WritePropertyName("args");
        writer.WriteStartArray();
        writer.WriteStringValue("schema");
        writer.WriteEndArray();
        writer.WriteEndObject();
        writer.WriteEndObject();

        if (value is IGettable<Schema>)
        {
            writer.WritePropertyName("get");
            writer.WriteStartObject();
            writer.WriteString("executable", value.FileName);
            writer.WritePropertyName("args");
            writer.WriteStartArray();
            writer.WriteStringValue("config");
            writer.WriteStringValue("get");
            writer.WriteStartObject();
            writer.WriteString("jsonInputArg", "--input");
            writer.WriteBoolean("mandatory", true);
            writer.WriteEndObject();
            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        if (value is ISettable<Schema>)
        {
            writer.WritePropertyName("set");
            writer.WriteStartObject();
            writer.WriteString("executable", value.FileName);
            writer.WritePropertyName("args");
            writer.WriteStartArray();
            writer.WriteStringValue("config");
            writer.WriteStringValue("set");
            writer.WriteStartObject();
            writer.WriteString("jsonInputArg", "--input");
            writer.WriteBoolean("mandatory", true);
            writer.WriteEndObject();
            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        if (value is ITestable<Schema>)
        {
            writer.WritePropertyName("test");
            writer.WriteStartObject();
            writer.WriteString("executable", value.FileName);
            writer.WritePropertyName("args");
            writer.WriteStartArray();
            writer.WriteStringValue("config");
            writer.WriteStringValue("test");
            writer.WriteStartObject();
            writer.WriteString("jsonInputArg", "--input");
            writer.WriteBoolean("mandatory", true);
            writer.WriteEndObject();
            writer.WriteEndArray();
            // TODO: Update dynamically
            writer.WriteString("return", "state");
            writer.WriteEndObject();
        }

        if (value is IDeletable<Schema>)
        {
            writer.WritePropertyName("delete");
            writer.WriteStartObject();
            writer.WriteString("executable", value.FileName);
            writer.WritePropertyName("args");
            writer.WriteStartArray();
            writer.WriteStringValue("config");
            writer.WriteStringValue("delete");
            writer.WriteStartObject();
            writer.WriteString("jsonInputArg", "--input");
            writer.WriteBoolean("mandatory", true);
            writer.WriteEndObject();
            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        if (value is IExportable<Schema>)
        {
            writer.WritePropertyName("export");
            writer.WriteStartObject();
            writer.WriteString("executable", value.FileName);
            writer.WritePropertyName("args");
            writer.WriteStartArray();
            writer.WriteStringValue("config");
            writer.WriteStringValue("export");
            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        writer.WriteEndObject();
    }
}
