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
        internal static Includable ToIncludable(this InternalIncludable internalIncludable)
        {
            if (internalIncludable == InternalIncludable.FileSearchCallResults)
            {
                return Includable.FileSearchCallResults;
            }
            if (internalIncludable == InternalIncludable.MessageInputImageImageUrl)
            {
                return Includable.MessageInputImageImageUrl;
            }
            if (internalIncludable == InternalIncludable.ComputerCallOutputOutputImageUrl)
            {
                return Includable.ComputerCallOutputOutputImageUrl;
            }
            if (internalIncludable == InternalIncludable.ReasoningEncryptedContent)
            {
                return Includable.ReasoningEncryptedContent;
            }
            if (internalIncludable == InternalIncludable.CodeInterpreterCallOutputs)
            {
                return Includable.CodeInterpreterCallOutputs;
            }
            throw new ArgumentException($"Unknown InternalIncludable value: {internalIncludable}", nameof(internalIncludable));
        }
    }
}
