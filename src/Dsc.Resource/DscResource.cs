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
                // DSC requires JSON lines for most output
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
#if NETSTANDARD2_0
                TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
                    new TestResultConverter<T>()
                }
#endif
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

    public virtual string GetSchema()
    {
#if NETSTANDARD2_0
        return JsonSerializerOptions.GetJsonSchemaAsNode(typeof(T), JsonSchemaExporterOptions).ToString();
#else
        throw new NotSupportedException("Use source-generated schema generation in .NET 6+ for trimming safety.");
#endif
    }

    public virtual T Parse(string json)
    {
#if NETSTANDARD2_0
        return JsonSerializer.Deserialize<T>(json, JsonSerializerOptions) ?? throw new InvalidDataException();
#else
        throw new NotSupportedException("Use source-generated deserialization in .NET 6+ for trimming safety.");
#endif
    }

    public virtual string ToJson(T item)
    {
#if NETSTANDARD2_0
        return JsonSerializer.Serialize(item, JsonSerializerOptions);
#else
        throw new NotSupportedException("Use source-generated serialization in .NET 6+ for trimming safety.");
#endif
    }
}
