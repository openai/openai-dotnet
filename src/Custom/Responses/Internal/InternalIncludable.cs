namespace OpenAI.Responses;

[CodeGenType("Includable")]
internal readonly partial struct InternalIncludable
{
    public static implicit operator string(InternalIncludable self) => self.ToString();
}