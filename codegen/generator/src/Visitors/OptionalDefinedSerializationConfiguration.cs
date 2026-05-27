using Microsoft.TypeSpec.Generator.Expressions;
using Microsoft.TypeSpec.Generator.Snippets;
using Microsoft.TypeSpec.Generator.Statements;
using System.Collections.Generic;

namespace OpenAILibraryPlugin.Visitors;

internal static class OptionalDefinedSerializationConfiguration
{
    private static readonly WritePropertyNameConditionInfo s_statusCondition = new("Status", "status", isCollection: false);
    private static readonly Dictionary<string, IReadOnlyList<WritePropertyNameConditionInfo>> s_conditionsByTypeName = new()
    {
        ["ChatCompletionOptions"] =
        [
            new("Messages", "messages", isCollection: true),
            new("Model", "model", isCollection: false),
        ],
        ["ResponseItem"] =
        [
            new("Id", "id", isCollection: false),
        ],
        ["ApplyPatchCallItem"] = [s_statusCondition],
        ["CodeInterpreterCallResponseItem"] = [s_statusCondition],
        ["ComputerCallResponseItem"] = [s_statusCondition],
        ["ComputerCallOutputResponseItem"] = [s_statusCondition],
        ["FileSearchCallResponseItem"] = [s_statusCondition],
        ["FunctionCallResponseItem"] = [s_statusCondition],
        ["FunctionCallOutputResponseItem"] = [s_statusCondition],
        ["ImageGenerationCallResponseItem"] = [s_statusCondition],
        ["MessageResponseItem"] = [s_statusCondition],
        ["ReasoningResponseItem"] = [s_statusCondition],
        ["WebSearchCallResponseItem"] = [s_statusCondition],
    };

    internal static SingleLineCommentStatement OptionalDefinedCheckComment { get; }
        = new("Plugin customization: apply Optional.Is*Defined() check based on type name dictionary lookup");

    internal static IReadOnlyList<WritePropertyNameConditionInfo> GetConditions(string typeName)
        => s_conditionsByTypeName.GetValueOrDefault(typeName) ?? [];

    internal static ScopedApi<bool> GetOptionalDefinedCondition(WritePropertyNameConditionInfo replacementInfo)
    {
        string methodName = replacementInfo.IsCollection ? "IsCollectionDefined" : "IsDefined";
        return new MemberExpression(null, "Optional")
            .Invoke(methodName, new MemberExpression(null, replacementInfo.PropertyName))
            .As<bool>();
    }
}

internal sealed class WritePropertyNameConditionInfo(string propertyName, string jsonName, bool isCollection)
{
    internal string PropertyName { get; } = propertyName;
    internal string JsonName { get; } = jsonName;
    internal bool IsCollection { get; } = isCollection;
}
