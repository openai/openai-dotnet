using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

[Experimental("OPENAI001")]
public class OpenAIResponsesContext
{
    public static ModelReaderWriterContext Default => global::OpenAI.OpenAIContext.Default;
}
