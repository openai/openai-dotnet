using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.Primitives;
using Microsoft.TypeSpec.Generator.Providers;
using System.ClientModel;

namespace OpenAILibraryPlugin.Visitors;

/// <summary>
/// This visitor removes the generated implicit operator that converts model to BinaryContent.
/// </summary>
public class ExplicitConversionFromClientResultVisitor : ScmLibraryVisitor
{
    protected override MethodProvider VisitMethod(MethodProvider method)
    {
        if (method.Signature.Modifiers.HasFlag(MethodSignatureModifiers.Explicit) &&
            method.Signature.Modifiers.HasFlag(MethodSignatureModifiers.Operator) &&
            method.Signature.Parameters.Count == 1 &&
            method.Signature.Parameters[0].Type.Name == nameof(ClientResult))
        {
            return null;
        }

        return method;
    }
}
