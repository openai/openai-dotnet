using OpenAI.Internal;
using System;
using System.ClientModel.Primitives;
using System.ComponentModel;

namespace OpenAI.Chat;

[CodeGenModel("ChatResponseFormat")]
public abstract partial class ChatResponseFormat : IEquatable<ChatResponseFormat>
{
    public static ChatResponseFormat Text { get; } = new InternalChatResponseFormatText();
    public static ChatResponseFormat JsonObject { get; } = new InternalChatResponseFormatJsonObject();

    public static ChatResponseFormat CreateTextFormat() => new InternalChatResponseFormatText();
    public static ChatResponseFormat CreateJsonObjectFormat() => new InternalChatResponseFormatJsonObject();
    public static ChatResponseFormat CreateJsonSchemaFormat(
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
        return new InternalChatResponseFormatJsonSchema(internalSchema);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool operator ==(ChatResponseFormat first, ChatResponseFormat second)
    {
        if (first is null)
        {
            return second is null;
        }
        return first.Equals(second);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool operator !=(ChatResponseFormat first, ChatResponseFormat second)
        => !(first == second);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object obj)
    {
        return (this as IEquatable<ChatResponseFormat>).Equals(obj as ChatResponseFormat)
            || ToString().Equals(obj?.ToString());
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() => ToString().GetHashCode();

    [EditorBrowsable(EditorBrowsableState.Never)]
    bool IEquatable<ChatResponseFormat>.Equals(ChatResponseFormat other)
    {
        if (other is null)
        {
            return false;
        }

        if (Object.ReferenceEquals(this, other))
        {
            return true;
        }

        return (this is InternalChatResponseFormatText && other is InternalChatResponseFormatText)
            || (this is InternalChatResponseFormatJsonObject && other is InternalChatResponseFormatJsonObject)
            || (this is InternalChatResponseFormatJsonSchema thisJsonSchema
                    && other is InternalChatResponseFormatJsonSchema otherJsonSchema
                    && thisJsonSchema.JsonSchema.Name.Equals(otherJsonSchema.JsonSchema.Name));
    }

    public override string ToString()
    {
        return ModelReaderWriter.Write(this).ToString();
    }
}