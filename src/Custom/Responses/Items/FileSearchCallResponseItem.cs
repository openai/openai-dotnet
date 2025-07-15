using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[CodeGenType("FileSearchToolCallItemResource")]
public partial class FileSearchCallResponseItem
{
    // CUSTOM: Retain optionality of OpenAPI read-only property value
    [CodeGenMember("Status")]
    public FileSearchCallStatus? Status { get; }

    // CUSTOM: For reuse as an input model
    internal FileSearchCallResponseItem(IEnumerable<string> queries, IEnumerable<FileSearchCallResult> results = null)
        : this(
              kind: InternalItemType.FileSearchCall,
              id: null,
              additionalBinaryDataProperties: null,
              queries.ToList(),
              results.ToList(),
              status: null)
    { }
}