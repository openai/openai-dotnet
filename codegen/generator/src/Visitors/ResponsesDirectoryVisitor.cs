using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.Providers;
using System;
using System.IO;

namespace OpenAILibraryPlugin.Visitors;

/// <summary>
/// Routes generated Responses source files into the input library's physical Responses tree
/// while keeping them in the unified package and generated context.
/// </summary>
public class ResponsesDirectoryVisitor : ScmLibraryVisitor
{
    private static string ResponsesNamespace =>
        $"{ScmCodeModelGenerator.Instance.InputLibrary.InputNamespace.Name}.Responses";

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
                string responsesModelRoot = Path.Combine("Models", "Responses");
                if (generatedRelativePath.StartsWith(responsesModelRoot, StringComparison.Ordinal))
                {
                    generatedRelativePath = Path.Combine("Models", Path.GetRelativePath(responsesModelRoot, generatedRelativePath));
                }
                type.Update(relativeFilePath: Path.Combine("..", ResponsesNamespace, "src", "Generated", generatedRelativePath));
            }
        }

        return type;
    }
}
