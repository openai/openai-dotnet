using System;

namespace OpenAI.Assistants;

/// <summary>
/// Represents an item of image URL content within an Assistants API message.
/// </summary>
/// <remarks>
/// Use the <see cref="MessageContent.FromImageUrl(Uri,MessageImageDetail?)"/> method to
/// create an instance of this type.
/// </remarks>
[CodeGenType("MessageContentRefusalObject")]
internal partial class InternalMessageContentRefusalObject
{
    [CodeGenMember("Refusal")]
    public string InternalRefusal { get; set; }
}
