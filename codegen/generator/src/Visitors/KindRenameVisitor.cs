using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.Input;
using Microsoft.TypeSpec.Generator.Providers;

namespace OpenAILibraryPlugin.Visitors;

/// <summary>
/// This small, magnanimous visitor simply renames generated properties named "Type" to "Kind."
/// </summary>
public class KindRenameVisitor : ScmLibraryVisitor
{
    protected override PropertyProvider? PreVisitProperty(InputProperty property, PropertyProvider? propertyProvider)
    {
        // Rename 'Type' to 'Kind'
        if (propertyProvider?.Name == "Type")
        {
            propertyProvider.GetType().GetProperty("Name")?.SetValue(propertyProvider, "Kind");
        }

        return propertyProvider;
    }
}