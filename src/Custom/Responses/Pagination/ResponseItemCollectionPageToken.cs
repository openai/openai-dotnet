using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

#nullable enable

namespace OpenAI.Responses;

internal class ResponseItemCollectionPageToken : ContinuationToken
{
    protected ResponseItemCollectionPageToken(int? limit, string? order, string? after, string? before)
    {
        Limit = limit;
        Order = order;
        After = after;
        Before = before;
    }

    public int? Limit { get; }

    public string? Order { get; }

    public string? After { get; }

    public string? Before { get; }

    public override BinaryData ToBytes()
    {
        using MemoryStream stream = new();
        using Utf8JsonWriter writer = new(stream);

        writer.WriteStartObject();

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

    public static ResponseItemCollectionPageToken FromToken(ContinuationToken token)
    {
        if (token is ResponseItemCollectionPageToken pageToken)
        {
            return pageToken;
        }

        BinaryData data = token.ToBytes();

        if (data.ToMemory().Length == 0)
        {
            throw new ArgumentException("Failed to create ResponsesItemCollectionPageToken from provided pageToken.", nameof(pageToken));
        }

        Utf8JsonReader reader = new(data);

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

        return new(limit, order, after, before);
    }

    public static ResponseItemCollectionPageToken FromOptions(int? limit, string? order, string? after, string? before)
        => new ResponseItemCollectionPageToken(limit, order, after, before);

    public static ResponseItemCollectionPageToken? FromResponse(ClientResult result, int? limit, string? order, string? before)
    {
        PipelineResponse response = result.GetRawResponse();
        using JsonDocument doc = JsonDocument.Parse(response.Content);
        string lastId = doc.RootElement.GetProperty("last_id"u8).GetString()!;
        bool hasMore = doc.RootElement.GetProperty("has_more"u8).GetBoolean();

        if (!hasMore || lastId is null)
        {
            return null;
        }

        return new(limit, order, lastId, before);
    }
}