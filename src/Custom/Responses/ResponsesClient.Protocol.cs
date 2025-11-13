using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenAI.Responses;

[CodeGenSuppress("GetResponse", typeof(string), typeof(string), typeof(IEnumerable<InternalIncludable>), typeof(bool?), typeof(int?), typeof(RequestOptions))]
[CodeGenSuppress("GetResponseAsync", typeof(string), typeof(string), typeof(IEnumerable<InternalIncludable>), typeof(bool?), typeof(int?), typeof(RequestOptions))]
[CodeGenSuppress("CancelResponse", typeof(string), typeof(IEnumerable<InternalIncludable>), typeof(bool?), typeof(int?), typeof(RequestOptions))]
[CodeGenSuppress("CancelResponseAsync", typeof(string), typeof(IEnumerable<InternalIncludable>), typeof(bool?), typeof(int?), typeof(RequestOptions))]
[CodeGenSuppress("GetResponse", typeof(string), typeof(IEnumerable<InternalIncludable>), typeof(bool?), typeof(int?), typeof(RequestOptions))]
[CodeGenSuppress("GetResponseAsync", typeof(string), typeof(IEnumerable<InternalIncludable>), typeof(bool?), typeof(int?), typeof(RequestOptions))]
public partial class ResponsesClient
{
    public virtual async Task<ClientResult> GetResponseAsync(string responseId, bool? stream, int? startingAfter, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(responseId, nameof(responseId));

        using PipelineMessage message = CreateGetResponseRequest(responseId, [], stream, startingAfter, options);

        PipelineResponse protocolResponse = await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false);
        return ClientResult.FromResponse(protocolResponse);
    }

    public virtual ClientResult GetResponse(string responseId, bool? stream, int? startingAfter, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(responseId, nameof(responseId));

        using PipelineMessage message = CreateGetResponseRequest(responseId, [], stream, startingAfter, options);
        PipelineResponse protocolResponse = Pipeline.ProcessMessage(message, options);
        return ClientResult.FromResponse(protocolResponse);
    }

    internal virtual PipelineMessage CreateGetResponseRequest(string responseId, IEnumerable<Includable> includables, bool? stream, int? startingAfter, RequestOptions options)
    {
        ClientUriBuilder uri = new ClientUriBuilder();
        uri.Reset(_endpoint);
        uri.AppendPath("/responses/", false);
        uri.AppendPath(responseId, true);
        if (includables != null && !(includables is ChangeTrackingList<Includable> changeTrackingList && changeTrackingList.IsUndefined))
        {
            foreach (var @param in includables)
            {
                uri.AppendQuery("include[]", @param.ToSerialString(), true);
            }
        }
        if (stream != null)
        {
            uri.AppendQuery("stream", TypeFormatters.ConvertToString(stream), true);
        }
        if (startingAfter != null)
        {
            uri.AppendQuery("starting_after", TypeFormatters.ConvertToString(startingAfter), true);
        }
        PipelineMessage message = Pipeline.CreateMessage(uri.ToUri(), "GET", PipelineMessageClassifier200);
        PipelineRequest request = message.Request;
        request.Headers.Set("Accept", "application/json, text/event-stream");
        message.Apply(options);
        return message;
    }
}