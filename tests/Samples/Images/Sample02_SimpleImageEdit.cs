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
        public void Sample02_SimpleImageEdit()
        {
            ImageClient client = new("dall-e-2", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

            string imageFilename = "edit_sample_image.png";
            string imagePath = Path.Combine("Assets", imageFilename);
            using Stream image = File.OpenRead(imagePath);

            string prompt = "An inflatable flamingo float in a pool";

            string maskFilename = "edit_sample_mask.png";
            string maskPath = Path.Combine("Assets", maskFilename);
            using Stream mask = File.OpenRead(maskPath);

            ImageEditOptions options = new()
            {
                Size = GeneratedImageSize.W512xH512,
                ResponseFormat = GeneratedImageFormat.Bytes
            };

            GeneratedImage edit = client.GenerateImageEdit(image, imageFilename, prompt, mask, maskFilename, options);
            BinaryData bytes = edit.ImageBytes;

            using FileStream stream = File.OpenWrite($"{Guid.NewGuid()}.png");
            bytes.ToStream().CopyTo(stream);
        }
    }
}
