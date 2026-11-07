using Microsoft.Extensions.Configuration;
using System;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Decisions;

// CUSTOM: Added client-specific endpoint and header options with the standard pipeline freezing behavior.
/// <summary> The options to configure the <see cref="DecisionsClient"/>. </summary>
[Experimental("OPENAI001")]
public class DecisionsClientOptions : ClientPipelineOptions
{
    private Uri _endpoint;
    private string _organizationId;
    private string _projectId;
    private string _userAgentApplicationId;

    /// <summary> The service endpoint that the client will send requests to. If not set, the default endpoint will be used. </summary>
    public Uri Endpoint
    {
        get => _endpoint;
        set
        {
            AssertNotFrozen();
            _endpoint = value;
        }
    }

    /// <summary> The value to use for the <c>OpenAI-Organization</c> request header. If not set, the default organization will be billed. </summary>
    public string OrganizationId
    {
        get => _organizationId;
        set
        {
            AssertNotFrozen();
            _organizationId = value;
        }
    }

    /// <summary> The value to use for the <c>OpenAI-Project</c> request header. If not set, the default project will be accessed. </summary>
    public string ProjectId
    {
        get => _projectId;
        set
        {
            AssertNotFrozen();
            _projectId = value;
        }
    }

    /// <summary> An optional application ID to use as part of the request User-Agent header. </summary>
    public string UserAgentApplicationId
    {
        get => _userAgentApplicationId;
        set
        {
            AssertNotFrozen();
            _userAgentApplicationId = value;
        }
    }

    public DecisionsClientOptions()
    {
    }

    // CUSTOM: Bind client-specific options from the nested configuration section.
    [Experimental("SCME0002")]
    internal DecisionsClientOptions(IConfigurationSection section) : base(section)
    {
        if (Uri.TryCreate(section[nameof(Endpoint)], UriKind.Absolute, out Uri endpoint))
        {
            Endpoint = endpoint;
        }

        if (section[nameof(OrganizationId)] is string organizationId)
        {
            OrganizationId = organizationId;
        }

        if (section[nameof(ProjectId)] is string projectId)
        {
            ProjectId = projectId;
        }

        if (section[nameof(UserAgentApplicationId)] is string userAgentApplicationId)
        {
            UserAgentApplicationId = userAgentApplicationId;
        }
    }
}
