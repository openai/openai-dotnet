using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Text.Json;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.Assistants;

// Protocol method version
internal class AssistantCollectionPageResult : PageResult
{
    private readonly string? _lastId;

    private readonly Func<string?, Task<AssistantCollectionPageResult>> _getNextAsync;
    private readonly Func<string?, AssistantCollectionPageResult> _getNext;

    private AssistantCollectionPageResult(
        bool hasNext,
        string? lastId,
        PipelineResponse response,
        Func<string?, Task<AssistantCollectionPageResult>> getNextAsync,
        Func<string?, AssistantCollectionPageResult> getNext)
        : base(hasNext, response)
    {
        _lastId = lastId;

        _getNextAsync = getNextAsync;
        _getNext = getNext;
    }

    public string? LastId { get { return _lastId; } }

    protected override async Task<PageResult> GetNextResultAsyncCore()
        => await _getNextAsync(_lastId).ConfigureAwait(false);

    protected override PageResult GetNextResultCore()
        => _getNext(_lastId);

    public static AssistantCollectionPageResult Create(ClientResult result,
        Func<string?, Task<AssistantCollectionPageResult>> getNextAsync,
        Func<string?, AssistantCollectionPageResult> getNext)
    {
        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        bool hasMore = doc.RootElement.GetProperty("has_more"u8).GetBoolean();
        string lastId = doc.RootElement.GetProperty("last_id"u8).GetString()!;

        return new(hasMore, lastId, response, getNextAsync, getNext);
    }
}
