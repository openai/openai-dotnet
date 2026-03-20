using System;
using System.ClientModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

#nullable enable

namespace OpenAI.Batch;

internal class CreateBatchOperationToken : ContinuationToken
{
    public CreateBatchOperationToken(string batchId)
    {
        BatchId = batchId;
    }

    public string BatchId { get; }

    public override BinaryData ToBytes()
    {
        using MemoryStream stream = new();
        using Utf8JsonWriter writer = new(stream);
        writer.WriteStartObject();

        writer.WriteString("batchId", BatchId);

        writer.WriteEndObject();

        writer.Flush();
        stream.Position = 0;

        return BinaryData.FromStream(stream);
    }

    public static CreateBatchOperationToken FromToken(ContinuationToken continuationToken)
    {
        if (continuationToken is CreateBatchOperationToken token)
        {
            return token;
        }

        BinaryData data = continuationToken.ToBytes();

        if (data.ToMemory().Length == 0)
        {
            throw new ArgumentException("Failed to create CreateBatchOperationToken from provided continuationToken.", nameof(continuationToken));
        }

        Utf8JsonReader reader = new(data);

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
                case "batchId":
                    reader.Read();
                    Debug.Assert(reader.TokenType == JsonTokenType.String);
                    batchId = reader.GetString()!;
                    break;

                default:
                    throw new JsonException($"Unrecognized property '{propertyName}'.");
            }
        }

        if (batchId is null)
        {
            throw new ArgumentException("Failed to create CreateBatchOperationToken from provided continuationToken.", nameof(continuationToken));
        }

        return new(batchId);
    }
}

