using System;
using System.ClientModel;
using System.Diagnostics;
using System.Text.Json;

#nullable enable

namespace OpenAI.Assistants;

internal class GetAssistantsPageToken : OpenAIPageToken
{
    public GetAssistantsPageToken(int? limit, string? order, string? after, string? before)
        : base(limit, order, after, before)
    {
    }

    public override OpenAIPageToken? GetNextPageToken(bool hasMore, string? lastId)
         => GetNextPageToken(Limit, Order, lastId, Before, hasMore);

    public static GetAssistantsPageToken FromOptions(int? limit, string? order, string? after, string? before)
        => new GetAssistantsPageToken(limit, order, after, before);

    public static GetAssistantsPageToken FromToken(ContinuationToken token)
    {
        if (token is GetAssistantsPageToken pageToken)
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

    public static GetAssistantsPageToken? GetNextPageToken(int? limit, string? order, string? after, string? before, bool hasMore)
    {
        if (!hasMore || after is null)
        {
            return null;
        }

        return new GetAssistantsPageToken(limit, order, after, before);
    }
}