// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System.Text.Json.Serialization;

namespace Dsc.Resource;

public sealed class Info
{
    [JsonPropertyName("info")]
    public string Message { get; set; } = string.Empty;
}

public sealed class Warning
{
    [JsonPropertyName("warn")]
    public string Message { get; set; } = string.Empty;
}

public sealed class Error
{
    [JsonPropertyName("error")]
    public string Message { get; set; } = string.Empty;
}

public sealed class Trace
{
    [JsonPropertyName("trace")]
    public string Message { get; set; } = string.Empty;
}

#if NET6_0_OR_GREATER
[JsonSourceGenerationOptions(WriteIndented = true,
                             PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
                             UseStringEnumConverter = true,
                             UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
                             GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(Info))]
[JsonSerializable(typeof(Warning))]
[JsonSerializable(typeof(Error))]
[JsonSerializable(typeof(Trace))]
internal partial class SourceGenerationContext : JsonSerializerContext
{

}
#endif
