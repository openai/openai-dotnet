using System;

namespace OpenAI.Chat
{
    internal static partial class ChatRequestModalityExtensions
    {
        public static string ToSerialString(this ChatRequestModality value) => value switch
        {
            ChatRequestModality.Text => "text",
            ChatRequestModality.Audio => "audio",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown CreateChatCompletionRequestModality value.")
        };

        public static ChatRequestModality ToCreateChatCompletionRequestModality(this string value)
        {
            if (StringComparer.OrdinalIgnoreCase.Equals(value, "text"))
            {
                return ChatRequestModality.Text;
            }
            if (StringComparer.OrdinalIgnoreCase.Equals(value, "audio"))
            {
                return ChatRequestModality.Audio;
            }
            throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown CreateChatCompletionRequestModality value.");
        }
    }
}