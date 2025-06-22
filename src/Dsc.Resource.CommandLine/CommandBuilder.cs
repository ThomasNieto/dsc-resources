// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System.CommandLine;
using System.Text.Json;

#if NET6_0_OR_GREATER
using System.Text.Json.Serialization;
#endif

namespace Dsc.Resource.CommandLine;

public static class CommandBuilder<TResource, TSchema> where TResource : IDscResource<TSchema>
{
#if NETSTANDARD2_0
    public static RootCommand Build(TResource resource, JsonSerializerOptions options)
    {
        var inputOption = new Option<string>("--input", "The file JSON input.");
        inputOption.AddAlias("-i");
        inputOption.IsRequired = true;

        var configCommand = new Command("config", "Manage resource.");

        if (resource is IGettable<TSchema> getter)
        {
            var getCommand = new Command("get", "Retrieve resource configuration.")
            {
                inputOption
            };

            getCommand.SetHandler((string inputOption) =>
            {
                GetHandler(resource, inputOption);
            }, inputOption);

            configCommand.AddCommand(getCommand);
        }

        if (resource is ISettable<TSchema> setter)
        {
            var setCommand = new Command("set", "Set resource configuration.")
            {
                inputOption
            };

            setCommand.SetHandler((string inputOption) =>
            {
                SetHandler(resource, inputOption);
            }, inputOption);

            configCommand.AddCommand(setCommand);
        }

        if (resource is ITestable<TSchema> tester)
        {
            var testCommand = new Command("test", "Test resource configuration.")
            {
                inputOption
            };

            testCommand.SetHandler((string inputOption) =>
            {
                TestHandler(resource, inputOption, options);
            }, inputOption);

            configCommand.AddCommand(testCommand);
        }

        if (resource is IDeletable<TSchema>)
        {
            var deleteCommand = new Command("delete", "Delete resource configuration.")
            {
                inputOption
            };

            deleteCommand.SetHandler((string inputOption) =>
            {
                DeleteHandler(resource, inputOption);
            }, inputOption);

            configCommand.AddCommand(deleteCommand);
        }

        if (resource is IExportable<TSchema>)
        {
            var exportCommand = new Command("export", "Export resource configuration.");

            exportCommand.SetHandler(() =>
            {
                ExportHandler(resource);
            });

            configCommand.AddCommand(exportCommand);
        }

        var schemaCommand = new Command("schema", "Retrieve resource JSON schema.");
        schemaCommand.SetHandler(() =>
        {
            SchemaHandler(resource);
        });

        var rootCommand = new RootCommand("Manage resource.");
        rootCommand.AddCommand(configCommand);
        rootCommand.AddCommand(schemaCommand);

        return rootCommand;
    }
#endif

#if NET6_0_OR_GREATER
    public static RootCommand Build(TResource resource, JsonSerializerContext context)
    {
        var inputOption = new Option<string>("--input", "The file JSON input.");
        inputOption.AddAlias("-i");
        inputOption.IsRequired = true;

        var configCommand = new Command("config", "Manage resource.");

        if (resource is IGettable<TSchema>)
        {
            var getCommand = new Command("get", "Retrieve resource configuration.")
            {
                inputOption
            };

            getCommand.SetHandler((string inputOption) =>
            {
                GetHandler(resource, inputOption);
            }, inputOption);

            configCommand.AddCommand(getCommand);
        }

        if (resource is ISettable<TSchema>)
        {
            var setCommand = new Command("set", "Set resource configuration.")
            {
                inputOption
            };

            setCommand.SetHandler((string inputOption) =>
            {
                SetHandler(resource, inputOption);
            }, inputOption);

            configCommand.AddCommand(setCommand);
        }

        if (resource is ITestable<TSchema>)
        {
            var testCommand = new Command("test", "Test resource configuration.")
            {
                inputOption
            };

            testCommand.SetHandler((string inputOption) =>
            {
                TestHandler(resource, inputOption, context);
            }, inputOption);

            configCommand.AddCommand(testCommand);
        }

        if (resource is IDeletable<TSchema>)
        {
            var deleteCommand = new Command("delete", "Delete resource configuration.")
            {
                inputOption
            };

            deleteCommand.SetHandler((string inputOption) =>
            {
                DeleteHandler(resource, inputOption);
            }, inputOption);

            configCommand.AddCommand(deleteCommand);
        }

        if (resource is IExportable<TSchema>)
        {
            var exportCommand = new Command("export", "Export resource configuration.");

            exportCommand.SetHandler(() =>
            {
                ExportHandler(resource);
            });

            configCommand.AddCommand(exportCommand);
        }

        var schemaCommand = new Command("schema", "Retrieve resource JSON schema.");
        schemaCommand.SetHandler(() =>
        {
            SchemaHandler(resource);
        });

        var rootCommand = new RootCommand("Manage resource.");
        rootCommand.AddCommand(configCommand);
        rootCommand.AddCommand(schemaCommand);

        return rootCommand;
    }
#endif

    private static void SchemaHandler(IDscResource<TSchema> resource)
    {
        Console.WriteLine(resource.GetSchema());
    }

    private static void GetHandler(IDscResource<TSchema> resource, string inputOption)
    {
        var instance = resource.Parse(inputOption);

        if (resource is not IGettable<TSchema> iGettable)
        {
            throw new NotImplementedException("Resource does not support Get capability.");
        }

        var result = iGettable.Get(instance);
        Console.WriteLine(resource.ToJson(result));
    }

    private static void SetHandler(IDscResource<TSchema> resource, string inputOption)
    {
        var instance = resource.Parse(inputOption);

        if (resource is not ISettable<TSchema> iSettable)
        {
            throw new NotImplementedException("Resource does not support Set capability.");
        }

        var result = iSettable.Set(instance);

        if (result is not null)
        {
            var json = resource.ToJson(result.ActualState);
            Console.WriteLine(json);
        }

        if (result?.ChangedProperties is not null)
        {
#if NET6_0_OR_GREATER
            var json = JsonSerializer.Serialize(result.ChangedProperties, typeof(string[]), SourceGenerationContext.Default);
#else
            var json = JsonSerializer.Serialize(result.ChangedProperties);
#endif
            Console.WriteLine(json);
        }
    }

#if NETSTANDARD2_0
    private static void TestHandler(IDscResource<TSchema> resource, string inputOption, JsonSerializerOptions options)
    {
        var instance = resource.Parse(inputOption);

        if (resource is not ITestable<TSchema> iTestable)
        {
            throw new NotImplementedException("Resource does not support Test capability.");
        }

        var result = iTestable.Test(instance);
        var json = JsonSerializer.Serialize(result, options);
        Console.WriteLine(json);

        if (result?.DifferingProperties is not null)
        {
            json = JsonSerializer.Serialize(result.DifferingProperties, options);
            Console.WriteLine(json);
        }
    }
#endif

#if NET6_0_OR_GREATER
    private static void TestHandler(IDscResource<TSchema> resource, string inputOption, JsonSerializerContext context)
    {
        var instance = resource.Parse(inputOption);

        if (resource is not ITestable<TSchema> iTestable)
        {
            throw new NotImplementedException("Resource does not support Test capability.");
        }

        var result = iTestable.Test(instance);
        var json = JsonSerializer.Serialize(result, typeof(TestResult<TSchema>), context);
        Console.WriteLine(json);

        if (result?.DifferingProperties is not null)
        {
            json = JsonSerializer.Serialize(result.DifferingProperties, typeof(string[]), SourceGenerationContext.Default);
            Console.WriteLine(json);
        }
    }
#endif

    private static void DeleteHandler(IDscResource<TSchema> resource, string inputOption)
    {
        var instance = resource.Parse(inputOption);

        if (resource is not IDeletable<TSchema> iTDeletable)
        {
            throw new NotImplementedException("Resource does not support Delete capability.");
        }

        iTDeletable.Delete(instance);
    }

    private static void ExportHandler(IDscResource<TSchema> resource)
    {
        if (resource is not IExportable<TSchema> iExportable)
        {
            throw new NotImplementedException("Resource does not support Export capability.");
        }

        foreach (var instance in iExportable.Export())
        {
            var json = resource.ToJson(instance);
            Console.WriteLine(json);
        }
    }
}
