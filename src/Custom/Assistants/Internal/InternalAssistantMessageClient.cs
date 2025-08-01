using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Threading;

namespace OpenAI.Assistants;

[CodeGenType("Messages")]
[CodeGenSuppress("InternalAssistantMessageClient", typeof(ClientPipeline), typeof(ApiKeyCredential), typeof(Uri))]
[CodeGenSuppress("CreateMessageAsync", typeof(string), typeof(MessageCreationOptions), typeof(CancellationToken))]
[CodeGenSuppress("CreateMessage", typeof(string), typeof(MessageCreationOptions), typeof(CancellationToken))]
[CodeGenSuppress("GetMessagesAsync", typeof(string), typeof(int?), typeof(OpenAI.VectorStores.VectorStoreCollectionOrder?), typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("GetMessages", typeof(string), typeof(int?), typeof(OpenAI.VectorStores.VectorStoreCollectionOrder?), typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("GetMessageAsync", typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("GetMessage", typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("ModifyMessageAsync", typeof(string), typeof(string), typeof(MessageModificationOptions), typeof(CancellationToken))]
[CodeGenSuppress("ModifyMessage", typeof(string), typeof(string), typeof(MessageModificationOptions), typeof(CancellationToken))]
[CodeGenSuppress("DeleteMessageAsync", typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("DeleteMessage", typeof(string), typeof(string), typeof(CancellationToken))]
internal partial class InternalAssistantMessageClient
{
    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="InternalAssistantMessageClient"/>. </summary>
    /// <param name="credential"> The API key to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="credential"/> is null. </exception>
    public InternalAssistantMessageClient(ApiKeyCredential credential) : this(credential, new OpenAIClientOptions())
    {
    }

    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="InternalAssistantMessageClient"/>. </summary>
    /// <param name="credential"> The API key to authenticate with the service. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="credential"/> is null. </exception>
    public InternalAssistantMessageClient(ApiKeyCredential credential, OpenAIClientOptions options) : this(OpenAIClient.CreateApiKeyAuthenticationPolicy(credential), options)
    {
    }

    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="InternalAssistantMessageClient"/>. </summary>
    /// <param name="authenticationPolicy"> The authentication policy used to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="authenticationPolicy"/> is null. </exception>
    public InternalAssistantMessageClient(AuthenticationPolicy authenticationPolicy) : this(authenticationPolicy, new OpenAIClientOptions())
    {
    }

    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="InternalAssistantMessageClient"/>. </summary>
    /// <param name="authenticationPolicy"> The authentication policy used to authenticate with the service. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="authenticationPolicy"/> is null. </exception>
    public InternalAssistantMessageClient(AuthenticationPolicy authenticationPolicy, OpenAIClientOptions options)
    {
        Argument.AssertNotNull(authenticationPolicy, nameof(authenticationPolicy));
        options ??= new OpenAIClientOptions();

        Pipeline = OpenAIClient.CreatePipeline(authenticationPolicy, options);
        _endpoint = OpenAIClient.GetEndpoint(options);
    }

    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    // - Made protected.
    /// <summary> Initializes a new instance of <see cref="InternalAssistantMessageClient"/>. </summary>
    /// <param name="pipeline"> The HTTP pipeline to send and receive REST requests and responses. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="pipeline"/> is null. </exception>
    protected internal InternalAssistantMessageClient(ClientPipeline pipeline, OpenAIClientOptions options)
    {
        Argument.AssertNotNull(pipeline, nameof(pipeline));
        options ??= new OpenAIClientOptions();

        Pipeline = pipeline;
        _endpoint = OpenAIClient.GetEndpoint(options);
    }
}
