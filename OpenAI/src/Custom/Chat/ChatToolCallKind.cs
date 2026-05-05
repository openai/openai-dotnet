using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Chat;

[CodeGenType("ChatToolCallKind")]
public enum ChatToolCallKind
{
    Function,
}
