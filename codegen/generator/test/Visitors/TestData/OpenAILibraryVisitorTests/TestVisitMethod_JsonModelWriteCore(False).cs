string format = (options.Format == "W") ? ((global::System.ClientModel.Primitives.IPersistableModel<global::Samples.TestModel>)this).GetFormatFromOptions(options) : options.Format;
if ((format != "J"))
{
    throw new global::System.FormatException($"The model {nameof(global::Samples.TestModel)} does not support writing '{format}' format.");
}
if (global::Samples.Optional.IsDefined(Cat))
{
    writer.WritePropertyName("cat"u8);
    writer.WriteStringValue(Cat);
}
writer0.WritePropertyName("requiredDog"u8);
writer0.WriteStringValue(RequiredDog);
if (((options.Format != "W") && (_additionalBinaryDataProperties != null)))
{
    foreach (var item in _additionalBinaryDataProperties)
    {
        if (global::Samples.ModelSerializationExtensions.IsSentinelValue(item.Value))
        {
            continue;
        }
        writer0.WritePropertyName(item.Key);
#if NET6_0_OR_GREATER
        writer0.WriteRawValue(item.Value);
#else
        using (global::System.Text.Json.JsonDocument document = global::System.Text.Json.JsonDocument.Parse(item.Value))
        {
            global::System.Text.Json.JsonSerializer.Serialize(writer0, document.RootElement);
        }
#endif
    }
}
