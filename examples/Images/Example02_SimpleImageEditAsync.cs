using NUnit.Framework;
using OpenAI.Images;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OpenAI.Examples;

public partial class ImageExamples
{
    [Test]
    public async Task Example02_SimpleImageEditAsync()
    {
        ImageClient client = new("dall-e-2", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        string imageFilePath = Path.Combine("Assets", "images_flower_vase.png");
        string prompt = "A vase full of beautiful flowers.";
        string maskFilePath = Path.Combine("Assets", "images_flower_vase_with_mask.png");

        ImageEditOptions options = new()
        {
            Size = GeneratedImageSize.W512xH512,
            ResponseFormat = GeneratedImageFormat.Bytes
        };

        GeneratedImage edit = await client.GenerateImageEditAsync(imageFilePath, prompt, maskFilePath, options);
        BinaryData bytes = edit.ImageBytes;

        using FileStream stream = File.OpenWrite($"{Guid.NewGuid()}.png");
        await bytes.ToStream().CopyToAsync(stream);
    }
}
