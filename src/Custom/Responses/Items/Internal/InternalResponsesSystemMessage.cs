using System.Collections.Generic;

namespace OpenAI.Responses;

[CodeGenType("ResponsesSystemMessageItemResource")]
internal partial class InternalResponsesSystemMessage
{
    // CUSTOM: Use generalized content type.
    [CodeGenMember("Content")]
    public IList<ResponseContentPart> InternalContent { get; }

    // CUSTOM: For reuse as an input model
    internal InternalResponsesSystemMessage(IEnumerable<ResponseContentPart> internalContent)
        : this(id: null, status: null, internalContent)
    { }
}