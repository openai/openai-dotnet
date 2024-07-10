//using OpenAI.Assistants;
//using System;
//using System.ClientModel;
//using System.ClientModel.Primitives;
//using System.Text.Json;
//using System.Threading.Tasks;

//#nullable enable

//namespace OpenAI;

//// This gets generated client-specific
//// There is an evolution story around this from protocol to convenience
//// When protocol-only, this inherits from OperationResultPoller
//// When conveniences are added, this inherits from OperationPoller<ThreadRun>.
//internal class ThreadRunPoller : OperationPoller<ThreadRun>
//{
//    private readonly ClientPipeline _pipeline;
//    private readonly Uri _endpoint;

//    public readonly string _threadId;
//    public readonly string _runId;

//    private readonly RequestOptions _options;

//    internal ThreadRunPoller(
//        ClientPipeline pipeline,
//        Uri endpoint,
//        ClientResult result,
//        string threadId,
//        string runId,
//        RequestOptions options) : base(result)
//    {
//        _pipeline = pipeline;
//        _endpoint = endpoint;

//        _threadId = threadId;
//        _runId = runId;

//        _options = options;
//    }

//    public override ThreadRun GetValueFromResult(ClientResult result)
//    {
//        PipelineResponse response = result.GetRawResponse();
//        return ThreadRun.FromResponse(response);
//    }

//    // Poller subclient method implementations
//    public override async Task<ClientResult> UpdateStatusAsync()
//        => await GetRunAsync(_options).ConfigureAwait(false);

//    public override ClientResult UpdateStatus()
//        => GetRun(_options);

//    public override bool HasStopped(ClientResult result)
//    {
//        PipelineResponse response = result.GetRawResponse();

//        using JsonDocument doc = JsonDocument.Parse(response.Content);
//        string status = doc.RootElement.GetProperty("status"u8).GetString()!;

//        bool hasStopped =
//            status == "expired" ||
//            status == "completed" ||
//            status == "failed" ||
//            status == "incomplete" ||
//            status == "cancelled";

//        return hasStopped;
//    }

//    private static PipelineMessageClassifier? _pipelineMessageClassifier200;
//    private static PipelineMessageClassifier PipelineMessageClassifier200 => _pipelineMessageClassifier200 ??= PipelineMessageClassifier.Create(stackalloc ushort[] { 200 });
//}
