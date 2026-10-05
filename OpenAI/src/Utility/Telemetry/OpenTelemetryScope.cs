using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using OpenAI.Responses;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;

using static OpenAI.Telemetry.OpenTelemetryConstants;

namespace OpenAI.Telemetry;

internal partial class OpenTelemetryScope : IDisposable
{
    private static readonly ActivitySource s_chatSource = new ActivitySource("OpenAI.ChatClient");
    private static readonly Meter s_chatMeter = new Meter("OpenAI.ChatClient");
    private static readonly ActivitySource s_responsesSource = new ActivitySource("OpenAI.ResponsesClient");
    private static readonly Meter s_responsesMeter = new Meter("OpenAI.ResponsesClient");

    private static readonly Histogram<double> s_chatDuration = CreateDurationHistogram(s_chatMeter);
    private static readonly Histogram<long> s_chatTokens = CreateTokenHistogram(s_chatMeter);
    private static readonly Histogram<double> s_responsesDuration = CreateDurationHistogram(s_responsesMeter);
    private static readonly Histogram<long> s_responsesTokens = CreateTokenHistogram(s_responsesMeter);
    private static readonly OpenTelemetryTokenMetrics s_chatInferenceTokens = new(s_chatMeter);
    private static readonly OpenTelemetryTokenMetrics s_responsesInferenceTokens = new(s_responsesMeter);
    private static readonly Histogram<double> s_responsesFirstChunk = CreateDurationHistogram(
        s_responsesMeter, "gen_ai.client.operation.time_to_first_chunk", "Time to receive the first response stream event.");
    private static readonly Histogram<double> s_responsesOutputChunk = CreateDurationHistogram(
        s_responsesMeter, "gen_ai.client.operation.time_per_output_chunk", "Time between successive output chunks.");
    // Telemetry sources are linked into tests, where the generated internal ResponseItemKind.Compaction is unavailable.
    private static readonly ResponseItemKind s_compactionItemKind = new("compaction");
    // The format discriminators are internal, including when these sources are linked into tests.
    private static readonly Type s_chatTextFormatType = ChatResponseFormat.CreateTextFormat().GetType();
    private static readonly Type s_chatJsonObjectFormatType = ChatResponseFormat.CreateJsonObjectFormat().GetType();
    private static readonly Type s_chatJsonSchemaFormatType = ChatResponseFormat.CreateJsonSchemaFormat("telemetry", BinaryData.FromString("{}")).GetType();

    private readonly ActivitySource _activitySource;
    private readonly Histogram<double> _durationHistogram;
    private readonly Histogram<long> _tokenHistogram;
    private readonly OpenTelemetryTokenMetrics _inferenceTokens;
    private readonly string _operationName;
    private readonly string _serverAddress;
    private readonly int _serverPort;
    private readonly string _requestModel;
    private readonly bool _useLatestSemanticConventions;
    private readonly bool _includeErrorDescription;
    private readonly ILogger _exceptionLogger;

    private Stopwatch _duration;
    private Activity _activity;
    private TagList _commonTags;
    private OpenTelemetryResponseMetadata _responseMetadata;
    private bool _exceptionLogged;

    private OpenTelemetryScope(
        ActivitySource activitySource,
        Histogram<double> durationHistogram,
        Histogram<long> tokenHistogram,
        OpenTelemetryTokenMetrics inferenceTokens,
        string model,
        string operationName,
        string serverAddress,
        int serverPort,
        bool useLatestSemanticConventions = false,
        bool includeErrorDescription = true,
        ILogger exceptionLogger = null)
    {
        _activitySource = activitySource;
        _durationHistogram = durationHistogram;
        _tokenHistogram = tokenHistogram;
        _inferenceTokens = inferenceTokens;
        _requestModel = model;
        _operationName = operationName;
        _serverAddress = serverAddress;
        _serverPort = serverPort;
        _useLatestSemanticConventions = useLatestSemanticConventions;
        _includeErrorDescription = includeErrorDescription;
        _exceptionLogger = exceptionLogger;
    }

    public static OpenTelemetryScope StartChat(
        string model,
        string operationName,
        string serverAddress,
        int serverPort,
        ChatCompletionOptions options,
        string providerAttributeKey,
        bool useLatestSemanticConventions = false,
        ILogger exceptionLogger = null)
    {
        if (!IsEnabled(s_chatSource, s_chatTokens, s_chatInferenceTokens, s_chatDuration, useLatestSemanticConventions, exceptionLogger))
        {
            return null;
        }

        var scope = new OpenTelemetryScope(
            s_chatSource,
            s_chatDuration,
            s_chatTokens,
            s_chatInferenceTokens,
            model,
            operationName,
            serverAddress,
            serverPort,
            useLatestSemanticConventions,
            exceptionLogger: exceptionLogger);
        scope.Start(providerAttributeKey, useLatestSemanticConventions ? OpenAiApiTypeChatCompletionsValue : null);
        scope.RecordChatRequestAttributes(options);
        return scope;
    }

    public static OpenTelemetryScope StartResponses(
        string model,
        string operationName,
        string serverAddress,
        int serverPort,
        CreateResponseOptions options,
        string providerAttributeKey,
        bool useLatestSemanticConventions,
        bool streaming = false,
        ILogger exceptionLogger = null)
    {
        var streamingMetricsEnabled = (streaming && useLatestSemanticConventions)
            && (s_responsesFirstChunk.Enabled || s_responsesOutputChunk.Enabled);
        if ((!IsEnabled(s_responsesSource, s_responsesTokens, s_responsesInferenceTokens, s_responsesDuration, useLatestSemanticConventions, exceptionLogger)) && (!streamingMetricsEnabled))
        {
            return null;
        }

        var scope = new OpenTelemetryScope(
            s_responsesSource,
            s_responsesDuration,
            s_responsesTokens,
            s_responsesInferenceTokens,
            model,
            operationName,
            serverAddress,
            serverPort,
            useLatestSemanticConventions,
            includeErrorDescription: false,
            exceptionLogger: exceptionLogger);
        scope.Start(
            providerAttributeKey,
            useLatestSemanticConventions ? OpenAiApiTypeResponsesValue : null,
            useLatestSemanticConventions ? streaming : null);
        scope.RecordResponsesRequestAttributes(options, useLatestSemanticConventions);
        return scope;
    }

    public void RecordChatCompletion(ChatCompletion completion)
    {
        var usage = _useLatestSemanticConventions ? OpenTelemetryTokenUsage.FromChat(completion.Usage) : default;
        RecordMetrics(completion.Model, completion.ServiceTier?.ToString(), null,
            completion.Usage?.InputTokenCount, completion.Usage?.OutputTokenCount,
            usage, completion.SystemFingerprint);

        if (_activity?.IsAllDataRequested == true)
        {
            if (_useLatestSemanticConventions)
            {
                RecordResponseMetadataAttributes(completion.Id, completion.Model, completion.ServiceTier?.ToString(), completion.SystemFingerprint);
                RecordUsageAttributes(usage);
            }
            else
            {
                RecordResponseAttributes(completion.Id, completion.Model, completion.Usage?.InputTokenCount, completion.Usage?.OutputTokenCount);
            }
            SetChatFinishReasonAttribute(completion.FinishReason);
        }
    }

    public void RecordResponseResult(ResponseResult response)
    {
        var errorType = GetResponseErrorType(response);
        var usage = _useLatestSemanticConventions ? OpenTelemetryTokenUsage.FromResponse(response.Usage) : default;
        if (_useLatestSemanticConventions)
        {
            RecordResponseMetadata(response);
        }
        RecordMetrics(
            _useLatestSemanticConventions ? _responseMetadata.Model : response.Model,
            _useLatestSemanticConventions ? _responseMetadata.ServiceTier : null, errorType,
            response.Usage?.InputTokenCount, response.Usage?.OutputTokenCount,
            usage, _useLatestSemanticConventions ? _responseMetadata.SystemFingerprint : null);

        if (_activity?.IsAllDataRequested == true)
        {
            if (_useLatestSemanticConventions)
            {
                RecordUsageAttributes(usage);
            }
            else
            {
                RecordResponseAttributes(response.Id, response.Model, response.Usage?.InputTokenCount, response.Usage?.OutputTokenCount);
            }
            SetResponseFinishReasonAttribute(response);

            if (_useLatestSemanticConventions)
            {
                if (response.OutputItems.Any(item => item?.Kind == s_compactionItemKind))
                {
                    _activity.SetTag(GenAiConversationCompactedKey, true);
                }
            }

            if (errorType != null)
            {
                RecordError(errorType, null);
            }
        }
    }

    public void RecordException(Exception ex, string responseModel = null, string responseServiceTier = null, bool recordMetrics = true)
    {
        if ((_useLatestSemanticConventions) && (!_exceptionLogged))
        {
            _exceptionLogged = true;
            OpenTelemetryExceptionLogger.Record(_exceptionLogger, ex);
        }
        var errorType = GetErrorType(ex);
        if (_useLatestSemanticConventions)
        {
            responseModel ??= _responseMetadata.Model;
            responseServiceTier ??= _responseMetadata.ServiceTier;
        }
        if (recordMetrics)
        {
            RecordMetrics(responseModel, responseServiceTier, errorType, null, null,
                responseSystemFingerprint: _useLatestSemanticConventions ? _responseMetadata.SystemFingerprint : null);
        }
        if (_activity?.IsAllDataRequested == true)
        {
            if (_useLatestSemanticConventions)
            {
                SetActivityTagIfNotNull(GenAiResponseModelKey, responseModel);
                SetActivityTagIfNotNull(OpenAiResponseServiceTierKey, responseServiceTier);
            }
            RecordError(errorType, _includeErrorDescription ? ex?.Message : null);
        }
    }

    public void Dispose()
    {
        _activity?.Stop();
    }

    private static Histogram<double> CreateDurationHistogram(
        Meter meter,
        string name = GenAiClientOperationDurationMetricName,
        string description = "Measures GenAI operation duration.")
    {
        return meter.CreateHistogram<double>(
            name,
            "s",
            description,
            advice: new InstrumentAdvice<double>
            {
                HistogramBucketBoundaries = [0.01, 0.02, 0.04, 0.08, 0.16, 0.32, 0.64, 1.28, 2.56, 5.12, 10.24, 20.48, 40.96, 81.92],
            });
    }

    private static Histogram<long> CreateTokenHistogram(Meter meter)
    {
        return meter.CreateHistogram<long>(
            GenAiClientTokenUsageMetricName,
            "{token}",
            "Measures the number of input and output tokens used.",
            advice: new InstrumentAdvice<long>
            {
                HistogramBucketBoundaries = [1, 4, 16, 64, 256, 1024, 4096, 16384, 65536, 262144, 1048576, 4194304, 16777216, 67108864],
            });
    }

    private static bool IsEnabled(ActivitySource activitySource, Histogram<long> tokens,
        OpenTelemetryTokenMetrics inferenceTokens, Histogram<double> duration, bool useLatestSemanticConventions,
        ILogger exceptionLogger)
    {
        return activitySource.HasListeners() || duration.Enabled
            || (useLatestSemanticConventions ? inferenceTokens.Enabled : tokens.Enabled)
            || ((useLatestSemanticConventions) && (exceptionLogger?.IsEnabled(LogLevel.Warning) == true));
    }

    private void Start(string providerAttributeKey, string openAiApiType = null, bool? streaming = null)
    {
        _duration = Stopwatch.StartNew();
        _commonTags = new TagList
        {
            { providerAttributeKey, GenAiSystemValue },
            { ServerAddressKey, _serverAddress },
            { ServerPortKey, _serverPort },
            { GenAiOperationNameKey, _operationName },
        };
        if (!string.IsNullOrEmpty(_requestModel))
        {
            _commonTags.Add(GenAiRequestModelKey, _requestModel);
        }

        var activityTags = _commonTags;
        if (openAiApiType != null)
        {
            activityTags.Add(OpenAiApiTypeKey, openAiApiType);
        }
        if (streaming == true)
        {
            activityTags.Add("gen_ai.request.stream", streaming.Value);
        }

        var activityName = string.IsNullOrEmpty(_requestModel)
            ? _operationName
            : string.Concat(_operationName, " ", _requestModel);
        _activity = _activitySource.StartActivity(
            activityName,
            ActivityKind.Client,
            parentContext: default,
            tags: activityTags);
    }

    private void RecordChatRequestAttributes(ChatCompletionOptions options)
    {
        if (_activity?.IsAllDataRequested == true)
        {
            SetActivityTagIfNotNull(GenAiRequestMaxTokensKey, options?.MaxOutputTokenCount);
            SetActivityTagIfNotNull(GenAiRequestTemperatureKey, options?.Temperature);
            SetActivityTagIfNotNull(GenAiRequestTopPKey, options?.TopP);
            if (_useLatestSemanticConventions)
            {
                SetActivityTagIfNotNull(GenAiRequestSeedKey, options?.Seed);
                SetActivityTagIfNotNull(GenAiRequestFrequencyPenaltyKey, (double?)options?.FrequencyPenalty);
                SetActivityTagIfNotNull(GenAiRequestPresencePenaltyKey, (double?)options?.PresencePenalty);
                SetActivityTagIfNotNull(GenAiRequestReasoningLevelKey, options?.ReasoningEffortLevel?.ToString());
                if ((options?.ServiceTier is ChatServiceTier serviceTier) && (serviceTier != ChatServiceTier.Auto))
                {
                    SetActivityTagIfNotNull(OpenAiRequestServiceTierKey, serviceTier.ToString());
                }
                var formatType = options?.ResponseFormat?.GetType();
                var outputType = formatType == s_chatTextFormatType ? "text"
                    : ((formatType == s_chatJsonObjectFormatType) || (formatType == s_chatJsonSchemaFormatType)) ? "json" : null;
                SetActivityTagIfNotNull(GenAiOutputTypeKey, outputType);
            }
        }
    }

    private void RecordResponsesRequestAttributes(CreateResponseOptions options, bool useLatestSemanticConventions)
    {
        if (_activity?.IsAllDataRequested != true)
        {
            return;
        }

        SetActivityTagIfNotNull(GenAiRequestMaxTokensKey, options?.MaxOutputTokenCount);
        SetActivityTagIfNotNull(GenAiRequestTemperatureKey, options?.Temperature);
        SetActivityTagIfNotNull(GenAiRequestTopPKey, options?.TopP);

        if (!useLatestSemanticConventions)
        {
            return;
        }

        SetActivityTagIfNotNull(GenAiRequestPreviousResponseIdKey, options?.PreviousResponseId);
        SetActivityTagIfNotNull(GenAiConversationIdKey, options?.ConversationOptions?.ConversationId);
        SetActivityTagIfNotNull(GenAiRequestReasoningLevelKey, options?.ReasoningOptions?.ReasoningEffortLevel?.ToString());

        if (options?.ServiceTier is ResponseServiceTier serviceTier && serviceTier != ResponseServiceTier.Auto)
        {
            SetActivityTagIfNotNull(OpenAiRequestServiceTierKey, serviceTier.ToString());
        }

        var outputType = options?.TextOptions?.TextFormat?.Kind switch
        {
            ResponseTextFormatKind.Text => "text",
            ResponseTextFormatKind.JsonObject or ResponseTextFormatKind.JsonSchema => "json",
            _ => null,
        };
        SetActivityTagIfNotNull(GenAiOutputTypeKey, outputType);
    }

    private TagList GetMetricTags(string responseModel, string responseServiceTier, string responseSystemFingerprint = null)
    {
        var tags = _commonTags;

        if (responseModel != null)
        {
            tags.Add(GenAiResponseModelKey, responseModel);
        }
        if ((_useLatestSemanticConventions) && (responseServiceTier != null))
        {
            tags.Add(OpenAiResponseServiceTierKey, responseServiceTier);
        }
        if ((_useLatestSemanticConventions) && (responseSystemFingerprint != null))
        {
            tags.Add(OpenAiResponseSystemFingerprintKey, responseSystemFingerprint);
        }

        return tags;
    }

    private void RecordMetrics(string responseModel, string responseServiceTier, string errorType,
        int? inputTokensUsage, int? outputTokensUsage, OpenTelemetryTokenUsage usage = default,
        string responseSystemFingerprint = null)
    {
        var tags = GetMetricTags(responseModel, responseServiceTier, responseSystemFingerprint);

        if (_useLatestSemanticConventions)
        {
            _inferenceTokens.Record(usage, tags);
        }
        else
        {
            if (inputTokensUsage != null)
            {
                var inputUsageTags = tags;
                inputUsageTags.Add(GenAiTokenTypeKey, "input");
                _tokenHistogram.Record(inputTokensUsage.Value, inputUsageTags);
            }

            if (outputTokensUsage != null)
            {
                var outputUsageTags = tags;
                outputUsageTags.Add(GenAiTokenTypeKey, "output");
                _tokenHistogram.Record(outputTokensUsage.Value, outputUsageTags);
            }
        }

        if (errorType != null)
        {
            tags.Add(ErrorTypeKey, errorType);
        }

        _durationHistogram.Record(_duration.Elapsed.TotalSeconds, tags);
    }

    private void RecordResponseAttributes(string responseId, string model, int? inputTokenCount, int? outputTokenCount)
    {
        SetActivityTagIfNotNull(GenAiResponseIdKey, responseId);
        SetActivityTagIfNotNull(GenAiResponseModelKey, model);
        SetActivityTagIfNotNull(GenAiUsageInputTokensKey, inputTokenCount);
        SetActivityTagIfNotNull(GenAiUsageOutputTokensKey, outputTokenCount);
    }

    private void RecordResponseMetadata(ResponseResult response)
    {
        _responseMetadata.Update(response);
        if (_activity?.IsAllDataRequested == true)
        {
            RecordResponseMetadataAttributes(_responseMetadata.Id, _responseMetadata.Model,
                _responseMetadata.ServiceTier, _responseMetadata.SystemFingerprint);
        }
    }

    private void RecordResponseMetadataAttributes(string id, string model, string serviceTier, string fingerprint)
    {
        SetActivityTagIfNotNull(GenAiResponseIdKey, id);
        SetActivityTagIfNotNull(GenAiResponseModelKey, model);
        SetActivityTagIfNotNull(OpenAiResponseServiceTierKey, serviceTier);
        SetActivityTagIfNotNull(OpenAiResponseSystemFingerprintKey, fingerprint);
    }

    private void RecordUsageAttributes(OpenTelemetryTokenUsage usage)
    {
        SetActivityTagIfNotNull(GenAiUsageInputTokensKey, usage.InputTokens);
        SetActivityTagIfNotNull(GenAiUsageOutputTokensKey, usage.OutputTokens);
        SetActivityTagIfNotNull(GenAiUsageCacheReadInputTokensKey, usage.CacheReadInputTokens);
        SetActivityTagIfNotNull(GenAiUsageCacheWriteInputTokensKey, usage.CacheWriteInputTokens);
        SetActivityTagIfNotNull(GenAiUsageReasoningOutputTokensKey, usage.ReasoningOutputTokens);
        SetActivityTagIfNotNull(GenAiUsageAudioInputTokensKey, usage.InputAudioTokens);
        SetActivityTagIfNotNull(GenAiUsageAudioOutputTokensKey, usage.OutputAudioTokens);
    }

    private void SetChatFinishReasonAttribute(ChatFinishReason? finishReason)
    {
        if (finishReason == null)
        {
            return;
        }

        var reasonStr = finishReason switch
        {
            ChatFinishReason.ContentFilter => "content_filter",
            ChatFinishReason.FunctionCall => "function_call",
            ChatFinishReason.Length => "length",
            ChatFinishReason.Stop => "stop",
            ChatFinishReason.ToolCalls => "tool_calls",
            _ => finishReason.ToString(),
        };

        // There could be multiple finish reasons, so semantic conventions use array type for the corresponding attribute.
        // It's likely to change, but for now let's report it as array.
        _activity.SetTag(GenAiResponseFinishReasonKey, new[] { reasonStr });
    }

    private void SetResponseFinishReasonAttribute(ResponseResult response)
    {
        var reason = response.Status switch
        {
            ResponseStatus.Completed => "stop",
            ResponseStatus.Failed or ResponseStatus.Cancelled => "error",
            ResponseStatus.Incomplete when response.IncompleteStatusDetails?.Reason == ResponseIncompleteStatusReason.MaxOutputTokens => "length",
            ResponseStatus.Incomplete when response.IncompleteStatusDetails?.Reason == ResponseIncompleteStatusReason.ContentFilter => "content_filter",
            ResponseStatus.Incomplete => "incomplete",
            _ => null,
        };
        if (reason != null)
        {
            _activity.SetTag(GenAiResponseFinishReasonKey, new[] { reason });
        }
    }

    private static string GetResponseErrorType(ResponseResult response)
    {
        if (response.Status == ResponseStatus.Failed)
        {
            return response.Error?.Code.ToString() switch
            {
                "server_error" => "server_error",
                "rate_limit_exceeded" => "rate_limit_exceeded",
                "invalid_prompt" => "invalid_prompt",
                "vector_store_timeout" => "vector_store_timeout",
                "invalid_image" => "invalid_image",
                "invalid_image_format" => "invalid_image_format",
                "invalid_base64_image" => "invalid_base64_image",
                "invalid_image_url" => "invalid_image_url",
                "image_too_large" => "image_too_large",
                "image_too_small" => "image_too_small",
                "image_parse_error" => "image_parse_error",
                "image_content_policy_violation" => "image_content_policy_violation",
                "invalid_image_mode" => "invalid_image_mode",
                "image_file_too_large" => "image_file_too_large",
                "unsupported_image_media_type" => "unsupported_image_media_type",
                "empty_image_file" => "empty_image_file",
                "failed_to_download_image" => "failed_to_download_image",
                "image_file_not_found" => "image_file_not_found",
                _ => "failed",
            };
        }
        if (response.Status == ResponseStatus.Cancelled)
        {
            return "cancelled";
        }
        return null;
    }

    private void RecordError(string errorType, string description)
    {
        _activity.SetTag(ErrorTypeKey, errorType);
        if (description is null)
        {
            _activity.SetStatus(ActivityStatusCode.Error);
        }
        else
        {
            _activity.SetStatus(ActivityStatusCode.Error, description);
        }
    }

    private string GetErrorType(Exception exception)
    {
        if (exception is ClientResultException requestFailedException)
        {
            // TODO (lmolkova) when we start targeting .NET 8 we should put
            // requestFailedException.InnerException.HttpRequestError into error.type
            return requestFailedException.Status.ToString();
        }

        return exception?.GetType()?.FullName;
    }

    private void SetActivityTagIfNotNull(string name, object value)
    {
        if (value != null)
        {
            _activity.SetTag(name, value);
        }
    }

    private void SetActivityTagIfNotNull(string name, int? value)
    {
        if (value.HasValue)
        {
            _activity.SetTag(name, value.Value);
        }
    }

    private void SetActivityTagIfNotNull(string name, float? value)
    {
        if (value.HasValue)
        {
            _activity.SetTag(name, value.Value);
        }
    }

    private void SetActivityTagIfNotNull(string name, long? value)
    {
        if (value.HasValue)
        {
            _activity.SetTag(name, value.Value);
        }
    }
}
