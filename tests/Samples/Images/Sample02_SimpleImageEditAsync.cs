using NUnit.Framework;
using OpenAI.Images;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OpenAI.Samples
{
    public partial class ImageSamples
    {
        [Test]
        [Ignore("Compilation validation only")]
        public async Task Sample02_SimpleImageEditAsync()
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

            GeneratedImage edit = await client.GenerateImageEditAsync(image, imageFilename, prompt, mask, maskFilename, options);
            BinaryData bytes = edit.ImageBytes;

            using FileStream stream = File.OpenWrite($"{Guid.NewGuid()}.png");
            await bytes.ToStream().CopyToAsync(stream);
        }
    }
}
