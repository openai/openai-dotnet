using System.ClientModel.Primitives;

#nullable enable

namespace OpenAI.Tests;

public class MockPipelineMessage : PipelineMessage
{
    protected internal MockPipelineMessage(PipelineRequest request) : base(request)
    {
    }

    public void SetResponse(MockPipelineResponse response)
    {
        Response = response;
    }
}