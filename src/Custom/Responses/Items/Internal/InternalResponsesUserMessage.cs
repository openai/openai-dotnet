using System.Collections.Generic;

namespace OpenAI.Responses;

[CodeGenType("ResponsesUserMessage")]
internal partial class InternalResponsesUserMessage
{
    // CUSTOM: Use generalized content type.
    [CodeGenMember("Content")]
    public IList<ResponseContentPart> InternalContent { get; }
}