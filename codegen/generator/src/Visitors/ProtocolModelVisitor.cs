using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.ClientModel.Providers;
using Microsoft.TypeSpec.Generator.Input;
using Microsoft.TypeSpec.Generator.Primitives;
using Microsoft.TypeSpec.Generator.Providers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenAILibraryPlugin.Visitors;

/// <summary>
/// This visitor provides the customizations needed for protocol models.
/// </summary>
public class ProtocolModelVisitor : ScmLibraryVisitor
{
    // Namespaces that contain protocol models.
    private static readonly HashSet<string> _protocolModelNamespaces = new(StringComparer.OrdinalIgnoreCase)
    {
        "OpenAI.Containers",
        "OpenAI.Conversations",
        "OpenAI.Responses",
    };

    // All the properties of protocol models should have setters, except for collection properties.
    protected override PropertyProvider? PreVisitProperty(InputProperty property, PropertyProvider? propertyProvider)
    {
        if (propertyProvider is not null
            && !propertyProvider.Type.IsCollection
            && _protocolModelNamespaces.Contains(propertyProvider.EnclosingType.Type.Namespace))
        {
            // Preserve the existing initialization expression, if any, so that default values are not lost.
            AutoPropertyBody? existingBody = propertyProvider.Body as AutoPropertyBody;
            propertyProvider.Update(body: new AutoPropertyBody(
                HasSetter: true,
                SetterModifiers: existingBody?.SetterModifiers ?? MethodSignatureModifiers.None,
                InitializationExpression: existingBody?.InitializationExpression));
        }

        return propertyProvider;
    }

    // All protocol models should have a public parameterless constructor.
    protected override TypeProvider? VisitType(TypeProvider typeProvider)
    {
        if (typeProvider is ModelProvider modelProvider
           && !modelProvider.Type.IsValueType
           && !modelProvider.DeclarationModifiers.HasFlag(TypeSignatureModifiers.Static))
        {
            List<ConstructorProvider> allGeneratedConstructors =
            [
              .. modelProvider.Constructors,
              .. modelProvider.SerializationProviders.SelectMany(mrwProvider => mrwProvider.Constructors),
            ];

            foreach (ConstructorProvider constructorProvider in allGeneratedConstructors)
            {
                if (constructorProvider is not null
                    && constructorProvider.Signature.Parameters.Count == 0 // Check that this is a default constructor
                    && modelProvider.DerivedModels.Count == 0 // The default constructor should be visible in the derived models, not the base model
                    && _protocolModelNamespaces.Contains(constructorProvider.EnclosingType.Type.Namespace))
                {
                    constructorProvider.Signature.Update(modifiers: MethodSignatureModifiers.Public);
                }
            }
        }

        return typeProvider;
    }

    // All protocol models should not have an implicit conversion to BinaryContent.
    // By contrast, this conversion should be surpressed for convenience models.
    protected override MethodProvider? VisitMethod(MethodProvider methodProvider)
    {
        if (methodProvider is not null
            && methodProvider.Signature.Modifiers.HasFlag(MethodSignatureModifiers.Implicit)
            && methodProvider.Signature.Modifiers.HasFlag(MethodSignatureModifiers.Operator)
            && methodProvider.Signature.Name == "BinaryContent"
            && methodProvider.Signature.Parameters.Count == 1
            && !_protocolModelNamespaces.Contains(methodProvider.EnclosingType.Type.Namespace))
        {
            return null;
        }

        return methodProvider;
    }
}