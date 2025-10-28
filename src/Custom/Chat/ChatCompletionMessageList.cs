using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using OpenAI;

namespace OpenAI.Chat
{
    public partial class ChatCompletionMessageList
    {
        [Experimental("SCME0001")]
        private JsonPatch _patch;

        internal ChatCompletionMessageList(IEnumerable<ChatCompletionMessageListDatum> data, string firstId, string lastId, bool hasMore)
        {
            Data = data.ToList();
            FirstId = firstId;
            LastId = lastId;
            HasMore = hasMore;
        }

#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        internal ChatCompletionMessageList(string @object, IList<ChatCompletionMessageListDatum> data, string firstId, string lastId, bool hasMore, in JsonPatch patch)
        {
            // Plugin customization: ensure initialization of collections
            Object = @object;
            Data = data ?? new ChangeTrackingList<ChatCompletionMessageListDatum>();
            FirstId = firstId;
            LastId = lastId;
            HasMore = hasMore;
            _patch = patch;
            _patch.SetPropagators(PropagateSet, PropagateGet);
        }
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch => ref _patch;

        public string Object { get; } = "list";

        public IList<ChatCompletionMessageListDatum> Data { get; }

        public string FirstId { get; }

        public string LastId { get; }

        public bool HasMore { get; }
    }
}
