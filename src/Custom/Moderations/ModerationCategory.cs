namespace OpenAI.Moderations;

public partial class ModerationCategory
{
    internal ModerationCategory(bool flagged, float score)
    {
        Flagged = flagged;
        Score = score;
    }

    public bool Flagged { get; }
    public float Score { get; }
}
