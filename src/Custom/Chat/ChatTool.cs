using System;

namespace OpenAI.Chat;

/// <summary>
///     A tool that the model may call.
///     <list>
///         <item>
///             Call <see cref="CreateFunctionTool(string, string, BinaryData, bool?)"/> to create a
///             <see cref="ChatTool"/> representing a function that the model may call.
///         </item>
///     </list>
/// </summary>
[CodeGenType("ChatCompletionTool")]
public partial class ChatTool
{
    // CUSTOM: Made internal.
    [CodeGenMember("Function")]
    internal InternalFunctionDefinition Function { get; }

    // CUSTOM: Made internal.
    internal ChatTool(InternalFunctionDefinition function)
    {
        Argument.AssertNotNull(function, nameof(function));

        Function = function;
    }
    // CUSTOM: Public type applied.
    /// <summary> The kind of tool. </summary>
    [CodeGenMember("Type")]
    public ChatToolKind Kind { get; } = ChatToolKind.Function;

    // CUSTOM: Spread.
    /// <summary> The name of the function. </summary>
    /// <remarks> Present when <see cref="Kind"/> is <see cref="ChatToolKind.Function"/>. </remarks>
    public string FunctionName => Function?.Name;

    // CUSTOM: Spread.
    /// <summary>
    ///     The description of what the function does, which is used by the model to choose when and how to call the
    ///     function.
    /// </summary>
    /// <remarks> Present when <see cref="Kind"/> is <see cref="ChatToolKind.Function"/>. </remarks>
    public string FunctionDescription => Function?.Description;

    // CUSTOM: Spread.
    /// <summary>
    ///     The parameters that the function accepts, which are described as a JSON schema. If omitted, this
    ///     defines a function with an empty parameter list. Learn more in the
    ///     <see href="https://platform.openai.com/docs/api-reference/chat/docs/guides/function-calling">function calling guide</see>
    ///     and the
    ///     <see href="https://json-schema.org/understanding-json-schema">JSON schema reference documentation</see>.
    /// </summary>
    /// <remarks> Present when <see cref="Kind"/> is <see cref="ChatToolKind.Function"/>. </remarks>
    public BinaryData FunctionParameters => Function?.Parameters;

    // CUSTOM: Spread.
    /// <summary>
    ///     <para>
    ///         Whether to enable strict schema adherence when generating the function call. If set to <c>true</c>, the
    ///         model will follow the exact schema defined in <see cref="FunctionParameters"/>
    ///     </para>
    ///     <para>
    ///         Only a subset of the JSON schema specification is supported when this is set to <c>true</c>. Learn more
    ///         about structured outputs in the
    ///         <see href="https://platform.openai.com/docs/api-reference/chat/docs/guides/function-calling">function calling guide</see>.
    ///     </para>
    /// </summary>
    /// <remarks> Present when <see cref="Kind"/> is <see cref="ChatToolKind.Function"/>. </remarks>
    public bool? FunctionSchemaIsStrict => Function?.Strict;

    /// <summary> Creates a new <see cref="ChatTool"/> representing a function that the model may call. </summary>
    /// <param name="functionName"> The name of the function. </param>
    /// <param name="functionDescription">
    ///     The description of what the function does, which is used by the model to choose when and how to call the
    ///     function.
    /// </param>
    /// <param name="functionParameters">
    ///     <para>
    ///         The parameters that the function accepts, which are described as a JSON schema. If omitted, this
    ///         defines a function with an empty parameter list. Learn more in the
    ///         <see href="https://platform.openai.com/docs/api-reference/chat/docs/guides/function-calling">function calling guide</see>
    ///         and the
    ///         <see href="https://json-schema.org/understanding-json-schema">JSON schema reference documentation</see>.
    ///     </para>
    ///     <para>
    ///         You can easily create a JSON schema via the factory methods of the <see cref="BinaryData"/> class, such
    ///         as <see cref="BinaryData.FromBytes(byte[])"/> or <see cref="BinaryData.FromString(string)"/>. For
    ///         example, the following code defines a simple schema for a function that takes a customer's order ID as
    ///         a <c>string</c> parameter:
    ///         <code>
    ///             BinaryData functionParameters = BinaryData.FromBytes("""
    ///                 {
    ///                     "type": "object",
    ///                     "properties": {
    ///                         "order_id": {
    ///                             "type": "string",
    ///                             "description": "The customer's order ID."
    ///                         }
    ///                     },
    ///                     "required": ["order_id"],
    ///                     "additionalProperties": false
    ///                 }
    ///                 """u8.ToArray());
    ///         </code>
    ///     </para>
    /// </param>
    /// <param name="functionSchemaIsStrict">
    ///     <para>
    ///         Whether to enable strict schema adherence when generating the function call. If set to <c>true</c>, the
    ///         model will follow the exact schema defined in <paramref name="functionParameters"/>.
    ///     </para>
    ///     <para>
    ///         Only a subset of the JSON schema specification is supported when this is set to <c>true</c>. Learn more
    ///         about structured outputs in the
    ///         <see href="https://platform.openai.com/docs/api-reference/chat/docs/guides/function-calling">function calling guide</see>.
    ///     </para>
    /// </param>
    public static ChatTool CreateFunctionTool(string functionName, string functionDescription = null, BinaryData functionParameters = null, bool? functionSchemaIsStrict = null)
    {
        Argument.AssertNotNull(functionName, nameof(functionName));

        return new(
            function: new(
                description: functionDescription,
                name: functionName,
                strict: functionSchemaIsStrict,
                parameters: functionParameters,
                additionalBinaryDataProperties: null),
            kind: ChatToolKind.Function,
            additionalBinaryDataProperties: null);
    }
}
