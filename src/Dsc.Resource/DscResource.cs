// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System.Text.Json;
using System.Text.Json.Schema;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Dsc.Resource;

public abstract class DscResource<T> : IDscResource<T>
{
    protected JsonSerializerOptions JsonSerializerOptions
    {
        get
        {
            _jsonSerializerOptions ??= new JsonSerializerOptions()
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
                TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };

            return _jsonSerializerOptions;
        }

        set
        {
            _jsonSerializerOptions = value;
        }
    }

    protected JsonSchemaExporterOptions JsonSchemaExporterOptions
    {
        get
        {
            _jsonSchemaExporterOptions ??= new JsonSchemaExporterOptions()
            {
                TreatNullObliviousAsNonNullable = true
            };

            return _jsonSchemaExporterOptions;
        }

        set
        {
            _jsonSchemaExporterOptions = value;
        }
    }

    private JsonSerializerOptions? _jsonSerializerOptions;
    private JsonSchemaExporterOptions? _jsonSchemaExporterOptions;

    public virtual string GetSchema(Type type)
    {
        return JsonSerializerOptions.GetJsonSchemaAsNode(type, JsonSchemaExporterOptions).ToString();
    }

    public virtual string ToJson(T input)
    {
        return JsonSerializer.Serialize(input, JsonSerializerOptions);
    }

    public virtual T Parse(string json)
    {
        return JsonSerializer.Deserialize<T>(json, JsonSerializerOptions) ?? throw new InvalidOperationException();
    }

    protected void WriteInfo(string message)
    {
        var infoMessage = new Info() { Message = message };
#if NET6_0_OR_GREATER
        string json = JsonSerializer.Serialize(infoMessage, typeof(Info), SourceGenerationContext.Default);
#else
        string json = JsonSerializer.Serialize(infoMessage, typeof(Info), JsonSerializerOptions);
#endif
        Console.Error.WriteLine(json);
    }

    protected void WriteWarning(string message)
    {
        var warningMessage = new Warning() { Message = message };
#if NET6_0_OR_GREATER
        string json = JsonSerializer.Serialize(warningMessage, typeof(Warning), SourceGenerationContext.Default);
#else
        string json = JsonSerializer.Serialize(warningMessage, JsonSerializerOptions);
#endif
        Console.Error.WriteLine(json);
    }

    protected void WriteError(string message)
    {
        var errorMessage = new Error() { Message = message };
#if NET6_0_OR_GREATER
        string json = JsonSerializer.Serialize(errorMessage, typeof(Error), SourceGenerationContext.Default);
#else
        string json = JsonSerializer.Serialize(errorMessage, JsonSerializerOptions);
#endif
        Console.Error.WriteLine(json);
    }

    protected void WriteTrace(string message)
    {
        var traceMessage = new Trace() { Message = message };
#if NET6_0_OR_GREATER
        string json = JsonSerializer.Serialize(traceMessage, typeof(Trace), SourceGenerationContext.Default);
#else
        string json = JsonSerializer.Serialize(traceMessage, JsonSerializerOptions);
#endif
        Console.Error.WriteLine(json);
    }
}
