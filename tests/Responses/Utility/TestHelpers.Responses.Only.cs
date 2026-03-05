using NUnit.Framework;
using OpenAI.Responses;
using System;
using System.ClientModel;

[assembly: LevelOfParallelism(8)]

namespace OpenAI.Tests;

internal static partial class TestHelpers
{
    public partial class TestScenario
    {
        public string ModelId { get; }

        public TestScenario(string modelId)
        {
            ModelId = modelId;
        }

        public virtual object CreateClient(string model, ApiKeyCredential credential, ResponsesClientOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
