using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses
{
    public partial class StreamingResponseFailedUpdate : StreamingResponseUpdate
    {
        internal StreamingResponseFailedUpdate(int sequenceNumber, ResponseResult response) : base(InternalResponseStreamEventType.ResponseFailed, sequenceNumber)
        {
            Response = response;
        }

#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        internal StreamingResponseFailedUpdate(InternalResponseStreamEventType kind, int sequenceNumber, in JsonPatch patch, ResponseResult response) : base(kind, sequenceNumber, patch)
        {
            Response = response;
        }
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

        public ResponseResult Response { get; }
    }
}
