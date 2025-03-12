using System.Collections.Generic;
namespace OpenAI.Responses;

[CodeGenType("ResponsesMessage")]
[CodeGenSuppress("ResponsesMessage", typeof(MessageRole))]
public partial class MessageResponseItem
{
    // CUSTOM: Made public.
    [CodeGenMember("Role")]
    public MessageRole Role { get; }

    // CUSTOM: Recombined content from derived types.
    public IList<ResponseContentPart> Content
        => (this as InternalResponsesUserMessage)?.InternalContent
        ?? (this as InternalResponsesDeveloperMessage)?.InternalContent
        ?? (this as InternalResponsesSystemMessage)?.InternalContent
        ?? (this as InternalResponsesAssistantMessage)?.InternalContent;
}
