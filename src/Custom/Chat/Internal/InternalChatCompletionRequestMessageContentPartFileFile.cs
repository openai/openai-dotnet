using System;
using System.Text.RegularExpressions;

namespace OpenAI.Chat;

[CodeGenType("ChatCompletionRequestMessageContentPartFileFile")]
internal partial class InternalChatCompletionRequestMessageContentPartFileFile
{
    private BinaryData _fileBytes;
    private string _fileBytesMediaType;

    // CUSTOM: Changed type from Uri to string to be able to support data URIs properly.
    /// <summary> Either a URL of the image or the base64 encoded image data. </summary>
    [CodeGenMember("FileData")]
    internal string InternalFileData
    {
        get => _internalFileData;
        set
        {
            _internalFileData = value;
            if (value is not null && !DataEncodingHelpers.TryParseDataUri(value, out _fileBytes, out _fileBytesMediaType))
            {
                throw new ArgumentException($"Input did not parse a valid data URI.");
            }
        }
    }
    private string _internalFileData;

    public InternalChatCompletionRequestMessageContentPartFileFile(BinaryData fileBytes, string fileBytesMediaType, string filename)
        : this(filename: filename, null, null, null)
    {
        Argument.AssertNotNull(fileBytes, nameof(fileBytes));
        Argument.AssertNotNull(fileBytesMediaType, nameof(fileBytesMediaType));

        _fileBytes = fileBytes;
        _fileBytesMediaType = fileBytesMediaType;
        _internalFileData = DataEncodingHelpers.CreateDataUri(fileBytes, fileBytesMediaType);
    }

    public BinaryData FileBytes => _fileBytes;

    public string FileBytesMediaType => _fileBytesMediaType;
}
