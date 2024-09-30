using NUnit.Framework;
using OpenAI.Models;
using OpenAI.Tests.Utility;
using System.ClientModel;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAI.Tests.Models;

[TestFixture(true)]
[TestFixture(false)]
[Parallelizable(ParallelScope.All)]
[Category("Models")]
[Category("Smoke")]
public class ModelsMockTests : SyncAsyncTestBase
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
        OpenAIModelClient client = new OpenAIModelClient(s_fakeCredential, clientOptions);

        OpenAIModel modelInfo = IsAsync
            ? await client.GetModelAsync("model_name")
            : client.GetModel("model_name");

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
        OpenAIModelClient client = new OpenAIModelClient(s_fakeCredential, clientOptions);

        OpenAIModelCollection modelInfoCollection = IsAsync
            ? await client.GetModelsAsync()
            : client.GetModels();
        OpenAIModel modelInfo = modelInfoCollection.Single();

        Assert.That(modelInfo.CreatedAt.ToUnixTimeSeconds(), Is.EqualTo(1704096000));
    }

    private OpenAIClientOptions GetClientOptionsWithMockResponse(int status, string content)
    {
        MockPipelineResponse response = new MockPipelineResponse(status);
        response.SetContent(content);

        return new OpenAIClientOptions()
        {
            Transport = new MockPipelineTransport(response)
        };
    }
}
