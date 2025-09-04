using System.Collections.Generic;

namespace OpenAI.Responses;

[CodeGenType("ResponsesDeveloperMessageItemResource")]
public partial class InternalResponsesDeveloperMessage
{
    // CUSTOM: Use generalized content type.
    [CodeGenMember("Content")]
    public IList<ResponseContentPart> InternalContent { get; }
}