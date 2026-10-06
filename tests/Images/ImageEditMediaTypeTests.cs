using Microsoft.AspNetCore.WebUtilities;
using NUnit.Framework;
using OpenAI.Images;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests.Images;

[Category("Images")]
[Category("Smoke")]
[Parallelizable(ParallelScope.All)]
public class ImageEditMediaTypeTests
{
    private static readonly Dictionary<string, string> MediaTypes = new()
    {
        ["synthetic.png"] = "image/png",
        ["ファイル.png"] = "image/png",
        ["файл.PNG"] = "image/png",
        ["파일.jpg"] = "image/jpeg",
        ["café.JPEG"] = "image/jpeg",
        ["synthetic picture.webp"] = "image/webp",
        ["synthetic.bin"] = null,
        ["synthetic"] = null,
    };

    [Test]
    public async Task EditRequestsPreserveFilePartsAndTheirMediaTypes(
        [Values(false, true)] bool asynchronous,
        [Values(false, true)] bool includeMask,
        [ValueSource(nameof(Filenames))] string filename)
    {
        byte[] imageBytes = [1, 2, 3, 255];
        byte[] maskBytes = [5, 6, 7];
        using RecordingHandler handler = new();
        using HttpClient http = new(handler);
        ImageClient client = new("gpt-image-2", new ApiKeyCredential("synthetic-key"),
            new OpenAIClientOptions { Transport = new HttpClientPipelineTransport(http) });
        using MemoryStream image = new(imageBytes);
        using MemoryStream mask = new(maskBytes);
        if (includeMask)
        {
            if (asynchronous) await client.GenerateImageEditsAsync(image: image, imageFilename: filename, prompt: "Synthetic edit", imageCount: 1, mask: mask, maskFilename: "マスク.png");
            else client.GenerateImageEdits(image: image, imageFilename: filename, prompt: "Synthetic edit", imageCount: 1, mask: mask, maskFilename: "マスク.png");
        }
        else
        {
            if (asynchronous) await client.GenerateImageEditsAsync(image, filename, "Synthetic edit", 1);
            else client.GenerateImageEdits(image, filename, "Synthetic edit", 1);
        }
        Assert.That(handler.Calls, Is.EqualTo(1));
        Assert.That(handler.Parts["image"].MediaType, Is.EqualTo(MediaTypes[filename]));
        Assert.That(handler.Parts["image"].Filename, Is.EqualTo(filename));
        Assert.That(handler.Parts["image"].Bytes, Is.EqualTo(imageBytes));
        Assert.That(Encoding.UTF8.GetString(handler.Parts["prompt"].Bytes), Is.EqualTo("Synthetic edit"));
        Assert.That(handler.Parts.ContainsKey("mask"), Is.EqualTo(includeMask));
        if (includeMask)
        {
            Assert.That(handler.Parts["mask"].MediaType, Is.EqualTo("image/png"));
            Assert.That(handler.Parts["mask"].Filename, Is.EqualTo("マスク.png"));
            Assert.That(handler.Parts["mask"].Bytes, Is.EqualTo(maskBytes));
        }
    }

    private static IEnumerable<string> Filenames => MediaTypes.Keys;

    private sealed class RecordingHandler : HttpMessageHandler
    {
        internal int Calls { get; private set; }
        internal Dictionary<string, (string Filename, string MediaType, byte[] Bytes)> Parts { get; } = new();
        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken token) => Respond(request).GetAwaiter().GetResult();
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token) => Respond(request);
        private async Task<HttpResponseMessage> Respond(HttpRequestMessage request)
        {
            Assert.That(request.RequestUri.AbsolutePath, Does.EndWith("/images/edits"));
            string boundary = request.Content.Headers.ContentType.Parameters.Single(parameter => parameter.Name == "boundary").Value.Trim('"');
            using MemoryStream body = new(await request.Content.ReadAsByteArrayAsync().ConfigureAwait(false));
            MultipartReader reader = new(boundary, body);
            MultipartSection section;
            while ((section = await reader.ReadNextSectionAsync().ConfigureAwait(false)) is not null)
            {
                ContentDispositionHeaderValue disposition = ContentDispositionHeaderValue.Parse(section.ContentDisposition);
                using MemoryStream bytes = new();
                await section.Body.CopyToAsync(bytes).ConfigureAwait(false);
                Parts[disposition.Name.Trim('"')] = (disposition.FileNameStar ?? disposition.FileName?.Trim('"'), section.ContentType, bytes.ToArray());
            }
            Calls++;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"created\":0,\"data\":[{\"b64_json\":\"AA==\"}]}", Encoding.UTF8, "application/json"),
            };
        }
    }
}
