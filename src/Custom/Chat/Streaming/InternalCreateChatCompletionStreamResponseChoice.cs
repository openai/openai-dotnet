namespace OpenAI.Chat;

[CodeGenModel("CreateChatCompletionStreamResponseChoice")]
internal partial class InternalCreateChatCompletionStreamResponseChoice
{
    // CUSTOM: Changed type from string.
    /// <summary>
    /// The reason the model stopped generating tokens. This will be `stop` if the model hit a natural stop point or a provided stop sequence,
    /// `length` if the maximum number of tokens specified in the request was reached,
    /// `content_filter` if content was omitted due to a flag from our content filters,
    /// `tool_calls` if the model called a tool, or `function_call` (deprecated) if the model called a function.
    /// </summary>
    [CodeGenMember("FinishReason")]
    public ChatFinishReason? FinishReason { get; }
}
