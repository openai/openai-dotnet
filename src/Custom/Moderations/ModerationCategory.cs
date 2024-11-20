namespace OpenAI.Moderations;

/// <summary> A category of potentially harmful content as classified by the model. </summary>
public partial class ModerationCategory
{
    internal ModerationCategory(bool flagged, float score, ModerationApplicableInputKinds applicableInputKinds)
    {
        Flagged = flagged;
        Score = score;
        ApplicableInputKinds = applicableInputKinds;
    }

    /// <summary> Indicates whether this category has been flagged by the model. </summary>
    public bool Flagged { get; }

    /// <summary> The score predicted by the model for this category. </summary>
    public float Score { get; }

    /// <summary> The kinds of inputs that the score is applicable to. </summary>
    public ModerationApplicableInputKinds ApplicableInputKinds { get; }
}
