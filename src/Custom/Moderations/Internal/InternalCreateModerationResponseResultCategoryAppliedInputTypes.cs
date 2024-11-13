using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenAI.Moderations;

[CodeGenModel("CreateModerationResponseResultCategoryAppliedInputTypes")]
internal partial class InternalCreateModerationResponseResultCategoryAppliedInputTypes
{
    // CUSTOM: normalize to List<string> for internal representation
    public IReadOnlyList<string> Hate { get; }
    public IReadOnlyList<string> HateThreatening { get; }
    public IReadOnlyList<string> Harassment { get; }
    public IReadOnlyList<string> HarassmentThreatening { get; }
    public IReadOnlyList<string> Illicit { get; }
    public IReadOnlyList<string> IllicitViolent { get; }
    public IReadOnlyList<string> SelfHarm { get; }
    public IReadOnlyList<string> SelfHarmIntent { get; }
    public IReadOnlyList<string> SelfHarmInstructions { get; }
    public IReadOnlyList<string> Sexual { get; }
    public IReadOnlyList<string> SexualMinors { get; }
    public IReadOnlyList<string> Violence { get; }
    public IReadOnlyList<string> ViolenceGraphic { get; }
}
