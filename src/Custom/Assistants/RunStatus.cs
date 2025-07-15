using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

[CodeGenType("RunObjectStatus")]
public readonly partial struct RunStatus
{
    /// <summary>
    /// [Helper property] Gets a value indicating whether this run status represents a condition wherein the run can
    /// no longer continue.
    /// </summary>
    /// <remarks>
    /// For more information, please refer to:
    /// https://platform.openai.com/docs/assistants/how-it-works/run-lifecycle
    /// </remarks>
    public bool IsTerminal
        => _value == CompletedValue
        || _value == ExpiredValue
        || _value == FailedValue
        || _value == IncompleteValue
        || _value == CancelledValue;
}
