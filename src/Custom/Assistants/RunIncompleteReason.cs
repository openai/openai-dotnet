using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace OpenAI.Assistants
{
    [Experimental("OPENAI001")]
    [CodeGenModel("RunObjectIncompleteDetailsReason")]
    public readonly partial struct RunIncompleteReason
    {
        // CUSTOM: Renamed.
        [CodeGenMember("MaxCompletionTokens")]
        public static RunIncompleteReason MaxOutputTokenCount { get; } = new RunIncompleteReason(MaxOutputTokenCountValue);

        // CUSTOM: Renamed.
        [CodeGenMember("MaxPromptTokens")]
        public static RunIncompleteReason MaxInputTokenCount { get; } = new RunIncompleteReason(MaxInputTokenCountValue);
    }
}
