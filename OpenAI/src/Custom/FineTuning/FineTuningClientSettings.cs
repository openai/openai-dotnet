using Microsoft.Extensions.Configuration;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.FineTuning;

[Experimental("SCME0002")]
public sealed class FineTuningClientSettings : ClientSettings
{
    public FineTuningClientOptions Options { get; set; }

    protected override void BindCore(IConfigurationSection section)
    {
        var optionsSection = section.GetSection("Options");
        if (optionsSection.Exists())
        {
            Options ??= new FineTuningClientOptions(optionsSection);
        }
    }
}
