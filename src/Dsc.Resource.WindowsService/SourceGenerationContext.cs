// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System.ServiceProcess;
using System.Text.Json.Serialization;

namespace Dsc.Resource.WindowsService;

[JsonSourceGenerationOptions(WriteIndented = false,
                             PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
                             UseStringEnumConverter = true,
                             DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                             Converters = [typeof(ResultConverter), typeof(ResourceConverter)])]
[JsonSerializable(typeof(Schema))]
[JsonSerializable(typeof(TestResult<Schema>))]
[JsonSerializable(typeof(string[]))]
[JsonSerializable(typeof(ServiceControllerStatus))]
[JsonSerializable(typeof(ServiceStartMode))]
[JsonSerializable(typeof(IDscResource<Schema>))]
internal partial class SourceGenerationContext : JsonSerializerContext
{

}
