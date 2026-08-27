import "./responses";
import "@typespec/openapi";
import "@typespec/http";
using TypeSpec.OpenAPI;
using TypeSpec.Http;

@service(#{ title: "OpenAI API Responses" })
@server("wss://api.openai.com/v1", "OpenAI Endpoint")
@useAuth(BearerAuth)
namespace OpenAI;