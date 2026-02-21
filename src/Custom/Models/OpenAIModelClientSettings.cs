using Microsoft.Extensions.Configuration;
using System;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Models;

[Experimental("SCME0002")]
public sealed class OpenAIModelClientSettings : ClientSettings
{
    public OpenAIClientOptions Options { get; set; }

    protected override void BindCore(IConfigurationSection section)
    {
        var optionsSection = section.GetSection("Options");
        if (optionsSection.Exists())
        {
            Options ??= new OpenAIClientOptions(optionsSection);
        }
    }
}
