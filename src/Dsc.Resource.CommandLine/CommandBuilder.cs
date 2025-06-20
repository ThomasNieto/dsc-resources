// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System.CommandLine;

namespace Dsc.Resource.CommandLine;

public class CommandBuilder<TResource, TSchema> where TResource : IDscResource<TSchema>
{
    public static RootCommand Build(TResource resource)
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

    private static void SetHandler(ISettable<TSchema> resource, string inputOption)
    {
        var instance = resource.Parse(inputOption);

        if (resource is not ISettable<TSchema> iSettable)
        {
            throw new NotImplementedException("Resource does not support Set capability.");
        }

        iSettable.Set(instance);

        // TODO: Capture output and write it out.
        // Account for null, ActualState, and ActualState/ChangedProperties
    }

    private static void TestHandler(ITestable<TSchema> resource, string inputOption)
    {
        var instance = resource.Parse(inputOption);

        if (resource is not ITestable<TSchema> iTestable)
        {
            throw new NotImplementedException("Resource does not support Test capability.");
        }

        iTestable.Test(instance);

        // TODO: Capture output and write it out.
        // Account for ActualState, and ActualState/DifferingProperties
    }

    private static void DeleteHandler(IDeletable<TSchema> resource, string inputOption)
    {
        var instance = resource.Parse(inputOption);

        if (resource is not IDeletable<TSchema> iTDeletable)
        {
            throw new NotImplementedException("Resource does not support Delete capability.");
        }

        iTDeletable.Delete(instance);
    }

    private static void ExportHandler(IExportable<TSchema> resource)
    {
        if (resource is not IExportable<TSchema> iExportable)
        {
            throw new NotImplementedException("Resource does not support Export capability.");
        }

        iExportable.Export();

        // TODO: Capture output and write it out.
    }
}
