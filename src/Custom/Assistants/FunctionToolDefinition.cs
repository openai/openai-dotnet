using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

[Experimental("OPENAI001")]
[CodeGenModel("AssistantToolsFunction")]
[CodeGenSuppress(nameof(FunctionToolDefinition), typeof(InternalFunctionDefinition))]
public partial class FunctionToolDefinition : ToolDefinition
{
    // CUSTOM: the visibility of the underlying function object is hidden to simplify the structure of the tool.

    [CodeGenMember("Function")]
    private readonly InternalFunctionDefinition _internalFunction;

    /// <inheritdoc cref="InternalFunctionDefinition.Name"/>
    public required string FunctionName
    {
        get => _internalFunction.Name;
        set => _internalFunction.Name = value;
    }

    /// <inheritdoc cref="InternalFunctionDefinition.Description"/>
    public string Description
    {
        get => _internalFunction.Description;
        set => _internalFunction.Description = value;
    }

    /// <inheritdoc cref="InternalFunctionDefinition.Parameters"/>
    public BinaryData Parameters
    {
        get => _internalFunction.Parameters;
        set => _internalFunction.Parameters = value;
    }

    public bool? StrictParameterSchemaEnabled
    {
        get => _internalFunction.Strict;
        set => _internalFunction.Strict = value;
    }

    /// <summary>
    /// Creates a new instance of <see cref="FunctionToolDefinition"/>. 
    /// </summary>
    [SetsRequiredMembers]
    public FunctionToolDefinition(string name)
        : base("function")
    {
        Argument.AssertNotNullOrEmpty(name, nameof(name));
        _internalFunction = new(null, name, null, null, null);
    }

    /// <summary>
    /// Creates a new instance of <see cref="FunctionToolDefinition"/>. 
    /// </summary>
    public FunctionToolDefinition()
        : base("function")
    {
        _internalFunction = new InternalFunctionDefinition();
    }

    [SetsRequiredMembers]
    internal FunctionToolDefinition(string type, IDictionary<string, BinaryData> serializedAdditionalRawData, InternalFunctionDefinition function)
        : base(type, serializedAdditionalRawData)
    {
        _internalFunction = function;
    }
}
