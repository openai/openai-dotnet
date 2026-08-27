using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;

#nullable enable

namespace OpenAI.FineTuning;

[Experimental("OPENAI001")]
internal class FineTuningCheckpointCollectionPageToken: ContinuationToken
{
    protected FineTuningCheckpointCollectionPageToken(string jobId, int? pageSize, string? afterCheckpointId)
    {
        JobId = jobId;
        PageSize = pageSize;
        AfterCheckpointId = afterCheckpointId;
    }

    public string JobId { get; }

    public int? PageSize { get; }

    public string? AfterCheckpointId { get; }

    public override BinaryData ToBytes()
    {
        using MemoryStream stream = new();
        using Utf8JsonWriter writer = new(stream);

        writer.WriteStartObject();
        writer.WriteString("jobId", JobId);

        if (PageSize.HasValue)
        {
            writer.WriteNumber("limit", PageSize.Value);
        }

        if (AfterCheckpointId is not null)
        {
            writer.WriteString("after", AfterCheckpointId);
        }

        writer.WriteEndObject();

        writer.Flush();
        stream.Position = 0;

        return BinaryData.FromStream(stream);
    }

    public static FineTuningCheckpointCollectionPageToken FromToken(ContinuationToken pageToken)
    {
        if (pageToken is FineTuningCheckpointCollectionPageToken token)
        {
            return token;
        }

        BinaryData data = pageToken.ToBytes();

        if (data.ToMemory().Length == 0)
        {
            throw new ArgumentException($"Failed to create {nameof(FineTuningCheckpointCollectionPageToken)} from provided argument.", nameof(pageToken));
        }

        Utf8JsonReader reader = new(data);

        string jobId = null!;
        int? limit = null;
        string? after = null;

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
                case "jobId":
                    reader.Read();
                    Debug.Assert(reader.TokenType == JsonTokenType.String);
                    jobId = reader.GetString()!;
                    break;
                case "limit":
                    reader.Read();
                    Debug.Assert(reader.TokenType == JsonTokenType.Number);
                    limit = reader.GetInt32();
                    break;
                case "after":
                    reader.Read();
                    Debug.Assert(reader.TokenType == JsonTokenType.String);
                    after = reader.GetString();
                    break;
                default:
                    throw new JsonException($"Unrecognized property '{propertyName}'.");
            }
        }

        if (jobId is null)
        {
            throw new ArgumentException($"Failed to create {nameof(FineTuningCheckpointCollectionPageToken)} from provided argument.", nameof(pageToken));
        }

        return new(jobId, limit, after);
    }

    public static FineTuningCheckpointCollectionPageToken FromOptions(string jobId, int? pageSize, string? afterCheckpointId)
        => new(jobId, pageSize, afterCheckpointId);

    public static FineTuningCheckpointCollectionPageToken? FromResponse(ClientResult result, string jobId, int? pageSize)
    {
        PipelineResponse response = result.GetRawResponse();
        using JsonDocument doc = JsonDocument.Parse(response.Content);
        string lastId = doc.RootElement.GetProperty("last_id"u8).GetString()!;
        bool hasMore = doc.RootElement.GetProperty("has_more"u8).GetBoolean();

        if (!hasMore || lastId is null)
        {
            return null;
        }

        return new(jobId, pageSize, lastId);
    }
}
