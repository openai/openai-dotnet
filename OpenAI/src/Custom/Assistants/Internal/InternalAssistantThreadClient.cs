using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Threading;

namespace OpenAI.Assistants;

[CodeGenType("Threads")]
[CodeGenSuppress("InternalAssistantThreadClient", typeof(ClientPipeline), typeof(ApiKeyCredential), typeof(Uri))]
[CodeGenSuppress("CreateThreadAsync", typeof(ThreadCreationOptions), typeof(CancellationToken))]
[CodeGenSuppress("CreateThread", typeof(ThreadCreationOptions), typeof(CancellationToken))]
[CodeGenSuppress("GetThreadAsync", typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("GetThread", typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("ModifyThreadAsync", typeof(string), typeof(ThreadModificationOptions), typeof(CancellationToken))]
[CodeGenSuppress("ModifyThread", typeof(string), typeof(ThreadModificationOptions), typeof(CancellationToken))]
[CodeGenSuppress("DeleteThreadAsync", typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("DeleteThread", typeof(string), typeof(CancellationToken))]
internal partial class InternalAssistantThreadClient
{
    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="InternalAssistantThreadClient"/>. </summary>
    /// <param name="credential"> The <see cref="ApiKeyCredential"/> to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="credential"/> is null. </exception>
    public InternalAssistantThreadClient(ApiKeyCredential credential) : this(credential, new AssistantClientOptions())
    {
    }

    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="InternalAssistantThreadClient"/>. </summary>
    /// <param name="credential"> The <see cref="ApiKeyCredential"/> to authenticate with the service. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="credential"/> is null. </exception>
    public InternalAssistantThreadClient(ApiKeyCredential credential, AssistantClientOptions options) : this(OpenAIClient.CreateApiKeyAuthenticationPolicy(credential), options)
    {
    }

    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="InternalAssistantThreadClient"/>. </summary>
    /// <param name="authenticationPolicy"> The authentication policy used to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="authenticationPolicy"/> is null. </exception>
    public InternalAssistantThreadClient(AuthenticationPolicy authenticationPolicy) : this(authenticationPolicy, new AssistantClientOptions())
    {
    }

    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="InternalAssistantThreadClient"/>. </summary>
    /// <param name="authenticationPolicy"> The authentication policy used to authenticate with the service. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="authenticationPolicy"/> is null. </exception>
    public InternalAssistantThreadClient(AuthenticationPolicy authenticationPolicy, AssistantClientOptions options)
    {
        Argument.AssertNotNull(authenticationPolicy, nameof(authenticationPolicy));
        options ??= new AssistantClientOptions();

        Pipeline = OpenAIClientUtilities.CreatePipeline(authenticationPolicy, options, options.UserAgentApplicationId, options.OrganizationId, options.ProjectId);
        _endpoint = OpenAIClientUtilities.GetEndpoint(options.Endpoint);
    }

    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    // - Made protected.
    /// <summary> Initializes a new instance of <see cref="InternalAssistantThreadClient"/>. </summary>
    /// <param name="pipeline"> The HTTP pipeline to send and receive REST requests and responses. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="pipeline"/> is null. </exception>
    protected internal InternalAssistantThreadClient(ClientPipeline pipeline, AssistantClientOptions options)
    {
        Argument.AssertNotNull(pipeline, nameof(pipeline));
        options ??= new AssistantClientOptions();

        Pipeline = pipeline;
        _endpoint = OpenAIClientUtilities.GetEndpoint(options.Endpoint);
    }
}
