using OpenAI.Assistants;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI;

internal class GetAssistantsPageResult : PageResult<Assistant>
{
    private readonly Func<ContinuationToken, GetAssistantsPageResult> _getNext;
    private readonly Func<ContinuationToken, Task<GetAssistantsPageResult>> _getNextAsync;

    public GetAssistantsPageResult(
        IReadOnlyList<Assistant> values,
        ContinuationToken pageToken,
        ContinuationToken? nextPageToken,
        PipelineResponse response,
        Func<ContinuationToken, GetAssistantsPageResult> getNext,
        Func<ContinuationToken, Task<GetAssistantsPageResult>> getNextAsync)
        : base(values, pageToken, nextPageToken, response)
    {
        _getNext = getNext;
        _getNextAsync = getNextAsync;
    }

    protected override async Task<PageResult> GetNextAsyncCore()
        => await _getNextAsync(NextPageToken!).ConfigureAwait(false);

    protected override PageResult GetNextCore()
        => _getNext(NextPageToken!);
}
