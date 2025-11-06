string format = (options.Format == "W") ? ((global::System.ClientModel.Primitives.IPersistableModel<global::Samples.TestModel>)this).GetFormatFromOptions(options) : options.Format;
if ((format != "J"))
{
    throw new global::System.FormatException($"The model {nameof(global::Samples.TestModel)} does not support writing '{format}' format.");
}
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
if ((global::Samples.Optional.IsDefined(Cat) && !Patch.Contains("$.cat"u8)))
{
    writer.WritePropertyName("cat"u8);
    writer.WriteStringValue(Cat);
}
if (!Patch.Contains("$.requiredDog"u8))
{
    writer.WritePropertyName("requiredDog"u8);
    writer.WriteStringValue(RequiredDog);
}

Patch.WriteTo(writer0);
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
