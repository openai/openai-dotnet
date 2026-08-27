import "@typespec/http";
import "@typespec/openapi";

import "./models.tsp";

using TypeSpec.Http;
using TypeSpec.OpenAPI;

namespace OpenAI;

@route("/threads")
interface Runs {
  @route("runs")
  @post
  @operationId("createThreadAndRun")
  @tag("Assistants")
  @summary("Create a thread and run it in one request.")
  createThreadAndRun(
    @header accept: "application/json" | "text/event-stream",
    @header("OpenAI-Beta") openAIBeta: "assistants=v2",
    @body requestBody: CreateThreadAndRunRequest,
  ): RunObject | RunsErrorResponse;

  @route("{thread_id}/runs")
  @post
  @operationId("createRun")
  @tag("Assistants")
  @summary("Create a run.")
  createRun(
    @header accept: "application/json" | "text/event-stream",
    @header("OpenAI-Beta") openAIBeta: "assistants=v2",

    /** The ID of the thread to run. */
    @path thread_id: string,

    ...IncludedRunStepPropertyParameter,
    @body requestBody: CreateRunRequest,
  ): RunObject | RunsErrorResponse;

  @route("{thread_id}/runs")
  @get
  @operationId("listRuns")
  @tag("Assistants")
  @summary("Returns a list of runs belonging to a thread.")
  @list
  listRuns(
    @header accept: "application/json",
    @header("OpenAI-Beta") openAIBeta: "assistants=v2",

    /** The ID of the thread the run belongs to. */
    @path thread_id: string,

    /**
   * A limit on the number of objects to be returned. Limit can range between 1 and 100, and the
   * default is 20.
   */
  @query limit?: int32 = 20,
  /**
   * Sort order by the `created_at` timestamp of the objects. `asc` for ascending order and`desc`
   * for descending order.
   */
  @query order?: RunCollectionOrder,
  /**
   * A cursor for use in pagination. `after` is an object ID that defines your place in the list.
   * For instance, if you make a list request and receive 100 objects, ending with obj_foo, your
   * subsequent call can include after=obj_foo in order to fetch the next page of the list.
   */
  @query @continuationToken after?: string,
  /**
   * A cursor for use in pagination. `before` is an object ID that defines your place in the list.
   * For instance, if you make a list request and receive 100 objects, ending with obj_foo, your
   * subsequent call can include before=obj_foo in order to fetch the previous page of the list.
   */
  @query before?: string,
  ): ListRunsResponse | RunsErrorResponse;

  @route("{thread_id}/runs/{run_id}")
  @get
  @operationId("getRun")
  @tag("Assistants")
  @summary("Retrieves a run.")
  getRun(
    @header accept: "application/json",
    @header("OpenAI-Beta") openAIBeta: "assistants=v2",

    /** The ID of the [thread](/docs/api-reference/threads) that was run. */
    @path thread_id: string,

    /** The ID of the run to retrieve. */
    @path run_id: string,
  ): RunObject | RunsErrorResponse;

  @route("{thread_id}/runs/{run_id}")
  @post
  @operationId("modifyRun")
  @tag("Assistants")
  @summary("Modifies a run.")
  modifyRun(
    @header accept: "application/json",
    @header("OpenAI-Beta") openAIBeta: "assistants=v2",

    /** The ID of the [thread](/docs/api-reference/threads) that was run. */
    @path thread_id: string,

    /** The ID of the run to modify. */
    @path run_id: string,

    @body requestBody: ModifyRunRequest,
  ): RunObject | RunsErrorResponse;

  @route("{thread_id}/runs/{run_id}/cancel")
  @post
  @operationId("cancelRun")
  @tag("Assistants")
  @summary("Cancels a run that is `in_progress`.")
  cancelRun(
    @header accept: "application/json",
    @header("OpenAI-Beta") openAIBeta: "assistants=v2",

    /** The ID of the thread to which this run belongs. */
    @path thread_id: string,

    /** The ID of the run to cancel. */
    @path run_id: string,
  ): RunObject | RunsErrorResponse;

  @route("{thread_id}/runs/{run_id}/submit_tool_outputs")
  @post
  @operationId("submitToolOutputsToRun")
  @tag("Assistants")
  @summary("""
    When a run has the `status: "requires_action"` and `required_action.type` is
    `submit_tool_outputs`, this endpoint can be used to submit the outputs from the tool calls once
    they're all completed. All outputs must be submitted in a single request.
    """)
  submitToolOutputsToRun(
    @header accept: "application/json" | "text/event-stream",
    @header("OpenAI-Beta") openAIBeta: "assistants=v2",

    /** The ID of the [thread](/docs/api-reference/threads) to which this run belongs. */
    @path thread_id: string,

    /** The ID of the run that requires the tool output submission. */
    @path run_id: string,

    @body requestBody: SubmitToolOutputsRunRequest,
  ): RunObject | RunsErrorResponse;

  @route("{thread_id}/runs/{run_id}/steps")
  @get
  @operationId("listRunSteps")
  @tag("Assistants")
  @summary("Returns a list of run steps belonging to a run.")
  @list
  listRunSteps(
    @header accept: "application/json",
    @header("OpenAI-Beta") openAIBeta: "assistants=v2",

    /** The ID of the thread the run and run steps belong to. */
    @path thread_id: string,

    /** The ID of the run the run steps belong to. */
    @path run_id: string,

    /**
   * A limit on the number of objects to be returned. Limit can range between 1 and 100, and the
   * default is 20.
   */
  @query limit?: int32 = 20,
  /**
   * Sort order by the `created_at` timestamp of the objects. `asc` for ascending order and`desc`
   * for descending order.
   */
  @query order?: RunCollectionOrder,
  /**
   * A cursor for use in pagination. `after` is an object ID that defines your place in the list.
   * For instance, if you make a list request and receive 100 objects, ending with obj_foo, your
   * subsequent call can include after=obj_foo in order to fetch the next page of the list.
   */
  @query @continuationToken after?: string,
  /**
   * A cursor for use in pagination. `before` is an object ID that defines your place in the list.
   * For instance, if you make a list request and receive 100 objects, ending with obj_foo, your
   * subsequent call can include before=obj_foo in order to fetch the previous page of the list.
   */
  @query before?: string,
    ...IncludedRunStepPropertyParameter,
  ): ListRunStepsResponse | RunsErrorResponse;

  @route("{thread_id}/runs/{run_id}/steps/{step_id}")
  @get
  @operationId("getRunStep")
  @tag("Assistants")
  @summary("Retrieves a run step.")
  getRunStep(
    @header accept: "application/json",
    @header("OpenAI-Beta") openAIBeta: "assistants=v2",

    /** The ID of the thread to which the run and run step belongs. */
    @path thread_id: string,

    /** The ID of the run to which the run step belongs. */
    @path run_id: string,

    /** The ID of the run step to retrieve. */
    @path step_id: string,

    ...IncludedRunStepPropertyParameter,
  ): RunStepObject | RunsErrorResponse;
}

alias IncludedRunStepPropertyParameter = {
  /**
   * A list of additional fields to include in the response. Currently the only supported value is
   * `step_details.tool_calls[*].file_search.results[*].content` to fetch the file search result
   * content.
   *
   * See the
   * [file search tool documentation](/docs/assistants/tools/file-search/customizing-file-search-settings)
   * for more information.
   */
  @query `include[]`?: IncludedRunStepProperty[];
};
