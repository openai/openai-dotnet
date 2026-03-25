using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.Expressions;
using Microsoft.TypeSpec.Generator.Providers;
using Microsoft.TypeSpec.Generator.Statements;
using System.Collections.Generic;
using System.Linq;

namespace OpenAILibraryPlugin.Visitors;

/// <summary>
/// This very simple visitor omits types with specific fully qualified names.
/// </summary>
public class OmittedTypesVisitor : ScmLibraryVisitor
{
    private static HashSet<string> FullyQualifiedTypesToOmit = new()
        {
            "OpenAI.Chat.ChatMessageContent",
            "OpenAI.FineTuneChatCompletionRequestAssistantMessage",
            "OpenAI.FineTuneChatCompletionRequestAssistantMessageWeight",
            "OpenAI.FineTuneChatCompletionRequestAssistantMessageRole",
        };

    // Root client types that should be omitted when the compilation target has no custom code for them.
    // This effectively omits them from the Responses project (which has no custom stubs) while keeping
    // them in the OpenAI project (which does).
    private static HashSet<string> OmitWhenNoCustomCode = new()
        {
            "OpenAIClient",
            "OpenAIClientOptions",
        };

    protected override TypeProvider? VisitType(TypeProvider type)
    {
        string fullyQualifiedName = $"{type.Type.Namespace}.{type.Name}";

        // Strip buildable attributes for omitted types from OpenAIContext
        if (type.Name == "OpenAIContext")
        {
            var filtered = new List<AttributeStatement>(type.Attributes.Count);
            foreach (var a in type.Attributes)
            {
                bool drop =
                    a.Type.Name == "ModelReaderWriterBuildableAttribute" &&
                    a.Arguments.Count == 1 &&
                    a.Arguments[0] is TypeOfExpression tof &&
                    FullyQualifiedTypesToOmit.Any(fqn => fqn.EndsWith($".{tof.Type.Name}"));

                if (!drop) filtered.Add(a);
            }

            if (filtered.Count != type.Attributes.Count)
                type.Update(attributes: filtered);
        }

        if (FullyQualifiedTypesToOmit.Contains(fullyQualifiedName))
            return null;

        if (OmitWhenNoCustomCode.Contains(type.Name) && type.CustomCodeView is null)
            return null;

        return base.VisitType(type);
    }
}