using OpenAI.Internal;
using System;

namespace OpenAI.Chat;

/// <summary>
///     The format that the model should output.
///     <list>
///         <item>
///             Call <see cref="CreateTextFormat()"/> to create a <see cref="ChatResponseFormat"/> requesting plain
///             text.
///         </item>
///         <item>
///             Call <see cref="CreateJsonObjectFormat()"/> to create a <see cref="ChatResponseFormat"/> requesting
///             valid JSON, a.k.a. JSON mode.
///         </item>
///         <item>
///             Call <see cref="CreateJsonSchemaFormat(string, BinaryData, string, bool?)"/> to create a
///             <see cref="ChatResponseFormat"/> requesting adherence to the specified JSON schema,
///             a.k.a. structured outputs.
///         </item>
///     </list>
/// </summary>
[CodeGenType("DotNetChatResponseFormat")]
public partial class ChatResponseFormat
{
    /// <summary> Creates a new <see cref="ChatResponseFormat"/> requesting plain text. </summary>
    public static ChatResponseFormat CreateTextFormat() => new InternalDotNetChatResponseFormatText();

    /// <summary> Creates a new <see cref="ChatResponseFormat"/> requesting valid JSON, a.k.a. JSON mode. </summary>
    public static ChatResponseFormat CreateJsonObjectFormat() => new InternalDotNetChatResponseFormatJsonObject();

    /// <summary>
    ///     Creates a new <see cref="ChatResponseFormat"/> requesting adherence to the specified JSON schema,
    ///     a.k.a. structured outputs.
    /// </summary>
    /// <param name="jsonSchemaFormatName"> The name of the response format. </param>
    /// <param name="jsonSchema">
    ///     <para>
    ///         The schema of the response format, described as a JSON schema. Learn more in the
    ///         <see href="https://platform.openai.com/docs/guides/structured-outputs">structured outputs guide</see>.
    ///         and the
    ///         <see href="https://json-schema.org/understanding-json-schema">JSON schema reference documentation</see>.
    ///     </para>
    ///     <para>
    ///         You can easily create a JSON schema via the factory methods of the <see cref="BinaryData"/> class, such
    ///         as <see cref="BinaryData.FromBytes(byte[])"/> or <see cref="BinaryData.FromString(string)"/>. For
    ///         example, the following code defines a simple schema for step-by-step responses to math problems:
    ///         <code>
    ///             BinaryData jsonSchema = BinaryData.FromBytes("""
    ///                 {
    ///                     "type": "object",
    ///                     "properties": {
    ///                         "steps": {
    ///                             "type": "array",
    ///                             "items": {
    ///                                 "type": "object",
    ///                                 "properties": {
    ///                                     "explanation": {"type": "string"},
    ///                                     "output": {"type": "string"}
    ///                                 },
    ///                                 "required": ["explanation", "output"],
    ///                                 "additionalProperties": false
    ///                             }
    ///                         },
    ///                         "final_answer": {"type": "string"}
    ///                     },
    ///                     "required": ["steps", "final_answer"],
    ///                     "additionalProperties": false
    ///                 }
    ///                 """U8.ToArray());
    ///         </code>
    ///     </para>
    /// </param>
    /// <param name="jsonSchemaFormatDescription">
    ///     The description of what the response format is for, which is used by the model to determine how to respond
    ///     in the format.
    /// </param>
    /// <param name="jsonSchemaIsStrict">
    ///     <para>
    ///         Whether to enable strict schema adherence when generating the response. If set to <c>true</c>, the
    ///         model will follow the exact schema defined in <paramref name="jsonSchema"/>.
    ///     </para>
    ///     <para>
    ///         Only a subset of the JSON schema specification is supported when this is set to <c>true</c>. Learn more
    ///         in the
    ///         <see href="https://platform.openai.com/docs/guides/structured-outputs">structured outputs guide</see>.
    ///     </para>
    /// </param>
    /// <exception cref="ArgumentNullException"> <paramref name="jsonSchemaFormatName"/> or <paramref name="jsonSchema"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="jsonSchemaFormatName"/> is an empty string, and was expected to be non-empty. </exception>
    public static ChatResponseFormat CreateJsonSchemaFormat(string jsonSchemaFormatName, BinaryData jsonSchema, string jsonSchemaFormatDescription = null, bool? jsonSchemaIsStrict = null)
    {
        Argument.AssertNotNullOrEmpty(jsonSchemaFormatName, nameof(jsonSchemaFormatName));
        Argument.AssertNotNull(jsonSchema, nameof(jsonSchema));

        InternalDotNetChatResponseFormatJsonSchemaJsonSchema internalSchema = new(
            jsonSchemaFormatDescription,
            jsonSchemaFormatName,
            schema: jsonSchema,
            jsonSchemaIsStrict,
            additionalBinaryDataProperties: null);

        return new InternalDotNetChatResponseFormatJsonSchema(internalSchema);
    }
}