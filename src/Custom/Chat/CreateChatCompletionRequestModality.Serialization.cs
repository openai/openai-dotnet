using System;

namespace OpenAI.Chat
{
    internal static partial class CreateChatCompletionRequestModalityExtensions
    {
        public static string ToSerialString(this ChatCompletionRequestModality value) => value switch
        {
            ChatCompletionRequestModality.Text => "text",
            ChatCompletionRequestModality.Audio => "audio",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown CreateChatCompletionRequestModality value.")
        };

        public static ChatCompletionRequestModality ToCreateChatCompletionRequestModality(this string value)
        {
            if (StringComparer.OrdinalIgnoreCase.Equals(value, "text"))
            {
                return ChatCompletionRequestModality.Text;
            }
            if (StringComparer.OrdinalIgnoreCase.Equals(value, "audio"))
            {
                return ChatCompletionRequestModality.Audio;
            }
            throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown CreateChatCompletionRequestModality value.");
        }
    }
}