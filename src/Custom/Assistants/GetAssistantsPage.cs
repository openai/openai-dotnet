using OpenAI.Assistants;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI;

// Question: can I write it in such a way that it calls through to the
// protocol mini client?


// Convenience method version
internal class GetAssistantsPage : PageResult<Assistant>
{
    private readonly Func<ContinuationToken, GetAssistantsPageToken> _getToken;
    private readonly GetAssistantsPageResult _protocolPageResult;

    private GetAssistantsPage(
        IReadOnlyList<Assistant> values,
        ContinuationToken pageToken,
        ContinuationToken? nextPageToken,
        Func<ContinuationToken, GetAssistantsPageToken> getToken,
        GetAssistantsPageResult protocolPageResult)
        : base(values, pageToken, nextPageToken, protocolPageResult.GetRawResponse())
    {
        _getToken = getToken;
        _protocolPageResult = protocolPageResult;
    }

    protected override async Task<PageResult> GetNextAsyncCore()
    {
        GetAssistantsPageResult nextPageResult = (GetAssistantsPageResult)await _protocolPageResult.GetNextAsync().ConfigureAwait(false);
        return FromProtocolPageResult(nextPageResult, _getToken(NextPageToken!), _getToken);
    }

    protected override PageResult GetNextCore()
    {
        GetAssistantsPageResult nextPageResult = (GetAssistantsPageResult)_protocolPageResult.GetNext();
        return FromProtocolPageResult(nextPageResult, _getToken(NextPageToken!), _getToken);
    }

    public static GetAssistantsPage FromProtocolPageResult(
        PageResult pageResult,
        GetAssistantsPageToken pageToken,
        Func<ContinuationToken, GetAssistantsPageToken> getToken)
    {
        GetAssistantsPageResult result = (GetAssistantsPageResult)pageResult;

        PipelineResponse response = result.GetRawResponse();
        InternalListAssistantsResponse list = ModelReaderWriter.Read<InternalListAssistantsResponse>(response.Content)!;
        OpenAIPageToken? nextPageToken = pageToken.GetNextPageToken(list.HasMore, list.LastId);

        return new GetAssistantsPage(list.Data, pageToken, nextPageToken, getToken, result);
    }

    //private readonly Func<ContinuationToken, GetAssistantsPageResult> _getNext;
    //private readonly Func<ContinuationToken, Task<GetAssistantsPageResult>> _getNextAsync;

    //public GetAssistantsPageResult(
    //    IReadOnlyList<Assistant> values,
    //    ContinuationToken pageToken,
    //    ContinuationToken? nextPageToken,
    //    PipelineResponse response,
    //    Func<ContinuationToken, GetAssistantsPageResult> getNext,
    //    Func<ContinuationToken, Task<GetAssistantsPageResult>> getNextAsync)
    //    : base(values, pageToken, nextPageToken, response)
    //{
    //    _getNext = getNext;
    //    _getNextAsync = getNextAsync;
    //}

    //protected override async Task<PageResult> GetNextAsyncCore()
    //    => await _getNextAsync(NextPageToken!).ConfigureAwait(false);

    //protected override PageResult GetNextCore()
    //    => _getNext(NextPageToken!);
}
