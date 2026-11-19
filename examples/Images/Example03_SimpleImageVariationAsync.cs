using NUnit.Framework;
using OpenAI.Images;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OpenAI.Examples;

public partial class ImageExamples
{
    [Test]
    public async Task Example03_SimpleImageVariationAsync()
    {
        ImageClient client = new("gpt-image-1", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        string imageFilePath = Path.Combine("Assets", "images_dog_and_cat.png");
        string prompt = "Create a variation of this image with the same animals and composition, but in a watercolor style.";

        GeneratedImage variation = await client.GenerateImageEditAsync(imageFilePath, prompt);
        BinaryData bytes = variation.ImageBytes;

        using FileStream stream = File.OpenWrite($"{Guid.NewGuid()}.png");
        bytes.ToStream().CopyTo(stream);
    }
}
