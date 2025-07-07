using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Realtime;

[Experimental("OPENAI002")]
public partial class ConversationToolChoice
{
    public ConversationToolChoiceKind Kind { get; }

    public string FunctionName
        => (_objectToolChoice as InternalRealtimeToolChoiceFunctionObject)?.Function?.Name;

    private readonly InternalRealtimeToolChoiceObject _objectToolChoice;

    public static ConversationToolChoice CreateAutoToolChoice() => new(ConversationToolChoiceKind.Auto, null);
    public static ConversationToolChoice CreateNoneToolChoice() => new(ConversationToolChoiceKind.None, null);
    public static ConversationToolChoice CreateRequiredToolChoice() => new(ConversationToolChoiceKind.Required, null);

    public static ConversationToolChoice CreateFunctionToolChoice(string functionName)
        => new(ConversationToolChoiceKind.Function, new InternalRealtimeToolChoiceFunctionObject(ConversationToolKind.Function, null, new InternalRealtimeToolChoiceFunctionObjectFunction(functionName)));

    internal ConversationToolChoice(ConversationToolChoiceKind choiceKind, InternalRealtimeToolChoiceObject objectToolChoice)
    {
        Kind = choiceKind;
        _objectToolChoice = objectToolChoice;
    }

    internal ConversationToolChoice() : this(ConversationToolChoiceKind.Unknown, null)
    {
    }
}