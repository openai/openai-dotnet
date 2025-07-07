using System;
using System.ClientModel.Primitives;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.FineTuning;

[Experimental("OPENAI001")]
[CodeGenType("CreateFineTuningJobRequestHyperparametersBatchSizeChoiceEnum")]
internal readonly partial struct InternalCreateFineTuningJobRequestHyperparametersBatchSizeChoiceEnum { }

[Experimental("OPENAI001")]
[CodeGenType("CreateFineTuningJobRequestHyperparametersBatchSizeOption")]
public partial class HyperparameterBatchSize : IEquatable<int>, IEquatable<string>, IJsonModel<HyperparameterBatchSize>
{
    private readonly string _stringValue;
    private readonly int? _intValue;

    internal HyperparameterBatchSize() { }

    internal HyperparameterBatchSize(string predefinedLabel)
    {
        _stringValue = predefinedLabel;
    }

    public HyperparameterBatchSize(int batchSize)
    {
        _intValue = batchSize;
    }

    public static HyperparameterBatchSize CreateAuto() => new(InternalCreateFineTuningJobRequestHyperparametersBatchSizeChoiceEnum.Auto.ToString());
    public static HyperparameterBatchSize CreateSize(int batchSize) => new(batchSize);

    public static implicit operator HyperparameterBatchSize(int batchSize) => new(batchSize);
    
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool operator ==(HyperparameterBatchSize first, HyperparameterBatchSize second)
    {
        if (first is null && second is null) return true;
        if (first is null || second is null) return false;
        if (first._intValue.HasValue != second._intValue.HasValue) return false;
        if (first._intValue.HasValue) return first._intValue == second._intValue;
        return first._stringValue == second._stringValue;
    }
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool operator !=(HyperparameterBatchSize first, HyperparameterBatchSize second) => !(first == second);
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool Equals(int other) => _intValue == other;
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool Equals(string other) => _intValue is null && _stringValue == other;
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object other) => other is HyperparameterBatchSize cc && cc == this;
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() => _intValue?.GetHashCode() ?? _stringValue.GetHashCode();

    void IJsonModel<HyperparameterBatchSize>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        SerializeHyperparameterBatchSize(this, writer, options);
    }

    HyperparameterBatchSize IJsonModel<HyperparameterBatchSize>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
    {
        using JsonDocument document = JsonDocument.ParseValue(ref reader);
        return DeserializeHyperparameterBatchSize(document.RootElement, options);
    }

    internal static void SerializeHyperparameterBatchSize(HyperparameterBatchSize instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        if (instance._intValue is not null)
        {
            writer.WriteNumberValue(instance._intValue.Value);
        }
        else
        {
            writer.WriteStringValue(instance._stringValue);
        }
    }

    BinaryData IPersistableModel<HyperparameterBatchSize>.Write(ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, options);

    HyperparameterBatchSize IPersistableModel<HyperparameterBatchSize>.Create(BinaryData data, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.DeserializeNewInstance(this, DeserializeHyperparameterBatchSize, data, options);

    internal static HyperparameterBatchSize DeserializeHyperparameterBatchSize(JsonElement element, ModelReaderWriterOptions options = null)
    {
        options ??= ModelSerializationExtensions.WireOptions;

        return element.ValueKind switch
        {
            JsonValueKind.Number => new(element.GetInt32()),
            JsonValueKind.String => new(element.GetString()),
            _ => throw new ArgumentException($"Unsupported JsonValueKind", nameof(HyperparameterBatchSize))
        };
    }

    string IPersistableModel<HyperparameterBatchSize>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";
}