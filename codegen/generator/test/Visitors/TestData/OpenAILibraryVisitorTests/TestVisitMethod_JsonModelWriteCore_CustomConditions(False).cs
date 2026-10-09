string format = (options.Format == "W") ? ((global::System.ClientModel.Primitives.IPersistableModel<global::Samples.ChatCompletionOptions>)this).GetFormatFromOptions(options) : options.Format;
if ((format != "J"))
{
    throw new global::System.FormatException($"The model {nameof(global::Samples.ChatCompletionOptions)} does not support writing '{format}' format.");
}
// Plugin customization: apply Optional.Is*Defined() check based on type name dictionary lookup
if ((Optional.IsDefined(Model) && (this._additionalBinaryDataProperties?.ContainsKey("model") != true)))
{
    writer.WritePropertyName("model"u8);
    writer.WriteStringValue(Model);
}
if (((options.Format != "W") && (_additionalBinaryDataProperties != null)))
{
    foreach (var item in _additionalBinaryDataProperties)
    {
        if (global::Samples.ModelSerializationExtensions.IsSentinelValue(item.Value))
        {
            continue;
        }
        writer.WritePropertyName(item.Key);
#if NET6_0_OR_GREATER
        writer.WriteRawValue(item.Value);
#else
        using (global::System.Text.Json.JsonDocument document = global::System.Text.Json.JsonDocument.Parse(item.Value))
        {
            global::System.Text.Json.JsonSerializer.Serialize(writer, document.RootElement);
        }
#endif
    }
}
