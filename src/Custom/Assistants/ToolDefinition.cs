using System;

namespace OpenAI.Assistants;

[CodeGenModel("AssistantToolDefinition")]
public abstract partial class ToolDefinition
{   
    public static CodeInterpreterToolDefinition CreateCodeInterpreter()
        => new CodeInterpreterToolDefinition();
    public static FileSearchToolDefinition CreateFileSearch()
        => new FileSearchToolDefinition();
    public static FunctionToolDefinition CreateFunction(string name, string description = null, BinaryData parameters = null)
        => new FunctionToolDefinition(name, description, parameters);
}
