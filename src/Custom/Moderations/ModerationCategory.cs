using System.Collections.Generic;

namespace OpenAI.Moderations;

public partial class ModerationCategory
{
    internal ModerationCategory(bool flagged, float score, ModerationApplicableInputKinds applicableInputKinds)
    {
        Flagged = flagged;
        Score = score;
        ApplicableInputKinds = applicableInputKinds;
    }

    public bool Flagged { get; }
    public float Score { get; }
    public ModerationApplicableInputKinds ApplicableInputKinds { get; }
}
