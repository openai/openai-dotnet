using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.Providers;
using System;
using System.Collections.Generic;
using System.IO;

namespace OpenAILibraryPlugin.Visitors;

/// <summary>
/// A visitor that organizes model files into subdirectories based on their namespace.
/// </summary>
public class ModelDirectoryVisitor : ScmLibraryVisitor
{
    private static readonly HashSet<string> _excludedNamespaces = new(StringComparer.OrdinalIgnoreCase)
    {
        "OpenAI.Responses",
    };

    protected override TypeProvider VisitType(TypeProvider type)
    {
        if (!_excludedNamespaces.Contains(type.Type.Namespace))
        {
            // Only apply to types in the Models folder
            if (type.RelativeFilePath.Contains("Models"))
            {
                var segments = type.Type.Namespace?.Split('.');

                if (segments is { Length: >= 2 } && segments[0] == "OpenAI")
                {
                    var folderName = segments[1]; // Use second segment, e.g., "Chat" from "OpenAI.Chat"
                    var fileName = Path.GetFileName(type.RelativeFilePath);
                    var newRelativePath = Path.Combine("src", "Generated", "Models", folderName, fileName);

                    type.Update(relativeFilePath: newRelativePath);
                }
            }
        }

        return type;
    }
}
