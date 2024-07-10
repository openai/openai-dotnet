using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI;

internal class PollingInterval
{
    private const int DefaultWaitMilliseconds = 1000;

    public void Wait()
    {
        Thread.Sleep(DefaultWaitMilliseconds);
    }

    public async Task WaitAsync()
    {
        await Task.Delay(DefaultWaitMilliseconds);
    }
}
