using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace OpenAI.Embeddings;

/// <summary>
/// Represents an embedding vector returned by embedding endpoint.
/// </summary>
[CodeGenModel("Embedding")]
[CodeGenSuppress("Embedding", typeof(int), typeof(BinaryData))]
public partial class Embedding
{
    // CUSTOM: Made private. The value of the embedding is publicly exposed as ReadOnlyMemory<float> instead of BinaryData.
    /// <summary>
    /// The embedding vector, which is a list of floats. The length of vector depends on the model as
    /// listed in the [embedding guide](/docs/guides/embeddings).
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
    /// <description><see cref="IList{T}"/> where <c>T</c> is of type <see cref="double"/></description>
    /// </item>
    /// <item>
    /// <description><see cref="string"/></description>
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
    private BinaryData EmbeddingProperty { get; }

    // CUSTOM: Made private. This property does not add value in the context of a strongly-typed class.
    /// <summary> The object type, which is always "embedding". </summary>
    private InternalEmbeddingObject Object { get; } = InternalEmbeddingObject.Embedding;

    // CUSTOM: Added logic to handle additional custom properties.
    /// <summary> Initializes a new instance of <see cref="Embedding"/>. </summary>
    /// <param name="index"> The index of the embedding in the list of embeddings. </param>
    /// <param name="embeddingProperty">
    /// The embedding vector, which is a list of floats. The length of vector depends on the model as
    /// listed in the [embedding guide](/docs/guides/embeddings).
    /// </param>
    /// <param name="object"> The object type, which is always "embedding". </param>
    /// <param name="serializedAdditionalRawData"> Keeps track of any properties unknown to the library. </param>
    internal Embedding(int index, BinaryData embeddingProperty, InternalEmbeddingObject @object, IDictionary<string, BinaryData> serializedAdditionalRawData)
    {
        Index = (int)index;
        EmbeddingProperty = embeddingProperty;
        Object = @object;
        _serializedAdditionalRawData = serializedAdditionalRawData;

        // Handle additional custom properties.
        Vector = ConvertToVectorOfFloats(embeddingProperty);
    }

    // CUSTOM: Added as a public, custom property. For slightly better performance, the embedding is always requested as a base64-encoded
    // string and then manually transformed into a more user-friendly ReadOnlyMemory<float>.
    /// <summary>
    /// The embedding vector, which is a list of floats.
    /// </summary>
    public ReadOnlyMemory<float> Vector { get; }

    // CUSTOM: Implemented custom logic to transform from BinaryData to ReadOnlyMemory<float>.
    private ReadOnlyMemory<float> ConvertToVectorOfFloats(BinaryData binaryData)
    {
        ReadOnlySpan<byte> base64 = binaryData.ToMemory().Span;

        // Remove quotes around base64 string.
        if (base64.Length < 2 || base64[0] != (byte)'"' || base64[base64.Length - 1] != (byte)'"')
        {
            ThrowInvalidData();
        }
        base64 = base64.Slice(1, base64.Length - 2);

        // Decode base64 string to bytes.
        byte[] bytes = ArrayPool<byte>.Shared.Rent(Base64.GetMaxDecodedFromUtf8Length(base64.Length));
        OperationStatus status = Base64.DecodeFromUtf8(base64, bytes.AsSpan(), out int bytesConsumed, out int bytesWritten);
        if (status != OperationStatus.Done || bytesWritten % sizeof(float) != 0)
        {
            ThrowInvalidData();
        }

        // Interpret bytes as floats
        float[] vector = new float[bytesWritten / sizeof(float)];
        bytes.AsSpan(0, bytesWritten).CopyTo(MemoryMarshal.AsBytes(vector.AsSpan()));
        if (!BitConverter.IsLittleEndian)
        {
            Span<int> ints = MemoryMarshal.Cast<float, int>(vector.AsSpan());
#if NET8_0_OR_GREATER
            BinaryPrimitives.ReverseEndianness(ints, ints);
#else
            for (int i = 0; i < ints.Length; i++)
            {
                ints[i] = BinaryPrimitives.ReverseEndianness(ints[i]);
            }
#endif
        }

        ArrayPool<byte>.Shared.Return(bytes);
        return new ReadOnlyMemory<float>(vector);

        static void ThrowInvalidData() =>
            throw new FormatException("The input is not a valid Base64 string of encoded floats.");
    }
}