using System;
using System.ClientModel;

namespace OpenAI.Embeddings;

[CodeGenType("CreateEmbeddingRequest")]
[CodeGenVisibility(nameof(EmbeddingGenerationOptions), CodeGenVisibility.Public)]
[CodeGenSuppress(nameof(EmbeddingGenerationOptions), typeof(BinaryData), typeof(InternalCreateEmbeddingRequestModel))]
public partial class EmbeddingGenerationOptions
{
    // CUSTOM:
    // - Made internal. This value comes from a parameter on the client method.
    // - Added setter.
    [CodeGenMember("Input")]
    internal BinaryData Input { get; set; }

    // CUSTOM:
    // - Made internal. The model is specified by the client.
    // - Added setter.
    [CodeGenMember("Model")]
    internal InternalCreateEmbeddingRequestModel Model { get; set; }

    // CUSTOM: Made internal. We always request the embedding as a base64-encoded string for better performance.
    [CodeGenMember("EncodingFormat")]
    internal InternalCreateEmbeddingRequestEncodingFormat? EncodingFormat { get; set; }

    // CUSTOM: Renamed.
    /// <summary>
    ///     A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
    ///     <see href="https://platform.openai.com/docs/guides/safety-best-practices/end-user-ids">Learn more</see>.
    /// </summary>
    [CodeGenMember("User")]
    public string EndUserId { get; set; }

    internal BinaryContent ToBinaryContent() => BinaryContent.Create(this, ModelSerializationExtensions.WireOptions);
}