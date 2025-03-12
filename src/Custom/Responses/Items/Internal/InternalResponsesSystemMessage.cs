using System.Collections.Generic;
using System.Linq;

namespace OpenAI.Responses;

[CodeGenType("ResponsesSystemMessage")]
[CodeGenSuppress(nameof(InternalResponsesSystemMessage), typeof(IEnumerable<ResponseContentPart>))]
internal partial class InternalResponsesSystemMessage
{
    // CUSTOM: Use generalized content type.
    [CodeGenMember("Content")]
    public IList<ResponseContentPart> InternalContent { get; }

    public InternalResponsesSystemMessage(IEnumerable<ResponseContentPart> internalContent) : base(MessageRole.System)
    {
        Argument.AssertNotNull(internalContent, nameof(internalContent));
        InternalContent = internalContent.ToList();
    }
}