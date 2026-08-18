# Agent Instructions

Read [CONTRIBUTING.md](CONTRIBUTING.md), [SECURITY.md](SECURITY.md), and the
[repository testing instructions](.github/skills/running-tests/SKILL.md) before
making changes.

This SDK is generated in collaboration with Microsoft. Treat
`specification/base/` as an upstream-owned copy; make client-specific changes in
`specification/client/` and the `OpenAI/src/Custom/` or
`OpenAI.Responses/src/Custom/` trees. Never hand-edit either project's
`src/Generated/` output. Use `./scripts/Invoke-CodeGen.ps1` when regeneration is
required, and keep generated source, documentation snippets, and `api/` listings
consistent with their authoritative inputs.

## Security Requirements

- **Secrets and fixtures:** Never commit, print, or embed API keys, access
  tokens, connection secrets, signing credentials, or NuGet publishing tokens.
  Read `OPENAI_API_KEY` from the environment or an approved secrets manager;
  use synthetic data, safe placeholders, and local mocked transports in examples,
  fixtures, tests, and generated artifacts.
- **Logs and recordings:** Redact authorization headers, credentials, signed
  URLs, and customer data in diagnostics, test output, exceptions, telemetry,
  and artifacts. Use synthetic prompts/model responses and sanitize sensitive
  request/response content in recordings. Most default recording sanitizers are
  disabled; explicitly sanitize sensitive fields and inspect every file in
  `tests/SessionRecords/` before committing it.
- **Dependencies and generators:** Review direct and transitive NuGet/npm
  changes, package provenance, trusted feeds and source mappings, local .NET
  tools, install scripts, and the root workspace `package-lock.json`.
  Use `npm ci` for reproducible installs and preserve security-related dependency
  overrides. Never change the `Moq` version pinned to `[4.18.2]` in
  `Directory.Packages.props` without legal approval. Keep
  `@typespec/http-client-csharp` and
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
  generated output, and signing/publishing changes. Agents must run tests only
  with `CLIENTMODEL_TEST_MODE=Playback` and
  `CLIENTMODEL_DISABLE_AUTO_RECORDING=true`; never run `Record` or `Live` mode
  or attempt to capture recordings with real credentials.
- **Vulnerability disclosure:** Report suspected vulnerabilities privately
  through [SECURITY.md](SECURITY.md) and the existing
  [OpenAI Bugcrowd program](https://bugcrowd.com/engagements/openai). Do not
  disclose vulnerabilities or secrets in public GitHub issues, pull requests,
  or discussions.
