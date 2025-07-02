using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Chat;

/// <summary>
/// Represents the <c>role</c> of a chat completion message.
/// </summary>
/// <remarks>
/// <list type="table">
/// <listheader>
///     <type>Type -</type>
///     <role>Role -</role>
///     <note>Description</note>
/// </listheader>
/// <item>
///     <type><see cref="Developer"/> -</type>
///     <role><c>developer</c> -</role>
///     <note>Instructions to the model that guide the behavior of future <c>assistant</c> messages. Replaces <c>system</c> in o1 and newer models.</note>
/// </item>
/// <item>
///     <type><see cref="System"/> -</type>
///     <role><c>system</c> -</role>
///     <note>Instructions to the model that guide the behavior of future <c>assistant</c> messages.</note>
/// </item>
/// <item>
///     <type><see cref="User"/> -</type>
///     <role><c>user</c> -</role>
///     <note>Input messages from the caller, typically paired with <c>assistant</c> messages in a conversation.</note>
/// </item>
/// <item>
///     <type><see cref="Assistant"/> -</type>
///     <role><c>assistant</c> -</role>
///     <note>
///         Output messages from the model with responses to the <c>user</c> or calls to tools or functions that are
///         needed to continue the logical conversation.
///     </note>
/// </item>
/// <item>
///     <type><see cref="Tool"/> -</type>
///     <role><c>tool</c> -</role>
///     <note>
///         Resolution information for a <see cref="ChatToolCall"/> in an earlier
///         <see cref="AssistantChatMessage"/> that was made against a supplied
///         <see cref="ChatTool"/>.
///     </note>
/// </item>
/// <item>
///     <type><see cref="Function"/> -</type>
///     <role><c>function</c> -</role>
///     <note>
///         Resolution information for a <see cref="ChatFunctionCall"/> in an earlier
///         <see cref="AssistantChatMessage"/> that was made against a supplied
///         <see cref="ChatFunction"/>. Note that <c>functions</c> are deprecated in favor of
///         <c>tool_calls</c>.
///     </note>
/// </item>
/// </list>
/// </remarks>
[CodeGenType("ChatCompletionRole")]
public enum ChatMessageRole
{
    /// <summary>
    /// The <c>system</c> role, which provides instructions to the model that guide the behavior of future
    /// <c>assistant</c> messages
    /// </summary>
    [CodeGenMember("System")]
    System,

    /// <summary>
    /// The <c>user</c> role that provides input from the caller as a prompt for model responses.
    /// </summary>
    [CodeGenMember("User")]
    User,

    /// <summary>
    /// The <c>assistant</c> role that provides output from the model that either issues completions in response to
    /// <c>user</c> messages or calls provided <c>tools</c> or <c>functions</c>.
    /// </summary>
    [CodeGenMember("Assistant")]
    Assistant,

    /// <summary>
    /// The <c>tool</c> role that provides resolving information to prior <c>tool_calls</c> made by the model against
    /// supplied <c>tools</c>.
    /// </summary>
    [CodeGenMember("Tool")]
    Tool,

    /// <summary>
    /// <para>
    /// The <c>function</c> role that provides resolving information to a prior <c>function_call</c> made by the model
    /// against a definition supplied in <c>functions</c>.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <c>functions</c> are deprecated in favor of <c>tools</c> and supplying <c>tools</c> will result in
    /// <c>tool_calls</c> that must be resolved via the <c>tool</c> role rather than a <c>function_call</c> resolved
    /// by a <c>function</c> role message.
    /// </remarks>
    [CodeGenMember("Function")]
    Function,

    /// <summary>
    /// The <c>developer</c> role, which provides instructions to the model that guide the behavior of future
    /// <c>assistant</c> messages
    /// </summary>
    /// <remarks>
    /// <c>developer</c> replaces <c>system</c> when using o1 and newer models.
    /// </remarks>
    [Experimental("OPENAI001")]
    [CodeGenMember("Developer")]
    Developer,
}