using System.Collections.Generic;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ResponsesMessageItemResource")]
public partial class MessageResponseItem
{
    // CUSTOM: Made internal in favor of a CLR enum.
    [CodeGenMember("Role")]
    internal InternalMessageRoleEx InternalRoleEx { get; set; }

    // CUSTOM: Made nullable since this is a read-only property.
    [CodeGenMember("Status")]
    public MessageStatus? Status { get; }

    // CUSTOM: Added to expose the content from derived types in the base type.
    public IList<ResponseContentPart> Content
        => (this as UserMessageResponseItem)?.InternalContent
        ?? (this as AssistantMessageResponseItem)?.InternalContent
        ?? (this as DeveloperMessageResponseItem)?.InternalContent
        ?? (this as SystemMessageResponseItem)?.InternalContent;

    // CUSTOM: Added to expose the message role as a CLR enum that can handle unknown values.
    public MessageRole Role
    {
        get
        {
            if (InternalRoleEx == InternalMessageRoleEx.Assistant)
            {
                return MessageRole.Assistant;
            }
            else if (InternalRoleEx == InternalMessageRoleEx.Developer)
            {
                return MessageRole.Developer;
            }
            else if (InternalRoleEx == InternalMessageRoleEx.System)
            {
                return MessageRole.System;
            }
            else if (InternalRoleEx == InternalMessageRoleEx.User)
            {
                return MessageRole.User;
            }
            else
            {
                return MessageRole.Unknown;
            }
        }
    }
}
