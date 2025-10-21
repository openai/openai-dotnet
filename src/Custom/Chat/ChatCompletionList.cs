using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OpenAI.Chat
{
    [Experimental("OPENAI001")]
    public partial class ChatCompletionList2
    {
        [Experimental("SCME0001")]
        private JsonPatch _patch;

        internal ChatCompletionList2(IEnumerable<ChatCompletionResult> data, string firstId, string lastId, bool hasMore)
        {
            Data = data.ToList();
            FirstId = firstId;
            LastId = lastId;
            HasMore = hasMore;
        }

        internal ChatCompletionList2(string @object, IList<ChatCompletionResult> data, string firstId, string lastId, bool hasMore, in JsonPatch patch)
        {
            // Plugin customization: ensure initialization of collections
            Object = @object;
            Data = data ?? new ChangeTrackingList<ChatCompletionResult>();
            FirstId = firstId;
            LastId = lastId;
            HasMore = hasMore;
            _patch = patch;
            _patch.SetPropagators(PropagateSet, PropagateGet);
        }

        public string Object { get; } = "list";

        public IList<ChatCompletionResult> Data { get; }

        public string FirstId { get; }

        public string LastId { get; }

        public bool HasMore { get; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch => ref _patch;
    }
}