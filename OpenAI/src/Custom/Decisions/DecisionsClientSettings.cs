using Microsoft.Extensions.Configuration;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Decisions;

// CUSTOM: Added settings to support configuration-based authentication and client registration.
[Experimental("SCME0002")]
public sealed class DecisionsClientSettings : ClientSettings
{
    public DecisionsClientOptions Options { get; set; }

    // CUSTOM: Bind the Decisions-specific options without replacing explicitly supplied options.
    protected override void BindCore(IConfigurationSection section)
    {
        var optionsSection = section.GetSection("Options");
        if (optionsSection.Exists())
        {
            Options ??= new DecisionsClientOptions(optionsSection);
        }
    }
}
