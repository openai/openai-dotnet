using OpenAI.Assistants;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;

namespace OpenAI.Responses;

[CodeGenType("ResponsesTool")]
public partial class ResponseTool
{
    // CUSTOM: Made internal.
    [CodeGenMember("Type")]
    internal InternalResponsesToolType Type { get; }

    //// CUSTOM: Exposed function tool properties.
    //public string FunctionName => (this as InternalResponsesFunctionTool)?.Name;
    //public string FunctionDescription => (this as InternalResponsesFunctionTool)?.Description;
    //public BinaryData FunctionParameters => (this as InternalResponsesFunctionTool)?.Parameters;
    //public bool? FunctionSchemaIsStrict => (this as InternalResponsesFunctionTool)?.Strict;

    //// CUSTOM: Exposed computer tool properties.
    //public int? ComputerDisplayWidth => (this as InternalResponsesComputerTool)?.DisplayWidth;
    //public int? ComputerDisplayHeight => (this as InternalResponsesComputerTool)?.DisplayHeight;
    //public ComputerToolEnvironment? ComputerEnvironment => (this as InternalResponsesComputerTool)?.Environment;

    //// CUSTOM: Exposed file search tool properties.
    //public IList<string> FileSearchVectorStoreIds => (this as InternalResponsesFileSearchTool)?.VectorStoreIds;
    //public int? FileSearchMaxResultCount => (this as InternalResponsesFileSearchTool)?.MaxNumResults;

    //// CUSTOM: Exposed web search tool properties.
    //public IList<string> WebSearchDomains => (this as InternalResponsesWebSearchTool)?.Domains;
    //public WebSearchToolUserLocation WebSearchUserLocation => (this as InternalResponsesWebSearchTool)?.UserLocation;

    //// CUSTOM: Exposed code interpreter tool properties.
    //// TODO

    public static ResponseTool CreateFunctionTool(string functionName, string functionDescription, BinaryData functionParameters, bool functionSchemaIsStrict = false)
    {
        return new InternalResponsesFunctionTool(
            type: InternalResponsesToolType.Function,
            additionalBinaryDataProperties: null,
            functionName,
            functionDescription,
            functionParameters,
            functionSchemaIsStrict);
    }

    [Experimental("OPENAICUA001")]
    public static ResponseTool CreateComputerTool(int displayWidth,int displayHeight, ComputerToolEnvironment environment)
    {
        return new InternalResponsesComputerTool(
            type: InternalResponsesToolType.Computer,
            additionalBinaryDataProperties: null,
            displayWidth,
            displayHeight,
            environment);
    }

    public static ResponseTool CreateFileSearchTool(IEnumerable<string> vectorStoreIds, int? maxResultCount = null, FileSearchToolRankingOptions rankingOptions = null, BinaryData filters = null)
    {
        return new InternalResponsesFileSearchTool(
            type: InternalResponsesToolType.FileSearch,
            additionalBinaryDataProperties: null,
            vectorStoreIds.ToList(),
            maxResultCount,
            rankingOptions,
            filters);
    }

    public static ResponseTool CreateWebSearchTool(WebSearchToolLocation webSearchToolUserLocation = null, WebSearchToolContextSize? webSearchToolContextSize = null)
    {
        return new InternalResponsesWebSearchTool(
            type: InternalResponsesToolType.WebSearch,
            additionalBinaryDataProperties: null,
            webSearchToolUserLocation,
            webSearchToolContextSize);
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
