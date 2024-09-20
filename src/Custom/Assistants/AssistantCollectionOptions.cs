using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

/// <summary> The options to configure how <see cref="Assistant"/> objects are retrieved and paginated. </summary>
[Experimental("OPENAI001")]
public class AssistantCollectionOptions
{
    /// <summary> Initializes a new instance of <see cref="AssistantCollectionOptions"/>. </summary>
    public AssistantCollectionOptions() { }

    /// <summary> 
    ///     A limit on the number of <see cref="Assistant"/> objects to be returned per page.
    /// </summary>
    public int? PageSizeLimit { get; set; }

    /// <summary>
    ///     The order in which to retrieve <see cref="Assistant"/> objects when sorted by their
    ///     <see cref="Assistant.CreatedAt"/> timestamp.
    /// </summary>
    public AssistantCollectionOrder? Order { get; set; }

    /// <summary>
    ///     The <see cref="Assistant.Id"/> used to retrieve the page of <see cref="Assistant"/> objects that come
    ///     after this one.
    /// </summary>
    public string AfterId { get; set; }

    /// <summary>
    ///     The <see cref="Assistant.Id"/> used to retrieve the page of <see cref="Assistant"/> objects that come
    ///     before this one.
    /// </summary>
    public string BeforeId { get; set; }
}
