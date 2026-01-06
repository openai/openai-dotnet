# OpenAI Client Decomposition Refactor Plan

Goal: Split the monolithic OpenAI client library into discrete client-focused packages (e.g., OpenAI.Chat, OpenAI.Responses, OpenAI.Assistants) with OpenAI.Shared for cross-cutting code and a meta-package OpenAI that depends on all sub-packages.

## Guiding principles
- Preserve current public API behavior where possible; explicitly document any breaking changes.
- Keep code generation deterministic; prefer minimal manual file moves post-generation.
- Avoid duplicated shared code; centralize in OpenAI.Shared with clear dependency rules.
- Ensure build, packaging, and testing flows stay green after each incremental step.

## Target package layout
- OpenAI.Shared: HTTP pipeline abstractions, auth, configuration, retries, serialization, telemetry, logging, diagnostics, common primitives, model base types, JSON helpers.
- OpenAI.Chat: ChatClient, models, helpers, streaming; depends on OpenAI.Shared.
- OpenAI.Responses: ResponsesClient and related models; depends on OpenAI.Shared.
- OpenAI.Assistants: AssistantClient, VectorStores, Files-as-resources, related models; depends on OpenAI.Shared.
- OpenAI.Audio, OpenAI.Images, OpenAI.Embeddings, OpenAI.Moderations, OpenAI.Models, OpenAI.Batch, OpenAI.Realtime, etc., as needed by surface area; each depends on OpenAI.Shared only.
- OpenAI (meta): Depends on all client packages; provides compatibility shims if needed.

## Workstreams and incremental tasks

### 0) Decisions and scaffolding
- Confirm package IDs, namespaces, assembly names, and versioning strategy (shared version vs independent with meta tracking).
- Define dependency graph rules (clients depend on Shared only; no lateral client dependencies).
- Choose codegen handling strategy:
  - Option C (selected): Keep generated files in their original location (`src/Generated` etc.) and reference them in the new client projects using `<Compile Include="..." Link="..." />`. This avoids breaking the code generation process while still allowing for separate assemblies.
- Decide on transitive dependency behavior of meta-package (rollup only vs including thin facade APIs).

### 1) Repository layout and project setup
- Create per-client project folders (e.g., src/Chat/OpenAI.Chat.csproj) and src/Shared/OpenAI.Shared.csproj.
- Update OpenAI.sln to include new projects; ensure Directory.Build.props/targets still apply.
- Wire new projects to nuget.config/global.json settings; ensure code signing keys and assembly info flow.
- Add package metadata (Description, PackageTags, Authors) per csproj.

### 2) Code generation updates
- Inspect Invoke-CodeGen.ps1 and codegen/emitter outputs.
- Implement Option C:
  - Identify which files belong to which client package.
  - Update the new client `.csproj` files to include the relevant files from `src/Generated` and `src/Custom` using `<Compile Include="..." />`.
  - **Critical**: Ensure the main `OpenAI.csproj` (monolith) explicitly excludes (`<Compile Remove="..." />`) the files that are now included in the new client packages to avoid duplicate compilation and type collisions when it references the new packages.
- Update TSP specs or emitter configs to emit client-specific namespaces matching package IDs.
  - **Note**: Changing namespaces in the generator will immediately affect the files. If `OpenAI.csproj` still compiles these files directly (before full transition), this will be a breaking change. Coordinate generator updates with the transition of `OpenAI.csproj` to a meta-package or use temporary compatibility shims.

### 3) Shared extraction
- Identify shared code: authentication handlers, HttpClient setup, pipeline policies, serialization settings, diagnostics, error models, retry logic, primitives (request/response base types), utility helpers.
- Include in OpenAI.Shared (via `<Compile Include="..." />` or move if safe) with namespace updates; expose clear public surface for reuse.
- Add analyzers or shared internals (InternalsVisibleTo) only if unavoidable; prefer public abstractions over friend assemblies.
- Update remaining code to consume Shared types; remove duplicates.

### 4) Client package ownership
- For each client area (Chat, Responses, Assistants, Audio, Images, Embeddings, Moderations, Models, Batch, Realtime, etc.):
  - Create the project file (e.g. `src/Chat/OpenAI.Chat.csproj`).
  - Add `<Compile Include="..." />` items pointing to the generated and handwritten code in `src/Generated` and `src/Custom`.
  - Update namespaces to package-specific roots.
  - Ensure csproj includes only owned files.
  - Add package-specific resources (resx), analyzers, and strong-name settings.

### 5) Meta-package and compatibility
- Rework existing OpenAI.csproj as meta-package depending on all client packages (PackageReference with aligned versions).
- Provide optional compatibility shims if public surface expects unified entry point (e.g., forwarding types, factory helpers) or publish a `OpenAI.All` package.
- Plan deprecation messaging for monolithic package consumers; add README and obsoletion attributes if needed.

### 6) Testing strategy
- Split tests by package (tests/Chat, tests/Responses, etc.) with separate test projects or shared test infra library.
- Update test assets and recorded sessions to align with new namespaces.
- Ensure integration tests can run against meta-package to validate combined usage.
- Add API compatibility tests per package (Test-ApiCompatibility.ps1) and AOT where applicable.

### 7) Documentation and samples
- Update docs and guides to reference new packages and namespaces; add migration guide covering package splits and namespace changes.
- Update examples/ to target client packages; keep a meta-package example for existing users.
- Refresh README and quickstarts; ensure code snippets map to package IDs.

### 8) Engineering system
- Update build pipelines to build and pack all new csproj files; ensure artifacts publish all packages.
- Adjust Directory.Build.targets/props for multi-package scenarios (signing, versioning, analyzers, nullable, LangVersion, determinism).
- Update packaging steps (nuget pack/dotnet pack) and signing; ensure symbol packages map correctly.
- Update CI scripts: Test-AotCompatibility.ps1, Test-ApiCompatibility.ps1, Update-Snippets.ps1, Invoke-CodeGen.ps1 integration.
- Ensure `dotnet restore` and `dotnet build` at repo root succeed with new project graph; update global.json if new SDK requirements emerge.

### 9) Validation and rollout
- Run end-to-end builds and tests per package and with meta-package.
- Validate code signing, nupkg contents, dependency trees, and transitive closure.
- Perform staged beta releases; collect feedback; monitor telemetry for issues.
- Publish final migration guide and mark old monolithic package as superseded.

## Incremental execution proposal
1) Prototype: Create OpenAI.Shared and one client package (e.g., Chat) with minimal moves; prove build + tests.
2) Codegen alignment: Adapt generator or post-move script; lock down deterministic outputs.
3) Roll out remaining clients iteratively, one package at a time, keeping main green.
4) Introduce meta-package; publish prerelease; gather feedback.
5) Complete docs/tests migration; finalize deprecations; ship stable.

## Near-term task breakdown (Shared → Responses)

### Phase 1: Land OpenAI.Shared
- [x] Create project scaffolding: src/Shared/OpenAI.Shared.csproj with package metadata, signing, and align Directory.Build.props/targets.
- [x] Update OpenAI.sln to include OpenAI.Shared; ensure restore/build succeeds.
- [x] Decide codegen approach for Shared types (Option C: reference in place); prototype on a branch.
 - [x] Identify and move shared primitives: HTTP pipeline abstractions, auth/config, retry/backoff, serialization settings, diagnostics/logging, base models/errors, JSON helpers; adjust namespaces to OpenAI.Shared.*. (Status: moved DataEncodingHelpers, GenericActionPipelinePolicy, SemaphoreSlimExtensions, AppContextSwitchHelper, Async/SseUpdateCollection, Telemetry helpers, CodeGenVisibility attribute, Argument/ModelSerializationExtensions/TypeFormatters/SerializationFormat, Polyfill types into OpenAI.Shared. Decision: keep CustomSerializationHelpers/OpenAIContext compiled with the meta-package to avoid Shared depending on per-client model surfaces.)
- [x] Update referencing code to consume OpenAI.Shared; remove duplicated utilities; add InternalsVisibleTo only if unavoidable. (OpenAI.csproj now uses a ProjectReference to Shared, excludes duplicate linked files, and builds green.)
- [x] Add minimal tests for Shared (unit coverage for serialization/pipeline helpers) and ensure existing tests that depended on Shared types still pass. (Added tests/Shared/DataEncodingHelpersTests.cs; fixed UserAgent string regression in OpenAIClient.)
- [x] CI/build: verify pack produces OpenAI.Shared.nupkg (including symbols) and signing works; add to Test-ApiCompatibility.ps1/Test-AotCompatibility.ps1 matrices.
- [x] Docs: add short note in README or docs/overview describing Shared purpose and dependency rule (clients depend on Shared only).

### Phase 2: Introduce OpenAI.Responses client package
- [x] Create src/Responses/OpenAI.Responses.csproj with package metadata and dependency on OpenAI.Shared.
- [x] Adjust codegen for Responses: ensure files are generated with correct namespaces (OpenAI.Responses).
- [x] Configure `src/Responses/OpenAI.Responses.csproj` to include all types in the `OpenAI.Responses` namespace (both generated and handwritten) from `src/Generated` and `src/Custom`.
- [x] Update Directory.Build.props/targets inclusions if they assume monolithic paths; ensure analyzers/nullability still apply.
- [x] Update tests: create tests/Responses project or relocate relevant tests; update assets/recordings paths if needed; wire into CI test matrix.
- [x] Build/pack: verify OpenAI.Responses packs correctly with dependency on OpenAI.Shared; validate nuspec contents and symbols.
- [x] Integration: ensure monolithic OpenAI.csproj references OpenAI.Responses (and Shared) so existing consumers remain unbroken during transition.
  - [x] **Crucial**: Configure `OpenAI.csproj` to exclude all types in the `OpenAI.Responses` namespace (via `<Compile Remove="..." />`) to prevent duplicate type definitions.
- [x] Docs/samples: update relevant snippets and examples to show OpenAI.Responses usage; add migration note for Responses users.

### Checkpoints and decision gates
- Gate after Phase 1: Shared builds, packs, and tests pass; codegen story chosen; downstream code compiles.
- Gate after Phase 2: Responses package builds/packs, tests pass, meta-package still builds, and codegen pipeline remains deterministic.

## Open questions to resolve early
- Versioning: single version for all packages vs independent? (meta-package likely pins).
- Namespace plan: maintain OpenAI.* root but align per package (OpenAI.Chat, OpenAI.Assistants, etc.).
- How to handle shared models referenced across clients (e.g., model IDs, content blocks): live in Shared vs duplicated contracts?
- Do we want thin facades in meta-package for current entry-point ergonomics, or only dependency rollup?
- Recording assets: reuse shared assets or duplicate per package for isolation?
