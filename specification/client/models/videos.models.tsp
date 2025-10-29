import "../../base/typespec/videos/main.tsp";
import "./common.models.tsp";
import "@azure-tools/typespec-client-generator-core";

using Azure.ClientGenerator.Core;
using TypeSpec.Http;

namespace OpenAI;

alias VideoCollectionOrderQueryParameter = {
   /**
    * Sort order by the `created_at` timestamp of the objects. `asc` for ascending order and`desc`
    * for descending order.
    */
   @query order?: VideoCollectionOrder;
};

union VideoCollectionOrder {
   string,
   Ascending: "asc",
   Descending: "desc",
}

@access(Access.public)
@usage(Usage.input)
model VideoCollectionOptions {
  ...DotNetCollectionAfterQueryParameter,
  ...DotNetCollectionLimitQueryParameter,
  ...VideoCollectionOrderQueryParameter,
}
