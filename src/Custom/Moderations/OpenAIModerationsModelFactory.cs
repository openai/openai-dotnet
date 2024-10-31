using System.Collections.Generic;
using System.Linq;

namespace OpenAI.Moderations;

/// <summary> Model factory for models. </summary>
public static partial class OpenAIModerationsModelFactory
{
    /// <summary> Initializes a new instance of <see cref="OpenAI.Moderations.ModerationCategory"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Moderations.ModerationCategory"/> instance for mocking. </returns>
    public static ModerationCategory ModerationCategory(bool flagged = default, float score = default, ModerationApplicableInputKinds applicableInputKinds = 0)
    {
        return new ModerationCategory(flagged, score, applicableInputKinds);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Moderations.ModerationResultCollection"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Moderations.ModerationResultCollection"/> instance for mocking. </returns>
    public static ModerationResultCollection ModerationResultCollection(string id = null, string model = null, IEnumerable<ModerationResult> items = null)
    {
        items ??= new List<ModerationResult>();

        return new ModerationResultCollection(
            id,
            model,
            items.ToList(),
            serializedAdditionalRawData: null);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Moderations.ModerationResult"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Moderations.ModerationResult"/> instance for mocking. </returns>
    public static ModerationResult ModerationResult(bool flagged = default, ModerationCategory hate = default, ModerationCategory hateThreatening = default, ModerationCategory harassment = default, ModerationCategory harassmentThreatening = default, ModerationCategory selfHarm = default, ModerationCategory selfHarmIntent = default, ModerationCategory selfHarmInstructions = default, ModerationCategory sexual = default, ModerationCategory sexualMinors = default, ModerationCategory violence = default, ModerationCategory violenceGraphic = default, ModerationCategory illicit = default, ModerationCategory illicitViolent = default)
    {
        return new ModerationResult(
            flagged,
            hate,
            hateThreatening,
            harassment,
            harassmentThreatening,
            illicit,
            illicitViolent,
            selfHarm,
            selfHarmIntent,
            selfHarmInstructions,
            sexual,
            sexualMinors,
            violence,
            violenceGraphic,
            serializedAdditionalRawData: null);
    }
}
