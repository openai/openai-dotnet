// using System;
// using System.ClientModel;
// using System.ClientModel.Primitives;
// using System.Collections.Generic;
// using System.Text;
// using System.Text.Json;
// using OpenAI.Responses;

// namespace OpenAI.Responses
// {
//     public partial class CreateResponseOptions : IJsonModel<CreateResponseOptions>
//     {
//         internal CreateResponseOptions() : this(null, default, default, null, default, null, default, null, default, default, null, null, null, null, default, null, null, default, default, default, default)
//         {
//         }

//         void IJsonModel<CreateResponseOptions>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
//         {
// #pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
//             if (Patch.Contains("$"u8))
//             {
//                 writer.WriteRawValue(Patch.GetJson("$"u8));
//                 return;
//             }
// #pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

//             writer.WriteStartObject();
//             JsonModelWriteCore(writer, options);
//             writer.WriteEndObject();
//         }

//         protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
//         {
//             string format = options.Format == "W" ? ((IPersistableModel<CreateResponseOptions>)this).GetFormatFromOptions(options) : options.Format;
//             if (format != "J")
//             {
//                 throw new FormatException($"The model {nameof(CreateResponseOptions)} does not support writing '{format}' format.");
//             }
// #pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
//             if (Optional.IsCollectionDefined(Metadata) && !Patch.Contains("$.metadata"u8))
//             {
//                 writer.WritePropertyName("metadata"u8);
//                 writer.WriteStartObject();
// #if NET8_0_OR_GREATER
//                 global::System.Span<byte> buffer = stackalloc byte[256];
// #endif
//                 foreach (var item in Metadata)
//                 {
// #if NET8_0_OR_GREATER
//                     int bytesWritten = global::System.Text.Encoding.UTF8.GetBytes(item.Key.AsSpan(), buffer);
//                     bool patchContains = (bytesWritten == 256) ? Patch.Contains("$.metadata"u8, global::System.Text.Encoding.UTF8.GetBytes(item.Key)) : Patch.Contains("$.metadata"u8, buffer.Slice(0, bytesWritten));
// #else
//                     bool patchContains = Patch.Contains("$.metadata"u8, Encoding.UTF8.GetBytes(item.Key));
// #endif
//                     if (!patchContains)
//                     {
//                         writer.WritePropertyName(item.Key);
//                         if (item.Value == null)
//                         {
//                             writer.WriteNullValue();
//                             continue;
//                         }
//                         writer.WriteStringValue(item.Value);
//                     }
//                 }

//                 Patch.WriteTo(writer, "$.metadata"u8);
//                 writer.WriteEndObject();
//             }
//             if (Optional.IsDefined(Temperature) && !Patch.Contains("$.temperature"u8))
//             {
//                 writer.WritePropertyName("temperature"u8);
//                 writer.WriteNumberValue(Temperature.Value);
//             }
//             if (Optional.IsDefined(TopP) && !Patch.Contains("$.top_p"u8))
//             {
//                 writer.WritePropertyName("top_p"u8);
//                 writer.WriteNumberValue(TopP.Value);
//             }
//             if (Optional.IsDefined(EndUserId) && !Patch.Contains("$.user"u8))
//             {
//                 writer.WritePropertyName("user"u8);
//                 writer.WriteStringValue(EndUserId);
//             }
//             if (Optional.IsDefined(ServiceTier) && !Patch.Contains("$.service_tier"u8))
//             {
//                 writer.WritePropertyName("service_tier"u8);
//                 writer.WriteStringValue(ServiceTier.Value.ToString());
//             }
//             if (Optional.IsDefined(PreviousResponseId) && !Patch.Contains("$.previous_response_id"u8))
//             {
//                 writer.WritePropertyName("previous_response_id"u8);
//                 writer.WriteStringValue(PreviousResponseId);
//             }
//             if (Optional.IsDefined(Model) && !Patch.Contains("$.model"u8))
//             {
//                 writer.WritePropertyName("model"u8);
//                 writer.WriteStringValue(Model.Value.ToString());
//             }
//             if (Optional.IsDefined(ReasoningOptions) && !Patch.Contains("$.reasoning"u8))
//             {
//                 writer.WritePropertyName("reasoning"u8);
//                 writer.WriteObjectValue(ReasoningOptions, options);
//             }
//             if (Optional.IsDefined(IsBackgroundModeEnabled) && !Patch.Contains("$.background"u8))
//             {
//                 writer.WritePropertyName("background"u8);
//                 writer.WriteBooleanValue(IsBackgroundModeEnabled.Value);
//             }
//             if (Optional.IsDefined(MaxOutputTokenCount) && !Patch.Contains("$.max_output_tokens"u8))
//             {
//                 writer.WritePropertyName("max_output_tokens"u8);
//                 writer.WriteNumberValue(MaxOutputTokenCount.Value);
//             }
//             if (Optional.IsDefined(Instructions) && !Patch.Contains("$.instructions"u8))
//             {
//                 writer.WritePropertyName("instructions"u8);
//                 writer.WriteStringValue(Instructions);
//             }
//             if (Optional.IsDefined(TextOptions) && !Patch.Contains("$.text"u8))
//             {
//                 writer.WritePropertyName("text"u8);
//                 writer.WriteObjectValue(TextOptions, options);
//             }
//             if (Patch.Contains("$.tools"u8))
//             {
//                 if (!Patch.IsRemoved("$.tools"u8))
//                 {
//                     writer.WritePropertyName("tools"u8);
//                     writer.WriteRawValue(Patch.GetJson("$.tools"u8));
//                 }
//             }
//             else if (Optional.IsCollectionDefined(Tools))
//             {
//                 writer.WritePropertyName("tools"u8);
//                 writer.WriteStartArray();
//                 for (int i = 0; i < Tools.Count; i++)
//                 {
//                     if (Tools[i].Patch.IsRemoved("$"u8))
//                     {
//                         continue;
//                     }
//                     writer.WriteObjectValue(Tools[i], options);
//                 }
//                 Patch.WriteTo(writer, "$.tools"u8);
//                 writer.WriteEndArray();
//             }
//             if (Optional.IsDefined(ToolChoice) && !Patch.Contains("$.tool_choice"u8))
//             {
//                 writer.WritePropertyName("tool_choice"u8);
//                 writer.WriteObjectValue(ToolChoice, options);
//             }
//             if (Optional.IsDefined(TruncationMode) && !Patch.Contains("$.truncation"u8))
//             {
//                 writer.WritePropertyName("truncation"u8);
//                 writer.WriteStringValue(TruncationMode.Value.ToString());
//             }
//             if (Patch.Contains("$.input"u8))
//             {
//                 if (!Patch.IsRemoved("$.input"u8))
//                 {
//                     writer.WritePropertyName("input"u8);
//                     writer.WriteRawValue(Patch.GetJson("$.input"u8));
//                 }
//             }
//             else
//             {
//                 writer.WritePropertyName("input"u8);
//                 writer.WriteStartArray();
//                 for (int i = 0; i < Input.Count; i++)
//                 {
//                     if (Input[i].Patch.IsRemoved("$"u8))
//                     {
//                         continue;
//                     }
//                     writer.WriteObjectValue(Input[i], options);
//                 }
//                 Patch.WriteTo(writer, "$.input"u8);
//                 writer.WriteEndArray();
//             }
//             if (Patch.Contains("$.include"u8))
//             {
//                 if (!Patch.IsRemoved("$.include"u8))
//                 {
//                     writer.WritePropertyName("include"u8);
//                     writer.WriteRawValue(Patch.GetJson("$.include"u8));
//                 }
//             }
//             else if (Optional.IsCollectionDefined(Include))
//             {
//                 writer.WritePropertyName("include"u8);
//                 writer.WriteStartArray();
//                 for (int i = 0; i < Include.Count; i++)
//                 {
//                     if (Patch.IsRemoved(Encoding.UTF8.GetBytes($"$.include[{i}]")))
//                     {
//                         continue;
//                     }
//                     writer.WriteStringValue(Include[i].ToSerialString());
//                 }
//                 Patch.WriteTo(writer, "$.include"u8);
//                 writer.WriteEndArray();
//             }
//             if (Optional.IsDefined(IsParallelToolCallsEnabled) && !Patch.Contains("$.parallel_tool_calls"u8))
//             {
//                 writer.WritePropertyName("parallel_tool_calls"u8);
//                 writer.WriteBooleanValue(IsParallelToolCallsEnabled.Value);
//             }
//             if (Optional.IsDefined(IsStoredOutputEnabled) && !Patch.Contains("$.store"u8))
//             {
//                 writer.WritePropertyName("store"u8);
//                 writer.WriteBooleanValue(IsStoredOutputEnabled.Value);
//             }
//             if (Optional.IsDefined(IsStreamingEnabled) && !Patch.Contains("$.stream"u8))
//             {
//                 writer.WritePropertyName("stream"u8);
//                 writer.WriteBooleanValue(IsStreamingEnabled.Value);
//             }

//             Patch.WriteTo(writer);
// #pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
//         }

//         CreateResponseOptions IJsonModel<CreateResponseOptions>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options) => JsonModelCreateCore(ref reader, options);

//         protected virtual CreateResponseOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
//         {
//             string format = options.Format == "W" ? ((IPersistableModel<CreateResponseOptions>)this).GetFormatFromOptions(options) : options.Format;
//             if (format != "J")
//             {
//                 throw new FormatException($"The model {nameof(CreateResponseOptions)} does not support reading '{format}' format.");
//             }
//             using JsonDocument document = JsonDocument.ParseValue(ref reader);
//             return DeserializeCreateResponseOptions(document.RootElement, null, options);
//         }

//         internal static CreateResponseOptions DeserializeCreateResponseOptions(JsonElement element, BinaryData data, ModelReaderWriterOptions options)
//         {
//             if (element.ValueKind == JsonValueKind.Null)
//             {
//                 return null;
//             }
//             IDictionary<string, string> metadata = default;
//             float? temperature = default;
//             float? topP = default;
//             string user = default;
//             ResponseServiceTier? serviceTier = default;
//             string previousResponseId = default;
//             ModelIdsResponses? model = default;
//             ResponseReasoningOptions reasoning = default;
//             bool? background = default;
//             int? maxOutputTokens = default;
//             string instructions = default;
//             ResponseTextOptions text = default;
//             IList<ResponseTool> tools = default;
//             ResponseToolChoice toolChoice = default;
//             ResponseTruncationMode? truncation = default;
//             IList<ResponseItem> input = default;
//             IList<Includable> include = default;
//             bool? parallelToolCalls = default;
//             bool? store = default;
//             bool? stream = default;
// #pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
//             JsonPatch patch = new JsonPatch(data is null ? ReadOnlyMemory<byte>.Empty : data.ToMemory());
// #pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
//             foreach (var prop in element.EnumerateObject())
//             {
//                 if (prop.NameEquals("metadata"u8))
//                 {
//                     if (prop.Value.ValueKind == JsonValueKind.Null)
//                     {
//                         continue;
//                     }
//                     Dictionary<string, string> dictionary = new Dictionary<string, string>();
//                     foreach (var prop0 in prop.Value.EnumerateObject())
//                     {
//                         if (prop0.Value.ValueKind == JsonValueKind.Null)
//                         {
//                             dictionary.Add(prop0.Name, null);
//                         }
//                         else
//                         {
//                             dictionary.Add(prop0.Name, prop0.Value.GetString());
//                         }
//                     }
//                     metadata = dictionary;
//                     continue;
//                 }
//                 if (prop.NameEquals("temperature"u8))
//                 {
//                     if (prop.Value.ValueKind == JsonValueKind.Null)
//                     {
//                         temperature = null;
//                         continue;
//                     }
//                     temperature = prop.Value.GetSingle();
//                     continue;
//                 }
//                 if (prop.NameEquals("top_p"u8))
//                 {
//                     if (prop.Value.ValueKind == JsonValueKind.Null)
//                     {
//                         topP = null;
//                         continue;
//                     }
//                     topP = prop.Value.GetSingle();
//                     continue;
//                 }
//                 if (prop.NameEquals("user"u8))
//                 {
//                     user = prop.Value.GetString();
//                     continue;
//                 }
//                 if (prop.NameEquals("service_tier"u8))
//                 {
//                     if (prop.Value.ValueKind == JsonValueKind.Null)
//                     {
//                         continue;
//                     }
//                     serviceTier = new ResponseServiceTier(prop.Value.GetString());
//                     continue;
//                 }
//                 if (prop.NameEquals("previous_response_id"u8))
//                 {
//                     if (prop.Value.ValueKind == JsonValueKind.Null)
//                     {
//                         previousResponseId = null;
//                         continue;
//                     }
//                     previousResponseId = prop.Value.GetString();
//                     continue;
//                 }
//                 if (prop.NameEquals("model"u8))
//                 {
//                     if (prop.Value.ValueKind == JsonValueKind.Null)
//                     {
//                         continue;
//                     }
//                     model = new ModelIdsResponses(prop.Value.GetString());
//                     continue;
//                 }
//                 if (prop.NameEquals("reasoning"u8))
//                 {
//                     if (prop.Value.ValueKind == JsonValueKind.Null)
//                     {
//                         reasoning = null;
//                         continue;
//                     }
//                     reasoning = ResponseReasoningOptions.DeserializeResponseReasoningOptions(prop.Value, prop.Value.GetUtf8Bytes(), options);
//                     continue;
//                 }
//                 if (prop.NameEquals("background"u8))
//                 {
//                     if (prop.Value.ValueKind == JsonValueKind.Null)
//                     {
//                         background = null;
//                         continue;
//                     }
//                     background = prop.Value.GetBoolean();
//                     continue;
//                 }
//                 if (prop.NameEquals("max_output_tokens"u8))
//                 {
//                     if (prop.Value.ValueKind == JsonValueKind.Null)
//                     {
//                         maxOutputTokens = null;
//                         continue;
//                     }
//                     maxOutputTokens = prop.Value.GetInt32();
//                     continue;
//                 }
//                 if (prop.NameEquals("instructions"u8))
//                 {
//                     if (prop.Value.ValueKind == JsonValueKind.Null)
//                     {
//                         instructions = null;
//                         continue;
//                     }
//                     instructions = prop.Value.GetString();
//                     continue;
//                 }
//                 if (prop.NameEquals("text"u8))
//                 {
//                     if (prop.Value.ValueKind == JsonValueKind.Null)
//                     {
//                         continue;
//                     }
//                     text = ResponseTextOptions.DeserializeResponseTextOptions(prop.Value, prop.Value.GetUtf8Bytes(), options);
//                     continue;
//                 }
//                 if (prop.NameEquals("tools"u8))
//                 {
//                     if (prop.Value.ValueKind == JsonValueKind.Null)
//                     {
//                         continue;
//                     }
//                     List<ResponseTool> array = new List<ResponseTool>();
//                     foreach (var item in prop.Value.EnumerateArray())
//                     {
//                         array.Add(ResponseTool.DeserializeResponseTool(item, item.GetUtf8Bytes(), options));
//                     }
//                     tools = array;
//                     continue;
//                 }
//                 if (prop.NameEquals("tool_choice"u8))
//                 {
//                     if (prop.Value.ValueKind == JsonValueKind.Null)
//                     {
//                         continue;
//                     }
//                     toolChoice = ResponseToolChoice.DeserializeResponseToolChoice(prop.Value, options);
//                     continue;
//                 }
//                 if (prop.NameEquals("truncation"u8))
//                 {
//                     if (prop.Value.ValueKind == JsonValueKind.Null)
//                     {
//                         truncation = null;
//                         continue;
//                     }
//                     truncation = new ResponseTruncationMode(prop.Value.GetString());
//                     continue;
//                 }
//                 if (prop.NameEquals("input"u8))
//                 {
//                     List<ResponseItem> array = new List<ResponseItem>();
//                     foreach (var item in prop.Value.EnumerateArray())
//                     {
//                         array.Add(ResponseItem.DeserializeResponseItem(item, item.GetUtf8Bytes(), options));
//                     }
//                     input = array;
//                     continue;
//                 }
//                 if (prop.NameEquals("include"u8))
//                 {
//                     if (prop.Value.ValueKind == JsonValueKind.Null)
//                     {
//                         continue;
//                     }
//                     List<Includable> array = new List<Includable>();
//                     foreach (var item in prop.Value.EnumerateArray())
//                     {
//                         array.Add(item.GetString().ToIncludable());
//                     }
//                     include = array;
//                     continue;
//                 }
//                 if (prop.NameEquals("parallel_tool_calls"u8))
//                 {
//                     if (prop.Value.ValueKind == JsonValueKind.Null)
//                     {
//                         parallelToolCalls = null;
//                         continue;
//                     }
//                     parallelToolCalls = prop.Value.GetBoolean();
//                     continue;
//                 }
//                 if (prop.NameEquals("store"u8))
//                 {
//                     if (prop.Value.ValueKind == JsonValueKind.Null)
//                     {
//                         store = null;
//                         continue;
//                     }
//                     store = prop.Value.GetBoolean();
//                     continue;
//                 }
//                 if (prop.NameEquals("stream"u8))
//                 {
//                     if (prop.Value.ValueKind == JsonValueKind.Null)
//                     {
//                         stream = null;
//                         continue;
//                     }
//                     stream = prop.Value.GetBoolean();
//                     continue;
//                 }
//                 patch.Set([.. "$."u8, .. Encoding.UTF8.GetBytes(prop.Name)], prop.Value.GetUtf8Bytes());
//             }
//             return new CreateResponseOptions(
//                 metadata ?? new ChangeTrackingDictionary<string, string>(),
//                 temperature,
//                 topP,
//                 user,
//                 serviceTier,
//                 previousResponseId,
//                 model,
//                 reasoning,
//                 background,
//                 maxOutputTokens,
//                 instructions,
//                 text,
//                 tools ?? new ChangeTrackingList<ResponseTool>(),
//                 toolChoice,
//                 truncation,
//                 input,
//                 include ?? new ChangeTrackingList<Includable>(),
//                 parallelToolCalls,
//                 store,
//                 stream,
//                 patch);
//         }

//         BinaryData IPersistableModel<CreateResponseOptions>.Write(ModelReaderWriterOptions options) => PersistableModelWriteCore(options);

//         protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options)
//         {
//             string format = options.Format == "W" ? ((IPersistableModel<CreateResponseOptions>)this).GetFormatFromOptions(options) : options.Format;
//             switch (format)
//             {
//                 case "J":
//                     return ModelReaderWriter.Write(this, options, OpenAIContext.Default);
//                 default:
//                     throw new FormatException($"The model {nameof(CreateResponseOptions)} does not support writing '{options.Format}' format.");
//             }
//         }

//         CreateResponseOptions IPersistableModel<CreateResponseOptions>.Create(BinaryData data, ModelReaderWriterOptions options) => PersistableModelCreateCore(data, options);

//         protected virtual CreateResponseOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options)
//         {
//             string format = options.Format == "W" ? ((IPersistableModel<CreateResponseOptions>)this).GetFormatFromOptions(options) : options.Format;
//             switch (format)
//             {
//                 case "J":
//                     using (JsonDocument document = JsonDocument.Parse(data))
//                     {
//                         return DeserializeCreateResponseOptions(document.RootElement, data, options);
//                     }
//                 default:
//                     throw new FormatException($"The model {nameof(CreateResponseOptions)} does not support reading '{options.Format}' format.");
//             }
//         }

//         string IPersistableModel<CreateResponseOptions>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";

// #pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
//         private bool PropagateGet(ReadOnlySpan<byte> jsonPath, out JsonPatch.EncodedValue value)
//         {
//             ReadOnlySpan<byte> local = jsonPath.SliceToStartOfPropertyName();
//             value = default;

//             if (local.StartsWith("reasoning"u8))
//             {
//                 return ReasoningOptions.Patch.TryGetEncodedValue([.. "$"u8, .. local.Slice("reasoning"u8.Length)], out value);
//             }
//             if (local.StartsWith("text"u8))
//             {
//                 return TextOptions.Patch.TryGetEncodedValue([.. "$"u8, .. local.Slice("text"u8.Length)], out value);
//             }
//             if (local.StartsWith("tools"u8))
//             {
//                 int propertyLength = "tools"u8.Length;
//                 ReadOnlySpan<byte> currentSlice = local.Slice(propertyLength);
//                 if (!currentSlice.TryGetIndex(out int index, out int bytesConsumed))
//                 {
//                     return false;
//                 }
//                 return Tools[index].Patch.TryGetEncodedValue([.. "$"u8, .. currentSlice.Slice(bytesConsumed)], out value);
//             }
//             return false;
//         }
// #pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

// #pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
//         private bool PropagateSet(ReadOnlySpan<byte> jsonPath, JsonPatch.EncodedValue value)
//         {
//             ReadOnlySpan<byte> local = jsonPath.SliceToStartOfPropertyName();

//             if (local.StartsWith("reasoning"u8))
//             {
//                 ReasoningOptions.Patch.Set([.. "$"u8, .. local.Slice("reasoning"u8.Length)], value);
//                 return true;
//             }
//             if (local.StartsWith("text"u8))
//             {
//                 TextOptions.Patch.Set([.. "$"u8, .. local.Slice("text"u8.Length)], value);
//                 return true;
//             }
//             if (local.StartsWith("tools"u8))
//             {
//                 int propertyLength = "tools"u8.Length;
//                 ReadOnlySpan<byte> currentSlice = local.Slice(propertyLength);
//                 if (!currentSlice.TryGetIndex(out int index, out int bytesConsumed))
//                 {
//                     return false;
//                 }
//                 Tools[index].Patch.Set([.. "$"u8, .. currentSlice.Slice(bytesConsumed)], value);
//                 return true;
//             }
//             return false;
//         }
// #pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

//         public static implicit operator BinaryContent(CreateResponseOptions createResponseOptions)
//         {
//             if (createResponseOptions == null)
//             {
//                 return null;
//             }
//             return BinaryContent.Create(createResponseOptions, ModelSerializationExtensions.WireOptions);
//         }
//     }
// }
