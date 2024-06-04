using NUnit.Framework;
using OpenAI.Images;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OpenAI.Examples;

public partial class ImageExamples
{
    [Test]
    public async Task Example01_SimpleImageGenerationAsync()
    {
        ImageClient client = new("dall-e-3", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        string prompt = "The concept for a living room that blends Scandinavian simplicity with Japanese minimalism for"
            + " a serene and cozy atmosphere. It's a space that invites relaxation and mindfulness, with natural light"
            + " and fresh air. Using neutral tones, including colors like white, beige, gray, and black, that create a"
            + " sense of harmony. Featuring sleek wood furniture with clean lines and subtle curves to add warmth and"
            + " elegance. Plants and flowers in ceramic pots adding color and life to a space. They can serve as focal"
            + " points, creating a connection with nature. Soft textiles and cushions in organic fabrics adding comfort"
            + " and softness to a space. They can serve as accents, adding contrast and texture.";

        ImageGenerationOptions options = new()
        {
            Quality = GeneratedImageQuality.High,
            Size = GeneratedImageSize.W1792xH1024,
            Style = GeneratedImageStyle.Vivid,
            ResponseFormat = GeneratedImageFormat.Bytes
        };

        GeneratedImage image = await client.GenerateImageAsync(prompt, options);
        BinaryData bytes = image.ImageBytes;

        using FileStream stream = File.OpenWrite($"{Guid.NewGuid()}.png");
        bytes.ToStream().CopyTo(stream);
    }
}
