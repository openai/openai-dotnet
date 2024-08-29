using System.Collections.Generic;

namespace OpenAI.Assistants;

[CodeGenModel("RunStepDetailsToolCallsCodeObject")]
internal partial class InternalRunStepCodeInterpreterToolCallDetails
{
    /// <inheritdoc cref="InternalRunStepDetailsToolCallsCodeObjectCodeInterpreter.Input"/>
    public string Input => _codeInterpreter.Input;

    /// <inheritdoc cref="InternalRunStepDetailsToolCallsCodeObjectCodeInterpreter.Outputs"/>
    public IReadOnlyList<RunStepCodeInterpreterOutput> Outputs => _codeInterpreter.Outputs;

    [CodeGenMember("CodeInterpreter")]
    internal InternalRunStepDetailsToolCallsCodeObjectCodeInterpreter _codeInterpreter;
}