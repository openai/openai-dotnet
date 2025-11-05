using System.ClientModel.Primitives;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Chat
{
    public partial class ChatCompletionStreamOptions
    {
        [Experimental("SCME0001")]
        private JsonPatch _patch;

        public ChatCompletionStreamOptions()
        {
        }

#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        internal ChatCompletionStreamOptions(bool? includeUsage, in JsonPatch patch)
        {
            IncludeUsage = includeUsage;
            _patch = patch;
        }
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch => ref _patch;

        public bool? IncludeUsage { get; set; }
    }
}