using System.Collections.Generic;
using System.Linq;

namespace OpenAI.Moderations;

/// <summary> Model factory for models. </summary>
public static partial class OpenAIModerationsModelFactory
{
    /// <summary> Initializes a new instance of <see cref="OpenAI.Moderations.ModerationCategories"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Moderations.ModerationCategories"/> instance for mocking. </returns>
    public static ModerationCategories ModerationCategories(bool hate = default, bool hateThreatening = default, bool harassment = default, bool harassmentThreatening = default, bool selfHarm = default, bool selfHarmIntent = default, bool selfHarmInstructions = default, bool sexual = default, bool sexualMinors = default, bool violence = default, bool violenceGraphic = default)
    {
        return new ModerationCategories(
            hate,
            hateThreatening,
            harassment,
            harassmentThreatening,
            selfHarm,
            selfHarmIntent,
            selfHarmInstructions,
            sexual,
            sexualMinors,
            violence,
            violenceGraphic,
            serializedAdditionalRawData: null);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Moderations.ModerationCategoryScores"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Moderations.ModerationCategoryScores"/> instance for mocking. </returns>
    public static ModerationCategoryScores ModerationCategoryScores(float hate = default, float hateThreatening = default, float harassment = default, float harassmentThreatening = default, float selfHarm = default, float selfHarmIntent = default, float selfHarmInstructions = default, float sexual = default, float sexualMinors = default, float violence = default, float violenceGraphic = default)
    {
        return new ModerationCategoryScores(
            hate,
            hateThreatening,
            harassment,
            harassmentThreatening,
            selfHarm,
            selfHarmIntent,
            selfHarmInstructions,
            sexual,
            sexualMinors,
            violence,
            violenceGraphic,
            serializedAdditionalRawData: null);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Moderations.ModerationCollection"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Moderations.ModerationCollection"/> instance for mocking. </returns>
    public static ModerationCollection ModerationCollection(string id = null, string model = null, IEnumerable<ModerationResult> items = null)
    {
        items ??= new List<ModerationResult>();

        return new ModerationCollection(
            id,
            model,
            items.ToList(),
            serializedAdditionalRawData: null);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Moderations.ModerationResult"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Moderations.ModerationResult"/> instance for mocking. </returns>
    public static ModerationResult ModerationResult(bool flagged = default, ModerationCategories categories = null, ModerationCategoryScores categoryScores = null)
    {
        return new ModerationResult(
            flagged,
            categories,
            categoryScores,
            serializedAdditionalRawData: null);
    }
}
