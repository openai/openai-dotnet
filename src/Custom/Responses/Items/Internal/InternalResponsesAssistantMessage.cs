using System.Collections.Generic;

namespace OpenAI.Responses;

[CodeGenType("ResponsesAssistantMessageItemResource")]
internal partial class InternalResponsesAssistantMessage
{
    // CUSTOM: Use generalized content type.
    [CodeGenMember("Content")]
    public IList<ResponseContentPart> InternalContent { get; }

    // CUSTOM: For reuse as an input model
    internal InternalResponsesAssistantMessage(IEnumerable<ResponseContentPart> internalContent)
        : this(id: null, status: null, internalContent)
    { }
}