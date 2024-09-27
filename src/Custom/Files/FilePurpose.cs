namespace OpenAI.Files;

[CodeGenModel("OpenAIFilePurpose")]
public enum FilePurpose
{
    Assistants,

    AssistantsOutput,

    Batch,

    BatchOutput,

    FineTune,

    FineTuneResults,

    Vision,
}