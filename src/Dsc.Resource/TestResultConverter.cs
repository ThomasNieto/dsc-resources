
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Diagnostics.CodeAnalysis;

namespace Dsc.Resource;

public class TestResultConverter<T> : JsonConverter<TestResult<T>>
{
    public override TestResult<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotSupportedException("Deserialization is not supported.");
    }

#if NET6_0_OR_GREATER
    [RequiresDynamicCode("Write method uses JSON serialization that may require runtime code generation.")]
    [RequiresUnreferencedCode("Write method uses JSON serialization that may be unsafe for trimming.")]
#endif
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
