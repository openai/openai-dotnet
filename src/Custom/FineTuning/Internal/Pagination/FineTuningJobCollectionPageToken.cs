using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Diagnostics;
using System.Text.Json;

#nullable enable

namespace OpenAI.FineTuning;

internal class FineTuningJobCollectionPageToken : ContinuationToken
{
    protected FineTuningJobCollectionPageToken(int? limit, string? after)
    {
        Limit = limit;
        After = after;
    }

    public int? Limit { get; }

    public string? After { get; }

    public static FineTuningJobCollectionPageToken FromToken(ContinuationToken pageToken)
    {
        if (pageToken is FineTuningJobCollectionPageToken token)
        {
            return token;
        }

        BinaryData data = pageToken.ToBytes();

        if (data.ToMemory().Length == 0)
        {
            throw new ArgumentException("Failed to create FineTuningJobCollectionPageToken from provided pageToken.", nameof(pageToken));
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

    public static FineTuningJobCollectionPageToken FromOptions(int? limit, string? after)
        => new(limit, after);

    public static FineTuningJobCollectionPageToken? FromResponse(ClientResult result, int? limit)
    {
        PipelineResponse response = result.GetRawResponse();
        using JsonDocument doc = JsonDocument.Parse(response.Content);
        string lastId = doc.RootElement.GetProperty("last_id"u8).GetString()!;
        bool hasMore = doc.RootElement.GetProperty("has_more"u8).GetBoolean();

        if (!hasMore || lastId is null)
        {
            return null;
        }

        return new(limit, lastId);
    }
}
