using System;
using System.ClientModel.Primitives;

#nullable enable

namespace OpenAI;

internal class OpenAIPageToken
{
    public OpenAIPageToken(int? pageSize,
        string? order,
        string? after,
        string? before,
        string? lastSeenId)
    {
        PageSize = pageSize;
        Order = order;
        After = after;
        Before = before;
        LastSeenId = lastSeenId;
    }

    public int? PageSize { get; }

    public string? Order { get; }

    public string? After { get; }

    public string? Before { get; }

    public string? LastSeenId { get; }

    public BinaryData ToBytes()
    {
        throw new NotImplementedException();
    }

    public static OpenAIPageToken FromBytes(BinaryData data)
    {
        throw new NotImplementedException();
    }

    public static BinaryData? GetNextPageToken(bool hasMore, string? lastId)
    {
        if (!hasMore || lastId is null)
        {
            return null;
        }

        throw new NotImplementedException();
    }
}