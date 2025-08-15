import "../common";

using TypeSpec.OpenAPI;

namespace OpenAI;

@minValue(1)
@maxValue(10)
scalar OneToTenInt extends int32;
