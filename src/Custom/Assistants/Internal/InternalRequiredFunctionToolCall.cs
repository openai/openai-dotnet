namespace OpenAI.Assistants;

/// <summary>
/// A requested invocation of a defined function tool, needed by an Assistants API run to continue.
/// </summary>
[CodeGenModel("RunToolCallObject")]
internal partial class InternalRequiredFunctionToolCall : InternalRequiredToolCall
{
    // CUSTOM:
    //  - 'Type' is hidden, as the object discriminator does not carry additional value to the caller in the context
    //    of a strongly-typed object model
    //  - 'Function' is hidden and its constituent 'Name' and 'Arguments' members are promoted to direct visibility

    [CodeGenMember("Type")]
    private readonly object _type;
    [CodeGenMember("Function")]
    internal readonly InternalRunToolCallObjectFunction _internalFunction;

    /// <inheritdoc cref="InternalRunToolCallObjectFunction.Name"/>
    public string InternalName => _internalFunction.Name;

    /// <inheritdoc cref="InternalRunToolCallObjectFunction.Arguments"/>
    public string InternalArguments => _internalFunction.Arguments;

}