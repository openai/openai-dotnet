import "@typespec/http";
import "@typespec/openapi";

import "../common";
import "./models.tsp";

using TypeSpec.Http;
using TypeSpec.OpenAPI;

namespace OpenAI;

@route("fine_tuning")
@tag("Fine-tuning")
interface FineTuning {
  @get
  @route("checkpoints/{fine_tuned_model_checkpoint}/permissions")
  @operationId("listFineTuningCheckpointPermissions")
  @summary("""
    NOTE: This endpoint requires an admin API key.
    Organization owners can use this endpoint to view all permissions for a fine-tuned model checkpoint.
    """)
  listFineTuningCheckpointPermissions(
    /** The ID of the fine-tuned model checkpoint to get permissions for. */
    @path fine_tuned_model_checkpoint: string,

    ...PageAfterQueryParameter,
    ...PageLimitQueryParameter,
    ...PageOrderQueryParameter,

    /** The ID of the project to get permissions for. */
    @query project_id?: string,
  ): ListFineTuningCheckpointPermissionResponse | ErrorResponse;

  @post
  @route("checkpoints/{fine_tuned_model_checkpoint}/permissions")
  @operationId("createFineTuningCheckpointPermission")
  @summary("""
    NOTE: This endpoint requires an admin API key.
    This enables organization owners to share fine-tuned models with other projects in their organization.
    """)
  createFineTuningCheckpointPermission(
    /** The ID of the fine-tuned model checkpoint to create a permission for. */
    @path fine_tuned_model_checkpoint: string,

    ...CreateFineTuningCheckpointPermissionRequest,
  ): ListFineTuningCheckpointPermissionResponse | ErrorResponse;

  @delete
  @route("checkpoints/{fine_tuned_model_checkpoint}/permissions/{permission_id}")
  @operationId("deleteFineTuningCheckpointPermission")
  @summary("""
    NOTE: This endpoint requires an admin API key.
    Organization owners can use this endpoint to delete a permission for a fine-tuned model checkpoint.
    """)
  deleteFineTuningCheckpointPermission(
    /** The ID of the fine-tuned model checkpoint to delete a permission for. */
    @path fine_tuned_model_checkpoint: string,

    /** The ID of the permission to delete. */
    @path permission_id: string,
  ): DeleteFineTuningCheckpointPermissionResponse | ErrorResponse;

  @route("jobs")
  @post
  @operationId("createFineTuningJob")
  @summary("""
    Creates a fine-tuning job which begins the process of creating a new model from a given dataset.
    
    Response includes details of the enqueued job including job status and the name of the fine-tuned models once complete.
    
    [Learn more about fine-tuning](/docs/guides/fine-tuning)
    """)
  createFineTuningJob(
    @header accept: "application/json",
    @body requestBody: CreateFineTuningJobRequest,
  ): FineTuningJob | ErrorResponse;

  @route("jobs")
  @get
  @operationId("listPaginatedFineTuningJobs")
  @summary("List your organization's fine-tuning jobs")
  listPaginatedFineTuningJobs(
    @header accept: "application/json",

    /** Identifier for the last job from the previous pagination request. */
    @query after?: string,

    /** Number of fine-tuning jobs to retrieve. */
    @query limit?: int32 = 20,
  ): ListPaginatedFineTuningJobsResponse | ErrorResponse;

  @route("jobs/{fine_tuning_job_id}")
  @get
  @operationId("retrieveFineTuningJob")
  @summary("""
    Get info about a fine-tuning job.
    
    [Learn more about fine-tuning](/docs/guides/fine-tuning)
    """)
  retrieveFineTuningJob(
    @header accept: "application/json",

    /** The ID of the fine-tuning job. */
    @path fine_tuning_job_id: string,
  ): FineTuningJob | ErrorResponse;

  @route("jobs/{fine_tuning_job_id}/cancel")
  @post
  @operationId("cancelFineTuningJob")
  @summary("Immediately cancel a fine-tune job.")
  cancelFineTuningJob(
    @header accept: "application/json",

    /** The ID of the fine-tuning job to cancel. */
    @path fine_tuning_job_id: string,
  ): FineTuningJob | ErrorResponse;

  @route("jobs/{fine_tuning_job_id}/checkpoints")
  @get
  @operationId("listFineTuningJobCheckpoints")
  @summary("List the checkpoints for a fine-tuning job.")
  listFineTuningJobCheckpoints(
    @header accept: "application/json",

    /** The ID of the fine-tuning job to get checkpoints for. */
    @path fine_tuning_job_id: string,

    /** Identifier for the last checkpoint ID from the previous pagination request. */
    @query after?: string,

    /** Number of checkpoints to retrieve. */
    @query limit?: int32 = 10,
  ): ListFineTuningJobCheckpointsResponse | ErrorResponse;

  @route("jobs/{fine_tuning_job_id}/events")
  @get
  @operationId("listFineTuningEvents")
  @summary("Get status updates for a fine-tuning job.")
  listFineTuningEvents(
    @header accept: "application/json",

    /** The ID of the fine-tuning job to get events for. */
    @path fine_tuning_job_id: string,

    /** Identifier for the last event from the previous pagination request. */
    @query after?: string,

    /** Number of events to retrieve. */
    @query limit?: int32 = 20,
  ): ListFineTuningJobEventsResponse | ErrorResponse;

  @post
  @route("jobs/{fine_tuning_job_id}/pause")
  @operationId("pauseFineTuningJob")
  @summary("""
    Pause a fine-tune job.
    """)
  pauseFineTuningJob(
    /** The ID of the fine-tuning job to pause. */
    @path fine_tuning_job_id: string,
  ): FineTuningJob | ErrorResponse;

  @post
  @route("jobs/{fine_tuning_job_id}/resume")
  @operationId("resumeFineTuningJob")
  @summary("""
    Resume a paused fine-tune job.
    """)
  resumeFineTuningJob(
    /** The ID of the fine-tuning job to resume. */
    @path fine_tuning_job_id: string,
  ): FineTuningJob | ErrorResponse;
}
