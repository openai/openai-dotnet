using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

[Experimental("OPENAI001")]
[CodeGenModel("RunStepObject")]
public partial class RunStep
{
    // CUSTOM: Made internal.
    /// <summary> The object type, which is always `thread.run.step`. </summary>
    [CodeGenMember("Object")]
    internal InternalRunStepObjectObject Object { get; } = InternalRunStepObjectObject.ThreadRunStep;

    /// <summary>
    /// The <c>step_details</c> associated with this run step.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Please note <see cref="RunStepDetails"/> is the base class.
    /// </para>
    /// <para>
    /// According to the scenario, a derived class of the base class might need to be assigned here, or this property
    /// needs to be casted to one of the possible derived classes.
    /// </para>
    /// <para>
    /// The available derived classes include <see cref="InternalRunStepDetailsMessageCreationObject"/> and <see cref="InternalRunStepToolCallDetailsCollection"/>.
    /// </para>
    /// </remarks>
    [CodeGenMember("StepDetails")]
    public RunStepDetails Details { get; }
}
