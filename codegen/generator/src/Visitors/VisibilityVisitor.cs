using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.Expressions;
using Microsoft.TypeSpec.Generator.Primitives;
using Microsoft.TypeSpec.Generator.Providers;
using Microsoft.TypeSpec.Generator.Statements;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenAILibraryPlugin.Visitors;

/// <summary>
/// This visitor updates the public/internal visibility of constructors, methods, and properties on a type:
///   - Visibility can be customized via the [CodeGenVisibility] attribute, which will always take precedence
///   - Default visibility is adjusted based on common patterns
/// </summary>
public class VisibilityVisitor : ScmLibraryVisitor
{
    protected override TypeProvider? PostVisitType(TypeProvider type)
    {
        List<AttributeStatement> visibilityAttributes = GetValidatedCodeGenVisibilityAttributes(type);

        IReadOnlyList<ConstructorProvider> allConstructors =
            [
                .. type.Constructors,
                .. type.SerializationProviders.SelectMany(serializer => serializer.Constructors),
            ];
        IReadOnlyList<PropertyProvider> allProperties =
            [
                .. type.Properties,
                .. type.SerializationProviders.SelectMany(serializer => serializer.Properties),
            ];
        IReadOnlyList<MethodProvider> allMethods =
            [
                .. type.Methods,
                .. type.SerializationProviders.SelectMany(serializer => serializer.Methods),
            ];

        foreach (ConstructorProvider constructor in allConstructors)
        {
            if (!TryUpdateVisibilityFromAttributes(constructor, visibilityAttributes))
            {
                _ = TryUpdateDefaultVisibility(constructor);
            }
        }

        foreach (PropertyProvider property in allProperties)
        {
            if (!TryUpdateVisibilityFromAttributes(property, visibilityAttributes))
            {
                _ = TryUpdateDefaultVisibility(property);
            }
        }

        foreach (MethodProvider method in allMethods)
        {
            if (!TryUpdateVisibilityFromAttributes(method, visibilityAttributes))
            {
                _ = TryUpdateDefaultVisibility(method);
            }
        }

        return type;
    }

    private static List<AttributeStatement> GetValidatedCodeGenVisibilityAttributes(TypeProvider type)
    {
        IEnumerable<AttributeStatement> allAttributes =
        [
            .. type.Attributes,
            .. type.CustomCodeView?.Attributes ?? [],
            .. type.SerializationProviders.SelectMany(serializer => serializer.Attributes),
            .. type.SerializationProviders.SelectMany(serializer => serializer.CustomCodeView?.Attributes ?? []),
        ];

        List<AttributeStatement> matchingAttributes = [];

        foreach (AttributeStatement attribute in allAttributes.Where(attribute => attribute.Type.Name == "CodeGenVisibilityAttribute"))
        {
            if (attribute.Arguments.Count < 2 || attribute.Arguments[0] is not LiteralExpression)
            {
                throw new ArgumentException($"Invalid CodeGenVisibilityAttribute provided for {type.Name}; a target name and visibility specifier are required");
            }
            matchingAttributes.Add(attribute);
        }

        return matchingAttributes;
    }

    private static bool TryUpdateVisibilityFromAttributes<T>(T target, IEnumerable<AttributeStatement> visibilityAttributes)
    {
        (string targetName, IReadOnlyList<ParameterProvider> targetParameters, MethodSignatureModifiers startingModifiers, TypeProvider enclosingType) = target switch
        {
            PropertyProvider propertyTarget => (propertyTarget.Name, [], propertyTarget.Modifiers, propertyTarget.EnclosingType),
            ConstructorProvider constructorTarget => (constructorTarget.EnclosingType.Name, constructorTarget.Signature.Parameters, constructorTarget.Signature.Modifiers, constructorTarget.EnclosingType),
            MethodProvider methodTarget => (methodTarget.Signature.Name, methodTarget.Signature.Parameters, methodTarget.Signature.Modifiers, methodTarget.EnclosingType),
            _ => throw new NotImplementedException()
        };

        IEnumerable<AttributeStatement> allNameMatchedVisibilityAttributes = visibilityAttributes
            .Where(attribute => attribute.Arguments[0] is LiteralExpression literalAttributeTarget
                && attribute.Arguments.Count == targetParameters.Count + 2
                && literalAttributeTarget.Literal?.ToString() == targetName);

        string? visibilityFromAttribute = null;

        foreach (AttributeStatement attribute in allNameMatchedVisibilityAttributes)
        {
            bool mismatchFound = false;
            for (int i = 2; i < attribute.Arguments.Count; i++)
            {
                if (attribute.Arguments[i] is not TypeOfExpression typeOfExpression || typeOfExpression.Type.Name != targetParameters[i - 2].Type.Name)
                {
                    mismatchFound = true;
                    break;
                }
            }
            if (!mismatchFound)
            {
                visibilityFromAttribute = attribute.Arguments[1].ToDisplayString();
                break;
            }
        }

        MethodSignatureModifiers? modifierFromAttribute = visibilityFromAttribute switch
        {
            null => null,
            "0" => MethodSignatureModifiers.Internal,
            "1" => MethodSignatureModifiers.Public,
            _ => throw new NotImplementedException(),
        };

        return modifierFromAttribute is not null && AssignModifier(target, modifierFromAttribute.Value) != startingModifiers;
    }

    private static bool TryUpdateDefaultVisibility<T>(T target)
    {
        if (target is PropertyProvider propertyTarget)
        {
            if (!propertyTarget.Type.IsPublic && propertyTarget.Modifiers.HasFlag(MethodSignatureModifiers.Public))
            {
                // Generated properties reflecting internal types can default to internal visibility.
                AssignModifier(propertyTarget, MethodSignatureModifiers.Internal);
                return true;
            }
        }
        else if (target is ConstructorProvider constructorTarget)
        {
            if (constructorTarget.Signature.Modifiers.HasFlag(MethodSignatureModifiers.Public)
                && constructorTarget.Signature.Parameters.Any(parameter => !parameter.Type.IsPublic))
            {
                // By default, public constructors with non-public parameter types should be internal
                AssignModifier(constructorTarget, MethodSignatureModifiers.Internal);
            }
        }
        else if (target is MethodProvider methodTarget)
        {
            // No current default visibility change for methods
        }
        else
        {
            throw new NotImplementedException();
        }

        return false;
    }

    private static MethodSignatureModifiers AssignModifier<T>(T target, MethodSignatureModifiers modifier)
    {
        MethodSignatureModifiers GetUpdatedModifiers(MethodSignatureModifiers originalModifiers) => modifier switch
        {
            MethodSignatureModifiers.Public => originalModifiers & ~MethodSignatureModifiers.Internal & ~MethodSignatureModifiers.Private | MethodSignatureModifiers.Public,
            MethodSignatureModifiers.Internal => originalModifiers & ~MethodSignatureModifiers.Public & ~MethodSignatureModifiers.Private | MethodSignatureModifiers.Internal,
            _ => throw new NotImplementedException()
        };

        MethodSignatureModifiers updatedModifiers = default;

        if (target is PropertyProvider propertyTarget)
        {
            updatedModifiers = GetUpdatedModifiers(propertyTarget.Modifiers);
            propertyTarget.GetType().GetProperty("Modifiers")?.SetValue(propertyTarget, updatedModifiers);

            // Discriminators being made public should not be settable.
            if (propertyTarget.IsDiscriminator && modifier == MethodSignatureModifiers.Public && propertyTarget.Body is AutoPropertyBody existingBody)
            {
                propertyTarget.Update(body: new AutoPropertyBody(HasSetter: false, MethodSignatureModifiers.None, existingBody.InitializationExpression));
            }
        }
        else if (target is ConstructorSignature constructorSignatureTarget)
        {
            updatedModifiers = GetUpdatedModifiers(constructorSignatureTarget.Modifiers);
            constructorSignatureTarget.Update(modifiers: updatedModifiers);
        }
        else if (target is MethodSignature methodSignatureTarget)
        {
            updatedModifiers = GetUpdatedModifiers(methodSignatureTarget.Modifiers);
            methodSignatureTarget.Update(modifiers: updatedModifiers);
        }
        else if (target is ConstructorProvider finalConstructorTarget)
        {
            return AssignModifier(finalConstructorTarget.Signature, modifier);
        }
        else if (target is MethodProvider finalMethodTarget)
        {
            return AssignModifier(finalMethodTarget.Signature, modifier);
        }
        else
        {
            throw new NotImplementedException();
        }

        return updatedModifiers;
    }
}