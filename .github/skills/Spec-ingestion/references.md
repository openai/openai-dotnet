# Reference PRs

Previously completed spec ingestion PRs that demonstrate the full workflow and common issues encountered.

## Summary Table

| Area | PR | Status | Notes |
|------|----|--------|-------|
| Embeddings, Images, Models | [#856](https://github.com/openai/openai-dotnet/pull/856) | Merged | Added `NumericPropertiesVisitor`, `ItemsPropertyVisitor`, new image properties |
| Moderations | [#888](https://github.com/openai/openai-dotnet/pull/888) | Merged | Added multimodal input support for text and image moderation |
| Files | [#894](https://github.com/openai/openai-dotnet/pull/894) | Merged | Updated file operations, pagination changes deferred to separate PR |
| Audio | [#913](https://github.com/openai/openai-dotnet/pull/913) | Merged | Streaming speech, diarization, usage metadata; new features deferred to follow-up PRs |
| VectorStore | [#935](https://github.com/openai/openai-dotnet/pull/935) | Merged | Parameter type changes (`long` → `int`), model reorganization, `NumericTypesVisitor` |

---

## Detailed Notes per PR

### PR #856 — Embeddings, Images, and Models

- **Author:** @joseharriaga
- **Key changes:**
  - Ingested latest TypeSpec for Embeddings, Images, and Models
  - Added `NumericPropertiesVisitor` to handle TypeSpec's `integer` type (maps to `long` by default, visitor converts to `int`)
  - Added `ItemsPropertyVisitor` to handle serialization of classes customized to inherit from `ReadOnlyCollection<T>`
  - Added `Background`, `OutputFileFormat`, `Quality`, `Size` properties to `GeneratedImageCollection`
  - Disambiguated between `HD` and `High` values in the `GeneratedImageQuality` enum
  - Added `InputFidelity`, `OutputCompressionFactor`, `OutputFileFormat` properties to `ImageEditOptions`
  - Added `OutputTokenDetails` property to `ImageTokenUsage`
- **Lessons learned:**
  - Numeric type handling required a new codegen visitor (NumericPropertiesVisitor)
  - Collection-based custom types needed special serialization handling (ItemsPropertyVisitor)

### PR #888 — Moderations

- **Author:** @ShivangiReja
- **Key changes:**
  - Ingested latest TypeSpec for Moderations
  - Added support for multimodal input (text + image content moderation)
- **Intermediate commits:** Spec update → Export API → Update latest TypeSpec → Add missing `[Experimental]` → Fix `ModerationInputPart` → Remove extra namespace
- **Lessons learned:**
  - Multiple iterations needed to fix client code after spec update
  - `[Experimental]` attributes needed for new features
  - Namespace cleanup may be required

### PR #894 — Files

- **Author:** @ShivangiReja
- **Key changes:**
  - Ingested latest TypeSpec for Files
  - Deferred pagination methods for `GetFiles` to follow-up PR [#895](https://github.com/openai/openai-dotnet/issues/895)
  - Deferred `ExpiresAfter` exposure in `OpenAIFileClient.UploadFileAsync()` to follow-up PR [#896](https://github.com/openai/openai-dotnet/issues/896)
  - Made `SizeInBytesLong` nullable
  - Removed old spec files
- **Lessons learned:**
  - Complex new features (pagination, new parameters) should be split into follow-up PRs
  - Test fixes may be needed after spec ingestion

### PR #913 — Audio

- **Author:** @ShivangiReja
- **Key changes:**
  - Ingested latest TypeSpec for Audio
  - Extended audio transcription models with diarized JSON responses, token/duration usage details
  - Introduced streaming speech response models/events (delta/done)
  - Added `stream_format` and corresponding .NET model surfaces
  - Updated TypeSpec to reflect new streaming/usage shapes
  - Removed obsolete constructs
- **Follow-up work deferred:**
  - Speech streaming events: [#914](https://github.com/openai/openai-dotnet/issues/914)
  - Diarized transcription response: [#916](https://github.com/openai/openai-dotnet/issues/916)
- **Lessons learned:**
  - New streaming event types require discriminated union wrappers in client models TSP
  - Internal stubs (`GeneratorStubs.cs`) needed for new transcription and speech types
  - Complex features (streaming, diarization) best deferred to follow-up PRs

### PR #935 — VectorStore

- **Author:** @ShivangiReja
- **Key changes:**
  - Ingested latest TypeSpec for VectorStore
  - Changed `limit` parameter types from `long?` to `int?` across multiple clients
  - Added `Description` property to `VectorStoreCreationOptions`
  - Reorganized VectorStore methods and updated TypeSpec definitions
  - Replaced specialized comparison/compound filter discriminated models with single base models
  - Renamed and enhanced `NumericPropertiesVisitor` to `NumericTypesVisitor` with broader scope (handles fields and methods too)
- **Lessons learned:**
  - Parameter type changes can cascade across multiple clients
  - Codegen visitors may need enhancements for new patterns
  - `git mv` doesn't always detect renames when content changes significantly

---

## Follow-up Issues Created from Ingestion PRs

| Issue | Created From | Description |
|-------|-------------|-------------|
| [#895](https://github.com/openai/openai-dotnet/issues/895) | Files #894 | Add pagination methods for `GetFiles` |
| [#896](https://github.com/openai/openai-dotnet/issues/896) | Files #894 | Expose `ExpiresAfter` in `UploadFileAsync()` |
| [#914](https://github.com/openai/openai-dotnet/issues/914) | Audio #913 | Add support for speech streaming events |
| [#916](https://github.com/openai/openai-dotnet/issues/916) | Audio #913 | Add support for diarized transcription response |
