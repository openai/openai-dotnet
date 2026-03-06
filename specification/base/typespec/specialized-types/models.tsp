/*
 * This file was automatically generated from an OpenAPI .yaml file.
 * Edits made directly to this file will be lost.
 */

import "./custom.tsp";

using Http;
using TypeSpec.OpenAPI;

namespace OpenAI;

/** A filter used to compare a specified attribute key to a given value using a defined comparison operation. */
model ComparisonFilter {
  @doc("""
    Specifies the comparison operator: `eq`, `ne`, `gt`, `gte`, `lt`, `lte`.
    - `eq`: equals
    - `ne`: not equal
    - `gt`: greater than
    - `gte`: greater than or equal
    - `lt`: less than
    - `lte`: less than or equal
    """)
  type: "eq" | "ne" | "gt" | "gte" | "lt" | "lte" = "eq";

  /** The key to compare against the value. */
  key: string;

  /** The value to compare against the attribute key; supports string, number, or boolean types. */
  value: string | float32 | boolean;
}

@doc("""
  Combine multiple filters using `and` or `or`.
  """)
model CompoundFilter {
  @doc("""
    Type of operation: `and` or `or`.
    """)
  type: "and" | "or";

  @doc("""
    Array of filters to combine. Items can be `ComparisonFilter` or `CompoundFilter`.
    """)
  filters: (ComparisonFilter | CompoundFilter)[];
}

alias VectorStoreFileAttributes = Record<unknown>;