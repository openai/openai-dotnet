using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OpenAI.Models;

/// <summary> A collection of models. </summary>
[CodeGenType("ListModelsResponse")]
[CodeGenSuppress(nameof(OpenAIModelCollection), typeof(IEnumerable<OpenAIModel>))]
public partial class OpenAIModelCollection : ReadOnlyCollection<OpenAIModel>
{
    // CUSTOM: Made private. This property does not add value in the context of a strongly-typed class.
    [CodeGenMember("Object")]
    private string Object { get; } = "list";

    // CUSTOM: Set the inherited Items property via the base constructor.
    internal OpenAIModelCollection(string @object, IList<OpenAIModel> items, IDictionary<string, BinaryData> additionalBinaryDataProperties)
        : base(items ?? new ChangeTrackingList<OpenAIModel>())
    {
        Object = @object;
        _additionalBinaryDataProperties = additionalBinaryDataProperties;
    }

    // CUSTOM: Call the base constructor.
    internal OpenAIModelCollection() : this(null, null, null)
    {
    }
}
