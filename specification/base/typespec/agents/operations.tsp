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
): AgentResource;

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
