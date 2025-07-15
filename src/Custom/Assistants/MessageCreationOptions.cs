using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

/// <summary>
/// Represents additional options available when creating a new <see cref="ThreadMessage"/>.
/// </summary>
[CodeGenType("CreateMessageRequest")]
[CodeGenVisibility(nameof(MessageCreationOptions), CodeGenVisibility.Public)]
[CodeGenSuppress(nameof(MessageCreationOptions), typeof(MessageRole), typeof(IEnumerable<MessageContent>))]
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

    internal BinaryContent ToBinaryContent() => BinaryContent.Create(this, ModelSerializationExtensions.WireOptions);
}
