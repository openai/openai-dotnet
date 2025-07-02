using System;
using System.Collections.Generic;

namespace OpenAI.Responses;

[CodeGenType("ItemContentInputFile")]
internal partial class InternalItemContentInputFile
{
    private BinaryData _inputFileBytes;
    private string _inputFileBytesMediaType;

    [CodeGenMember("FileData")]
    internal string InternalFileData
    {
        get => _internalFileData;
        set
        {
            _internalFileData = value;
            if (value is not null && !DataEncodingHelpers.TryParseDataUri(value, out _inputFileBytes, out _inputFileBytesMediaType))
            {
                throw new ArgumentException($"Input did not parse a valid data URI.");
            }
        }
    }
    private string _internalFileData;

    public InternalItemContentInputFile(string filename, BinaryData fileBytes, string fileBytesMediaType)
        : this(InternalItemContentType.InputFile, null, null, filename, null)
    {
        Argument.AssertNotNullOrEmpty(filename, nameof(filename));
        Argument.AssertNotNull(fileBytes, nameof(fileBytes));
        Argument.AssertNotNull(fileBytesMediaType, nameof(fileBytesMediaType));

        _inputFileBytes = fileBytes;
        _inputFileBytesMediaType = fileBytesMediaType;
        _internalFileData = DataEncodingHelpers.CreateDataUri(fileBytes, fileBytesMediaType);
    }

    public BinaryData InternalFileBytes => _inputFileBytes;

    public string InternalFileBytesMediaType => _inputFileBytesMediaType;
}