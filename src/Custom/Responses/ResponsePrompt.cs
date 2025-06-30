using System;
using System.Collections.Generic;

namespace OpenAI.Responses;

[CodeGenType("ResponsesPrompt")]
public partial class ResponsePrompt
{
	private protected IDictionary<string, BinaryData> _additionalBinaryDataProperties;

	public ResponsePrompt()
	{
		Variables = new ChangeTrackingDictionary<string, BinaryData>();
	}

	internal ResponsePrompt(string id, string version, IDictionary<string, BinaryData> variables, IDictionary<string, BinaryData> additionalBinaryDataProperties)
	{
		Id = id;
		Version = version;
		Variables = variables;
		_additionalBinaryDataProperties = additionalBinaryDataProperties;
	}

	public string Id { get; set; }

	public string Version { get; set; }

	public IDictionary<string, BinaryData> Variables { get; }

	internal IDictionary<string, BinaryData> SerializedAdditionalRawData {
		get => _additionalBinaryDataProperties;
		set => _additionalBinaryDataProperties = value;
	}
}
