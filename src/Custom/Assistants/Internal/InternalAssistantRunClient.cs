using System;
using System.ClientModel;
using System.ClientModel.Primitives;

namespace OpenAI.Assistants;

[CodeGenClient("Runs")]
[CodeGenSuppress("InternalAssistantRunClient", typeof(ClientPipeline), typeof(ApiKeyCredential), typeof(Uri))]
[CodeGenSuppress("CreateThreadAndRunAsync", typeof(InternalCreateThreadAndRunRequest))]
[CodeGenSuppress("CreateThreadAndRun", typeof(InternalCreateThreadAndRunRequest))]
[CodeGenSuppress("CreateRunAsync", typeof(string), typeof(RunCreationOptions))]
[CodeGenSuppress("CreateRun", typeof(string), typeof(RunCreationOptions))]
[CodeGenSuppress("GetRunsAsync", typeof(string), typeof(int?), typeof(ListOrder?), typeof(string), typeof(string))]
[CodeGenSuppress("GetRuns", typeof(string), typeof(int?), typeof(ListOrder?), typeof(string), typeof(string))]
[CodeGenSuppress("GetRunAsync", typeof(string), typeof(string))]
[CodeGenSuppress("GetRun", typeof(string), typeof(string))]
[CodeGenSuppress("ModifyRunAsync", typeof(string), typeof(string), typeof(RunModificationOptions))]
[CodeGenSuppress("ModifyRun", typeof(string), typeof(string), typeof(RunModificationOptions))]
[CodeGenSuppress("CancelRunAsync", typeof(string), typeof(string))]
[CodeGenSuppress("CancelRun", typeof(string), typeof(string))]
[CodeGenSuppress("SubmitToolOutputsToRunAsync", typeof(string), typeof(string), typeof(InternalSubmitToolOutputsRunRequest))]
[CodeGenSuppress("SubmitToolOutputsToRun", typeof(string), typeof(string), typeof(InternalSubmitToolOutputsRunRequest))]
[CodeGenSuppress("GetRunStepsAsync", typeof(string), typeof(string), typeof(int?), typeof(ListOrder?), typeof(string), typeof(string))]
[CodeGenSuppress("GetRunSteps", typeof(string), typeof(string), typeof(int?), typeof(ListOrder?), typeof(string), typeof(string))]
[CodeGenSuppress("GetRunStepAsync", typeof(string), typeof(string), typeof(string))]
[CodeGenSuppress("GetRunStep", typeof(string), typeof(string), typeof(string))]
internal partial class InternalAssistantRunClient
{
    /// <summary>
    /// Initializes a new instance of <see cref="InternalAssistantRunClient"/> that will use an API key when authenticating.
    /// </summary>
    /// <param name="credential"> The API key used to authenticate with the service endpoint. </param>
    /// <param name="options"> Additional options to customize the client. </param>
    /// <exception cref="ArgumentNullException"> The provided <paramref name="credential"/> was null. </exception>
    public InternalAssistantRunClient(ApiKeyCredential credential, OpenAIClientOptions options = default)
        : this(
              OpenAIClient.CreatePipeline(OpenAIClient.GetApiKey(credential, requireExplicitCredential: true), options),
              OpenAIClient.GetEndpoint(options),
              options)
    { }

    /// <summary>
    /// Initializes a new instance of <see cref="InternalAssistantRunClient"/> that will use an API key from the OPENAI_API_KEY
    /// environment variable when authenticating.
    /// </summary>
    /// <remarks>
    /// To provide an explicit credential instead of using the environment variable, use an alternate constructor like
    /// <see cref="InternalAssistantRunClient(ApiKeyCredential,OpenAIClientOptions)"/>.
    /// </remarks>
    /// <param name="options"> Additional options to customize the client. </param>
    /// <exception cref="InvalidOperationException"> The OPENAI_API_KEY environment variable was not found. </exception>
    public InternalAssistantRunClient(OpenAIClientOptions options = default)
        : this(
              OpenAIClient.CreatePipeline(OpenAIClient.GetApiKey(), options),
              OpenAIClient.GetEndpoint(options),
              options)
    { }

    /// <summary> Initializes a new instance of <see cref="InternalAssistantRunClient"/>. </summary>
    /// <param name="pipeline"> The HTTP pipeline for sending and receiving REST requests and responses. </param>
    /// <param name="endpoint"> OpenAI Endpoint. </param>
    /// <param name="options"> Client-wide options to propagate settings from. </param>
    protected internal InternalAssistantRunClient(ClientPipeline pipeline, Uri endpoint, OpenAIClientOptions options)
    {
        _pipeline = pipeline;
        _endpoint = endpoint;
    }
}
