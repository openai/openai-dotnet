import "@typespec/http";
import "@typespec/openapi";

import "./models.tsp";
import "./models_items.tsp";
import "./models_streaming.tsp";
import "./models_tools.tsp";

using TypeSpec.Http;
using TypeSpec.OpenAPI;

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
