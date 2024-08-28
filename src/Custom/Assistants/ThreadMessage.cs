using System.Collections.Generic;

namespace OpenAI.Assistants;

[CodeGenModel("MessageObject")]
public partial class ThreadMessage
{
    // CUSTOM: Made internal.
    /// <summary> The object type, which is always `thread.message`. </summary>
    [CodeGenMember("Object")]
    internal InternalMessageObjectObject Object { get; } = InternalMessageObjectObject.ThreadMessage;


    /// <inheritdoc cref="MessageRole"/>
    [CodeGenMember("Role")]
    public MessageRole Role { get; }

    /// <summary> A list of files attached to the message, and the tools they were added to. </summary>
    public IReadOnlyList<MessageCreationAttachment> Attachments { get; }
}
