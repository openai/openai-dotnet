using OpenAI.Assistants;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI;

#pragma warning disable OPENAI001

internal class AssistantPage : PageResult<Assistant>
{
    private readonly Func<ContinuationToken, AssistantPage> _getNextPage;
    private readonly Func<ContinuationToken, Task<AssistantPage>> _getNextPageAsync;

    public AssistantPage(
        IReadOnlyList<Assistant> values,
        ContinuationToken pageToken,
        ContinuationToken? nextPageToken,
        PipelineResponse response,
        Func<ContinuationToken, AssistantPage> getNextPage,
        Func<ContinuationToken, Task<AssistantPage>> getNextPageAsync)
        : base(values, pageToken, nextPageToken, response)
    {
        _getNextPage = getNextPage;
        _getNextPageAsync = getNextPageAsync;
    }

    protected override PageResult<Assistant> GetNextPage()
    {
        if (NextPageToken is null)
        {
            throw new InvalidOperationException("Cannot get next page when there is no NextPageToken available.");
        }

        return _getNextPage(NextPageToken);
    }

    protected override async Task<PageResult<Assistant>> GetNextPageAsync()
    {
        if (NextPageToken is null)
        {
            throw new InvalidOperationException("Cannot get next page when there is no NextPageToken available.");
        }

        return await _getNextPageAsync(NextPageToken).ConfigureAwait(false);
    }
}
#pragma warning restore OPENAI001