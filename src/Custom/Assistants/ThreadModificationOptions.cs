namespace OpenAI.Assistants;

/// <summary>
/// Represents additional options available when modifying an existing <see cref="AssistantThread"/>.
/// </summary>
[CodeGenModel("ModifyThreadRequest")]
public partial class ThreadModificationOptions
{
    // CUSTOM: reuse common request/response models for tool resources. Note that modification operations use the
    //          response models (which do not contain resource initialization helpers).

    /// <inheritdoc cref="ToolResources"/>
    [CodeGenMember("ToolResources")]
    public ToolResources ToolResources { get; init; }
}