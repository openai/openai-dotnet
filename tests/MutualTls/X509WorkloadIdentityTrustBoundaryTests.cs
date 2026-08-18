using NUnit.Framework;
using System;
using System.ClientModel;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace OpenAI.Tests.MutualTls;

public sealed partial class X509WorkloadIdentityTests
{
    [TestCase("https://tenant.openai.azure.com/openai/v1")]
    [TestCase("https://tenant.cognitiveservices.azure.com/openai/v1")]
    [TestCase("https://tenant.openai.azure.cn/openai/v1")]
    [TestCase("https://TENANT.OPENAI.AZURE.COM./openai/v1")]
    [TestCase("https://bedrock-runtime.us-east-1.amazonaws.com/v1")]
    [TestCase("https://bedrock-runtime.us-east-1.api.aws/v1")]
    [TestCase("https://BEDROCK-RUNTIME.US-EAST-1.API.AWS.:8443/v1")]
    [TestCase("https://bedrock-runtime.cn-north-1.api.amazonwebservices.com.cn/v1")]
    [TestCase("https://bedrock-runtime.us-iso-east-1.c2s.ic.gov/v1")]
    [TestCase("https://bedrock-runtime.us-isob-east-1.sc2s.sgov.gov/v1")]
    [TestCase("https://bedrock-runtime.eu-isoe-west-1.cloud.adc-e.uk/v1")]
    [TestCase("https://bedrock-runtime.us-isof-south-1.csp.hci.ic.gov/v1")]
    [TestCase("https://bedrock-runtime.eusc-de-east-1.amazonaws.eu/v1")]
    [TestCase("https://bedrock-runtime.eusc-de-east-1.api.amazonwebservices.eu/v1")]
    [TestCase("https://generativelanguage.googleapis.com/v1")]
    public async Task ClientRejectsProviderOwnedEndpointsBeforeTokenAcquisition(string endpoint)
    {
        await using TestServer server = await TestServer.StartAsync();
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClientOptions options = new() { Endpoint = new Uri(endpoint) };

        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => new OpenAIClient(credential, options));

        Assert.That(exception.Message, Does.Contain("provider").IgnoreCase);
        Assert.That(options.Transport, Is.Null);
        Assert.That(server.ExchangeCount, Is.Zero);
        Assert.That(server.ApiCount, Is.Zero);
    }

    [Test]
    public void ApiKeyClientsContinueToSupportAzureEndpoints()
    {
        Uri endpoint = new("https://tenant.openai.azure.com/openai/v1");
        OpenAIClient client = new(new ApiKeyCredential("existing-api-key"), new() { Endpoint = endpoint });

        Assert.That(client.Endpoint, Is.EqualTo(endpoint));
    }

    [Test]
    public async Task CustomerOwnedGatewayContainingProviderTextRemainsSupported()
    {
        await using TestServer server = await TestServer.StartAsync();
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        Uri endpoint = new("https://openai.azure.com.customer.example.test/v1");
        OpenAIClient client = new(credential, new() { Endpoint = endpoint });

        using System.ClientModel.Primitives.PipelineMessage response = await SendAsync(client);

        Assert.That(response.Response.Status, Is.EqualTo(200));
        Assert.That(server.ExchangeCount, Is.EqualTo(1));
        Assert.That(server.ApiCount, Is.EqualTo(1));
    }

    [TestCase(false)]
    [TestCase(true)]
    public void ConstructorRejectsHostDependentClientCertificateSelectors(bool pinCertificateContext)
    {
        using ECDsa authKey = ECDsa.Create();
        using ECDsa apiKey = ECDsa.Create();
        CertificateRequest authRequest = new("CN=auth-workload", authKey, HashAlgorithmName.SHA256);
        CertificateRequest apiRequest = new("CN=api-workload", apiKey, HashAlgorithmName.SHA256);
        using X509Certificate2 authCertificate = authRequest.CreateSelfSigned(
            DateTimeOffset.UtcNow.AddMinutes(-1), DateTimeOffset.UtcNow.AddHours(1));
        using X509Certificate2 apiCertificate = apiRequest.CreateSelfSigned(
            DateTimeOffset.UtcNow.AddMinutes(-1), DateTimeOffset.UtcNow.AddHours(1));
        using SocketsHttpHandler handler = new() { AllowAutoRedirect = false, UseCookies = false };
        handler.SslOptions.LocalCertificateSelectionCallback = (_, host, _, _, _) =>
            host == "mtls.auth.openai.com" ? authCertificate : apiCertificate;
        if (pinCertificateContext)
        {
            handler.SslOptions.ClientCertificateContext = SslStreamCertificateContext.Create(
                authCertificate,
                new X509Certificate2Collection(),
                offline: true);
        }

        ArgumentException exception = Assert.Throws<ArgumentException>(() => CreateCredential(handler));

        Assert.That(exception.Message, Does.Contain("certificate").IgnoreCase);
    }

    [Test]
    public void ConstructorRejectsAutomaticClientCertificateSelection()
    {
        using HttpClientHandler handler = new()
        {
            AllowAutoRedirect = false,
            UseCookies = false,
            ClientCertificateOptions = ClientCertificateOption.Automatic,
        };

        ArgumentException exception = Assert.Throws<ArgumentException>(() => CreateCredential(handler));

        Assert.That(exception.Message, Does.Contain("certificate").IgnoreCase);
    }

    [TestCase(false)]
    [TestCase(true)]
    public void ConstructorRejectsMultipleSelectableClientCertificates(bool useSocketsHandler)
    {
        using ECDsa firstKey = ECDsa.Create();
        using ECDsa secondKey = ECDsa.Create();
        CertificateRequest firstRequest = new("CN=first-workload", firstKey, HashAlgorithmName.SHA256);
        CertificateRequest secondRequest = new("CN=second-workload", secondKey, HashAlgorithmName.SHA256);
        using X509Certificate2 first = firstRequest.CreateSelfSigned(
            DateTimeOffset.UtcNow.AddMinutes(-1), DateTimeOffset.UtcNow.AddHours(1));
        using X509Certificate2 second = secondRequest.CreateSelfSigned(
            DateTimeOffset.UtcNow.AddMinutes(-1), DateTimeOffset.UtcNow.AddHours(1));
        using HttpMessageHandler handler = useSocketsHandler
            ? new SocketsHttpHandler
            {
                AllowAutoRedirect = false,
                UseCookies = false,
                SslOptions = new SslClientAuthenticationOptions
                {
                    ClientCertificates = new X509CertificateCollection { first, second },
                },
            }
            : new HttpClientHandler
            {
                AllowAutoRedirect = false,
                UseCookies = false,
                ClientCertificates = { first, second },
            };

        ArgumentException exception = Assert.Throws<ArgumentException>(() => CreateCredential(handler));

        Assert.That(exception.Message, Does.Contain("certificate").IgnoreCase);
    }

    [Test]
    public async Task ClientRejectsCertificateSelectorAddedAfterCredentialConstruction()
    {
        await using TestServer server = await TestServer.StartAsync();
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient client = new(credential);
        handler.SslOptions.LocalCertificateSelectionCallback = (_, _, _, _, _) => null;

        ArgumentException exception = Assert.ThrowsAsync<ArgumentException>(async () =>
            await SendAsync(client));

        Assert.That(exception.Message, Does.Contain("certificate").IgnoreCase);
        Assert.That(server.ExchangeCount, Is.Zero);
        Assert.That(server.ApiCount, Is.Zero);
    }

    [TestCase(false)]
    [TestCase(true)]
    public void ImplicitProxyFallsBackToHandlerCredentialsWhenSharedCredentialsAreAbsent(
        bool useSocketsHandler)
    {
        IWebProxy previous = HttpClient.DefaultProxy;
        try
        {
            HttpClient.DefaultProxy = new WebProxy("http://proxy.example.test:8080");
            NetworkCredential credentials = new("handler-user", "handler-secret");
            using HttpMessageHandler handler = useSocketsHandler
                ? new SocketsHttpHandler
                {
                    AllowAutoRedirect = false,
                    UseCookies = false,
                    DefaultProxyCredentials = credentials,
                }
                : new HttpClientHandler
                {
                    AllowAutoRedirect = false,
                    UseCookies = false,
                    DefaultProxyCredentials = credentials,
                };
            using X509WorkloadIdentityCredential credential = CreateCredential(handler);
            IWebProxy protectedProxy = useSocketsHandler
                ? ((SocketsHttpHandler)handler).Proxy
                : ((HttpClientHandler)handler).Proxy;

            Assert.That(protectedProxy.Credentials, Is.SameAs(credentials));
            Assert.That(HttpClient.DefaultProxy.Credentials, Is.Null);
        }
        finally
        {
            HttpClient.DefaultProxy = previous;
        }
    }
}
