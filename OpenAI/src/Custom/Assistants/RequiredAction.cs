using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

/// <summary>
/// An abstract, base representation for an action that an Assistants API run requires outputs
/// from in order to continue.
/// </summary>
/// <remarks>
/// <see cref="RequiredAction"/> is the abstract base type for all required actions. Its
/// concrete type can be one of:
/// <list type="bullet">
/// <item> <see cref="InternalRequiredFunctionToolCall"/> </item> 
/// </list>
/// </remarks>
[Experimental("OPENAI001")]
public abstract partial class RequiredAction
{
    /// <inheritdoc cref="InternalRequiredFunctionToolCall.InternalName"/>
    public string FunctionName => AsFunction?.InternalName;

    /// <inheritdoc cref="InternalRequiredFunctionToolCall.InternalArguments"/>
    public string FunctionArguments => AsFunction?.InternalArguments;

    /// <inheritdoc cref="InternalRequiredFunctionToolCall.Id"/>
    public string ToolCallId => AsFunction?.Id;

    private InternalRequiredFunctionToolCall AsFunction => this as InternalRequiredFunctionToolCall;
}