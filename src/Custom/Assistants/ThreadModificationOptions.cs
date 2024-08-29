using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

/// <summary>
/// Represents additional options available when modifying an existing <see cref="AssistantThread"/>.
/// </summary>
[Experimental("OPENAI001")]
[CodeGenModel("ModifyThreadRequest")]
public partial class ThreadModificationOptions
{
    // CUSTOM: reuse common request/response models for tool resources. Note that modification operations use the
    //          response models (which do not contain resource initialization helpers).

    /// <inheritdoc cref="ToolResources"/>
    [CodeGenMember("ToolResources")]
    public ToolResources ToolResources { get; set; }
}
