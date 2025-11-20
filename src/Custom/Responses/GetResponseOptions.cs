
using System.Collections.Generic;

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

        public IEnumerable<IncludedResponseProperty> IncludedProperties { get; set;}

        public bool? IncludeObfuscation { get; set; }
    }
}   