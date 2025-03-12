using System.Collections.Generic;
using System.Linq;

namespace OpenAI.Responses;

[CodeGenType("ResponsesDeveloperMessage")]
[CodeGenSuppress(nameof(InternalResponsesDeveloperMessage), typeof(IEnumerable<ResponseContentPart>))]
internal partial class InternalResponsesDeveloperMessage
{
    // CUSTOM: Use generalized content type.
    [CodeGenMember("Content")]
    public IList<ResponseContentPart> InternalContent { get; }

    public InternalResponsesDeveloperMessage(IEnumerable<ResponseContentPart> internalContent) : base(MessageRole.Developer)
    {
        Argument.AssertNotNull(internalContent, nameof(internalContent));
        InternalContent = internalContent.ToList();
    }
}