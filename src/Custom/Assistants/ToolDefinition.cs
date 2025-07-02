using System;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

[Experimental("OPENAI001")]
[CodeGenType("AssistantToolDefinition")]
public partial class ToolDefinition
{
    public static CodeInterpreterToolDefinition CreateCodeInterpreter()
        => new CodeInterpreterToolDefinition();
    public static FileSearchToolDefinition CreateFileSearch(int? maxResults = null)
    {
        return new FileSearchToolDefinition()
        {
            MaxResults = maxResults
        };
    }
    public static FunctionToolDefinition CreateFunction(string name, string description = null, BinaryData parameters = null, bool? strictParameterSchemaEnabled = null)
        => new FunctionToolDefinition(name)
        {
            Description = description,
            Parameters = parameters,
            StrictParameterSchemaEnabled = strictParameterSchemaEnabled,
        };
}
