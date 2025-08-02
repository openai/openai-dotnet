/*
 * This file was automatically generated from an OpenAPI .yaml file.
 * Edits made directly to this file will be lost.
 */

import "./custom.tsp";

using Http;
using TypeSpec.OpenAPI;

namespace OpenAI;

// Tool generated type. Extracts from AuditLog.certificates_activated.certificates
alias AuditLogCertificatesActivatedCertificate = {
  /** The certificate ID. */
  id?: string;

  /** The name of the certificate. */
  name?: string;
};

// Tool generated type. Extracts from AuditLog.certificates_deactivated.certificates
alias AuditLogCertificatesDeactivatedCertificate = {
  /** The certificate ID. */
  id?: string;

  /** The name of the certificate. */
  name?: string;
};

// Tool generated type. Extracts from Invite.projects
alias InviteProject = {
  /** Project's public ID */
  id?: string;

  /** Project membership role */
  role?: "member" | "owner";
};

// Tool generated type. Extracts from InviteRequest.projects
alias InviteRequestProject = {
  /** Project's public ID */
  id: string;

  /** Project membership role */
  role: "member" | "owner";
};

/** Represents an individual Admin API key in an org. */
model AdminApiKey {
  @doc("""
    The object type, which is always `organization.admin_api_key`
    """)
  object: string;

  /** The identifier, which can be referenced in API endpoints */
  id: string;

  /** The name of the API key */
  name: string;

  /** The redacted value of the API key */
  redacted_value: string;

  /** The value of the API key. Only shown on create. */
  value?: string;

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) of when the API key was created */
  @encode("unixTimestamp", int64)
  created_at: utcDateTime;

  /** The Unix timestamp (in seconds) of when the API key was last used */
  last_used_at: int64 | null;

  owner: {
    @doc("""
      Always `user`
      """)
    type?: string;

    /** The object type, which is always organization.user */
    object?: string;

    /** The identifier, which can be referenced in API endpoints */
    id?: string;

    /** The name of the user */
    name?: string;

    // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
    /** The Unix timestamp (in seconds) of when the user was created */
    @encode("unixTimestamp", int64)
    created_at?: utcDateTime;

    @doc("""
      Always `owner`
      """)
    role?: string;
  };
}

model ApiKeyList {
  object?: string;
  data?: AdminApiKey[];
  has_more?: boolean;
  first_id?: string;
  last_id?: string;
}

/** The service account that performed the audit logged action. */
model AuditLogActorServiceAccount {
  /** The service account id. */
  id?: string;
}

/** The user who performed the audit logged action. */
model AuditLogActorUser {
  /** The user id. */
  id?: string;

  /** The user email. */
  email?: string;
}

/** The API Key used to perform the audit logged action. */
model AuditLogActorApiKey {
  /** The tracking id of the API key. */
  id?: string;

  @doc("""
    The type of API key. Can be either `user` or `service_account`.
    """)
  type?: "user" | "service_account";

  user?: AuditLogActorUser;
  service_account?: AuditLogActorServiceAccount;
}

/** The session in which the audit logged action was performed. */
model AuditLogActorSession {
  user?: AuditLogActorUser;

  /** The IP address from which the action was performed. */
  ip_address?: string;
}

/** The actor who performed the audit logged action. */
model AuditLogActor {
  @doc("""
    The type of actor. Is either `session` or `api_key`.
    """)
  type?: "session" | "api_key";

  session?: AuditLogActorSession;
  api_key?: AuditLogActorApiKey;
}

/** The event type. */
union AuditLogEventType {
  "api_key.created",
  "api_key.updated",
  "api_key.deleted",
  "checkpoint_permission.created",
  "checkpoint_permission.deleted",
  "invite.sent",
  "invite.accepted",
  "invite.deleted",
  "login.succeeded",
  "login.failed",
  "logout.succeeded",
  "logout.failed",
  "organization.updated",
  "project.created",
  "project.updated",
  "project.archived",
  "service_account.created",
  "service_account.updated",
  "service_account.deleted",
  "rate_limit.updated",
  "rate_limit.deleted",
  "user.added",
  "user.updated",
  "user.deleted",
}

/** A log of a user action or configuration change within this organization. */
model AuditLog {
  /** The ID of this log. */
  id: string;

  type: AuditLogEventType;

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) of the event. */
  @encode("unixTimestamp", int32)
  effective_at: utcDateTime;

  /** The project that the action was scoped to. Absent for actions not scoped to projects. */
  project?: {
    /** The project ID. */
    id?: string;

    /** The project title. */
    name?: string;
  };

  actor: AuditLogActor;

  @doc("""
    The details for events with this `type`.
    """)
  @encodedName("application/json", "api_key.created")
  api_key_created?: {
    /** The tracking ID of the API key. */
    id?: string;

    /** The payload used to create the API key. */
    data?: {
      @doc("""
        A list of scopes allowed for the API key, e.g. `["api.model.request"]`
        """)
      scopes?: string[];
    };
  };

  @doc("""
    The details for events with this `type`.
    """)
  @encodedName("application/json", "api_key.updated")
  api_key_updated?: {
    /** The tracking ID of the API key. */
    id?: string;

    /** The payload used to update the API key. */
    changes_requested?: {
      @doc("""
        A list of scopes allowed for the API key, e.g. `["api.model.request"]`
        """)
      scopes?: string[];
    };
  };

  @doc("""
    The details for events with this `type`.
    """)
  @encodedName("application/json", "api_key.deleted")
  api_key_deleted?: {
    /** The tracking ID of the API key. */
    id?: string;
  };

  /** The project and fine-tuned model checkpoint that the checkpoint permission was created for. */
  @encodedName("application/json", "checkpoint_permission.created")
  checkpoint_permission_created?: {
    /** The ID of the checkpoint permission. */
    id?: string;

    /** The payload used to create the checkpoint permission. */
    data?: {
      /** The ID of the project that the checkpoint permission was created for. */
      project_id?: string;

      /** The ID of the fine-tuned model checkpoint. */
      fine_tuned_model_checkpoint?: string;
    };
  };

  @doc("""
    The details for events with this `type`.
    """)
  @encodedName("application/json", "checkpoint_permission.deleted")
  checkpoint_permission_deleted?: {
    /** The ID of the checkpoint permission. */
    id?: string;
  };

  @doc("""
    The details for events with this `type`.
    """)
  @encodedName("application/json", "invite.sent")
  invite_sent?: {
    /** The ID of the invite. */
    id?: string;

    /** The payload used to create the invite. */
    data?: {
      /** The email invited to the organization. */
      email?: string;

      @doc("""
        The role the email was invited to be. Is either `owner` or `member`.
        """)
      role?: string;
    };
  };

  @doc("""
    The details for events with this `type`.
    """)
  @encodedName("application/json", "invite.accepted")
  invite_accepted?: {
    /** The ID of the invite. */
    id?: string;
  };

  @doc("""
    The details for events with this `type`.
    """)
  @encodedName("application/json", "invite.deleted")
  invite_deleted?: {
    /** The ID of the invite. */
    id?: string;
  };

  @doc("""
    The details for events with this `type`.
    """)
  @encodedName("application/json", "login.failed")
  login_failed?: {
    /** The error code of the failure. */
    error_code?: string;

    /** The error message of the failure. */
    error_message?: string;
  };

  @doc("""
    The details for events with this `type`.
    """)
  @encodedName("application/json", "logout.failed")
  logout_failed?: {
    /** The error code of the failure. */
    error_code?: string;

    /** The error message of the failure. */
    error_message?: string;
  };

  @doc("""
    The details for events with this `type`.
    """)
  @encodedName("application/json", "organization.updated")
  organization_updated?: {
    /** The organization ID. */
    id?: string;

    /** The payload used to update the organization settings. */
    changes_requested?: {
      /** The organization title. */
      title?: string;

      /** The organization description. */
      description?: string;

      /** The organization name. */
      name?: string;

      @doc("""
        Visibility of the threads page which shows messages created with the Assistants API and Playground. One of `ANY_ROLE`, `OWNERS`, or `NONE`.
        """)
      threads_ui_visibility?: string;

      @doc("""
        Visibility of the usage dashboard which shows activity and costs for your organization. One of `ANY_ROLE` or `OWNERS`.
        """)
      usage_dashboard_visibility?: string;

      @doc("""
        How your organization logs data from supported API calls. One of `disabled`, `enabled_per_call`, `enabled_for_all_projects`, or `enabled_for_selected_projects`
        """)
      api_call_logging?: string;

      @doc("""
        The list of project ids if api_call_logging is set to `enabled_for_selected_projects`
        """)
      api_call_logging_project_ids?: string;
    };
  };

  @doc("""
    The details for events with this `type`.
    """)
  @encodedName("application/json", "project.created")
  project_created?: {
    /** The project ID. */
    id?: string;

    /** The payload used to create the project. */
    data?: {
      /** The project name. */
      name?: string;

      /** The title of the project as seen on the dashboard. */
      title?: string;
    };
  };

  @doc("""
    The details for events with this `type`.
    """)
  @encodedName("application/json", "project.updated")
  project_updated?: {
    /** The project ID. */
    id?: string;

    /** The payload used to update the project. */
    changes_requested?: {
      /** The title of the project as seen on the dashboard. */
      title?: string;
    };
  };

  @doc("""
    The details for events with this `type`.
    """)
  @encodedName("application/json", "project.archived")
  project_archived?: {
    /** The project ID. */
    id?: string;
  };

  @doc("""
    The details for events with this `type`.
    """)
  @encodedName("application/json", "rate_limit.updated")
  rate_limit_updated?: {
    /** The rate limit ID */
    id?: string;

    /** The payload used to update the rate limits. */
    changes_requested?: {
      /** The maximum requests per minute. */
      max_requests_per_1_minute?: int32;

      /** The maximum tokens per minute. */
      max_tokens_per_1_minute?: int32;

      /** The maximum images per minute. Only relevant for certain models. */
      max_images_per_1_minute?: int32;

      /** The maximum audio megabytes per minute. Only relevant for certain models. */
      max_audio_megabytes_per_1_minute?: int32;

      /** The maximum requests per day. Only relevant for certain models. */
      max_requests_per_1_day?: int32;

      /** The maximum batch input tokens per day. Only relevant for certain models. */
      batch_1_day_max_input_tokens?: int32;
    };
  };

  @doc("""
    The details for events with this `type`.
    """)
  @encodedName("application/json", "rate_limit.deleted")
  rate_limit_deleted?: {
    /** The rate limit ID */
    id?: string;
  };

  @doc("""
    The details for events with this `type`.
    """)
  @encodedName("application/json", "service_account.created")
  service_account_created?: {
    /** The service account ID. */
    id?: string;

    /** The payload used to create the service account. */
    data?: {
      @doc("""
        The role of the service account. Is either `owner` or `member`.
        """)
      role?: string;
    };
  };

  @doc("""
    The details for events with this `type`.
    """)
  @encodedName("application/json", "service_account.updated")
  service_account_updated?: {
    /** The service account ID. */
    id?: string;

    /** The payload used to updated the service account. */
    changes_requested?: {
      @doc("""
        The role of the service account. Is either `owner` or `member`.
        """)
      role?: string;
    };
  };

  @doc("""
    The details for events with this `type`.
    """)
  @encodedName("application/json", "service_account.deleted")
  service_account_deleted?: {
    /** The service account ID. */
    id?: string;
  };

  @doc("""
    The details for events with this `type`.
    """)
  @encodedName("application/json", "user.added")
  user_added?: {
    /** The user ID. */
    id?: string;

    /** The payload used to add the user to the project. */
    data?: {
      @doc("""
        The role of the user. Is either `owner` or `member`.
        """)
      role?: string;
    };
  };

  @doc("""
    The details for events with this `type`.
    """)
  @encodedName("application/json", "user.updated")
  user_updated?: {
    /** The project ID. */
    id?: string;

    /** The payload used to update the user. */
    changes_requested?: {
      @doc("""
        The role of the user. Is either `owner` or `member`.
        """)
      role?: string;
    };
  };

  @doc("""
    The details for events with this `type`.
    """)
  @encodedName("application/json", "user.deleted")
  user_deleted?: {
    /** The user ID. */
    id?: string;
  };

  @doc("""
    The details for events with this `type`.
    """)
  @encodedName("application/json", "certificate.created")
  certificate_created?: {
    /** The certificate ID. */
    id?: string;

    /** The name of the certificate. */
    name?: string;
  };

  @doc("""
    The details for events with this `type`.
    """)
  @encodedName("application/json", "certificate.updated")
  certificate_updated?: {
    /** The certificate ID. */
    id?: string;

    /** The name of the certificate. */
    name?: string;
  };

  @doc("""
    The details for events with this `type`.
    """)
  @encodedName("application/json", "certificate.deleted")
  certificate_deleted?: {
    /** The certificate ID. */
    id?: string;

    /** The name of the certificate. */
    name?: string;

    /** The certificate content in PEM format. */
    certificate?: string;
  };

  @doc("""
    The details for events with this `type`.
    """)
  @encodedName("application/json", "certificates.activated")
  certificates_activated?: {
    certificates?: AuditLogCertificatesActivatedCertificate[];
  };

  @doc("""
    The details for events with this `type`.
    """)
  @encodedName("application/json", "certificates.deactivated")
  certificates_deactivated?: {
    certificates?: AuditLogCertificatesDeactivatedCertificate[];
  };
}

model ListAuditLogsResponse {
  object: "list";
  data: AuditLog[];
  first_id: string;
  last_id: string;
  has_more: boolean;
}

@doc("""
  Represents an individual `invite` to the organization.
  """)
model Invite {
  @doc("""
    The object type, which is always `organization.invite`
    """)
  object: "organization.invite";

  /** The identifier, which can be referenced in API endpoints */
  id: string;

  /** The email address of the individual to whom the invite was sent */
  email: string;

  @doc("""
    `owner` or `reader`
    """)
  role: "owner" | "reader";

  @doc("""
    `accepted`,`expired`, or `pending`
    """)
  status: "accepted" | "expired" | "pending";

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) of when the invite was sent. */
  @encode("unixTimestamp", int32)
  invited_at: utcDateTime;

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) of when the invite expires. */
  @encode("unixTimestamp", int32)
  expires_at: utcDateTime;

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) of when the invite was accepted. */
  @encode("unixTimestamp", int32)
  accepted_at?: utcDateTime;

  /** The projects that were granted membership upon acceptance of the invite. */
  projects?: InviteProject[];
}

model InviteListResponse {
  @doc("""
    The object type, which is always `list`
    """)
  object: "list";

  data: Invite[];

  @doc("""
    The first `invite_id` in the retrieved `list`
    """)
  first_id?: string;

  @doc("""
    The last `invite_id` in the retrieved `list`
    """)
  last_id?: string;

  @doc("""
    The `has_more` property is used for pagination to indicate there are additional results.
    """)
  has_more?: boolean;
}

model InviteRequest {
  /** Send an email to this address */
  email: string;

  @doc("""
    `owner` or `reader`
    """)
  role: "reader" | "owner";

  /** An array of projects to which membership is granted at the same time the org invite is accepted. If omitted, the user will be invited to the default project for compatibility with legacy behavior. */
  projects?: InviteRequestProject[];
}

model InviteDeleteResponse {
  @doc("""
    The object type, which is always `organization.invite.deleted`
    """)
  object: "organization.invite.deleted";

  id: string;
  deleted: boolean;
}

@doc("""
  Represents an individual `user` within an organization.
  """)
model User {
  @doc("""
    The object type, which is always `organization.user`
    """)
  object: "organization.user";

  /** The identifier, which can be referenced in API endpoints */
  id: string;

  /** The name of the user */
  name: string;

  /** The email address of the user */
  email: string;

  @doc("""
    `owner` or `reader`
    """)
  role: "owner" | "reader";

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) of when the user was added. */
  @encode("unixTimestamp", int32)
  added_at: utcDateTime;
}

model UserListResponse {
  object: "list";
  data: User[];
  first_id: string;
  last_id: string;
  has_more: boolean;
}

model UserRoleUpdateRequest {
  @doc("""
    `owner` or `reader`
    """)
  role: "owner" | "reader";
}

model UserDeleteResponse {
  object: "organization.user.deleted";
  id: string;
  deleted: boolean;
}

/** Represents an individual project. */
model Project {
  /** The identifier, which can be referenced in API endpoints */
  id: string;

  @doc("""
    The object type, which is always `organization.project`
    """)
  object: "organization.project";

  /** The name of the project. This appears in reporting. */
  name: string;

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) of when the project was created. */
  @encode("unixTimestamp", int32)
  created_at: utcDateTime;

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  @doc("""
    The Unix timestamp (in seconds) of when the project was archived or `null`.
    """)
  @encode("unixTimestamp", int32)
  archived_at?: utcDateTime | null;

  @doc("""
    `active` or `archived`
    """)
  status: "active" | "archived";
}

model ProjectListResponse {
  object: "list";
  data: Project[];
  first_id: string;
  last_id: string;
  has_more: boolean;
}

model ProjectCreateRequest {
  /** The friendly name of the project, this name appears in reports. */
  name: string;
}

model ProjectUpdateRequest {
  /** The updated name of the project, this name appears in reports. */
  name: string;
}

/** Represents an individual user in a project. */
model ProjectUser {
  @doc("""
    The object type, which is always `organization.project.user`
    """)
  object: "organization.project.user";

  /** The identifier, which can be referenced in API endpoints */
  id: string;

  /** The name of the user */
  name: string;

  /** The email address of the user */
  email: string;

  @doc("""
    `owner` or `member`
    """)
  role: "owner" | "member";

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) of when the project was added. */
  @encode("unixTimestamp", int32)
  added_at: utcDateTime;
}

model ProjectUserListResponse {
  object: string;
  data: ProjectUser[];
  first_id: string;
  last_id: string;
  has_more: boolean;
}

model ProjectUserCreateRequest {
  /** The ID of the user. */
  user_id: string;

  @doc("""
    `owner` or `member`
    """)
  role: "owner" | "member";
}

model ProjectUserUpdateRequest {
  @doc("""
    `owner` or `member`
    """)
  role: "owner" | "member";
}

model ProjectUserDeleteResponse {
  object: "organization.project.user.deleted";
  id: string;
  deleted: boolean;
}

/** Represents an individual service account in a project. */
model ProjectServiceAccount {
  @doc("""
    The object type, which is always `organization.project.service_account`
    """)
  object: "organization.project.service_account";

  /** The identifier, which can be referenced in API endpoints */
  id: string;

  /** The name of the service account */
  name: string;

  @doc("""
    `owner` or `member`
    """)
  role: "owner" | "member";

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) of when the service account was created */
  @encode("unixTimestamp", int32)
  created_at: utcDateTime;
}

model ProjectServiceAccountListResponse {
  object: "list";
  data: ProjectServiceAccount[];
  first_id: string;
  last_id: string;
  has_more: boolean;
}

model ProjectServiceAccountCreateRequest {
  /** The name of the service account being created. */
  name: string;
}

model ProjectServiceAccountCreateResponse {
  object: "organization.project.service_account";
  id: string;
  name: string;

  @doc("""
    Service accounts can only have one role of type `member`
    """)
  role: "member";

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  @encode("unixTimestamp", int32)
  created_at: utcDateTime;

  api_key: ProjectServiceAccountApiKey;
}

model ProjectServiceAccountApiKey {
  @doc("""
    The object type, which is always `organization.project.service_account.api_key`
    """)
  object: "organization.project.service_account.api_key";

  value: string;
  name: string;

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  @encode("unixTimestamp", int32)
  created_at: utcDateTime;

  id: string;
}

model ProjectServiceAccountDeleteResponse {
  object: "organization.project.service_account.deleted";
  id: string;
  deleted: boolean;
}

/** Represents an individual API key in a project. */
model ProjectApiKey {
  @doc("""
    The object type, which is always `organization.project.api_key`
    """)
  object: "organization.project.api_key";

  /** The redacted value of the API key */
  redacted_value: string;

  /** The name of the API key */
  name: string;

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) of when the API key was created */
  @encode("unixTimestamp", int32)
  created_at: utcDateTime;

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) of when the API key was last used. */
  @encode("unixTimestamp", int32)
  last_used_at: utcDateTime;

  /** The identifier, which can be referenced in API endpoints */
  id: string;

  owner: {
    @doc("""
      `user` or `service_account`
      """)
    type?: "user" | "service_account";

    user?: ProjectUser;
    service_account?: ProjectServiceAccount;
  };
}

model ProjectApiKeyListResponse {
  object: "list";
  data: ProjectApiKey[];
  first_id: string;
  last_id: string;
  has_more: boolean;
}

model ProjectApiKeyDeleteResponse {
  object: "organization.project.api_key.deleted";
  id: string;
  deleted: boolean;
}

/** The aggregated costs details of the specific time bucket. */
model CostsResult {
  object: "organization.costs.result";

  /** The monetary value in its associated currency. */
  amount?: {
    /** The numeric value of the cost. */
    value?: float32;

    /** Lowercase ISO-4217 currency e.g. "usd" */
    currency?: string;
  };

  @doc("""
    When `group_by=line_item`, this field provides the line item of the grouped costs result.
    """)
  line_item?: string | null;

  @doc("""
    When `group_by=project_id`, this field provides the project ID of the grouped costs result.
    """)
  project_id?: string | null;
}

/** Represents a project rate limit config. */
model ProjectRateLimit {
  @doc("""
    The object type, which is always `project.rate_limit`
    """)
  object: "project.rate_limit";

  /** The identifier, which can be referenced in API endpoints. */
  id: string;

  /** The model this rate limit applies to. */
  `model`: string;

  /** The maximum requests per minute. */
  max_requests_per_1_minute: int32;

  /** The maximum tokens per minute. */
  max_tokens_per_1_minute: int32;

  /** The maximum images per minute. Only present for relevant models. */
  max_images_per_1_minute?: int32;

  /** The maximum audio megabytes per minute. Only present for relevant models. */
  max_audio_megabytes_per_1_minute?: int32;

  /** The maximum requests per day. Only present for relevant models. */
  max_requests_per_1_day?: int32;

  /** The maximum batch input tokens per day. Only present for relevant models. */
  batch_1_day_max_input_tokens?: int32;
}

model ProjectRateLimitListResponse {
  object: "list";
  data: ProjectRateLimit[];
  first_id: string;
  last_id: string;
  has_more: boolean;
}

model ProjectRateLimitUpdateRequest {
  /** The maximum requests per minute. */
  max_requests_per_1_minute?: int32;

  /** The maximum tokens per minute. */
  max_tokens_per_1_minute?: int32;

  /** The maximum images per minute. Only relevant for certain models. */
  max_images_per_1_minute?: int32;

  /** The maximum audio megabytes per minute. Only relevant for certain models. */
  max_audio_megabytes_per_1_minute?: int32;

  /** The maximum requests per day. Only relevant for certain models. */
  max_requests_per_1_day?: int32;

  /** The maximum batch input tokens per day. Only relevant for certain models. */
  batch_1_day_max_input_tokens?: int32;
}

/** The aggregated audio speeches usage details of the specific time bucket. */
model UsageAudioSpeechesResult {
  object: "organization.usage.audio_speeches.result";

  /** The number of characters processed. */
  characters: int32;

  /** The count of requests made to the model. */
  num_model_requests: int32;

  @doc("""
    When `group_by=project_id`, this field provides the project ID of the grouped usage result.
    """)
  project_id?: string | null;

  @doc("""
    When `group_by=user_id`, this field provides the user ID of the grouped usage result.
    """)
  user_id?: string | null;

  @doc("""
    When `group_by=api_key_id`, this field provides the API key ID of the grouped usage result.
    """)
  api_key_id?: string | null;

  @doc("""
    When `group_by=model`, this field provides the model name of the grouped usage result.
    """)
  `model`?: string | null;
}

/** The aggregated audio transcriptions usage details of the specific time bucket. */
model UsageAudioTranscriptionsResult {
  object: "organization.usage.audio_transcriptions.result";

  /** The number of seconds processed. */
  seconds: int32;

  /** The count of requests made to the model. */
  num_model_requests: int32;

  @doc("""
    When `group_by=project_id`, this field provides the project ID of the grouped usage result.
    """)
  project_id?: string | null;

  @doc("""
    When `group_by=user_id`, this field provides the user ID of the grouped usage result.
    """)
  user_id?: string | null;

  @doc("""
    When `group_by=api_key_id`, this field provides the API key ID of the grouped usage result.
    """)
  api_key_id?: string | null;

  @doc("""
    When `group_by=model`, this field provides the model name of the grouped usage result.
    """)
  `model`?: string | null;
}

/** The aggregated code interpreter sessions usage details of the specific time bucket. */
model UsageCodeInterpreterSessionsResult {
  object: "organization.usage.code_interpreter_sessions.result";

  /** The number of code interpreter sessions. */
  num_sessions?: int32;

  @doc("""
    When `group_by=project_id`, this field provides the project ID of the grouped usage result.
    """)
  project_id?: string | null;
}

/** The aggregated completions usage details of the specific time bucket. */
model UsageCompletionsResult {
  object: "organization.usage.completions.result";

  /** The aggregated number of text input tokens used, including cached tokens. For customers subscribe to scale tier, this includes scale tier tokens. */
  input_tokens: int32;

  /** The aggregated number of text input tokens that has been cached from previous requests. For customers subscribe to scale tier, this includes scale tier tokens. */
  input_cached_tokens?: int32;

  /** The aggregated number of text output tokens used. For customers subscribe to scale tier, this includes scale tier tokens. */
  output_tokens: int32;

  /** The aggregated number of audio input tokens used, including cached tokens. */
  input_audio_tokens?: int32;

  /** The aggregated number of audio output tokens used. */
  output_audio_tokens?: int32;

  /** The count of requests made to the model. */
  num_model_requests: int32;

  @doc("""
    When `group_by=project_id`, this field provides the project ID of the grouped usage result.
    """)
  project_id?: string | null;

  @doc("""
    When `group_by=user_id`, this field provides the user ID of the grouped usage result.
    """)
  user_id?: string | null;

  @doc("""
    When `group_by=api_key_id`, this field provides the API key ID of the grouped usage result.
    """)
  api_key_id?: string | null;

  @doc("""
    When `group_by=model`, this field provides the model name of the grouped usage result.
    """)
  `model`?: string | null;

  @doc("""
    When `group_by=batch`, this field tells whether the grouped usage result is batch or not.
    """)
  batch?: boolean | null;
}

/** The aggregated embeddings usage details of the specific time bucket. */
model UsageEmbeddingsResult {
  object: "organization.usage.embeddings.result";

  /** The aggregated number of input tokens used. */
  input_tokens: int32;

  /** The count of requests made to the model. */
  num_model_requests: int32;

  @doc("""
    When `group_by=project_id`, this field provides the project ID of the grouped usage result.
    """)
  project_id?: string | null;

  @doc("""
    When `group_by=user_id`, this field provides the user ID of the grouped usage result.
    """)
  user_id?: string | null;

  @doc("""
    When `group_by=api_key_id`, this field provides the API key ID of the grouped usage result.
    """)
  api_key_id?: string | null;

  @doc("""
    When `group_by=model`, this field provides the model name of the grouped usage result.
    """)
  `model`?: string | null;
}

/** The aggregated images usage details of the specific time bucket. */
model UsageImagesResult {
  object: "organization.usage.images.result";

  /** The number of images processed. */
  images: int32;

  /** The count of requests made to the model. */
  num_model_requests: int32;

  @doc("""
    When `group_by=source`, this field provides the source of the grouped usage result, possible values are `image.generation`, `image.edit`, `image.variation`.
    """)
  source?: string | null;

  @doc("""
    When `group_by=size`, this field provides the image size of the grouped usage result.
    """)
  size?: string | null;

  @doc("""
    When `group_by=project_id`, this field provides the project ID of the grouped usage result.
    """)
  project_id?: string | null;

  @doc("""
    When `group_by=user_id`, this field provides the user ID of the grouped usage result.
    """)
  user_id?: string | null;

  @doc("""
    When `group_by=api_key_id`, this field provides the API key ID of the grouped usage result.
    """)
  api_key_id?: string | null;

  @doc("""
    When `group_by=model`, this field provides the model name of the grouped usage result.
    """)
  `model`?: string | null;
}

/** The aggregated moderations usage details of the specific time bucket. */
model UsageModerationsResult {
  object: "organization.usage.moderations.result";

  /** The aggregated number of input tokens used. */
  input_tokens: int32;

  /** The count of requests made to the model. */
  num_model_requests: int32;

  @doc("""
    When `group_by=project_id`, this field provides the project ID of the grouped usage result.
    """)
  project_id?: string | null;

  @doc("""
    When `group_by=user_id`, this field provides the user ID of the grouped usage result.
    """)
  user_id?: string | null;

  @doc("""
    When `group_by=api_key_id`, this field provides the API key ID of the grouped usage result.
    """)
  api_key_id?: string | null;

  @doc("""
    When `group_by=model`, this field provides the model name of the grouped usage result.
    """)
  `model`?: string | null;
}

model UsageResponse {
  object: "page";
  data: UsageTimeBucket[];
  has_more: boolean;
  next_page: string;
}

model UsageTimeBucket {
  object: "bucket";
  start_time: int32;
  end_time: int32;
  result: (
    | UsageCompletionsResult
    | UsageEmbeddingsResult
    | UsageModerationsResult
    | UsageImagesResult
    | UsageAudioSpeechesResult
    | UsageAudioTranscriptionsResult
    | UsageVectorStoresResult
    | UsageCodeInterpreterSessionsResult
    | CostsResult)[];
}

/** The aggregated vector stores usage details of the specific time bucket. */
model UsageVectorStoresResult {
  object: "organization.usage.vector_stores.result";

  /** The vector stores usage in bytes. */
  usage_bytes: int32;

  @doc("""
    When `group_by=project_id`, this field provides the project ID of the grouped usage result.
    """)
  project_id?: string | null;
}

@doc("""
  Represents an individual `certificate` uploaded to the organization.
  """)
model Certificate {
  @doc("""
    The object type.
    
    - If creating, updating, or getting a specific certificate, the object type is `certificate`.
    - If listing, activating, or deactivating certificates for the organization, the object type is `organization.certificate`.
    - If listing, activating, or deactivating certificates for a project, the object type is `organization.project.certificate`.
    """)
  object: "certificate" | "organization.certificate" | "organization.project.certificate";

  /** The identifier, which can be referenced in API endpoints */
  id: string;

  /** The name of the certificate. */
  name: string;

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) of when the certificate was uploaded. */
  @encode("unixTimestamp", int32)
  created_at: utcDateTime;

  certificate_details: {
    // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
    /** The Unix timestamp (in seconds) of when the certificate becomes valid. */
    @encode("unixTimestamp", int32)
    valid_at?: utcDateTime;

    // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
    /** The Unix timestamp (in seconds) of when the certificate expires. */
    @encode("unixTimestamp", int32)
    expires_at?: utcDateTime;

    /** The content of the certificate in PEM format. */
    content?: string;
  };

  /** Whether the certificate is currently active at the specified scope. Not returned when getting details for a specific certificate. */
  active?: boolean;
}

model DeleteCertificateResponse {
  @doc("""
    The object type, must be `certificate.deleted`.
    """)
  object: "certificate.deleted";

  /** The ID of the certificate that was deleted. */
  id: string;
}

model ListCertificatesResponse {
  data: Certificate[];
  first_id?: string;
  last_id?: string;
  has_more: boolean;
  object: "list";
}

model ModifyCertificateRequest {
  /** The updated name for the certificate */
  name: string;
}

model ToggleCertificatesRequest {
  @minItems(1)
  @maxItems(10)
  certificate_ids: string[];
}

model UploadCertificateRequest {
  /** An optional name for the certificate */
  name?: string;

  /** The certificate content in PEM format */
  content: string;
}
