using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenAI.Examples;

/// <summary>
/// Pipeline policy that adds the Stainless polling headers.
/// If customPollIntervalMs is provided, it sets X-Stainless-Custom-Poll-Interval as well.
/// </summary>
public sealed class StainlessPollingHeadersPolicy : PipelinePolicy
{
    private readonly int? _customPollIntervalMs;

    public StainlessPollingHeadersPolicy(int? customPollIntervalMs = null)
    {
        _customPollIntervalMs = customPollIntervalMs;
    }

    public override async ValueTask ProcessAsync(PipelineMessage message, IReadOnlyList<PipelinePolicy> pipeline, int currentIndex)
    {
        AddHeaders(message);
        await ProcessNextAsync(message, pipeline, currentIndex);
    }

    public override void Process(PipelineMessage message, IReadOnlyList<PipelinePolicy> pipeline, int currentIndex)
    {
        AddHeaders(message);
        ProcessNext(message, pipeline, currentIndex);
    }

    private void AddHeaders(PipelineMessage message)
    {
        PipelineRequestHeaders headers = message.Request.Headers;

        headers.Set("X-Stainless-Poll-Helper", "true");
        if (_customPollIntervalMs is int ms)
        {
            headers.Set("X-Stainless-Custom-Poll-Interval", ms.ToString());
        }
    }
}
