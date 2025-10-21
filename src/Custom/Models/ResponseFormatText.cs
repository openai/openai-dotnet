using System;
using System.ClientModel.Primitives;
using OpenAI.Chat;
using OpenAI.Internal;

namespace OpenAI
{
    internal partial class ResponseFormatText : ChatResponseFormat
    {
        public ResponseFormatText() : this(ResponseFormatType2.Text, default)
        {
        }

#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        internal ResponseFormatText(ResponseFormatType2 kind, in JsonPatch patch) : base(kind switch{ ResponseFormatType2.Text => InternalResponseFormatType.Text, ResponseFormatType2.JsonObject => InternalResponseFormatType.JsonObject, ResponseFormatType2.JsonSchema => InternalResponseFormatType.JsonSchema, _ => throw new InvalidOperationException("Unknown response format type") }, patch)
        {
        }
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
    }
}