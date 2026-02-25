using Microsoft.Extensions.Configuration;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Evals;

[Experimental("SCME0002")]
public sealed class EvaluationClientSettings : ClientSettings
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
