using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenAI;

internal static partial class ClientPipelineExtensions
{
    // CUSTOM: Supplemented exception body with deserialized OpenAI error details.

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
