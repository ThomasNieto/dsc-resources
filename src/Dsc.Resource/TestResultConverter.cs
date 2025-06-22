// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.


using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dsc.Resource;

#if NETSTANDARD2_0
public class TestResultConverter<T> : JsonConverter<TestResult<T>>
{
    public override TestResult<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotSupportedException("Deserialization is not supported.");
    }

    public override void Write(Utf8JsonWriter writer, TestResult<T> value, JsonSerializerOptions options)
    {
        var json = JsonSerializer.SerializeToElement(value.ActualState, options);

        writer.WriteStartObject();

        foreach (var prop in json.EnumerateObject())
        {
            prop.WriteTo(writer);
        }

        writer.WriteBoolean("_inDesiredState", value.InDesiredState);
        writer.WriteEndObject();
    }
}
#endif
