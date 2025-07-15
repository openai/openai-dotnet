using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[CodeGenType("FileSearchToolCallItemParamResult")]
public partial class FileSearchCallResult
{
    // CUSTOM: Use a plain dictionary.
    [CodeGenMember("Attributes")]
    public IReadOnlyDictionary<string, BinaryData> Attributes { get; }
}