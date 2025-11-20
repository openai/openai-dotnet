using System;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses
{
    [Experimental("OPENAI001")]
    public enum Includable
    {
        FileSearchCallResults,
        MessageInputImageImageUrl,
        ComputerCallOutputOutputImageUrl,
        ReasoningEncryptedContent,
        CodeInterpreterCallOutputs
    }

    internal static partial class IncludableExtensions
    {
        internal static Includable ToIncludable(this IncludedResponseProperty includable)
        {
            if (includable == IncludedResponseProperty.FileSearchCallResults)
            {
                return Includable.FileSearchCallResults;
            }
            if (includable == IncludedResponseProperty.MessageInputImageUri)
            {
                return Includable.MessageInputImageImageUrl;
            }
            if (includable == IncludedResponseProperty.ComputerCallOutputImageUri)
            {
                return Includable.ComputerCallOutputOutputImageUrl;
            }
            if (includable == IncludedResponseProperty.ReasoningEncryptedContent)
            {
                return Includable.ReasoningEncryptedContent;
            }
            if (includable == IncludedResponseProperty.CodeInterpreterCallOutputs)
            {
                return Includable.CodeInterpreterCallOutputs;
            }
            throw new ArgumentException($"Unknown Includable value: {includable}", nameof(includable));
        }
    }
}
