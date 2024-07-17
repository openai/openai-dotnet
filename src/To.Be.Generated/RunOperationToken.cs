using System;
using System.ClientModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

#nullable enable

namespace OpenAI.Assistants;

internal class RunOperationToken : ContinuationToken
{
    public RunOperationToken(string threadId, string runId)
    {
        ThreadId = threadId;
        RunId = runId;
    }

    public string ThreadId { get; }

    public string RunId { get; }

    public override BinaryData ToBytes()
    {
        using MemoryStream stream = new();
        using Utf8JsonWriter writer = new(stream);

        writer.WriteStartObject();

        writer.WriteString("threadId", ThreadId);
        writer.WriteString("runId", RunId);

        writer.WriteEndObject();

        writer.Flush();
        stream.Position = 0;

        return BinaryData.FromStream(stream);
    }

    public static RunOperationToken FromToken(ContinuationToken continuationToken)
    {
        if (continuationToken is RunOperationToken token)
        {
            return token;
        }

        BinaryData data = continuationToken.ToBytes();

        if (data.ToMemory().Length == 0)
        {
            throw new ArgumentException("Failed to create RunOperationToken from provided continuationToken.", nameof(continuationToken));
        }

        Utf8JsonReader reader = new(data);

        string threadId = null!;
        string runId = null!;

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
                case "threadId":
                    reader.Read();
                    Debug.Assert(reader.TokenType == JsonTokenType.String);
                    threadId = reader.GetString()!;
                    break;

                case "runId":
                    reader.Read();
                    Debug.Assert(reader.TokenType == JsonTokenType.String);
                    threadId = reader.GetString()!;
                    break;

                default:
                    throw new JsonException($"Unrecognized property '{propertyName}'.");
            }
        }

        if (threadId is null || runId is null)
        {
            throw new ArgumentException("Failed to create RunOperationToken from provided continuationToken.", nameof(continuationToken));
        }

        return new(threadId, runId);
    }
}

