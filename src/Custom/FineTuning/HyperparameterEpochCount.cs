using System;
using System.ClientModel.Primitives;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.FineTuning;

[Experimental("OPENAI001")]
[CodeGenType("CreateFineTuningJobRequestHyperparametersNEpochsChoiceEnum")]
internal readonly partial struct InternalCreateFineTuningJobRequestHyperparametersNEpochsChoiceEnum { }

[Experimental("OPENAI001")]
[CodeGenType("CreateFineTuningJobRequestHyperparametersNEpochsOption")]
public partial class HyperparameterEpochCount : IEquatable<int>, IEquatable<string>, IJsonModel<HyperparameterEpochCount>
{
    private readonly string _stringValue;
    private readonly int? _intValue;

    internal HyperparameterEpochCount(string predefinedLabel)
    {
        _stringValue = predefinedLabel;
    }

    internal HyperparameterEpochCount() { }

    public HyperparameterEpochCount(int epochCount)
    {
        _intValue = epochCount;
    }

    public static HyperparameterEpochCount CreateAuto() => new(InternalCreateFineTuningJobRequestHyperparametersNEpochsChoiceEnum.Auto.ToString());
    public static HyperparameterEpochCount CreateEpochCount(int epochCount) => new(epochCount);

    public static implicit operator HyperparameterEpochCount(int epochCount) => new(epochCount);
    
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool operator ==(HyperparameterEpochCount first, HyperparameterEpochCount second)
    {
        if (first is null && second is null) return true;
        if (first is null || second is null) return false;
        if (first._intValue.HasValue != second._intValue.HasValue) return false;
        if (first._intValue.HasValue) return first._intValue == second._intValue;
        return first._stringValue == second._stringValue;
    }
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool operator !=(HyperparameterEpochCount first, HyperparameterEpochCount second) => !(first == second);
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool Equals(int other) => _intValue == other;
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool Equals(string other) => _intValue is null && _stringValue == other;
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object other) => other is HyperparameterEpochCount cc && cc == this;
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() => _intValue?.GetHashCode() ?? _stringValue.GetHashCode();

    void IJsonModel<HyperparameterEpochCount>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        SerializeHyperparameterEpochCount(this, writer, options);
    }

    HyperparameterEpochCount IJsonModel<HyperparameterEpochCount>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
    {
        using JsonDocument document = JsonDocument.ParseValue(ref reader);
        return DeserializeHyperparameterEpochCount(document.RootElement, options);
    }

    internal static void SerializeHyperparameterEpochCount(HyperparameterEpochCount instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
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

    BinaryData IPersistableModel<HyperparameterEpochCount>.Write(ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, options);

    HyperparameterEpochCount IPersistableModel<HyperparameterEpochCount>.Create(BinaryData data, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.DeserializeNewInstance(this, DeserializeHyperparameterEpochCount, data, options);

    internal static HyperparameterEpochCount DeserializeHyperparameterEpochCount(JsonElement element, ModelReaderWriterOptions options = null)
    {
        options ??= ModelSerializationExtensions.WireOptions;

        return element.ValueKind switch
        {
            JsonValueKind.Number => new(element.GetInt32()),
            JsonValueKind.String => new(element.GetString()),
            _ => throw new ArgumentException($"Unsupported JsonValueKind", nameof(HyperparameterEpochCount))
        };
    }

    string IPersistableModel<HyperparameterEpochCount>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";
}