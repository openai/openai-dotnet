using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OpenAI.Assistants;

[Experimental("OPENAI001")]
public partial class ThreadInitializationMessage : MessageCreationOptions
{
    /// <summary>
    /// Creates a new instance of <see cref="ThreadInitializationMessage"/>.
    /// </summary>
    /// <param name="content">
    /// The content items that should be included in the message, added to the thread being created.
    /// </param>
    public ThreadInitializationMessage(MessageRole role, IEnumerable<MessageContent> content) : base(null, null, role, content?.ToList(), null)
    {
        Argument.AssertNotNull(content, nameof(content));
    }

    internal ThreadInitializationMessage(MessageCreationOptions baseOptions)
        : base(baseOptions.Attachments, baseOptions.Metadata, baseOptions.Role, baseOptions.Content, null)
    { }

    internal ThreadInitializationMessage() : this(default, null)
    {
    }

    /// <summary>
    /// Implicitly creates a new instance of <see cref="ThreadInitializationMessage"/> from a single item of plain text
    /// content, assuming the role of <see cref="MessageRole.User"/>.
    /// </summary>
    /// <remarks>
    /// Using a <see cref="string"/> in the position of a <see cref="ThreadInitializationMessage"/> is equivalent to
    /// using the <see cref="ThreadInitializationMessage(MessageRole,IEnumerable{MessageContent})"/> constructor with
    /// <see cref="MessageRole.User"/> and a single <see cref="MessageContent.FromText(string)"/> content instance.
    /// </remarks>
    public static implicit operator ThreadInitializationMessage(string initializationMessage)
        => new(MessageRole.User, [initializationMessage]);
}
