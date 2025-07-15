using OpenAI.Chat;
using System.ClientModel;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

/// <summary>
/// Represents additional options available when modifying an existing <see cref="Assistant"/>.
/// </summary>
[CodeGenType("ModifyAssistantRequest")]
public partial class AssistantModificationOptions
{
    /// <summary>
    /// The replacement model that the assistant should use.
    /// </summary>
    public string Model { get; set; }

    /// <summary>
    /// <para>
    /// A list of tool enabled on the assistant.
    /// </para>
    /// There can be a maximum of 128 tools per assistant. Tools can be of types `code_interpreter`, `file_search`, or `function`.
    /// </summary>
    [CodeGenMember("Tools")]
    public IList<ToolDefinition> DefaultTools { get; }

    // CUSTOM: reuse common request/response models for tool resources. Note that modification operations use the
    //          response models (which do not contain resource initialization helpers).

    /// <inheritdoc cref="ToolResources"/>
    [CodeGenMember("ToolResources")]
    public ToolResources ToolResources { get; set; }

    /// <inheritdoc cref="AssistantResponseFormat"/>
    [CodeGenMember("ResponseFormat")]
    public AssistantResponseFormat ResponseFormat { get; set; }

    /// <summary>
    /// An alternative to sampling with temperature, called nucleus sampling, where the model considers the results of the tokens with top_p probability mass. So 0.1 means only the tokens comprising the top 10% probability mass are considered.
    ///
    /// We generally recommend altering this or temperature but not both.
    /// </summary>
    [CodeGenMember("TopP")]
    public float? NucleusSamplingFactor { get; set; }

    // CUSTOM: Made internal.
    [CodeGenMember("ReasoningEffort")]
    internal ChatReasoningEffortLevel? ReasoningEffortLevel { get; set; }

    internal BinaryContent ToBinaryContent() => BinaryContent.Create(this, ModelSerializationExtensions.WireOptions);
}
