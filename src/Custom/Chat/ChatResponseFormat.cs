using OpenAI.Models;

namespace OpenAI.Chat;

/// <summary>
/// Represents a requested <c>response_format</c> for the model to use, enabling "JSON mode" for guaranteed valid output.
/// </summary>
/// <remarks>
/// <b>Important</b>: when using JSON mode, the model <b><u>must</u></b> also be instructed to produce JSON via a
/// <c>system</c> or <c>user</c> message.
/// <para>
/// Without this paired, message-based accompaniment, the model may generate an unending stream of whitespace until the
/// generation reaches the token limit, resulting in a long-running and seemingly "stuck" request.
/// </para>
/// <para>
/// Also note that the message content may be partially cut off if <c>finish_reason</c> is <c>length</c>, which
/// indicates that the generation exceeded <c>max_tokens</c> or the conversation exceeded the max context length for
/// the model.
/// </para>
/// </remarks>
[CodeGenModel("CreateChatCompletionRequestResponseFormat")]
public partial class ChatResponseFormat
{
    // CUSTOM: Made internal.

    /// <summary> Must be one of `text` or `json_object`. </summary>
    [CodeGenMember("Type")]
    internal InternalCreateChatCompletionRequestResponseFormatType? Type { get; set; }

    // CUSTOM: Made internal.
    /// <summary> Initializes a new instance of <see cref="ChatResponseFormat"/>. </summary>
    internal ChatResponseFormat()
    {
    }

    internal ChatResponseFormat(InternalCreateChatCompletionRequestResponseFormatType? type)
    {
        Type = type;
    }

    /// <summary> text. </summary>
    public static ChatResponseFormat Text { get; } = new ChatResponseFormat(InternalCreateChatCompletionRequestResponseFormatType.Text);
    /// <summary> json_object. </summary>
    public static ChatResponseFormat JsonObject { get; } = new ChatResponseFormat(InternalCreateChatCompletionRequestResponseFormatType.JsonObject);
}