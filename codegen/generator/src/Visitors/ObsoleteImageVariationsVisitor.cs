using System;
using System.ClientModel;
using System.Linq;
using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.Providers;
using Microsoft.TypeSpec.Generator.Snippets;
using Microsoft.TypeSpec.Generator.Statements;

namespace OpenAILibraryPlugin.Visitors;

/// <summary>
/// Marks the generated image variation protocol methods as obsolete.
/// </summary>
public class ObsoleteImageVariationsVisitor : ScmLibraryVisitor
{
    private static readonly AttributeStatement _obsoleteAttribute = new(
        typeof(ObsoleteAttribute),
        Snippet.Literal(
            "This endpoint is retired and no longer available. " +
            "Use the image edits endpoint with a GPT Image model " +
            "and a prompt to create a variation of an image."));

    protected override MethodProvider? VisitMethod(MethodProvider method)
    {
        if (method.EnclosingType.Type.Namespace != "OpenAI.Images" ||
            method.EnclosingType.Name != "ImageClient" ||
            method.Signature.Name is not (
                "GenerateImageVariations" or "GenerateImageVariationsAsync") ||
            method.Signature.Parameters.Count == 0 ||
            !method.Signature.Parameters[0].Type.Equals(typeof(BinaryContent)))
        {
            return method;
        }

        if (!method.Signature.Attributes.Any(
            attribute => attribute.Type.Equals(typeof(ObsoleteAttribute))))
        {
            method.Signature.Update(
                attributes: [.. method.Signature.Attributes, _obsoleteAttribute]);
        }

        return method;
    }
}
