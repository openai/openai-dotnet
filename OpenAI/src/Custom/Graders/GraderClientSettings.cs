using Microsoft.Extensions.Configuration;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Graders;

[Experimental("SCME0002")]
public sealed class GraderClientSettings : ClientSettings
{
    public GraderClientOptions Options { get; set; }

    protected override void BindCore(IConfigurationSection section)
    {
        var optionsSection = section.GetSection("Options");
        if (optionsSection.Exists())
        {
            Options ??= new GraderClientOptions(optionsSection);
        }
    }
}
