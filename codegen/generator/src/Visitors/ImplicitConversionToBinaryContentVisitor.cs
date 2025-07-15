using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.Primitives;
using Microsoft.TypeSpec.Generator.Providers;

namespace OpenAILibraryPlugin.Visitors;

/// <summary>
/// This visitor removes the generated implicit operator that converts model to BinaryContent.
/// </summary>
public class ImplicitConversionToBinaryContentVisitor : ScmLibraryVisitor
{
    protected override MethodProvider VisitMethod(MethodProvider method)
    {
        if (method.Signature.Modifiers.HasFlag(MethodSignatureModifiers.Implicit) &&
            method.Signature.Modifiers.HasFlag(MethodSignatureModifiers.Operator) &&
            method.Signature.Name == "BinaryContent" &&
            method.Signature.Parameters.Count == 1)
        {
            return null;
        }

        return method;
    }
}
