#nullable disable

using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace OpenAI.Responses
{
	public partial class ResponsePrompt : IJsonModel<ResponsePrompt>
    {
		void IJsonModel<ResponsePrompt>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
		{
			writer.WriteStartObject();
			JsonModelWriteCore(writer, options);
			writer.WriteEndObject();
		}

		protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
		{
			writer.WriteStartObject();

			if (Id != null)
			{
				writer.WritePropertyName("id");
				writer.WriteStringValue(Id);
			}

			if (Version != null)
			{
				writer.WritePropertyName("version");
				writer.WriteStringValue(Version);
			}

			if (Variables != null && Variables.Count > 0)
			{
				writer.WritePropertyName("variables");
				writer.WriteStartObject();
				foreach (var item in Variables)
				{
					writer.WritePropertyName(item.Key);
					if (item.Value == null)
					{
						writer.WriteNullValue();
						continue;
					}
#if NET6_0_OR_GREATER
					writer.WriteRawValue(item.Value);
#else
                    using (JsonDocument document = JsonDocument.Parse(item.Value))
                    {
                        JsonSerializer.Serialize(writer, document.RootElement);
                    }
#endif
				}
				writer.WriteEndObject();
			}

			writer.WriteEndObject();
		}

		ResponsePrompt IJsonModel<ResponsePrompt>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options) => JsonModelCreateCore(ref reader, options);

		protected virtual ResponsePrompt JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
		{
			string format = options.Format == "W" ? ((IPersistableModel<ResponsePrompt>)this).GetFormatFromOptions(options) : options.Format;
			if (format != "J")
			{
				throw new FormatException($"The model {nameof(ResponsePrompt)} does not support reading '{format}' format.");
			}
			using JsonDocument document = JsonDocument.ParseValue(ref reader);
			return DeserializeResponsePrompt(document.RootElement, options);
		}


		internal static ResponsePrompt DeserializeResponsePrompt(JsonElement element, ModelReaderWriterOptions options)
		{
			if (element.ValueKind == JsonValueKind.Null)
            {
                return null;
            }
            string id = null;
            string version = null;
			IDictionary<string, BinaryData> variables = new ChangeTrackingDictionary<string, BinaryData>();
			IDictionary<string, BinaryData> additionalBinaryDataProperties = new ChangeTrackingDictionary<string, BinaryData>();

			foreach (var prop in element.EnumerateObject())
            {
                if (prop.NameEquals("id"))
                {
                    id = prop.Value.GetString();
					continue;
				}
				if (prop.NameEquals("version"))
                {
                    version = prop.Value.GetString();
					continue;
				}
				if (prop.NameEquals("variables"))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    Dictionary<string, BinaryData> dictionary = new Dictionary<string, BinaryData>();
                    foreach (var prop0 in prop.Value.EnumerateObject())
                    {
                        if (prop0.Value.ValueKind == JsonValueKind.Null)
                        {
                            dictionary.Add(prop0.Name, null);
                        }
                        else
                        {
                            dictionary.Add(prop0.Name, BinaryData.FromString(prop0.Value.GetRawText()));
                        }
                    }
                    variables = dictionary;
                    continue;
                }
				additionalBinaryDataProperties.Add(prop.Name, BinaryData.FromString(prop.Value.GetRawText()));
			}

			return new ResponsePrompt(
				id,
				version,
				variables,
				additionalBinaryDataProperties);
        }
		BinaryData IPersistableModel<ResponsePrompt>.Write(ModelReaderWriterOptions options) => PersistableModelWriteCore(options);

		protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options)
		{
			string format = options.Format == "W" ? ((IPersistableModel<ResponsePrompt>)this).GetFormatFromOptions(options) : options.Format;
			switch (format)
			{
				case "J":
					return ModelReaderWriter.Write(this, options);
				default:
					throw new FormatException($"The model {nameof(ResponsePrompt)} does not support writing '{options.Format}' format.");
			}
		}

		ResponsePrompt IPersistableModel<ResponsePrompt>.Create(BinaryData data, ModelReaderWriterOptions options) => PersistableModelCreateCore(data, options);

		protected virtual ResponsePrompt PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options)
		{
			string format = options.Format == "W" ? ((IPersistableModel<ResponsePrompt>)this).GetFormatFromOptions(options) : options.Format;
			switch (format)
			{
				case "J":
					using (JsonDocument document = JsonDocument.Parse(data))
					{
						return DeserializeResponsePrompt(document.RootElement, options);
					}
				default:
					throw new FormatException($"The model {nameof(ResponsePrompt)} does not support reading '{options.Format}' format.");
			}
		}

		string IPersistableModel<ResponsePrompt>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";

		public static implicit operator BinaryContent(ResponsePrompt responsePrompt)
		{
			if (responsePrompt == null)
			{
				return null;
			}
			return BinaryContent.Create(responsePrompt, ModelSerializationExtensions.WireOptions);
		}

		public static explicit operator ResponsePrompt(ClientResult result)
		{
			using PipelineResponse response = result.GetRawResponse();
			using JsonDocument document = JsonDocument.Parse(response.Content);
			return DeserializeResponsePrompt(document.RootElement, ModelSerializationExtensions.WireOptions);
		}
	}
}
