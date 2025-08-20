/*
 * This file was automatically generated from an OpenAPI .yaml file.
 * Edits made directly to this file will be lost.
 */

using Http;
using TypeSpec.OpenAPI;

namespace OpenAI;

model ListModelsResponse {
  object: "list";
  data: Model[];
}

model DeleteModelResponse {
  id: string;
  deleted: boolean;

  // Tool customization: add a clear enum enforcement of constrained 'object' label
  object: "model";
}

/** Describes an OpenAI model offering that can be used with the API. */
model Model {
  /** The model identifier, which can be referenced in the API endpoints. */
  id: string;

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) when the model was created. */
  @encode("unixTimestamp", int32)
  created: utcDateTime;

  /** The object type, which is always "model". */
  object: "model";

  /** The organization that owns the model. */
  owned_by: string;
}
