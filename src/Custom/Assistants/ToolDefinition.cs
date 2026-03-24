namespace OpenAI.Assistants;

using Microsoft.TypeSpec.Generator.Customizations;

using System;

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

    public static global::OpenAI.OpenApiToolDefinition CreateOpenApi(string name, BinaryData specification, string description = null, BinaryData authentication = null)
        => new(new OpenApiToolDefinitionDetails(name, specification)
        {
            Description = description,
            Auth = authentication,
        });
}
