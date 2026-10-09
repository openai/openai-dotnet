using System.Diagnostics;
using System.Diagnostics.Metrics;

using static OpenAI.Telemetry.OpenTelemetryConstants;

namespace OpenAI.Telemetry;

internal sealed class OpenTelemetryTokenMetrics
{
    private readonly Counter<long> _input;
    private readonly Counter<long> _output;
    private readonly Counter<long> _cacheRead;
    private readonly Counter<long> _cacheWrite;
    private readonly Counter<long> _reasoning;
    private readonly Histogram<long> _operationInput;
    private readonly Histogram<long> _operationOutput;

    public OpenTelemetryTokenMetrics(Meter meter)
    {
        _input = meter.CreateCounter<long>(GenAiClientInferenceUsageInputTokensMetricName, "{token}", "Input tokens used, including cached tokens.");
        _output = meter.CreateCounter<long>(GenAiClientInferenceUsageOutputTokensMetricName, "{token}", "Output tokens used, including reasoning tokens.");
        _cacheRead = meter.CreateCounter<long>(GenAiClientInferenceUsageCacheReadInputTokensMetricName, "{token}", "Input tokens served from a provider-managed cache.");
        _cacheWrite = meter.CreateCounter<long>(GenAiClientInferenceUsageCacheWriteInputTokensMetricName, "{token}", "Input tokens written to a provider-managed cache.");
        _reasoning = meter.CreateCounter<long>(GenAiClientInferenceUsageReasoningOutputTokensMetricName, "{token}", "Output tokens used for reasoning.");
        _operationInput = CreateHistogram(meter, GenAiClientInferenceOperationInputTokensMetricName, "Input tokens used per inference operation.");
        _operationOutput = CreateHistogram(meter, GenAiClientInferenceOperationOutputTokensMetricName, "Output tokens used per inference operation.");
    }

    public bool Enabled => (_input.Enabled) || (_output.Enabled) || (_cacheRead.Enabled) || (_cacheWrite.Enabled)
        || (_reasoning.Enabled) || (_operationInput.Enabled) || (_operationOutput.Enabled);

    public void Record(OpenTelemetryTokenUsage usage, TagList tags)
    {
        RecordTotal(_input, _operationInput, usage.InputTokens, usage.InputAudioTokens, tags);
        RecordTotal(_output, _operationOutput, usage.OutputTokens, usage.OutputAudioTokens, tags);
        RecordCounter(_cacheRead, usage.CacheReadInputTokens, "unknown", tags);
        RecordCounter(_cacheWrite, usage.CacheWriteInputTokens, "unknown", tags);
        RecordCounter(_reasoning, usage.ReasoningOutputTokens, "unknown", tags);
    }

    private static Histogram<long> CreateHistogram(Meter meter, string name, string description)
        => meter.CreateHistogram<long>(name, "{token}", description, advice: new InstrumentAdvice<long>
        {
            HistogramBucketBoundaries = [1, 4, 16, 64, 256, 1024, 4096, 16384, 65536, 262144, 1048576, 4194304, 16777216, 67108864],
        });

    private static void RecordTotal(Counter<long> counter, Histogram<long> histogram, long? total, long? audio, TagList tags)
    {
        if ((total is null) || (total < 0))
        {
            return;
        }

        histogram.Record(total.Value, tags);

        if ((audio >= 0) && (audio <= total))
        {
            RecordCounter(counter, audio, "audio", tags);

            if (audio < total)
            {
                RecordCounter(counter, total - audio, "unknown", tags);
            }
        }
        else
        {
            RecordCounter(counter, total, "unknown", tags);
        }
    }

    private static void RecordCounter(Counter<long> counter, long? count, string modality, TagList tags)
    {
        if ((count is null) || (count < 0))
        {
            return;
        }

        tags.Add(GenAiTokenModalityKey, modality);
        counter.Add(count.Value, tags);
    }
}
