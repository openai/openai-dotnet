import "@typespec/http";
import "@typespec/openapi";
import "@typespec/sse";

import "./models.tsp";
import "./models_items.tsp";
import "./models_streaming.tsp";
import "./models_tools.tsp";
import "./models_vaults.tsp";

using TypeSpec.Http;
using TypeSpec.OpenAPI;
using TypeSpec.SSE;

namespace OpenAI;

/**
 * Creates a reusable agent without storing credentials. See
 * [agent configuration](https://developers.openai.com/api/docs/guides/agents-api/configuration).
 */
@summary("Create an agent")
@post
@route("agents")
@tag("Agents")
op createAgent(
  /** Parameters for creating a reusable agent. */
  @body
  agent: CreateAgentParams,
): {
  @statusCode statusCode: 201;

  /** The created agent. */
  @body agent: AgentResource;
};

/**
 * Retrieves a reusable agent by ID. See
 * [agent configuration](https://developers.openai.com/api/docs/guides/agents-api/configuration).
 */
@summary("Retrieve an agent")
@get
@route("agents/{agent_id}")
@tag("Agents")
op retrieveAgent(
  /** The ID of the reusable agent. */
  @path
  agent_id: string,
): AgentResource;

/**
 * Updates a reusable agent by ID. See
 * [agent configuration](https://developers.openai.com/api/docs/guides/agents-api/configuration).
 */
@summary("Update an agent")
@post
@route("agents/{agent_id}")
@tag("Agents")
op updateAgent(
  /** The ID of the reusable agent. */
  @path
  agent_id: string,

  /** Fields to replace on the existing reusable agent. */
  @body
  agent: UpdateAgentParams,
): AgentResource;

/**
 * Deletes a reusable agent by ID. See
 * [agent configuration](https://developers.openai.com/api/docs/guides/agents-api/configuration).
 */
@summary("Delete an agent")
@delete
@route("agents/{agent_id}")
@tag("Agents")
op deleteAgent(
  /** The ID of the reusable agent. */
  @path
  agent_id: string,
): DeletedAgentResource;

/**
 * Lists reusable agents in the current project. See
 * [agent configuration](https://developers.openai.com/api/docs/guides/agents-api/configuration).
 */
@summary("List agents")
@get
@route("agents")
@tag("Agents")
@list
op listAgents(
  ...AgentCollectionOptions,
): AgentCollection;

/**
 * Creates a Managed Agents session. Streaming is not yet modeled; this returns the session.
 * See [running sessions](https://developers.openai.com/api/docs/guides/agents-api/sessions).
 */
@summary("Create an agent session")
@post
@route("agents/sessions")
@tag("Agents")
op createAgentSession(
  /** Parameters for creating a Managed Agents session. */
  @body
  session: CreateAgentSessionParams,
): {
  @statusCode statusCode: 201;

  /** The created session. */
  @body session: SessionResource;
};

/**
 * Retrieves the current state of a Managed Agents session. See
 * [managing sessions](https://developers.openai.com/api/docs/guides/agents-api/sessions/manage).
 */
@summary("Retrieve an agent session")
@get
@route("agents/sessions/{session_id}")
@tag("Agents")
op retrieveAgentSession(
  /** The ID of the session. */
  @path
  session_id: string,
): SessionResource;

/**
 * Updates session metadata. Omitted fields are unchanged. See
 * [managing sessions](https://developers.openai.com/api/docs/guides/agents-api/sessions/manage).
 */
@summary("Update an agent session")
@post
@route("agents/sessions/{session_id}")
@tag("Agents")
op updateAgentSession(
  /** The ID of the session. */
  @path
  session_id: string,

  /** Fields to update on the existing session. */
  @body
  session: UpdateAgentSessionParams,
): SessionResource;

/**
 * Removes a Managed Agents session from the public API. See
 * [managing sessions](https://developers.openai.com/api/docs/guides/agents-api/sessions/manage).
 */
@summary("Delete an agent session")
@delete
@route("agents/sessions/{session_id}")
@tag("Agents")
op deleteAgentSession(
  /** The ID of the session. */
  @path
  session_id: string,
): DeletedSessionResource;

/**
 * Lists Managed Agents sessions using ID-based pagination. See
 * [managing sessions](https://developers.openai.com/api/docs/guides/agents-api/sessions/manage).
 */
@summary("List agent sessions")
@get
@route("agents/sessions")
@tag("Agents")
@list
op listAgentSessions(
  ...AgentSessionCollectionOptions,
): SessionListResource;

/**
 * Lists artifacts published by a session's completed turns. See
 * [managing sessions](https://developers.openai.com/api/docs/guides/agents-api/sessions/manage).
 */
@summary("List agent session artifacts")
@get
@route("agents/sessions/{session_id}/artifacts")
@tag("Agents")
@list
op listAgentSessionArtifacts(
  ...AgentSessionArtifactCollectionOptions,
): SessionArtifactListResource;

/**
 * Retrieves a single session artifact's metadata. See
 * [managing sessions](https://developers.openai.com/api/docs/guides/agents-api/sessions/manage).
 */
@summary("Retrieve an agent session artifact")
@get
@route("agents/sessions/{session_id}/artifacts/{artifact_id}")
@tag("Agents")
op retrieveAgentSessionArtifact(
  /** The ID of the session that owns the artifact. */
  @path
  session_id: string,

  /** The immutable session artifact ID. */
  @path
  artifact_id: string,
): SessionArtifactResource;

/**
 * Deletes a single session artifact. See
 * [managing sessions](https://developers.openai.com/api/docs/guides/agents-api/sessions/manage).
 */
@summary("Delete an agent session artifact")
@delete
@route("agents/sessions/{session_id}/artifacts/{artifact_id}")
@tag("Agents")
op deleteAgentSessionArtifact(
  /** The ID of the session that owns the artifact. */
  @path
  session_id: string,

  /** The immutable session artifact ID. */
  @path
  artifact_id: string,
): DeletedSessionArtifactResource;

/**
 * Downloads the immutable contents of a session artifact. See
 * [managing sessions](https://developers.openai.com/api/docs/guides/agents-api/sessions/manage).
 */
@summary("Retrieve agent session artifact content")
@get
@route("agents/sessions/{session_id}/artifacts/{artifact_id}/content")
@tag("Agents")
op retrieveAgentSessionArtifactContent(
  /** The ID of the session that owns the artifact. */
  @path
  session_id: string,

  /** The immutable session artifact ID. */
  @path
  artifact_id: string,
): {
  @header contentType: "application/octet-stream";

  /** The immutable artifact contents. */
  @body content: bytes;
};

/**
 * Lists turns in a session using ID-based pagination. See
 * [managing sessions](https://developers.openai.com/api/docs/guides/agents-api/sessions/manage).
 */
@summary("List agent session turns")
@get
@route("agents/sessions/{session_id}/turns")
@tag("Agents")
@list
op listAgentSessionTurns(
  ...AgentSessionTurnCollectionOptions,
): SessionTurnListResource;

/**
 * Retrieves a single turn in a session. See
 * [managing sessions](https://developers.openai.com/api/docs/guides/agents-api/sessions/manage).
 */
@summary("Retrieve an agent session turn")
@get
@route("agents/sessions/{session_id}/turns/{turn_id}")
@tag("Agents")
op retrieveAgentSessionTurn(
  /** The ID of the session that owns the turn. */
  @path
  session_id: string,

  /** The ID of the turn. */
  @path
  turn_id: string,
): TurnResource;

/**
 * Lists items in a session using ID-based pagination. See
 * [managing sessions](https://developers.openai.com/api/docs/guides/agents-api/sessions/manage).
 */
@summary("List agent session items")
@get
@route("agents/sessions/{session_id}/items")
@tag("Agents")
@list
op listAgentSessionItems(
  ...AgentSessionItemCollectionOptions,
): SessionItemListResource;

/**
 * Lists subagents in a session, including nested and closed subagents. See
 * [subagent workflows](https://developers.openai.com/api/docs/guides/agents-api/multi-agent).
 */
@summary("List session subagents")
@get
@route("agents/sessions/{session_id}/subagents")
@tag("Agents")
@list
op listAgentSessionSubagents(
  ...AgentSessionSubagentCollectionOptions,
): SubagentListResource;

/**
 * Retrieves a subagent belonging to a session. See
 * [subagent workflows](https://developers.openai.com/api/docs/guides/agents-api/multi-agent).
 */
@summary("Retrieve a session subagent")
@get
@route("agents/sessions/{session_id}/subagents/{subagent_id}")
@tag("Agents")
op retrieveAgentSessionSubagent(
  /** The ID of the session that owns the subagent. */
  @path
  session_id: string,

  /** The ID of the subagent in this session. */
  @path
  subagent_id: string,
): SubagentResource;

/**
 * Lists a subagent's own items across all of its turns. See
 * [subagent workflows](https://developers.openai.com/api/docs/guides/agents-api/multi-agent).
 */
@summary("List subagent items")
@get
@route("agents/sessions/{session_id}/subagents/{subagent_id}/items")
@tag("Agents")
@list
op listAgentSessionSubagentItems(
  ...AgentSessionSubagentItemCollectionOptions,
): SessionItemListResource;

/**
 * Lists a subagent's turns. See
 * [subagent workflows](https://developers.openai.com/api/docs/guides/agents-api/multi-agent).
 */
@summary("List subagent turns")
@get
@route("agents/sessions/{session_id}/subagents/{subagent_id}/turns")
@tag("Agents")
@list
op listAgentSessionSubagentTurns(
  ...AgentSessionSubagentTurnCollectionOptions,
): SessionTurnListResource;

/**
 * Retrieves a single turn belonging to a subagent. See
 * [subagent workflows](https://developers.openai.com/api/docs/guides/agents-api/multi-agent).
 */
@summary("Retrieve a subagent turn")
@get
@route("agents/sessions/{session_id}/subagents/{subagent_id}/turns/{turn_id}")
@tag("Agents")
op retrieveAgentSessionSubagentTurn(
  /** The ID of the session that owns the subagent. */
  @path
  session_id: string,

  /** The ID of the subagent in this session. */
  @path
  subagent_id: string,

  /** The ID of the turn. */
  @path
  turn_id: string,
): TurnResource;

/**
 * Lists the items in a single subagent turn. See
 * [subagent workflows](https://developers.openai.com/api/docs/guides/agents-api/multi-agent).
 */
@summary("List subagent turn items")
@get
@route("agents/sessions/{session_id}/subagents/{subagent_id}/turns/{turn_id}/items")
@tag("Agents")
@list
op listAgentSessionSubagentTurnItems(
  ...AgentSessionSubagentTurnItemCollectionOptions,
): SessionItemListResource;

/**
 * Streams live events for an agent session as server-sent events. See
 * [session events](https://developers.openai.com/api/docs/guides/agents-api/sessions/events).
 */
@summary("Stream agent session events")
@get
@route("agents/sessions/{session_id}/events")
@tag("Agents")
op listAgentSessionEvents(
  /** The ID of the session. */
  @path
  session_id: string,
): SSEStream<SessionEvent>;

/**
 * Submits message, cancellation, or tool-result events to a managed agent session. See
 * [session events](https://developers.openai.com/api/docs/guides/agents-api/sessions/events).
 */
@summary("Create agent session input events")
@post
@route("agents/sessions/{session_id}/events")
@tag("Agents")
op createAgentSessionEvents(
  /** The ID of the session. */
  @path
  session_id: string,

  /** An optional client-generated key that makes retries of submitted messages idempotent. */
  @header("Idempotency-Key")
  idempotencyKey?: string,

  /** The input events to submit to the session. */
  @body
  events: CreateSessionEventsParams,
): {
  @statusCode statusCode: 202;
};

/**
 * Lists vaults using ID-based pagination. See
 * [vaults](https://developers.openai.com/api/docs/guides/agents-api/tools/vaults).
 */
@summary("List vaults")
@get
@route("vaults")
@tag("Vaults")
@list
op listVaults(
  ...VaultCollectionOptions,
): VaultListResource;

/**
 * Creates a vault to store credentials used by agent tools. See
 * [vaults](https://developers.openai.com/api/docs/guides/agents-api/tools/vaults).
 */
@summary("Create a vault")
@post
@route("vaults")
@tag("Vaults")
op createVault(
  /** Parameters for creating the vault. */
  @body
  vault: CreateVaultParams,
): {
  @statusCode statusCode: 201;

  /** The created vault. */
  @body vault: VaultResource;
};

/**
 * Retrieves a vault by its ID. See
 * [vaults](https://developers.openai.com/api/docs/guides/agents-api/tools/vaults).
 */
@summary("Retrieve a vault")
@get
@route("vaults/{vault_id}")
@tag("Vaults")
op retrieveVault(
  /** The ID of the vault. */
  @path
  vault_id: string,
): VaultResource;

/**
 * Deletes a vault and all its credentials. See
 * [vaults](https://developers.openai.com/api/docs/guides/agents-api/tools/vaults).
 */
@summary("Delete a vault")
@delete
@route("vaults/{vault_id}")
@tag("Vaults")
op deleteVault(
  /** The ID of the vault. */
  @path
  vault_id: string,
): DeletedVaultResource;

/**
 * Lists a vault's credentials using ID-based pagination without returning secret values. See
 * [vaults](https://developers.openai.com/api/docs/guides/agents-api/tools/vaults).
 */
@summary("List vault credentials")
@get
@route("vaults/{vault_id}/credentials")
@tag("Vaults")
@list
op listVaultCredentials(
  ...VaultCredentialCollectionOptions,
): VaultCredentialListResource;

/**
 * Creates a vault credential. Secret values are write-only and are never returned. See
 * [vaults](https://developers.openai.com/api/docs/guides/agents-api/tools/vaults).
 */
@summary("Create a vault credential")
@post
@route("vaults/{vault_id}/credentials")
@tag("Vaults")
op createVaultCredential(
  /** The ID of the vault. */
  @path
  vault_id: string,

  /** Parameters for creating the credential. */
  @body
  credential: CreateVaultCredentialParams,
): {
  @statusCode statusCode: 201;

  /** The created vault credential without secret values. */
  @body credential: VaultCredentialResource;
};

/**
 * Retrieves vault credential metadata without returning secret values. See
 * [vaults](https://developers.openai.com/api/docs/guides/agents-api/tools/vaults).
 */
@summary("Retrieve a vault credential")
@get
@route("vaults/{vault_id}/credentials/{credential_id}")
@tag("Vaults")
op retrieveVaultCredential(
  /** The ID of the vault. */
  @path
  vault_id: string,

  /** The ID of the vault credential. */
  @path
  credential_id: string,
): VaultCredentialResource;

/**
 * Rotates a vault credential's secret values without changing its authentication method. See
 * [vaults](https://developers.openai.com/api/docs/guides/agents-api/tools/vaults).
 */
@summary("Rotate a vault credential")
@post
@route("vaults/{vault_id}/credentials/{credential_id}")
@tag("Vaults")
op rotateVaultCredential(
  /** The ID of the vault. */
  @path
  vault_id: string,

  /** The ID of the vault credential. */
  @path
  credential_id: string,

  /** Replacement secret values for the credential. */
  @body
  credential: RotateVaultCredentialParams,
): VaultCredentialResource;

/**
 * Deletes a vault credential. See
 * [vaults](https://developers.openai.com/api/docs/guides/agents-api/tools/vaults).
 */
@summary("Delete a vault credential")
@delete
@route("vaults/{vault_id}/credentials/{credential_id}")
@tag("Vaults")
op deleteVaultCredential(
  /** The ID of the vault. */
  @path
  vault_id: string,

  /** The ID of the vault credential. */
  @path
  credential_id: string,
): DeletedVaultCredentialResource;
