namespace OpenAI.SpecProcessor.Spec;

/// <summary> Describes the kind of change detected between spec versions. </summary>
public enum ChangeType
{
    /// <summary> A new element was added. </summary>
    Added,

    /// <summary> An existing element was removed. </summary>
    Removed,

    /// <summary> An existing element was modified. </summary>
    Changed
}
