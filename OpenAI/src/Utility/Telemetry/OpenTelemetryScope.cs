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

internal class OpenTelemetryScope : IDisposable
{
    private static readonly ActivitySource s_chatSource = new ActivitySource("OpenAI.ChatClient");
    private static readonly Meter s_chatMeter = new Meter("OpenAI.ChatClient");
    private static readonly ActivitySource s_responsesSource = new ActivitySource("OpenAI.ResponsesClient");
    private static readonly Meter s_responsesMeter = new Meter("OpenAI.ResponsesClient");

    private static readonly Histogram<double> s_chatDuration = CreateDurationHistogram(s_chatMeter);
    private static readonly Histogram<long> s_chatTokens = CreateTokenHistogram(s_chatMeter);
    private static readonly Histogram<double> s_responsesDuration = CreateDurationHistogram(s_responsesMeter);
    private static readonly Histogram<long> s_responsesTokens = CreateTokenHistogram(s_responsesMeter);

    private readonly ActivitySource _activitySource;
    private readonly Histogram<double> _durationHistogram;
    private readonly Histogram<long> _tokenHistogram;
    private readonly string _operationName;
    private readonly string _serverAddress;
    private readonly int _serverPort;
    private readonly string _requestModel;
    private readonly bool _useLatestSemanticConventions;
    private readonly bool _includeErrorDescription;

    private Stopwatch _duration;
    private Activity _activity;
    private TagList _commonTags;

    private OpenTelemetryScope(
        ActivitySource activitySource,
        Histogram<double> durationHistogram,
        Histogram<long> tokenHistogram,
        string model,
        string operationName,
        string serverAddress,
        int serverPort,
        bool useLatestSemanticConventions = false,
        bool includeErrorDescription = true)
    {
        _activitySource = activitySource;
        _durationHistogram = durationHistogram;
        _tokenHistogram = tokenHistogram;
        _requestModel = model;
        _operationName = operationName;
        _serverAddress = serverAddress;
        _serverPort = serverPort;
        _useLatestSemanticConventions = useLatestSemanticConventions;
        _includeErrorDescription = includeErrorDescription;
    }

    public static OpenTelemetryScope StartChat(
        string model,
        string operationName,
        string serverAddress,
        int serverPort,
        ChatCompletionOptions options,
        string providerAttributeKey)
    {
        if (!IsEnabled(s_chatSource, s_chatTokens, s_chatDuration))
        {
            return null;
        }

        var scope = new OpenTelemetryScope(
            s_chatSource,
            s_chatDuration,
            s_chatTokens,
            model,
            operationName,
            serverAddress,
            serverPort);
        scope.Start(providerAttributeKey);
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
        bool useLatestSemanticConventions)
    {
        if (!IsEnabled(s_responsesSource, s_responsesTokens, s_responsesDuration))
        {
            return null;
        }

        var scope = new OpenTelemetryScope(
            s_responsesSource,
            s_responsesDuration,
            s_responsesTokens,
            model,
            operationName,
            serverAddress,
            serverPort,
            useLatestSemanticConventions,
            includeErrorDescription: false);
        scope.Start(
            providerAttributeKey,
            useLatestSemanticConventions ? OpenAiApiTypeResponsesValue : null);
        scope.RecordResponsesRequestAttributes(options, useLatestSemanticConventions);
        return scope;
    }

    public void RecordChatCompletion(ChatCompletion completion)
    {
        RecordMetrics(completion.Model, null, null, completion.Usage?.InputTokenCount, completion.Usage?.OutputTokenCount);

        if (_activity?.IsAllDataRequested == true)
        {
            RecordResponseAttributes(completion.Id, completion.Model, completion.Usage?.InputTokenCount, completion.Usage?.OutputTokenCount);
            SetChatFinishReasonAttribute(completion.FinishReason);
        }
    }

    public void RecordResponseResult(ResponseResult response)
    {
        var errorType = GetResponseErrorType(response);
        var responseServiceTier = _useLatestSemanticConventions ? response.ServiceTier?.ToString() : null;
        RecordMetrics(response.Model, responseServiceTier, errorType, response.Usage?.InputTokenCount, response.Usage?.OutputTokenCount);

        if (_activity?.IsAllDataRequested == true)
        {
            RecordResponseAttributes(response.Id, response.Model, response.Usage?.InputTokenCount, response.Usage?.OutputTokenCount);
            SetResponseFinishReasonAttribute(response);

            if (_useLatestSemanticConventions)
            {
                SetActivityTagIfNotNull(OpenAiResponseServiceTierKey, responseServiceTier);
                SetActivityTagIfNotNull(GenAiUsageCacheReadInputTokensKey, response.Usage?.InputTokenDetails?.CachedTokenCount);
                SetActivityTagIfNotNull(GenAiUsageReasoningOutputTokensKey, response.Usage?.OutputTokenDetails?.ReasoningTokenCount);
                if (response.OutputItems.Any(item => string.Equals(item.Kind.ToString(), "compaction", StringComparison.Ordinal)))
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

    public void RecordException(Exception ex)
    {
        var errorType = GetErrorType(ex);
        RecordMetrics(null, null, errorType, null, null);
        if (_activity?.IsAllDataRequested == true)
        {
            RecordError(errorType, _includeErrorDescription ? ex?.Message : null);
        }
    }

    public void Dispose()
    {
        _activity?.Stop();
    }

    private static Histogram<double> CreateDurationHistogram(Meter meter)
    {
        return meter.CreateHistogram<double>(
            GenAiClientOperationDurationMetricName,
            "s",
            "Measures GenAI operation duration.",
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
            "Measures the number of input and output token used.",
            advice: new InstrumentAdvice<long>
            {
                HistogramBucketBoundaries = [1, 4, 16, 64, 256, 1024, 4096, 16384, 65536, 262144, 1048576, 4194304, 16777216, 67108864],
            });
    }

    private static bool IsEnabled(ActivitySource activitySource, Histogram<long> tokens, Histogram<double> duration)
    {
        return activitySource.HasListeners() || tokens.Enabled || duration.Enabled;
    }

    private void Start(string providerAttributeKey, string openAiApiType = null)
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

        var activityName = string.IsNullOrEmpty(_requestModel)
            ? _operationName
            : string.Concat(_operationName, " ", _requestModel);
        _activity = _activitySource.StartActivity(
            activityName,
            ActivityKind.Client,
            Activity.Current?.Context ?? default,
            activityTags);
    }

    private void RecordChatRequestAttributes(ChatCompletionOptions options)
    {
        if (_activity?.IsAllDataRequested == true)
        {
            SetActivityTagIfNotNull(GenAiRequestMaxTokensKey, options?.MaxOutputTokenCount);
            SetActivityTagIfNotNull(GenAiRequestTemperatureKey, options?.Temperature);
            SetActivityTagIfNotNull(GenAiRequestTopPKey, options?.TopP);
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

    private void RecordMetrics(string responseModel, string responseServiceTier, string errorType, int? inputTokensUsage, int? outputTokensUsage)
    {
        var tags = _commonTags;

        if (responseModel != null)
        {
            tags.Add(GenAiResponseModelKey, responseModel);
        }
        if (responseServiceTier != null)
        {
            tags.Add(OpenAiResponseServiceTierKey, responseServiceTier);
        }

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
            var errorType = response.Error?.Code.ToString();
            return string.IsNullOrEmpty(errorType) ? "failed" : errorType;
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
}
