#if SNIPPET || NET9_0_OR_GREATER
using NUnit.Framework;
using OpenAI.Chat;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace OpenAI.Tests.Snippets;

[Explicit("This test provides compile-time verification for the README mTLS snippet.")]
[Category("Snippets")]
[TestFixture]
public class MutualTlsReadMeSnippets
{
    [Test]
    public async Task MutualTls()
    {
        #region Snippet:ReadMe_MutualTls
        string pfxPath = Environment.GetEnvironmentVariable("OPENAI_CLIENT_PFX_PATH")
            ?? throw new InvalidOperationException("OPENAI_CLIENT_PFX_PATH is required.");
        string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
            ?? throw new InvalidOperationException("OPENAI_API_KEY is required.");

        X509Certificate2Collection certificates =
            X509CertificateLoader.LoadPkcs12CollectionFromFile(
                pfxPath,
                Environment.GetEnvironmentVariable("OPENAI_CLIENT_PFX_PASSWORD"));

        try
        {
            X509Certificate2 clientCertificate =
                certificates.Single(certificate => certificate.HasPrivateKey);
            X509Certificate2Collection intermediateCertificates = new(
                certificates
                    .Where(certificate => !certificate.HasPrivateKey)
                    .ToArray());
            SslStreamCertificateContext certificateContext =
                SslStreamCertificateContext.Create(
                    clientCertificate,
                    intermediateCertificates,
                    offline: true);

            SocketsHttpHandler handler = new()
            {
                // Do not automatically follow a redirect with a certificate-bearing handler.
                AllowAutoRedirect = false,
            };
            handler.SslOptions.ClientCertificateContext = certificateContext;

            using HttpClient httpClient = new(handler)
            {
                // Let the SDK pipeline enforce OpenAIClientOptions.NetworkTimeout.
                Timeout = System.Threading.Timeout.InfiniteTimeSpan,
            };
            OpenAIClientOptions options = new()
            {
                Endpoint = new Uri("https://mtls.api.openai.com/v1"),
                Transport = new HttpClientPipelineTransport(httpClient),
            };

            OpenAIClient client = new(
                new ApiKeyCredential(apiKey),
                options);

            ChatCompletion completion = await client
                .GetChatClient("gpt-4o-mini")
                .CompleteChatAsync("Reply with exactly: mTLS request succeeded.");
        }
        finally
        {
            foreach (X509Certificate2 certificate in certificates)
            {
                certificate.Dispose();
            }
        }
        #endregion
    }
}
#endif
