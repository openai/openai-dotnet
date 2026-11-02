using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Linq;

namespace OpenAI.Decisions;

[CodeGenVisibility(nameof(DecisionsJsonData), CodeGenVisibility.Internal)]
[CodeGenVisibility("Patch", CodeGenVisibility.Internal)]
public partial class DecisionsJsonData
{
    internal DecisionsJsonData()
    {
    }

#pragma warning disable SCME0001
    internal DecisionsJsonData(DecisionsJsonObject objectValue, IReadOnlyList<DecisionsJsonObject> arrayValue, in JsonPatch patch)
    {
        ObjectValue = objectValue;
        ArrayValue = arrayValue;
        _patch = patch;
        _patch.SetPropagators(PropagateSet, PropagateGet);
    }
#pragma warning restore SCME0001

    public DecisionsJsonData(DecisionsJsonObject objectValue)
    {
        Argument.AssertNotNull(objectValue, nameof(objectValue));
        ObjectValue = objectValue;
#pragma warning disable SCME0001
        _patch.SetPropagators(PropagateSet, PropagateGet);
#pragma warning restore SCME0001
    }

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

    [CodeGenMember("ObjectValue")]
    public DecisionsJsonObject ObjectValue { get; }

    [CodeGenMember("ArrayValue")]
    public IReadOnlyList<DecisionsJsonObject> ArrayValue { get; }

    public static implicit operator DecisionsJsonData(DecisionsJsonObject value) => value is null ? null : new(value);

    public static implicit operator DecisionsJsonData(DecisionsJsonObject[] value) => value is null ? null : new(value);
}
