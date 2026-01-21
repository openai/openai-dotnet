using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.Providers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenAILibraryPlugin.Visitors;

/// <summary>
/// There are a few models that represent collections of other models. We have customized some of
/// these models to be classes derived from collection types (e.g., `ReadOnlyCollection<T>`).
/// But because these collection types already have an `Items` property that is inherited by the
/// derived class, there is a conflict when a second `Items` property is generated from the schema.
/// The purpose of this visitor is simply to remove that duplicated `Items` property and let the
/// classes use the inherited property instead.
/// </summary>
public class ItemsPropertyVisitor : ScmLibraryVisitor
{
    // Classes that have been customized to derive from collection types, e.g., `ReadOnlyCollection<T>`.
    private static readonly HashSet<string> _collectionTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "OpenAI.Embeddings.OpenAIEmbeddingCollection",
        "OpenAI.Images.GeneratedImageCollection",
        "OpenAI.Models.OpenAIModelCollection",
    };

    protected override TypeProvider? PostVisitType(TypeProvider typeProvider)
    {
        if (typeProvider is not null
            && _collectionTypes.Contains($"{typeProvider.Type.Namespace}.{typeProvider.Name}"))
        {
            // Remove the "Items" property from collection types.
            typeProvider.Update(
                properties: typeProvider.Properties
                    .Where(p => !p.Name.Equals("Items", StringComparison.OrdinalIgnoreCase)));
        }

        return typeProvider;
    }
}