using System.Collections.Generic;

namespace OpenAI.Responses;

[CodeGenType("ResponsesAssistantMessageItemResource")]
public partial class InternalResponsesAssistantMessage
{
    // CUSTOM: Use generalized content type.
    [CodeGenMember("Content")]
    public IList<ResponseContentPart> InternalContent { get; }
}