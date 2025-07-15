using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[CodeGenType("DotnetResponseWebSearchLocation")]
public partial class WebSearchUserLocation
{
    public static WebSearchUserLocation CreateApproximateLocation(string country = null, string region = null, string city = null, string timezone = null)
    {
        return new InternalDotnetResponseWebSearchApproximateLocation(
            InternalWebSearchUserLocationKind.Approximate,
            additionalBinaryDataProperties: null,
            country,
            region,
            city,
            timezone);
    }
}