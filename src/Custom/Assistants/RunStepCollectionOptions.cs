using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

/// <summary> The options to configure how <see cref="RunStep"/> objects are retrieved and paginated. </summary>
[Experimental("OPENAI001")]
public class RunStepCollectionOptions
{
    /// <summary> Initializes a new instance of <see cref="RunStepCollectionOptions"/>. </summary>
    public RunStepCollectionOptions() { }

    /// <summary> 
    ///     A limit on the number of <see cref="RunStep"/> objects to be returned per page.
    /// </summary>
    public int? PageSizeLimit { get; set; }

    /// <summary>
    ///     The order in which to retrieve <see cref="RunStep"/> objects when sorted by their
    ///     <see cref="RunStep.CreatedAt"/> timestamp.
    /// </summary>
    public RunStepCollectionOrder? Order { get; set; }

    /// <summary>
    ///     The <see cref="RunStep.Id"/> used to retrieve the page of <see cref="RunStep"/> objects that come
    ///     after this one.
    /// </summary>
    public string AfterId { get; set; }

    /// <summary>
    ///     The <see cref="RunStep.Id"/> used to retrieve the page of <see cref="RunStep"/> objects that come
    ///     before this one.
    /// </summary>
    public string BeforeId { get; set; }
}
