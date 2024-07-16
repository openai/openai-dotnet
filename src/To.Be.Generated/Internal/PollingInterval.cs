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

    private readonly TimeSpan _interval;

    public PollingInterval(TimeSpan? interval = default)
    {
        _interval = interval ?? new TimeSpan(DefaultWaitMilliseconds);
    }

    public async Task WaitAsync()
    {
        await Task.Delay(_interval);
    }

    public void Wait()
    {
        Thread.Sleep(_interval);
    }
}
