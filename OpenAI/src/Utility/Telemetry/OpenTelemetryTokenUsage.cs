using OpenAI.Chat;
using OpenAI.Responses;
using System;
using System.ClientModel.Primitives;

namespace OpenAI.Telemetry;

#pragma warning disable SCME0001

internal sealed class OpenTelemetryTokenUsage
{
    public static OpenTelemetryTokenUsage Empty { get; } = new();

    public long? InputTokens { get; }
    public long? OutputTokens { get; }
    public long? InputAudioTokens { get; }
    public long? OutputAudioTokens { get; }
    public long? CacheReadInputTokens { get; }
    public long? CacheWriteInputTokens { get; }
    public long? ReasoningOutputTokens { get; }

    public OpenTelemetryTokenUsage(
        long? inputTokens = null,
        long? outputTokens = null,
        long? inputAudioTokens = null,
        long? outputAudioTokens = null,
        long? cacheReadInputTokens = null,
        long? cacheWriteInputTokens = null,
        long? reasoningOutputTokens = null)
    {
        InputTokens = inputTokens;
        OutputTokens = outputTokens;
        InputAudioTokens = GetBoundedCount(inputAudioTokens, inputTokens);
        OutputAudioTokens = GetBoundedCount(outputAudioTokens, outputTokens);
        CacheReadInputTokens = GetBoundedCount(cacheReadInputTokens, inputTokens);
        CacheWriteInputTokens = GetBoundedCount(cacheWriteInputTokens, inputTokens);
        ReasoningOutputTokens = GetBoundedCount(reasoningOutputTokens, outputTokens);
    }

    public static OpenTelemetryTokenUsage FromChat(ChatTokenUsage usage)
    {
        if (usage is null)
        {
            return Empty;
        }

        return new OpenTelemetryTokenUsage(
            inputTokens: ReadCount(usage.Patch, "$.prompt_tokens"u8, usage.InputTokenCount),
            outputTokens: ReadCount(usage.Patch, "$.completion_tokens"u8, usage.OutputTokenCount),
            inputAudioTokens: usage.InputTokenDetails is { } input
                ? ReadCount(input.Patch, "$.audio_tokens"u8, GetNonzeroCount(input.AudioTokenCount)) : null,
            outputAudioTokens: usage.OutputTokenDetails is { } output
                ? ReadCount(output.Patch, "$.audio_tokens"u8, GetNonzeroCount(output.AudioTokenCount)) : null,
            cacheReadInputTokens: usage.InputTokenDetails is { } cached
                ? ReadCount(cached.Patch, "$.cached_tokens"u8, GetNonzeroCount(cached.CachedTokenCount)) : null,
            reasoningOutputTokens: usage.OutputTokenDetails is { } reasoning
                ? ReadCount(reasoning.Patch, "$.reasoning_tokens"u8, GetNonzeroCount(reasoning.ReasoningTokenCount)) : null);
    }

    public static OpenTelemetryTokenUsage FromResponse(ResponseTokenUsage usage)
    {
        if (usage is null)
        {
            return Empty;
        }

        return new OpenTelemetryTokenUsage(
            inputTokens: ReadCount(usage.Patch, "$.input_tokens"u8, usage.InputTokenCount),
            outputTokens: ReadCount(usage.Patch, "$.output_tokens"u8, usage.OutputTokenCount),
            cacheReadInputTokens: usage.InputTokenDetails is { } cached
                ? ReadCount(cached.Patch, "$.cached_tokens"u8, GetNonzeroCount(cached.CachedTokenCount)) : null,
            cacheWriteInputTokens: usage.InputTokenDetails is { } written
                ? ReadCount(written.Patch, "$.cache_write_tokens"u8, GetNonzeroCount(written.CacheWriteTokenCount)) : null,
            reasoningOutputTokens: usage.OutputTokenDetails is { } reasoning
                ? ReadCount(reasoning.Patch, "$.reasoning_tokens"u8, GetNonzeroCount(reasoning.ReasoningTokenCount)) : null);
    }

    public static string GetResponseSystemFingerprint(ResponseResult response)
        => response.Patch.TryGetValue("$.system_fingerprint"u8, out string fingerprint) ? fingerprint : null;

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
