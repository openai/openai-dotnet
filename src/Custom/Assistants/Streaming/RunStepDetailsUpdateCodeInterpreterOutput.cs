using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

[CodeGenType("RunStepDeltaStepDetailsToolCallsCodeObjectCodeInterpreterOutputsObject")]
public partial class RunStepUpdateCodeInterpreterOutput
{
    /// <inheritdoc cref="InternalRunStepDeltaStepDetailsToolCallsCodeOutputLogsObject.Index"/>
    public int OutputIndex => AsLogs?.Index ?? AsImage?.Index ?? 0;

    /// <inheritdoc cref="InternalRunStepDeltaStepDetailsToolCallsCodeOutputLogsObject.InternalLogs"/>
    public string Logs => AsLogs?.InternalLogs;

    /// <inheritdoc cref="InternalRunStepDeltaStepDetailsToolCallsCodeOutputImageObjectImage.FileId"/>
    public string ImageFileId => AsImage?.Image?.FileId;

    private InternalRunStepDeltaStepDetailsToolCallsCodeOutputLogsObject AsLogs
        => this as InternalRunStepDeltaStepDetailsToolCallsCodeOutputLogsObject;
    private InternalRunStepDeltaStepDetailsToolCallsCodeOutputImageObject AsImage
        => this as InternalRunStepDeltaStepDetailsToolCallsCodeOutputImageObject;
}
