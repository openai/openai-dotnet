using NUnit.Framework;
using OpenAI.Chat;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace OpenAI.Examples;

public partial class MutualTlsExamples
{
    [Test]
    [Explicit("Requires an enrolled X.509 workload identity provider and client certificate.")]
    public async Task Example02_X509WorkloadIdentityAsync()
    {
        if (!string.Equals(Environment.GetEnvironmentVariable("OPENAI_AUTH_MODE"), "x509", StringComparison.OrdinalIgnoreCase))
        {
            OpenAIClient apiKeyClient = new(GetRequiredEnvironmentVariable("OPENAI_API_KEY"));
            ChatCompletion apiKeyResult = await apiKeyClient.GetChatClient("gpt-4o-mini")
                .CompleteChatAsync("Reply with exactly: API-key request succeeded.");
            Console.WriteLine(apiKeyResult.Content[0].Text);
            return;
        }

        X509Certificate2Collection certificates = X509CertificateLoader.LoadPkcs12CollectionFromFile(
            GetRequiredEnvironmentVariable("OPENAI_MTLS_PFX_PATH"),
            Environment.GetEnvironmentVariable("OPENAI_MTLS_PFX_PASSWORD"));

        try
        {
            X509Certificate2 leaf = certificates.Single(certificate => certificate.HasPrivateKey);
            X509Certificate2Collection intermediates = new(
                certificates.Where(certificate => !certificate.HasPrivateKey).ToArray());

            using SocketsHttpHandler handler = new() { AllowAutoRedirect = false, UseCookies = false };
            handler.SslOptions.ClientCertificateContext =
                SslStreamCertificateContext.Create(leaf, intermediates, offline: true);

            using X509WorkloadIdentityCredential credential = new(
                GetRequiredEnvironmentVariable("OPENAI_IDENTITY_PROVIDER_ID"),
                GetRequiredEnvironmentVariable("OPENAI_SERVICE_ACCOUNT_ID"),
                new() { Handler = handler });

            string endpoint = Environment.GetEnvironmentVariable("OPENAI_BASE_URL");
            OpenAIClient client = endpoint is null
                ? new OpenAIClient(credential)
                : new OpenAIClient(credential, new() { Endpoint = GetEndpoint(endpoint) });

            ChatCompletion completion = await client.GetChatClient("gpt-4o-mini")
                .CompleteChatAsync("Reply with exactly: X.509 workload identity succeeded.");
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
}
