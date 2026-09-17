using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

[CodeGenType("ResponseWebSocketSteerMessage")]
public partial class ResponseWebSocketSteerMessage
{
    /// <summary>Creates a user message with an empty content collection.</summary>
    public ResponseWebSocketSteerMessage() : this([])
    {
    }

    /// <summary>Creates a user message containing one input text part.</summary>
    public ResponseWebSocketSteerMessage(string content)
        : this(content is null ? null : [ResponseContentPart.CreateInputTextPart(content)])
    {
    }
}
