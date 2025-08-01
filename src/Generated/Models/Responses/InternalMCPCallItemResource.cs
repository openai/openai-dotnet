// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;

namespace OpenAI.Responses
{
    internal partial class InternalMCPCallItemResource : ResponseItem
    {
        internal InternalMCPCallItemResource(string id, string serverLabel, string name, string arguments) : base(InternalItemType.McpCall, id)
        {
            ServerLabel = serverLabel;
            Name = name;
            Arguments = arguments;
        }

        internal InternalMCPCallItemResource(InternalItemType kind, string id, IDictionary<string, BinaryData> additionalBinaryDataProperties, string serverLabel, string name, string arguments, string output, string error) : base(kind, id, additionalBinaryDataProperties)
        {
            ServerLabel = serverLabel;
            Name = name;
            Arguments = arguments;
            Output = output;
            Error = error;
        }

        public string ServerLabel { get; }

        public string Name { get; }

        public string Arguments { get; }

        public string Output { get; }

        public string Error { get; }
    }
}
