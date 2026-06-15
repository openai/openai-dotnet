using Microsoft.Extensions.Configuration;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Containers;

[Experimental("SCME0002")]
public sealed class ContainerClientSettings : ClientSettings
{
    public ContainerClientOptions Options { get; set; }

    protected override void BindCore(IConfigurationSection section)
    {
        var optionsSection = section.GetSection("Options");
        if (optionsSection.Exists())
        {
            Options ??= new ContainerClientOptions(optionsSection);
        }
    }
}
