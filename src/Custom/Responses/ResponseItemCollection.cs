using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OpenAI.Responses
{
    [Experimental("OPENAI001")]
    public partial class ResponseItemCollection
    {
        [Experimental("SCME0001")]
        private JsonPatch _patch;

        internal ResponseItemCollection(IEnumerable<ResponseItem> data, bool hasMore, string firstId, string lastId)
        {
            Data = data.ToList();
            HasMore = hasMore;
            FirstId = firstId;
            LastId = lastId;
        }

#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        internal ResponseItemCollection(string @object, IList<ResponseItem> data, bool hasMore, string firstId, string lastId, in JsonPatch patch)
        {
            // Plugin customization: ensure initialization of collections
            Object = @object;
            Data = data ?? new ChangeTrackingList<ResponseItem>();
            HasMore = hasMore;
            FirstId = firstId;
            LastId = lastId;
            _patch = patch;
            _patch.SetPropagators(PropagateSet, PropagateGet);
        }
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch => ref _patch;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public string Object { get; } = "list";

        public IList<ResponseItem> Data { get; }

        public bool HasMore { get; }

        public string FirstId { get; }

        public string LastId { get; }
    }
}
