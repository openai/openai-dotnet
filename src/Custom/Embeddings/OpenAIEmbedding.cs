using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Buffers.Text;
using System.ClientModel.Primitives;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace OpenAI.Embeddings;

// CUSTOM: Renamed.
/// <summary> Represents an embedding vector returned by embedding endpoint. </summary>
[CodeGenType("Embedding")]
[CodeGenSuppress("OpenAIEmbedding", typeof(int), typeof(BinaryData))]
public partial class OpenAIEmbedding
{
    private ReadOnlyMemory<float> _vector;

    // CUSTOM: Made private. The value of the embedding is publicly exposed as ReadOnlyMemory<float> instead of BinaryData.
    [CodeGenMember("Embedding")]
    private BinaryData EmbeddingProperty { get; }

    // CUSTOM: Made private. This property does not add value in the context of a strongly-typed class.
    [CodeGenMember("Object")]
    private string Object { get; } = "embedding";

    // CUSTOM: Added logic to handle additional custom properties.
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
    internal OpenAIEmbedding(int index, BinaryData embeddingProperty, string @object, in JsonPatch patch)
    {
        Index = index;
        EmbeddingProperty = embeddingProperty;
        Object = @object;
        _patch = patch;

        // Handle additional custom properties.
        _vector = ConvertToVectorOfFloats(embeddingProperty);
    }
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

    // CUSTOM: Entirely custom constructor used by the Model Factory.
    internal OpenAIEmbedding(int index, ReadOnlyMemory<float> vector)
    {
        Index = index;
        _vector = vector;
    }

    // CUSTOM: Added as a public, custom method. For slightly better performance, the embedding is always requested as a base64-encoded
    // string and then manually transformed into a more user-friendly ReadOnlyMemory<float>.
    /// <summary>
    /// Gets the embedding vector as a list of floats.
    /// </summary>
    /// <returns>A read-only memory segment of floats representing the embedding vector.</returns>
    public ReadOnlyMemory<float> ToFloats() => _vector;

    private static ReadOnlyMemory<float> ConvertToVectorOfFloats(BinaryData binaryData)
    {
        ReadOnlySpan<byte> bytes = binaryData.ToMemory().Span;

        // Remove quotes around base64 string.
        if (bytes.Length > 2 && bytes[0] == (byte)'"' && bytes[bytes.Length - 1] == (byte)'"')
        {
            return ConvertFromBase64(bytes);
        }
        return ConvertFromJsonArray(binaryData);
    }

    private static ReadOnlyMemory<float> ConvertFromBase64(ReadOnlySpan<byte> base64)
    {
        base64 = base64.Slice(1, base64.Length - 2);

        // Decode base64 string to bytes.
        byte[] bytes = null;
        try
        {
            bytes = ArrayPool<byte>.Shared.Rent(Base64.GetMaxDecodedFromUtf8Length(base64.Length));
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

            return new ReadOnlyMemory<float>(vector);
        }
        finally
        {
            if (bytes is not null)
            {
                ArrayPool<byte>.Shared.Return(bytes);
            }
        }

        static void ThrowInvalidData()
            => throw new FormatException("The input is not a valid Base64 string of encoded floats.");
    }

    private static ReadOnlyMemory<float> ConvertFromJsonArray(BinaryData jsonArray)
    {
        using JsonDocument document = JsonDocument.Parse(jsonArray);
        JsonElement array = document.RootElement;
        if (array.ValueKind != JsonValueKind.Array)
        {
            throw new FormatException("The input is not a valid JSON array");
        }

        int arrayLength = array.GetArrayLength();
        float[] vector = new float[arrayLength];
        int index = 0;
        try
        {
            foreach (JsonElement value in array.EnumerateArray())
            {
                vector[index++] = value.GetSingle();
            }
            return vector.AsMemory();
        }
        catch
        {
            throw new FormatException("The input is not a valid JSON array of float values");
        }
    }
}
