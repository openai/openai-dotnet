using Microsoft.TypeSpec.Generator.Customizations;
using System.ClientModel.Primitives;

namespace OpenAI.Decisions;

// CUSTOM: Represent the string/boolean union through composition, hiding empty construction and synthetic patch paths.
[CodeGenVisibility(nameof(DecisionsChoiceValue), CodeGenVisibility.Internal)]
[CodeGenVisibility("Patch", CodeGenVisibility.Internal)]
public partial class DecisionsChoiceValue
{
#pragma warning disable SCME0001
    // CUSTOM: Initialize wire-value patch propagation for model reader/writer construction.
    internal DecisionsChoiceValue()
    {
        _patch.SetPropagators(PropagateSet, PropagateGet);
    }

    // CUSTOM: Restore the active union component and its patch while initializing propagation during deserialization.
    internal DecisionsChoiceValue(string stringValue, bool? booleanValue, in JsonPatch patch)
    {
        StringValue = stringValue;
        BooleanValue = booleanValue;
        _patch = patch;
        _patch.SetPropagators(PropagateSet, PropagateGet);
    }
#pragma warning restore SCME0001

    // CUSTOM: Initialize only the string component and reject null union instances.
    public DecisionsChoiceValue(string stringValue)
    {
        Argument.AssertNotNull(stringValue, nameof(stringValue));
        StringValue = stringValue;
#pragma warning disable SCME0001
        _patch.SetPropagators(PropagateSet, PropagateGet);
#pragma warning restore SCME0001
    }

    // CUSTOM: Initialize only the boolean component, including false as an active value.
    public DecisionsChoiceValue(bool booleanValue)
    {
        BooleanValue = booleanValue;
#pragma warning disable SCME0001
        _patch.SetPropagators(PropagateSet, PropagateGet);
#pragma warning restore SCME0001
    }

    // CUSTOM: Keep the nullable string component read-only so the active component cannot change.
    [CodeGenMember("StringValue")]
    public string StringValue { get; }

    // CUSTOM: Keep the nullable boolean component read-only so the active component cannot change.
    [CodeGenMember("BooleanValue")]
    public bool? BooleanValue { get; }

    // CUSTOM: Allow convenient string assignment while preserving null as a null union reference.
    public static implicit operator DecisionsChoiceValue(string value) => value is null ? null : new(value);

    // CUSTOM: Allow convenient boolean assignment without exposing the synthetic wrapper shape.
    public static implicit operator DecisionsChoiceValue(bool value) => new(value);
}
