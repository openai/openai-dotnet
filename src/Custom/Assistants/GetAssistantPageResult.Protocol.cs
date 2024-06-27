using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Text.Json;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.Assistants;

internal class GetAssistantProtocolPageResult : PageResult
{
    private readonly string? _lastId;

    private readonly Func<string?, Task<GetAssistantProtocolPageResult>> _getNextAsync;
    private readonly Func<string?, GetAssistantProtocolPageResult> _getNext;

    private GetAssistantProtocolPageResult(
        bool hasNext,
        string? lastId,
        PipelineResponse response,
        Func<string?, Task<GetAssistantProtocolPageResult>> getNextAsync,
        Func<string?, GetAssistantProtocolPageResult> getNext)
        : base(hasNext, response)
    {
        _lastId = lastId;

        _getNextAsync = getNextAsync;
        _getNext = getNext;
    }

    protected override async Task<PageResult> GetNextAsyncCore()
        => await _getNextAsync(_lastId).ConfigureAwait(false);

    protected override PageResult GetNextCore()
        => _getNext(_lastId);

    //public static GetAssistantProtocolPageResult Create(ClientResult result)
    //{
    //    PipelineResponse response = result.GetRawResponse();

    //    using JsonDocument doc = JsonDocument.Parse(response.Content);
    //    bool hasMore = doc.RootElement.GetProperty("has_more"u8).GetBoolean();
    //    string lastId = doc.RootElement.GetProperty("last_id"u8).GetString()!;

    //    return new(hasMore, lastId, response, getNextAsync, getNext);
    //}

    public static GetAssistantProtocolPageResult Create(ClientResult result,
        Func<string?, Task<GetAssistantProtocolPageResult>> getNextAsync,
        Func<string?, GetAssistantProtocolPageResult> getNext)
    {
        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        bool hasMore = doc.RootElement.GetProperty("has_more"u8).GetBoolean();
        string lastId = doc.RootElement.GetProperty("last_id"u8).GetString()!;

        return new(hasMore, lastId, response, getNextAsync, getNext);
    }
}

//private readonly string? _lastId;

//private readonly Func<ClientResult, GetAssistantProtocolPageResult> _getNext;
//private readonly Func<ClientResult, Task<GetAssistantProtocolPageResult>> _getNextAsync;

//public GetAssistantProtocolPageResult(
//    ClientResult result,
//    Func<ClientResult, GetAssistantProtocolPageResult> getNext,
//    Func<ClientResult, Task<GetAssistantProtocolPageResult>> getNextAsync)
//    : base(GetHasNext(result), result.GetRawResponse())
//{
//    _getNext = getNext;
//    _getNextAsync = getNextAsync;
//}

//protected override async Task<PageResult> GetNextAsyncCore()
//    => await _getNextAsync(_lastId).ConfigureAwait(false);

//protected override PageResult GetNextCore()
//    => _getNext();

//private static bool GetHasNext(ClientResult result)
//{
//    PipelineResponse response = result.GetRawResponse();
//    using JsonDocument doc = JsonDocument.Parse(response.Content);
//    return doc.RootElement.GetProperty("has_more"u8).GetBoolean();
//}

