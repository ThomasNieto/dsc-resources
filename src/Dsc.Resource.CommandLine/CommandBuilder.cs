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
        Console.WriteLine(resource.GetSchema(typeof(TSchema)));
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

    private static void SetHandler(ISettable<TSchema> resource, TSchema schema)
    {
        throw new NotImplementedException();
    }

    private static void TestHandler(ITestable<TSchema> resource, TSchema schema)
    {
        throw new NotImplementedException();
    }

    private static void DeleteHandler(IDeletable<TSchema> resource, TSchema schema)
    {
        throw new NotImplementedException();
    }

    private static void ExportHandler(IExportable<TSchema> resource)
    {
        throw new NotImplementedException();
    }
}
