
using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.Input;
using Microsoft.TypeSpec.Generator.Primitives;
using Microsoft.TypeSpec.Generator.Providers;
using System;
using System.Collections.Generic;

namespace OpenAILibraryPlugin.Visitors;

/// <summary>
/// We prefer to use 32-bit numeric types (`int`, `float`) for numeric properties unless there is a specific reason to
/// use 64-bit numeric types (`long`, `double`). Because using 64-bit types is therefore the exception, this visitor
/// converts all `long` properties to `int` and all `double` properties to `float` by default unless they are explicitly excluded.
/// </summary>
public class NumericPropertiesVisitor : ScmLibraryVisitor
{
    // Add any long properties that should remain long here.
    private static readonly HashSet<string> _excludedLongProperties = new(StringComparer.OrdinalIgnoreCase)
    {
        "OpenAI.Chat.ChatCompletionOptions.Seed",

        "OpenAI.LegacyCompletions.InternalCreateCompletionRequest.Seed",
    };

    // Add any double properties that should remain double here.
    private static readonly HashSet<string> _excludedDoubleProperties = new(StringComparer.OrdinalIgnoreCase) { };

    protected override PropertyProvider? PreVisitProperty(InputProperty property, PropertyProvider? propertyProvider)
    {
        if (propertyProvider is not null
            && propertyProvider.Type.Equals(typeof(long))
            && !_excludedLongProperties.Contains($"{propertyProvider.EnclosingType.Type.Namespace}.{propertyProvider.EnclosingType.Name}.{propertyProvider.Name}"))
        {
            propertyProvider.Update(type: new CSharpType(typeof(int), propertyProvider.Type.IsNullable));
        }

        if (propertyProvider is not null
            && propertyProvider.Type.Equals(typeof(double))
            && !_excludedDoubleProperties.Contains($"{propertyProvider.EnclosingType.Type.Namespace}.{propertyProvider.EnclosingType.Name}.{propertyProvider.Name}"))
        {
            propertyProvider.Update(type: new CSharpType(typeof(float), propertyProvider.Type.IsNullable));
        }

        return propertyProvider;
    }
}