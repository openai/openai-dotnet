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

    UserData,
}