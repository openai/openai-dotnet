using System;
using System.ClientModel.Primitives;

namespace OpenAI;

/// <summary> The options to configure the client. </summary>
[CodeGenModel("OpenAIClientOptions")]
public partial class OpenAIClientOptions : ClientPipelineOptions
{
    private Uri _endpoint;
    private string _organizationId;
    private string _projectId;
    private string _applicationId;

    /// <summary>
    /// The service endpoint that the client will send requests to. If not set, the default endpoint will be used.
    /// </summary>
    public Uri Endpoint
    {
        get => _endpoint;
        set
        {
            AssertNotFrozen();
            _endpoint = value;
        }
    }

    /// <summary>
    /// The value to use for the <c>OpenAI-Organization</c> request header. Users who belong to multiple organizations
    /// can set this value to specify which organization is used for an API request. Usage from these API requests will
    /// count against the specified organization's quota. If not set, the header will be omitted, and the default
    /// organization will be billed. You can change your default organization in your user settings.
    /// <see href="https://platform.openai.com/docs/guides/production-best-practices/setting-up-your-organization">Learn more</see>.
    /// </summary>
    public string OrganizationId
    {
        get => _organizationId;
        set
        {
            AssertNotFrozen();
            _organizationId = value;
        }
    }

    /// <summary>
    /// The value to use for the <c>OpenAI-Project</c> request header. Users who are accessing their projects through
    /// their legacy user API key can set this value to specify which project is used for an API request. Usage from
    /// these API requests will count as usage for the specified project. If not set, the header will be omitted, and
    /// the default project will be accessed.
    /// </summary>
    public string ProjectId
    {
        get => _projectId;
        set
        {
            AssertNotFrozen();
            _projectId = value;
        }
    }

    /// <summary>
    /// An optional application ID to use as part of the request User-Agent header.
    /// </summary>
    public string ApplicationId
    {
        get => _applicationId;
        set
        {
            AssertNotFrozen();
            _applicationId = value;
        }
    }
}
