using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.Providers;
using Microsoft.TypeSpec.Generator.Snippets;
using Microsoft.TypeSpec.Generator.Statements;

namespace OpenAILibraryPlugin.Visitors;

public class ImageVariationDeprecationVisitor : ScmLibraryVisitor
{
    private static readonly HashSet<(string Namespace, string ClientName, string MethodName)> _methodsToDeprecate =
    [
        ("OpenAI.Images", "ImageClient", "GenerateImageVariations"),
        ("OpenAI.Images", "ImageClient", "GenerateImageVariationsAsync")
    ];

    private static readonly AttributeStatement _obsoleteAttribute = new(
        typeof(ObsoleteAttribute),
        Snippet.Literal("This endpoint is retired and no longer available. Use the image edits endpoint with a GPT Image model and a prompt to create a variation of an image."));

    protected override MethodProvider? VisitMethod(MethodProvider method)
    {
        var methodKey = (method.EnclosingType.Type.Namespace, method.EnclosingType.Name, method.Signature.Name);
        if (_methodsToDeprecate.Contains(methodKey) &&
            !method.Signature.Attributes.Any(attribute => attribute.Type.Equals(typeof(ObsoleteAttribute))))
        {
            method.Signature.Update(attributes: [.. method.Signature.Attributes, _obsoleteAttribute]);
        }

        return base.VisitMethod(method);
    }
}
