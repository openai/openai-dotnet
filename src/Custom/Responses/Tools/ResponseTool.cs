using OpenAI.Assistants;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[Experimental("OPENAI001")]
[CodeGenType("Tool")]
public partial class ResponseTool
{
    public static ResponseTool CreateFunctionTool(string functionName, string functionDescription, BinaryData functionParameters, bool functionSchemaIsStrict = false)
    {
        return new InternalFunctionTool(functionName, functionSchemaIsStrict, functionParameters)
        {
            Description = functionDescription
        };
    }

    [Experimental("OPENAICUA001")]
    public static ResponseTool CreateComputerTool(ComputerToolEnvironment environment, int displayWidth,int displayHeight)
    {
        return new InternalComputerUsePreviewTool(environment, displayWidth, displayHeight);
    }

    public static ResponseTool CreateFileSearchTool(IEnumerable<string> vectorStoreIds, int? maxResultCount = null, FileSearchToolRankingOptions rankingOptions = null, BinaryData filters = null)
    {
        return new InternalFileSearchTool(vectorStoreIds)
        {
            MaxNumResults = maxResultCount,
            RankingOptions = rankingOptions,
            Filters = filters,
        };
    }

    public static ResponseTool CreateWebSearchTool(WebSearchUserLocation userLocation = null, WebSearchContextSize? searchContextSize = null)
    {
        return new InternalWebSearchTool()
        {
            UserLocation = userLocation,
            SearchContextSize = searchContextSize,
        };
    }


	/*****************************************************************************
	 * <GP> CUSTOM: Added code interpreter tool.                                 */
	class InternalCodeInterpreterTool : ResponseTool, IJsonModel<InternalCodeInterpreterTool>
	{
		public InternalCodeInterpreterTool(InternalResponsesToolType @type, IDictionary<string, BinaryData> additionalBinaryDataProperties, CodeInterpreterToolDefinition definition) :
			base(@type, additionalBinaryDataProperties)
		{
			this.additionalBinaryDataProperties = additionalBinaryDataProperties;
			this.Definition = definition;
		}

		public CodeInterpreterToolDefinition Definition { get; set; }
		public IDictionary<string, BinaryData> additionalBinaryDataProperties = new ChangeTrackingDictionary<string, BinaryData>();

		void IJsonModel<InternalCodeInterpreterTool>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
		{
			writer.WriteStartObject();
			JsonModelWriteCore(writer, options);
			writer.WriteEndObject();
		}

		protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
		{
			string format = options.Format == "W" ? ((IPersistableModel<InternalCodeInterpreterTool>)this).GetFormatFromOptions(options) : options.Format;
			if (format != "J")
			{
				throw new FormatException($"The model {nameof(InternalCodeInterpreterTool)} does not support writing '{format}' format.");
			}
			base.JsonModelWriteCore(writer, options);
			if (Optional.IsDefined(Definition) && _additionalBinaryDataProperties?.ContainsKey("container") != true)
			{
				writer.WritePropertyName("container"u8);
				writer.WriteObjectValue(Definition, options);
			}
		}

		InternalCodeInterpreterTool IJsonModel<InternalCodeInterpreterTool>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options) => (InternalCodeInterpreterTool)JsonModelCreateCore(ref reader, options);

		protected override ResponseTool JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
		{
			string format = options.Format == "W" ? ((IPersistableModel<InternalCodeInterpreterTool>)this).GetFormatFromOptions(options) : options.Format;
			if (format != "J")
			{
				throw new FormatException($"The model {nameof(InternalCodeInterpreterTool)} does not support reading '{format}' format.");
			}
			using JsonDocument document = JsonDocument.ParseValue(ref reader);
			return DeserializeInternalCodeInterpreterTool(document.RootElement, options);
		}

		internal static InternalCodeInterpreterTool DeserializeInternalCodeInterpreterTool(JsonElement element, ModelReaderWriterOptions options)
		{
			if (element.ValueKind == JsonValueKind.Null)
			{
				return null;
			}
			InternalResponsesToolType @type = default;
			IDictionary<string, BinaryData> additionalBinaryDataProperties = new ChangeTrackingDictionary<string, BinaryData>();
			CodeInterpreterToolDefinition definition = default;
			foreach (var prop in element.EnumerateObject())
			{
				if (prop.NameEquals("type"u8))
				{
					@type = new InternalResponsesToolType(prop.Value.GetString());
					continue;
				}
				if (prop.NameEquals("container"u8))
				{
					if (prop.Value.ValueKind == JsonValueKind.Null)
					{
						continue;
					}
					definition = CodeInterpreterToolDefinition.DeserializeCodeInterpreterToolDefinition(prop.Value, options);
					continue;
				}
				additionalBinaryDataProperties.Add(prop.Name, BinaryData.FromString(prop.Value.GetRawText()));
			}
			return new InternalCodeInterpreterTool(@type, additionalBinaryDataProperties, definition);
		}

		BinaryData IPersistableModel<InternalCodeInterpreterTool>.Write(ModelReaderWriterOptions options) => PersistableModelWriteCore(options);

		protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options)
		{
			string format = options.Format == "W" ? ((IPersistableModel<InternalCodeInterpreterTool>)this).GetFormatFromOptions(options) : options.Format;
			switch (format)
			{
				case "J":
					return ModelReaderWriter.Write(this, options);
				default:
					throw new FormatException($"The model {nameof(InternalCodeInterpreterTool)} does not support writing '{options.Format}' format.");
			}
		}

		InternalCodeInterpreterTool IPersistableModel<InternalCodeInterpreterTool>.Create(BinaryData data, ModelReaderWriterOptions options) => (InternalCodeInterpreterTool)PersistableModelCreateCore(data, options);

		protected override ResponseTool PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options)
		{
			string format = options.Format == "W" ? ((IPersistableModel<InternalCodeInterpreterTool>)this).GetFormatFromOptions(options) : options.Format;
			switch (format)
			{
				case "J":
					using (JsonDocument document = JsonDocument.Parse(data))
					{
						return DeserializeInternalCodeInterpreterTool(document.RootElement, options);
					}
				default:
					throw new FormatException($"The model {nameof(InternalCodeInterpreterTool)} does not support reading '{options.Format}' format.");
			}
		}

		string IPersistableModel<InternalCodeInterpreterTool>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";

		public static implicit operator BinaryContent(InternalCodeInterpreterTool InternalCodeInterpreterTool)
		{
			if (InternalCodeInterpreterTool == null)
			{
				return null;
			}
			return BinaryContent.Create(InternalCodeInterpreterTool, ModelSerializationExtensions.WireOptions);
		}

		public static explicit operator InternalCodeInterpreterTool(ClientResult result)
		{
			using PipelineResponse response = result.GetRawResponse();
			using JsonDocument document = JsonDocument.Parse(response.Content);
			return DeserializeInternalCodeInterpreterTool(document.RootElement, ModelSerializationExtensions.WireOptions);
		}
	}

    public static ResponseTool CreateCodeInterpreterTool(string type = "auto", IDictionary<string, BinaryData> additionalBinaryDataProperties = null)
	{
		return new InternalCodeInterpreterTool(
			InternalResponsesToolType.CodeInterpreter,
			additionalBinaryDataProperties,
			new CodeInterpreterToolDefinition(type, null)
			);
	}

	/* </GP> CUSTOM: Added code interpreter tool.                               *
	 ****************************************************************************/
}
