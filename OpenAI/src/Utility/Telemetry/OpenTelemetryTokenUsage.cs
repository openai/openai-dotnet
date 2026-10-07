using OpenAI.Chat;
using OpenAI.Responses;
using System;
using System.ClientModel.Primitives;

namespace OpenAI.Telemetry;

#pragma warning disable SCME0001

internal struct OpenTelemetryTokenUsage
{
    public long? InputTokens { get; set; }

    public long? OutputTokens { get; set; }

    public long? InputAudioTokens { get; set; }

    public long? OutputAudioTokens { get; set; }

    public long? CacheReadInputTokens { get; set; }

    public long? CacheWriteInputTokens { get; set; }

    public long? ReasoningOutputTokens { get; set; }

    public static OpenTelemetryTokenUsage FromChat(ChatTokenUsage usage)
    {
        if (usage is null)
        {
            return default;
        }

        var normalized = new OpenTelemetryTokenUsage
        {
            InputTokens = ReadCount(usage.Patch, "$.prompt_tokens"u8, usage.InputTokenCount),
            OutputTokens = ReadCount(usage.Patch, "$.completion_tokens"u8, usage.OutputTokenCount),
            InputAudioTokens = usage.InputTokenDetails is { } input
                ? ReadCount(input.Patch, "$.audio_tokens"u8, GetNonzeroCount(input.AudioTokenCount)) : null,
            OutputAudioTokens = usage.OutputTokenDetails is { } output
                ? ReadCount(output.Patch, "$.audio_tokens"u8, GetNonzeroCount(output.AudioTokenCount)) : null,
            CacheReadInputTokens = usage.InputTokenDetails is { } cached
                ? ReadCount(cached.Patch, "$.cached_tokens"u8, GetNonzeroCount(cached.CachedTokenCount)) : null,
            ReasoningOutputTokens = usage.OutputTokenDetails is { } reasoning
                ? ReadCount(reasoning.Patch, "$.reasoning_tokens"u8, GetNonzeroCount(reasoning.ReasoningTokenCount)) : null,
        };
        normalized.NormalizeSubsets();

        return normalized;
    }

    public static OpenTelemetryTokenUsage FromResponse(ResponseTokenUsage usage)
    {
        if (usage is null)
        {
            return default;
        }

        var normalized = new OpenTelemetryTokenUsage
        {
            InputTokens = ReadCount(usage.Patch, "$.input_tokens"u8, usage.InputTokenCount),
            OutputTokens = ReadCount(usage.Patch, "$.output_tokens"u8, usage.OutputTokenCount),
            CacheReadInputTokens = usage.InputTokenDetails is { } cached
                ? ReadCount(cached.Patch, "$.cached_tokens"u8, GetNonzeroCount(cached.CachedTokenCount)) : null,
            CacheWriteInputTokens = usage.InputTokenDetails is { } written
                ? ReadCount(written.Patch, "$.cache_write_tokens"u8, GetNonzeroCount(written.CacheWriteTokenCount)) : null,
            ReasoningOutputTokens = usage.OutputTokenDetails is { } reasoning
                ? ReadCount(reasoning.Patch, "$.reasoning_tokens"u8, GetNonzeroCount(reasoning.ReasoningTokenCount)) : null,
        };
        normalized.NormalizeSubsets();

        return normalized;
    }

    public static string GetResponseSystemFingerprint(ResponseResult response)
        => response.Patch.TryGetValue("$.system_fingerprint"u8, out string fingerprint) ? fingerprint : null;

    private void NormalizeSubsets()
    {
        InputAudioTokens = GetBoundedCount(InputAudioTokens, InputTokens);
        OutputAudioTokens = GetBoundedCount(OutputAudioTokens, OutputTokens);
        CacheReadInputTokens = GetBoundedCount(CacheReadInputTokens, InputTokens);
        CacheWriteInputTokens = GetBoundedCount(CacheWriteInputTokens, InputTokens);
        ReasoningOutputTokens = GetBoundedCount(ReasoningOutputTokens, OutputTokens);
    }

    private static long? GetBoundedCount(long? count, long? total) => count > total ? null : count;

    private static long? ReadCount(JsonPatch patch, ReadOnlySpan<byte> path, long? fallback)
    {
        // Generated nonnullable properties turn missing fields into zero; the retained JSON preserves availability.
        if (patch.TryGetValue(path, out long count))
        {
            return count >= 0 ? count : null;
        }

        // A nonzero typed value can also come from a manually populated model, but zero cannot prove presence.
        return GetNonzeroCount(fallback ?? 0);
    }

    private static long? GetNonzeroCount(long count) => count > 0 ? count : null;
}

#pragma warning restore SCME0001
