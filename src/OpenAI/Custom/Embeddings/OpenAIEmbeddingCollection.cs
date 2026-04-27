using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OpenAI.Embeddings;

/// <summary> A collection of embeddings. </summary>
[CodeGenType("CreateEmbeddingResponse")]
[CodeGenSuppress(nameof(OpenAIEmbeddingCollection), typeof(IEnumerable<OpenAIEmbedding>), typeof(string), typeof(EmbeddingTokenUsage))]
public partial class OpenAIEmbeddingCollection : ReadOnlyCollection<OpenAIEmbedding>
{
    // CUSTOM: Made private. This property does not add value in the context of a strongly-typed class.
    [CodeGenMember("Object")]
    private string Object { get; } = "list";

#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
    // CUSTOM: Set the inherited Items property via the base constructor.
    internal OpenAIEmbeddingCollection(IList<OpenAIEmbedding> items, string model, string @object, EmbeddingTokenUsage usage, in JsonPatch patch)
        : base(items ?? new ChangeTrackingList<OpenAIEmbedding>())
    {
        Model = model;
        Object = @object;
        Usage = usage;
        _patch = patch;
        _patch.SetPropagators(PropagateSet, PropagateGet);
    }
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

    // CUSTOM: Call the base constructor.
    internal OpenAIEmbeddingCollection() : this(null, null, null, null, default)
    {
    }
}