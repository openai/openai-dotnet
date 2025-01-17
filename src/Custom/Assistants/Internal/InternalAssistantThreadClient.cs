using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Threading;

namespace OpenAI.Assistants;

[CodeGenClient("Threads")]
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
    /// <param name="credential"> The API key to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="credential"/> is null. </exception>
    public InternalAssistantThreadClient(ApiKeyCredential credential) : this(credential, new OpenAIClientOptions())
    {
    }

    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="InternalAssistantThreadClient"/>. </summary>
    /// <param name="credential"> The API key to authenticate with the service. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="credential"/> is null. </exception>
    public InternalAssistantThreadClient(ApiKeyCredential credential, OpenAIClientOptions options)
    {
        Argument.AssertNotNull(credential, nameof(credential));
        options ??= new OpenAIClientOptions();

        Pipeline = OpenAIClient.CreatePipeline(credential, options);
        _endpoint = OpenAIClient.GetEndpoint(options);
    }

    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    // - Made protected.
    /// <summary> Initializes a new instance of <see cref="InternalAssistantThreadClient"/>. </summary>
    /// <param name="pipeline"> The HTTP pipeline to send and receive REST requests and responses. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="pipeline"/> is null. </exception>
    protected internal InternalAssistantThreadClient(ClientPipeline pipeline, OpenAIClientOptions options)
    {
        Argument.AssertNotNull(pipeline, nameof(pipeline));
        options ??= new OpenAIClientOptions();

        Pipeline = pipeline;
        _endpoint = OpenAIClient.GetEndpoint(options);
    }
}
