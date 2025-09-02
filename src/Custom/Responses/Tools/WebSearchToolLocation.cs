using System;
using System.Collections.Generic;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("Location")]
public partial class WebSearchToolLocation
{
    public static WebSearchToolApproximateLocation CreateApproximateLocation(string country = null, string region = null, string city = null, string timezone = null)
    {
        return new WebSearchToolApproximateLocation(
            kind: InternalWebSearchUserLocationKind.Approximate,
            additionalBinaryDataProperties: null,
            country: country,
            region: region,
            city: city,
            timezone: timezone);
    }
}