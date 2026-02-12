using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.Input;
using Microsoft.TypeSpec.Generator.Primitives;
using Microsoft.TypeSpec.Generator.Providers;
using System;
using System.Collections.Generic;

namespace OpenAILibraryPlugin.Visitors;

/// <summary>
/// We prefer to use 32-bit numeric types (`int`, `float`) for numeric properties and parameters unless there is a specific reason to
/// use 64-bit numeric types (`long`, `double`). Because using 64-bit types is therefore the exception, this visitor
/// converts all `long` properties/parameters to `int` and all `double` properties to `float` by default unless they are explicitly excluded.
/// </summary>
public class NumericTypesVisitor : ScmLibraryVisitor
{
    // Add any long properties that should remain long here.
    private static readonly HashSet<string> _excludedLongProperties = new(StringComparer.OrdinalIgnoreCase)
    {
        "OpenAI.Chat.ChatCompletionOptions.Seed",

        "OpenAI.LegacyCompletions.InternalCreateCompletionRequest.Seed",
    };

    // Add any double properties that should remain double here.
    private static readonly HashSet<string> _excludedDoubleProperties = new(StringComparer.OrdinalIgnoreCase) { };

    // Add any long parameters that should remain long here (format: "Namespace.ClassName.MethodName.ParameterName").
    private static readonly HashSet<string> _excludedLongParameters = new(StringComparer.OrdinalIgnoreCase) { };

    // Add any long fields that should remain long here (format: "Namespace.ClassName.FieldName").
    private static readonly HashSet<string> _excludedLongFields = new(StringComparer.OrdinalIgnoreCase) { };

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

    protected override FieldProvider VisitField(FieldProvider field)
    {
        // Convert long fields to int
        if (field.EnclosingType is not null
            && field.Type.Equals(typeof(long))
            && !_excludedLongFields.Contains($"{field.EnclosingType.Type.Namespace}.{field.EnclosingType.Name}.{field.Name}")
            && field.EnclosingType.Type.Namespace != "OpenAI")
        {
            field.Update(type: new CSharpType(typeof(int), field.Type.IsNullable));
        }

        return field;
    }

    protected override MethodProvider? VisitMethod(MethodProvider methodProvider)
    {
        // Convert long parameters to int
        if (methodProvider.EnclosingType is not null
            && methodProvider.EnclosingType.Type.Namespace != "OpenAI")
        {
            foreach (var parameter in methodProvider.Signature.Parameters)
            {
                var parameterKey = $"{methodProvider.EnclosingType.Type.Namespace}.{methodProvider.EnclosingType.Name}.{methodProvider.Signature.Name}.{parameter.Name}";

                if (parameter.Type.Equals(typeof(long))
                    && !_excludedLongParameters.Contains(parameterKey))
                {
                    parameter.Update(type: new CSharpType(typeof(int), parameter.Type.IsNullable));
                }
            }
        }

        return methodProvider;
    }
}