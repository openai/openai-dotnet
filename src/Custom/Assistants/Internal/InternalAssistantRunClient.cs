using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Threading;

namespace OpenAI.Assistants;

[CodeGenType("Runs")]
[CodeGenSuppress("InternalAssistantRunClient", typeof(ClientPipeline), typeof(ApiKeyCredential), typeof(Uri))]
[CodeGenSuppress("CreateThreadAndRunAsync", typeof(InternalCreateThreadAndRunRequest), typeof(CancellationToken))]
[CodeGenSuppress("CreateThreadAndRun", typeof(InternalCreateThreadAndRunRequest), typeof(CancellationToken))]
[CodeGenSuppress("CreateRunAsync", typeof(string), typeof(RunCreationOptions), typeof(IEnumerable<InternalIncludedRunStepProperty>), typeof(CancellationToken))]
[CodeGenSuppress("CreateRun", typeof(string), typeof(RunCreationOptions), typeof(IEnumerable<InternalIncludedRunStepProperty>), typeof(CancellationToken))]
[CodeGenSuppress("GetRunsAsync", typeof(string), typeof(int?), typeof(OpenAI.VectorStores.VectorStoreCollectionOrder?), typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("GetRuns", typeof(string), typeof(int?), typeof(OpenAI.VectorStores.VectorStoreCollectionOrder?), typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("GetRunAsync", typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("GetRun", typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("ModifyRunAsync", typeof(string), typeof(string), typeof(RunModificationOptions), typeof(CancellationToken))]
[CodeGenSuppress("ModifyRun", typeof(string), typeof(string), typeof(RunModificationOptions), typeof(CancellationToken))]
[CodeGenSuppress("CancelRunAsync", typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("CancelRun", typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("SubmitToolOutputsToRunAsync", typeof(string), typeof(string),  typeof(InternalSubmitToolOutputsRunRequest), typeof(CancellationToken))]
[CodeGenSuppress("SubmitToolOutputsToRun", typeof(string), typeof(string),  typeof(InternalSubmitToolOutputsRunRequest), typeof(CancellationToken))]
[CodeGenSuppress("GetRunStepsAsync", typeof(string), typeof(string), typeof(int?), typeof(OpenAI.VectorStores.VectorStoreCollectionOrder?), typeof(string), typeof(string), typeof(IEnumerable<InternalIncludedRunStepProperty>), typeof(CancellationToken))]
[CodeGenSuppress("GetRunSteps", typeof(string), typeof(string), typeof(int?), typeof(OpenAI.VectorStores.VectorStoreCollectionOrder?), typeof(string), typeof(string), typeof(IEnumerable<InternalIncludedRunStepProperty>), typeof(CancellationToken))]
[CodeGenSuppress("GetRunStepAsync", typeof(string), typeof(string), typeof(string), typeof(IEnumerable<InternalIncludedRunStepProperty>), typeof(CancellationToken))]
[CodeGenSuppress("GetRunStep", typeof(string), typeof(string), typeof(string),  typeof(IEnumerable<InternalIncludedRunStepProperty>), typeof(CancellationToken))]
internal partial class InternalAssistantRunClient
{
    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="InternalAssistantRunClient"/>. </summary>
    /// <param name="credential"> The API key to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="credential"/> is null. </exception>
    public InternalAssistantRunClient(ApiKeyCredential credential) : this(credential, new OpenAIClientOptions())
    {
    }

    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="InternalAssistantRunClient"/>. </summary>
    /// <param name="credential"> The API key to authenticate with the service. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="credential"/> is null. </exception>
    public InternalAssistantRunClient(ApiKeyCredential credential, OpenAIClientOptions options)
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
    /// <summary> Initializes a new instance of <see cref="InternalAssistantRunClient"/>. </summary>
    /// <param name="pipeline"> The HTTP pipeline to send and receive REST requests and responses. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="pipeline"/> is null. </exception>
    protected internal InternalAssistantRunClient(ClientPipeline pipeline, OpenAIClientOptions options)
    {
        Argument.AssertNotNull(pipeline, nameof(pipeline));
        options ??= new OpenAIClientOptions();

        Pipeline = pipeline;
        _endpoint = OpenAIClient.GetEndpoint(options);
    }
}
