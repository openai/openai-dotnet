using OpenAI.Chat;
using System;
using System.ClientModel;
using System.Diagnostics;
using System.Diagnostics.Metrics;

using static OpenAI.Telemetry.OpenTelemetryConstants;

namespace OpenAI.Telemetry;

internal class OpenTelemetryScope : IDisposable
{
    private static readonly ActivitySource s_chatSource = new ActivitySource("OpenAI.ChatClient");
    private static readonly Meter s_chatMeter = new Meter("OpenAI.ChatClient");

    // TODO: add explicit histogram buckets once System.Diagnostics.DiagnosticSource 9.0 is used
    private static readonly Histogram<double> s_duration = s_chatMeter.CreateHistogram<double>(GenAiClientOperationDurationMetricName, "s", "Measures GenAI operation duration.");
    private static readonly Histogram<long> s_tokens = s_chatMeter.CreateHistogram<long>(GenAiClientTokenUsageMetricName, "{token}", "Measures the number of input and output token used.");

    private readonly string _operationName;
    private readonly string _serverAddress;
    private readonly int _serverPort;
    private readonly string _requestModel;

    private Stopwatch _duration;
    private Activity _activity;
    private TagList _commonTags;

    private OpenTelemetryScope(
        string model, string operationName,
        string serverAddress, int serverPort)
    {
        _requestModel = model;
        _operationName = operationName;
        _serverAddress = serverAddress;
        _serverPort = serverPort;
    }

    private static bool IsChatEnabled => s_chatSource.HasListeners() || s_tokens.Enabled || s_duration.Enabled;

    public static OpenTelemetryScope StartChat(string model, string operationName,
        string serverAddress, int serverPort, ChatCompletionOptions options)
    {
        if (IsChatEnabled)
        {
            var scope = new OpenTelemetryScope(model, operationName, serverAddress, serverPort);
            scope.StartChat(options);
            return scope;
        }

        return null;
    }

    private void StartChat(ChatCompletionOptions options)
    {
        _duration = Stopwatch.StartNew();
        _commonTags = new TagList
        {
            { GenAiSystemKey, GenAiSystemValue },
            { GenAiRequestModelKey, _requestModel },
            { ServerAddressKey, _serverAddress },
            { ServerPortKey, _serverPort },
            { GenAiOperationNameKey, _operationName },
        };

        _activity = s_chatSource.StartActivity(string.Concat(_operationName, " ", _requestModel), ActivityKind.Client);
        if (_activity?.IsAllDataRequested == true)
        {
            RecordCommonAttributes();
            SetActivityTagIfNotNull(GenAiRequestMaxTokensKey, options?.MaxTokens);
            SetActivityTagIfNotNull(GenAiRequestTemperatureKey, options?.Temperature);
            SetActivityTagIfNotNull(GenAiRequestTopPKey, options?.TopP);
        }

        return;
    }

    public void RecordChatCompletion(ChatCompletion completion)
    {
        RecordMetrics(completion.Model, null, completion.Usage?.InputTokens, completion.Usage?.OutputTokens);

        if (_activity?.IsAllDataRequested == true)
        {
            RecordResponseAttributes(completion.Id, completion.Model, completion.FinishReason, completion.Usage);
        }
    }

    public void RecordException(Exception ex)
    {
        var errorType = GetErrorType(ex);
        RecordMetrics(null, errorType, null, null);
        if (_activity?.IsAllDataRequested == true)
        {
            _activity?.SetTag(OpenTelemetryConstants.ErrorTypeKey, errorType);
            _activity?.SetStatus(ActivityStatusCode.Error, ex?.Message ?? errorType);
        }
    }

    public void Dispose()
    {
        _activity?.Stop();
    }

    private void RecordCommonAttributes()
    {
        _activity.SetTag(GenAiSystemKey, GenAiSystemValue);
        _activity.SetTag(GenAiRequestModelKey, _requestModel);
        _activity.SetTag(ServerAddressKey, _serverAddress);
        _activity.SetTag(ServerPortKey, _serverPort);
        _activity.SetTag(GenAiOperationNameKey, _operationName);
    }

    private void RecordMetrics(string responseModel, string errorType, int? inputTokensUsage, int? outputTokensUsage)
    {
        // tags is a struct, let's copy and modify them
        var tags = _commonTags;

        if (responseModel != null)
        {
            tags.Add(GenAiResponseModelKey, responseModel);
        }

        if (inputTokensUsage != null)
        {
            var inputUsageTags = tags;
            inputUsageTags.Add(GenAiTokenTypeKey, "input");
            s_tokens.Record(inputTokensUsage.Value, inputUsageTags);
        }

        if (outputTokensUsage != null)
        {
            var outputUsageTags = tags;
            outputUsageTags.Add(GenAiTokenTypeKey, "output");
            s_tokens.Record(outputTokensUsage.Value, outputUsageTags);
        }

        if (errorType != null)
        {
            tags.Add(ErrorTypeKey, errorType);
        }

        s_duration.Record(_duration.Elapsed.TotalSeconds, tags);
    }

    private void RecordResponseAttributes(string responseId, string model, ChatFinishReason? finishReason, ChatTokenUsage usage)
    {
        SetActivityTagIfNotNull(GenAiResponseIdKey, responseId);
        SetActivityTagIfNotNull(GenAiResponseModelKey, model);
        SetActivityTagIfNotNull(GenAiUsageInputTokensKey, usage?.InputTokens);
        SetActivityTagIfNotNull(GenAiUsageOutputTokensKey, usage?.OutputTokens);
        SetFinishReasonAttribute(finishReason);
    }

    private void SetFinishReasonAttribute(ChatFinishReason? finishReason)
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

        // There could be multiple finish reasons, so semantic conventions use array type for the corrresponding attribute.
        // It's likely to change, but for now let's report it as array.
        _activity.SetTag(GenAiResponseFinishReasonKey, new[] { reasonStr });
    }

    private string GetChatMessageRole(ChatMessageRole? role) =>
        role switch
        {
            ChatMessageRole.Assistant => "assistant",
            ChatMessageRole.Function => "function",
            ChatMessageRole.System => "system",
            ChatMessageRole.Tool => "tool",
            ChatMessageRole.User => "user",
            _ => role?.ToString(),
        };

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
