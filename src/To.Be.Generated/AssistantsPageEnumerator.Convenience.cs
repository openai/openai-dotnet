using System.ClientModel;
using System.ClientModel.Primitives;

#nullable enable

namespace OpenAI.Assistants;

internal partial class AssistantsPageEnumerator : PageResultEnumerator
{
    public PageResult<Assistant> GetPageFromResult(ClientResult result)
    {
        PipelineResponse response = result.GetRawResponse();

        InternalListAssistantsResponse list = ModelReaderWriter.Read<InternalListAssistantsResponse>(response.Content)!;

        AssistantsPageToken pageToken = AssistantsPageToken.FromOptions(_limit, _order, _after, _before);
        AssistantsPageToken? nextPageToken = pageToken.GetNextPageToken(list.HasMore, list.LastId);

        return PageResult<Assistant>.Create(list.Data, pageToken, nextPageToken, response);
    }
}