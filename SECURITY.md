# Security Policy

## Reporting a vulnerability

Please report suspected vulnerabilities privately through OpenAI's
[Bugcrowd program](https://bugcrowd.com/engagements/openai). The program page
contains OpenAI's vulnerability disclosure guidelines and scope.

Do not report security vulnerabilities through public GitHub issues, pull
requests, or discussions.

This policy applies to the source code in this repository and the official
[`OpenAI` NuGet package](https://www.nuget.org/packages/OpenAI) published from
it.

For vulnerabilities in other OpenAI products or services, use the same
[OpenAI Bugcrowd program](https://bugcrowd.com/engagements/openai) and select
the appropriate target.

## What to include

When reporting a vulnerability, include:

- The affected package or product and version, or the affected commit.
- A clear description of the potential impact.
- Sanitized steps to reproduce the issue or a minimal proof of concept.
- The .NET runtime, target framework, and operating system, when relevant.
- Any known mitigations or workarounds.

Do not include live credentials, API keys, customer data, or unredacted
sensitive logs.

## Coordinated disclosure

Please give the maintainers a reasonable opportunity to investigate and
address the issue before public disclosure.

## Trust boundaries

The OpenAI .NET library is a client for the OpenAI API and for caller-configured
compatible endpoints. It turns caller input into requests and service responses
into client results. It is not a security boundary for enforcing service
authorization, business rules, or the safety of application content.

### Scope of reports

An SDK vulnerability report should demonstrate how an untrusted party can cross
a security boundary owned by this library or its development infrastructure in
its intended environment. Identify the affected client and source revision,
the attacker's access, the input they control, the path to the affected
operation, and the observed impact. Include any special configuration required
to reproduce the issue and distinguish default behavior from caller
customizations. The [reporting instructions](#reporting-a-vulnerability) and
[information to include](#what-to-include) above still apply.

We accept reports only for issues that reproduce at the current HEAD of
`main` or on an active feature branch. Findings limited to other revisions
are not in scope.

Verify the report against current code and, where relevant, official service
documentation. Explain any claims you could not verify. A hypothetical
deployment, deliberately unsafe caller configuration, or finding that requires
prior compromise of the host, network, or service does not by itself establish
an SDK vulnerability. The same applies to actions available only to maintainers
or other trusted parties through their existing privileges.

### Service requests and responses

- Clients use the API key or authentication policy supplied by the caller.
  The service validates credentials and enforces authorization. The SDK does
  not determine what permissions a credential grants.

- The service must validate requests and ensure that its responses are safe.
  Clients may check inputs needed to construct requests, but do not replace
  service-side security checks or sanitize arbitrary request payloads.

- Clients trust responses from the configured service, including values used
  in subsequent protocol requests. They do not provide a security boundary
  against a compromised service or a malicious caller-selected endpoint.

- Callers must validate data they pass to the SDK and data they receive from it
  before using that data in sensitive application operations, including
  rendering content or accessing local resources.

### Client configuration and the host environment

Callers control the endpoint, credentials, transport, and other client options.
The default HTTP endpoint is `https://api.openai.com/v1`, but an application
can configure another endpoint, including a proxy or an OpenAI-compatible
service. Callers must verify that their configured destination is the one they
intend, use appropriate transport security, and avoid disabling certificate
validation. Selecting an attacker-controlled endpoint and supplying it with a
credential is not, by itself, an SDK vulnerability.

The SDK runs inside the caller's application. It cannot protect its inputs,
outputs, or credentials from a compromised host. A custom transport, policy,
or authentication implementation also executes with the application's
privileges and remains the caller's responsibility.

### Defaults and customization

The SDK is responsible for its own default behavior. A report about redirects
or other transport behavior should show what the affected client and transport
actually do, how the destination is chosen, and whether credentials or other
sensitive data cross an unintended boundary. Do not infer a vulnerability
solely from the existence of configurable transport or redirect support.
Applications that replace the transport or enable redirect handling must
assess the security of those choices.

Clients retry certain transient HTTP errors by default. Callers can choose
settings appropriate to their availability and resource requirements.
Expected retries and delays under the selected configuration do not alone
establish a denial-of-service vulnerability.

### Diagnostics, telemetry, and recordings

The library supports optional OpenTelemetry tracing and metrics, and sends
SDK and platform description headers by default. These are separate features,
with separate opt-out controls described in
[Observability](./docs/Observability.md). Evaluate a disclosure claim against
the data the affected feature actually emits. OpenTelemetry instrumentation
does not opt in to capturing prompts or generated content.

Callers control any additional HTTP diagnostics, logging, and telemetry in
their application, as well as where that output is stored and who can access
it. They must protect sensitive output and redact content when enabling
logging or adding custom instrumentation. If an application exposes its logs
through its own storage or access settings, that exposure is not, by itself,
an SDK vulnerability. Test recordings are sanitized by the test framework,
but contributors must check recordings for sensitive data before sharing them.

### Development tools and automation

Repository scripts, code generators, test runners, and GitHub Actions workflows
are development tools, not public-facing SDK services. Developers and
maintainers choose when to run them and what files, commands, specifications,
and extensions to supply. A report about these tools should show how an
untrusted party can affect an execution in its intended environment without
already having the privileges of the person running it. A developer
intentionally supplying a command, path outside a working directory, or
untrusted executable extension does not alone demonstrate that boundary
crossing.

The repository generates client code from a local representation of
the [official OpenAI REST specification](https://github.com/openai/openai-openapi/blob/main/openapi.yaml).
Treat source specifications and executable generator inputs as trusted
development inputs, not as untrusted requests to a public service.
Review the provenance of any replacement specifications or extensions before
running code generation. Do not assume that a CI runner or test environment
has no access to credentials or other sensitive resources without checking
the specific workflow and its permissions.

## Automated security scanning

Automated tools and AI-assisted scanners must apply the trust boundaries above
and meet the same [reporting requirements](#scope-of-reports) as other reports.
Review generated findings against the affected code and intended environment,
run the proposed reproduction, and verify that the observed result supports
the claimed impact. Remove unsupported claims, disclose what remains
unverified, and submit a concise, privately filed report rather than raw
scanner output.
