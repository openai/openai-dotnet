import "@typespec/http";
import "@typespec/openapi";

using TypeSpec.Http;

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
@server("https://api.openai.com/v1", "OpenAI Endpoint")
@useAuth(BearerAuth)
namespace OpenAI;
