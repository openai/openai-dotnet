using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace OpenAI.Moderations
{
    [CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Moderations.ModerationResult>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
    public partial class ModerationResult : IJsonModel<ModerationResult>
    {
        // CUSTOM:
        // - Serializes `ModerationCategories` and `ModerationCategoryScores` properties.
        void IJsonModel<ModerationResult>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            var format = options.Format == "W" ? ((IPersistableModel<ModerationResult>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(ModerationResult)} does not support writing '{format}' format.");
            }

            writer.WriteStartObject();
            if (SerializedAdditionalRawData?.ContainsKey("flagged") != true)
            {
                writer.WritePropertyName("flagged"u8);
                writer.WriteBooleanValue(Flagged);
            }
            if (SerializedAdditionalRawData?.ContainsKey("categories") != true)
            {
                writer.WritePropertyName("categories"u8);
                InternalModerationCategories internalCategories = new(
                    hate: Hate.Flagged,
                    hateThreatening: HateThreatening.Flagged,
                    harassment: Harassment.Flagged,
                    harassmentThreatening: HarassmentThreatening.Flagged,
                    illicit: Illicit.Flagged,
                    illicitViolent: IllicitViolent.Flagged,
                    selfHarm: SelfHarm.Flagged,
                    selfHarmIntent: SelfHarmIntent.Flagged,
                    selfHarmInstructions: SelfHarmInstructions.Flagged,
                    sexual: Sexual.Flagged,
                    sexualMinors: SexualMinors.Flagged,
                    violence: Violence.Flagged,
                    violenceGraphic: ViolenceGraphic.Flagged,
                    serializedAdditionalRawData: null);
                writer.WriteObjectValue(internalCategories, options);
            }
            if (SerializedAdditionalRawData?.ContainsKey("category_scores") != true)
            {
                writer.WritePropertyName("category_scores"u8);
                InternalModerationCategoryScores internalCategoryScores = new(
                    hate: Hate.Score,
                    hateThreatening: HateThreatening.Score,
                    harassment: Harassment.Score,
                    harassmentThreatening: HarassmentThreatening.Score,
                    illicit: Illicit.Score,
                    illicitViolent: IllicitViolent.Score,
                    selfHarm: SelfHarm.Score,
                    selfHarmIntent: SelfHarmIntent.Score,
                    selfHarmInstructions: SelfHarmInstructions.Score,
                    sexual: Sexual.Score,
                    sexualMinors: SexualMinors.Score,
                    violence: Violence.Score,
                    violenceGraphic: ViolenceGraphic.Score,
                    serializedAdditionalRawData: null);
                writer.WriteObjectValue(internalCategoryScores, options);
            }
            if (SerializedAdditionalRawData?.ContainsKey("category_applied_input_types") != true)
            {
                writer.WritePropertyName("category_applied_input_types"u8);
                InternalCreateModerationResponseResultCategoryAppliedInputTypes internalAppliedInputTypes = new(
                    hate: Hate.ApplicableInputKinds.ToInternalApplicableInputKinds(),
                    hateThreatening: HateThreatening.ApplicableInputKinds.ToInternalApplicableInputKinds(),
                    harassment: Harassment.ApplicableInputKinds.ToInternalApplicableInputKinds(),
                    harassmentThreatening: HarassmentThreatening.ApplicableInputKinds.ToInternalApplicableInputKinds(),
                    illicit: Illicit.ApplicableInputKinds.ToInternalApplicableInputKinds(),
                    illicitViolent: IllicitViolent.ApplicableInputKinds.ToInternalApplicableInputKinds(),
                    selfHarm: SelfHarm.ApplicableInputKinds.ToInternalApplicableInputKinds(),
                    selfHarmIntent: SelfHarmIntent.ApplicableInputKinds.ToInternalApplicableInputKinds(),
                    selfHarmInstructions: SelfHarmInstructions.ApplicableInputKinds.ToInternalApplicableInputKinds(),
                    sexual: Sexual.ApplicableInputKinds.ToInternalApplicableInputKinds(),
                    sexualMinors: SexualMinors.ApplicableInputKinds.ToInternalApplicableInputKinds(),
                    violence: Violence.ApplicableInputKinds.ToInternalApplicableInputKinds(),
                    violenceGraphic: ViolenceGraphic.ApplicableInputKinds.ToInternalApplicableInputKinds(),
                    serializedAdditionalRawData: null);
                writer.WriteObjectValue(internalAppliedInputTypes, options);
            }
            if (SerializedAdditionalRawData != null)
            {
                foreach (var item in SerializedAdditionalRawData)
                {
                    if (ModelSerializationExtensions.IsSentinelValue(item.Value))
                    {
                        continue;
                    }
                    writer.WritePropertyName(item.Key);
#if NET6_0_OR_GREATER
				writer.WriteRawValue(item.Value);
#else
                    using (JsonDocument document = JsonDocument.Parse(item.Value))
                    {
                        JsonSerializer.Serialize(writer, document.RootElement);
                    }
#endif
                }
            }
            writer.WriteEndObject();
        }

        internal static ModerationResult DeserializeModerationResult(JsonElement element, ModelReaderWriterOptions options = null)
        {
            options ??= ModelSerializationExtensions.WireOptions;

            if (element.ValueKind == JsonValueKind.Null)
            {
                return null;
            }
            bool flagged = default;

            InternalModerationCategories internalCategories = default;
            InternalModerationCategoryScores internalCategoryScores = default;
            InternalCreateModerationResponseResultCategoryAppliedInputTypes internalAppliedInputTypes = default;
            IDictionary<string, BinaryData> serializedAdditionalRawData = default;
            Dictionary<string, BinaryData> rawDataDictionary = new Dictionary<string, BinaryData>();
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("flagged"u8))
                {
                    flagged = property.Value.GetBoolean();
                    continue;
                }
                if (property.NameEquals("categories"u8))
                {
                    internalCategories = InternalModerationCategories.DeserializeInternalModerationCategories(property.Value, options);
                    continue;
                }
                if (property.NameEquals("category_scores"u8))
                {
                    internalCategoryScores = InternalModerationCategoryScores.DeserializeInternalModerationCategoryScores(property.Value, options);
                    continue;
                }
                if (property.NameEquals("category_applied_input_types"u8))
                {
                    internalAppliedInputTypes = InternalCreateModerationResponseResultCategoryAppliedInputTypes.DeserializeInternalCreateModerationResponseResultCategoryAppliedInputTypes(property.Value, options);
                    continue;
                }
                if (true)
                {
                    rawDataDictionary ??= new Dictionary<string, BinaryData>();
                    rawDataDictionary.Add(property.Name, BinaryData.FromString(property.Value.GetRawText()));
                }
            }

            ModerationCategory MakeCategory<T>(
                Func<InternalModerationCategories, bool> categoryFlaggedGetter,
                Func<InternalModerationCategoryScores, float> scoreGetter,
                Func<
                    InternalCreateModerationResponseResultCategoryAppliedInputTypes,
                    IReadOnlyList<T>
                > internalAppliedInputTypesGetter)
            {
                IReadOnlyList<T> genericInputTypes
                    = internalAppliedInputTypes is null ? null
                        : internalAppliedInputTypesGetter.Invoke(internalAppliedInputTypes);
                IReadOnlyList<string> stringInputTypes =
                    (genericInputTypes as IReadOnlyList<string>)
                        ?? genericInputTypes?.Select(t => t.ToString()).ToList().AsReadOnly();
                return new ModerationCategory(
                    categoryFlaggedGetter.Invoke(internalCategories),
                    scoreGetter.Invoke(internalCategoryScores),
                    ModerationApplicableInputKindsExtensions.FromInternalApplicableInputKinds(stringInputTypes));
            }

            serializedAdditionalRawData = rawDataDictionary;
            return new ModerationResult(
               flagged: flagged,
               hate: MakeCategory(cats => cats.Hate, catScores => catScores.Hate, types => types.Hate),
               hateThreatening: MakeCategory(cats => cats.HateThreatening, catScores => catScores.HateThreatening, types => types.HateThreatening),
               harassment: MakeCategory(cats => cats.Harassment, catScores => catScores.Harassment, types => types.Harassment),
               harassmentThreatening: MakeCategory(cats => cats.HarassmentThreatening, catScores => catScores.HarassmentThreatening, types => types.HarassmentThreatening),
               illicit: MakeCategory(cats => cats.Illicit, catScores => catScores.Illicit, types => types.Illicit),
               illicitViolent: MakeCategory(cats => cats.IllicitViolent, catScores => catScores.IllicitViolent, types => types.IllicitViolent),
               selfHarm: MakeCategory(cats => cats.SelfHarm, catScores => catScores.SelfHarm, types => types.SelfHarm),
               selfHarmIntent: MakeCategory(cats => cats.SelfHarmIntent, catScores => catScores.SelfHarmIntent, types => types.SelfHarmIntent),
               selfHarmInstructions: MakeCategory(cats => cats.SelfHarmInstructions, catScores => catScores.SelfHarmInstructions, types => types.SelfHarmInstructions),
               sexual: MakeCategory(cats => cats.Sexual, catScores => catScores.Sexual, types => types.Sexual),
               sexualMinors: MakeCategory(cats => cats.SexualMinors, catScores => catScores.SexualMinors, types => types.SexualMinors),
               violence: MakeCategory(cats => cats.Violence, catScores => catScores.Violence, types => types.Violence),
               violenceGraphic: MakeCategory(cats => cats.ViolenceGraphic, catScores => catScores.ViolenceGraphic, types => types.ViolenceGraphic),
               serializedAdditionalRawData: serializedAdditionalRawData);
        }
    }
}
