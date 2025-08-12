namespace OpenAI.Responses;

[CodeGenType("Includable")]
public readonly partial struct InternalIncludable
{
    public static implicit operator string(InternalIncludable self) => self.ToString();
}