import "@typespec/http";
import "@typespec/openapi";
import "../common/models.tsp";
import "./models.tsp";
using TypeSpec.Http;
using TypeSpec.OpenAPI;
namespace OpenAI;
#suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/response-schema-problem" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-names" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("List all skills for the current project.")
@get
@route("/skills")
@extension(
  "x-oaiMeta",
  #{
    examples: #{
      response: "",
      request: #{
        `node.js`: """
          import OpenAI from 'openai';
          
          const client = new OpenAI({
            apiKey: process.env['OPENAI_API_KEY'], // This is the default and can be omitted
          });
          
          // Automatically fetches more pages as needed.
          for await (const skill of client.skills.list()) {
            console.log(skill.id);
          }
          """,
        python: """
          import os
          from openai import OpenAI
          
          client = OpenAI(
              api_key=os.environ.get("OPENAI_API_KEY"),  # This is the default and can be omitted
          )
          page = client.skills.list()
          page = page.data[0]
          print(page.id)
          """,
        go: """
          package main
          
          import (
          	"context"
          	"fmt"
          
          	"github.com/openai/openai-go"
          	"github.com/openai/openai-go/option"
          )
          
          func main() {
          	client := openai.NewClient(
          		option.WithAPIKey("My API Key"),
          	)
          	page, err := client.Skills.List(context.TODO(), openai.SkillListParams{})
          	if err != nil {
          		panic(err.Error())
          	}
          	fmt.Printf("%+v
          ", page)
          }
          
          """,
        java: """
          package com.openai.example;
          
          import com.openai.client.OpenAIClient;
          import com.openai.client.okhttp.OpenAIOkHttpClient;
          import com.openai.models.skills.SkillListPage;
          import com.openai.models.skills.SkillListParams;
          
          public final class Main {
              private Main() {}
          
              public static void main(String[] args) {
                  OpenAIClient client = OpenAIOkHttpClient.fromEnv();
          
                  SkillListPage page = client.skills().list();
              }
          }
          """,
        ruby: """
          require "openai"
          
          openai = OpenAI::Client.new(api_key: "My API Key")
          
          page = openai.skills.list
          
          puts(page)
          """,
      },
    },
  }
)
@tag("Skills")
op ListSkills(
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
): SkillListResource | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
};

#suppress "@azure-tools/typespec-azure-core/byos" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/response-schema-problem" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("Create a new skill.")
@post
@route("/skills")
@extension(
  "x-oaiMeta",
  #{
    examples: #{
      response: "",
      request: #{
        `node.js`: """
          import OpenAI from 'openai';
          
          const client = new OpenAI({
            apiKey: process.env['OPENAI_API_KEY'], // This is the default and can be omitted
          });
          
          const skill = await client.skills.create();
          
          console.log(skill.id);
          """,
        python: """
          import os
          from openai import OpenAI
          
          client = OpenAI(
              api_key=os.environ.get("OPENAI_API_KEY"),  # This is the default and can be omitted
          )
          skill = client.skills.create()
          print(skill.id)
          """,
        go: """
          package main
          
          import (
          	"context"
          	"fmt"
          
          	"github.com/openai/openai-go"
          	"github.com/openai/openai-go/option"
          )
          
          func main() {
          	client := openai.NewClient(
          		option.WithAPIKey("My API Key"),
          	)
          	skill, err := client.Skills.New(context.TODO(), openai.SkillNewParams{})
          	if err != nil {
          		panic(err.Error())
          	}
          	fmt.Printf("%+v
          ", skill.ID)
          }
          
          """,
        java: """
          package com.openai.example;
          
          import com.openai.client.OpenAIClient;
          import com.openai.client.okhttp.OpenAIOkHttpClient;
          import com.openai.models.skills.Skill;
          import com.openai.models.skills.SkillCreateParams;
          
          public final class Main {
              private Main() {}
          
              public static void main(String[] args) {
                  OpenAIClient client = OpenAIOkHttpClient.fromEnv();
          
                  Skill skill = client.skills().create();
              }
          }
          """,
        ruby: """
          require "openai"
          
          openai = OpenAI::Client.new(api_key: "My API Key")
          
          skill = openai.skills.create
          
          puts(skill)
          """,
      },
    },
  }
)
@sharedRoute
@tag("Skills")
op CreateSkillMultipart(
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  @header
  contentType: "multipart/form-data",

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  @multipartBody
  body: CreateSkillBody,
): SkillResource | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
};

#suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/response-schema-problem" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("Create a new skill.")
@post
@route("/skills")
@extension(
  "x-oaiMeta",
  #{
    examples: #{
      response: "",
      request: #{
        `node.js`: """
          import OpenAI from 'openai';
          
          const client = new OpenAI({
            apiKey: process.env['OPENAI_API_KEY'], // This is the default and can be omitted
          });
          
          const skill = await client.skills.create();
          
          console.log(skill.id);
          """,
        python: """
          import os
          from openai import OpenAI
          
          client = OpenAI(
              api_key=os.environ.get("OPENAI_API_KEY"),  # This is the default and can be omitted
          )
          skill = client.skills.create()
          print(skill.id)
          """,
        go: """
          package main
          
          import (
          	"context"
          	"fmt"
          
          	"github.com/openai/openai-go"
          	"github.com/openai/openai-go/option"
          )
          
          func main() {
          	client := openai.NewClient(
          		option.WithAPIKey("My API Key"),
          	)
          	skill, err := client.Skills.New(context.TODO(), openai.SkillNewParams{})
          	if err != nil {
          		panic(err.Error())
          	}
          	fmt.Printf("%+v
          ", skill.ID)
          }
          
          """,
        java: """
          package com.openai.example;
          
          import com.openai.client.OpenAIClient;
          import com.openai.client.okhttp.OpenAIOkHttpClient;
          import com.openai.models.skills.Skill;
          import com.openai.models.skills.SkillCreateParams;
          
          public final class Main {
              private Main() {}
          
              public static void main(String[] args) {
                  OpenAIClient client = OpenAIOkHttpClient.fromEnv();
          
                  Skill skill = client.skills().create();
              }
          }
          """,
        ruby: """
          require "openai"
          
          openai = OpenAI::Client.new(api_key: "My API Key")
          
          skill = openai.skills.create
          
          puts(skill)
          """,
      },
    },
  }
)
@sharedRoute
@tag("Skills")
op CreateSkillJson(
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  @body
  body: CreateSkillBody,
): SkillResource | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
};

#suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/response-schema-problem" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-names" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("Delete a skill by its ID.")
@delete
@route("/skills/{skill_id}")
@extension(
  "x-oaiMeta",
  #{
    examples: #{
      response: "",
      request: #{
        `node.js`: """
          import OpenAI from 'openai';
          
          const client = new OpenAI({
            apiKey: process.env['OPENAI_API_KEY'], // This is the default and can be omitted
          });
          
          const deletedSkill = await client.skills.delete('skill_123');
          
          console.log(deletedSkill.id);
          """,
        python: """
          import os
          from openai import OpenAI
          
          client = OpenAI(
              api_key=os.environ.get("OPENAI_API_KEY"),  # This is the default and can be omitted
          )
          deleted_skill = client.skills.delete(
              "skill_123",
          )
          print(deleted_skill.id)
          """,
        go: """
          package main
          
          import (
          	"context"
          	"fmt"
          
          	"github.com/openai/openai-go"
          	"github.com/openai/openai-go/option"
          )
          
          func main() {
          	client := openai.NewClient(
          		option.WithAPIKey("My API Key"),
          	)
          	deletedSkill, err := client.Skills.Delete(context.TODO(), "skill_123")
          	if err != nil {
          		panic(err.Error())
          	}
          	fmt.Printf("%+v
          ", deletedSkill.ID)
          }
          
          """,
        java: """
          package com.openai.example;
          
          import com.openai.client.OpenAIClient;
          import com.openai.client.okhttp.OpenAIOkHttpClient;
          import com.openai.models.skills.DeletedSkill;
          import com.openai.models.skills.SkillDeleteParams;
          
          public final class Main {
              private Main() {}
          
              public static void main(String[] args) {
                  OpenAIClient client = OpenAIOkHttpClient.fromEnv();
          
                  DeletedSkill deletedSkill = client.skills().delete("skill_123");
              }
          }
          """,
        ruby: """
          require "openai"
          
          openai = OpenAI::Client.new(api_key: "My API Key")
          
          deleted_skill = openai.skills.delete("skill_123")
          
          puts(deleted_skill)
          """,
      },
    },
  }
)
@tag("Skills")
op DeleteSkill(
  /** The identifier of the skill to delete. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  skill_id: string,
): DeletedSkillResource | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
};

#suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/response-schema-problem" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-names" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("Get a skill by its ID.")
@get
@route("/skills/{skill_id}")
@extension(
  "x-oaiMeta",
  #{
    examples: #{
      response: "",
      request: #{
        `node.js`: """
          import OpenAI from 'openai';
          
          const client = new OpenAI({
            apiKey: process.env['OPENAI_API_KEY'], // This is the default and can be omitted
          });
          
          const skill = await client.skills.retrieve('skill_123');
          
          console.log(skill.id);
          """,
        python: """
          import os
          from openai import OpenAI
          
          client = OpenAI(
              api_key=os.environ.get("OPENAI_API_KEY"),  # This is the default and can be omitted
          )
          skill = client.skills.retrieve(
              "skill_123",
          )
          print(skill.id)
          """,
        go: """
          package main
          
          import (
          	"context"
          	"fmt"
          
          	"github.com/openai/openai-go"
          	"github.com/openai/openai-go/option"
          )
          
          func main() {
          	client := openai.NewClient(
          		option.WithAPIKey("My API Key"),
          	)
          	skill, err := client.Skills.Get(context.TODO(), "skill_123")
          	if err != nil {
          		panic(err.Error())
          	}
          	fmt.Printf("%+v
          ", skill.ID)
          }
          
          """,
        java: """
          package com.openai.example;
          
          import com.openai.client.OpenAIClient;
          import com.openai.client.okhttp.OpenAIOkHttpClient;
          import com.openai.models.skills.Skill;
          import com.openai.models.skills.SkillRetrieveParams;
          
          public final class Main {
              private Main() {}
          
              public static void main(String[] args) {
                  OpenAIClient client = OpenAIOkHttpClient.fromEnv();
          
                  Skill skill = client.skills().retrieve("skill_123");
              }
          }
          """,
        ruby: """
          require "openai"
          
          openai = OpenAI::Client.new(api_key: "My API Key")
          
          skill = openai.skills.retrieve("skill_123")
          
          puts(skill)
          """,
      },
    },
  }
)
@tag("Skills")
op GetSkill(
  /** The identifier of the skill to retrieve. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  skill_id: string,
): SkillResource | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
};

#suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/response-schema-problem" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("Update the default version pointer for a skill.")
@post
@route("/skills/{skill_id}")
@extension(
  "x-oaiMeta",
  #{
    examples: #{
      response: "",
      request: #{
        `node.js`: """
          import OpenAI from 'openai';
          
          const client = new OpenAI({
            apiKey: process.env['OPENAI_API_KEY'], // This is the default and can be omitted
          });
          
          const skill = await client.skills.update('skill_123', { default_version: 'default_version' });
          
          console.log(skill.id);
          """,
        python: """
          import os
          from openai import OpenAI
          
          client = OpenAI(
              api_key=os.environ.get("OPENAI_API_KEY"),  # This is the default and can be omitted
          )
          skill = client.skills.update(
              skill_id="skill_123",
              default_version="default_version",
          )
          print(skill.id)
          """,
        go: """
          package main
          
          import (
          	"context"
          	"fmt"
          
          	"github.com/openai/openai-go"
          	"github.com/openai/openai-go/option"
          )
          
          func main() {
          	client := openai.NewClient(
          		option.WithAPIKey("My API Key"),
          	)
          	skill, err := client.Skills.Update(
          		context.TODO(),
          		"skill_123",
          		openai.SkillUpdateParams{
          			DefaultVersion: "default_version",
          		},
          	)
          	if err != nil {
          		panic(err.Error())
          	}
          	fmt.Printf("%+v
          ", skill.ID)
          }
          
          """,
        java: """
          package com.openai.example;
          
          import com.openai.client.OpenAIClient;
          import com.openai.client.okhttp.OpenAIOkHttpClient;
          import com.openai.models.skills.Skill;
          import com.openai.models.skills.SkillUpdateParams;
          
          public final class Main {
              private Main() {}
          
              public static void main(String[] args) {
                  OpenAIClient client = OpenAIOkHttpClient.fromEnv();
          
                  SkillUpdateParams params = SkillUpdateParams.builder()
                      .skillId("skill_123")
                      .defaultVersion("default_version")
                      .build();
                  Skill skill = client.skills().update(params);
              }
          }
          """,
        ruby: """
          require "openai"
          
          openai = OpenAI::Client.new(api_key: "My API Key")
          
          skill = openai.skills.update("skill_123", default_version: "default_version")
          
          puts(skill)
          """,
      },
    },
  }
)
@tag("Skills")
op UpdateSkillDefaultVersion(
  /** The identifier of the skill. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  skill_id: string,

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-closed-literal-union" "Auto-suppressed warnings non-applicable rules during import."
  @header
  contentType: "application/json" | "application/x-www-form-urlencoded",

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  @body
  body: SetDefaultSkillVersionBody,
): SkillResource | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
};

#suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/response-schema-problem" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-names" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("Download a skill zip bundle by its ID.")
@get
@route("/skills/{skill_id}/content")
@extension(
  "x-oaiMeta",
  #{
    examples: #{
      response: "",
      request: #{
        `node.js`: """
          import OpenAI from 'openai';
          
          const client = new OpenAI({
            apiKey: process.env['OPENAI_API_KEY'], // This is the default and can be omitted
          });
          
          const content = await client.skills.content.retrieve('skill_123');
          
          console.log(content);
          
          const data = await content.blob();
          console.log(data);
          """,
        python: """
          import os
          from openai import OpenAI
          
          client = OpenAI(
              api_key=os.environ.get("OPENAI_API_KEY"),  # This is the default and can be omitted
          )
          content = client.skills.content.retrieve(
              "skill_123",
          )
          print(content)
          data = content.read()
          print(data)
          """,
        go: """
          package main
          
          import (
          	"context"
          	"fmt"
          
          	"github.com/openai/openai-go"
          	"github.com/openai/openai-go/option"
          )
          
          func main() {
          	client := openai.NewClient(
          		option.WithAPIKey("My API Key"),
          	)
          	content, err := client.Skills.Content.Get(context.TODO(), "skill_123")
          	if err != nil {
          		panic(err.Error())
          	}
          	fmt.Printf("%+v
          ", content)
          }
          
          """,
        java: """
          package com.openai.example;
          
          import com.openai.client.OpenAIClient;
          import com.openai.client.okhttp.OpenAIOkHttpClient;
          import com.openai.core.http.HttpResponse;
          import com.openai.models.skills.content.ContentRetrieveParams;
          
          public final class Main {
              private Main() {}
          
              public static void main(String[] args) {
                  OpenAIClient client = OpenAIOkHttpClient.fromEnv();
          
                  HttpResponse content = client.skills().content().retrieve("skill_123");
              }
          }
          """,
        ruby: """
          require "openai"
          
          openai = OpenAI::Client.new(api_key: "My API Key")
          
          content = openai.skills.content.retrieve("skill_123")
          
          puts(content)
          """,
      },
    },
  }
)
@tag("Skills")
op GetSkillContent(
  /** The identifier of the skill to download. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  skill_id: string,
): {
  @header
  contentType: "application/zip";

  @body
  body: bytes;
} | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
};

#suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/response-schema-problem" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-names" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("List skill versions for a skill.")
@get
@route("/skills/{skill_id}/versions")
@extension(
  "x-oaiMeta",
  #{
    examples: #{
      response: "",
      request: #{
        `node.js`: """
          import OpenAI from 'openai';
          
          const client = new OpenAI({
            apiKey: process.env['OPENAI_API_KEY'], // This is the default and can be omitted
          });
          
          // Automatically fetches more pages as needed.
          for await (const skillVersion of client.skills.versions.list('skill_123')) {
            console.log(skillVersion.id);
          }
          """,
        python: """
          import os
          from openai import OpenAI
          
          client = OpenAI(
              api_key=os.environ.get("OPENAI_API_KEY"),  # This is the default and can be omitted
          )
          page = client.skills.versions.list(
              skill_id="skill_123",
          )
          page = page.data[0]
          print(page.id)
          """,
        go: """
          package main
          
          import (
          	"context"
          	"fmt"
          
          	"github.com/openai/openai-go"
          	"github.com/openai/openai-go/option"
          )
          
          func main() {
          	client := openai.NewClient(
          		option.WithAPIKey("My API Key"),
          	)
          	page, err := client.Skills.Versions.List(
          		context.TODO(),
          		"skill_123",
          		openai.SkillVersionListParams{},
          	)
          	if err != nil {
          		panic(err.Error())
          	}
          	fmt.Printf("%+v
          ", page)
          }
          
          """,
        java: """
          package com.openai.example;
          
          import com.openai.client.OpenAIClient;
          import com.openai.client.okhttp.OpenAIOkHttpClient;
          import com.openai.models.skills.versions.VersionListPage;
          import com.openai.models.skills.versions.VersionListParams;
          
          public final class Main {
              private Main() {}
          
              public static void main(String[] args) {
                  OpenAIClient client = OpenAIOkHttpClient.fromEnv();
          
                  VersionListPage page = client.skills().versions().list("skill_123");
              }
          }
          """,
        ruby: """
          require "openai"
          
          openai = OpenAI::Client.new(api_key: "My API Key")
          
          page = openai.skills.versions.list("skill_123")
          
          puts(page)
          """,
      },
    },
  }
)
@tag("Skills")
op ListSkillVersions(
  /** The identifier of the skill. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  skill_id: string,

  /** Number of versions to retrieve. */
  #suppress "@azure-tools/typespec-azure-core/no-query-explode" "Auto-suppressed warnings non-applicable rules during import."
  @query(#{ explode: true })
  @maxValue(100)
  @minValue(0)
  limit?: integer,

  /** Sort order of results by version number. */
  #suppress "@azure-tools/typespec-azure-core/no-query-explode" "Auto-suppressed warnings non-applicable rules during import."
  @query(#{ explode: true })
  order?: OrderEnum,

  /** The skill version ID to start after. */
  #suppress "@azure-tools/typespec-azure-core/no-query-explode" "Auto-suppressed warnings non-applicable rules during import."
  @query(#{ explode: true })
  after?: string,
): SkillVersionListResource | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
};

#suppress "@azure-tools/typespec-azure-core/byos" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/response-schema-problem" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("Create a new immutable skill version.")
@post
@route("/skills/{skill_id}/versions")
@extension(
  "x-oaiMeta",
  #{
    examples: #{
      response: "",
      request: #{
        `node.js`: """
          import OpenAI from 'openai';
          
          const client = new OpenAI({
            apiKey: process.env['OPENAI_API_KEY'], // This is the default and can be omitted
          });
          
          const skillVersion = await client.skills.versions.create('skill_123');
          
          console.log(skillVersion.id);
          """,
        python: """
          import os
          from openai import OpenAI
          
          client = OpenAI(
              api_key=os.environ.get("OPENAI_API_KEY"),  # This is the default and can be omitted
          )
          skill_version = client.skills.versions.create(
              skill_id="skill_123",
          )
          print(skill_version.id)
          """,
        go: """
          package main
          
          import (
          	"context"
          	"fmt"
          
          	"github.com/openai/openai-go"
          	"github.com/openai/openai-go/option"
          )
          
          func main() {
          	client := openai.NewClient(
          		option.WithAPIKey("My API Key"),
          	)
          	skillVersion, err := client.Skills.Versions.New(
          		context.TODO(),
          		"skill_123",
          		openai.SkillVersionNewParams{},
          	)
          	if err != nil {
          		panic(err.Error())
          	}
          	fmt.Printf("%+v
          ", skillVersion.ID)
          }
          
          """,
        java: """
          package com.openai.example;
          
          import com.openai.client.OpenAIClient;
          import com.openai.client.okhttp.OpenAIOkHttpClient;
          import com.openai.models.skills.versions.SkillVersion;
          import com.openai.models.skills.versions.VersionCreateParams;
          
          public final class Main {
              private Main() {}
          
              public static void main(String[] args) {
                  OpenAIClient client = OpenAIOkHttpClient.fromEnv();
          
                  SkillVersion skillVersion = client.skills().versions().create("skill_123");
              }
          }
          """,
        ruby: """
          require "openai"
          
          openai = OpenAI::Client.new(api_key: "My API Key")
          
          skill_version = openai.skills.versions.create("skill_123")
          
          puts(skill_version)
          """,
      },
    },
  }
)
@sharedRoute
@tag("Skills")
op CreateSkillVersionMultipart(
  /** The identifier of the skill to version. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  skill_id: string,

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  @header
  contentType: "multipart/form-data",

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  @multipartBody
  body: CreateSkillVersionBody,
): SkillVersionResource | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
};

#suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/response-schema-problem" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("Create a new immutable skill version.")
@post
@route("/skills/{skill_id}/versions")
@extension(
  "x-oaiMeta",
  #{
    examples: #{
      response: "",
      request: #{
        `node.js`: """
          import OpenAI from 'openai';
          
          const client = new OpenAI({
            apiKey: process.env['OPENAI_API_KEY'], // This is the default and can be omitted
          });
          
          const skillVersion = await client.skills.versions.create('skill_123');
          
          console.log(skillVersion.id);
          """,
        python: """
          import os
          from openai import OpenAI
          
          client = OpenAI(
              api_key=os.environ.get("OPENAI_API_KEY"),  # This is the default and can be omitted
          )
          skill_version = client.skills.versions.create(
              skill_id="skill_123",
          )
          print(skill_version.id)
          """,
        go: """
          package main
          
          import (
          	"context"
          	"fmt"
          
          	"github.com/openai/openai-go"
          	"github.com/openai/openai-go/option"
          )
          
          func main() {
          	client := openai.NewClient(
          		option.WithAPIKey("My API Key"),
          	)
          	skillVersion, err := client.Skills.Versions.New(
          		context.TODO(),
          		"skill_123",
          		openai.SkillVersionNewParams{},
          	)
          	if err != nil {
          		panic(err.Error())
          	}
          	fmt.Printf("%+v
          ", skillVersion.ID)
          }
          
          """,
        java: """
          package com.openai.example;
          
          import com.openai.client.OpenAIClient;
          import com.openai.client.okhttp.OpenAIOkHttpClient;
          import com.openai.models.skills.versions.SkillVersion;
          import com.openai.models.skills.versions.VersionCreateParams;
          
          public final class Main {
              private Main() {}
          
              public static void main(String[] args) {
                  OpenAIClient client = OpenAIOkHttpClient.fromEnv();
          
                  SkillVersion skillVersion = client.skills().versions().create("skill_123");
              }
          }
          """,
        ruby: """
          require "openai"
          
          openai = OpenAI::Client.new(api_key: "My API Key")
          
          skill_version = openai.skills.versions.create("skill_123")
          
          puts(skill_version)
          """,
      },
    },
  }
)
@sharedRoute
@tag("Skills")
op CreateSkillVersionJson(
  /** The identifier of the skill to version. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  skill_id: string,

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  @body
  body: CreateSkillVersionBody,
): SkillVersionResource | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
};

#suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/response-schema-problem" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-names" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("Delete a skill version.")
@delete
@route("/skills/{skill_id}/versions/{version}")
@extension(
  "x-oaiMeta",
  #{
    examples: #{
      response: "",
      request: #{
        `node.js`: """
          import OpenAI from 'openai';
          
          const client = new OpenAI({
            apiKey: process.env['OPENAI_API_KEY'], // This is the default and can be omitted
          });
          
          const deletedSkillVersion = await client.skills.versions.delete('version', {
            skill_id: 'skill_123',
          });
          
          console.log(deletedSkillVersion.id);
          """,
        python: """
          import os
          from openai import OpenAI
          
          client = OpenAI(
              api_key=os.environ.get("OPENAI_API_KEY"),  # This is the default and can be omitted
          )
          deleted_skill_version = client.skills.versions.delete(
              version="version",
              skill_id="skill_123",
          )
          print(deleted_skill_version.id)
          """,
        go: """
          package main
          
          import (
          	"context"
          	"fmt"
          
          	"github.com/openai/openai-go"
          	"github.com/openai/openai-go/option"
          )
          
          func main() {
          	client := openai.NewClient(
          		option.WithAPIKey("My API Key"),
          	)
          	deletedSkillVersion, err := client.Skills.Versions.Delete(
          		context.TODO(),
          		"skill_123",
          		"version",
          	)
          	if err != nil {
          		panic(err.Error())
          	}
          	fmt.Printf("%+v
          ", deletedSkillVersion.ID)
          }
          
          """,
        java: """
          package com.openai.example;
          
          import com.openai.client.OpenAIClient;
          import com.openai.client.okhttp.OpenAIOkHttpClient;
          import com.openai.models.skills.versions.DeletedSkillVersion;
          import com.openai.models.skills.versions.VersionDeleteParams;
          
          public final class Main {
              private Main() {}
          
              public static void main(String[] args) {
                  OpenAIClient client = OpenAIOkHttpClient.fromEnv();
          
                  VersionDeleteParams params = VersionDeleteParams.builder()
                      .skillId("skill_123")
                      .version("version")
                      .build();
                  DeletedSkillVersion deletedSkillVersion = client.skills().versions().delete(params);
              }
          }
          """,
        ruby: """
          require "openai"
          
          openai = OpenAI::Client.new(api_key: "My API Key")
          
          deleted_skill_version = openai.skills.versions.delete("version", skill_id: "skill_123")
          
          puts(deleted_skill_version)
          """,
      },
    },
  }
)
@tag("Skills")
op DeleteSkillVersion(
  /** The identifier of the skill. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  skill_id: string,

  /** The skill version number. */
  @path
  version: string,
): DeletedSkillVersionResource | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
};

#suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/response-schema-problem" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-names" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("Get a specific skill version.")
@get
@route("/skills/{skill_id}/versions/{version}")
@extension(
  "x-oaiMeta",
  #{
    examples: #{
      response: "",
      request: #{
        `node.js`: """
          import OpenAI from 'openai';
          
          const client = new OpenAI({
            apiKey: process.env['OPENAI_API_KEY'], // This is the default and can be omitted
          });
          
          const skillVersion = await client.skills.versions.retrieve('version', { skill_id: 'skill_123' });
          
          console.log(skillVersion.id);
          """,
        python: """
          import os
          from openai import OpenAI
          
          client = OpenAI(
              api_key=os.environ.get("OPENAI_API_KEY"),  # This is the default and can be omitted
          )
          skill_version = client.skills.versions.retrieve(
              version="version",
              skill_id="skill_123",
          )
          print(skill_version.id)
          """,
        go: """
          package main
          
          import (
          	"context"
          	"fmt"
          
          	"github.com/openai/openai-go"
          	"github.com/openai/openai-go/option"
          )
          
          func main() {
          	client := openai.NewClient(
          		option.WithAPIKey("My API Key"),
          	)
          	skillVersion, err := client.Skills.Versions.Get(
          		context.TODO(),
          		"skill_123",
          		"version",
          	)
          	if err != nil {
          		panic(err.Error())
          	}
          	fmt.Printf("%+v
          ", skillVersion.ID)
          }
          
          """,
        java: """
          package com.openai.example;
          
          import com.openai.client.OpenAIClient;
          import com.openai.client.okhttp.OpenAIOkHttpClient;
          import com.openai.models.skills.versions.SkillVersion;
          import com.openai.models.skills.versions.VersionRetrieveParams;
          
          public final class Main {
              private Main() {}
          
              public static void main(String[] args) {
                  OpenAIClient client = OpenAIOkHttpClient.fromEnv();
          
                  VersionRetrieveParams params = VersionRetrieveParams.builder()
                      .skillId("skill_123")
                      .version("version")
                      .build();
                  SkillVersion skillVersion = client.skills().versions().retrieve(params);
              }
          }
          """,
        ruby: """
          require "openai"
          
          openai = OpenAI::Client.new(api_key: "My API Key")
          
          skill_version = openai.skills.versions.retrieve("version", skill_id: "skill_123")
          
          puts(skill_version)
          """,
      },
    },
  }
)
@tag("Skills")
op GetSkillVersion(
  /** The identifier of the skill. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  skill_id: string,

  /** The version number to retrieve. */
  @path
  version: string,
): SkillVersionResource | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
};

#suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/response-schema-problem" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-names" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("Download a skill version zip bundle.")
@get
@route("/skills/{skill_id}/versions/{version}/content")
@extension(
  "x-oaiMeta",
  #{
    examples: #{
      response: "",
      request: #{
        `node.js`: """
          import OpenAI from 'openai';
          
          const client = new OpenAI({
            apiKey: process.env['OPENAI_API_KEY'], // This is the default and can be omitted
          });
          
          const content = await client.skills.versions.content.retrieve('version', { skill_id: 'skill_123' });
          
          console.log(content);
          
          const data = await content.blob();
          console.log(data);
          """,
        python: """
          import os
          from openai import OpenAI
          
          client = OpenAI(
              api_key=os.environ.get("OPENAI_API_KEY"),  # This is the default and can be omitted
          )
          content = client.skills.versions.content.retrieve(
              version="version",
              skill_id="skill_123",
          )
          print(content)
          data = content.read()
          print(data)
          """,
        go: """
          package main
          
          import (
          	"context"
          	"fmt"
          
          	"github.com/openai/openai-go"
          	"github.com/openai/openai-go/option"
          )
          
          func main() {
          	client := openai.NewClient(
          		option.WithAPIKey("My API Key"),
          	)
          	content, err := client.Skills.Versions.Content.Get(
          		context.TODO(),
          		"skill_123",
          		"version",
          	)
          	if err != nil {
          		panic(err.Error())
          	}
          	fmt.Printf("%+v
          ", content)
          }
          
          """,
        java: """
          package com.openai.example;
          
          import com.openai.client.OpenAIClient;
          import com.openai.client.okhttp.OpenAIOkHttpClient;
          import com.openai.core.http.HttpResponse;
          import com.openai.models.skills.versions.content.ContentRetrieveParams;
          
          public final class Main {
              private Main() {}
          
              public static void main(String[] args) {
                  OpenAIClient client = OpenAIOkHttpClient.fromEnv();
          
                  ContentRetrieveParams params = ContentRetrieveParams.builder()
                      .skillId("skill_123")
                      .version("version")
                      .build();
                  HttpResponse content = client.skills().versions().content().retrieve(params);
              }
          }
          """,
        ruby: """
          require "openai"
          
          openai = OpenAI::Client.new(api_key: "My API Key")
          
          content = openai.skills.versions.content.retrieve("version", skill_id: "skill_123")
          
          puts(content)
          """,
      },
    },
  }
)
@tag("Skills")
op GetSkillVersionContent(
  /** The identifier of the skill. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  skill_id: string,

  /** The skill version number. */
  @path
  version: string,
): {
  @header
  contentType: "application/zip";

  @body
  body: bytes;
} | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
};
