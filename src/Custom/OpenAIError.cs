using System;
using System.ClientModel.Primitives;
using System.Text;
using System.Text.Json;

namespace OpenAI.Internal;

// Custom:
// - Renamed
// - 'FromResponse' added for convenience with parent type
// - 'ToExceptionMessage' added for encapsulated message formatting

[CodeGenType("Error")]
internal partial class OpenAIError
{
    internal static OpenAIError TryCreateFromResponse(PipelineResponse response)
    {
        try
        {
            using JsonDocument errorDocument = JsonDocument.Parse(response.Content);
            OpenAIErrorResponse errorResponse
                = OpenAIErrorResponse.DeserializeOpenAIErrorResponse(errorDocument.RootElement, null);
            return errorResponse.Error;
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

    public string ToExceptionMessage(int httpStatus)
    {
        StringBuilder messageBuilder = new();
        messageBuilder.Append("HTTP ").Append(httpStatus).Append(" (").Append(Kind).Append(": ").Append(Code).AppendLine(")");
        if (!string.IsNullOrEmpty(Param))
        {
            messageBuilder.Append("Parameter: ").AppendLine(Param);
        }
        messageBuilder.AppendLine();
        messageBuilder.Append(Message);
        return messageBuilder.ToString();
    }
}

// Custom:
// - Renamed

[CodeGenType("ErrorResponse")]
internal partial class OpenAIErrorResponse { }
