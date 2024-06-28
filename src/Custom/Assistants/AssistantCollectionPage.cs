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
internal class AssistantCollectionPage : PageResult<Assistant>
{
    private readonly Func<ContinuationToken, AssistantCollectionPageToken> _getToken;
    private readonly AssistantCollectionPageResult _protocolPageResult;

    private AssistantCollectionPage(
        IReadOnlyList<Assistant> values,
        ContinuationToken pageToken,
        ContinuationToken? nextPageToken,
        Func<ContinuationToken, AssistantCollectionPageToken> getToken,
        AssistantCollectionPageResult protocolPageResult)
        : base(values, pageToken, nextPageToken, protocolPageResult.GetRawResponse())
    {
        _getToken = getToken;
        _protocolPageResult = protocolPageResult;
    }

    protected override async Task<PageResult> GetNextAsyncCore()
    {
        AssistantCollectionPageResult nextPageResult = (AssistantCollectionPageResult)await _protocolPageResult.GetNextAsync().ConfigureAwait(false);
        return FromProtocolPageResult(nextPageResult, _getToken(NextPageToken!), _getToken);
    }

    protected override PageResult GetNextCore()
    {
        AssistantCollectionPageResult nextPageResult = (AssistantCollectionPageResult)_protocolPageResult.GetNext();
        return FromProtocolPageResult(nextPageResult, _getToken(NextPageToken!), _getToken);
    }

    public static AssistantCollectionPage FromProtocolPageResult(
        PageResult pageResult,
        AssistantCollectionPageToken pageToken,
        Func<ContinuationToken, AssistantCollectionPageToken> getToken)
    {
        AssistantCollectionPageResult result = (AssistantCollectionPageResult)pageResult;

        PipelineResponse response = result.GetRawResponse();
        InternalListAssistantsResponse list = ModelReaderWriter.Read<InternalListAssistantsResponse>(response.Content)!;
        OpenAIPageToken? nextPageToken = pageToken.GetNextPageToken(list.HasMore, list.LastId);

        return new AssistantCollectionPage(list.Data, pageToken, nextPageToken, getToken, result);
    }
}
