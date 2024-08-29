using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

[Experimental("OPENAI001")]
[CodeGenModel("AssistantObject")]
public partial class Assistant
{
    // CUSTOM: Made internal.
    /// <summary> The object type, which is always `assistant`. </summary>
    [CodeGenMember("Object")]
    internal InternalAssistantObjectObject Object { get; } = InternalAssistantObjectObject.Assistant;

    /// <inheritdoc cref="AssistantResponseFormat"/>
    public AssistantResponseFormat ResponseFormat { get; }

    /// <summary>
    /// An alternative to sampling with temperature, called nucleus sampling, where the model considers the results of the tokens with top_p probability mass. So 0.1 means only the tokens comprising the top 10% probability mass are considered.
    ///
    /// We generally recommend altering this or temperature but not both.
    /// </summary>
    [CodeGenMember("TopP")]
    public float? NucleusSamplingFactor { get; }
}
