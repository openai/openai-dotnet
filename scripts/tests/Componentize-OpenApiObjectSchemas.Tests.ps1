<#
.SYNOPSIS
    Runs focused tests for scripts/Componentize-OpenApiObjectSchemas.ps1.

.DESCRIPTION
    Exercises candidate discovery, semantic naming, pointer relocation,
    component materialization, input validation, and structural/byte idempotence.
    The suite is dependency-free so it can run directly under PowerShell 7:

        pwsh ./scripts/tests/Componentize-OpenApiObjectSchemas.Tests.ps1
#>

[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$scriptDirectory = Join-Path $PSScriptRoot ".."
$componentizeScriptPath = Join-Path $scriptDirectory "Componentize-OpenApiObjectSchemas.ps1"

# Dot-source the command under test so its pure conversion functions can be
# exercised without file I/O. The script's invocation guard must prevent its CLI
# entry point from running during this load; the first test locks that contract.
. $componentizeScriptPath
. (Join-Path $PSScriptRoot "_OpenApiTestFixture.Helpers.ps1")

$script:PassedTests = 0
$script:FailedTests = 0
$script:Failures = [System.Collections.Generic.List[string]]::new()

function Invoke-Test {
    param([string]$Name, [scriptblock]$Test)
    try {
        & $Test
        $script:PassedTests++
        Write-Host "[PASS] $Name"
    }
    catch {
        $script:FailedTests++
        $message = "[FAIL] $Name`: $($_.Exception.Message)"
        $script:Failures.Add($message)
        Write-Host $message -ForegroundColor Red
    }
}

function Assert-True {
    param([bool]$Condition, [string]$Message)
    if (-not $Condition) { throw $Message }
}

function Assert-Equal {
    param([AllowNull()][object]$Expected, [AllowNull()][object]$Actual, [string]$Message)
    if ($Expected -ne $Actual) { throw "$Message Expected '$Expected', got '$Actual'." }
}

function Assert-Sequence {
    param(
        [AllowEmptyCollection()][object[]]$Expected,
        [AllowEmptyCollection()][object[]]$Actual,
        [string]$Message
    )
    $expectedJson = ConvertTo-Json -InputObject @($Expected) -Compress -Depth 100
    $actualJson = ConvertTo-Json -InputObject @($Actual) -Compress -Depth 100
    if (-not [string]::Equals($expectedJson, $actualJson, [System.StringComparison]::Ordinal)) {
        throw "$Message Expected '$expectedJson', got '$actualJson'."
    }
}

function Assert-JsonEqual {
    param([object]$Expected, [object]$Actual, [string]$Message)
    if (-not (Test-JsonValuesEqual -Left $Expected -Right $Actual)) {
        throw $Message
    }
}

function Assert-Throws {
    param([scriptblock]$Action, [string]$MessagePattern)
    try { & $Action }
    catch {
        if ($_.Exception.Message -notlike $MessagePattern) {
            throw "Expected error matching '$MessagePattern', got '$($_.Exception.Message)'."
        }
        return
    }
    throw "Expected an error matching '$MessagePattern', but no error was thrown."
}

function Assert-BytesEqual {
    param([byte[]]$Expected, [byte[]]$Actual, [string]$Message)
    if (-not [System.Linq.Enumerable]::SequenceEqual($Expected, $Actual)) {
        throw $Message
    }
}

# The compact fixture intentionally combines one root request object, nested
# properties, additionalProperties, repeated oneOf branches, an existing schema
# root, source-authored refs/mappings, and literal lookalikes. This gives the
# golden test broad coverage while the smaller mutation tests isolate decisions.
$fixtureDirectory = Join-Path $PSScriptRoot "fixtures" "Componentize-OpenApiObjectSchemas"
$sourceFixturePath = Join-Path $fixtureDirectory "source.openapi.json"
$expectedFixturePath = Join-Path $fixtureDirectory "expected.componentized.openapi.json"
$temporaryDirectory = Join-Path ([System.IO.Path]::GetTempPath()) "componentize-openapi-$([Guid]::NewGuid().ToString('N'))"
$null = [System.IO.Directory]::CreateDirectory($temporaryDirectory)

try {
    # Tests consume the script as a function library; loading it must not write
    # the canonical production artifact.
    Invoke-Test "dot-sourcing exposes componentizer functions without running the CLI" {
        Assert-True ($null -ne (Get-Command ConvertTo-ComponentizedOpenApiDocument -ErrorAction SilentlyContinue)) "Conversion function was not defined."
        Assert-True ($null -ne (Get-Command Invoke-OpenApiObjectSchemaComponentizer -ErrorAction SilentlyContinue)) "CLI function was not defined."
        Assert-Equal `
            ([System.IO.Path]::GetFullPath("./specification/openapi/responses.openapi.json")) `
            $script:DefaultComponentizerInputPath `
            "Canonical input path was incorrect."
        Assert-Equal `
            ([System.IO.Path]::GetFullPath("./specification/openapi/responses.componentized.openapi.json")) `
            $script:DefaultComponentizerOutputPath `
            "Canonical output path was incorrect."
    }

    # Discovery is schema-aware and parent-first: component roots and literal
    # lookalikes are excluded, while a direct oneOf owner precedes its branches.
    Invoke-Test "candidate discovery finds each inline schema occurrence exactly once" {
        $source = Read-OpenApiDocument -Path $sourceFixturePath
        $candidates = @(Get-InlineOpenApiSchemaCandidates -Document $source)
        Assert-Equal 8 $candidates.Count "Candidate count was incorrect."
        Assert-Sequence `
            @("Object", "Object", "Object", "Object", "OneOf", "Object", "Object", "Object") `
            @($candidates.Kind) `
            "Candidate kinds or encounter order were incorrect."
        Assert-Sequence `
            @(0, 1, 2, 1, 1, 2, 2, 0) `
            @($candidates.CandidateDepth) `
            "Candidate nesting depths were incorrect."
        Assert-Equal 3 @($candidates | Where-Object { $null -ne $_.TypeEnumValue }).Count "Singleton enum count was incorrect."
        Assert-True (-not ($candidates.Pointer -ccontains "#/components/schemas/ExistingRoot")) "Existing component root was rediscovered."
    }

    # The golden locks one-component-per-occurrence semantics, deterministic
    # names, nested reference chains, relocation, and existing component order.
    Invoke-Test "compact fixture matches its componentized golden" {
        $source = Read-OpenApiDocument -Path $sourceFixturePath
        $expected = Read-OpenApiDocument -Path $expectedFixturePath
        $sourceBefore = ConvertTo-Json -InputObject $source -Compress -Depth 100
        $result = ConvertTo-ComponentizedOpenApiDocument -Document $source
        Assert-Equal 7 $result.DiscoveredObjectSchemaCount "Discovery statistic was incorrect."
        Assert-Equal 1 $result.DiscoveredOneOfSchemaCount "oneOf discovery statistic was incorrect."
        Assert-Equal 8 $result.DiscoveredSchemaCount "Total discovery statistic was incorrect."
        Assert-Equal 8 $result.GeneratedComponentCount "Generated statistic was incorrect."
        Assert-Equal 8 $result.RelocationCount "Relocation statistic was incorrect."
        Assert-JsonEqual $expected $result.Document "Componentized fixture did not match its golden."
        Assert-Equal $sourceBefore (ConvertTo-Json -InputObject $source -Compress -Depth 100) "Conversion mutated its input."
    }

    # Names combine underscore-separated operation/property context and direct
    # singleton type enums; only colliding bases receive numeric suffixes.
    Invoke-Test "component names are semantic enum-aware and suffix only collisions" {
        $source = Read-OpenApiDocument -Path $sourceFixturePath
        $result = ConvertTo-ComponentizedOpenApiDocument -Document $source
        $schemas = $result.Document.components.schemas
        Assert-Sequence @(
            "ExistingRoot",
            "CreateWidget_Request",
            "CreateWidget_Request_Child_TypeOfChildKind",
            "CreateWidget_Request_Child",
            "CreateWidget_Request_List",
            "CreateWidget_Request_Variants",
            "CreateWidget_Request_Variants_TypeOfAlpha",
            "CreateWidget_Request_Variants_TypeOfAlpha_2",
            "ExistingRoot_Nested"
        ) @((Get-JsonProperties -Value $schemas).Name) "Generated component names or order were incorrect."
    }

    # A direct oneOf owner claims the ordinary semantic path name before its
    # branches. A branch without a singleton type enum then uses the same base
    # name and receives the normal numeric collision suffix.
    Invoke-Test "oneOf owners precede branches and use ordinary collision suffixes" {
        $source = Read-TestJsonFixture `
            -RelativePath "Componentize-OpenApiObjectSchemas/source.openapi.json"
        $source.paths["/widgets"].post.requestBody.content["application/json"].schema.properties["toolChoice"] = [ordered]@{
            oneOf = @(
                [ordered]@{
                    type = "string"
                    enum = @("auto", "none")
                },
                [ordered]@{
                    type = "object"
                    properties = [ordered]@{
                        type = [ordered]@{
                            type = "string"
                            enum = @("file_search", "web_search")
                        }
                    }
                }
            )
        }

        $result = ConvertTo-ComponentizedOpenApiDocument -Document $source
        $wrapper = @($result.Candidates | Where-Object {
            $_.Pointer.EndsWith("/properties/toolChoice", [System.StringComparison]::Ordinal)
        })
        $branch = @($result.Candidates | Where-Object {
            $_.Pointer.EndsWith("/properties/toolChoice/oneOf/1", [System.StringComparison]::Ordinal)
        })
        Assert-Equal 1 $wrapper.Count "Direct oneOf owner was not discovered exactly once."
        Assert-Equal "OneOf" $wrapper[0].Kind "Direct oneOf owner had the wrong candidate kind."
        Assert-Equal "CreateWidget_Request_ToolChoice" $wrapper[0].Name "Direct oneOf owner did not claim the semantic base name."
        Assert-Equal "CreateWidget_Request_ToolChoice_2" $branch[0].Name "Object branch did not receive the numeric collision suffix."
        Assert-Equal `
            "#/components/schemas/CreateWidget_Request_ToolChoice" `
            $result.Document.components.schemas.CreateWidget_Request.properties.toolChoice.'$ref' `
            "Direct oneOf owner was not replaced by its component reference."
        Assert-Equal `
            "#/components/schemas/CreateWidget_Request_ToolChoice_2" `
            $result.Document.components.schemas.CreateWidget_Request_ToolChoice.oneOf[1].'$ref' `
            "Componentized oneOf did not reference its object branch."
    }

    # The candidate is the complete Schema Object, so siblings of oneOf move into
    # the component together. Direct oneOf eligibility wins over type=object and
    # prevents the same source occurrence from producing two components.
    Invoke-Test "oneOf components preserve sibling schema keywords" {
        $source = Read-TestJsonFixture `
            -RelativePath "Componentize-OpenApiObjectSchemas/source.openapi.json"
        $source.paths["/widgets"].post.requestBody.content["application/json"].schema.properties["hybrid"] = [ordered]@{
            type = "object"
            description = "A constrained union."
            maxProperties = 2
            discriminator = [ordered]@{ propertyName = "type" }
            oneOf = @(
                [ordered]@{
                    type = "object"
                    required = @("type")
                    properties = [ordered]@{
                        type = [ordered]@{ type = "string"; enum = @("first") }
                    }
                },
                [ordered]@{
                    type = "object"
                    required = @("type")
                    properties = [ordered]@{
                        type = [ordered]@{ type = "string"; enum = @("second") }
                    }
                }
            )
        }

        $result = ConvertTo-ComponentizedOpenApiDocument -Document $source
        $ownerPointer = "#/paths/~1widgets/post/requestBody/content/application~1json/schema/properties/hybrid"
        $owners = @($result.Candidates | Where-Object Pointer -eq $ownerPointer)
        Assert-Equal 1 $owners.Count "Object/oneOf schema produced duplicate candidates."
        Assert-Equal "OneOf" $owners[0].Kind "Direct oneOf did not take precedence over type=object."
        Assert-Sequence @('$ref') @((Get-JsonProperties -Value $result.Document.components.schemas.CreateWidget_Request.properties.hybrid).Name) "Inline oneOf siblings remained beside the generated ref."
        $component = $result.Document.components.schemas.CreateWidget_Request_Hybrid
        Assert-Equal "object" $component.type "Type sibling did not move with oneOf."
        Assert-Equal "A constrained union." $component.description "Description sibling did not move with oneOf."
        Assert-Equal 2 $component.maxProperties "Constraint sibling did not move with oneOf."
        Assert-Equal "type" $component.discriminator.propertyName "Discriminator sibling did not move with oneOf."
        Assert-Equal 2 $component.oneOf.Count "oneOf branches changed during componentization."
    }

    # A nested candidate inherits the containing candidate's semantic enum name,
    # not only literal source-pointer tokens around the nested occurrence. This
    # synthetic extension models a production shape like:
    #   variant(type=alpha) -> tools[] -> branch(type=function)
    # and proves `TypeOfAlpha`, which is absent from the raw child pointer, stays
    # in the child's generated name.
    Invoke-Test "nested names inherit enum-derived parent context" {
        $source = Read-TestJsonFixture `
            -RelativePath "Componentize-OpenApiObjectSchemas/source.openapi.json"
        $variant = $source.paths["/widgets"].post.requestBody.content["application/json"].schema.properties.variants.oneOf[0]
        $variant.properties["tools"] = [ordered]@{
            type = "array"
            items = [ordered]@{
                oneOf = @(
                    [ordered]@{
                        type = "object"
                        properties = [ordered]@{
                            type = [ordered]@{
                                type = "string"
                                enum = @("function")
                            }
                        }
                    }
                )
            }
        }

        $result = ConvertTo-ComponentizedOpenApiDocument -Document $source
        $nested = @($result.Candidates | Where-Object {
            $_.Pointer.EndsWith("/properties/tools/items/oneOf/0", [System.StringComparison]::Ordinal)
        })[0]
        Assert-Equal `
            "CreateWidget_Request_Variants_TypeOfAlpha_Tools_TypeOfFunction" `
            $nested.Name `
            "Nested component omitted its enum-derived parent name."
    }

    # Identical oneOf branches intentionally remain separate definitions with
    # distinct names and refs; no structural deduplication is permitted.
    Invoke-Test "structurally identical occurrences remain separate components" {
        $source = Read-OpenApiDocument -Path $sourceFixturePath
        $result = ConvertTo-ComponentizedOpenApiDocument -Document $source
        $schemas = $result.Document.components.schemas
        $first = $schemas.CreateWidget_Request_Variants_TypeOfAlpha
        $second = $schemas.CreateWidget_Request_Variants_TypeOfAlpha_2
        Assert-True (Test-JsonValuesEqual -Left $first -Right $second) "Duplicate fixture branches were not structurally equal."
        $variantRefs = $schemas.CreateWidget_Request_Variants.oneOf
        Assert-True (-not [string]::Equals($variantRefs[0].'$ref', $variantRefs[1].'$ref', [System.StringComparison]::Ordinal)) "Duplicate occurrences shared one component."
    }

    # Parent components reference nested components, while each generated root
    # remains the complete object definition that originally occupied its pointer.
    Invoke-Test "nested objects become component reference chains" {
        $source = Read-OpenApiDocument -Path $sourceFixturePath
        $result = ConvertTo-ComponentizedOpenApiDocument -Document $source
        $schemas = $result.Document.components.schemas
        Assert-Equal "#/components/schemas/CreateWidget_Request" $result.Document.paths["/widgets"].post.requestBody.content["application/json"].schema.'$ref' "Root schema was not replaced."
        Assert-Equal "object" $schemas.CreateWidget_Request.type "Root component lost its object type."
        Assert-Equal "#/components/schemas/CreateWidget_Request_Child_TypeOfChildKind" $schemas.CreateWidget_Request.properties.child.'$ref' "Child component ref was incorrect."
        Assert-Equal "#/components/schemas/CreateWidget_Request_Child" $schemas.CreateWidget_Request_Child_TypeOfChildKind.additionalProperties.'$ref' "Additional-properties ref was incorrect."
        Assert-Equal "object" $schemas.CreateWidget_Request_Child.type "Nested component lost its root definition."
        Assert-Equal 0 @(Get-InlineOpenApiObjectSchemaCandidates -Document $result.Document).Count "Componentized output retained inline object candidates."
    }

    # Existing source pointers and discriminator mappings follow their moved
    # definitions using longest-prefix relocation. The assertions cover both a
    # ref into a promoted child and mappings to repeated composition branches.
    Invoke-Test "references and discriminator mappings relocate to generated components" {
        $source = Read-OpenApiDocument -Path $sourceFixturePath
        $result = ConvertTo-ComponentizedOpenApiDocument -Document $source
        $schemas = $result.Document.components.schemas
        Assert-Equal "#/components/schemas/CreateWidget_Request_Child_TypeOfChildKind" $schemas.CreateWidget_Request.properties.childAlias.'$ref' "Existing schema ref was not relocated."
        Assert-Equal "#/components/schemas/CreateWidget_Request_Variants" $schemas.CreateWidget_Request.properties.variants.'$ref' "oneOf owner was not relocated."
        Assert-Equal "#/components/schemas/CreateWidget_Request_Variants_TypeOfAlpha" $schemas.CreateWidget_Request_Variants.discriminator.mapping.first "First mapping was not relocated."
        Assert-Equal "#/components/schemas/CreateWidget_Request_Variants_TypeOfAlpha_2" $schemas.CreateWidget_Request_Variants.discriminator.mapping.second "Second mapping was not relocated."
        Assert-OpenApiReferencesResolve -Document $result.Document
    }

    # Components unrelated to schemas are preserved, and existing schema roots
    # stay roots while only their nested inline objects are promoted.
    Invoke-Test "existing component roots and security schemes are preserved" {
        $source = Read-OpenApiDocument -Path $sourceFixturePath
        $result = ConvertTo-ComponentizedOpenApiDocument -Document $source
        Assert-Equal "object" $result.Document.components.schemas.ExistingRoot.type "Existing schema root changed."
        Assert-Equal "#/components/schemas/ExistingRoot_Nested" $result.Document.components.schemas.ExistingRoot.properties.nested.'$ref' "Existing component nested object was not promoted."
        Assert-Equal "apiKey" $result.Document.components.securitySchemes.ApiKey.type "Security scheme type changed."
        Assert-Equal "x-api-key" $result.Document.components.securitySchemes.ApiKey.name "Security scheme name changed."
    }

    # Type arrays and example objects are not exact schema-context object
    # candidates and must survive unchanged.
    Invoke-Test "near misses and literal object lookalikes remain inline" {
        $source = Read-TestJsonFixture `
            -RelativePath "Componentize-OpenApiObjectSchemas/source.openapi.json"
        $source.paths["/widgets"].post.requestBody.content["application/json"].schema.properties.literal.example["oneOf"] = @(
            [ordered]@{ type = "object" }
        )
        $result = ConvertTo-ComponentizedOpenApiDocument -Document $source
        $root = $result.Document.components.schemas.CreateWidget_Request
        Assert-Sequence @("object", "null") @($root.properties.nearMiss.type) "Type-array near miss changed."
        Assert-Equal "object" $root.properties.literal.example.type "Literal object changed."
        Assert-Equal "object" $root.properties.literal.example.properties.ignored.type "Nested literal object changed."
        Assert-Equal 1 $root.properties.literal.example.oneOf.Count "Literal oneOf lookalike changed."
    }

    # Moving a schema that establishes JSON Schema URI or anchor scope can change
    # reference meaning. Every such keyword is rejected with its source pointer.
    Invoke-Test "scope-sensitive schema keywords fail explicitly" {
        foreach ($case in @(
            [PSCustomObject]@{ Keyword = '$id'; Value = "urn:test:root" },
            [PSCustomObject]@{ Keyword = '$anchor'; Value = "root" },
            [PSCustomObject]@{ Keyword = '$dynamicAnchor'; Value = "node" },
            [PSCustomObject]@{ Keyword = '$recursiveAnchor'; Value = $true }
        )) {
            $source = Read-TestJsonFixture `
                -RelativePath "Componentize-OpenApiObjectSchemas/source.openapi.json"
            $schema = $source.paths["/widgets"].post.requestBody.content["application/json"].schema
            $schema[$case.Keyword] = $case.Value
            Assert-Throws {
                ConvertTo-ComponentizedOpenApiDocument -Document $source
            } "*scope-sensitive keyword '$($case.Keyword)'*schema*$($case.Keyword)*"
        }

        $inherited = Read-TestJsonFixture `
            -RelativePath "Componentize-OpenApiObjectSchemas/source.openapi.json"
        $inherited.components.schemas.ExistingRoot['$id'] = "urn:test:existing"
        Assert-Throws {
            ConvertTo-ComponentizedOpenApiDocument -Document $inherited
        } "*inherits scope from `$id*ExistingRoot*`$id*"
    }

    # Readable names are intentionally not truncated, and a unique long name
    # does not acquire a numeric suffix.
    Invoke-Test "long readable component names are not truncated" {
        $source = Read-TestJsonFixture `
            -RelativePath "Componentize-OpenApiObjectSchemas/source.openapi.json"
        $longPropertyName = "segment_" + ("x" * 140)
        $source.paths["/widgets"].post.requestBody.content["application/json"].schema.properties[$longPropertyName] = [ordered]@{
            type = "object"
        }
        $result = ConvertTo-ComponentizedOpenApiDocument -Document $source
        $longCandidate = @($result.Candidates | Where-Object { $_.Pointer.EndsWith("/$longPropertyName", [System.StringComparison]::Ordinal) })[0]
        Assert-True ($longCandidate.Name.Length -gt 100) "Long component name was truncated."
        Assert-True (-not ($longCandidate.Name -match "_[0-9]+$")) "Unique long component received a numeric suffix."
        Assert-True ($longCandidate.Name.StartsWith("CreateWidget_Request_Segment", [System.StringComparison]::Ordinal)) "Long component sections were incorrect."
    }

    # Existing schema names participate in collision checks, causing only the
    # conflicting generated base name to acquire a suffix.
    Invoke-Test "existing component names are never overwritten" {
        $source = Read-TestJsonFixture `
            -RelativePath "Componentize-OpenApiObjectSchemas/source.openapi.json"
        $source.components.schemas["CreateWidget_Request"] = [ordered]@{ type = "string" }
        $result = ConvertTo-ComponentizedOpenApiDocument -Document $source
        Assert-Equal "string" $result.Document.components.schemas.CreateWidget_Request.type "Existing colliding component was overwritten."
        Assert-Equal "#/components/schemas/CreateWidget_Request_2" $result.Document.paths["/widgets"].post.requestBody.content["application/json"].schema.'$ref' "Generated name did not suffix the collision."
    }

    # Once every inline candidate has moved, a second conversion performs no
    # work and preserves the complete document structurally.
    Invoke-Test "componentization is structurally idempotent" {
        $source = Read-OpenApiDocument -Path $sourceFixturePath
        $first = ConvertTo-ComponentizedOpenApiDocument -Document $source
        $second = ConvertTo-ComponentizedOpenApiDocument -Document $first.Document
        Assert-Equal 0 $second.DiscoveredSchemaCount "Second pass rediscovered candidates."
        Assert-Equal 0 $second.DiscoveredObjectSchemaCount "Second pass rediscovered candidates."
        Assert-Equal 0 $second.DiscoveredOneOfSchemaCount "Second pass rediscovered oneOf candidates."
        Assert-Equal 0 $second.GeneratedComponentCount "Second pass generated components."
        Assert-JsonEqual $first.Document $second.Document "Second pass changed the componentized document."
    }

    # Default naming supports arbitrary sibling inputs and recognizes already
    # componentized paths without repeating the suffix.
    Invoke-Test "default output paths insert the componentized marker once" {
        $ordinary = Join-Path $temporaryDirectory "sample.openapi.json"
        $expected = Join-Path $temporaryDirectory "sample.componentized.openapi.json"
        Assert-Equal ([System.IO.Path]::GetFullPath($expected)) (Get-DefaultComponentizedOpenApiPath -InputPath $ordinary) "OpenAPI default output path was incorrect."
        Assert-Equal ([System.IO.Path]::GetFullPath($expected)) (Get-DefaultComponentizedOpenApiPath -InputPath $expected) "Componentized suffix repeated."
        Assert-Equal ([System.IO.Path]::GetFullPath((Join-Path $temporaryDirectory "sample.componentized.json"))) (Get-DefaultComponentizedOpenApiPath -InputPath (Join-Path $temporaryDirectory "sample.json")) "Plain JSON output path was incorrect."
    }

    # Public file handling preserves the source and emits deterministic UTF-8
    # bytes for default and explicit destinations. Byte checks are intentional:
    # these artifacts are committed, so BOM or newline drift creates noisy diffs
    # even when the parsed JSON is equivalent.
    Invoke-Test "CLI default and explicit outputs are deterministic and preserve input" {
        $inputPath = Join-Path $temporaryDirectory "sample.openapi.json"
        Copy-Item -LiteralPath $sourceFixturePath -Destination $inputPath
        [byte[]]$inputBefore = [System.IO.File]::ReadAllBytes($inputPath)
        $null = Invoke-OpenApiObjectSchemaComponentizer -InputPath $inputPath
        $defaultOutput = Join-Path $temporaryDirectory "sample.componentized.openapi.json"
        Assert-True ([System.IO.File]::Exists($defaultOutput)) "Default output was not created."
        Assert-BytesEqual $inputBefore ([System.IO.File]::ReadAllBytes($inputPath)) "Componentizer modified its input."
        [byte[]]$defaultBytes = [System.IO.File]::ReadAllBytes($defaultOutput)
        $hasBom = $defaultBytes.Length -ge 3 -and $defaultBytes[0] -eq 0xEF -and $defaultBytes[1] -eq 0xBB -and $defaultBytes[2] -eq 0xBF
        Assert-True (-not $hasBom) "Output contains a UTF-8 BOM."
        Assert-Equal 10 $defaultBytes[$defaultBytes.Length - 1] "Output lacks a trailing LF."

        $explicitOutput = Join-Path $temporaryDirectory "explicit.json"
        $null = Invoke-OpenApiObjectSchemaComponentizer `
            -InputPath $inputPath `
            -OutputPath $explicitOutput
        Assert-BytesEqual $defaultBytes ([System.IO.File]::ReadAllBytes($explicitOutput)) "Explicit output differed from default output."
    }

    # Reprocessing a componentized file in place is a byte no-op and reports no
    # newly generated components.
    Invoke-Test "CLI componentization is byte-idempotent in place" {
        $inputPath = Join-Path $temporaryDirectory "already.componentized.openapi.json"
        Copy-Item -LiteralPath $expectedFixturePath -Destination $inputPath
        $null = Invoke-OpenApiObjectSchemaComponentizer -InputPath $inputPath
        [byte[]]$firstRun = [System.IO.File]::ReadAllBytes($inputPath)
        $second = Invoke-OpenApiObjectSchemaComponentizer -InputPath $inputPath
        Assert-Equal 0 $second.GeneratedComponentCount "In-place second pass generated components."
        Assert-BytesEqual $firstRun ([System.IO.File]::ReadAllBytes($inputPath)) "In-place componentization changed bytes."
    }

    # Input validation belongs at the CLI boundary and must fail before writing
    # a partial componentized artifact.
    Invoke-Test "missing directory and invalid JSON inputs fail clearly" {
        Assert-Throws {
            Invoke-OpenApiObjectSchemaComponentizer -InputPath (Join-Path $temporaryDirectory "missing.json")
        } "*cannot find path*"
        Assert-Throws {
            Invoke-OpenApiObjectSchemaComponentizer -InputPath $temporaryDirectory
        } "*is not a file*"

        $invalidPath = Join-Path $temporaryDirectory "invalid.json"
        [System.IO.File]::WriteAllText($invalidPath, "{ invalid", [System.Text.UTF8Encoding]::new($false))
        Assert-Throws {
            Invoke-OpenApiObjectSchemaComponentizer -InputPath $invalidPath
        } "*Could not parse OpenAPI JSON*"

        $unsafe = Read-TestJsonFixture `
            -RelativePath "Componentize-OpenApiObjectSchemas/source.openapi.json"
        $unsafe.paths["/widgets"].post.requestBody.content["application/json"].schema['$anchor'] = "root"
        $unsafePath = Join-Path $temporaryDirectory "unsafe.json"
        $unsafeOutputPath = Join-Path $temporaryDirectory "unsafe-output.json"
        $null = Write-OpenApiJsonFile -Document $unsafe -Path $unsafePath
        [byte[]]$sentinel = [System.Text.Encoding]::UTF8.GetBytes("existing output`n")
        [System.IO.File]::WriteAllBytes($unsafeOutputPath, $sentinel)
        Assert-Throws {
            Invoke-OpenApiObjectSchemaComponentizer `
                -InputPath $unsafePath `
                -OutputPath $unsafeOutputPath
        } "*scope-sensitive keyword*`$anchor*"
        Assert-BytesEqual $sentinel ([System.IO.File]::ReadAllBytes($unsafeOutputPath)) "Failed componentization changed output bytes."
    }
}
finally {
    # Each suite run owns a GUID-named directory, making cleanup safe even when
    # multiple local or CI invocations execute concurrently.
    if ([System.IO.Directory]::Exists($temporaryDirectory)) {
        [System.IO.Directory]::Delete($temporaryDirectory, $true)
    }
}

Write-Host ""
Write-Host "Passed: $script:PassedTests"
Write-Host "Failed: $script:FailedTests"
if ($script:FailedTests -gt 0) {
    Write-Host ""
    foreach ($failure in $script:Failures) {
        Write-Host $failure -ForegroundColor Red
    }
    exit 1
}
