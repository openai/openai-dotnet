using System.Collections.Generic;
using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

[CodeGenType("ResponseWebSocketCreateCommand")]
public partial class ResponseWebSocketCreateCommand
{
    [CodeGenMember("Input")]
    public IList<ResponseItem> InputItems { get; }
    [CodeGenMember("ToolChoice")]
    public ResponseToolChoice ToolChoice { get; set; }
    [CodeGenMember("Include")]
    public IList<IncludedResponseProperty> IncludedProperties { get; }
}
