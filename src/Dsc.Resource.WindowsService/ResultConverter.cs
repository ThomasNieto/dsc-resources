// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dsc.Resource.WindowsService;

public class ResultConverter : JsonConverter<TestResult<Schema>>
{
    public override TestResult<Schema>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotSupportedException();
    }

    public override void Write(Utf8JsonWriter writer, TestResult<Schema> value, JsonSerializerOptions options)
    {
        var typeInfo = SourceGenerationContext.Default.Schema;
        var json = JsonSerializer.SerializeToElement(value.ActualState, typeInfo);

        writer.WriteStartObject();

        foreach (var prop in json.EnumerateObject())
        {
            prop.WriteTo(writer);
        }

        writer.WriteBoolean("_inDesiredState", value.InDesiredState);
        writer.WriteEndObject();
    }
}
