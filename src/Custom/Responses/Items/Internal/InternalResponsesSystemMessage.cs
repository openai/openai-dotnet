using System.Collections.Generic;

namespace OpenAI.Responses;

[CodeGenType("ResponsesSystemMessage")]
internal partial class InternalResponsesSystemMessage
{
    // CUSTOM: Use generalized content type.
    [CodeGenMember("Content")]
    public IList<ResponseContentPart> InternalContent { get; }
}