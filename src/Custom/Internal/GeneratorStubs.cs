using Microsoft.TypeSpec.Generator.Customizations;
using Microsoft.Extensions.Configuration;
using System;

namespace OpenAI;

[CodeGenType("OpenAIClientSettings")]
[CodeGenSuppress("BindCore", typeof(IConfigurationSection))]
internal partial class InternalOpenAIClientSettings
{
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
        // AuthenticationPolicy is abstract and cannot be directly instantiated from configuration.
        // Authentication is handled via ApiKey or custom policies at the client level.
        IConfigurationSection optionsSection = section.GetSection("Options");
        if (optionsSection.Exists())
        {
            Options = new OpenAIClientOptions(optionsSection);
        }
    }
}
