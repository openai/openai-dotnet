namespace OpenAI.Responses;

[CodeGenType("ResponsesWebSearchLocation")]
public partial class WebSearchToolLocation
{
    public static WebSearchToolLocation CreateApproximateLocation(string country = null, string region = null, string city = null, string timezone = null)
    {
        return new InternalResponsesWebSearchApproximateLocation(
            type: "approximate",
            additionalBinaryDataProperties: null,
            country,
            region,
            city,
            timezone);
    }
}