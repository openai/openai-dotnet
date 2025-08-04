import "@typespec/http";
import "@typespec/openapi";

import "../common";
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
    ...AcceptJsonOrEventStreamHeader,
    ...AssistantsBetaHeader,
    @body requestBody: CreateThreadAndRunRequest,
  ): RunObject | ErrorResponse;

  @route("{thread_id}/runs")
  @post
  @operationId("createRun")
  @tag("Assistants")
  @summary("Create a run.")
  createRun(
    ...AcceptJsonOrEventStreamHeader,
    ...AssistantsBetaHeader,

    /** The ID of the thread to run. */
    @path thread_id: string,

    ...IncludedRunStepPropertyParameter,
    @body requestBody: CreateRunRequest,
  ): RunObject | ErrorResponse;

  @route("{thread_id}/runs")
  @get
  @operationId("listRuns")
  @tag("Assistants")
  @summary("Returns a list of runs belonging to a thread.")
  listRuns(
    ...AcceptJsonHeader,
    ...AssistantsBetaHeader,

    /** The ID of the thread the run belongs to. */
    @path thread_id: string,

    ...CommonPageQueryParameters,
  ): ListRunsResponse | ErrorResponse;

  @route("{thread_id}/runs/{run_id}")
  @get
  @operationId("getRun")
  @tag("Assistants")
  @summary("Retrieves a run.")
  getRun(
    ...AcceptJsonHeader,
    ...AssistantsBetaHeader,

    /** The ID of the [thread](/docs/api-reference/threads) that was run. */
    @path thread_id: string,

    /** The ID of the run to retrieve. */
    @path run_id: string,
  ): RunObject | ErrorResponse;

  @route("{thread_id}/runs/{run_id}")
  @post
  @operationId("modifyRun")
  @tag("Assistants")
  @summary("Modifies a run.")
  modifyRun(
    ...AcceptJsonHeader,
    ...AssistantsBetaHeader,

    /** The ID of the [thread](/docs/api-reference/threads) that was run. */
    @path thread_id: string,

    /** The ID of the run to modify. */
    @path run_id: string,

    @body requestBody: ModifyRunRequest,
  ): RunObject | ErrorResponse;

  @route("{thread_id}/runs/{run_id}/cancel")
  @post
  @operationId("cancelRun")
  @tag("Assistants")
  @summary("Cancels a run that is `in_progress`.")
  cancelRun(
    ...AcceptJsonHeader,
    ...AssistantsBetaHeader,

    /** The ID of the thread to which this run belongs. */
    @path thread_id: string,

    /** The ID of the run to cancel. */
    @path run_id: string,
  ): RunObject | ErrorResponse;

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
    ...AcceptJsonOrEventStreamHeader,
    ...AssistantsBetaHeader,

    /** The ID of the [thread](/docs/api-reference/threads) to which this run belongs. */
    @path thread_id: string,

    /** The ID of the run that requires the tool output submission. */
    @path run_id: string,

    @body requestBody: SubmitToolOutputsRunRequest,
  ): RunObject | ErrorResponse;

  @route("{thread_id}/runs/{run_id}/steps")
  @get
  @operationId("listRunSteps")
  @tag("Assistants")
  @summary("Returns a list of run steps belonging to a run.")
  listRunSteps(
    ...AcceptJsonHeader,
    ...AssistantsBetaHeader,

    /** The ID of the thread the run and run steps belong to. */
    @path thread_id: string,

    /** The ID of the run the run steps belong to. */
    @path run_id: string,

    ...CommonPageQueryParameters,
    ...IncludedRunStepPropertyParameter,
  ): ListRunStepsResponse | ErrorResponse;

  @route("{thread_id}/runs/{run_id}/steps/{step_id}")
  @get
  @operationId("getRunStep")
  @tag("Assistants")
  @summary("Retrieves a run step.")
  getRunStep(
    ...AcceptJsonHeader,
    ...AssistantsBetaHeader,

    /** The ID of the thread to which the run and run step belongs. */
    @path thread_id: string,

    /** The ID of the run to which the run step belongs. */
    @path run_id: string,

    /** The ID of the run step to retrieve. */
    @path step_id: string,

    ...IncludedRunStepPropertyParameter,
  ): RunStepObject | ErrorResponse;
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
