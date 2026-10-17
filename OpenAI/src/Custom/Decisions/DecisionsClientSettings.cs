using Microsoft.Extensions.Configuration;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Decisions;

[Experimental("SCME0002")]
public sealed class DecisionsClientSettings : ClientSettings
{
    public DecisionsClientOptions Options { get; set; }

    protected override void BindCore(IConfigurationSection section)
    {
        var optionsSection = section.GetSection("Options");
        if (optionsSection.Exists())
        {
            Options ??= new DecisionsClientOptions(optionsSection);
        }
    }
}
