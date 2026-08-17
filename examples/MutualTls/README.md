# API-key + HTTP mTLS example

This example configures mTLS on .NET's native `SocketsHttpHandler` and passes the resulting `HttpClient` to the OpenAI SDK through `HttpClientPipelineTransport`. Keeping certificate handling in the HTTP transport preserves .NET's native certificate, proxy, and rotation capabilities without adding an SDK-specific mTLS option.

Before running it, follow the [OpenAI Mutual TLS Beta Program](https://help.openai.com/en/articles/10876024-openai-mutual-tls-beta-program) instructions to understand request authentication and enroll. Upload and activate the CA certificate that issues your client certificate; keep the leaf client certificate and its private key in the PFX used by this example. To manage uploaded CA certificates programmatically, see the [Certificates API reference](https://developers.openai.com/api/reference/resources/admin/subresources/organization/subresources/certificates/).

## Certificate bundle

Certificate-chain support is available by request and must be enabled for your organization. Without it, the leaf client certificate must be signed directly by the uploaded CA. When chain support is enabled, set `OPENAI_CLIENT_PFX_PATH` to a PFX/PKCS#12 bundle containing:

1. Exactly one certificate with a private key: the leaf client certificate.
2. Every required intermediate certificate.

Do not include the root CA certificate; TLS peers establish trust from their own root stores. Do not rely on `X509Certificate2.CreateFromPemFile` for a certificate-chain PEM: it loads only the first certificate. A PFX bundle keeps the leaf, private key, and intermediates together for `SslStreamCertificateContext`, which ensures the TLS handshake can present the complete chain.

## Endpoint

Set `OPENAI_BASE_URL` to the base URL that matches your data residency:

| Scope | Base URL |
| --- | --- |
| Global (default) | `https://mtls.api.openai.com/v1` |
| EU Data Residency | `https://mtls-eu.api.openai.com/v1` |

For safety, the example accepts only these two HTTPS origins and the `/v1` base path. It rejects custom hosts, non-default ports, credentials, query strings, and fragments so the API key and client certificate cannot be sent to an unintended server.

## Run the example

From PowerShell at the repository root, set the environment variables for the
current process and run the explicit example test:

```powershell
$env:OPENAI_API_KEY = "sk-..."
$env:OPENAI_CLIENT_PFX_PATH = "C:\path\to\client-chain.pfx"
$env:OPENAI_CLIENT_PFX_PASSWORD = "optional-password"

dotnet test .\examples\OpenAI.Examples.csproj `
  --framework net10.0 `
  -- NUnit.Where="test == 'OpenAI.Examples.MutualTlsExamples.Example01_MutualTlsAsync'"
```

The example calls the supported Chat Completions endpoint and prints its text
response. For EU Data Residency, set:

```powershell
$env:OPENAI_BASE_URL = "https://mtls-eu.api.openai.com/v1"
```

The handler disables automatic redirects so the certificate-bearing transport does not follow a redirect to another host. Keep the `HttpClient` alive for the lifetime of the SDK client. When rotating the certificate, create a new handler, transport, and SDK client; an existing pooled TLS connection will not renegotiate with replacement credentials.

## End-to-end testing

The automated mTLS tests create a private root, intermediate, server, and client
certificate for each test. They start a loopback TLS server that requires a
trusted client certificate, send a request through the real
`SocketsHttpHandler`, `HttpClientPipelineTransport`, and OpenAI Chat client, and
verify the TLS chain, API key, HTTP request, and deserialized SDK response. They
also verify that missing or untrusted client credentials fail closed and that
the certificate-bearing handler does not follow redirects.

Run these deterministic tests from PowerShell:

```powershell
dotnet test .\tests\OpenAI.Tests.csproj `
  --filter "TestCategory=MutualTls"
```

The explicit example test above is the live end-to-end check against OpenAI. It
requires an API key and a client certificate whose CA has been uploaded and
activated for the same organization or project, so it is not suitable for
credential-free pull request CI.

## X.509 workload identity federation

`Example02_X509WorkloadIdentityAsync` demonstrates an application-owned rollout toggle between API-key authentication and X.509 workload identity federation:

```powershell
$env:OPENAI_AUTH_MODE = "x509"
$env:OPENAI_IDENTITY_PROVIDER_ID = "idp_..."
$env:OPENAI_SERVICE_ACCOUNT_ID = "svc_acct_..."
$env:OPENAI_MTLS_PFX_PATH = "C:\path\to\client-chain.pfx"
$env:OPENAI_MTLS_PFX_PASSWORD = "optional-password"

dotnet test .\examples\OpenAI.Examples.csproj `
  --framework net10.0 `
  -- NUnit.Where="test == 'OpenAI.Examples.MutualTlsExamples.Example02_X509WorkloadIdentityAsync'"
```

Set `OPENAI_AUTH_MODE=api_key` and provide `OPENAI_API_KEY` to keep the existing API-key path. The application loads the PFX and configures a caller-owned `SocketsHttpHandler`; the credential accepts that handler as its only transport input, verifies redirects are disabled, and constructs one shared HTTP client for token exchange and HTTP API calls. Do not provide a separate SDK transport.

X.509 endpoints must use HTTPS without userinfo and cannot target another cloud provider; trusted custom OpenAI gateways remain supported, and API-key clients can still use Azure OpenAI. Configure exactly one fixed client certificate because automatic and host-dependent certificate selection can mix identities between token exchange and API requests. Custom pipeline policies cannot change the final API origin or replace the issued bearer. Disable automatic handler cookies with `UseCookies = false` and configure an HTTP CONNECT proxy when needed; native default-proxy credential precedence is preserved, credential changes stay handler-local, and HTTPS proxies and destination-server credentials are rejected to prevent certificate or credential disclosure.

Keep the handler, credential, and OpenAI clients alive together. The credential disposes only the HTTP-client wrapper it creates, never the application's certificate-bearing handler. Rotate the group together to establish new TLS connections with the replacement certificate. X.509 workload identity supports HTTP APIs only; Realtime WebSockets require separate support.
