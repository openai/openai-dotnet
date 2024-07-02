using OpenAI.Chat;
using System;
using System.ClientModel;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace OpenAI.Custom.Common.Instrumentation;

internal class InstrumentationScope : IDisposable
{
    private static readonly ActivitySource s_chatSource = new ActivitySource("OpenAI.ChatClient");
    private static readonly Meter s_chatMeter = new Meter("OpenAI.ChatClient");
    private static readonly Histogram<double> s_duration = s_chatMeter.CreateHistogram<double>(Constants.GenAiClientOperationDurationMetricName, "s", "Measures GenAI operation duration.");
    private static readonly Histogram<long> s_tokens = s_chatMeter.CreateHistogram<long>(Constants.GenAiClientTokenUsageMetricName, "{token}", "Measures the number of input and output token used.");

    private readonly string _operationName;
    private readonly string _serverAddress;
    private readonly int _serverPort;
    private readonly string _requestModel;

    private Stopwatch _duration;
    private Activity _activity;
    private TagList _commonTags;

    private InstrumentationScope(
        string model, string operationName,
        string serverAddress, int serverPort)
    {
        _requestModel = model;
        _operationName = operationName;
        _serverAddress = serverAddress;
        _serverPort = serverPort;
    }

    public static InstrumentationScope StartChat(string model, string operationName,
        string serverAddress, int serverPort, ChatCompletionOptions options)
    {
        if (s_chatSource.HasListeners() || s_tokens.Enabled || s_duration.Enabled)
        {
            var scope = new InstrumentationScope(model, operationName, serverAddress, serverPort);
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
            { Constants.GenAiSystemKey, Constants.GenAiSystemValue },
            { Constants.GenAiRequestModelKey, _requestModel },
            { Constants.ServerAddressKey, _serverAddress },
            { Constants.ServerPortKey, _serverPort },
            { Constants.GenAiOperationNameKey, _operationName },
        };

        _activity = s_chatSource.StartActivity(string.Concat(_operationName, " ", _requestModel), ActivityKind.Client);
        if (_activity?.IsAllDataRequested == true)
        {
            RecordCommonAttributes();
            SetActivityTagIfNotNull(Constants.GenAiRequestMaxTokensKey, options?.MaxTokens);
            SetActivityTagIfNotNull(Constants.GenAiRequestTemperatureKey, options?.Temperature);
            SetActivityTagIfNotNull(Constants.GenAiRequestTopPKey, options?.TopP);
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
        SetActivityError(ex, errorType);
    }

    public void Dispose()
    {
        _activity?.Stop();
    }

    private void RecordCommonAttributes()
    {
        _activity.SetTag(Constants.GenAiSystemKey, Constants.GenAiSystemValue);
        _activity.SetTag(Constants.GenAiRequestModelKey, _requestModel);
        _activity.SetTag(Constants.ServerAddressKey, _serverAddress);
        _activity.SetTag(Constants.ServerPortKey, _serverPort);
        _activity.SetTag(Constants.GenAiOperationNameKey, _operationName);
    }

    private void RecordMetrics(string responseModel, string errorType, int? inputTokensUsage, int? outputTokensUsage)
    {
        TagList tags = ResponseTagsWithError(responseModel, errorType);
        s_duration.Record(_duration.Elapsed.TotalSeconds, tags);

        if (inputTokensUsage != null)
        {
            // tags is a struct, let's copy them
            TagList inputUsageTags = tags;
            inputUsageTags.Add(Constants.GenAiTokenTypeKey, "input");
            s_tokens.Record(inputTokensUsage.Value, inputUsageTags);
        }

        if (outputTokensUsage != null)
        { 
            TagList outputUsageTags = tags;
            outputUsageTags.Add(Constants.GenAiTokenTypeKey, "output");

            s_tokens.Record(outputTokensUsage.Value, outputUsageTags);
        }
    }

    private TagList ResponseTagsWithError(string responseModel, string errorType)
    {
        // tags is a struct, let's copy them
        var tags = _commonTags;

        if (responseModel != null)
        {
            tags.Add(Constants.GenAiResponseModelKey, responseModel);
        }

        if (errorType != null)
        {
            tags.Add(Constants.ErrorTypeKey, errorType);
        }

        return tags;
    }

    private void RecordResponseAttributes(string responseId, string model, ChatFinishReason? finishReason, ChatTokenUsage usage)
    {
        SetActivityTagIfNotNull(Constants.GenAiResponseIdKey, responseId);
        SetActivityTagIfNotNull(Constants.GenAiResponseModelKey, model);
        SetActivityTagIfNotNull(Constants.GenAiResponseFinishReasonKey, GetFinishReason(finishReason));
        SetActivityTagIfNotNull(Constants.GenAiUsageInputTokensKey, usage?.InputTokens);
        SetActivityTagIfNotNull(Constants.GenAiUsageOutputTokensKey, usage?.OutputTokens);
    }

    private string GetFinishReason(ChatFinishReason? reason) =>
        reason switch
        {
            ChatFinishReason.ContentFilter => "content_filter",
            ChatFinishReason.FunctionCall => "function_call",
            ChatFinishReason.Length => "length",
            ChatFinishReason.Stop => "stop",
            ChatFinishReason.ToolCalls => "tool_calls",
            _ => reason?.ToString(),
        };

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
            // TODO (limolkova) when we start targeting .NET 8 we should put
            // requestFailedException.InnerException.HttpRequestError into error.type
            return requestFailedException.Status.ToString();
        }

        return exception?.GetType()?.FullName;
    }

    private void SetActivityError(Exception exception, string errorType)
    {
        if (exception != null || errorType != null)
        {
            _activity?.SetTag(Constants.ErrorTypeKey, errorType);
            _activity?.SetStatus(ActivityStatusCode.Error, exception?.Message ?? errorType);
        }
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
