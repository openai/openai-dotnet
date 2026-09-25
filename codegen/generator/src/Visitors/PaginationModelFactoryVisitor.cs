using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.Primitives;
using Microsoft.TypeSpec.Generator.Providers;

namespace OpenAILibraryPlugin.Visitors;

/// <summary>
/// Preserves the established parameter order of pagination model factory methods while the
/// underlying shared models retain their HTTP query order.
/// </summary>
public class PaginationModelFactoryVisitor : ScmLibraryVisitor
{
    private static readonly Dictionary<string, string[]> _parameterOrder = new(StringComparer.Ordinal)
    {
        ["AssistantCollectionOptions"] = ["afterId", "beforeId", "pageSizeLimit", "order"],
        ["MessageCollectionOptions"] = ["afterId", "beforeId", "pageSizeLimit", "order"],
        ["RunCollectionOptions"] = ["afterId", "beforeId", "pageSizeLimit", "order"],
        ["RunStepCollectionOptions"] = ["afterId", "beforeId", "pageSizeLimit", "order"],
        ["VectorStoreCollectionOptions"] = ["afterId", "beforeId", "pageSizeLimit", "order"],
        ["VectorStoreFileCollectionOptions"] = ["afterId", "beforeId", "pageSizeLimit", "order", "filter"],
        ["ResponseItemCollectionOptions"] = ["responseId", "afterId", "beforeId", "pageSizeLimit", "order"],
    };

    protected override MethodProvider? VisitMethod(MethodProvider method)
    {
        if (method.EnclosingType is not ModelFactoryProvider ||
            !_parameterOrder.TryGetValue(method.Signature.Name, out var desiredOrder))
        {
            return base.VisitMethod(method);
        }

        var parametersByName = method.Signature.Parameters.ToDictionary(parameter => parameter.Name, StringComparer.Ordinal);
        if (parametersByName.Count != desiredOrder.Length || desiredOrder.Any(name => !parametersByName.ContainsKey(name)))
        {
            return base.VisitMethod(method);
        }

        var signature = method.Signature;
        var reorderedParameters = desiredOrder.Select(name => parametersByName[name]).ToArray();
        method.Update(signature: new MethodSignature(
            signature.Name,
            signature.Description,
            signature.Modifiers,
            signature.ReturnType,
            signature.ReturnDescription,
            reorderedParameters,
            signature.Attributes,
            signature.GenericArguments,
            signature.GenericParameterConstraints,
            signature.ExplicitInterface,
            signature.NonDocumentComment));
        return method;
    }
}
