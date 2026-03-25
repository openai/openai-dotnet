namespace OpenAI.Assistants;

public abstract partial class RunStepCodeInterpreterOutput
{
    /// <inheritdoc cref="InternalRunStepDetailsToolCallsCodeOutputImageObject.FileId"/>
    public string ImageFileId => AsInternalImage?.FileId;
    /// <inheritdoc cref="InternalRunStepCodeInterpreterLogOutput.Logs"/>
    public string Logs => AsInternalLogs?.InternalLogs;

    private InternalRunStepDetailsToolCallsCodeOutputImageObject AsInternalImage => this as InternalRunStepDetailsToolCallsCodeOutputImageObject;
    private InternalRunStepCodeInterpreterLogOutput AsInternalLogs => this as InternalRunStepCodeInterpreterLogOutput;
}
