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

    /// <summary>
    /// Implicitly creates a new instance of <see cref="ThreadInitializationMessage"/> from a single item of plain text
    /// content.
    /// </summary>
    /// <remarks>
    /// Using a <see cref="string"/> in the position of a <see cref="ThreadInitializationMessage"/> is equivalent to
    /// using the <see cref="ThreadInitializationMessage(IEnumerable{MessageContent})"/> constructor with a single
    /// <see cref="MessageContent.FromText(string)"/> content instance.
    /// </remarks>
    public static implicit operator ThreadInitializationMessage(string initializationMessage) => new([initializationMessage]);
}
