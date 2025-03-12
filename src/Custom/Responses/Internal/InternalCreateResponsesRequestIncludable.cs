using OpenAI.Telemetry;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace OpenAI.Responses;

[CodeGenType("CreateResponsesRequestIncludable")]
internal readonly partial struct InternalCreateResponsesRequestIncludable
{
    public static implicit operator string(InternalCreateResponsesRequestIncludable self) => self.ToString();
}