using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Realtime;

[CodeGenType("RealtimeResponseStatusDetails")]
public partial class ConversationStatusDetails
{
    [CodeGenMember("Type")]
    public ConversationStatus StatusKind { get; }

    [CodeGenMember("Reason")]
    public ConversationIncompleteReason? IncompleteReason { get; }

    public string ErrorKind => Error?.Kind ?? string.Empty;

    public string ErrorCode => Error?.Code ?? string.Empty;

    [CodeGenMember("Error")]
    internal InternalRealtimeResponseStatusDetailsError Error { get; set; }
}