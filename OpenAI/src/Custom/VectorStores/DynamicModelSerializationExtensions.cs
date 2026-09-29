namespace OpenAI.VectorStores;

internal static class DynamicModelSerializationExtensions
{
    public static string ToSerialString(this VectorStoreStatus value) => value.ToString();
    public static VectorStoreStatus ToVectorStoreStatus(this string value) => new(value);

    public static string ToSerialString(this VectorStoreFileStatus value) => value.ToString();
    public static VectorStoreFileStatus ToVectorStoreFileStatus(this string value) => new(value);
}