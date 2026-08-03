using Microsoft.TypeSpec.Generator.Customizations;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
// - Converted to extensible enum.
[CodeGenType("MessageStatus")]
public readonly partial struct MessageStatus { }
