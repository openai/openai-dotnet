using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;

#nullable enable

namespace OpenAI.FineTuning;

[Experimental("OPENAI001")]
internal class FineTuningCollectionPageToken : ContinuationToken
{
    protected FineTuningCollectionPageToken(int? limit, string? after)
    {
        Limit = limit;
        After = after;
    }

    public int? Limit { get; }

    public string? After { get; }

    public static FineTuningCollectionPageToken FromToken(ContinuationToken pageToken)
    {
        if (pageToken is FineTuningCollectionPageToken token)
        {
            return token;
        }

        BinaryData data = pageToken.ToBytes();

        if (data.ToMemory().Length == 0)
        {
            throw new ArgumentException("Failed to create FineTuningCollectionPageToken from provided pageToken.", nameof(pageToken));
        }

        Utf8JsonReader reader = new(data);

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

        return new(limit, after);
    }

    public static FineTuningCollectionPageToken FromOptions(int? limit, string? after)
        => new(limit, after);

    public static FineTuningCollectionPageToken? FromResponse(ClientResult result, int? limit)
    {
        PipelineResponse response = result.GetRawResponse();
        using JsonDocument doc = JsonDocument.Parse(response.Content);
        string? lastId;
        bool hasLast = TryGetLastId(doc, out lastId);
        bool hasMore = doc.RootElement.GetProperty("has_more"u8).GetBoolean();

        if (!hasMore || lastId is null)
        {
            return null;
        }

        return new(limit, lastId);
    }

    private static bool TryGetLastId(JsonDocument json, out string? lastId)
    {
        if (!json.RootElement.GetProperty("has_more"u8).GetBoolean())
        {
            lastId = null;
            return false;
        }

        if (json?.RootElement.TryGetProperty("data", out JsonElement dataElement) == true
            && dataElement.EnumerateArray().LastOrDefault().TryGetProperty("id", out JsonElement idElement) == true)
        {
            lastId = idElement.GetString();
            return true;
        }

        lastId = null;
        return false;
    }
}
