// Import order matters. Somehow, importing the server decorated namespace afterwards,
// will cause the the `description` field to be missing in the generated OpenAPI spec.
import "./specialized-types.tsp";
import "../../typespec/servers/rest.tsp";
import "../../typespec";
