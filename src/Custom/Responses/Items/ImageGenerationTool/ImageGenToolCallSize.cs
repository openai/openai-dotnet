using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

[CodeGenType("ImageGenToolCallItemResourceSize")]
public readonly partial struct ImageGenToolCallSize
{
    [CodeGenMember("_1024x1024")]
    public static ImageGenToolCallSize W1024x1024 { get; } = new ImageGenToolCallSize(_1024x1024Value);

    [CodeGenMember("_1536x1024")]
    public static ImageGenToolCallSize W1536x1024 { get; } = new ImageGenToolCallSize(_1536x1024Value);

    [CodeGenMember("_1024x1536")]
    public static ImageGenToolCallSize W1024x1536 { get; } = new ImageGenToolCallSize(_1024x1536Value);

    [CodeGenMember("_256x256")]
    public static ImageGenToolCallSize W256x256 { get; } = new ImageGenToolCallSize(_256x256Value);

    [CodeGenMember("_512x512")]
    public static ImageGenToolCallSize W512x512 { get; } = new ImageGenToolCallSize(_512x512Value);

    [CodeGenMember("_1792x1024")]
    public static ImageGenToolCallSize W1792x1024 { get; } = new ImageGenToolCallSize(_1792x1024Value);

    [CodeGenMember("_1024x1792")]
    public static ImageGenToolCallSize W1024x1792 { get; } = new ImageGenToolCallSize(_1024x1792Value);
}