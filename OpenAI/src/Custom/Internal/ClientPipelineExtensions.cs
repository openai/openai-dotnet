using System.ClientModel;
using System.ClientModel.Primitives;
using System.Threading.Tasks;

namespace OpenAI;

internal static partial class ClientPipelineExtensions
{
    // CUSTOM:
    // - Supplemented exception body with deserialized OpenAI error details

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
        => Internal.OpenAIError.TryCreateFromResponse(response)?.ToExceptionMessage(response.Status);
}
