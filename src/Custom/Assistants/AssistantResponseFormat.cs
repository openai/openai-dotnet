using OpenAI.Internal;
using System;
using System.ClientModel.Primitives;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

[Experimental("OPENAI001")]
[CodeGenModel("AssistantResponseFormat")]
public partial class AssistantResponseFormat : IEquatable<AssistantResponseFormat>, IEquatable<string>
{
    public static AssistantResponseFormat Auto { get; } = CreateAutoFormat();
    public static AssistantResponseFormat Text { get; } = CreateTextFormat();
    public static AssistantResponseFormat JsonObject { get; } = CreateJsonObjectFormat();

    public static AssistantResponseFormat CreateAutoFormat()
        => new InternalAssistantResponseFormatPlainTextNoObject("auto");
    public static AssistantResponseFormat CreateTextFormat()
        => new InternalAssistantResponseFormatText();
    public static AssistantResponseFormat CreateJsonObjectFormat()
        => new InternalAssistantResponseFormatJsonObject();
    public static AssistantResponseFormat CreateJsonSchemaFormat(
        string name,
        BinaryData jsonSchema,
        string description = null,
        bool? strictSchemaEnabled = null)
    {
        Argument.AssertNotNullOrEmpty(name, nameof(name));
        Argument.AssertNotNull(jsonSchema, nameof(jsonSchema));

        InternalResponseFormatJsonSchemaJsonSchema internalSchema = new(
            description,
            name,
            jsonSchema,
            strictSchemaEnabled,
            null);
        return new InternalAssistantResponseFormatJsonSchema(internalSchema);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool operator ==(AssistantResponseFormat first, AssistantResponseFormat second)
    {
        if (first is null)
        {
            return second is null;
        }
        return first.Equals(second);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool operator !=(AssistantResponseFormat first, AssistantResponseFormat second)
        => !(first == second);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object obj)
    {
        return (this as IEquatable<AssistantResponseFormat>).Equals(obj as AssistantResponseFormat)
            || ToString().Equals(obj?.ToString());
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() => ToString().GetHashCode();

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static implicit operator AssistantResponseFormat(string plainTextFormat)
        => new InternalAssistantResponseFormatPlainTextNoObject(plainTextFormat);

    [EditorBrowsable(EditorBrowsableState.Never)]
    bool IEquatable<AssistantResponseFormat>.Equals(AssistantResponseFormat other)
    {
        if (other is null)
        {
            return false;
        }

        if (Object.ReferenceEquals(this, other))
        {
            return true;
        }

        return
            (this is InternalAssistantResponseFormatPlainTextNoObject thisPlainText
                && other is InternalAssistantResponseFormatPlainTextNoObject otherPlainText
                && thisPlainText.Value.Equals(otherPlainText.Value))
            || (this is InternalAssistantResponseFormatText && other is InternalAssistantResponseFormatText)
            || (this is InternalAssistantResponseFormatJsonObject && other is InternalAssistantResponseFormatJsonObject)
            || (this is InternalAssistantResponseFormatJsonSchema thisJsonSchema
                && other is InternalAssistantResponseFormatJsonSchema otherJsonSchema
                && thisJsonSchema.JsonSchema.Name.Equals(otherJsonSchema.JsonSchema.Name));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    bool IEquatable<string>.Equals(string other)
    {
        return this is InternalAssistantResponseFormatPlainTextNoObject thisPlainText
            && thisPlainText.Value.Equals(other);
    }

    public override string ToString()
    {
        if (this is InternalAssistantResponseFormatPlainTextNoObject plainTextInstance)
        {
            return plainTextInstance.Value;
        }
        else
        {
            return ModelReaderWriter.Write(this).ToString();
        }
    }
}
