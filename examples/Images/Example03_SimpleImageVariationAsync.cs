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
        ImageClient client = new("dall-e-2", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        string imageFilePath = Path.Combine("Assets", "images_dog_and_cat.png");

        ImageVariationOptions options = new()
        {
            Size = GeneratedImageSize.W256xH256,
            ResponseFormat = GeneratedImageFormat.Bytes
        };

        GeneratedImage variation = await client.GenerateImageVariationAsync(imageFilePath, options);
        BinaryData bytes = variation.ImageBytes;

        using FileStream stream = File.OpenWrite($"{Guid.NewGuid()}.png");
        bytes.ToStream().CopyTo(stream);
    }
}
