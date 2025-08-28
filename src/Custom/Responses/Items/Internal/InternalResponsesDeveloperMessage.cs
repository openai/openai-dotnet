using System.Collections.Generic;

namespace OpenAI.Responses;

[CodeGenType("ResponsesDeveloperMessageItemResource")]
internal partial class InternalResponsesDeveloperMessage
{
    // CUSTOM: Use generalized content type.
    [CodeGenMember("Content")]
    public IList<ResponseContentPart> InternalContent { get; }
}