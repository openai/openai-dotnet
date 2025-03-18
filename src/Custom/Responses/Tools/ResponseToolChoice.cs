
using OpenAI.Assistants;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

public partial class ResponseToolChoice
{
    public ResponseToolChoiceKind Kind
        => _toolChoiceObject?.Kind
        ?? _toolChoiceOption?.Kind
        ?? ResponseToolChoiceKind.Unknown;

    public string FunctionName
        => (_toolChoiceObject as InternalResponsesToolChoiceObjectFunction)?.Name;

    private readonly InternalResponsesToolChoiceObject _toolChoiceObject;
    private readonly InternalResponsesToolChoiceOption? _toolChoiceOption;

    public static ResponseToolChoice CreateFunctionChoice(string functionName)
        => new(new InternalResponsesToolChoiceObjectFunction(functionName));

    public static ResponseToolChoice CreateFileSearchChoice()
        => new(new InternalResponsesToolChoiceObjectFileSearch());

    public static ResponseToolChoice CreateWebSearchChoice()
        => new(new InternalResponsesToolChoiceObjectWebSearch());

    [Experimental("OPENAICUA001")]
    public static ResponseToolChoice CreateComputerChoice()
        => new(new InternalResponsesToolChoiceObjectComputer());

    public static ResponseToolChoice CreateAutoChoice()
        => new(InternalResponsesToolChoiceOption.Auto);

    public static ResponseToolChoice CreateNoneChoice()
        => new(InternalResponsesToolChoiceOption.None);

    public static ResponseToolChoice CreateRequiredChoice()
        => new(InternalResponsesToolChoiceOption.Required);

    internal ResponseToolChoice(InternalResponsesToolChoiceObject toolChoiceObject)
    {
        _toolChoiceObject = toolChoiceObject;
    }

    internal ResponseToolChoice(InternalResponsesToolChoiceOption toolChoiceOption)
    {
        _toolChoiceOption = toolChoiceOption;
    }

    // CUSTOM: Supply an internal default constructor for serialization and mocking.
    internal ResponseToolChoice()
    { }

}
