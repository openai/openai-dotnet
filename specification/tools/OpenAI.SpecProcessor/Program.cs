using System.CommandLine;
using Microsoft.Extensions.Configuration;
using OpenAI.SpecProcessor;
using OpenAI.SpecProcessor.Commands;

// Resolve the configuration file. The workflow and the local scripts both pass an explicit path
// so that the committed configuration is the single source of truth, but the file that ships
// alongside the binary is used when no path is given.

var configPath = ResolveConfigPath(args);

var configuration = new ConfigurationBuilder()
    .SetBasePath(Path.GetDirectoryName(configPath) is { Length: > 0 } dir ? dir : AppContext.BaseDirectory)
    .AddJsonFile(Path.GetFileName(configPath), optional: true)
    .Build();

var settings = new ProcessorSettings();
configuration.Bind(settings);

// Build and invoke the CLI.

var rootCommand = new RootCommand("OpenAI REST API specification processor: clean, split, validate, and diff");
rootCommand.Options.Add(new Option<FileInfo?>("--config")
{
    Description = "Path to the processor configuration file [default: appsettings.json alongside the tool]"
});
rootCommand.Subcommands.Add(PreprocessCommand.Create(settings));
rootCommand.Subcommands.Add(IdentityCommand.Create());

return rootCommand.Parse(args).Invoke();

// Configuration has to be read before the parser is built, because the parsed settings supply the
// default values for the command options. So the path is pulled from the raw arguments here.

static string ResolveConfigPath(string[] args)
{
    for (var index = 0; index < args.Length; index++)
    {
        if (args[index] == "--config" && index + 1 < args.Length)
        {
            return Path.GetFullPath(args[index + 1]);
        }

        if (args[index].StartsWith("--config=", StringComparison.Ordinal))
        {
            return Path.GetFullPath(args[index]["--config=".Length..]);
        }
    }

    return Path.Combine(AppContext.BaseDirectory, "appsettings.json");
}
