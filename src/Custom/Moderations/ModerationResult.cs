using System.Collections.Generic;
using System;

namespace OpenAI.Moderations;

// CUSTOM: 
// - Removes the redundant classes `ModerationCategories` and `ModerationCategoryScores` and moves their properties into this type.
[CodeGenModel("CreateModerationResponseResult")]
[CodeGenSuppress("Categories")]
[CodeGenSuppress("CategoryScores")]
[CodeGenSuppress("CategoryAppliedInputTypes")]
[CodeGenSuppress(nameof(ModerationResult), typeof(bool), typeof(InternalModerationCategories), typeof(InternalModerationCategoryScores), typeof(InternalCreateModerationResponseResultCategoryAppliedInputTypes))]
[CodeGenSuppress(nameof(ModerationResult), typeof(bool), typeof(InternalModerationCategories), typeof(InternalModerationCategoryScores), typeof(InternalCreateModerationResponseResultCategoryAppliedInputTypes), typeof(IDictionary<string, BinaryData>))]
public partial class ModerationResult
{
    internal IDictionary<string, BinaryData> SerializedAdditionalRawData { get; set; }
    
    internal ModerationResult(bool flagged, ModerationCategory hate, ModerationCategory hateThreatening, ModerationCategory harassment, ModerationCategory harassmentThreatening, ModerationCategory illicit, ModerationCategory illicitViolent, ModerationCategory selfHarm, ModerationCategory selfHarmIntent, ModerationCategory selfHarmInstructions, ModerationCategory sexual, ModerationCategory sexualMinors, ModerationCategory violence, ModerationCategory violenceGraphic)
    {
        Flagged = flagged;
        Hate = hate;
        HateThreatening = hateThreatening;
        Harassment = harassment;
        HarassmentThreatening = harassmentThreatening;
        Illicit = illicit;
        IllicitViolent = illicitViolent;
        SelfHarm = selfHarm;
        SelfHarmIntent = selfHarmIntent;
        SelfHarmInstructions = selfHarmInstructions;
        Sexual = sexual;
        SexualMinors = sexualMinors;
        Violence = violence;
        ViolenceGraphic = violenceGraphic;
    }

    internal ModerationResult(bool flagged, ModerationCategory hate, ModerationCategory hateThreatening, ModerationCategory harassment, ModerationCategory harassmentThreatening, ModerationCategory illicit, ModerationCategory illicitViolent, ModerationCategory selfHarm, ModerationCategory selfHarmIntent, ModerationCategory selfHarmInstructions, ModerationCategory sexual, ModerationCategory sexualMinors, ModerationCategory violence, ModerationCategory violenceGraphic, IDictionary<string, BinaryData> serializedAdditionalRawData)
    {
        Flagged = flagged;
        Hate = hate;
        HateThreatening = hateThreatening;
        Harassment = harassment;
        HarassmentThreatening = harassmentThreatening;
        Illicit = illicit;
        IllicitViolent = illicitViolent;
        SelfHarm = selfHarm;
        SelfHarmIntent = selfHarmIntent;
        SelfHarmInstructions = selfHarmInstructions;
        Sexual = sexual;
        SexualMinors = sexualMinors;
        Violence = violence;
        ViolenceGraphic = violenceGraphic;
        SerializedAdditionalRawData = serializedAdditionalRawData;
    }

    internal ModerationResult()
    {
    }

    public ModerationCategory Hate { get; }

    public ModerationCategory HateThreatening { get; }

    public ModerationCategory Harassment { get; }

    public ModerationCategory HarassmentThreatening { get; }

    public ModerationCategory Illicit { get; }

    public ModerationCategory IllicitViolent { get; }

    public ModerationCategory SelfHarm { get; }

    public ModerationCategory SelfHarmIntent { get; }

    public ModerationCategory SelfHarmInstructions { get; }

    public ModerationCategory Sexual { get; }

    public ModerationCategory SexualMinors { get; }

    public ModerationCategory Violence { get; }

    public ModerationCategory ViolenceGraphic { get; }
}
