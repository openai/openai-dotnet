import "./administration";
import "./audio";
import "./assistants";
import "./batch";
import "./chat";
import "./completions";
import "./containers";
import "./conversations";
import "./embeddings";
import "./evals";
import "./files";
import "./fine-tuning";
import "./graders";
import "./images";
import "./messages";
import "./models";
import "./moderations";
import "./realtime";
import "./runs";
import "./threads";
import "./responses";
import "./vector-stores";
import "./videos";
import "./uploads";
import "@typespec/openapi";
import "@typespec/http";
using TypeSpec.OpenAPI;
using TypeSpec.Http;
#suppress "@azure-tools/typespec-azure-core/require-versioned" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/auth-required" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@tagMetadata(
  "Audit Logs",
  #{
    description: "List user actions and configuration changes within this organization.",
  }
)
@tagMetadata(
  "Moderations",
  #{
    description: "Given text and/or image inputs, classifies if those inputs are potentially harmful.",
  }
)
@tagMetadata(
  "Models",
  #{ description: "List and describe the various models available in the API." }
)
@tagMetadata(
  "Images",
  #{
    description: "Given a prompt and/or an input image, the model will generate a new image.",
  }
)
@tagMetadata(
  "Uploads",
  #{ description: "Use Uploads to upload large files in multiple parts." }
)
@tagMetadata(
  "Files",
  #{
    description: "Files are used to upload documents that can be used with features like Assistants and Fine-tuning.",
  }
)
@tagMetadata(
  "Batch",
  #{
    description: "Create large batches of API requests to run asynchronously.",
  }
)
@tagMetadata(
  "Graders",
  #{ description: "Manage and run graders in the OpenAI platform." }
)
@tagMetadata(
  "Fine-tuning",
  #{
    description: "Manage fine-tuning jobs to tailor a model to your specific training data.",
  }
)
@tagMetadata(
  "Evals",
  #{ description: "Manage and run evals in the OpenAI platform." }
)
@tagMetadata(
  "Embeddings",
  #{
    description: "Get a vector representation of a given input that can be easily consumed by machine learning models and algorithms.",
  }
)
@tagMetadata(
  "Completions",
  #{
    description: "Given a prompt, the model will return one or more predicted completions, and can also return the probabilities of alternative tokens at each position.",
  }
)
@tagMetadata(
  "Conversations",
  #{ description: "Manage conversations and conversation items." }
)
@tagMetadata(
  "Chat",
  #{
    description: "Given a list of messages comprising a conversation, the model will return a response.",
  }
)
@tagMetadata(
  "Audio",
  #{ description: "Turn audio into text or text into audio." }
)
@tagMetadata(
  "Assistants",
  #{ description: "Build Assistants that can call models and use tools." }
)
/** The OpenAI REST API. Please see https://platform.openai.com/docs/api-reference for more details. */
@OpenAPI.info(#{
  termsOfService: "https://openai.com/policies/terms-of-use",
  contact: #{ name: "OpenAI Support", url: "https://help.openai.com" },
  license: #{
    name: "MIT",
    url: "https://github.com/openai/openai-openapi/blob/master/LICENSE",
  },
})

@service(#{ title: "OpenAI API" })
@server("wss://api.openai.com/v1", "OpenAI Endpoint")
@useAuth(BearerAuth)
namespace OpenAI;
