using System.ClientModel;
using System.ClientModel.Primitives;

#nullable enable

namespace OpenAI.Assistants;

internal partial class MessagesPageEnumerator : PageResultEnumerator
{
    // Note: this is the deserialization method that converts protocol to convenience
    public PageResult<ThreadMessage> GetPageFromResult(ClientResult result)
    {
        PipelineResponse response = result.GetRawResponse();

        InternalListMessagesResponse list = ModelReaderWriter.Read<InternalListMessagesResponse>(response.Content)!;

        MessagesPageToken pageToken = MessagesPageToken.FromOptions(_threadId, _limit, _order, _after, _before);
        MessagesPageToken? nextPageToken = pageToken.GetNextPageToken(list.HasMore, list.LastId);

        return PageResult<ThreadMessage>.Create(list.Data, pageToken, nextPageToken, response);
    }
}