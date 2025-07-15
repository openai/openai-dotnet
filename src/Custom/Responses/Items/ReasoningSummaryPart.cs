using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[CodeGenType("ReasoningItemSummaryPart")]
public partial class ReasoningSummaryPart
{
    // CUSTOM: Added as a constructor alternative for discoverability
    /// <summary>
    /// Creates a new instance of <see cref="ReasoningSummaryTextPart"/>.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    /// <remarks>
    /// This is equivalent to creating an instance of <see cref="ReasoningSummaryTextPart"/> via its constructor.
    /// </remarks>
    public static ReasoningSummaryPart CreateTextPart(string text)
        => new ReasoningSummaryTextPart(text);
}
