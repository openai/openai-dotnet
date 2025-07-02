using System.Collections.Generic;

namespace OpenAI.Responses;

[CodeGenType("ResponsesUserMessageItemResource")]
internal partial class InternalResponsesUserMessage
{
    // CUSTOM: Use generalized content type.
    [CodeGenMember("Content")]
    public IList<ResponseContentPart> InternalContent { get; }

    // CUSTOM: For reuse as an input model
    internal InternalResponsesUserMessage(IEnumerable<ResponseContentPart> internalContent)
        : this(id: null, status: null, internalContent)
    { }
}