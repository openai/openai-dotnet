
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
    }
}   