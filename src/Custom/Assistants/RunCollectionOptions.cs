using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

/// <summary> The options to configure how <see cref="ThreadRun"/> objects are retrieved and paginated. </summary>
[Experimental("OPENAI001")]
public class RunCollectionOptions
{
    /// <summary> Initializes a new instance of <see cref="RunCollectionOptions"/>. </summary>
    public RunCollectionOptions() { }

    /// <summary> 
    ///     A limit on the number of <see cref="ThreadRun"/> objects to be returned per page.
    /// </summary>
    public int? PageSizeLimit { get; set; }

    /// <summary>
    ///     The order in which to retrieve <see cref="ThreadRun"/> objects when sorted by their
    ///     <see cref="ThreadRun.CreatedAt"/> timestamp.
    /// </summary>
    public RunCollectionOrder? Order { get; set; }

    /// <summary>
    ///     The <see cref="ThreadRun.Id"/> used to retrieve the page of <see cref="ThreadRun"/> objects that come
    ///     after this one.
    /// </summary>
    public string AfterId { get; set; }

    /// <summary>
    ///     The <see cref="ThreadRun.Id"/> used to retrieve the page of <see cref="ThreadRun"/> objects that come
    ///     before this one.
    /// </summary>
    public string BeforeId { get; set; }
}
