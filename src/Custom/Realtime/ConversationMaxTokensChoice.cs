using System;
using System.Collections.Generic;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Realtime;

[Experimental("OPENAI002")]
public partial class ConversationMaxTokensChoice
{
    public int? NumericValue { get; }
    private readonly bool? _isDefaultNullValue;
    private readonly string _stringValue;

    public static ConversationMaxTokensChoice CreateInfiniteMaxTokensChoice()
        => new("inf");
    public static ConversationMaxTokensChoice CreateDefaultMaxTokensChoice()
        => new(isDefaultNullValue: true);
    public static ConversationMaxTokensChoice CreateNumericMaxTokensChoice(int maxTokens)
        => new(numberValue: maxTokens);

    public ConversationMaxTokensChoice(int numberValue)
    {
        NumericValue = numberValue;
    }

    internal ConversationMaxTokensChoice(string stringValue)
    {
        _stringValue = stringValue;
    }

    internal ConversationMaxTokensChoice(bool isDefaultNullValue)
    {
        _isDefaultNullValue = true;
    }

    internal ConversationMaxTokensChoice() { }

    public static implicit operator ConversationMaxTokensChoice(int maxTokens)
        => CreateNumericMaxTokensChoice(maxTokens);
}