using Microsoft.ClientModel.TestFramework;
using Microsoft.ClientModel.TestFramework.Mocks;
using NUnit.Framework;
using OpenAI.Models;
using OpenAI.Tests.Utility;
using System.ClientModel;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAI.Tests.Models;

[Parallelizable(ParallelScope.All)]
[Category("Models")]
[Category("Smoke")]
public class ModelsMockTests : ClientTestBase
{
    private static readonly ApiKeyCredential s_fakeCredential = new ApiKeyCredential("key");

    public ModelsMockTests(bool isAsync)
        : base(isAsync)
    {
    }

    [Test]
    public async Task GetModelDeserializesCreatedAt()
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "created": 1704096000
        }
        """);
        OpenAIModelClient client = CreateProxyFromClient(new OpenAIModelClient(s_fakeCredential, clientOptions));

        OpenAIModel modelInfo = await client.GetModelAsync("model_name");

        Assert.That(modelInfo.CreatedAt.ToUnixTimeSeconds(), Is.EqualTo(1704096000));
    }

    [Test]
    public async Task GetModelsDeserializesCreatedAt()
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "data": [
                {
                    "created": 1704096000
                }
            ]
        }
        """);
        OpenAIModelClient client = CreateProxyFromClient(new OpenAIModelClient(s_fakeCredential, clientOptions));

        OpenAIModelCollection modelInfoCollection = await client.GetModelsAsync();
        OpenAIModel modelInfo = modelInfoCollection.Single();

        Assert.That(modelInfo.CreatedAt.ToUnixTimeSeconds(), Is.EqualTo(1704096000));
    }

    private OpenAIClientOptions GetClientOptionsWithMockResponse(int status, string content)
    {
        MockPipelineResponse response = new MockPipelineResponse(status).WithContent(content);

        return new OpenAIClientOptions()
        {
            Transport = new MockPipelineTransport(_ => response)
            {
                ExpectSyncPipeline = !IsAsync
            }
        };
    }
}
