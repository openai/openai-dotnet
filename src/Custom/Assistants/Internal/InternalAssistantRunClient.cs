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
    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="InternalAssistantRunClient">. </summary>
    /// <param name="credential"> The API key to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="credential"/> is null. </exception>
    public InternalAssistantRunClient(ApiKeyCredential credential) : this(credential, new OpenAIClientOptions())
    {
    }

    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="InternalAssistantRunClient">. </summary>
    /// <param name="credential"> The API key to authenticate with the service. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="credential"/> is null. </exception>
    public InternalAssistantRunClient(ApiKeyCredential credential, OpenAIClientOptions options)
    {
        Argument.AssertNotNull(credential, nameof(credential));
        options ??= new OpenAIClientOptions();

        _pipeline = OpenAIClient.CreatePipeline(credential, options);
        _endpoint = OpenAIClient.GetEndpoint(options);
    }

    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    // - Made protected.
    /// <summary> Initializes a new instance of <see cref="InternalAssistantRunClient">. </summary>
    /// <param name="pipeline"> The HTTP pipeline to send and receive REST requests and responses. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="pipeline"/> is null. </exception>
    protected internal InternalAssistantRunClient(ClientPipeline pipeline, OpenAIClientOptions options)
    {
        Argument.AssertNotNull(pipeline, nameof(pipeline));
        options ??= new OpenAIClientOptions();

        _pipeline = pipeline;
        _endpoint = OpenAIClient.GetEndpoint(options);
    }
}
