using System;

namespace OpenAI.Moderations;

/// <summary> The kinds of inputs that <see cref="ModerationCategory.Score"/> is applicable to. </summary>
[Flags]
[CodeGenModel("ModerationAppliedInputType")]
public enum ModerationApplicableInputKinds : int
{
    None = 0,

    Other = 1 << 0,

    [CodeGenMember("Text")]
    Text = 1 << 1,

    [CodeGenMember("Image")]
    Image = 1 << 2,
}