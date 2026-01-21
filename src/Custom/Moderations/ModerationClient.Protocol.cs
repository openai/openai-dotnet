using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Threading.Tasks;

namespace OpenAI.Moderations
{
    public partial class ModerationClient
    {
        // CUSTOM: Legacy. Retained for backward compatibility with previously GA'd API.
        public virtual ClientResult ClassifyText(BinaryContent content, RequestOptions options = null)
        {
            Argument.AssertNotNull(content, nameof(content));

            using PipelineMessage message = CreateClassifyInputsRequest(content, options);
            return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
        }

        /// CUSTOM: Legacy. Retained for backward compatibility with previously GA'd API.
        public virtual async Task<ClientResult> ClassifyTextAsync(BinaryContent content, RequestOptions options = null)
        {
            Argument.AssertNotNull(content, nameof(content));

            using PipelineMessage message = CreateClassifyInputsRequest(content, options);
            return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
        }
    }
}
