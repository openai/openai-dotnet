using System.ClientModel.Primitives;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Chat
{
    [Experimental("OPENAI001")]
    public partial class ResponseFormat
    {
        [Experimental("SCME0001")]
        private JsonPatch _patch;

        internal protected ResponseFormat(ResponseFormatType kind)
        {
            Kind = kind;
        }

        internal ResponseFormat(ResponseFormatType kind, in JsonPatch patch)
        {
            Kind = kind;
            _patch = patch;
        }

        internal ResponseFormatType Kind { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch => ref _patch;
    }
}