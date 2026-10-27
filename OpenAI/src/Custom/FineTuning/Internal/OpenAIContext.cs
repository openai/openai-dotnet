using System.ClientModel.Primitives;

namespace OpenAI;

[ModelReaderWriterBuildable(typeof(FineTuning.InternalListFineTuningJobCheckpointsResponse))]
[ModelReaderWriterBuildable(typeof(FineTuning.InternalListFineTuningJobEventsResponse))]
[ModelReaderWriterBuildable(typeof(FineTuning.InternalListPaginatedFineTuningJobsResponse))]
public partial class OpenAIContext
{
}
