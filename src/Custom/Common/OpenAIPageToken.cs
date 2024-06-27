using System;
using System.ClientModel;
using System.IO;
using System.Text.Json;

#nullable enable

namespace OpenAI;

internal abstract class OpenAIPageToken : ContinuationToken
{
    public OpenAIPageToken(int? limit, string? order, string? after, string? before)
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

    public abstract OpenAIPageToken? GetNextPageToken(bool hasMore, string? lastId);

    //public static OpenAIPageToken FromOptions(int? limit, string? order, string? after, string? before)
    //    => new OpenAIPageToken(limit, order, after, before);

    //public static OpenAIPageToken FromToken(ContinuationToken token)
    //{
    //    if (token is OpenAIPageToken openAIPageToken)
    //    {
    //        return openAIPageToken;
    //    }

    //    BinaryData data = token.ToBytes();
        
    //    if (data.ToMemory().Length == 0)
    //    {
    //        return new(default, default, default, default);
    //    }

    //    Utf8JsonReader reader = new(data);

    //    int? limit = null;
    //    string? order = null;
    //    string? after = null;
    //    string? before = null;

    //    reader.Read();
    //    Debug.Assert(reader.TokenType == JsonTokenType.StartObject);

    //    while (reader.Read())
    //    {
    //        if (reader.TokenType == JsonTokenType.EndObject)
    //        {
    //            break;
    //        }

    //        Debug.Assert(reader.TokenType == JsonTokenType.PropertyName);
    //        string propertyName = reader.GetString()!;

    //        switch (propertyName)
    //        {
    //            case "limit":
    //                reader.Read();
    //                Debug.Assert(reader.TokenType == JsonTokenType.Number);
    //                limit = reader.GetInt32();
    //                break;
    //            case "order":
    //                reader.Read();
    //                Debug.Assert(reader.TokenType == JsonTokenType.String);
    //                order = reader.GetString();
    //                break;
    //            case "after":
    //                reader.Read();
    //                Debug.Assert(reader.TokenType == JsonTokenType.String);
    //                after = reader.GetString();
    //                break;
    //            case "before":
    //                reader.Read();
    //                Debug.Assert(reader.TokenType == JsonTokenType.String);
    //                before = reader.GetString();
    //                break;
    //            default:
    //                throw new JsonException($"Unrecognized property '{propertyName}'.");
    //        }
    //    }

    //    return new(limit, order, after, before);
    //}

    //public static OpenAIPageToken? GetNextPageToken(int? limit, string? order, string? after, string? before, bool hasMore)
    //{
    //    if (!hasMore || after is null)
    //    {
    //        return null;
    //    }

    //    return new OpenAIPageToken(limit, order, after, before);
    //}
}