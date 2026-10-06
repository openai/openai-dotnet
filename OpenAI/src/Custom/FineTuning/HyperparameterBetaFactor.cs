using Microsoft.TypeSpec.Generator.Customizations;
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
public partial class HyperparameterBetaFactor : IEquatable<double>, IEquatable<int>, IEquatable<string>, IJsonModel<HyperparameterBetaFactor>
{
    private readonly string _stringValue;
    private readonly double? _doubleValue;

    internal HyperparameterBetaFactor() { }
    internal HyperparameterBetaFactor(string predefinedLabel)
    {
        _stringValue = predefinedLabel;
    }

    public HyperparameterBetaFactor(int beta) : this((double)beta)
    {
    }

    public HyperparameterBetaFactor(double beta)
    {
        _doubleValue = beta;
    }

    public static HyperparameterBetaFactor CreateAuto() => new(InternalCreateFineTuningJobRequestHyperparametersBetaChoiceEnum.Auto.ToString());
    public static HyperparameterBetaFactor CreateBeta(int beta) => new(beta);
    public static HyperparameterBetaFactor CreateBeta(double beta) => new(beta);

    public static implicit operator HyperparameterBetaFactor(int beta) => new(beta);
    public static implicit operator HyperparameterBetaFactor(double beta) => new(beta);
    
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool operator ==(HyperparameterBetaFactor first, HyperparameterBetaFactor second)
    {
        if (first is null && second is null) return true;
        if (first is null || second is null) return false;
        if (first._doubleValue.HasValue != second._doubleValue.HasValue) return false;
        if (first._doubleValue.HasValue) return first._doubleValue == second._doubleValue;
        return first._stringValue == second._stringValue;
    }
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool operator !=(HyperparameterBetaFactor first, HyperparameterBetaFactor second) => !(first == second);
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool Equals(int other) => _doubleValue == other;
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool Equals(double other) => _doubleValue == other;
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool Equals(string other) => _doubleValue is null && _stringValue == other;
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object other) => other is HyperparameterBetaFactor cc && cc == this;
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() => _doubleValue?.GetHashCode() ?? _stringValue.GetHashCode();

    void IJsonModel<HyperparameterBetaFactor>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        SerializeHyperparameterBeta(this, writer, options);
    }

    HyperparameterBetaFactor IJsonModel<HyperparameterBetaFactor>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
    {
        using JsonDocument document = JsonDocument.ParseValue(ref reader);
        return DeserializeHyperparameterBeta(document.RootElement, options);
    }

    internal static void SerializeHyperparameterBeta(HyperparameterBetaFactor instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        if (instance._doubleValue is not null)
        {
            writer.WriteNumberValue(instance._doubleValue.Value);
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
            JsonValueKind.Number => new(element.GetDouble()),
            JsonValueKind.String => new(element.GetString()),
            _ => throw new ArgumentException($"Unsupported JsonValueKind", "beta")
        };
    }

    string IPersistableModel<HyperparameterBetaFactor>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";
}