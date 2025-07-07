using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Files;

[CodeGenType("OpenAIFilePurpose")]
public enum FilePurpose
{
    Assistants,

    AssistantsOutput,

    Batch,

    BatchOutput,

    FineTune,

    FineTuneResults,

    Vision,

    [Experimental("OPENAI001")]
    UserData,

    [Experimental("OPENAI001")]
    Evaluations,
}