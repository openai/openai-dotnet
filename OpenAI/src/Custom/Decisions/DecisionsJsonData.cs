using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Linq;

namespace OpenAI.Decisions;

// CUSTOM: Represent the object/object-array union through composition, hiding empty construction and synthetic patch paths.
[CodeGenVisibility(nameof(DecisionsJsonData), CodeGenVisibility.Internal)]
[CodeGenVisibility("Patch", CodeGenVisibility.Internal)]
public partial class DecisionsJsonData
{
    // CUSTOM: Keep model reader/writer construction internal.
    internal DecisionsJsonData()
    {
    }

#pragma warning disable SCME0001
    // CUSTOM: Restore the active JSON component and initialize nested patch propagation during deserialization.
    internal DecisionsJsonData(DecisionsJsonObject objectValue, IReadOnlyList<DecisionsJsonObject> arrayValue, in JsonPatch patch)
    {
        ObjectValue = objectValue;
        ArrayValue = arrayValue;
        _patch = patch;
        _patch.SetPropagators(PropagateSet, PropagateGet);
    }
#pragma warning restore SCME0001

    // CUSTOM: Initialize only the object component and forward nested patches to its model.
    public DecisionsJsonData(DecisionsJsonObject objectValue)
    {
        Argument.AssertNotNull(objectValue, nameof(objectValue));
        ObjectValue = objectValue;
#pragma warning disable SCME0001
        _patch.SetPropagators(PropagateSet, PropagateGet);
#pragma warning restore SCME0001
    }

    // CUSTOM: Snapshot non-null object components in a read-only list and initialize indexed patch propagation.
    public DecisionsJsonData(IEnumerable<DecisionsJsonObject> arrayValue)
    {
        Argument.AssertNotNull(arrayValue, nameof(arrayValue));
        List<DecisionsJsonObject> objects = arrayValue.ToList();
        if (objects.Any(item => item is null))
        {
            throw new ArgumentException("Decision JSON data arrays must contain only objects.", nameof(arrayValue));
        }
        ArrayValue = objects.AsReadOnly();
#pragma warning disable SCME0001
        _patch.SetPropagators(PropagateSet, PropagateGet);
#pragma warning restore SCME0001
    }

    // CUSTOM: Keep the nullable object component read-only so the active component cannot change.
    [CodeGenMember("ObjectValue")]
    public DecisionsJsonObject ObjectValue { get; }

    // CUSTOM: Keep the nullable array component read-only so the active component cannot change.
    [CodeGenMember("ArrayValue")]
    public IReadOnlyList<DecisionsJsonObject> ArrayValue { get; }

    // CUSTOM: Allow convenient object assignment while preserving null as a null union reference.
    public static implicit operator DecisionsJsonData(DecisionsJsonObject value) => value is null ? null : new(value);

    // CUSTOM: Allow convenient object-array assignment while preserving null as a null union reference.
    public static implicit operator DecisionsJsonData(DecisionsJsonObject[] value) => value is null ? null : new(value);
}
