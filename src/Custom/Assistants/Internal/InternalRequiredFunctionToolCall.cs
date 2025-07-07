namespace OpenAI.Assistants;

/// <summary>
/// A requested invocation of a defined function tool, needed by an Assistants API run to continue.
/// </summary>
[CodeGenType("RunToolCallObject")]
internal partial class InternalRequiredFunctionToolCall : InternalRequiredToolCall
{
    // CUSTOM:
    //  - 'Function' is hidden and its constituent 'Name' and 'Arguments' members are promoted to direct visibility

    [CodeGenMember("Function")]
    internal readonly InternalRunToolCallObjectFunction _internalFunction;

    /// <inheritdoc cref="InternalRunToolCallObjectFunction.Name"/>
    public string InternalName => _internalFunction.Name;

    /// <inheritdoc cref="InternalRunToolCallObjectFunction.Arguments"/>
    public string InternalArguments => _internalFunction.Arguments;

}