using System;
using System.ClientModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

#nullable enable

namespace OpenAI.VectorStores;

internal class CreateBatchFileJobOperationToken : ContinuationToken
{
    public CreateBatchFileJobOperationToken(string vectorStoreId, string batchId)
    {
        VectorStoreId = vectorStoreId;
        BatchId = batchId;
    }

    public string VectorStoreId { get; }

    public string BatchId { get; }

    public override BinaryData ToBytes()
    {
        using MemoryStream stream = new();
        using Utf8JsonWriter writer = new(stream);
        writer.WriteStartObject();

        writer.WriteString("vectorStoreId", VectorStoreId);
        writer.WriteString("batchId", BatchId);

        writer.WriteEndObject();

        writer.Flush();
        stream.Position = 0;

        return BinaryData.FromStream(stream);
    }

    public static CreateBatchFileJobOperationToken FromToken(ContinuationToken continuationToken)
    {
        if (continuationToken is CreateBatchFileJobOperationToken token)
        {
            return token;
        }

        BinaryData data = continuationToken.ToBytes();

        if (data.ToMemory().Length == 0)
        {
            throw new ArgumentException("Failed to create AddFileBatchToVectorStoreOperationToken from provided continuationToken.", nameof(continuationToken));
        }

        Utf8JsonReader reader = new(data);

        string vectorStoreId = null!;
        string batchId = null!;

        reader.Read();

        Debug.Assert(reader.TokenType == JsonTokenType.StartObject);

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            Debug.Assert(reader.TokenType == JsonTokenType.PropertyName);

            string propertyName = reader.GetString()!;

            switch (propertyName)
            {
                case "vectorStoreId":
                    reader.Read();
                    Debug.Assert(reader.TokenType == JsonTokenType.String);
                    vectorStoreId = reader.GetString()!;
                    break;

                case "batchId":
                    reader.Read();
                    Debug.Assert(reader.TokenType == JsonTokenType.String);
                    batchId = reader.GetString()!;
                    break;

                default:
                    throw new JsonException($"Unrecognized property '{propertyName}'.");
            }
        }

        if (vectorStoreId is null || batchId is null)
        {
            throw new ArgumentException("Failed to create AddFileBatchToVectorStoreOperationToken from provided continuationToken.", nameof(continuationToken));
        }

        return new(vectorStoreId, batchId);
    }
}

