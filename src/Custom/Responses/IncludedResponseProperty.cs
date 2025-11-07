namespace OpenAI.Responses;

[CodeGenType("Includable")]
public readonly partial struct IncludedResponseProperty
{
    [CodeGenMember("MessageInputImageImageUrl")]
    public static IncludedResponseProperty MessageInputImageUri { get; } = new IncludedResponseProperty(MessageInputImageImageUrlValue);

    [CodeGenMember("ComputerCallOutputOutputImageUrl")]
    public static IncludedResponseProperty ComputerCallOutputImageUri { get; } = new IncludedResponseProperty(ComputerCallOutputOutputImageUrlValue);
}