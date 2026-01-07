using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.Input;
using Microsoft.TypeSpec.Generator.Primitives;
using Microsoft.TypeSpec.Generator.Providers;
using System;
using System.Collections.Generic;

namespace OpenAILibraryPlugin.Visitors;

/// <summary>
/// We prefer to use `int` (Int32) for numeric properties unless there is a specific reason to
/// use `long` (Int64). Because using `long` is therefore the exception, this visitor converts all
/// `long` properties to `int` by default unless they are explicitly excluded.
/// </summary>
public class NumericPropertiesVisitor : ScmLibraryVisitor
{
    private static readonly HashSet<string> _excludedProperties = new(StringComparer.OrdinalIgnoreCase)
    {
        "OpenAI.Chat.ChatCompletionOptions.Seed",

        "OpenAI.LegacyCompletions.InternalCreateCompletionRequest.Seed",
    };

    protected override PropertyProvider? PreVisitProperty(InputProperty property, PropertyProvider? propertyProvider)
    {
        if (propertyProvider is not null
            && propertyProvider.Type.Equals(typeof(long))
            && !_excludedProperties.Contains($"{propertyProvider.EnclosingType.Type.Namespace}.{propertyProvider.EnclosingType.Name}.{propertyProvider.Name}"))
        {
            propertyProvider.Update(type: new CSharpType(typeof(int), propertyProvider.Type.IsNullable));
        }

        return propertyProvider;
    }
}