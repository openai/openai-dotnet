using System.ClientModel;
using System.ClientModel.Primitives;

#nullable enable

namespace OpenAI.Assistants;

internal partial class RunStepsPageEnumerator : PageResultEnumerator
{
    // Note: this is the deserialization method that converts protocol to convenience
    public PageResult<RunStep> GetPageFromResult(ClientResult result)
    {
        PipelineResponse response = result.GetRawResponse();

        InternalListRunStepsResponse list = ModelReaderWriter.Read<InternalListRunStepsResponse>(response.Content)!;

        RunStepsPageToken pageToken = RunStepsPageToken.FromOptions(_threadId, _runId, _limit, _order, _after, _before);
        RunStepsPageToken? nextPageToken = pageToken.GetNextPageToken(list.HasMore, list.LastId);

        return PageResult<RunStep>.Create(list.Data, pageToken, nextPageToken, response);
    }
}