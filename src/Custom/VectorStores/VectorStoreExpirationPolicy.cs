using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.VectorStores;

/// <summary>
/// Represents the the configuration that controls when a vector store will be automatically deleted.
/// </summary>
[Experimental("OPENAI001")]
[CodeGenModel("VectorStoreExpirationAfter")]
[CodeGenSuppress(nameof(VectorStoreExpirationPolicy))]
[CodeGenSuppress(nameof(VectorStoreExpirationPolicy), typeof(int))]
[CodeGenSuppress(nameof(VectorStoreExpirationPolicy), typeof(VectorStoreExpirationAnchor), typeof(int), typeof(IDictionary<string, BinaryData>))]
public partial class VectorStoreExpirationPolicy
{
    private IDictionary<string, BinaryData> SerializedAdditionalRawData;

    [CodeGenMember("Anchor")]
    private VectorStoreExpirationAnchor _anchor;
    [CodeGenMember("Days")]
    private int _days;

    /// <summary> Anchor timestamp after which the expiration policy applies. Supported anchors: `last_active_at`. </summary>
    public required VectorStoreExpirationAnchor Anchor
    {
        get => _anchor;
        set => _anchor = value;
    }

    /// <summary> The number of days after the anchor time that the vector store will expire. </summary>
    public required int Days
    {
        get => _days;
        set => _days = value;
    }

    /// <summary> Initializes a new instance of <see cref="VectorStoreExpirationPolicy"/>. </summary>
    [SetsRequiredMembers]
    public VectorStoreExpirationPolicy(VectorStoreExpirationAnchor anchor, int days)
        : this(anchor, days, null)
    {
        Days = days;
        Anchor = anchor;
    }

    /// <summary> Initializes a new instance of <see cref="VectorStoreExpirationPolicy"/>. </summary>
    public VectorStoreExpirationPolicy()
    {
        SerializedAdditionalRawData = new ChangeTrackingDictionary<string, BinaryData>();
    }

    /// <summary> Initializes a new instance of <see cref="VectorStoreExpirationPolicy"/>. </summary>
    /// <param name="anchor"> Anchor timestamp after which the expiration policy applies. Supported anchors: `last_active_at`. </param>
    /// <param name="days"> The number of days after the anchor time that the vector store will expire. </param>
    /// <param name="serializedAdditionalRawData"> Keeps track of any properties unknown to the library. </param>
    [SetsRequiredMembers]
    internal VectorStoreExpirationPolicy(VectorStoreExpirationAnchor anchor, int days, IDictionary<string, BinaryData> serializedAdditionalRawData)
    {
        Anchor = anchor;
        Days = days;
        SerializedAdditionalRawData = serializedAdditionalRawData ?? new ChangeTrackingDictionary<string, BinaryData>();
    }
}
