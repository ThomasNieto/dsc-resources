using System.CommandLine;

class Program
{
    static async Task<int> Main(string[] args)
    {
        int exitCode = 0;
        var inputOption = new Option<string>("--input", "The file JSON input.");
        inputOption.AddAlias("-i");

        var whatIfOption = new Option<bool>("--what-if", "Run as a what-if operation instead of applying the file configuration.");
        whatIfOption.AddAlias("-w");

        var getCommand = new Command("get", "Retrieve file configuration.")
        {
            inputOption
        };

        var setCommand = new Command("set", "Apply file configuration.")
        {
            inputOption,
            whatIfOption
        };

        var deleteCommand = new Command("delete", "Delete file configuration.")
        {
            inputOption
        };

        getCommand.SetHandler((input) =>
        {
            exitCode = GetHandler(input);
        }, inputOption);

        setCommand.SetHandler((input, whatIf) =>
        {
            exitCode = SetHandler(input, whatIf);
        }, inputOption, whatIfOption);

        deleteCommand.SetHandler((input) =>
        {
            exitCode = DeleteHandler(input);
        }, inputOption);

        var configCommand = new Command("config", "Manage file configuration.");
        configCommand.AddCommand(getCommand);
        configCommand.AddCommand(setCommand);
        configCommand.AddCommand(deleteCommand);

        var schemaCommand = new Command("schema", "Retrieve JSON schema.");
        schemaCommand.SetHandler(() =>
        {
            exitCode = SchemaHandler();
        });

        var rootCommand = new RootCommand("Manage state of Windows files.");
        rootCommand.AddCommand(configCommand);
        rootCommand.AddCommand(schemaCommand);

        await rootCommand.InvokeAsync(args);
        return exitCode;
    }

    static int SchemaHandler()
    {
        Console.WriteLine("in schema");
        return 0;
    }

    static int GetHandler(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return 1;
        }

        Console.WriteLine($"in get {json}");
        return 0;
    }

    static int SetHandler(string json, bool whatIf)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return 1;
        }

        Console.WriteLine($"in set {json} :: what-if {whatIf}");
        return 0;
    }

    static int DeleteHandler(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return 1;
        }

        Console.WriteLine($"in delete {json}");

        return 0;
    }
}