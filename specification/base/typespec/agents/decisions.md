# Agents API — Decisions & Naming Ledger

Running log of decision points, spec divergences, and name mappings made while translating the
OpenAI Agents API into this SDK's TypeSpec. Add a new entry whenever a decision is made.

## Decision log

### D1 — Create operations must expect HTTP 201
- **Context:** The spec marks `createAgent` (and `createAgentSession`) responses as `201 Created`.
  Returning the resource type directly from a `@post` in TypeSpec defaults the generated success
  classifier to `200`, so the client would treat the server's real `201` as an error and throw.
- **Decision:** Model these create ops with an explicit `{ @statusCode statusCode: 201; @body ... }`
  response so the generated `PipelineMessageClassifier` expects `201`.
- **Status:** Applied to `createAgent`. Will apply to `createAgentSession`.

### D2 — Pagination modeled as the SDK collection pattern
- **Context:** The spec exposes list endpoints as `*ListResource` + inline `limit`/`order`/`after`
  query params.
- **Decision:** Model as the established SDK pattern — a `*CollectionOptions` input model plus a
  `*CollectionPage` page model with `@list` — matching Containers/VectorStores/etc. The HTTP wire
  contract is unchanged; only the .NET surface differs.

### D3 — `createAgentSession` streaming deferred
- **Context:** `createAgentSession` is an SSE operation (`x-oai-streaming`) that can return a
  `SessionEvent` stream (30+ discriminated variants that also reference turns, items, content parts,
  and reasoning-summary deltas).
- **Decision:** Model it non-streaming for now (returns `AgentSession`). Defer the SSE `SessionEvent`
  hierarchy to a dedicated later pass.

### D4 — Environment/turns/items *operations* deferred (models included)
- **Context:** Session models require the Environment (and input/session-tool) models, but the spec
  also has separate environment, environment-template, environment-file, turns, items, artifacts,
  events, and subagent operations.
- **Decision:** Model the dependency *models* needed by the session CRUD; defer those additional
  *operations* to later passes.

### D5 — Naming convention
- **Decision:** .NET public names use `Agent*` / `AgentSession*` prefixes and drop the spec's
  `*Resource` / `*Params` suffixes **for the primary user-facing types only**. Supporting/nested
  models keep their spec names (including `*Resource` / `*Param` suffixes), matching how the reusable
  Agents area kept `ReasoningResource`, `PersistedAgentToolConfigParam`, etc. See the rename tables.

### D6 — Session `environment` kept opaque (BinaryData)
- **Context:** `SessionResource.environment` / `CreateAgentSessionParams.environment` reference the
  Environment subsystem (`none`/`openai_hosted`/`self_hosted`), and `openai_hosted` branches into
  packages, network policy, skills, plugins, and files — a ~50-70 model subsystem.
- **Decision:** Model the `environment` field as `unknown` (surfaces as `BinaryData`) for now, so the
  session CRUD works without the Environment subsystem. The rest of the session graph (agent config,
  tools, input, status, required actions, token usage) is modeled fully. Environment models are a
  dedicated later pass.

## Renames — Reusable Agents

| Spec name | .NET name | Mechanism |
|---|---|---|
| `CreateAgentParams` | `AgentCreationOptions` | `@@clientName` |
| `UpdateAgentParams` | `AgentModificationOptions` | `@@clientName` |
| `AgentResource` | `Agent` | `@@clientName` |
| `DeletedAgentResource` | `AgentDeletionResult` | `@@clientName` |
| `DeletedAgentResource.id` | `AgentId` | `@@clientName` |
| `AgentListResource` | `AgentCollectionPage` | modeled as tsp `AgentCollection`, renamed in stub |
| (inline list query params) | `AgentCollectionOptions` | tsp options model |
| `ListOrderParam` | `AgentCollectionOrder` (`asc`→`Ascending`, `desc`→`Descending`) | tsp union + `@@clientName` |
| generated client `Agents` | `AgentClient` | custom stub rename |
| `listAgents` (op) | `GetAgents` (method) | emitter `@list` convention |

## Renames — Agent Sessions

Using the `AgentSession*` prefix. (Filled in as implemented.)

| Spec name | .NET name | Mechanism |
|---|---|---|
| `SessionResource` | `AgentSession` | `@@clientName` |
| `CreateAgentSessionParams` | `AgentSessionCreationOptions` | `@@clientName` |
| `UpdateAgentSessionParams` | `AgentSessionModificationOptions` | `@@clientName` |
| `DeletedSessionResource` | `AgentSessionDeletionResult` | `@@clientName` |
| `DeletedSessionResource.id` | `AgentSessionId` | `@@clientName` |
| `SessionListResource` | `AgentSessionCollectionPage` | tsp `SessionListResource` collection + stub rename |
| (inline session list query params) | `AgentSessionCollectionOptions` | tsp options model |
| `listAgentSessions` (op) | `GetAgentSessions` (method) | emitter `@list` convention |

Supporting/nested session models keep their spec names (e.g. `SessionStatusResource`,
`SessionRequiredActionResource`, `SessionAgentResource`, `SessionAgentConfigParam`,
`TokenUsageResource`, `AgentToolResource`, `AgentToolConfigParam`, `McpTransportResource`,
`McpTransportConfigParam`, `InputMessageParam`, `InputContentParam`). The `environment` field is
modeled as `BinaryData` (see D6).
