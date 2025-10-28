using System.ClientModel.Primitives;

namespace OpenAI.Chat
{
    public partial class ResponseFormatJsonObject : ResponseFormat
    {
        public ResponseFormatJsonObject() : this(ResponseFormatType.JsonObject, default)
        {
        }

#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        internal ResponseFormatJsonObject(ResponseFormatType kind, in JsonPatch patch) : base(kind, patch)
        {
        }
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
    }
}