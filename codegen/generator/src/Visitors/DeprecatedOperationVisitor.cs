using System;
using System.Linq;
using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.ClientModel.Providers;
using Microsoft.TypeSpec.Generator.Primitives;
using Microsoft.TypeSpec.Generator.Providers;
using Microsoft.TypeSpec.Generator.Snippets;
using Microsoft.TypeSpec.Generator.Statements;

namespace OpenAILibraryPlugin.Visitors;

/// <summary>
/// Adds <see cref="ObsoleteAttribute"/> to client methods for operations marked with TypeSpec <c>#deprecated</c>.
/// </summary>
public class DeprecatedOperationVisitor : ScmLibraryVisitor
{
    protected override MethodProvider? VisitMethod(MethodProvider method)
    {
        if (method is not ScmMethodProvider
            {
                Kind: ScmMethodKind.Protocol or ScmMethodKind.Convenience,
                ServiceMethod.Operation.Deprecated: { } message
            })
        {
            return base.VisitMethod(method);
        }

        if (method.Signature.Attributes.FirstOrDefault(IsObsolete) is { } existing)
        {
            // The generator can strip its original attribute when the last public contract was
            // not obsolete. Replace it with an explicit SDK annotation, preserving its arguments.
            method.Signature.Update(attributes: [.. method.Signature.Attributes.Select(attribute =>
                ReferenceEquals(attribute, existing)
                    ? new AttributeStatement(existing.Type, existing.Arguments, existing.PositionalArguments)
                    : attribute)]);
        }
        else if (method.EnclosingType.CustomCodeView?.Methods.Any(customMethod =>
            MethodSignatureBase.SignatureComparer.Equals(customMethod.Signature, method.Signature) &&
            customMethod.Signature.Attributes.Any(IsObsolete)) != true)
        {
            method.Signature.Update(attributes: [.. method.Signature.Attributes,
                new AttributeStatement(typeof(ObsoleteAttribute), Snippet.Literal(message))]);
        }

        return base.VisitMethod(method);
    }

    private static bool IsObsolete(AttributeStatement attribute) => attribute.Type.Equals(typeof(ObsoleteAttribute));
}
