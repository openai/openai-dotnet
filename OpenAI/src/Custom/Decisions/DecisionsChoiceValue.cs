using Microsoft.TypeSpec.Generator.Customizations;
using System.ClientModel.Primitives;

namespace OpenAI.Decisions;

[CodeGenVisibility(nameof(DecisionsChoiceValue), CodeGenVisibility.Internal)]
[CodeGenVisibility("Patch", CodeGenVisibility.Internal)]
public partial class DecisionsChoiceValue
{
#pragma warning disable SCME0001
    internal DecisionsChoiceValue()
    {
        _patch.SetPropagators(PropagateSet, PropagateGet);
    }

    internal DecisionsChoiceValue(string stringValue, bool? booleanValue, in JsonPatch patch)
    {
        StringValue = stringValue;
        BooleanValue = booleanValue;
        _patch = patch;
        _patch.SetPropagators(PropagateSet, PropagateGet);
    }
#pragma warning restore SCME0001

    public DecisionsChoiceValue(string stringValue)
    {
        Argument.AssertNotNull(stringValue, nameof(stringValue));
        StringValue = stringValue;
#pragma warning disable SCME0001
        _patch.SetPropagators(PropagateSet, PropagateGet);
#pragma warning restore SCME0001
    }

    public DecisionsChoiceValue(bool booleanValue)
    {
        BooleanValue = booleanValue;
#pragma warning disable SCME0001
        _patch.SetPropagators(PropagateSet, PropagateGet);
#pragma warning restore SCME0001
    }

    [CodeGenMember("StringValue")]
    public string StringValue { get; }

    [CodeGenMember("BooleanValue")]
    public bool? BooleanValue { get; }

    public static implicit operator DecisionsChoiceValue(string value) => value is null ? null : new(value);

    public static implicit operator DecisionsChoiceValue(bool value) => new(value);
}
