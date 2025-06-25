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
                try
                {
                    GetHandler(resource, inputOption);
                }
                catch (Exception e)
                {
                    HandleException(resource, e);
                }
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
                try
                {
                    SetHandler(resource, inputOption);
                }
                catch (Exception e)
                {
                    HandleException(resource, e);
                }
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
                try
                {
                    TestHandler(resource, inputOption, options);
                }
                catch (Exception e)
                {
                    HandleException(resource, e);
                }
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
                try
                {
                    DeleteHandler(resource, inputOption);
                }
                catch (Exception e)
                {
                    HandleException(resource, e);
                }
            }, inputOption);

            configCommand.AddCommand(deleteCommand);
        }

        if (resource is IExportable<TSchema>)
        {
            var exportCommand = new Command("export", "Export resource configuration.");

            exportCommand.SetHandler(() =>
            {
                try
                {
                    ExportHandler(resource);
                }
                catch (Exception e)
                {
                    HandleException(resource, e);
                }
            });

            configCommand.AddCommand(exportCommand);
        }

        var schemaCommand = new Command("schema", "Retrieve resource JSON schema.");
        schemaCommand.SetHandler(() =>
        {
            try
            {
                SchemaHandler(resource);
            }
            catch (Exception e)
            {
                HandleException(resource, e);
            }
        });

        var manifestCommand = new Command("manifest", "Retrieve resource manifest.");
        manifestCommand.SetHandler(() =>
        {
            try
            {
                ManifestHandler(resource, options);
            }
            catch (Exception e)
            {
                HandleException(resource, e);
            }
        });

        var rootCommand = new RootCommand("Manage resource.");
        rootCommand.AddCommand(configCommand);
        rootCommand.AddCommand(schemaCommand);
        rootCommand.AddCommand(manifestCommand);

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
                try
                {
                    GetHandler(resource, inputOption);
                }
                catch (Exception e)
                {
                    HandleException(resource, e);
                }
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
                try
                {
                    SetHandler(resource, inputOption);
                }
                catch (Exception e)
                {
                    HandleException(resource, e);
                }
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
                try
                {
                    TestHandler(resource, inputOption, context);
                }
                catch (Exception e)
                {
                    HandleException(resource, e);
                }
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
                try
                {
                    DeleteHandler(resource, inputOption);
                }
                catch (Exception e)
                {
                    HandleException(resource, e);
                }
            }, inputOption);

            configCommand.AddCommand(deleteCommand);
        }

        if (resource is IExportable<TSchema>)
        {
            var exportCommand = new Command("export", "Export resource configuration.");

            exportCommand.SetHandler(() =>
            {
                try
                {
                    ExportHandler(resource);
                }
                catch (Exception e)
                {
                    HandleException(resource, e);
                }
            });

            configCommand.AddCommand(exportCommand);
        }

        var schemaCommand = new Command("schema", "Retrieve resource JSON schema.");
        schemaCommand.SetHandler(() =>
        {
            try
            {
                SchemaHandler(resource);
            }
            catch (Exception e)
            {
                HandleException(resource, e);
            }
        });

        var manifestCommand = new Command("manifest", "Retrieve resource manifest.");
        manifestCommand.SetHandler(() =>
        {
            try
            {
                ManifestHandler(resource, context);
            }
            catch (Exception e)
            {
                HandleException(resource, e);
            }
        });

        var rootCommand = new RootCommand("Manage resource.");
        rootCommand.AddCommand(configCommand);
        rootCommand.AddCommand(schemaCommand);
        rootCommand.AddCommand(manifestCommand);

        return rootCommand;
    }
#endif

    private static void SchemaHandler(IDscResource<TSchema> resource)
    {
        Console.WriteLine(resource.GetSchema());
    }

#if NET6_0_OR_GREATER
    private static void ManifestHandler(IDscResource<TSchema> resource, JsonSerializerContext context)
    {
        var json = JsonSerializer.Serialize(resource, typeof(IDscResource<TSchema>), context);
        Console.WriteLine(json);
    }
#endif

#if NETSTANDARD2_0
    private static void ManifestHandler(IDscResource<TSchema> resource, JsonSerializerOptions options)
    {
        var json = JsonSerializer.Serialize(resource, typeof(IDscResource<TSchema>), options);
        Console.WriteLine(json);
    }
#endif

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

    private static void HandleException(TResource resource, Exception e)
    {
        Logger.WriteError(e.Message);
        Logger.WriteTrace($"Exception: {e.GetType().FullName}");

        if (!string.IsNullOrEmpty(e.StackTrace))
        {
            Logger.WriteTrace(e.StackTrace);
        }

        try
        {
            var exitCode = ExitCodeResolver.GetExitCode(resource.ExitCodes, e.GetType());
            Environment.Exit(exitCode);
        }
        catch
        {
            Environment.Exit(int.MaxValue);
        }
    }
}
