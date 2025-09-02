using System;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[CodeGenType("Annotation")]
[CodeGenVisibility(nameof(Kind), CodeGenVisibility.Public)]
public partial class ResponseMessageAnnotation
{
}
