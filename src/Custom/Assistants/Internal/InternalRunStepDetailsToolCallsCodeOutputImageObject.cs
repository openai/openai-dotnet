namespace OpenAI.Assistants;

[CodeGenModel("RunStepDetailsToolCallsCodeOutputImageObject")]
internal partial class InternalRunStepDetailsToolCallsCodeOutputImageObject
{
    /// <inheritdoc cref="InternalRunStepDetailsToolCallsCodeOutputImageObjectImage.FileId"/>
    public string FileId => _image.FileId;

    [CodeGenMember("Image")]
    internal InternalRunStepDetailsToolCallsCodeOutputImageObjectImage _image;
}