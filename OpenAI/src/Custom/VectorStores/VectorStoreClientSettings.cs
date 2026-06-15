using Microsoft.Extensions.Configuration;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.VectorStores;

[Experimental("SCME0002")]
public sealed class VectorStoreClientSettings : ClientSettings
{
    public VectorStoreClientOptions Options { get; set; }

    protected override void BindCore(IConfigurationSection section)
    {
        var optionsSection = section.GetSection("Options");
        if (optionsSection.Exists())
        {
            Options ??= new VectorStoreClientOptions(optionsSection);
        }
    }
}
