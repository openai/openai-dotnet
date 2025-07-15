using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants
{
    [CodeGenType("RunCompletionUsage")]
    public partial class RunTokenUsage
    {
        // CUSTOM: Renamed.
        [CodeGenMember("CompletionTokens")]
        public int OutputTokenCount { get; }

        // CUSTOM: Renamed.
        [CodeGenMember("PromptTokens")]
        public int InputTokenCount { get; }

        // CUSTOM: Renamed.
        [CodeGenMember("TotalTokens")]
        public int TotalTokenCount { get; }
    }
}
