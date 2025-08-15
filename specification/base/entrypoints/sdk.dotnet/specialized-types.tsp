namespace OpenAI;

// .NET code generation doesn't yet support many-to-one mappings of discriminators; here, we substitute one-to-one
// model mappings that would unnecessarily complicate OpenAPI emission.

@discriminator("type")
model ComparisonFilter {
  type: "eq" | "ne" | "gt" | "gte" | "lt" | "lte";
  key: string;
  value: string | float32 | boolean;
}

model ComparisonFilterEquals extends ComparisonFilter {
  type: "eq";
}
model ComparisonFilterNotEquals extends ComparisonFilter {
  type: "ne";
}
model ComparisonFilterGreaterThan extends ComparisonFilter {
  type: "gt";
}
model ComparisonFilterGreaterThanOrEquals extends ComparisonFilter {
  type: "gte";
}
model ComparisonFilterLessThan extends ComparisonFilter {
  type: "lt";
}
model ComparisonFilterLessThanOrEquals extends ComparisonFilter {
  type: "lte";
}

@discriminator("type")
model CompoundFilter {
  type: "and" | "or";
  filters: (ComparisonFilter | CompoundFilter)[];
}
model CompoundFilterAnd extends CompoundFilter {
  type: "and";
}
model CompoundFilterOr extends CompoundFilter {
  type: "or";
}

alias VectorStoreFileAttributes = Record<unknown>;
