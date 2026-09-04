using System;
using System.ClientModel.Primitives;
using System.Globalization;
using System.Numerics;

namespace OpenAI;

internal sealed class RetryAfterMessageClassifier(PipelineMessageClassifier inner) : PipelineMessageClassifier
{
    // WaitHandle.WaitOne(TimeSpan) is the smaller shared limit of the native
    // synchronous and asynchronous retry timers. Refuse longer server delays
    // rather than overflowing their parser or throwing from a timer.
    private static readonly TimeSpan s_maximumDelay = TimeSpan.FromMilliseconds(int.MaxValue);

    public override bool TryClassify(PipelineMessage message, out bool isError)
        => inner.TryClassify(message, out isError);

    public override bool TryClassify(PipelineMessage message, Exception exception, out bool isRetriable)
    {
        if (message.Response?.Headers.TryGetValue("Retry-After", out string value) == true && CannotHonorDelay(value))
        {
            isRetriable = false;
            return true;
        }

        return inner.TryClassify(message, exception, out isRetriable);
    }

    private static bool CannotHonorDelay(string value)
    {
        // Standard delay-seconds have no fixed digit limit. Parsing without
        // Int32 overflow distinguishes excessive valid integers from bad input.
        if (BigInteger.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out BigInteger seconds))
        {
            return seconds > int.MaxValue / 1000;
        }

        if (!DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset retryAt))
        {
            return false;
        }

        TimeSpan delay = retryAt - DateTimeOffset.UtcNow;
        // The native retry policy parses dates using the current culture. A
        // different interpretation could turn a future minimum into backoff.
        return delay > s_maximumDelay
            || (delay > TimeSpan.Zero
                && (!DateTimeOffset.TryParse(value, out DateTimeOffset nativeRetryAt) || nativeRetryAt != retryAt));
    }
}
