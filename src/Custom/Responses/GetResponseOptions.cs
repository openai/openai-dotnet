
namespace OpenAI.Responses
{
    [CodeGenType("GetResponseOptions")]
    public partial class GetResponseOptions
    {
        public GetResponseOptions(string responseId)
        {
            ResponseId = responseId;
        }

        /// <summary>
        /// Gets or sets the unique identifier of the response to retrieve. This corresponds to the "responseId" path parameter in the API endpoint.
        /// </summary>
        [CodeGenMember("ResponseId")]
        public string ResponseId { get; set; }

        // /// <summary>
        // /// Gets or sets the cursor for pagination to retrieve results starting after the specified ID. This corresponds to the "starting_after" query parameter.
        // /// </summary>
        // public int? StartingAfter { get; set; }

        // /// <summary>
        // /// Gets or sets whether streaming is enabled for the response. This corresponds to the "stream" query parameter.
        // /// </summary>
        // public bool StreamingEnabled { get; set; }

        // /// <summary>
        // /// Gets or sets the list of fields to include in the response. This corresponds to the "include" query parameter.
        // /// </summary>
        // public IEnumerable<IncludedResponseProperty> IncludedProperties { get; set;}

        // /// <summary>
        // /// Gets or sets whether to include obfuscation data in the response. This corresponds to the "include_obfuscation" query parameter.
        // /// </summary>
        // public bool? IncludeObfuscation { get; set; }
    }
}   