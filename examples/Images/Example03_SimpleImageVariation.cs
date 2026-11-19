using NUnit.Framework;
using OpenAI.Images;
using System;
using System.IO;

namespace OpenAI.Examples;

public partial class ImageExamples
{
    [Test]
    public void Example03_SimpleImageVariation()
    {
        ImageClient client = new("gpt-image-1", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        string imageFilePath = Path.Combine("Assets", "images_dog_and_cat.png");
        string prompt = "Create a variation of this image with the same animals and composition, but in a watercolor style.";

        GeneratedImage variation = client.GenerateImageEdit(imageFilePath, prompt);
        BinaryData bytes = variation.ImageBytes;

        using FileStream stream = File.OpenWrite($"{Guid.NewGuid()}.png");
        bytes.ToStream().CopyTo(stream);
    }
}
