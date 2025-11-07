using System.Collections.Generic;
using System.Linq;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ResponsesAssistantMessageItemResource")]
public partial class AssistantMessageResponseItem
{
    // CUSTOM: Made internal and renamed to expose it via the base class.
    [CodeGenMember("Content")]
    internal IList<ResponseContentPart> InternalContent { get; }

    // CUSTOM: Renamed parameter.
    public AssistantMessageResponseItem(IEnumerable<ResponseContentPart> content) : base(InternalMessageRoleEx.Assistant)
    {
        Argument.AssertNotNull(content, nameof(content));

        InternalContent = content.ToList();
    }

    // CUSTOM: Added as a convenience.
    public AssistantMessageResponseItem(string outputTextContent) : this([ResponseContentPart.CreateOutputTextPart(outputTextContent, [])])
    {
    }
}