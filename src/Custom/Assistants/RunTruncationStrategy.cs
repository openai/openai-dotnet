using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Assistants
{
    /// <summary> Controls for how a thread will be truncated prior to the run. Use this to control the initial context window of the run. </summary>
    [CodeGenType("TruncationObject")]
    [CodeGenSuppress(nameof(RunTruncationStrategy), typeof(InternalCreateThreadAndRunRequestTruncationStrategyType))]
    public partial class RunTruncationStrategy
    {
        /// <summary> The number of most recent messages from the thread when constructing the context for the run. </summary>
        /// <remarks>
        /// </remarks>
        [CodeGenMember("LastMessages")]
        public int? LastMessages { get; }

        /// <summary>
        /// The default <see cref="RunTruncationStrategy"/> that will eliminate messages in the middle of the thread
        /// to fit within the context length of the model or the max prompt tokens.
        /// </summary>
        public static RunTruncationStrategy Auto { get; } = new(InternalCreateThreadAndRunRequestTruncationStrategyType.Auto, 0, null);

        /// <summary>
        /// Creates a new <see cref="RunTruncationStrategy"/> instance using the <c>last_messages</c> strategy type,
        /// which will truncate the thread to specified count of preceding messages for the run.
        /// </summary>
        /// <param name="lastMessageCount"> The count of last messages that the run should evaluate. </param>
        /// <returns></returns>
        public static RunTruncationStrategy CreateLastMessagesStrategy(int lastMessageCount)
            => new(InternalCreateThreadAndRunRequestTruncationStrategyType.LastMessages, lastMessageCount, null);
    }
}
