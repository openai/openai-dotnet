using System.ClientModel.Primitives;
using OpenAI.Chat;

namespace OpenAI.Chat
{
    public partial class ResponseFormatText : ResponseFormat
    {
        public ResponseFormatText() : this(ResponseFormatType.Text, default)
        {
        }

#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        public ResponseFormatText(ResponseFormatType kind, in JsonPatch patch) : base(kind)
        {
        }
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
    }
}