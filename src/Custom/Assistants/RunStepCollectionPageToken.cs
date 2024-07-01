using System;
using System.ClientModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

#nullable enable

namespace OpenAI.Assistants;

internal class RunStepCollectionPageToken : OpenAIPageToken
{
    public RunStepCollectionPageToken(string threadId, string runId, int? limit, string? order, string? after, string? before)
        : base(limit, order, after, before)
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
        writer.WriteString("runId", ThreadId);

        if (Limit.HasValue)
        {
            writer.WriteNumber("limit", Limit.Value);
        }

        if (Order is not null)
        {
            writer.WriteString("order", Order);
        }

        if (After is not null)
        {
            writer.WriteString("after", After);
        }

        if (Before is not null)
        {
            writer.WriteString("before", Before);
        }

        writer.WriteEndObject();

        writer.Flush();
        stream.Position = 0;

        return BinaryData.FromStream(stream);
    }
    
    public override OpenAIPageToken? GetNextPageToken(bool hasMore, string? lastId)
         => GetNextPageToken(ThreadId, RunId, Limit, Order, lastId, Before, hasMore);

    // Convenience - continuation page request
    public static RunStepCollectionPageToken FromToken(ContinuationToken pageToken)
    {
        if (pageToken is RunStepCollectionPageToken token)
        {
            return token;
        }

        BinaryData data = pageToken.ToBytes();

        if (data.ToMemory().Length == 0)
        {
            throw new ArgumentException("Failed to create MessageCollectionPageToken from provided pageToken.", nameof(pageToken));
        }

        Utf8JsonReader reader = new(data);

        string threadId = null!;
        string runId = null!;
        int? limit = null;
        string? order = null;
        string? after = null;
        string? before = null;

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
                case "limit":
                    reader.Read();
                    Debug.Assert(reader.TokenType == JsonTokenType.Number);
                    limit = reader.GetInt32();
                    break;
                case "order":
                    reader.Read();
                    Debug.Assert(reader.TokenType == JsonTokenType.String);
                    order = reader.GetString();
                    break;
                case "after":
                    reader.Read();
                    Debug.Assert(reader.TokenType == JsonTokenType.String);
                    after = reader.GetString();
                    break;
                case "before":
                    reader.Read();
                    Debug.Assert(reader.TokenType == JsonTokenType.String);
                    before = reader.GetString();
                    break;
                default:
                    throw new JsonException($"Unrecognized property '{propertyName}'.");
            }
        }

        if (threadId is null || runId is null)
        {
            throw new ArgumentException("Failed to create MessageCollectionPageToken from provided pageToken.", nameof(pageToken));
        }

        return new(threadId, runId, limit, order, after, before);
    }

    // Protocol
    public static RunStepCollectionPageToken FromOptions(string threadId, string runId, int? limit, string? order, string? after, string? before)
        => new RunStepCollectionPageToken(threadId, runId, limit, order, after, before);

    private static RunStepCollectionPageToken? GetNextPageToken(string threadId, string runId, int? limit, string? order, string? after, string? before, bool hasMore)
    {
        if (!hasMore || after is null)
        {
            return null;
        }

        return new RunStepCollectionPageToken(threadId, runId, limit, order, after, before);
    }
}