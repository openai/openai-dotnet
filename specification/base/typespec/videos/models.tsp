import "@typespec/openapi";
import "@typespec/http";
import "@azure-tools/typespec-client-generator-core";
using TypeSpec.OpenAPI;
using TypeSpec.Http;
using Azure.ClientGenerator.Core;
namespace OpenAI;

#suppress "@azure-tools/typespec-azure-core/no-enum" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
enum VideosOrderEnum {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  asc,
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  desc,
}

#suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model `Error-2` {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  code: string;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  message: string;
}

/** Structured information describing a generated video job. */
@summary("Video job")
model VideoResource {
  /** Unique identifier for the video job. */
  id: string;

  /** The object type, which is always `video`. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  object: "video" = "video";

  /** The video generation model that produced the job. */
  `model`: VideoModel;

  /** Current lifecycle status of the video job. */
  status: VideoStatus;

  /** Approximate completion percentage for the generation task. */
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  progress: integer;

  /** Unix timestamp (seconds) for when the job was created. */
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  created_at: integer;

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  completed_at: integer | null;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  expires_at: integer | null;

  /** The resolution of the generated video. */
  size: VideoSize;

  /** Duration of the generated clip in seconds. */
  seconds: VideoSeconds;

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  remixed_from_video_id: string | null;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  error: `Error-2` | null;
}

#suppress "@azure-tools/typespec-azure-core/no-enum" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
enum VideoModel {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  `sora-2`,
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  `sora-2-pro`,
}

#suppress "@azure-tools/typespec-azure-core/no-enum" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
enum VideoStatus {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  queued,
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  in_progress,
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  completed,
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  failed,
}

#suppress "@azure-tools/typespec-azure-core/no-enum" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
enum VideoSize {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  `720x1280`,
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  `1280x720`,
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  `1024x1792`,
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  `1792x1024`,
}

#suppress "@azure-tools/typespec-azure-core/no-enum" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
enum VideoSeconds {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  `4`,
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  `8`,
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  `12`,
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model VideoListResource {
  /** The type of object returned, must be `list`. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  object: "list" = "list";

  /** A list of items */
  @pageItems data: VideoResource[];

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  first_id: string;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  @continuationToken last_id: string;

  /** Whether there are more items available. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  has_more: boolean;
}

/** Parameters for creating a new video generation job. */
@summary("Create video request")
model CreateVideoBody {
  /** The video generation model to use. Defaults to `sora-2`. */
  `model`?: HttpPart<VideoModel>;

  /** Text prompt that describes the video to generate. */
  prompt: HttpPart<VideoPrompt>;

  /** Optional image reference that guides generation. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  input_reference?: HttpPart<bytes>;

  /** Clip duration in seconds. Defaults to 4 seconds. */
  seconds?: HttpPart<VideoSeconds>;

  /** Output resolution formatted as width x height. Defaults to 720x1280. */
  size?: HttpPart<VideoSize>;
}

/** Confirmation payload returned after deleting a video. */
@summary("Deleted video response")
model DeletedVideoResource {
  /** The object type that signals the deletion response. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  object: "video.deleted" = "video.deleted";

  /** Indicates that the video resource was deleted. */
  deleted: boolean;

  /** Identifier of the deleted video. */
  id: string;
}

/** Parameters for remixing an existing generated video. */
@summary("Create video remix request")
model CreateVideoRemixBody {
  /** Updated text prompt that directs the remix generation. */
  prompt: HttpPart<VideoPrompt>;
}

@maxLength(32000)
@minLength(1)
scalar VideoPrompt extends string;

#suppress "@azure-tools/typespec-azure-core/no-enum" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
enum VideoContentVariant {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  video,
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  thumbnail,
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  spritesheet,
}

union VideoCollectionOrder {
   string,
   Ascending: "asc",
   Descending: "desc",
}

@access(Access.public)
@usage(Usage.input)
model VideoCollectionOptions {
   /**
    * A cursor for use in pagination. `after` is an object ID that defines your place in the list.
    * For instance, if you make a list request and receive 100 objects, ending with obj_foo, your
    * subsequent call can include after=obj_foo in order to fetch the next page of the list.
    */
   @query @continuationToken afterId?: string;

   /**
    * A limit on the number of objects to be returned. Limit can range between 1 and 100, and the
    * default is 20.
    */
   @query pageSizeLimit?: int32 = 20;

   /**
    * Sort order by the `created_at` timestamp of the objects. `asc` for ascending order and`desc`
    * for descending order.
    */
   @query order?: VideoCollectionOrder;
}
