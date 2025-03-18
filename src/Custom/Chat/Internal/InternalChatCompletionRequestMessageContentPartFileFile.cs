using System;
using System.Text.RegularExpressions;

namespace OpenAI.Chat;

[CodeGenType("ChatCompletionRequestMessageContentPartFileFile")]
internal partial class InternalChatCompletionRequestMessageContentPartFileFile
{
    private readonly BinaryData _fileBytes;
    private readonly string _fileBytesMediaType;

    // CUSTOM: Changed type from Uri to string to be able to support data URIs properly.
    /// <summary> Either a URL of the image or the base64 encoded image data. </summary>
    [CodeGenMember("FileData")]
    internal string FileData { get; }

    public InternalChatCompletionRequestMessageContentPartFileFile(BinaryData fileBytes, string fileBytesMediaType)
    {
        Argument.AssertNotNull(fileBytes, nameof(fileBytes));
        Argument.AssertNotNull(fileBytesMediaType, nameof(fileBytesMediaType));

        _fileBytes = fileBytes;
        _fileBytesMediaType = fileBytesMediaType;

        string base64EncodedData = Convert.ToBase64String(_fileBytes.ToArray());
        FileData = $"data:{_fileBytesMediaType};base64,{base64EncodedData}";
    }

    public BinaryData FileBytes => _fileBytes;

    public string FileBytesMediaType => _fileBytesMediaType;
}
