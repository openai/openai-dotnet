using System;
using System.ClientModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

#nullable enable

namespace OpenAI.VectorStores;

internal class CreateVectorStoreOperationToken : ContinuationToken
{
    public CreateVectorStoreOperationToken(string vectorStoreId)
    {
        VectorStoreId = vectorStoreId;
    }

    public string VectorStoreId { get; }

    public override BinaryData ToBytes()
    {
        using MemoryStream stream = new();
        using Utf8JsonWriter writer = new(stream);
        writer.WriteStartObject();

        writer.WriteString("vectorStoreId", VectorStoreId);

        writer.WriteEndObject();

        writer.Flush();
        stream.Position = 0;

        return BinaryData.FromStream(stream);
    }

    public static CreateVectorStoreOperationToken FromToken(ContinuationToken continuationToken)
    {
        if (continuationToken is CreateVectorStoreOperationToken token)
        {
            return token;
        }

        BinaryData data = continuationToken.ToBytes();

        if (data.ToMemory().Length == 0)
        {
            throw new ArgumentException("Failed to create CreateVectorStoreOperationToken from provided continuationToken.", nameof(continuationToken));
        }

        Utf8JsonReader reader = new(data);

        string vectorStoreId = null!;

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

                default:
                    throw new JsonException($"Unrecognized property '{propertyName}'.");
            }
        }

        if (vectorStoreId is null)
        {
            throw new ArgumentException("Failed to create CreateVectorStoreOperationToken from provided continuationToken.", nameof(continuationToken));
        }

        return new(vectorStoreId);
    }
}

