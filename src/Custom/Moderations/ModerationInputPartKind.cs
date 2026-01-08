namespace OpenAI.Moderations;

// CUSTOM: Renamed
[CodeGenType("CreateModerationRequestInputType")]
public enum ModerationInputPartKind
{
    [CodeGenMember("Text")]
    Text,

    [CodeGenMember("ImageUrl")]
    Image,
}
