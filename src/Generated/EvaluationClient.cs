// <auto-generated/>

#nullable disable

using System;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Evals
{
    [Experimental("OPENAI001")]
    public partial class EvaluationClient
    {
        private readonly Uri _endpoint;

        protected EvaluationClient()
        {
        }

        internal EvaluationClient(ClientPipeline pipeline, Uri endpoint)
        {
            _endpoint = endpoint;
            Pipeline = pipeline;
        }

        public ClientPipeline Pipeline { get; }
    }
}
