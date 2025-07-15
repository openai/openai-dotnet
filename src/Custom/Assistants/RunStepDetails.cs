using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants
{
    [CodeGenType("RunStepObjectStepDetails")]
    public abstract partial class RunStepDetails
    {
        public string CreatedMessageId => AsInternalMessageCreation?.InternalMessageId;
        public IReadOnlyList<RunStepToolCall> ToolCalls => AsInternalToolCalls ?? [];

        private InternalRunStepDetailsMessageCreationObject AsInternalMessageCreation => this as InternalRunStepDetailsMessageCreationObject;
        private InternalRunStepDetailsToolCallsObject AsInternalToolCalls => this as InternalRunStepDetailsToolCallsObject;
    }
}
