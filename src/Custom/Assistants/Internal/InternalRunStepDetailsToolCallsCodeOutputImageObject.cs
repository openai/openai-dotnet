namespace OpenAI.Assistants;

[CodeGenType("RunStepDetailsToolCallsCodeOutputImageObject")]
public partial class InternalRunStepDetailsToolCallsCodeOutputImageObject
{
    /// <inheritdoc cref="InternalRunStepDetailsToolCallsCodeOutputImageObjectImage.FileId"/>
    public string FileId => _image.FileId;

    [CodeGenMember("Image")]
    internal InternalRunStepDetailsToolCallsCodeOutputImageObjectImage _image;
}