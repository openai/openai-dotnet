using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.Providers;

namespace OpenAILibraryPlugin.Visitors;

/// <summary>
/// A visitor that merges the Responses model-reader-writer context into the main OpenAI context.
/// </summary>
public class ResponseContextVisitor : ScmLibraryVisitor
{
    protected override TypeProvider VisitType(TypeProvider type)
    {
        if (type.Name == "OpenAIResponsesContext" && type.Type.Namespace == "OpenAI.Responses")
        {
            type.Update(name: "OpenAIContext", namespace: "OpenAI");
        }

        return type;
    }
}
