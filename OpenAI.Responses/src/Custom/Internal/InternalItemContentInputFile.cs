using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Responses;

[CodeGenType("ItemContentInputFile")]
internal partial class InternalItemContentInputFile
{
    private DataUriValue _fileValue;

    [CodeGenMember("FileData")]
    internal string InternalFileData
    {
        get => _fileValue?.GetValue(cache: true);
        set
        {
            _fileValue = value is null ? null : new DataUriValue(value, parseDataUri: true);
            if (value is not null && !_fileValue.IsDataUri)
            {
                throw new ArgumentException($"Input did not parse a valid data URI.");
            }
        }
    }

    public InternalItemContentInputFile(string filename, BinaryData fileBytes, string fileBytesMediaType)
        : this(InternalItemContentType.InputFile, default, null, null, filename, null)
    {
        Argument.AssertNotNullOrEmpty(filename, nameof(filename));
        Argument.AssertNotNull(fileBytes, nameof(fileBytes));
        Argument.AssertNotNull(fileBytesMediaType, nameof(fileBytesMediaType));

        _fileValue = new DataUriValue(fileBytes, fileBytesMediaType);
    }

    public BinaryData InternalFileBytes => _fileValue?.Bytes;

    public string InternalFileBytesMediaType => _fileValue?.MediaType;

    protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        string format = options.Format == "W" ? ((IPersistableModel<InternalItemContentInputFile>)this).GetFormatFromOptions(options) : options.Format;
        if (format != "J")
        {
            throw new FormatException($"The model {nameof(InternalItemContentInputFile)} does not support writing '{format}' format.");
        }
        base.JsonModelWriteCore(writer, options);
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        if (Optional.IsDefined(FileId) && !Patch.Contains("$.file_id"u8))
        {
            writer.WritePropertyName("file_id"u8);
            writer.WriteStringValue(FileId);
        }
        if (Optional.IsDefined(FileUrl) && !Patch.Contains("$.file_url"u8))
        {
            writer.WritePropertyName("file_url"u8);
            writer.WriteStringValue(FileUrl.AbsoluteUri);
        }
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

        Patch.WriteTo(writer);
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
    }
}