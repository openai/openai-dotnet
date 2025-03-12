using System.Collections.Generic;
using System.Linq;

namespace OpenAI.Responses;

[CodeGenType("ResponsesUserMessage")]
[CodeGenSuppress(nameof(InternalResponsesUserMessage), typeof(IEnumerable<ResponseContentPart>))]
internal partial class InternalResponsesUserMessage
{
    // CUSTOM: Use generalized content type.
    [CodeGenMember("Content")]
    public IList<ResponseContentPart> InternalContent { get; }

    public InternalResponsesUserMessage(IEnumerable<ResponseContentPart> internalContent) : base(MessageRole.User)
    {
        Argument.AssertNotNull(internalContent, nameof(internalContent));
        InternalContent = internalContent.ToList();
    }

}