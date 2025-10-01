namespace OpenAI.Assistants;

public partial class InternalAssistantResponseFormatPlainTextNoObject : AssistantResponseFormat
{
    public string Value { get; set; }

    internal InternalAssistantResponseFormatPlainTextNoObject() : base() { }

    public InternalAssistantResponseFormatPlainTextNoObject(string value)
    {
        Value = value;
    }
}