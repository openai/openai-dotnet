# Agent Instructions

Read [CONTRIBUTING.md](CONTRIBUTING.md) and [SECURITY.md](SECURITY.md)
before making changes.

## Agent Test Execution

Agents must run tests only with `CLIENTMODEL_TEST_MODE=Playback` and
`CLIENTMODEL_DISABLE_AUTO_RECORDING=true`; never run `Record` or `Live` mode.
These settings avoid test runs that require unavailable credentials; they are
not a security boundary.

When running, writing, modifying, debugging, or validating tests, read the
[repository testing instructions](.github/skills/running-tests/SKILL.md)
before taking action.

## Security Requirements

- **Secrets and fixtures:** Never request, read, accept, or use real API keys,
  access tokens, connection secrets, signing credentials, or NuGet publishing
  tokens. Never commit, print, or embed credentials. Use synthetic data, safe
  placeholders, and local mocked transports in examples, fixtures, tests, and
  generated artifacts.
- **Logs and recordings:** Redact authorization headers, credentials, signed
  URLs, and customer data in diagnostics, test output, exceptions, telemetry,
  and artifacts. Use synthetic prompts/model responses. Sanitizers for known
  sensitive OpenAI headers and fields are enabled but may have gaps. Only
  explicitly authorized humans may capture recordings locally; they must review
  automatically sanitized recordings before publication. Never manually sanitize
  recordings or use a workflow that publishes before human review.
- **Dependencies and generators:** Review direct and transitive NuGet/npm
  changes, package provenance, trusted feeds and source mappings, local .NET
  tools, install scripts, and the root workspace `package-lock.json`.
  Neither agents nor human contributors may change dependency versions without
  an explicit request from the core team. Use `npm ci` for reproducible installs
  and preserve security-related dependency overrides. Never change the `Moq`
  version pinned to `[4.18.2]` in `Directory.Packages.props` without legal
  approval. Keep `@typespec/http-client-csharp` and
  `Microsoft.TypeSpec.Generator.ClientModel` synchronized through the existing
  coordinated generator-update workflow.
- **CI and releases:** Pin external GitHub Actions to full immutable commit
  SHAs. Keep workflow permissions and credentials least-privileged; never expose
  secrets to untrusted pull requests. Preserve the existing `release`
  environment and the protected `publish` environment, OIDC/Azure Key Vault
  signing, scoped `GITHUB_TOKEN` access, and NuGet publishing credentials.
- **Sensitive changes and testing:** Require focused maintainer review and
  regression coverage for authentication, custom endpoints, redirects, TLS,
  serialization, streaming, file uploads, logging, dependency resolution,
  generated output, and signing/publishing changes. Follow the test execution
  requirements above; never capture recordings with real credentials.
- **Vulnerability disclosure:** Report suspected vulnerabilities privately
  through [SECURITY.md](SECURITY.md) and the existing
  [OpenAI Bugcrowd program](https://bugcrowd.com/engagements/openai). Do not
  disclose vulnerabilities or secrets in public GitHub issues, pull requests,
  or discussions.
