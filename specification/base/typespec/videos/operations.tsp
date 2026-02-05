import "@typespec/http";
import "@typespec/openapi";
import "../common";
import "./models.tsp";
using TypeSpec.Http;
using TypeSpec.OpenAPI;
namespace OpenAI;
/** List videos */
#suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-names" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("List videos")
@get
@route("/videos")
@extension(
  "x-oaiMeta",
  #{
    name: "List videos",
    group: "videos",
    path: "list",
    returns: "Returns a paginated list of [video jobs](https://platform.openai.com/docs/api-reference/videos/object) for the organization.",
    examples: #[
      #{
        title: "List recent videos",
        request: #{
          curl: "curl https://api.openai.com/v1/videos \\\n  -H \"Authorization: Bearer $OPENAI_API_KEY\"\n",
          javascript: "import OpenAI from 'openai';\n\nconst openai = new OpenAI();\n\n// Automatically fetches more pages as needed.\nfor await (const video of openai.videos.list()) {\n  console.log(video.id);\n}\n",
          python: "from openai import OpenAI\n\nclient = OpenAI(\n    api_key=\"My API Key\",\n)\npage = client.videos.list()\npage = page.data[0]\nprint(page.id)",
          go: "package main\n\nimport (\n  \"context\"\n  \"fmt\"\n\n  \"github.com/openai/openai-go\"\n  \"github.com/openai/openai-go/option\"\n)\n\nfunc main() {\n  client := openai.NewClient(\n    option.WithAPIKey(\"My API Key\"),\n  )\n  page, err := client.Videos.List(context.TODO(), openai.VideoListParams{\n\n  })\n  if err != nil {\n    panic(err.Error())\n  }\n  fmt.Printf(\"%+v\\n\", page)\n}\n",
          java: "package com.openai.example;\n\nimport com.openai.client.OpenAIClient;\nimport com.openai.client.okhttp.OpenAIOkHttpClient;\nimport com.openai.models.videos.VideoListPage;\nimport com.openai.models.videos.VideoListParams;\n\npublic final class Main {\n    private Main() {}\n\n    public static void main(String[] args) {\n        OpenAIClient client = OpenAIOkHttpClient.fromEnv();\n\n        VideoListPage page = client.videos().list();\n    }\n}",
          ruby: "require \"openai\"\n\nopenai = OpenAI::Client.new(api_key: \"My API Key\")\n\npage = openai.videos.list\n\nputs(page)",
          `node.js`: "import OpenAI from 'openai';\n\nconst client = new OpenAI({\n  apiKey: 'My API Key',\n});\n\n// Automatically fetches more pages as needed.\nfor await (const video of client.videos.list()) {\n  console.log(video.id);\n}",
        },
        response: "{\n  \"data\": [\n    {\n      \"id\": \"video_123\",\n      \"object\": \"video\",\n      \"model\": \"sora-2\",\n      \"status\": \"completed\"\n    }\n  ],\n  \"object\": \"list\"\n}\n",
      }
    ],
  }
)
@tag("Videos")
@list
op ListVideos(
  /** Number of items to retrieve */
  #suppress "@azure-tools/typespec-azure-core/no-query-explode" "Auto-suppressed warnings non-applicable rules during import."
  @query(#{ explode: true })
  @maxValue(100)
  @minValue(0)
  limit?: integer,

  /** Sort order of results by timestamp. Use `asc` for ascending order or `desc` for descending order. */
  #suppress "@azure-tools/typespec-azure-core/no-query-explode" "Auto-suppressed warnings non-applicable rules during import."
  @query(#{ explode: true })
  order?: OrderEnum,

  /** Identifier for the last item from the previous pagination request */
  #suppress "@azure-tools/typespec-azure-core/no-query-explode" "Auto-suppressed warnings non-applicable rules during import."
  @query(#{ explode: true })
  after?: string,
): VideoListResource;

/** Create a video */
#suppress "@azure-tools/typespec-azure-core/byos" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("Create video")
@post
@route("/videos")
@extension(
  "x-oaiMeta",
  #{
    name: "Create video",
    group: "videos",
    path: "create",
    returns: "Returns the newly created [video job](https://platform.openai.com/docs/api-reference/videos/object).",
    examples: #[
      #{
        title: "Create a video render",
        request: #{
          curl: "curl https://api.openai.com/v1/videos \\\n  -H \"Authorization: Bearer $OPENAI_API_KEY\" \\\n  -F \"model=sora-2\" \\\n  -F \"prompt=A calico cat playing a piano on stage\"\n",
          javascript: "import OpenAI from 'openai';\n\nconst openai = new OpenAI();\n\nconst video = await openai.videos.create({ prompt: 'A calico cat playing a piano on stage' });\n\nconsole.log(video.id);\n",
          python: "from openai import OpenAI\n\nclient = OpenAI(\n    api_key=\"My API Key\",\n)\nvideo = client.videos.create(\n    prompt=\"x\",\n)\nprint(video.id)",
          go: "package main\n\nimport (\n  \"context\"\n  \"fmt\"\n\n  \"github.com/openai/openai-go\"\n  \"github.com/openai/openai-go/option\"\n)\n\nfunc main() {\n  client := openai.NewClient(\n    option.WithAPIKey(\"My API Key\"),\n  )\n  video, err := client.Videos.New(context.TODO(), openai.VideoNewParams{\n    Prompt: \"x\",\n  })\n  if err != nil {\n    panic(err.Error())\n  }\n  fmt.Printf(\"%+v\\n\", video.ID)\n}\n",
          java: "package com.openai.example;\n\nimport com.openai.client.OpenAIClient;\nimport com.openai.client.okhttp.OpenAIOkHttpClient;\nimport com.openai.models.videos.Video;\nimport com.openai.models.videos.VideoCreateParams;\n\npublic final class Main {\n    private Main() {}\n\n    public static void main(String[] args) {\n        OpenAIClient client = OpenAIOkHttpClient.fromEnv();\n\n        VideoCreateParams params = VideoCreateParams.builder()\n            .prompt(\"x\")\n            .build();\n        Video video = client.videos().create(params);\n    }\n}",
          ruby: "require \"openai\"\n\nopenai = OpenAI::Client.new(api_key: \"My API Key\")\n\nvideo = openai.videos.create(prompt: \"x\")\n\nputs(video)",
          `node.js`: "import OpenAI from 'openai';\n\nconst client = new OpenAI({\n  apiKey: 'My API Key',\n});\n\nconst video = await client.videos.create({ prompt: 'x' });\n\nconsole.log(video.id);",
        },
        response: "{\n  \"id\": \"video_123\",\n  \"object\": \"video\",\n  \"model\": \"sora-2\",\n  \"status\": \"queued\",\n  \"progress\": 0,\n  \"created_at\": 1712697600,\n  \"size\": \"1024x1808\",\n  \"seconds\": \"8\",\n  \"quality\": \"standard\"\n}\n",
      }
    ],
  }
)
@tag("Videos")
op createVideo(
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  @header
  contentType: "multipart/form-data",

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  @multipartBody
  body: CreateVideoBody,
): VideoResource;

/** Delete a video */
#suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-names" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("Delete video")
@delete
@route("/videos/{video_id}")
@extension(
  "x-oaiMeta",
  #{
    name: "Delete video",
    group: "videos",
    path: "delete",
    returns: "Returns the deleted video job metadata.",
    examples: #{
      response: "",
      request: #{
        `node.js`: "import OpenAI from 'openai';\n\nconst client = new OpenAI({\n  apiKey: 'My API Key',\n});\n\nconst video = await client.videos.delete('video_123');\n\nconsole.log(video.id);",
        python: "from openai import OpenAI\n\nclient = OpenAI(\n    api_key=\"My API Key\",\n)\nvideo = client.videos.delete(\n    \"video_123\",\n)\nprint(video.id)",
        go: "package main\n\nimport (\n  \"context\"\n  \"fmt\"\n\n  \"github.com/openai/openai-go\"\n  \"github.com/openai/openai-go/option\"\n)\n\nfunc main() {\n  client := openai.NewClient(\n    option.WithAPIKey(\"My API Key\"),\n  )\n  video, err := client.Videos.Delete(context.TODO(), \"video_123\")\n  if err != nil {\n    panic(err.Error())\n  }\n  fmt.Printf(\"%+v\\n\", video.ID)\n}\n",
        java: "package com.openai.example;\n\nimport com.openai.client.OpenAIClient;\nimport com.openai.client.okhttp.OpenAIOkHttpClient;\nimport com.openai.models.videos.VideoDeleteParams;\nimport com.openai.models.videos.VideoDeleteResponse;\n\npublic final class Main {\n    private Main() {}\n\n    public static void main(String[] args) {\n        OpenAIClient client = OpenAIOkHttpClient.fromEnv();\n\n        VideoDeleteResponse video = client.videos().delete(\"video_123\");\n    }\n}",
        ruby: "require \"openai\"\n\nopenai = OpenAI::Client.new(api_key: \"My API Key\")\n\nvideo = openai.videos.delete(\"video_123\")\n\nputs(video)",
      },
    },
  }
)
@tag("Videos")
op DeleteVideo(
  /** The identifier of the video to delete. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  video_id: string,
): DeletedVideoResource;

/** Retrieve a video */
#suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-names" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("Retrieve video")
@get
@route("/videos/{video_id}")
@extension(
  "x-oaiMeta",
  #{
    name: "Retrieve video",
    group: "videos",
    path: "retrieve",
    returns: "Returns the [video job](https://platform.openai.com/docs/api-reference/videos/object) matching the provided identifier.",
    examples: #{
      response: "",
      request: #{
        `node.js`: "import OpenAI from 'openai';\n\nconst client = new OpenAI({\n  apiKey: 'My API Key',\n});\n\nconst video = await client.videos.retrieve('video_123');\n\nconsole.log(video.id);",
        python: "from openai import OpenAI\n\nclient = OpenAI(\n    api_key=\"My API Key\",\n)\nvideo = client.videos.retrieve(\n    \"video_123\",\n)\nprint(video.id)",
        go: "package main\n\nimport (\n  \"context\"\n  \"fmt\"\n\n  \"github.com/openai/openai-go\"\n  \"github.com/openai/openai-go/option\"\n)\n\nfunc main() {\n  client := openai.NewClient(\n    option.WithAPIKey(\"My API Key\"),\n  )\n  video, err := client.Videos.Get(context.TODO(), \"video_123\")\n  if err != nil {\n    panic(err.Error())\n  }\n  fmt.Printf(\"%+v\\n\", video.ID)\n}\n",
        java: "package com.openai.example;\n\nimport com.openai.client.OpenAIClient;\nimport com.openai.client.okhttp.OpenAIOkHttpClient;\nimport com.openai.models.videos.Video;\nimport com.openai.models.videos.VideoRetrieveParams;\n\npublic final class Main {\n    private Main() {}\n\n    public static void main(String[] args) {\n        OpenAIClient client = OpenAIOkHttpClient.fromEnv();\n\n        Video video = client.videos().retrieve(\"video_123\");\n    }\n}",
        ruby: "require \"openai\"\n\nopenai = OpenAI::Client.new(api_key: \"My API Key\")\n\nvideo = openai.videos.retrieve(\"video_123\")\n\nputs(video)",
      },
    },
  }
)
@tag("Videos")
op GetVideo(
  /** The identifier of the video to retrieve. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  video_id: string,
): VideoResource;

/** Download video content */
#suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/response-schema-problem" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-names" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("Retrieve video content")
@get
@route("/videos/{video_id}/content")
@extension(
  "x-oaiMeta",
  #{
    name: "Retrieve video content",
    group: "videos",
    path: "content",
    returns: "Streams the rendered video content for the specified video job.",
    examples: #{
      response: "",
      request: #{
        `node.js`: "import OpenAI from 'openai';\n\nconst client = new OpenAI({\n  apiKey: 'My API Key',\n});\n\nconst response = await client.videos.downloadContent('video_123');\n\nconsole.log(response);\n\nconst content = await response.blob();\nconsole.log(content);",
        python: "from openai import OpenAI\n\nclient = OpenAI(\n    api_key=\"My API Key\",\n)\nresponse = client.videos.download_content(\n    video_id=\"video_123\",\n)\nprint(response)\ncontent = response.read()\nprint(content)",
        go: "package main\n\nimport (\n  \"context\"\n  \"fmt\"\n\n  \"github.com/openai/openai-go\"\n  \"github.com/openai/openai-go/option\"\n)\n\nfunc main() {\n  client := openai.NewClient(\n    option.WithAPIKey(\"My API Key\"),\n  )\n  response, err := client.Videos.DownloadContent(\n    context.TODO(),\n    \"video_123\",\n    openai.VideoDownloadContentParams{\n\n    },\n  )\n  if err != nil {\n    panic(err.Error())\n  }\n  fmt.Printf(\"%+v\\n\", response)\n}\n",
        java: "package com.openai.example;\n\nimport com.openai.client.OpenAIClient;\nimport com.openai.client.okhttp.OpenAIOkHttpClient;\nimport com.openai.core.http.HttpResponse;\nimport com.openai.models.videos.VideoDownloadContentParams;\n\npublic final class Main {\n    private Main() {}\n\n    public static void main(String[] args) {\n        OpenAIClient client = OpenAIOkHttpClient.fromEnv();\n\n        HttpResponse response = client.videos().downloadContent(\"video_123\");\n    }\n}",
        ruby: "require \"openai\"\n\nopenai = OpenAI::Client.new(api_key: \"My API Key\")\n\nresponse = openai.videos.download_content(\"video_123\")\n\nputs(response)",
      },
    },
  }
)
@tag("Videos")
op RetrieveVideoContent(
  /** The identifier of the video whose media to download. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  video_id: string,

  /** Which downloadable asset to return. Defaults to the MP4 video. */
  #suppress "@azure-tools/typespec-azure-core/no-query-explode" "Auto-suppressed warnings non-applicable rules during import."
  @query(#{ explode: true })
  variant?: VideoContentVariant,
): {
  @header
  contentType: "video/mp4";

  @body
  body: bytes;
} | {
  @header
  contentType: "image/webp";

  @body
  body: bytes;
} | Body<string>;

/** Create a video remix */
#suppress "@azure-tools/typespec-azure-core/byos" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("Remix video")
@post
@route("/videos/{video_id}/remix")
@extension(
  "x-oaiMeta",
  #{
    name: "Remix video",
    group: "videos",
    path: "remix",
    returns: "Creates a remix of the specified [video job](https://platform.openai.com/docs/api-reference/videos/object) using the provided prompt.",
    examples: #[
      #{
        title: "Remix a generated video",
        request: #{
          curl: "curl -X POST https://api.openai.com/v1/videos/video_123/remix \\\n  -H \"Authorization: Bearer $OPENAI_API_KEY\" \\\n  -H \"Content-Type: application/json\" \\\n  -d '{\n    \"prompt\": \"Extend the scene with the cat taking a bow to the cheering audience\"\n  }'\n",
          javascript: "import OpenAI from 'openai';\n\nconst client = new OpenAI();\n\nconst video = await client.videos.remix('video_123', { prompt: 'Extend the scene with the cat taking a bow to the cheering audience' });\n\nconsole.log(video.id);\n",
          python: "from openai import OpenAI\n\nclient = OpenAI(\n    api_key=\"My API Key\",\n)\nvideo = client.videos.remix(\n    video_id=\"video_123\",\n    prompt=\"x\",\n)\nprint(video.id)",
          go: "package main\n\nimport (\n  \"context\"\n  \"fmt\"\n\n  \"github.com/openai/openai-go\"\n  \"github.com/openai/openai-go/option\"\n)\n\nfunc main() {\n  client := openai.NewClient(\n    option.WithAPIKey(\"My API Key\"),\n  )\n  video, err := client.Videos.Remix(\n    context.TODO(),\n    \"video_123\",\n    openai.VideoRemixParams{\n      Prompt: \"x\",\n    },\n  )\n  if err != nil {\n    panic(err.Error())\n  }\n  fmt.Printf(\"%+v\\n\", video.ID)\n}\n",
          java: "package com.openai.example;\n\nimport com.openai.client.OpenAIClient;\nimport com.openai.client.okhttp.OpenAIOkHttpClient;\nimport com.openai.models.videos.Video;\nimport com.openai.models.videos.VideoRemixParams;\n\npublic final class Main {\n    private Main() {}\n\n    public static void main(String[] args) {\n        OpenAIClient client = OpenAIOkHttpClient.fromEnv();\n\n        VideoRemixParams params = VideoRemixParams.builder()\n            .videoId(\"video_123\")\n            .prompt(\"x\")\n            .build();\n        Video video = client.videos().remix(params);\n    }\n}",
          ruby: "require \"openai\"\n\nopenai = OpenAI::Client.new(api_key: \"My API Key\")\n\nvideo = openai.videos.remix(\"video_123\", prompt: \"x\")\n\nputs(video)",
          `node.js`: "import OpenAI from 'openai';\n\nconst client = new OpenAI({\n  apiKey: 'My API Key',\n});\n\nconst video = await client.videos.remix('video_123', { prompt: 'x' });\n\nconsole.log(video.id);",
        },
        response: "{\n  \"id\": \"video_456\",\n  \"object\": \"video\",\n  \"model\": \"sora-2\",\n  \"status\": \"queued\",\n  \"progress\": 0,\n  \"created_at\": 1712698600,\n  \"size\": \"720x1280\",\n  \"seconds\": \"8\",\n  \"remixed_from_video_id\": \"video_123\"\n}\n",
      }
    ],
  }
)
@tag("Videos")
op CreateVideoRemix(
  /** The identifier of the completed video to remix. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  video_id: string,

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  @header
  contentType: "multipart/form-data",

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  @multipartBody
  body: CreateVideoRemixBody,
): VideoResource;
