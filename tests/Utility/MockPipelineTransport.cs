using System;
using System.ClientModel.Primitives;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.Tests;

public class MockPipelineTransport : PipelineTransport
{
    public MockPipelineRequest MockRequest { get; set; }
    public MockPipelineResponse MockResponse { get; set; }

    public MockPipelineTransport(BinaryData requestData, BinaryData responseData)
    {
        MockRequest = new MockPipelineRequest(requestData);
        MockResponse = new MockPipelineResponse(200);
        MockResponse.SetContent(responseData.ToArray(), bufferImmediately: true);
    }

    public MockPipelineTransport(MockPipelineResponse response)
    {
        MockRequest = new MockPipelineRequest();
        MockResponse = response;
    }

    protected override PipelineMessage CreateMessageCore()
    {
        return new MockPipelineMessage(MockRequest);
    }

    protected override void ProcessCore(PipelineMessage message)
    {
        (message as MockPipelineMessage)!.SetResponse(MockResponse);
    }

    protected override ValueTask ProcessCoreAsync(PipelineMessage message)
    {
        (message as MockPipelineMessage)!.SetResponse(MockResponse);
        return ValueTask.CompletedTask;
    }
}

