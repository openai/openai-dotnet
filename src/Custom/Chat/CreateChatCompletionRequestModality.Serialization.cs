using System;

namespace OpenAI
{
    internal static partial class CreateChatCompletionRequestModalityExtensions
    {
        public static string ToSerialString(this CreateChatCompletionRequestModality value) => value switch
        {
            CreateChatCompletionRequestModality.Text => "text",
            CreateChatCompletionRequestModality.Audio => "audio",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown CreateChatCompletionRequestModality value.")
        };

        public static CreateChatCompletionRequestModality ToCreateChatCompletionRequestModality(this string value)
        {
            if (StringComparer.OrdinalIgnoreCase.Equals(value, "text"))
            {
                return CreateChatCompletionRequestModality.Text;
            }
            if (StringComparer.OrdinalIgnoreCase.Equals(value, "audio"))
            {
                return CreateChatCompletionRequestModality.Audio;
            }
            throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown CreateChatCompletionRequestModality value.");
        }
    }
}