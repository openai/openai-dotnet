using System.Collections.Generic;
using System.Linq;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ResponsesUserMessageItemResource")]
public partial class UserMessageResponseItem
{
    // CUSTOM: Made internal and renamed to expose it via the base class.
    [CodeGenMember("Content")]
    internal IList<ResponseContentPart> InternalContent { get; }

    // CUSTOM: Renamed parameter.
    public UserMessageResponseItem(IEnumerable<ResponseContentPart> content) : base(InternalMessageRoleEx.User)
    {
        Argument.AssertNotNull(content, nameof(content));

        InternalContent = content.ToList();
    }

    // CUSTOM: Added as a convenience.
    public UserMessageResponseItem(string inputTextContent) : this([ResponseContentPart.CreateInputTextPart(inputTextContent)])
    {
    }
}