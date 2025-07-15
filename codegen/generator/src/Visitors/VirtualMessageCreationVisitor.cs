using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.Primitives;
using Microsoft.TypeSpec.Generator.Providers;

namespace OpenAILibraryPlugin.Visitors;

/// <summary>
/// This visitor updates generated PipelineMessage creation methods to be virtual (and thus overrideable via derived clients).
/// </summary>
public class VirtualMessageCreationVisitor : ScmLibraryVisitor
{
    protected override MethodProvider VisitMethod(MethodProvider method)
    {
        if (method.Signature?.ReturnType?.Name != "PipelineMessage"
            || method.Signature?.Name?.StartsWith("Create") != true
            || method.Signature?.Name?.EndsWith("Request") != true
            || method.Signature?.Modifiers.HasFlag(MethodSignatureModifiers.Internal) != true)
        {
            return method;
        }

        if (!method.Signature.Modifiers.HasFlag(MethodSignatureModifiers.Virtual))
        {
            MethodSignature newSignature = new(
                method.Signature.Name,
                method.Signature.Description,
                method.Signature.Modifiers | MethodSignatureModifiers.Virtual,
                method.Signature.ReturnType,
                method.Signature.ReturnDescription,
                method.Signature.Parameters,
                method.Signature.Attributes,
                method.Signature.GenericArguments,
                method.Signature.GenericParameterConstraints,
                method.Signature.ExplicitInterface,
                NonDocumentComment: "Plugin customization: make PipelineMessage creation methods virtual");
            method.Update(signature: newSignature);
        }

        return method;
    }
}