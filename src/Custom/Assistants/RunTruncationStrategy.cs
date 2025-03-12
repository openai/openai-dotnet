using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants
{
    /// <summary> Controls for how a thread will be truncated prior to the run. Use this to control the intial context window of the run. </summary>
    [Experimental("OPENAI001")]
    [CodeGenType("TruncationObject")]
    [CodeGenSuppress(nameof(RunTruncationStrategy), typeof(InternalTruncationObjectType))]
    public partial class RunTruncationStrategy
    {
        /// <summary> The truncation strategy to use for the thread. The default is `auto`. If set to `last_messages`, the thread will be truncated to the n most recent messages in the thread. When set to `auto`, messages in the middle of the thread will be dropped to fit the context length of the model, `max_prompt_tokens`. </summary>
        [CodeGenMember("Type")]
        internal readonly InternalTruncationObjectType _type;

        /// <summary> The number of most recent messages from the thread when constructing the context for the run. </summary>
        /// <remarks>
        /// </remarks>
        [CodeGenMember("LastMessages")]
        public int? LastMessages { get; }

        /// <summary>
        /// The default <see cref="RunTruncationStrategy"/> that will eliminate messages in the middle of the thread
        /// to fit within the context length of the model or the max prompt tokens.
        /// </summary>
        public static RunTruncationStrategy Auto { get; } = new(0, InternalTruncationObjectType.Auto, null);

        /// <summary>
        /// Creates a new <see cref="RunTruncationStrategy"/> instance using the <c>last_messages</c> strategy type,
        /// which will truncate the thread to specified count of preceding messages for the run.
        /// </summary>
        /// <param name="lastMessageCount"> The count of last messages that the run should evaluate. </param>
        /// <returns></returns>
        public static RunTruncationStrategy CreateLastMessagesStrategy(int lastMessageCount)
            => new(lastMessageCount, InternalTruncationObjectType.LastMessages, null);
    }
}
