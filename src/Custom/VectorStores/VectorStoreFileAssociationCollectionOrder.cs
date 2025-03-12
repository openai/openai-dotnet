using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.VectorStores;

[Experimental("OPENAI001")]
public readonly partial struct VectorStoreFileAssociationCollectionOrder : IEquatable<VectorStoreFileAssociationCollectionOrder>
{
    public static VectorStoreFileAssociationCollectionOrder Ascending { get; } = new VectorStoreFileAssociationCollectionOrder("asc");

    public static VectorStoreFileAssociationCollectionOrder Descending { get; } = new VectorStoreFileAssociationCollectionOrder("desc");

    private readonly string _value;
    private const string AscValue = "asc";
    private const string DescValue = "desc";

    public VectorStoreFileAssociationCollectionOrder(string value)
    {
        Argument.AssertNotNull(value, nameof(value));

        _value = value;
    }

    public static bool operator ==(VectorStoreFileAssociationCollectionOrder left, VectorStoreFileAssociationCollectionOrder right) => left.Equals(right);

    public static bool operator !=(VectorStoreFileAssociationCollectionOrder left, VectorStoreFileAssociationCollectionOrder right) => !left.Equals(right);

    public static implicit operator VectorStoreFileAssociationCollectionOrder(string value) => new VectorStoreFileAssociationCollectionOrder(value);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object obj) => obj is VectorStoreFileAssociationCollectionOrder other && Equals(other);

    public bool Equals(VectorStoreFileAssociationCollectionOrder other) => string.Equals(_value, other._value, StringComparison.InvariantCultureIgnoreCase);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() => _value != null ? StringComparer.InvariantCultureIgnoreCase.GetHashCode(_value) : 0;

    public override string ToString() => _value;
}
