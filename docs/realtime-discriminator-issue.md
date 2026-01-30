# Realtime API TypeSpec uses unions instead of discriminators for polymorphic model types

## Summary

The Realtime API TypeSpec specification at `microsoft/openai-openapi-pr` uses TypeSpec union types in several places where discriminated model hierarchies would be more appropriate. When unions are used for polymorphic model types, the C# code generator produces `BinaryData` properties instead of strongly-typed models, degrading the developer experience and type safety.

## Background

TypeSpec supports two patterns for polymorphic types:

1. **Union types**: `PropertyType | OtherType` — generates loosely-typed properties (often `BinaryData` in C#)
2. **Discriminated models**: `@discriminator("type") model Base { type: string; }` with derived models — generates strongly-typed inheritance hierarchies

The current spec correctly uses the discriminator pattern for some types (e.g., `RealtimeClientEvent`, `RealtimeServerEvent`, `RealtimeConversationItem`, `RealtimeMCPError`) but inconsistently uses raw unions for others.

## Problem Areas

### 1. Session Configuration Types in Events

The `session.created` and `session.updated` events use inline unions:

```typespec
session: RealtimeSessionCreateRequestGA | RealtimeTranscriptionSessionCreateRequestGA;
```

This should instead use a discriminated base model with `type: "realtime" | "transcription"` as the discriminator, allowing consumers to work with strongly-typed session objects.

### 2. Tool Arrays

Tool collections use unions:

```typespec
tools?: (RealtimeFunctionTool | MCPTool)[];
```

Without a discriminator, the generator cannot produce a strongly-typed `RealtimeTool` base class that supports both function tools and MCP tools.

### 3. Tool Choice Types

Tool choice is specified as:

```typespec
tool_choice?: ToolChoiceOptions | ToolChoiceFunction | ToolChoiceMCP;
```

This should be a discriminated model hierarchy to enable type-safe handling of the different tool choice modes.

### 4. Content Part Types

Content parts in `RealtimeServerEventResponseContentPartAddedPart` and similar models have:

```typespec
type?: "text" | "audio";
```

But are defined as flat models rather than a discriminated hierarchy. The `type` field suggests polymorphism but without the discriminator decorator, the generator cannot leverage it.

### 5. Transcription Usage Types

The usage field uses:

```typespec
usage: TranscriptTextUsageTokens | TranscriptTextUsageDuration;
```

This should be a discriminated union based on a `type` field to allow type-safe access to either tokens-based or duration-based billing information.

### 6. Turn Detection Types

Turn detection configuration appears in multiple forms:
- Inline anonymous objects with `type?: string`
- `RealtimeTurnDetection` with `@discriminator("type")`

The spec should consistently use the discriminated model pattern for all turn detection configurations, including `server_vad` and `semantic_vad` subtypes.

### 7. Message Content Types

`RealtimeConversationItemMessageUserContent` and similar models have:

```typespec
type?: "input_text" | "input_audio" | "input_image";
```

But lack a proper discriminated hierarchy, making it difficult to work with different content types in a type-safe manner.

### 8. Session Create Request/Response Unions

The types `RealtimeSessionCreateRequestUnion` and `RealtimeSessionCreateResponseUnion` have discriminator decorators but their child types (`RealtimeSessionCreateRequestGA`, `RealtimeTranscriptionSessionCreateRequestGA`, etc.) are used inconsistently throughout the spec.

## Impact

When unions are used instead of discriminated models:

1. **C# consumers receive `BinaryData`** — Developers must manually deserialize and inspect JSON to determine the actual type
2. **No IntelliSense support** — IDE cannot provide property completion for polymorphic types
3. **Runtime type checking required** — What should be compile-time type safety becomes runtime inspection
4. **Inconsistent API surface** — Some parts of the API are strongly typed while others require manual JSON handling

## Recommendation

Audit all union usages in the spec and convert those representing discrete, mutually-exclusive model types to discriminated model hierarchies. The discriminator pattern should be applied to:

- Session configuration types (realtime vs. transcription)
- Tool types (function vs. MCP)
- Tool choice types
- Content part types
- Usage/billing types
- Turn detection configuration types
- Message content types

This will enable code generators to produce strongly-typed models throughout the Realtime API surface, matching the pattern already established for events and conversation items.
