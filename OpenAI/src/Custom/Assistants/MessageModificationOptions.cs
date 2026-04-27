using Microsoft.TypeSpec.Generator.Customizations;
using System.ClientModel;

namespace OpenAI.Assistants;

/// <summary>
/// Represents additional options available when modifying an existing <see cref="ThreadMessage"/>.
/// </summary>
[CodeGenType("ModifyMessageRequest")]
public partial class MessageModificationOptions
{
    internal BinaryContent ToBinaryContent() => BinaryContent.Create(this, ModelSerializationExtensions.WireOptions);
}
