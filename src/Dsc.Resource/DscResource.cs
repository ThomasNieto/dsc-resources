// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Schema;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Text.RegularExpressions;

using NuGet.Versioning;

namespace Dsc.Resource;

public abstract class DscResource<T> : IDscResource<T>
{
    [JsonPropertyName("$schema")]
    public string ManifestSchema => "https://aka.ms/dsc/schemas/v3/bundled/resource/manifest.json";

    public string Type
    {
        get
        {
            return _type;
        }

        set
        {
            var match = Regex.Match(value, @"^\w+(\.\w+){0,2}\/\w+$");

            if (!match.Success)
            {
                throw new ArgumentException("Value does not match format: <owner>[.<group>][.<area>]/<name>");
            }

            _type = value;
        }
    }

    public string Description { get; set; } = string.Empty;

    public SemanticVersion Version
    {
        get
        {
            if (_semanticVersion is null)
            {
                var version = Process.GetCurrentProcess()?.MainModule?.FileVersionInfo?.ProductVersion
                    ?? throw new InvalidOperationException();
                _semanticVersion = SemanticVersion.Parse(version);
            }

            return _semanticVersion;
        }

        set
        {
            _semanticVersion = value;
        }
    }

    public IEnumerable<string> Tags { get; set; } = [];
    public IDictionary<int, ResourceExitCode> ExitCodes { get; set; } = new Dictionary<int, ResourceExitCode>();

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
    private SemanticVersion? _semanticVersion;
    private string _type = string.Empty;

    protected DscResource(string type)
    {
        Type = type;
        ExitCodes.Add(0, new() { Description = "Success" });
        ExitCodes.Add(1, new() { Description = "Invalid parameter" });
        ExitCodes.Add(2, new() { Exception = typeof(Exception), Description = "Generic Error" });
        ExitCodes.Add(3, new() { Exception = typeof(JsonException), Description = "Invalid JSON" });
    }

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
