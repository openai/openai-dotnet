#nullable enable

using System.ClientModel;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.FineTuning;

[Experimental("OPENAI001")]
[CodeGenType("FineTuningJobsPageToken")]
internal partial class InternalFineTuningJobsPageToken : ContinuationToken
{
    public InternalFineTuningJobsPageToken? GetNextPageToken(bool hasMore, string? after)
    {
        if (!hasMore || after is null)
        {
            return null;
        }

        return new InternalFineTuningJobsPageToken(Limit, After, null);
    }
}
