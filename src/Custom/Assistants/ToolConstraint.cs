using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

[CodeGenType("AssistantsNamedToolChoice")]
[CodeGenVisibility(nameof(ToolConstraint), CodeGenVisibility.Internal, typeof(InternalAssistantsNamedToolChoiceType))]
public partial class ToolConstraint
{
    private readonly string _plainTextValue;
    private readonly string _objectFunctionName;

    // CUSTOM: Discriminator made nullable to facilitate combination with literal variants
    [CodeGenMember("Type")]
    internal InternalAssistantsNamedToolChoiceType? Kind { get; set; }

    // CUSTOM: Made internal.
    /// <summary> Gets or sets the function. </summary>
    [CodeGenMember("Function")]
    internal InternalAssistantsNamedToolChoiceFunction Function { get; set; }

    public static ToolConstraint None { get; } = new("none");
    public static ToolConstraint Auto { get; } = new("auto");
    public static ToolConstraint Required { get; } = new("required");

    public ToolConstraint(ToolDefinition toolDefinition)
    {
        switch (toolDefinition)
        {
            case CodeInterpreterToolDefinition:
                Kind = "code_interpreter";
                break;
            case FileSearchToolDefinition:
                Kind = "file_search";
                break;
            case FunctionToolDefinition functionTool:
                Kind = "function";
                _objectFunctionName = functionTool.FunctionName;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(toolDefinition));
        }
        SerializedAdditionalRawData = new ChangeTrackingDictionary<string, BinaryData>();
    }

    internal ToolConstraint(string plainTextValue)
        : this(plainTextValue, null, null, null)
    { }

    internal ToolConstraint(string plainTextValue, string objectType, string objectFunctionName, IDictionary<string, BinaryData> serializedAdditionalRawData)
    {
        _plainTextValue = plainTextValue;
        Kind = objectType is null ? null : new(objectType);
        _objectFunctionName = objectFunctionName;
        SerializedAdditionalRawData = serializedAdditionalRawData ?? new ChangeTrackingDictionary<string, BinaryData>();
    }
}
