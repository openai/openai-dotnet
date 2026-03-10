using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[Experimental("OPENAI001")]
public partial class ResponseToolChoice
{
    public ResponseToolChoiceKind Kind
        => _toolChoiceOption?.ToResponseToolChoiceKind()
        ?? _toolChoiceObject?.Kind.ToResponseToolChoiceKind()
        ?? ResponseToolChoiceKind.Unknown;

    public string FunctionName
        => (_toolChoiceObject as InternalToolChoiceObjectFunction)?.Name;

    private readonly InternalToolChoiceObject _toolChoiceObject;
    private readonly InternalToolChoiceOptions? _toolChoiceOption;

    public static ResponseToolChoice CreateFunctionChoice(string functionName)
        => new(new InternalToolChoiceObjectFunction(functionName));

    public static ResponseToolChoice CreateFileSearchChoice()
        => new(new InternalToolChoiceObjectFileSearch());

    public static ResponseToolChoice CreateWebSearchChoice()
        => new(new InternalToolChoiceObjectWebSearch());

    [Experimental("OPENAICUA001")]
    public static ResponseToolChoice CreateComputerChoice()
        => new(new InternalToolChoiceObjectComputer());

    public static ResponseToolChoice CreateAutoChoice()
        => new(InternalToolChoiceOptions.Auto);

    public static ResponseToolChoice CreateNoneChoice()
        => new(InternalToolChoiceOptions.None);

    public static ResponseToolChoice CreateRequiredChoice()
        => new(InternalToolChoiceOptions.Required);

    internal ResponseToolChoice(InternalToolChoiceObject toolChoiceObject)
    {
        _toolChoiceObject = toolChoiceObject;
    }

    internal ResponseToolChoice(InternalToolChoiceOptions toolChoiceOption)
    {
        _toolChoiceOption = toolChoiceOption;
    }

    // CUSTOM: Supply an internal default constructor for serialization and mocking.
    internal ResponseToolChoice()
    { }

}
