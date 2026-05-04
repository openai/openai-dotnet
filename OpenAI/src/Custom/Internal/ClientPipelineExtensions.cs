using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenAI;

internal static partial class ClientPipelineExtensions
{
    // CUSTOM:
    // - Supplemented exception body with deserialized OpenAI error details
    // - Generic overloads use per-area error types for richer error messages

    public static async ValueTask<PipelineResponse> ProcessMessageAsync<TClient>(
        this ClientPipeline pipeline,
        PipelineMessage message,
        RequestOptions options)
    {
        await pipeline.SendAsync(message).ConfigureAwait(false);

        if (message.Response.IsError && (options?.ErrorOptions & ClientErrorBehaviors.NoThrow) != ClientErrorBehaviors.NoThrow)
        {
            throw await TryBufferResponseAndCreateErrorAsync<TClient>(message).ConfigureAwait(false) switch
            {
                string errorMessage when !string.IsNullOrEmpty(errorMessage)
                    => new ClientResultException(errorMessage, message.Response),
                _ => new ClientResultException(message.Response),
            };
        }

        return message.BufferResponse ?
            message.Response :
            message.ExtractResponse();
    }

    public static PipelineResponse ProcessMessage<TClient>(
        this ClientPipeline pipeline,
        PipelineMessage message,
        RequestOptions options)
    {
        pipeline.Send(message);

        if (message.Response.IsError && (options?.ErrorOptions & ClientErrorBehaviors.NoThrow) != ClientErrorBehaviors.NoThrow)
        {
            throw TryBufferResponseAndCreateError<TClient>(message) switch
            {
                string errorMessage when !string.IsNullOrEmpty(errorMessage)
                    => new ClientResultException(errorMessage, message.Response),
                _ => new ClientResultException(message.Response),
            };
        }

        return message.BufferResponse ?
            message.Response :
            message.ExtractResponse();
    }

    public static async ValueTask<PipelineResponse> ProcessMessageAsync(
        this ClientPipeline pipeline,
        PipelineMessage message,
        RequestOptions options)
    {
        await pipeline.SendAsync(message).ConfigureAwait(false);

        if (message.Response.IsError && (options?.ErrorOptions & ClientErrorBehaviors.NoThrow) != ClientErrorBehaviors.NoThrow)
        {
            throw await TryBufferResponseAndCreateErrorAsync(message).ConfigureAwait(false) switch
            {
                string errorMessage when !string.IsNullOrEmpty(errorMessage)
                    => new ClientResultException(errorMessage, message.Response),
                _ => new ClientResultException(message.Response),
            };
        }

        return message.BufferResponse ?
            message.Response :
            message.ExtractResponse();
    }

    public static PipelineResponse ProcessMessage(
        this ClientPipeline pipeline,
        PipelineMessage message,
        RequestOptions options)
    {
        pipeline.Send(message);

        if (message.Response.IsError && (options?.ErrorOptions & ClientErrorBehaviors.NoThrow) != ClientErrorBehaviors.NoThrow)
        {
            throw TryBufferResponseAndCreateError(message) switch
            {
                string errorMessage when !string.IsNullOrEmpty(errorMessage)
                    => new ClientResultException(errorMessage, message.Response),
                _ => new ClientResultException(message.Response),
            };
        }

        return message.BufferResponse ?
            message.Response :
            message.ExtractResponse();
    }

    private static string TryBufferResponseAndCreateError<TClient>(PipelineMessage message)
    {
        message.Response.BufferContent();
        return TryCreateErrorMessageFromResponse<TClient>(message.Response);
    }

    private static async Task<string> TryBufferResponseAndCreateErrorAsync<TClient>(PipelineMessage message)
    {
        await message.Response.BufferContentAsync().ConfigureAwait(false);
        return TryCreateErrorMessageFromResponse<TClient>(message.Response);
    }

    private static string TryBufferResponseAndCreateError(PipelineMessage message)
    {
        message.Response.BufferContent();
        return TryCreateErrorMessageFromResponse(message.Response);
    }

    private static async Task<string> TryBufferResponseAndCreateErrorAsync(PipelineMessage message)
    {
        await message.Response.BufferContentAsync().ConfigureAwait(false);
        return TryCreateErrorMessageFromResponse(message.Response);
    }

    private static string TryCreateErrorMessageFromResponse<TClient>(PipelineResponse response)
    {
        try
        {
            using JsonDocument errorDocument = JsonDocument.Parse(response.Content);
            JsonElement rootElement = errorDocument.RootElement;

            string code = null, message = null, param = null, kind = null;

            if (typeof(TClient) == typeof(Batch.BatchClient) || typeof(TClient) == typeof(Batch.CreateBatchOperation))
            {
                var errorResponse = Batch.InternalBatchErrorResponse.DeserializeInternalBatchErrorResponse(rootElement, null);
                code = errorResponse.Error.Code;
                message = errorResponse.Error.Message;
                param = errorResponse.Error.Param;
                kind = errorResponse.Error.Kind;
            }
            else if (typeof(TClient) == typeof(Chat.ChatClient))
            {
                var errorResponse = Chat.InternalChatErrorResponse.DeserializeInternalChatErrorResponse(rootElement, null);
                code = errorResponse.Error.Code;
                message = errorResponse.Error.Message;
                param = errorResponse.Error.Param;
                kind = errorResponse.Error.Kind;
            }
            else if (typeof(TClient) == typeof(Containers.ContainerClient))
            {
                var errorResponse = Containers.InternalContainersErrorResponse.DeserializeInternalContainersErrorResponse(rootElement, null);
                code = errorResponse.Error.Code;
                message = errorResponse.Error.Message;
                param = errorResponse.Error.Param;
                kind = errorResponse.Error.Kind;
            }
            else if (typeof(TClient) == typeof(FineTuning.FineTuningClient) || typeof(TClient) == typeof(FineTuning.FineTuningJob))
            {
                var errorResponse = FineTuning.InternalFineTuningErrorResponse.DeserializeInternalFineTuningErrorResponse(rootElement, null);
                code = errorResponse.Error.Code;
                message = errorResponse.Error.Message;
                param = errorResponse.Error.InvalidParameter;
            }
            else if (typeof(TClient) == typeof(Files.OpenAIFileClient) || typeof(TClient) == typeof(Files.InternalUploadsClient))
            {
                var errorResponse = Files.InternalUploadsErrorResponse.DeserializeInternalUploadsErrorResponse(rootElement, null);
                code = errorResponse.Error.Code;
                message = errorResponse.Error.Message;
                param = errorResponse.Error.Param;
                kind = errorResponse.Error.Kind;
            }
            else if (typeof(TClient) == typeof(LegacyCompletions.LegacyCompletionClient))
            {
                var errorResponse = LegacyCompletions.InternalCompletionsErrorResponse.DeserializeInternalCompletionsErrorResponse(rootElement, null);
                code = errorResponse.Error.Code;
                message = errorResponse.Error.Message;
                param = errorResponse.Error.Param;
                kind = errorResponse.Error.Kind;
            }
            else if (typeof(TClient) == typeof(Assistants.AssistantClient))
            {
                var errorResponse = Assistants.InternalAssistantsErrorResponse.DeserializeInternalAssistantsErrorResponse(rootElement, null);
                code = errorResponse.Error.Code;
                message = errorResponse.Error.Message;
                param = errorResponse.Error.Param;
                kind = errorResponse.Error.Kind;
            }
            else if (typeof(TClient) == typeof(Assistants.InternalAssistantRunClient))
            {
                var errorResponse = Assistants.InternalRunsErrorResponse.DeserializeInternalRunsErrorResponse(rootElement, null);
                code = errorResponse.Error.Code;
                message = errorResponse.Error.Message;
                param = errorResponse.Error.Param;
                kind = errorResponse.Error.Kind;
            }
            else if (typeof(TClient) == typeof(Assistants.InternalAssistantMessageClient))
            {
                var errorResponse = Assistants.InternalMessagesErrorResponse.DeserializeInternalMessagesErrorResponse(rootElement, null);
                code = errorResponse.Error.Code;
                message = errorResponse.Error.Message;
                param = errorResponse.Error.Param;
                kind = errorResponse.Error.Kind;
            }
            else if (typeof(TClient) == typeof(Assistants.InternalAssistantThreadClient))
            {
                var errorResponse = Assistants.InternalThreadsErrorResponse.DeserializeInternalThreadsErrorResponse(rootElement, null);
                code = errorResponse.Error.Code;
                message = errorResponse.Error.Message;
                param = errorResponse.Error.Param;
                kind = errorResponse.Error.Kind;
            }
            else if (typeof(TClient) == typeof(VectorStores.VectorStoreClient))
            {
                var errorResponse = VectorStores.InternalVectorStoresErrorResponse.DeserializeInternalVectorStoresErrorResponse(rootElement, null);
                code = errorResponse.Error.Code;
                message = errorResponse.Error.Message;
                param = errorResponse.Error.Param;
                kind = errorResponse.Error.Kind;
            }
            else if (typeof(TClient) == typeof(Evals.EvaluationClient))
            {
                if (rootElement.TryGetProperty("error", out JsonElement errorElement))
                {
                    var error = Evals.InternalEvalApiError.DeserializeInternalEvalApiError(errorElement, null);
                    code = error.Code;
                    message = error.Message;
                }
            }
            else
            {
                return TryCreateErrorMessageFromResponse(response);
            }

            if (message != null)
            {
                return FormatErrorMessage(response.Status, code, message, param, kind);
            }

            return null;
        }
        catch (InvalidOperationException)
        {
            return null;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static string TryCreateErrorMessageFromResponse(PipelineResponse response)
    {
        try
        {
            using JsonDocument errorDocument = JsonDocument.Parse(response.Content);

            if (errorDocument.RootElement.TryGetProperty("error", out JsonElement errorElement))
            {
                string code = errorElement.TryGetProperty("code", out JsonElement c) && c.ValueKind == JsonValueKind.String ? c.GetString() : null;
                string message = errorElement.TryGetProperty("message", out JsonElement m) ? m.GetString() : null;
                string param = errorElement.TryGetProperty("param", out JsonElement p) && p.ValueKind == JsonValueKind.String ? p.GetString() : null;
                string kind = errorElement.TryGetProperty("type", out JsonElement t) ? t.GetString() : null;

                if (message != null)
                {
                    return FormatErrorMessage(response.Status, code, message, param, kind);
                }
            }

            return null;
        }
        catch (InvalidOperationException)
        {
            return null;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static string FormatErrorMessage(int httpStatus, string code, string message, string param, string kind)
    {
        StringBuilder messageBuilder = new();
        messageBuilder.Append("HTTP ").Append(httpStatus).Append(" (").Append(kind).Append(": ").Append(code).AppendLine(")");

        if (!string.IsNullOrEmpty(param))
        {
            messageBuilder.Append("Parameter: ").AppendLine(param);
        }

        messageBuilder.AppendLine();
        messageBuilder.Append(message);
        return messageBuilder.ToString();
    }
}
