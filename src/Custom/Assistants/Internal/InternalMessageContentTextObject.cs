using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace OpenAI.Assistants;

/// <summary>
/// Represents an item of annotated text content within an Assistants API response message.
/// </summary>
[CodeGenType("MessageContentTextObject")]
internal partial class InternalMessageContentTextObject
{
    [CodeGenMember("Text")]
    internal BinaryData InternalText;

    internal InternalMessageContentTextObjectText1 InternalTextObjectValue { get; set; }
    internal string InternalTextLiteralValue { get; set; }

    internal IReadOnlyList<TextAnnotation> WrappedAnnotations
        => _wrappedAnnotations
        ??= (InternalTextObjectValue?.Annotations ?? []).Select(internalAnnotation => new TextAnnotation(internalAnnotation)).ToList();
    private List<TextAnnotation> _wrappedAnnotations;

    // CUSTOM: Furnish custom constructors for union variants
    internal InternalMessageContentTextObject(string textLiteralValue)
        : this(InternalMessageContentType.Text, additionalBinaryDataProperties: null, internalText: BinaryData.Empty)
    {
        InternalTextLiteralValue = textLiteralValue;
    }

    internal InternalMessageContentTextObject(InternalMessageContentTextObjectText1 textObjectValue, IDictionary<string, BinaryData> additionalBinaryDataProperties)
        : this(InternalMessageContentType.Text, additionalBinaryDataProperties, internalText: BinaryData.Empty)
    {
        InternalTextObjectValue = textObjectValue;
    }

    protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        string format = options.Format == "W" ? ((IPersistableModel<InternalMessageContentTextObject>)this).GetFormatFromOptions(options) : options.Format;
        if (format != "J")
        {
            throw new FormatException($"The model {nameof(InternalMessageContentTextObject)} does not support writing '{format}' format.");
        }
        base.JsonModelWriteCore(writer, options);
        if (_additionalBinaryDataProperties?.ContainsKey("text") != true)
        {
            writer.WritePropertyName("text"u8);
            // CUSTOM: Selectively write either text literal (union variant 0) or object (union variant 1)
            if (!string.IsNullOrEmpty(InternalTextLiteralValue))
            {
                writer.WriteStringValue(InternalTextLiteralValue);
            }
            else
            {
                writer.WriteObjectValue(InternalTextObjectValue, options);
            }
        }
    }

    internal static InternalMessageContentTextObject DeserializeInternalMessageContentTextObject(JsonElement element, ModelReaderWriterOptions options)
    {
        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }
        InternalMessageContentType kind = default;
        IDictionary<string, BinaryData> additionalBinaryDataProperties = new ChangeTrackingDictionary<string, BinaryData>();
        string internalTextLiteralValue = default;
        InternalMessageContentTextObjectText1 internalTextObjectValue = default;
        foreach (var prop in element.EnumerateObject())
        {
            if (prop.NameEquals("type"u8))
            {
                kind = new InternalMessageContentType(prop.Value.GetString());
                continue;
            }
            if (prop.NameEquals("text"u8))
            {
                if (prop.Value.ValueKind == JsonValueKind.String)
                {
                    internalTextLiteralValue = prop.Value.GetString();
                }
                else
                {
                    internalTextObjectValue = InternalMessageContentTextObjectText1.DeserializeInternalMessageContentTextObjectText1(prop.Value, options);
                }
                continue;
            }
            additionalBinaryDataProperties.Add(prop.Name, BinaryData.FromString(prop.Value.GetRawText()));
        }
        return string.IsNullOrEmpty(internalTextLiteralValue)
            ? new InternalMessageContentTextObject(internalTextObjectValue, additionalBinaryDataProperties)
            : new InternalMessageContentTextObject(internalTextLiteralValue);
    }
}
