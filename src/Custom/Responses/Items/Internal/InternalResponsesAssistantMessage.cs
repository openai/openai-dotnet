using System.Collections.Generic;

namespace OpenAI.Responses;

[CodeGenType("ResponsesAssistantMessage")]
internal partial class InternalResponsesAssistantMessage
{
    // CUSTOM: Use generalized content type.
    [CodeGenMember("Content")]
    public IList<ResponseContentPart> InternalContent { get; }
}