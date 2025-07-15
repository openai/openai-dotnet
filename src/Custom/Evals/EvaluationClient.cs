using System.ClientModel;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace OpenAI.Evals;

// CUSTOM:
// - Renamed.
// - Suppressed constructor that takes endpoint parameter; endpoint is now a property in the options class.
// - Suppressed convenience methods for now.
/// <summary> The service client for OpenAI Evaluation operations. </summary>
[CodeGenType("Evals")]
[CodeGenSuppress("CreateEval", typeof(InternalCreateEvalRequest), typeof(CancellationToken))]
[CodeGenSuppress("CreateEvalAsync", typeof(InternalCreateEvalRequest), typeof(CancellationToken))]
[CodeGenSuppress("GetEval", typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("GetEvalAsync", typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("UpdateEval", typeof(string), typeof(InternalUpdateEvalRequest), typeof(CancellationToken))]
[CodeGenSuppress("UpdateEvalAsync", typeof(string), typeof(InternalUpdateEvalRequest), typeof(CancellationToken))]
[CodeGenSuppress("DeleteEval", typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("DeleteEvalAsync", typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("CreateEvalRun", typeof(string), typeof(InternalCreateEvalRunRequest), typeof(CancellationToken))]
[CodeGenSuppress("CreateEvalRunAsync", typeof(string), typeof(InternalCreateEvalRunRequest), typeof(CancellationToken))]
[CodeGenSuppress("GetEvalRun", typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("GetEvalRunAsync", typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("CancelEvalRun", typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("CancelEvalRunAsync", typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("DeleteEvalRun", typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("DeleteEvalRunAsync", typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("GetEvalRunOutputItem", typeof(string), typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("GetEvalRunOutputItemAsync", typeof(string), typeof(string), typeof(string), typeof(CancellationToken))]
public partial class EvaluationClient
{
    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="EvaluationClient"/>. </summary>
    /// <param name="apiKey"> The API key to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="apiKey"/> is null. </exception>
    public EvaluationClient(string apiKey) : this(new ApiKeyCredential(apiKey), new OpenAIClientOptions())
    {
    }

    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="EvaluationClient"/>. </summary>
    /// <param name="credential"> The API key to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="credential"/> is null. </exception>
    public EvaluationClient(ApiKeyCredential credential) : this(credential, new OpenAIClientOptions())
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="EvaluationClient"/> that will use an API key when authenticating.
    /// </summary>
    /// <param name="credential"> The API key used to authenticate with the service endpoint. </param>
    /// <param name="options"> Additional options to customize the client. </param>
    /// <exception cref="ArgumentNullException"> The provided <paramref name="credential"/> was null. </exception>
    public EvaluationClient(ApiKeyCredential credential, OpenAIClientOptions options)
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
    /// <summary> Initializes a new instance of <see cref="EvaluationClient"/>. </summary>
    /// <param name="pipeline"> The HTTP pipeline to send and receive REST requests and responses. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="pipeline"/> is null. </exception>
    protected internal EvaluationClient(ClientPipeline pipeline, OpenAIClientOptions options)
    {
        Argument.AssertNotNull(pipeline, nameof(pipeline));
        options ??= new OpenAIClientOptions();

        Pipeline = pipeline;
        _endpoint = OpenAIClient.GetEndpoint(options);
    }
}
