using System.Collections.Generic;

namespace OpenAI.Responses;

[CodeGenType("ResponsesSystemMessageItemResource")]
public partial class InternalResponsesSystemMessage
{
    // CUSTOM: Use generalized content type.
    [CodeGenMember("Content")]
    public IList<ResponseContentPart> InternalContent { get; }
}