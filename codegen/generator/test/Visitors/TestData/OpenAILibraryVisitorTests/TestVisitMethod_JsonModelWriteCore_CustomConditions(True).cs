string format = (options.Format == "W") ? ((global::System.ClientModel.Primitives.IPersistableModel<global::Samples.ChatCompletionOptions>)this).GetFormatFromOptions(options) : options.Format;
if ((format != "J"))
{
    throw new global::System.FormatException($"The model {nameof(global::Samples.ChatCompletionOptions)} does not support writing '{format}' format.");
}
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
// Plugin customization: apply Optional.Is*Defined() check based on type name dictionary lookup
if ((Optional.IsDefined(Model) && !Patch.Contains("$.model"u8)))
{
    writer.WritePropertyName("model"u8);
    writer.WriteStringValue(Model);
}

Patch.WriteTo(writer0);
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
