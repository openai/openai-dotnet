using NUnit.Framework;
using OpenAI.Audio;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests.Audio;

[Parallelizable(ParallelScope.All)]
[Category("Audio")]
[Category("Smoke")]
public class TranslationTemperatureTests
{
    [Test]
    public async Task TranslationSendsConfiguredTemperature(
        [Values(false, true)] bool asynchronous,
        [Values("en-US", "de-DE")] string cultureName,
        [Values(null, 0f, 0.25f, 1f)] float? temperature)
    {
        CultureInfo previous = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(cultureName);
        try
        {
            using RecordingHandler handler = new();
            using HttpClient http = new(handler);
            AudioClient client = new("whisper-1", new ApiKeyCredential("synthetic-key"),
                new OpenAIClientOptions { Transport = new HttpClientPipelineTransport(http) });
            using MemoryStream audio = new(Encoding.UTF8.GetBytes("synthetic audio fixture"));
            AudioTranslationOptions options = new()
            {
                Temperature = temperature,
                Prompt = "Synthetic translation prompt",
                ResponseFormat = AudioTranslationFormat.Simple,
            };
            AudioTranslation result = asynchronous
                ? (await client.TranslateAudioAsync(audio, "synthetic.wav", options)).Value
                : client.TranslateAudio(audio, "synthetic.wav", options).Value;
            Assert.That(result.Text, Is.EqualTo("Synthetic translated text"));
            Assert.That(handler.RequestBody, Does.Contain("synthetic audio fixture"));
            Assert.That(handler.RequestBody, Does.Contain("Synthetic translation prompt"));
            Assert.That(handler.RequestBody, Does.Contain("whisper-1"));
            Match field = Regex.Match(handler.RequestBody,
                "name=\"?temperature\"?\\r\\n(?:[^\\r\\n]+\\r\\n)*\\r\\n([^\\r\\n]+)");
            if (temperature.HasValue)
            {
                Assert.That(field.Success, Is.True, "The configured temperature must be included in multipart content.");
                Assert.That(float.Parse(field.Groups[1].Value, CultureInfo.InvariantCulture), Is.EqualTo(temperature.Value));
                Assert.That(field.Groups[1].Value, Does.Not.Contain(","));
            }
            else Assert.That(field.Success, Is.False);
            Assert.That(options.Temperature, Is.EqualTo(temperature));
            Assert.That(handler.Count, Is.EqualTo(1));
        }
        finally { CultureInfo.CurrentCulture = previous; }
    }

    private sealed class RecordingHandler : HttpMessageHandler
    {
        internal string RequestBody { get; private set; }
        internal int Count { get; private set; }
        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken token) => Respond(request);
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token) => Task.FromResult(Respond(request));
        private HttpResponseMessage Respond(HttpRequestMessage request)
        {
            Assert.That(request.RequestUri.AbsolutePath, Does.EndWith("/audio/translations"));
            RequestBody = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            Count++;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"text\":\"Synthetic translated text\"}", Encoding.UTF8, "application/json"),
            };
        }
    }
}
