// Demonstrates API-key + HTTP mTLS with OpenAI by configuring .NET's native
// SocketsHttpHandler and passing its HttpClient through the SDK's existing
// transport seam. Keeping certificate handling in the HTTP transport preserves
// native .NET support for certificate stores, proxies, and credential rotation.

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

namespace OpenAI.Examples;

public partial class MutualTlsExamples
{
    [Test]
    [Explicit("Requires an API key and a client certificate enrolled in the OpenAI mTLS beta.")]
    public async Task Example01_MutualTlsAsync()
    {
        string apiKey = GetRequiredEnvironmentVariable("OPENAI_API_KEY");
        string pfxPath = GetRequiredEnvironmentVariable("OPENAI_CLIENT_PFX_PATH");
        Uri endpoint = GetEndpoint();

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
                AllowAutoRedirect = false,
            };
            handler.SslOptions.ClientCertificateContext = certificateContext;

            using HttpClient httpClient = new(handler);
            OpenAIClientOptions options = new()
            {
                Endpoint = endpoint,
                Transport = new HttpClientPipelineTransport(httpClient),
            };

            OpenAIClient client = new(new ApiKeyCredential(apiKey), options);
            ChatCompletion completion = await client
                .GetChatClient("gpt-4o-mini")
                .CompleteChatAsync("Reply with exactly: mTLS request succeeded.");

            Console.WriteLine(completion.Content[0].Text);
        }
        finally
        {
            foreach (X509Certificate2 certificate in certificates)
            {
                certificate.Dispose();
            }
        }
    }

    private static string GetRequiredEnvironmentVariable(string name)
    {
        string value = Environment.GetEnvironmentVariable(name);
        return !string.IsNullOrWhiteSpace(value)
            ? value
            : throw new InvalidOperationException($"{name} is required.");
    }

    private static Uri GetEndpoint()
    {
        // Choose the global endpoint or set OPENAI_BASE_URL to the EU endpoint:
        // https://mtls-eu.api.openai.com/v1
        string value = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
            ?? "https://mtls.api.openai.com/v1";

        if (!Uri.TryCreate(value, UriKind.Absolute, out Uri endpoint)
            || endpoint.Scheme != Uri.UriSchemeHttps)
        {
            throw new InvalidOperationException(
                "OPENAI_BASE_URL must be an absolute HTTPS URI.");
        }

        return endpoint;
    }
}
