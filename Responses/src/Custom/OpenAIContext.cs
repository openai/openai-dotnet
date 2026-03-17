using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

[Experimental("OPENAI001")]
public partial class OpenAIContext : ModelReaderWriterContext
{
    internal static OpenAIContext Default { get; } = new OpenAIContext();
}