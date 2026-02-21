using OpenAI.Responses;
using System.ClientModel;

namespace OpenAI.Tests;

internal static partial class TestHelpers
{
    public partial class TestScenario
    {
        public static readonly TestScenario Responses = new ResponsesScenario();

        private class ResponsesScenario : TestScenario
        {
            public ResponsesScenario() : base("gpt-4o-mini") { }
            public override object CreateClient(string model, ApiKeyCredential credential, OpenAIClientOptions options)
#pragma warning disable OPENAI003
                => new ResponsesClient(model, credential, ResponsesClientOptions.FromOpenAIClientOptions(options));
#pragma warning restore OPENAI003
        }
    }
}
