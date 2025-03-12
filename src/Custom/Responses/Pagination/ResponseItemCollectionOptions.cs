using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

/// <summary> The options to configure how <see cref="ResponseItem"/> objects are retrieved and paginated. </summary>
[Experimental("OPENAI001")]
public class ResponseItemCollectionOptions
{
    /// <summary> Initializes a new instance of <see cref="ResponseItemCollectionOptions"/>. </summary>
    public ResponseItemCollectionOptions() { }

    /// <summary> 
    ///     A limit on the number of <see cref="Response"/> objects to be returned per page.
    /// </summary>
    public int? PageSizeLimit { get; set; }

    /// <summary>
    ///     The order in which to retrieve <see cref="Response"/> objects when sorted by their
    ///     <see cref="Response.CreatedAt"/> timestamp.
    /// </summary>
    public ResponseItemCollectionOrder? Order { get; set; }

    /// <summary>
    ///     The <see cref="Response.Id"/> used to retrieve the page of <see cref="Response"/> objects that come
    ///     after this one.
    /// </summary>
    public string AfterId { get; set; }

    /// <summary>
    ///     The <see cref="Response.Id"/> used to retrieve the page of <see cref="Response"/> objects that come
    ///     before this one.
    /// </summary>
    public string BeforeId { get; set; }
}
