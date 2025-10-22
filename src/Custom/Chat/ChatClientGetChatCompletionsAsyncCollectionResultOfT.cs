using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenAI;

namespace OpenAI.Chat
{
    internal partial class ChatPMClientGetChatCompletionsAsyncCollectionResultOfT : AsyncCollectionResult<ChatCompletionResult>
    {
        private readonly ChatClient _client;
        private readonly string _after;
        private readonly int? _limit;
        private readonly string _order;
        private readonly IDictionary<string, string> _metadata;
        private readonly string _model;
        private readonly RequestOptions _options;

        public ChatPMClientGetChatCompletionsAsyncCollectionResultOfT(ChatClient client, string after, int? limit, string order, IDictionary<string, string> metadata, string model, RequestOptions options)
        {
            _client = client;
            _after = after;
            _limit = limit;
            _order = order;
            _metadata = metadata;
            _model = model;
            _options = options;
        }

        public override async IAsyncEnumerable<ClientResult> GetRawPagesAsync()
        {
            PipelineMessage message = _client.CreateGetChatCompletionsRequest(_after, _limit, _order, _metadata, _model, _options);
            string nextToken = null;
            while (true)
            {
                ClientResult result = ClientResult.FromResponse(await _client.Pipeline.ProcessMessageAsync(message, _options).ConfigureAwait(false));
                yield return result;

                // Plugin customization: add hasMore assignment
                bool hasMore = ((ChatCompletionList)result).HasMore;
                nextToken = ((ChatCompletionList)result).LastId;
                // Plugin customization: add hasMore == false check to pagination condition
                if (nextToken == null || !hasMore)
                {
                    yield break;
                }
                message = _client.CreateGetChatCompletionsRequest(nextToken, _limit, _order, _metadata, _model, _options);
            }
        }

        public override ContinuationToken GetContinuationToken(ClientResult page)
        {
            string nextPage = ((ChatCompletionList)page).LastId;
            if (nextPage != null)
            {
                return ContinuationToken.FromBytes(BinaryData.FromString(nextPage));
            }
            else
            {
                return null;
            }
        }

        protected override async IAsyncEnumerable<ChatCompletionResult> GetValuesFromPageAsync(ClientResult page)
        {
            foreach (ChatCompletionResult item in ((ChatCompletionList)page).Data)
            {
                yield return item;
                await Task.Yield();
            }
        }
    }
}