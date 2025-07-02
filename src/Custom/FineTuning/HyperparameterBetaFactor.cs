using System;
using System.ClientModel.Primitives;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.FineTuning;

[Experimental("OPENAI001")]
[CodeGenType("CreateFineTuningJobRequestHyperparametersBetaChoiceEnum")]
internal readonly partial struct InternalCreateFineTuningJobRequestHyperparametersBetaChoiceEnum { }

[Experimental("OPENAI001")]
[CodeGenType("CreateFineTuningJobRequestHyperparametersBetaOption")]
public partial class HyperparameterBetaFactor : IEquatable<int>, IEquatable<string>, IJsonModel<HyperparameterBetaFactor>
{
    private readonly string _stringValue;
    private readonly int? _intValue;

    internal HyperparameterBetaFactor() { }
    internal HyperparameterBetaFactor(string predefinedLabel)
    {
        _stringValue = predefinedLabel;
    }

    public HyperparameterBetaFactor(int beta)
    {
        _intValue = beta;
    }

    public static HyperparameterBetaFactor CreateAuto() => new(InternalCreateFineTuningJobRequestHyperparametersBetaChoiceEnum.Auto.ToString());
    public static HyperparameterBetaFactor CreateBeta(int beta) => new(beta);

    public static implicit operator HyperparameterBetaFactor(int beta) => new(beta);
    
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool operator ==(HyperparameterBetaFactor first, HyperparameterBetaFactor second)
    {
        if (first is null && second is null) return true;
        if (first is null || second is null) return false;
        if (first._intValue.HasValue != second._intValue.HasValue) return false;
        if (first._intValue.HasValue) return first._intValue == second._intValue;
        return first._stringValue == second._stringValue;
    }
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool operator !=(HyperparameterBetaFactor first, HyperparameterBetaFactor second) => !(first == second);
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool Equals(int other) => _intValue == other;
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool Equals(string other) => _intValue is null && _stringValue == other;
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object other) => other is HyperparameterBetaFactor cc && cc == this;
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() => _intValue?.GetHashCode() ?? _stringValue.GetHashCode();

    void IJsonModel<HyperparameterBetaFactor>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        throw new NotImplementedException();
    }

    HyperparameterBetaFactor IJsonModel<HyperparameterBetaFactor>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
    {
        using JsonDocument document = JsonDocument.ParseValue(ref reader);
        return DeserializeHyperparameterBeta(document.RootElement, options);
    }

    internal static void SerializeHyperparameterBeta(HyperparameterBetaFactor instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
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

    BinaryData IPersistableModel<HyperparameterBetaFactor>.Write(ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, options);

    HyperparameterBetaFactor IPersistableModel<HyperparameterBetaFactor>.Create(BinaryData data, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.DeserializeNewInstance(this, DeserializeHyperparameterBeta, data, options);

    internal static HyperparameterBetaFactor DeserializeHyperparameterBeta(JsonElement element, ModelReaderWriterOptions options = null)
    {
        options ??= ModelSerializationExtensions.WireOptions;

        return element.ValueKind switch
        {
            JsonValueKind.Number => new(element.GetInt32()),
            JsonValueKind.String => new(element.GetString()),
            _ => throw new ArgumentException($"Unsupported JsonValueKind", "beta")
        };
    }

    string IPersistableModel<HyperparameterBetaFactor>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";
}