using System.Collections.Generic;

namespace OpenAI.Responses;

[CodeGenType("ResponsesUserMessageItemResource")]
public partial class InternalResponsesUserMessage
{
    // CUSTOM: Use generalized content type.
    [CodeGenMember("Content")]
    public IList<ResponseContentPart> InternalContent { get; }
}