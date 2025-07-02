namespace OpenAI.Chat;

/// <summary>
/// The reason the model stopped generating tokens. This will be:
/// <list type="table">
/// <listheader>
///     <member>Property</member>
///     <rest>REST</rest>
///     <cond>Condition</cond>
/// </listheader>
/// <item>
///     <member><see cref="Stop"/></member>
///     <rest><c>stop</c></rest>
///     <cond>The model encountered a natural stop point or provided stop sequence.</cond>
/// </item>
/// <item>
///     <member><see cref="Length"/></member>
///     <rest><c>length</c></rest>
///     <cond>The maximum number of tokens specified in the request was reached.</cond>
/// </item>
/// <item>
///     <member><see cref="ContentFilter"/></member>
///     <rest><c>content_filter</c></rest>
///     <cond>Content was omitted due to a triggered content filter rule.</cond>
/// </item>
/// <item>
///     <member><see cref="ToolCalls"/></member>
///     <rest><c>tool_calls</c></rest>
///     <cond>
///         With no explicit <c>tool_choice</c>, the model called one or more tools that were defined in the request.
///     </cond>
/// </item>
/// <item>
///     <member><see cref="FunctionCall"/></member>
///     <rest><c>function_call</c></rest>
///     <cond>(Deprecated) The model called a function that was defined in the request.</cond>
/// </item>
/// </list>
/// </summary>
[CodeGenType("CreateChatCompletionResponseChoiceFinishReason")]
public enum ChatFinishReason
{
    /// <summary>
    /// Indicates that the model encountered a natural stop point or provided stop sequence.
    /// </summary>
    [CodeGenMember("Stop")]
    Stop,

    /// <summary>
    /// Indicates that the model reached the maximum number of tokens allowed for the request.
    /// </summary>
    [CodeGenMember("Length")]
    Length,

    /// <summary>
    /// Indicates that content was omitted due to a triggered content filter rule.
    /// </summary>
    [CodeGenMember("ContentFilter")]
    ContentFilter,

    /// <summary>
    /// Indicates that the model called a function that was defined in the request.
    /// </summary>
    /// <remarks>
    /// To resolve tool calls, append the message associated with the tool calls followed by matching instances of
    /// <see cref="ToolChatMessage"/> for each tool call, then perform another chat completion with the combined
    /// set of messages.
    /// <para>
    /// <b>Note</b>: <see cref="ToolCalls"/> is <i>not</i> provided as the <c>finish_reason</c> if the model calls a
    /// tool in response to an explicit <c>tool_choice</c> via <see cref="ChatCompletionOptions.ToolChoice"/>.
    /// In that case, calling the specified tool is assumed and the expected reason is <see cref="Stop"/>.
    /// </para>
    /// </remarks>
    [CodeGenMember("ToolCalls")]
    ToolCalls,

    /// <summary>
    /// Indicates that the model called a function that was defined in the request.
    /// </summary>
    /// <remarks>
    /// To resolve a function call, append the message associated with the function call followed by a
    /// <see cref="FunctionChatMessage"/> with the appropriate name and arguments, then perform another chat
    /// completion with the combined set of messages.
    /// </remarks>
    [CodeGenMember("FunctionCall")]
    FunctionCall,
}
