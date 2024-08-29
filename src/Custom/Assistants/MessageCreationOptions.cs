using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

/// <summary>
/// Represents additional options available when creating a new <see cref="ThreadMessage"/>.
/// </summary>
[Experimental("OPENAI001")]
[CodeGenModel("CreateMessageRequest")]
[CodeGenSuppress("MessageCreationOptions", typeof(MessageRole), typeof(IEnumerable<MessageContent>))]
[CodeGenSerialization(nameof(Content), SerializationValueHook = nameof(SerializeContent))]
public partial class MessageCreationOptions
{
    // CUSTOM: role is hidden, as this required property is promoted to a method parameter

    [CodeGenMember("Role")]
    internal MessageRole Role { get; set; }

    // CUSTOM: content is hidden to allow the promotion of required request information into top-level
    //          method signatures.

    [CodeGenMember("Content")]
    internal IList<MessageContent> Content { get; }

    /// <summary>
    /// Creates a new instance of <see cref="MessageCreationOptions"/>.
    /// </summary>
    public MessageCreationOptions()
    : this(
        MessageRole.User,
        new ChangeTrackingList<MessageContent>(),
        new ChangeTrackingList<MessageCreationAttachment>(),
        new ChangeTrackingDictionary<string, string>(),
        new ChangeTrackingDictionary<string, BinaryData>())
    { }

    internal MessageCreationOptions(IEnumerable<MessageContent> content) : this()
    {
        Content = [.. content];
    }
}
