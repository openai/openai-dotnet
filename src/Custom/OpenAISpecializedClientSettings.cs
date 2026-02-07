using Microsoft.Extensions.Configuration;
using System;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI;

[Experimental("SCME0002")]
public abstract class OpenAISpecializedClientSettings : ClientSettings
{
    public string Model { get; set; }

    public OpenAIClientOptions Options { get; set; } = new OpenAIClientOptions();

    protected override void BindCore(IConfigurationSection section)
    {
        Model = section["Model"];

        var optionsSection = section.GetSection("Options");
        if (optionsSection.Exists())
        {
            Options ??= new OpenAIClientOptions();

            var organizationId = optionsSection["OrganizationId"];
            if (!string.IsNullOrEmpty(organizationId))
            {
                Options.OrganizationId = organizationId;
            }

            var projectId = optionsSection["ProjectId"];
            if (!string.IsNullOrEmpty(projectId))
            {
                Options.ProjectId = projectId;
            }

            var userAgentApplicationId = optionsSection["UserAgentApplicationId"];
            if (!string.IsNullOrEmpty(userAgentApplicationId))
            {
                Options.UserAgentApplicationId = userAgentApplicationId;
            }

            var endpointValue = optionsSection["Endpoint"];
            if (!string.IsNullOrEmpty(endpointValue) && Uri.TryCreate(endpointValue, UriKind.Absolute, out var endpointUri))
            {
                Options.Endpoint = endpointUri;
            }
        }
    }
}