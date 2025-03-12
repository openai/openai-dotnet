using System.Collections.Generic;
using System.Linq;

namespace OpenAI.Responses;

[CodeGenType("ResponsesAssistantMessage")]
[CodeGenSuppress(nameof(InternalResponsesAssistantMessage), typeof(IEnumerable<ResponseContentPart>))]
internal partial class InternalResponsesAssistantMessage
{
    // CUSTOM: Use generalized content type.
    [CodeGenMember("Content")]
    public IList<ResponseContentPart> InternalContent { get; }

    public InternalResponsesAssistantMessage(IEnumerable<ResponseContentPart> internalContent) : base(MessageRole.Assistant)
    {
        Argument.AssertNotNull(internalContent, nameof(internalContent));
        InternalContent = internalContent.ToList();
    }
}