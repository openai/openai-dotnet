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
    /// </remarks>
    [CodeGenMember("StepDetails")]
    public RunStepDetails Details { get; }
}
