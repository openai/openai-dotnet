using System.ClientModel.Primitives;
using OpenAI.Chat;

namespace OpenAI
{
    internal partial class ResponseFormatJsonObject : ChatResponseFormat
    {
        public ResponseFormatJsonObject() : this(ResponseFormatType2.JsonObject, default)
        {
        }

#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        internal ResponseFormatJsonObject(ResponseFormatType2 kind, in JsonPatch patch) : base(kind, patch)
        {
        }
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
    }
}