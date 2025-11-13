
namespace OpenAI.Responses
{
    public class GetResponseOptions
    {
        public GetResponseOptions(string responseId)
        {
            ResponseId = responseId;
        }

        public string ResponseId { get; set; }

        public int? StartingAfter { get; set; }

        public bool Stream { get; set; }
    }
}   