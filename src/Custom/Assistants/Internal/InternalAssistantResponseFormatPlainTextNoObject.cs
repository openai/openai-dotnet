namespace OpenAI.Assistants;

internal partial class InternalAssistantResponseFormatPlainTextNoObject : AssistantResponseFormat
{
    public string Value { get; set; }

    public InternalAssistantResponseFormatPlainTextNoObject(string value)
    {
        Value = value;
    }
}