/*
 * This file was automatically generated from an OpenAPI .yaml file.
 * Edits made directly to this file will be lost.
 */

import "../common";
import "../messages";
import "./custom.tsp";

using Http;
using TypeSpec.OpenAPI;

namespace OpenAI;

/**
 * Options to create a new thread. If no thread is provided when running a
 * request, an empty thread will be created.
 */
model CreateThreadRequest {
  /** A list of [messages](/docs/api-reference/messages) to start the thread with. */
  messages?: CreateMessageRequest[];

  @doc("""
    A set of resources that are made available to the assistant's tools in this thread. The resources are specific to the type of tool. For example, the `code_interpreter` tool requires a list of file IDs, while the `file_search` tool requires a list of vector store IDs.
    """)
  tool_resources?: {
    code_interpreter?: {
      @doc("""
        A list of [file](/docs/api-reference/files) IDs made available to the `code_interpreter` tool. There can be a maximum of 20 files associated with the tool.
        """)
      @maxItems(20)
      file_ids?: string[] = #[];
    };

    // Tool customization: use custom type for sophisticated union
    file_search?: ToolResourcesFileSearch;
  } | null;

  ...MetadataPropertyForRequest;
}

model ModifyThreadRequest {
  @doc("""
    A set of resources that are made available to the assistant's tools in this thread. The resources are specific to the type of tool. For example, the `code_interpreter` tool requires a list of file IDs, while the `file_search` tool requires a list of vector store IDs.
    """)
  tool_resources?: {
    code_interpreter?: {
      @doc("""
        A list of [file](/docs/api-reference/files) IDs made available to the `code_interpreter` tool. There can be a maximum of 20 files associated with the tool.
        """)
      @maxItems(20)
      file_ids?: string[] = #[];
    };

    // Tool customization: use custom type for sophisticated union
    file_search?: ToolResourcesFileSearchIdsOnly;
  } | null;

  ...MetadataPropertyForRequest;
}

model DeleteThreadResponse {
  id: string;
  deleted: boolean;
  object: "thread.deleted";
}

/** Represents a thread that contains [messages](/docs/api-reference/messages). */
model ThreadObject {
  /** The identifier, which can be referenced in API endpoints. */
  id: string;

  @doc("""
    The object type, which is always `thread`.
    """)
  object: "thread";

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) for when the thread was created. */
  @encode("unixTimestamp", int32)
  created_at: utcDateTime;

  @doc("""
    A set of resources that are made available to the assistant's tools in this thread. The resources are specific to the type of tool. For example, the `code_interpreter` tool requires a list of file IDs, while the `file_search` tool requires a list of vector store IDs.
    """)
  tool_resources: {
    code_interpreter?: {
      @doc("""
        A list of [file](/docs/api-reference/files) IDs made available to the `code_interpreter` tool. There can be a maximum of 20 files associated with the tool.
        """)
      @maxItems(20)
      file_ids?: string[] = #[];
    };
    file_search?: {
      /** The [vector store](/docs/api-reference/vector-stores/object) attached to this thread. There can be a maximum of 1 vector store attached to the thread. */
      @maxItems(1)
      vector_store_ids?: string[];
    };
  } | null;

  ...MetadataPropertyForResponse;
}
