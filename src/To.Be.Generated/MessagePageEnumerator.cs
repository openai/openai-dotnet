using System;

#nullable enable

using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;

namespace OpenAI.Assistants;

internal class MessagePageEnumerator : PageEnumerator<ThreadMessage>
{
    public MessagePageEnumerator(IEnumerator<ClientResult> resultEnumerator)
        : base(resultEnumerator)
    {
    }

    public override PageResult<ThreadMessage> GetPageFromResult(ClientResult result)
    {
        PipelineResponse response = result.GetRawResponse();
        InternalListMessagesResponse list = ModelReaderWriter.Read<InternalListMessagesResponse>(response.Content)!;

        MessagePageResultEnumerator resultEnumerator = (MessagePageResultEnumerator)ResultEnumerator;

        MessageCollectionPageToken pageToken = MessageCollectionPageToken.FromOptions(
            resultEnumerator.ThreadId,
            resultEnumerator.Limit,
            resultEnumerator.Order,
            resultEnumerator.After,
            resultEnumerator.Before);

        MessageCollectionPageToken? nextPageToken = pageToken.GetNextPageToken(list.HasMore, list.LastId);

        return PageResult<ThreadMessage>.Create(list.Data, pageToken, nextPageToken, response);
    }
}
