// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System.Text.Json;
using System.Text.Json.Serialization;

#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Dsc.Resource;

#if NET6_0_OR_GREATER
    [RequiresDynamicCodeAttribute("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
    [RequiresUnreferencedCodeAttribute("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
#endif
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
