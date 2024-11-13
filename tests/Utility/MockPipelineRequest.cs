using System;
using System.ClientModel;
using System.ClientModel.Primitives;

namespace OpenAI.Tests;

public class MockPipelineRequest : PipelineRequest
{
    protected override string MethodCore { get; set; } = "POST";

    protected override Uri UriCore { get; set; }

    protected override PipelineRequestHeaders HeadersCore { get; } = new MockPipelineRequestHeaders();

    protected override BinaryContent ContentCore { get; set; }

    public MockPipelineRequest()
    {
    }

    public MockPipelineRequest(BinaryData requestData)
    {
        ContentCore = BinaryContent.Create(requestData);
    }

    public override void Dispose()
    {
        ContentCore?.Dispose();
    }
}