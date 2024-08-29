using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

[Experimental("OPENAI001")]
[CodeGenModel("RunStepDetailsToolCallsObjectToolCallsObject")]
public partial class RunStepToolCall
{
    public string ToolCallId
        => AsCodeInterpreter?.Id
        ?? AsFunction?.Id
        ?? AsFileSearch?.Id
        ?? (SerializedAdditionalRawData?.TryGetValue("id", out BinaryData idData) == true
            ? idData.ToString()
            : null);

    public string CodeInterpreterInput => AsCodeInterpreter?.Input;
    public IReadOnlyList<RunStepCodeInterpreterOutput> CodeInterpreterOutputs => AsCodeInterpreter?.Outputs ?? [];

    public string FunctionName => AsFunction?.InternalName;
    public string FunctionArguments => AsFunction?.InternalArguments;
    public string FunctionOutput => AsFunction?.InternalOutput;

    public RunStepToolCallKind ToolKind
        => AsCodeInterpreter is not null ? RunStepToolCallKind.CodeInterpreter
        : AsFileSearch is not null ? RunStepToolCallKind.FileSearch
        : AsFunction is not null ? RunStepToolCallKind.Function
        : RunStepToolCallKind.Unknown;

    private InternalRunStepCodeInterpreterToolCallDetails AsCodeInterpreter
        => this as InternalRunStepCodeInterpreterToolCallDetails;
    private InternalRunStepFunctionToolCallDetails AsFunction => this as InternalRunStepFunctionToolCallDetails;
    private InternalRunStepFileSearchToolCallDetails AsFileSearch => this as InternalRunStepFileSearchToolCallDetails;
}
