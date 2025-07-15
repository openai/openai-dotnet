using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[CodeGenType("DotNetItemReferenceItemResource")]
[CodeGenVisibility(nameof(ReferenceResponseItem), CodeGenVisibility.Internal)]
public partial class ReferenceResponseItem
{
    // CUSTOM: Added to support ease of input model use
    public ReferenceResponseItem(string id)
        : this(InternalItemType.ItemReference, id, additionalBinaryDataProperties: null)
    {
    }
}
