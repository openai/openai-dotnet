using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.Assistants;

/// <summary>
/// The update type presented when the status of a run step changes.
/// </summary>
[Experimental("OPENAI001")]
public class RunStepUpdate : StreamingUpdate<RunStep>
{
    internal RunStepUpdate(RunStep runStep, StreamingUpdateReason updateKind)
        : base(runStep, updateKind)
    { }

    internal static IEnumerable<StreamingUpdate<RunStep>> DeserializeRunStepUpdates(
        JsonElement element,
        StreamingUpdateReason updateKind,
        ModelReaderWriterOptions options = null)
    {
        RunStep runStep = RunStep.DeserializeRunStep(element, options);
        return updateKind switch
        {
            _ => [new RunStepUpdate(runStep, updateKind)],
        };
    }
}