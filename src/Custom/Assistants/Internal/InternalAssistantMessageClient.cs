using System;
using System.ClientModel;
using System.ClientModel.Primitives;

namespace OpenAI.Assistants;

[CodeGenClient("Messages")]
[CodeGenSuppress("InternalAssistantMessageClient", typeof(ClientPipeline), typeof(ApiKeyCredential), typeof(Uri))]
[CodeGenSuppress("CreateMessageAsync", typeof(string), typeof(MessageCreationOptions))]
[CodeGenSuppress("CreateMessage", typeof(string), typeof(MessageCreationOptions))]
[CodeGenSuppress("GetMessagesAsync", typeof(string), typeof(int?), typeof(ListOrder?), typeof(string), typeof(string))]
[CodeGenSuppress("GetMessages", typeof(string), typeof(int?), typeof(ListOrder?), typeof(string), typeof(string))]
[CodeGenSuppress("GetMessageAsync", typeof(string), typeof(string))]
[CodeGenSuppress("GetMessage", typeof(string), typeof(string))]
[CodeGenSuppress("ModifyMessageAsync", typeof(string), typeof(string), typeof(MessageModificationOptions))]
[CodeGenSuppress("ModifyMessage", typeof(string), typeof(string), typeof(MessageModificationOptions))]
[CodeGenSuppress("DeleteMessageAsync", typeof(string), typeof(string))]
[CodeGenSuppress("DeleteMessage", typeof(string), typeof(string))]
internal partial class InternalAssistantMessageClient
{
    /// <summary>
    /// Initializes a new instance of <see cref="InternalAssistantMessageClient"/> that will use an API key when authenticating.
    /// </summary>
    /// <param name="credential"> The API key used to authenticate with the service endpoint. </param>
    /// <param name="options"> Additional options to customize the client. </param>
    /// <exception cref="ArgumentNullException"> The provided <paramref name="credential"/> was null. </exception>
    public InternalAssistantMessageClient(ApiKeyCredential credential, OpenAIClientOptions options = default)
        : this(
              OpenAIClient.CreatePipeline(OpenAIClient.GetApiKey(credential, requireExplicitCredential: true), options),
              OpenAIClient.GetEndpoint(options),
              options)
    { }

    /// <summary>
    /// Initializes a new instance of <see cref="InternalAssistantMessageClient"/> that will use an API key from the OPENAI_API_KEY
    /// environment variable when authenticating.
    /// </summary>
    /// <remarks>
    /// To provide an explicit credential instead of using the environment variable, use an alternate constructor like
    /// <see cref="InternalAssistantMessageClient(ApiKeyCredential,OpenAIClientOptions)"/>.
    /// </remarks>
    /// <param name="options"> Additional options to customize the client. </param>
    /// <exception cref="InvalidOperationException"> The OPENAI_API_KEY environment variable was not found. </exception>
    public InternalAssistantMessageClient(OpenAIClientOptions options = default)
        : this(
              OpenAIClient.CreatePipeline(OpenAIClient.GetApiKey(), options),
              OpenAIClient.GetEndpoint(options),
              options)
    { }

    /// <summary> Initializes a new instance of <see cref="InternalAssistantMessageClient"/>. </summary>
    /// <param name="pipeline"> The HTTP pipeline for sending and receiving REST requests and responses. </param>
    /// <param name="endpoint"> OpenAI Endpoint. </param>
    protected internal InternalAssistantMessageClient(ClientPipeline pipeline, Uri endpoint, OpenAIClientOptions options)
    {
        _pipeline = pipeline;
        _endpoint = endpoint;
    }
}
