using System.Collections.Generic;
using System.Linq;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ResponsesSystemMessageItemResource")]
public partial class SystemMessageResponseItem
{
    // CUSTOM: Made internal and renamed to expose it via the base class.
    [CodeGenMember("Content")]
    internal IList<ResponseContentPart> InternalContent { get; }

    // CUSTOM: Renamed parameter.
    public SystemMessageResponseItem(IEnumerable<ResponseContentPart> content) : base(InternalMessageRoleEx.System)
    {
        Argument.AssertNotNull(content, nameof(content));

        InternalContent = content.ToList();
    }

    // CUSTOM: Added as a convenience.
    public SystemMessageResponseItem(string inputTextContent) : this([ResponseContentPart.CreateInputTextPart(inputTextContent)])
    {
    }
}