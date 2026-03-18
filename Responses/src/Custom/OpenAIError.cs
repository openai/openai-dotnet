using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
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
    private protected IDictionary<string, BinaryData> _additionalBinaryDataProperties;

        internal OpenAIError(string code, string message, string @param, string kind)
        {
            Code = code;
            Message = message;
            Param = @param;
            Kind = kind;
        }

        internal OpenAIError(string code, string message, string @param, string kind, IDictionary<string, BinaryData> additionalBinaryDataProperties)
        {
            Code = code;
            Message = message;
            Param = @param;
            Kind = kind;
            _additionalBinaryDataProperties = additionalBinaryDataProperties;
        }

        public string Code { get; }

        public string Message { get; }

        public string Param { get; }

        public string Kind { get; }

        internal IDictionary<string, BinaryData> SerializedAdditionalRawData
        {
            get => _additionalBinaryDataProperties;
            set => _additionalBinaryDataProperties = value;
        }
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
