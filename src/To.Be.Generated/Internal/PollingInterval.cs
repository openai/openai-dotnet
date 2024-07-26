using System;
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

    public async Task WaitAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(_interval, cancellationToken);
    }

    public void Wait()
    {
        Thread.Sleep(_interval);
    }
}
