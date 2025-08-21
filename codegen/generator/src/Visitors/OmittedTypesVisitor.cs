using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.Expressions;
using Microsoft.TypeSpec.Generator.Providers;
using Microsoft.TypeSpec.Generator.Statements;
using System.Collections.Generic;
using System.Linq;

namespace OpenAILibraryPlugin.Visitors;

/// <summary>
/// This very simple visitor omits types with specific names.
/// </summary>
public class OmittedTypesVisitor : ScmLibraryVisitor
{
    private static IReadOnlyList<string> TypeNamesToOmit =
        [
            "ChatMessageContent",
            "FineTuneChatCompletionRequestAssistantMessage",
            "FineTuneChatCompletionRequestAssistantMessageWeight",
            "FineTuneChatCompletionRequestAssistantMessageRole",
        ];

    protected override TypeProvider? VisitType(TypeProvider type)
    {
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
                    TypeNamesToOmit.Contains(tof.Type.Name);

                if (!drop) filtered.Add(a);
            }

            if (filtered.Count != type.Attributes.Count)
                type.Update(attributes: filtered);
        }

        // Omit the types themselves
        if (TypeNamesToOmit.Contains(type.Name))
            return null;

        return base.VisitType(type);
    }
}