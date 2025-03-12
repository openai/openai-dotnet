using System.Collections.Generic;
using System.Linq;

namespace OpenAI.Responses;

// CUSTOM:
// - Renamed.
// - Suppressed constructor in favor of custom constructor with required `id` parameter.
[CodeGenType("ResponsesFileSearchCallItem")]
[CodeGenSuppress(nameof(FileSearchCallResponseItem), typeof(IEnumerable<string>), typeof(IEnumerable<FileSearchCallResult>))]
public partial class FileSearchCallResponseItem
{
    public FileSearchCallResponseItem(string id, IEnumerable<string> queries, IEnumerable<FileSearchCallResult> results) : base(InternalResponsesItemType.FileSearchCall, id)
    {
        Argument.AssertNotNull(id, nameof(id));
        Argument.AssertNotNull(queries, nameof(queries));

        Queries = queries.ToList();
        Results = results?.ToList();
    }
}