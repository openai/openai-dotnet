
using System.Collections.Generic;

namespace OpenAI.Responses
{
    public class GetResponseOptions
    {
        public int? StartingAfter { get; set; }

        /// <summary>
        /// Gets or sets whether streaming is enabled for the response. This corresponds to the "stream" property in the JSON representation.
        /// </summary>
        public bool StreamingEnabled { get; set; }

        /// <summary>
        /// Gets or sets the list of fields to include in the response. This corresponds to the "include" query parameter.
        /// </summary>
        public IEnumerable<IncludedResponseProperty> IncludedProperties { get; set;}

        /// <summary>
        /// Gets or sets whether to include obfuscation data in the response. This corresponds to the "include_obfuscation" property in the JSON representation.
        /// </summary>
        public bool? IncludeObfuscation { get; set; }
    }
}   