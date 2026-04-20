using Microsoft.TypeSpec.Generator.Customizations;
using Microsoft.Extensions.Configuration;
using System;

namespace OpenAI;

[CodeGenType("OpenAIClientSettings")]
[CodeGenSuppress("BindCore", typeof(IConfigurationSection))]
internal partial class InternalOpenAIClientSettings
{
    // CUSTOM: Override BindCore to avoid trying to instantiate abstract AuthenticationPolicy.
    protected override void BindCore(IConfigurationSection section)
    {
        if (Uri.TryCreate(section["Endpoint"], UriKind.Absolute, out Uri endpoint))
        {
            Endpoint = endpoint;
        }
        string apiKey = section["ApiKey"];
        if (!string.IsNullOrEmpty(apiKey))
        {
            ApiKey = apiKey;
        }
        IConfigurationSection optionsSection = section.GetSection("Options");
        if (optionsSection.Exists())
        {
            Options = new OpenAIClientOptions(optionsSection);
        }
    }
}
