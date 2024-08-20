using System;
using System.Collections.Generic;

namespace OpenAI.Embeddings;

[CodeGenModel("CreateEmbeddingRequest")]
[CodeGenSuppress("EmbeddingGenerationOptions", typeof(BinaryData), typeof(InternalCreateEmbeddingRequestModel))]
public partial class EmbeddingGenerationOptions
{
    // CUSTOM:
    // - Made internal. This value comes from a parameter on the client method.
    // - Added setter.
    /// <summary>
    /// Input text to embed, encoded as a string or array of tokens. To embed multiple inputs in a
    /// single request, pass an array of strings or array of token arrays. Each input must not exceed
    /// the max input tokens for the model (8191 tokens for `text-embedding-ada-002`) and cannot be an
    /// empty string.
    /// [Example Python code](https://github.com/openai/openai-cookbook/blob/main/examples/How_to_count_tokens_with_tiktoken.ipynb)
    /// for counting tokens.
    /// <para>
    /// To assign an object to this property use <see cref="BinaryData.FromObjectAsJson{T}(T, System.Text.Json.JsonSerializerOptions?)"/>.
    /// </para>
    /// <para>
    /// To assign an already formatted json string to this property use <see cref="BinaryData.FromString(string)"/>.
    /// </para>
    /// <para>
    /// <remarks>
    /// Supported types:
    /// <list type="bullet">
    /// <item>
    /// <description><see cref="string"/></description>
    /// </item>
    /// <item>
    /// <description><see cref="IList{T}"/> where <c>T</c> is of type <see cref="string"/></description>
    /// </item>
    /// <item>
    /// <description><see cref="IList{T}"/> where <c>T</c> is of type <see cref="long"/></description>
    /// </item>
    /// <item>
    /// <description><see cref="IList{T}"/> where <c>T</c> is of type <c>IList{long}</c></description>
    /// </item>
    /// </list>
    /// </remarks>
    /// Examples:
    /// <list type="bullet">
    /// <item>
    /// <term>BinaryData.FromObjectAsJson("foo")</term>
    /// <description>Creates a payload of "foo".</description>
    /// </item>
    /// <item>
    /// <term>BinaryData.FromString("\"foo\"")</term>
    /// <description>Creates a payload of "foo".</description>
    /// </item>
    /// <item>
    /// <term>BinaryData.FromObjectAsJson(new { key = "value" })</term>
    /// <description>Creates a payload of { "key": "value" }.</description>
    /// </item>
    /// <item>
    /// <term>BinaryData.FromString("{\"key\": \"value\"}")</term>
    /// <description>Creates a payload of { "key": "value" }.</description>
    /// </item>
    /// </list>
    /// </para>
    /// </summary>
    internal BinaryData Input { get; set; }

    // CUSTOM:
    // - Made internal. The model is specified by the client.
    // - Added setter.
    /// <summary>
    /// ID of the model to use. You can use the [List models](/docs/api-reference/models/list) API to
    /// see all of your available models, or see our [Model overview](/docs/models/overview) for
    /// descriptions of them.
    /// </summary>
    internal InternalCreateEmbeddingRequestModel Model { get; set; }

    // CUSTOM: Made internal. We always request the embedding as a base64-encoded string for better performance.
    /// <summary>
    /// The format to return the embeddings in. Can be either `float` or
    /// [`base64`](https://pypi.org/project/pybase64/).
    /// </summary>
    internal InternalCreateEmbeddingRequestEncodingFormat? EncodingFormat { get; set; }

    // CUSTOM: Made public now that there are no required properties.
    /// <summary> Initializes a new instance of <see cref="EmbeddingGenerationOptions"/>. </summary>
    public EmbeddingGenerationOptions()
    {
    }

    // CUSTOM: Renamed.
    /// <summary>
    ///     A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
    ///     <see href="https://platform.openai.com/docs/guides/safety-best-practices/end-user-ids">Learn more</see>.
    /// </summary>
    [CodeGenMember("User")]
    public string EndUserId { get; set; }
}