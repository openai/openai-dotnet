using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.Providers;
using System;
using System.IO;

namespace OpenAILibraryPlugin.Visitors;

/// <summary>
/// Routes generated Responses source files into the physical OpenAI.Responses tree
/// while keeping them in the unified OpenAI package and generated context.
/// </summary>
public class ResponsesDirectoryVisitor : ScmLibraryVisitor
{
    private const string ResponsesNamespace = "OpenAI.Responses";

    protected override TypeProvider VisitType(TypeProvider type)
    {
        if (type.Type.Namespace == ResponsesNamespace
            || type.Type.Namespace.StartsWith($"{ResponsesNamespace}.", StringComparison.Ordinal))
        {
            string generatedRoot = Path.Combine("src", "Generated");
            string relativePath = type.RelativeFilePath;

            if (relativePath.StartsWith(generatedRoot, StringComparison.Ordinal)
                && !relativePath.StartsWith(Path.Combine(generatedRoot, "Internal"), StringComparison.Ordinal))
            {
                string generatedRelativePath = Path.GetRelativePath(generatedRoot, relativePath);
                type.Update(relativeFilePath: Path.Combine("..", "OpenAI.Responses", "src", "Generated", generatedRelativePath));
            }
        }

        return type;
    }
}
