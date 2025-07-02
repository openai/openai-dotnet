using System;

namespace OpenAI.Assistants;

/// <summary>
/// Represents an item of image URL content within an Assistants API message.
/// </summary>
/// <remarks>
/// Use the <see cref="MessageContent.FromImageUrl(Uri,MessageImageDetail?)"/> method to
/// create an instance of this type.
/// </remarks>
[CodeGenType("MessageContentImageUrlObject")]
internal partial class InternalMessageContentImageUrlObject
{
}
