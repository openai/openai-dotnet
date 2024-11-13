using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

/// <summary> The options to configure how <see cref="ThreadMessage"/> objects are retrieved and paginated. </summary>
[Experimental("OPENAI001")]
public class MessageCollectionOptions
{
    /// <summary> Initializes a new instance of <see cref="MessageCollectionOptions"/>. </summary>
    public MessageCollectionOptions() { }

    /// <summary> 
    ///     A limit on the number of <see cref="ThreadMessage"/> objects to be returned per page.
    /// </summary>
    public int? PageSizeLimit { get; set; }

    /// <summary>
    ///     The order in which to retrieve <see cref="ThreadMessage"/> objects when sorted by their
    ///     <see cref="ThreadMessage.CreatedAt"/> timestamp.
    /// </summary>
    public MessageCollectionOrder? Order { get; set; }

    /// <summary>
    ///     The <see cref="ThreadMessage.Id"/> used to retrieve the page of <see cref="ThreadMessage"/> objects that come
    ///     after this one.
    /// </summary>
    public string AfterId { get; set; }

    /// <summary>
    ///     The <see cref="ThreadMessage.Id"/> used to retrieve the page of <see cref="ThreadMessage"/> objects that come
    ///     before this one.
    /// </summary>
    public string BeforeId { get; set; }
}