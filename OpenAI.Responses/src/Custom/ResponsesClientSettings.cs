using Microsoft.Extensions.Configuration;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

[Experimental("SCME0002")]
public sealed class ResponsesClientSettings : ClientSettings
{
    public ResponsesClientOptions Options { get; set; }

    protected override void BindCore(IConfigurationSection section)
    {
        var optionsSection = section.GetSection("Options");
        if (optionsSection.Exists())
        {
            Options ??= new ResponsesClientOptions(optionsSection);
        }
    }
}
