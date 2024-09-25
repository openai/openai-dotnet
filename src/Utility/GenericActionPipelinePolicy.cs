using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenAI;

internal partial class GenericActionPipelinePolicy(Action<PipelineMessage> processMessageAction) : PipelinePolicy
{
    public override void Process(PipelineMessage message, IReadOnlyList<PipelinePolicy> pipeline, int currentIndex)
    {
        processMessageAction(message);
        if (currentIndex < pipeline.Count - 1)
        {
            pipeline[currentIndex + 1].Process(message, pipeline, currentIndex + 1);
        }
    }

    public override async ValueTask ProcessAsync(PipelineMessage message, IReadOnlyList<PipelinePolicy> pipeline, int currentIndex)
    {
        processMessageAction(message);
        if (currentIndex < pipeline.Count - 1)
        {
            await pipeline[currentIndex + 1].ProcessAsync(message, pipeline, currentIndex + 1);
        }
    }
}