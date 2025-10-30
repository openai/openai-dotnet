using System;

namespace OpenAI.Responses
{
    internal static partial class IncludableExtensions
    {
        public static string ToSerialString(this Includable value) => value switch
        {
            Includable.FileSearchCallResults => "file_search_call.results",
            Includable.MessageInputImageImageUrl => "message.input_image.image_url",
            Includable.ComputerCallOutputOutputImageUrl => "computer_call_output.output.image_url",
            Includable.ReasoningEncryptedContent => "reasoning.encrypted_content",
            Includable.CodeInterpreterCallOutputs => "code_interpreter_call.outputs",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown Includable value.")
        };

        public static Includable ToIncludable(this string value)
        {
            if (StringComparer.OrdinalIgnoreCase.Equals(value, "file_search_call.results"))
            {
                return Includable.FileSearchCallResults;
            }
            if (StringComparer.OrdinalIgnoreCase.Equals(value, "message.input_image.image_url"))
            {
                return Includable.MessageInputImageImageUrl;
            }
            if (StringComparer.OrdinalIgnoreCase.Equals(value, "computer_call_output.output.image_url"))
            {
                return Includable.ComputerCallOutputOutputImageUrl;
            }
            if (StringComparer.OrdinalIgnoreCase.Equals(value, "reasoning.encrypted_content"))
            {
                return Includable.ReasoningEncryptedContent;
            }
            if (StringComparer.OrdinalIgnoreCase.Equals(value, "code_interpreter_call.outputs"))
            {
                return Includable.CodeInterpreterCallOutputs;
            }
            throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown Includable value.");
        }
    }
}
