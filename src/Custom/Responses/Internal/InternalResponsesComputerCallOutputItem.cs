//using OpenAI.Telemetry;
//using System.ClientModel;
//using System.ClientModel.Primitives;
//using System.Collections.Generic;
//using System.Threading;

//namespace OpenAI.Responses;

//[CodeGenType("ResponsesComputerCallOutputItem")]
//internal partial class InternalResponsesComputerCallOutputItem
//{
//    internal InternalResponsesComputerCallOutputItem(
//        string callId,
//        InternalResponsesComputerCallOutputItemOutput output)
//            : base(InternalResponsesItemType.ComputerCallOutput)
//    {
//        Argument.AssertNotNull(callId, nameof(callId));
//        Argument.AssertNotNull(output, nameof(output));

//        CallId = callId;
//        Output = output;
//    }

//    [CodeGenMember("Output")]
//    internal InternalResponsesComputerCallOutputItemOutput Output { get; set; }
//}
