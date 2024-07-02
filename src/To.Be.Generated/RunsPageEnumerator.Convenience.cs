using System.ClientModel;
using System.ClientModel.Primitives;

#nullable enable

namespace OpenAI.Assistants;

internal partial class RunsPageEnumerator : PageResultEnumerator
{
    // Note: this is the deserialization method that converts protocol to convenience
    public PageResult<ThreadRun> GetPageFromResult(ClientResult result)
    {
        PipelineResponse response = result.GetRawResponse();

        InternalListRunsResponse list = ModelReaderWriter.Read<InternalListRunsResponse>(response.Content)!;

        RunsPageToken pageToken = RunsPageToken.FromOptions(_threadId, _limit, _order, _after, _before);
        RunsPageToken? nextPageToken = pageToken.GetNextPageToken(list.HasMore, list.LastId);

        return PageResult<ThreadRun>.Create(list.Data, pageToken, nextPageToken, response);
    }
}