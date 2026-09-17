using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

[CodeGenType("ResponseWebSocketSteerCommand")]
public partial class ResponseWebSocketSteerCommand
{
    /// <summary>Creates a steering command containing one user message with an input text part.</summary>
    public ResponseWebSocketSteerCommand(string previousResponseId, string input)
        : this(previousResponseId, input is null ? null : [new ResponseWebSocketSteerMessage(input)])
    {
    }
}
