import "../../base/typespec/responses/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using Azure.ClientGenerator.Core;

namespace OpenAI;

@access(Access.public)
@usage(Usage.input | Usage.output)
@discriminator("type")
model DotnetResponseWebSearchLocation {
  ...Location;
}

@access(Access.public)
@usage(Usage.input | Usage.output)
model DotnetResponseWebSearchApproximateLocation
  extends DotnetResponseWebSearchLocation {
  ...ApproximateLocation;
}

@access(Access.public)
@usage(Usage.input | Usage.output)
model DotNetItemReferenceItemResource extends ItemResource {
  type: ItemType.item_reference;
  ...ItemReferenceItemBase;
}
