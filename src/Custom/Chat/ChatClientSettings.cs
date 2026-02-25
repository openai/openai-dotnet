using Microsoft.Extensions.Configuration;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Chat;

[Experimental("SCME0002")]
public sealed class ChatClientSettings : ClientSettings
{
    public string Model { get; set; }

    public OpenAIClientOptions Options { get; set; }

    protected override void BindCore(IConfigurationSection section)
    {
        Model = section["Model"];

        var optionsSection = section.GetSection("Options");
        if (optionsSection.Exists())
        {
            Options ??= new OpenAIClientOptions(optionsSection);
        }
    }
}
