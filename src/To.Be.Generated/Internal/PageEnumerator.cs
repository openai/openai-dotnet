//using System;
//using System.ClientModel;
//using System.Collections;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//#nullable enable

//namespace OpenAI;

//internal abstract class PageEnumerator<T> : IAsyncEnumerator<PageResult<T>>, IEnumerator<PageResult<T>>
//{
//    public PageEnumerator(PageResultEnumerator resultEnumerator)
//    {
//        ResultEnumerator = resultEnumerator;
//    }

//    public PageResultEnumerator ResultEnumerator { get; }

//    public abstract PageResult<T> GetPageFromResult(ClientResult result);

//    public PageResult<T> Current => GetPageFromResult(ResultEnumerator.Current);

//    #region IEnumerator<PageResult<T>> implementation

//    object IEnumerator.Current => Current;

//    public bool MoveNext() => ResultEnumerator.MoveNext();

//    public void Reset() => ResultEnumerator.Reset();

//    public void Dispose() => ResultEnumerator.Dispose();

//    #endregion

//    #region IAsyncEnumerator<PageResult<T>> implementation

//    public ValueTask<bool> MoveNextAsync() => ResultEnumerator.MoveNextAsync();

//    public ValueTask DisposeAsync() => ResultEnumerator.DisposeAsync();

//    #endregion
//}
//using System.ClientModel;
//using System.ClientModel.Primitives;

//#nullable enable

//namespace OpenAI.Assistants;

//internal partial class MessagesPageEnumerator : PageEnumerator<ThreadMessage>
//{
//    public MessagesPageEnumerator(MessagesPageResultEnumerator resultEnumerator) 
//        : base(resultEnumerator)
//    {
//    }

//    // Note: this is the deserialization method that converts protocol to convenience
//    public override PageResult<ThreadMessage> GetPageFromResult(ClientResult result)
//    {
//        PipelineResponse response = result.GetRawResponse();

//        InternalListMessagesResponse list = ModelReaderWriter.Read<InternalListMessagesResponse>(response.Content)!;

//        MessagesPageResultEnumerator resultEnumerator = (MessagesPageResultEnumerator)ResultEnumerator;

//        MessagesPageToken pageToken = MessagesPageToken.FromOptions(
//            resultEnumerator.
//            _threadId, _limit, _order, _after, _before);
//        MessagesPageToken? nextPageToken = pageToken.GetNextPageToken(list.HasMore, list.LastId);

//        return PageResult<ThreadMessage>.Create(list.Data, pageToken, nextPageToken, response);
//    }
//}
