using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Chat;

[CodeGenType("ChatCompletionRequestMessageContentPartFileFile")]
internal partial class InternalChatCompletionRequestMessageContentPartFileFile
{
    private DataUriValue _fileValue;

    // CUSTOM: Changed type from Uri to string to be able to support data URIs properly.
    /// <summary> Either a URL of the image or the base64 encoded image data. </summary>
    [CodeGenMember("FileData")]
    internal string InternalFileData
    {
        get => _fileValue?.GetValue(cache: true);
        set
        {
            _fileValue = value is null ? null : new DataUriValue(value, parseDataUri: true);
            if (value is not null && !_fileValue.IsDataUri)
            {
                throw new ArgumentException("Input did not parse a valid data URI.", nameof(value));
            }
        }
    }

    public InternalChatCompletionRequestMessageContentPartFileFile(BinaryData fileBytes, string fileBytesMediaType, string filename)
        : this(filename: filename, null, null, default)
    {
        Argument.AssertNotNull(fileBytes, nameof(fileBytes));
        Argument.AssertNotNull(fileBytesMediaType, nameof(fileBytesMediaType));

        _fileValue = new DataUriValue(fileBytes, fileBytesMediaType);
    }

    public BinaryData FileBytes => _fileValue?.Bytes;

    public string FileBytesMediaType => _fileValue?.MediaType;

    protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        string format = options.Format == "W" ? ((IPersistableModel<InternalChatCompletionRequestMessageContentPartFileFile>)this).GetFormatFromOptions(options) : options.Format;
        if (format != "J")
        {
            throw new FormatException($"The model {nameof(InternalChatCompletionRequestMessageContentPartFileFile)} does not support writing '{format}' format.");
        }
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        if (Optional.IsDefined(Filename) && !Patch.Contains("$.filename"u8))
        {
            writer.WritePropertyName("filename"u8);
            writer.WriteStringValue(Filename);
        }
        if (_fileValue is not null && !Patch.Contains("$.file_data"u8))
        {
            writer.WritePropertyName("file_data"u8);
            writer.WriteStringValue(_fileValue.GetValue(cache: false));
        }
        if (Optional.IsDefined(FileId) && !Patch.Contains("$.file_id"u8))
        {
            writer.WritePropertyName("file_id"u8);
            writer.WriteStringValue(FileId);
        }

        Patch.WriteTo(writer);
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
    }
}
