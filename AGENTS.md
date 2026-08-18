# Agent Instructions

Read [CONTRIBUTING.md](CONTRIBUTING.md), [SECURITY.md](SECURITY.md), and the
[repository testing instructions](.github/skills/running-tests/SKILL.md) before
making changes.

## Security Requirements

- **Secrets and fixtures:** Never commit, print, or embed API keys, access
  tokens, connection secrets, signing credentials, or NuGet publishing tokens.
  Read `OPENAI_API_KEY` from the environment or an approved secrets manager;
  use synthetic data, safe placeholders, and local mocked transports in examples,
  fixtures, tests, and generated artifacts.
- **Logs and recordings:** Redact authorization headers, credentials, signed
  URLs, and customer data in diagnostics, test output, exceptions, telemetry,
  and artifacts. Use synthetic prompts/model responses. Sanitizers for known
  sensitive OpenAI headers and fields are enabled, but evolving service behavior
  can leave gaps. An authorized human must inspect every recording in
  `tests/SessionRecords/`. If sensitive data remains, add or improve an enabled
  deterministic sanitizer and regenerate the recording, or report the gap before
  publication. Never manually sanitize recordings. Do not use recording
  workflows that stage, commit, push, or upload files before an authorized human
  has reviewed them locally.
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
  generated output, and signing/publishing changes. Agents must run tests only
  with `CLIENTMODEL_TEST_MODE=Playback` and
  `CLIENTMODEL_DISABLE_AUTO_RECORDING=true`; never run `Record` or `Live` mode
  or attempt to capture recordings with real credentials.
- **Vulnerability disclosure:** Report suspected vulnerabilities privately
  through [SECURITY.md](SECURITY.md) and the existing
  [OpenAI Bugcrowd program](https://bugcrowd.com/engagements/openai). Do not
  disclose vulnerabilities or secrets in public GitHub issues, pull requests,
  or discussions.
