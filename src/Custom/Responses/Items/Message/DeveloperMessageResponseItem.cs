using System.Collections.Generic;
using System.Linq;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ResponsesDeveloperMessageItemResource")]
public partial class DeveloperMessageResponseItem
{
    // CUSTOM: Made internal and renamed to expose it via the base class.
    [CodeGenMember("Content")]
    internal IList<ResponseContentPart> InternalContent { get; }

    // CUSTOM: Renamed parameter.
    public DeveloperMessageResponseItem(IEnumerable<ResponseContentPart> content) : base(InternalMessageRoleEx.Developer)
    {
        Argument.AssertNotNull(content, nameof(content));

        InternalContent = content.ToList();
    }

    // CUSTOM: Added as a convenience.
    public DeveloperMessageResponseItem(string inputTextContent) : this([ResponseContentPart.CreateInputTextPart(inputTextContent)])
    {
    }
}