using NUnit.Framework;
using OpenAI.Images;
using System;
using System.IO;

namespace OpenAI.Samples
{
    public partial class ImageSamples
    {
        [Test]
        [Ignore("Compilation validation only")]
        public void Sample03_SimpleImageVariation()
        {
            ImageClient client = new("dall-e-2", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

            string imageFilename = "variation_sample_image.png";
            string imagePath = Path.Combine("Assets", imageFilename);
            using Stream image = File.OpenRead(imagePath);

            ImageVariationOptions options = new()
            {
                Size = GeneratedImageSize.W256xH256,
                ResponseFormat = GeneratedImageFormat.Bytes
            };

            GeneratedImage variation = client.GenerateImageVariation(image, imageFilename, options);
            BinaryData bytes = variation.ImageBytes;

            using FileStream stream = File.OpenWrite($"{Guid.NewGuid()}.png");
            bytes.ToStream().CopyTo(stream);
        }
    }
}
