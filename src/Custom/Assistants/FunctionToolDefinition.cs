using System;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

[CodeGenType("AssistantToolsFunction")]
[CodeGenSuppress(nameof(FunctionToolDefinition), typeof(InternalFunctionDefinition))]
public partial class FunctionToolDefinition
{
    // CUSTOM: the visibility of the underlying function object is hidden to simplify the structure of the tool.

    /// <inheritdoc cref="InternalFunctionDefinition.Name"/>
    public string FunctionName
    {
        get => Function.Name;
        set => Function.Name = value;
    }

    /// <inheritdoc cref="InternalFunctionDefinition.Description"/>
    public string Description
    {
        get => Function.Description;
        set => Function.Description = value;
    }

    /// <inheritdoc cref="InternalFunctionDefinition.Parameters"/>
    public BinaryData Parameters
    {
        get => Function.Parameters;
        set => Function.Parameters = value;
    }

    public bool? StrictParameterSchemaEnabled
    {
        get => Function.Strict;
        set => Function.Strict = value;
    }

    public FunctionToolDefinition(string name)
        : this(kind: InternalAssistantToolDefinitionType.Function, null, new(name: name))
    {
        Argument.AssertNotNullOrEmpty(name, nameof(name));
    }

    // CUSTOM: Ensure default constructor initializes discriminator value and inner type.
    public FunctionToolDefinition() : this(kind: InternalAssistantToolDefinitionType.Function, additionalBinaryDataProperties: null, function: new())
    { }
}
