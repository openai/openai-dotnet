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
/// Finalizes visibility for generated constructors, methods, and properties, including serialization partials.
/// A matching [CodeGenVisibility] attribute is authoritative, even if it matches the current modifiers.
/// Without an override, two accessibility safeguards restrict the modifiers produced by codegen and preceding
/// visitors: public properties whose types are non-public become internal, and
/// public constructors with any non-public parameter type become internal. Other members retain their visibility.
/// </summary>
public class VisibilityVisitor : ScmLibraryVisitor
{
    protected override TypeProvider? PostVisitType(TypeProvider type)
    {
        // Generated and handwritten attributes can target members on either partial of the same model.
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
            if (!TryApplyVisibilityOverride(constructor, visibilityAttributes))
            {
                RestrictVisibilityForNonPublicTypes(constructor);
            }
        }

        foreach (PropertyProvider property in allProperties)
        {
            if (!TryApplyVisibilityOverride(property, visibilityAttributes))
            {
                RestrictVisibilityForNonPublicTypes(property);
            }
        }

        foreach (MethodProvider method in allMethods)
        {
            if (!TryApplyVisibilityOverride(method, visibilityAttributes))
            {
                RestrictVisibilityForNonPublicTypes(method);
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

        foreach (AttributeStatement attribute in allAttributes.Where(attribute => 
             attribute.Type.Name == "CodeGenVisibilityAttribute" || attribute.Type.Name == "CodeGenVisibility"))
        {
            if (attribute.Arguments.Count < 2 || attribute.Arguments[0] is not LiteralExpression)
            {
                // When Roslyn can't fully resolve the CodeGenVisibilityAttribute, its short name "CodeGenVisibility"
                // may also match other symbols (e.g. the CodeGenVisibility enum). Skip non-matching entries.
                continue;
            }
            matchingAttributes.Add(attribute);
        }

        return matchingAttributes;
    }

    private static bool TryApplyVisibilityOverride<T>(T target, IEnumerable<AttributeStatement> visibilityAttributes)
    {
        // Match the member name and parameter types so an override only applies to the intended overload.
        // Property overrides have no parameter types; constructor overrides use the enclosing type's name.
        (string targetName, IReadOnlyList<ParameterProvider> targetParameters) = target switch
        {
            PropertyProvider propertyTarget => (propertyTarget.Name, (IReadOnlyList<ParameterProvider>)[]),
            ConstructorProvider constructorTarget => (constructorTarget.EnclosingType.Name, constructorTarget.Signature.Parameters),
            MethodProvider methodTarget => (methodTarget.Signature.Name, methodTarget.Signature.Parameters),
            _ => throw new NotImplementedException()
        };

        // Arguments are member name, requested visibility, then one typeof argument per target parameter.
        // The extra two arguments explain Count + 2; matching name and arity narrows the overload candidates.
        IEnumerable<AttributeStatement> allNameMatchedVisibilityAttributes = visibilityAttributes
            .Where(attribute => attribute.Arguments[0] is LiteralExpression literalAttributeTarget
                && attribute.Arguments.Count == targetParameters.Count + 2
                && literalAttributeTarget.Literal?.ToString() == targetName);

        string? visibilityFromAttribute = null;

        foreach (AttributeStatement attribute in allNameMatchedVisibilityAttributes)
        {
            // Compare parameter type names in declaration order, skipping the name and visibility arguments.
            // Apply the first exact signature match; properties have no parameters to compare.
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
            "2" => MethodSignatureModifiers.Protected | MethodSignatureModifiers.Internal,
            _ => throw new NotImplementedException(),
        };

        if (modifierFromAttribute is null)
        {
            return false;
        }

        // True means an explicit override was found, not that visibility changed. Returning false for
        // an already-matching override would let accessibility safeguards overwrite that explicit choice.
        AssignModifier(target, modifierFromAttribute.Value);
        return true;
    }

    private static void RestrictVisibilityForNonPublicTypes<T>(T target)
    {
        // These fallback rules only run when no explicit override matched. They prevent generated public
        // signatures from exposing types that consumers cannot name; they do not promote any member.
        if (target is PropertyProvider propertyTarget)
        {
            if (!propertyTarget.Type.IsPublic && propertyTarget.Modifiers.HasFlag(MethodSignatureModifiers.Public))
            {
                // For example, a generated public property of an internal enum type must also be internal
                // to avoid inconsistent accessibility. This checks the property's type, not its declaring class.
                AssignModifier(propertyTarget, MethodSignatureModifiers.Internal);
            }
        }
        else if (target is ConstructorProvider constructorTarget)
        {
            if (constructorTarget.Signature.Modifiers.HasFlag(MethodSignatureModifiers.Public)
                && constructorTarget.Signature.Parameters.Any(parameter => !parameter.Type.IsPublic))
            {
                // Consumers cannot call a public constructor whose signature exposes a non-public type.
                AssignModifier(constructorTarget, MethodSignatureModifiers.Internal);
            }
        }
        else if (target is MethodProvider methodTarget)
        {
            // Method signatures are not currently restricted by this safeguard.
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    private static MethodSignatureModifiers AssignModifier<T>(T target, MethodSignatureModifiers modifier)
    {
        // Replace only accessibility flags, preserving modifiers such as static or virtual.
        const MethodSignatureModifiers visibilityMask =
            MethodSignatureModifiers.Public |
            MethodSignatureModifiers.Internal |
            MethodSignatureModifiers.Protected |
            MethodSignatureModifiers.Private;

        MethodSignatureModifiers GetUpdatedModifiers(MethodSignatureModifiers originalModifiers) => modifier switch
        {
            MethodSignatureModifiers.Public => originalModifiers & ~visibilityMask | MethodSignatureModifiers.Public,
            MethodSignatureModifiers.Internal => originalModifiers & ~visibilityMask | MethodSignatureModifiers.Internal,
            MethodSignatureModifiers.Protected | MethodSignatureModifiers.Internal => originalModifiers & ~visibilityMask | MethodSignatureModifiers.Protected | MethodSignatureModifiers.Internal,
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
