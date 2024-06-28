using System;
using System.ClientModel;
using System.Diagnostics;
using System.Text.Json;

#nullable enable

namespace OpenAI.Assistants;

internal class AssistantCollectionPageToken : OpenAIPageToken
{
    public AssistantCollectionPageToken(int? limit, string? order, string? after, string? before)
        : base(limit, order, after, before)
    {
    }

    public override OpenAIPageToken? GetNextPageToken(bool hasMore, string? lastId)
         => GetNextPageToken(Limit, Order, lastId, Before, hasMore);

    // Convenience - first page request
    public static AssistantCollectionPageToken FromOptions(AssistantCollectionOptions options)
        => new(options?.PageSize, options?.Order?.ToString(), options?.AfterId, options?.BeforeId);

    // Convenience - continuation page request
    public static AssistantCollectionPageToken FromToken(ContinuationToken token)
    {
        if (token is AssistantCollectionPageToken pageToken)
        {
            return pageToken;
        }

        BinaryData data = token.ToBytes();

        if (data.ToMemory().Length == 0)
        {
            return new(default, default, default, default);
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

    // Protocol
    public static AssistantCollectionPageToken FromOptions(int? limit, string? order, string? after, string? before)
        => new AssistantCollectionPageToken(limit, order, after, before);

    private static AssistantCollectionPageToken? GetNextPageToken(int? limit, string? order, string? after, string? before, bool hasMore)
    {
        if (!hasMore || after is null)
        {
            return null;
        }

        return new AssistantCollectionPageToken(limit, order, after, before);
    }
}