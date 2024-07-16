namespace OpenAI.Assistants;

/// <summary>
/// Represents addition options available when requesting a collection of <see cref="ThreadMessage"/> instances.
/// </summary>
public class MessageCollectionOptions
{
    /// <summary>
    /// Creates a new instance of <see cref="MessageCollectionOptions"/>.
    /// </summary>
    public MessageCollectionOptions() { }

    /// <summary>
    /// The <c>order</c> that results should appear in the list according to
    /// their <c>created_at</c> timestamp.
    /// </summary>
    public ListOrder? Order { get; init; }

    /// <summary>
    /// The number of values to return in a page result.
    /// </summary>
    public int? PageSize { get; init; }

    /// <summary>
    /// The id of the item preceeding the first item in the collection.
    /// </summary>
    public string AfterId { get; init; }

    /// <summary>
    /// The id of the item following the last item in the collection.
    /// </summary>
    public string BeforeId { get; init; }
}
