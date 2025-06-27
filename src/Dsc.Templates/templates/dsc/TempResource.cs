using System.Text.Json.Serialization;

namespace Temp;

public sealed class TempResource : AotDscResource<TempSchema>, IGettable<TempSchema>, IExportable<TempSchema>
{
    public TempResource(JsonSerializerContext context) : base("OpenDsc.Windows/Service", context)
    {
        Description = "Manage Windows services.";
        Tags = ["Windows"];
        ExitCodes.Add(10, new() { Exception = typeof(Win32Exception), Description = "Failed to get services" });
    }

    public TempSchema Get(TempSchema instance)
    {
        foreach (var service in ServiceController.GetServices())
        {
            if (string.Equals(service.ServiceName, instance.Name, StringComparison.OrdinalIgnoreCase))
            {
                return new TempSchema()
                {
                    Name = service.ServiceName,
                    DisplayName = service.DisplayName,
                    Status = service.Status,
                    StartType = service.StartType
                };
            }
        }

        return new TempSchema()
        {
            Name = instance.Name,
            Exist = false
        };
    }

    public IEnumerable<TempSchema> Export()
    {
        foreach (var service in ServiceController.GetServices())
        {
            yield return new TempSchema
            {
                Name = service.ServiceName,
                DisplayName = service.DisplayName,
                Status = service.Status,
                StartType = service.StartType
            };
        }
    }
}
