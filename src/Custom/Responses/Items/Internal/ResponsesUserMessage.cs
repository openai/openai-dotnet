using System.Collections.Generic;

namespace OpenAI.Responses;

[CodeGenType("ResponsesUserMessageItemResource")]
public partial class ResponsesUserMessage
{
    // CUSTOM: Use generalized content type.
    [CodeGenMember("Content")]
    public IList<ResponseContentPart> Content { get; }

    // CUSTOM: For reuse as an input model
    internal ResponsesUserMessage(IEnumerable<ResponseContentPart> internalContent)
        : this(id: null, status: null, internalContent)
    { }
}