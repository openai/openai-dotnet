﻿using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenAI.Tests;

internal partial class TestPipelinePolicy : PipelinePolicy
{
    private Action<PipelineMessage> _processMessageAction;

    public TestPipelinePolicy(Action<PipelineMessage> processMessageAction)
    {
        _processMessageAction = processMessageAction;
    }

    public override void Process(PipelineMessage message, IReadOnlyList<PipelinePolicy> pipeline, int currentIndex)
    {
        _processMessageAction(message); // for request
        ProcessNext(message, pipeline, currentIndex);
        _processMessageAction(message); // for response
    }

    public override async ValueTask ProcessAsync(PipelineMessage message, IReadOnlyList<PipelinePolicy> pipeline, int currentIndex)
    {
        _processMessageAction(message); // for request
        await ProcessNextAsync(message, pipeline, currentIndex);
        _processMessageAction(message); // for response
    }
}