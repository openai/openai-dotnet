using System;

namespace OpenAI.Assistants;

/// <summary>
/// Represents an item of image file content within an Assistants API message.
/// </summary>
/// <remarks>
/// Use the <see cref="MessageContent.FromImageFileId(string,MessageImageDetail?)"/> method to
/// create an instance of this type.
/// </remarks>
[CodeGenType("MessageContentImageFileObject")]
internal partial class InternalMessageContentImageFileObject
{
}
