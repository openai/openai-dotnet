using System.Collections.Generic;

namespace OpenAI.Assistants;

public partial class ThreadInitializationMessage : MessageCreationOptions
{
    /// <summary>
    /// Creates a new instance of <see cref="ThreadInitializationMessage"/>.
    /// </summary>
    /// <param name="content">
    /// The content items that should be included in the message, added to the thread being created.
    /// </param>
    public ThreadInitializationMessage(IEnumerable<MessageContent> content) : base(content)
    { }

    internal ThreadInitializationMessage(MessageCreationOptions baseOptions)
        : base(baseOptions.Role, baseOptions.Content, baseOptions.Attachments, baseOptions.Metadata, null)
    { }
}
