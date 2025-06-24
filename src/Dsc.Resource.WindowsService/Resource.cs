// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System.ComponentModel;
using System.ServiceProcess;
using System.Text.Json;
using System.Text.Json.Schema;

namespace Dsc.Resource.WindowsService;

public sealed class Resource : DscResource<Schema>, IGettable<Schema>, IExportable<Schema>
{
    public Resource() : base("OpenDsc.Windows/Service")
    {
        Description = "Manage Windows services.";
        Tags = ["Windows"];
        ExitCodes.Add(10, new() { Exception = typeof(Win32Exception), Description = "Failed to get services" });
    }

    public override string GetSchema()
    {
        return SourceGenerationContext.Default.Schema.GetJsonSchemaAsNode(JsonSchemaExporterOptions).ToJsonString();
    }

    public override Schema Parse(string json)
    {
        return JsonSerializer.Deserialize(json, SourceGenerationContext.Default.Schema) ?? throw new InvalidDataException();
    }

    public override string ToJson(Schema instance)
    {
        return JsonSerializer.Serialize(instance, SourceGenerationContext.Default.Schema);
    }

    public Schema Get(Schema instance)
    {
        foreach (var service in ServiceController.GetServices())
        {
            if (string.Equals(service.ServiceName, instance.Name, StringComparison.OrdinalIgnoreCase))
            {
                return new Schema()
                {
                    Name = service.ServiceName,
                    DisplayName = service.DisplayName,
                    Status = service.Status,
                    StartType = service.StartType
                };
            }
        }

        return new Schema()
        {
            Name = instance.Name,
            Exist = false
        };
    }

    public IEnumerable<Schema> Export()
    {
        foreach (var service in ServiceController.GetServices())
        {
            yield return new Schema
            {
                Name = service.ServiceName,
                DisplayName = service.DisplayName,
                Status = service.Status,
                StartType = service.StartType
            };
        }
    }
}
