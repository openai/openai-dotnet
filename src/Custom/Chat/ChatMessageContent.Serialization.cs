using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;

namespace OpenAI.Chat;

public partial class ChatMessageContent
{
    internal void WriteTo(Utf8JsonWriter writer, ModelReaderWriterOptions options = null)
    {
        if (Count == 0)
        {
            writer.WriteNullValue();
        }
        else if (Count == 1 && this[0].Kind == ChatMessageContentPartKind.Text)
        {
            writer.WriteStringValue(this[0].Text);
        }
        else
        {
            writer.WriteStartArray();
            foreach (ChatMessageContentPart part in this)
            {
                writer.WriteObjectValue(part, options);
            }
            writer.WriteEndArray();
        }
    }

    internal static ChatMessageContent DeserializeChatMessageContent(JsonElement element, ModelReaderWriterOptions options = null)
    {
        options ??= new("W");
        if (element.ValueKind == JsonValueKind.String)
        {
            return new(element.GetString());
        }
        else if (element.ValueKind == JsonValueKind.Array)
        {
            List<ChatMessageContentPart> parts = [];
            foreach (JsonElement contentPartElement in element.EnumerateArray())
            {
                parts.Add(ChatMessageContentPart.DeserializeChatMessageContentPart(contentPartElement, options));
            }
            return new(parts);
        }
        return new();
    }
}
