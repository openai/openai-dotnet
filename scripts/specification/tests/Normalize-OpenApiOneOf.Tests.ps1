<#
.SYNOPSIS
    Runs focused tests for scripts/specification/pipeline/Normalize-OpenApiOneOf.ps1.
#>

[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$scriptDirectory = Join-Path $PSScriptRoot ".." "pipeline"
$normalizeScriptPath = Join-Path $scriptDirectory "Normalize-OpenApiOneOf.ps1"
$sortScriptPath = Join-Path $scriptDirectory "Sort-OpenApiDocument.ps1"
. $normalizeScriptPath
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
    $expectedJson = ConvertTo-Json -InputObject $Expected -Compress -Depth 100
    $actualJson = ConvertTo-Json -InputObject $Actual -Compress -Depth 100
    if (-not [string]::Equals($expectedJson, $actualJson, [System.StringComparison]::Ordinal)) {
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

function New-PromotableAnyOfSchema {
    return Read-TestJsonFixture `
        -RelativePath "Normalize-OpenApiOneOf/promotable-anyof.schema.json"
}

$fixtureDirectory = Join-Path $PSScriptRoot "fixtures" "Normalize-OpenApiOneOf"
$schemaCases = Read-TestJsonFixture -RelativePath "Normalize-OpenApiOneOf/cases.schema.json"
$temporaryDirectory = Join-Path ([System.IO.Path]::GetTempPath()) "normalize-openapi-oneof-$([Guid]::NewGuid().ToString('N'))"
$null = [System.IO.Directory]::CreateDirectory($temporaryDirectory)

try {
    # The stage is consumed as both a command and a function library; loading
    # it for focused tests must not generate files.
    Invoke-Test "dot-sourcing defines normalizer functions without running the CLI" {
        Assert-True ($null -ne (Get-Command ConvertTo-NormalizedOpenApiOneOfDocument -ErrorAction SilentlyContinue)) "Conversion function was not defined."
        Assert-True ($null -ne (Get-Command Invoke-OpenApiOneOfNormalizer -ErrorAction SilentlyContinue)) "CLI function was not defined."
    }

    # This fixture mirrors the production defect: an array-level discriminator
    # is orphaned while the nested items discriminator is paired with oneOf.
    Invoke-Test "line 7132 shape removes only the outer orphan" {
        $source = Read-OpenApiDocument -Path (Join-Path $fixtureDirectory "source.openapi.json")
        $expected = Read-OpenApiDocument -Path (Join-Path $fixtureDirectory "expected.oneof-normalized.openapi.json")
        $sourceBefore = ConvertTo-Json -InputObject $source -Compress -Depth 100
        $result = ConvertTo-NormalizedOpenApiOneOfDocument -Document $source
        Assert-Equal 1 $result.RemovedDiscriminatorCount "Unexpected removal count."
        Assert-Equal 1 $result.PromotedAnyOfCount "Unexpected promotion count."
        Assert-Equal 1 $result.RelocationCount "Unexpected relocation count."
        Assert-JsonEqual $expected $result.Document "Normalized fixture did not match its golden."
        Assert-Equal $sourceBefore (ConvertTo-Json -InputObject $source -Compress -Depth 100) "Conversion mutated its input."

        $outputs = (Get-TestResponseSchema -Document $result.Document).properties.outputs
        Assert-True (-not (Test-JsonProperty -Object $outputs -Name "discriminator")) "Outer orphan remained."
        Assert-True (Test-JsonProperty -Object $outputs.items -Name "discriminator") "Nested paired discriminator was removed."
        $promotable = (Get-TestResponseSchema -Document $result.Document).properties.promotable
        Assert-Sequence @("description", "oneOf", "discriminator") @((Get-JsonProperties $promotable).Name) "Promotion changed member order."
    }

    # Promotion requires finite, required, mutually exclusive discriminator
    # values so the eventual explicit mapping is deterministic.
    Invoke-Test "promotion requires finite required exclusive values" {
        $source = New-TestOpenApiDocument -Schema (New-PromotableAnyOfSchema)
        $result = ConvertTo-NormalizedOpenApiOneOfDocument -Document $source
        $schema = Get-TestResponseSchema $result.Document
        Assert-Equal 1 $result.PromotedAnyOfCount "Finite discriminator candidate was not promoted."
        Assert-True (-not (Test-JsonProperty $schema "anyOf")) "Promoted schema retained anyOf."
        Assert-True (Test-JsonProperty $schema "oneOf") "Promoted schema did not gain oneOf."
        Assert-True (Test-JsonProperty $schema "discriminator") "Promoted schema lost its discriminator."
    }

    Invoke-Test "promotion accepts compatible nested discriminated oneOf branches" {
        $leaf = {
            param([string]$Value)
            [ordered]@{
                type = "object"
                required = @("kind")
                properties = [ordered]@{
                    kind = [ordered]@{ type = "string"; const = $Value }
                }
            }
        }
        $nested = [ordered]@{
            discriminator = [ordered]@{ propertyName = "kind" }
            oneOf = @((& $leaf "nested_a"), (& $leaf "nested_b"))
        }
        $schema = [ordered]@{
            discriminator = [ordered]@{ propertyName = "kind" }
            anyOf = @($nested, (& $leaf "outer"))
        }

        $result = ConvertTo-NormalizedOpenApiOneOfDocument `
            -Document (New-TestOpenApiDocument -Schema $schema)
        $normalized = Get-TestResponseSchema $result.Document
        Assert-Equal 1 $result.PromotedAnyOfCount "Nested discriminator candidate was not promoted."
        Assert-True (-not (Test-JsonProperty $normalized "anyOf")) "Nested candidate retained anyOf."
        Assert-Equal 2 $normalized.oneOf.Count "Nested candidate changed branch count before flattening."

        $invalid = Copy-JsonValue -Value $schema
        $invalid.anyOf[0]["type"] = "string"
        $invalidResult = ConvertTo-NormalizedOpenApiOneOfDocument `
            -Document (New-TestOpenApiDocument -Schema $invalid)
        $invalidNormalized = Get-TestResponseSchema $invalidResult.Document
        Assert-Equal 0 $invalidResult.PromotedAnyOfCount "Explicit non-object wrapper was promoted."
        Assert-True (Test-JsonProperty $invalidNormalized "anyOf") "Explicit non-object wrapper lost anyOf."
        Assert-True (-not (Test-JsonProperty $invalidNormalized "discriminator")) "Invalid anyOf discriminator remained."
    }

    Invoke-Test "promotion preserves distinct discriminator levels and normalizes the outer discriminator property" {
        $cases = Read-TestJsonFixture -RelativePath "Add-OpenApiOneOfDiscriminators/cases.schema.json"
        $schema = Get-TestFixtureCase -Cases $cases -Name "nestedHierarchy"
        $schema["anyOf"] = $schema.oneOf
        $schema.Remove("oneOf")
        $source = New-TestOpenApiDocument -Schema $schema
        $before = ConvertTo-Json -InputObject $source -Compress -Depth 100
        $result = ConvertTo-NormalizedOpenApiOneOfDocument -Document $source
        $normalized = Get-TestResponseSchema -Document $result.Document
        Assert-Equal 1 $result.PromotedAnyOfCount "Hierarchical anyOf was not promoted."
        Assert-Equal "type" $normalized.discriminator.propertyName "Outer discriminator was lost."
        Assert-Equal "role" $normalized.oneOf[0].discriminator.propertyName "Inner discriminator was lost."
        Assert-Sequence @("message") @($normalized.oneOf[0].properties.type.enum) "Common outer discriminator value was not materialized."
        Assert-Sequence @("type") @($normalized.oneOf[0].required) "Intermediate base did not require the outer discriminator property."
        Assert-JsonEqual $schema.anyOf[0].oneOf $normalized.oneOf[0].oneOf "Role-specific leaves changed."
        Assert-Equal $before (ConvertTo-Json -InputObject $source -Compress -Depth 100) "Promotion mutated its source."
        $second = ConvertTo-NormalizedOpenApiOneOfDocument -Document $result.Document
        Assert-JsonEqual $result.Document $second.Document "Second pass changed the hierarchy."
    }

    # Every eligibility gate is conservative: malformed or mismatched unions
    # stay anyOf, after which their unpaired discriminator follows cleanup rules.
    Invoke-Test "near misses remain anyOf and follow orphan cleanup" {
        $cases = @(
            [PSCustomObject]@{ Name = "missing discriminator"; Mutate = { param($schema) $schema.Remove("discriminator") } },
            [PSCustomObject]@{ Name = "scalar discriminator"; Mutate = { param($schema) $schema.discriminator = "kind" } },
            [PSCustomObject]@{ Name = "missing propertyName"; Mutate = { param($schema) $schema.discriminator = [ordered]@{} } },
            [PSCustomObject]@{ Name = "non-string propertyName"; Mutate = { param($schema) $schema.discriminator.propertyName = 42 } },
            [PSCustomObject]@{ Name = "empty propertyName"; Mutate = { param($schema) $schema.discriminator.propertyName = "" } },
            [PSCustomObject]@{ Name = "empty anyOf"; Mutate = { param($schema) $schema.anyOf = @() } },
            [PSCustomObject]@{ Name = "non-array anyOf"; Mutate = { param($schema) $schema.anyOf = "not an array" } },
            [PSCustomObject]@{ Name = "scalar branch"; Mutate = { param($schema) $schema.anyOf = @($schema.anyOf[0], "not an object") } },
            [PSCustomObject]@{ Name = "non-object branch"; Mutate = { param($schema) $schema.anyOf[1].type = "string" } },
            [PSCustomObject]@{ Name = "missing properties"; Mutate = { param($schema) $schema.anyOf[1].Remove("properties") } },
            [PSCustomObject]@{ Name = "non-object properties"; Mutate = { param($schema) $schema.anyOf[1].properties = "not an object" } },
            [PSCustomObject]@{ Name = "missing matching property"; Mutate = { param($schema) $schema.anyOf[1].properties = [ordered]@{ other = [ordered]@{} } } },
            [PSCustomObject]@{ Name = "case mismatch"; Mutate = { param($schema) $schema.anyOf[1].properties = [ordered]@{ Kind = [ordered]@{} } } },
            [PSCustomObject]@{ Name = "optional property"; Mutate = { param($schema) $schema.anyOf[1].Remove("required") } },
            [PSCustomObject]@{ Name = "unconstrained property"; Mutate = { param($schema) $schema.anyOf[1].properties.kind = [ordered]@{ type = "string" } } },
            [PSCustomObject]@{ Name = "duplicate value"; Mutate = { param($schema) $schema.anyOf[1].properties.kind = [ordered]@{ type = "string"; const = "first" } } }
        )
        foreach ($case in $cases) {
            $schema = New-PromotableAnyOfSchema
            & $case.Mutate $schema
            $result = ConvertTo-NormalizedOpenApiOneOfDocument -Document (New-TestOpenApiDocument $schema)
            $normalized = Get-TestResponseSchema $result.Document
            Assert-Equal 0 $result.PromotedAnyOfCount "Near miss '$($case.Name)' was promoted."
            Assert-True (Test-JsonProperty $normalized "anyOf") "Near miss '$($case.Name)' lost anyOf."
        }
    }

    # Overwriting or merging an existing oneOf would be ambiguous, so a schema
    # satisfying both shapes must fail without mutating its source.
    Invoke-Test "eligible anyOf with an existing oneOf fails clearly" {
        $schema = New-PromotableAnyOfSchema
        $schema["oneOf"] = @([ordered]@{ type = "object" })
        $source = New-TestOpenApiDocument $schema
        $before = ConvertTo-Json $source -Compress -Depth 100
        Assert-Throws {
            ConvertTo-NormalizedOpenApiOneOfDocument -Document $source
        } "*#/paths/~1test/post/responses/200/content/application~1json/schema*already contains oneOf*"
        Assert-Equal $before (ConvertTo-Json $source -Compress -Depth 100) "Collision handling mutated input."
    }

    # Orphan detection is based on direct sibling presence only; malformed,
    # empty, or null oneOf values are preserved for separate validation.
    Invoke-Test "any exact direct oneOf sibling preserves discriminator" {
        $source = New-TestOpenApiDocument -Schema (
            Get-TestFixtureCase -Cases $schemaCases -Name "directPairs"
        )
        $result = ConvertTo-NormalizedOpenApiOneOfDocument -Document $source
        Assert-Equal 0 $result.RemovedDiscriminatorCount "A direct pair was removed."
        foreach ($property in (Get-JsonProperties -Value (Get-TestResponseSchema $result.Document).properties)) {
            Assert-True (Test-JsonProperty -Object $property.Value -Name "discriminator") "Paired discriminator '$($property.Name)' was removed."
        }
    }

    # `properties.discriminator` names user data and must survive, while an
    # orphaned discriminator keyword inside that property's schema is removed.
    Invoke-Test "property-map key is preserved while its schema is traversed" {
        $source = New-TestOpenApiDocument -Schema (
            Get-TestFixtureCase -Cases $schemaCases -Name "propertyMap"
        )
        $result = ConvertTo-NormalizedOpenApiOneOfDocument -Document $source
        $properties = (Get-TestResponseSchema $result.Document).properties
        Assert-Equal 1 $result.RemovedDiscriminatorCount "Nested schema keyword was not removed."
        Assert-True (Test-JsonProperty -Object $properties -Name "discriminator") "Property-map key was removed."
        Assert-True (-not (Test-JsonProperty -Object $properties.discriminator -Name "discriminator")) "Orphan inside the property schema remained."
    }

    # Schema context and exact casing prevent global text/property replacement
    # from corrupting examples, descriptions, enums, or ordinary objects.
    Invoke-Test "literal values ordinary objects and case variants remain unchanged" {
        $source = New-TestOpenApiDocument -Schema (
            Get-TestFixtureCase -Cases $schemaCases -Name "protectedLiterals"
        )
        $source.info["discriminator"] = "ordinary OpenAPI member"
        $before = ConvertTo-Json -InputObject $source -Compress -Depth 100
        $result = ConvertTo-NormalizedOpenApiOneOfDocument -Document $source
        Assert-Equal 0 $result.RemovedDiscriminatorCount "A protected occurrence was removed."
        Assert-Equal $before (ConvertTo-Json -InputObject $result.Document -Compress -Depth 100) "A protected occurrence changed."
    }

    # Orphans can occur under properties, composition branches, and items; each
    # schema is evaluated independently without reordering surviving members.
    Invoke-Test "nested schema orphans are removed independently in encounter order" {
        $source = New-TestOpenApiDocument -Schema (
            Get-TestFixtureCase -Cases $schemaCases -Name "nestedOrphans"
        )
        $result = ConvertTo-NormalizedOpenApiOneOfDocument -Document $source
        $properties = (Get-TestResponseSchema $result.Document).properties
        Assert-Equal 3 $result.RemovedDiscriminatorCount "Nested orphan count was incorrect."
        Assert-Sequence @("type", "description") @((Get-JsonProperties $properties.first).Name) "First schema order changed."
        Assert-True (-not (Test-JsonProperty $properties.second.anyOf[0] "discriminator")) "Composition orphan remained."
        Assert-True (-not (Test-JsonProperty $properties.third.items "discriminator")) "Items orphan remained."
    }

    # The orphan postcondition catches traversal regressions that would let a
    # removable discriminator escape normalization.
    Invoke-Test "postcondition rejects an orphaned schema discriminator" {
        $source = New-TestOpenApiDocument -Schema (
            Get-TestFixtureCase -Cases $schemaCases -Name "orphan"
        )
        Assert-Throws {
            Assert-NoOrphanedOpenApiDiscriminators -Value $source
        } "*retained an orphaned discriminator*"
    }

    # The promotion postcondition proves that no schema satisfying the declared
    # eligibility rules remains as anyOf after normalization.
    Invoke-Test "promotion postcondition rejects an eligible anyOf" {
        $source = New-TestOpenApiDocument -Schema (New-PromotableAnyOfSchema)
        Assert-Throws {
            Assert-NoPromotableOpenApiAnyOf -Value $source
        } "*retained a promotable discriminated anyOf*"
    }

    # A preserved discriminator subtree keeps its location, so pointers into a
    # valid pair must continue to resolve unchanged.
    Invoke-Test "references to paired discriminators remain valid" {
        $targetPointer = "#/paths/~1test/post/responses/200/content/application~1json/schema/properties/target/discriminator"
        $source = New-TestOpenApiDocument -Schema (
            Get-TestFixtureCase -Cases $schemaCases -Name "pairedReference"
        )
        $result = ConvertTo-NormalizedOpenApiOneOfDocument -Document $source
        Assert-Equal 0 $result.RemovedDiscriminatorCount "Paired target was removed."
        Assert-Equal $targetPointer (Get-TestResponseSchema $result.Document).properties.alias.'$ref' "Retained reference changed."
    }

    # Renaming anyOf moves every descendant pointer; all reference-valued
    # keywords and discriminator mappings must follow `/anyOf` to `/oneOf`.
    Invoke-Test "promotion relocates references and discriminator mappings" {
        $targetBase = "#/paths/~1test/post/responses/200/content/application~1json/schema/properties/target"
        $candidate = New-PromotableAnyOfSchema
        $candidate.discriminator["mapping"] = [ordered]@{ first = "$targetBase/anyOf/0" }
        $schema = Get-TestFixtureCase -Cases $schemaCases -Name "relocation"
        $schema.properties.target = $candidate
        $source = New-TestOpenApiDocument -Schema $schema
        $result = ConvertTo-NormalizedOpenApiOneOfDocument -Document $source
        $properties = (Get-TestResponseSchema $result.Document).properties
        $expected = "$targetBase/oneOf/0"
        Assert-Equal 1 $result.PromotedAnyOfCount "Pointer target was not promoted."
        Assert-Equal 1 $result.RelocationCount "Unexpected relocation count."
        Assert-Equal $expected $properties.target.discriminator.mapping.first "Mapping pointer was not relocated."
        Assert-Equal $expected $properties.refAlias.'$ref' "`$ref was not relocated."
        Assert-Equal $expected $properties.dynamicAlias.'$dynamicRef' "`$dynamicRef was not relocated."
        Assert-Equal $expected $properties.recursiveAlias.'$recursiveRef' "`$recursiveRef was not relocated."
    }

    # Nested key moves require chained longest-prefix relocation so a pointer is
    # rewritten through both outer and inner promotions.
    Invoke-Test "nested promotions relocate pointers through longest prefixes" {
        $targetBase = "#/paths/~1test/post/responses/200/content/application~1json/schema/properties/outer"
        $outer = New-PromotableAnyOfSchema
        $outer.anyOf[0].properties["inner"] = New-PromotableAnyOfSchema
        $schema = Get-TestFixtureCase -Cases $schemaCases -Name "nestedRelocation"
        $schema.properties.outer = $outer
        $source = New-TestOpenApiDocument -Schema $schema
        $result = ConvertTo-NormalizedOpenApiOneOfDocument -Document $source
        $properties = (Get-TestResponseSchema $result.Document).properties
        $expected = "$targetBase/oneOf/0/properties/inner/oneOf/0"
        Assert-Equal 2 $result.PromotedAnyOfCount "Nested candidates were not both promoted."
        Assert-Equal 2 $result.RelocationCount "Nested relocation count was incorrect."
        Assert-Equal $expected $properties.alias.'$ref' "Nested pointer was not fully relocated."
        Assert-True (Test-JsonProperty $properties.outer.oneOf[0].properties.inner "oneOf") "Nested candidate retained anyOf."
    }

    # Pointer rewriting must preserve RFC 6901 escaping when promoted schemas
    # sit under property names containing slash or tilde.
    Invoke-Test "promotion relocation preserves escaped JSON Pointer tokens" {
        $propertyName = "a/b~c"
        $encodedPropertyName = "a~1b~0c"
        $targetBase = "#/paths/~1test/post/responses/200/content/application~1json/schema/properties/$encodedPropertyName"
        $schema = Get-TestFixtureCase -Cases $schemaCases -Name "escapedRelocation"
        $schema.properties[$propertyName] = New-PromotableAnyOfSchema
        $source = New-TestOpenApiDocument -Schema $schema
        $result = ConvertTo-NormalizedOpenApiOneOfDocument -Document $source
        $properties = (Get-TestResponseSchema $result.Document).properties
        Assert-Equal "$targetBase/oneOf/0" $properties.alias.'$ref' "Escaped pointer was not relocated."
        Assert-True (Test-JsonProperty $properties[$propertyName] "oneOf") "Escaped-name candidate was not promoted."
    }

    # Removing an orphan has no valid relocation target; retained references
    # into that subtree must fail instead of becoming silently dangling.
    Invoke-Test "references into removed discriminator subtrees fail clearly" {
        $targetPointer = "#/paths/~1test/post/responses/200/content/application~1json/schema/properties/target/discriminator"
        foreach ($referenceName in @('$ref', '$dynamicRef', '$recursiveRef')) {
            $alias = [ordered]@{}
            $alias[$referenceName] = $targetPointer
            $schema = Get-TestFixtureCase -Cases $schemaCases -Name "removedTarget"
            $schema.properties.alias = $alias
            $source = New-TestOpenApiDocument -Schema $schema
            Assert-Throws {
                ConvertTo-NormalizedOpenApiOneOfDocument -Document $source
            } "*has no 'discriminator' property*"
        }
    }

    # Normalize is semantic and order-preserving; only the downstream Sort stage
    # may canonicalize property order.
    Invoke-Test "normalization preserves order and Sort canonicalizes afterward" {
        $source = Read-OpenApiDocument -Path (Join-Path $fixtureDirectory "source.openapi.json")
        $normalized = (ConvertTo-NormalizedOpenApiOneOfDocument -Document $source).Document
        $normalizedProperties = (Get-TestResponseSchema $normalized).properties
        Assert-Sequence `
            @("outputs", "promotable", "discriminator", "emptyPair", "malformedPair", "caseVariant") `
            @((Get-JsonProperties $normalizedProperties).Name) `
            "Normalizer reordered properties."

        . $sortScriptPath
        $sorted = ConvertTo-SortedOpenApiDocument -Document $normalized
        Assert-Sequence `
            @("caseVariant", "discriminator", "emptyPair", "malformedPair", "outputs", "promotable") `
            @((Get-JsonProperties (Get-TestResponseSchema $sorted).properties).Name) `
            "Sort did not canonicalize normalized properties."
    }

    # Default and explicit destinations must emit identical deterministic bytes
    # without modifying the Simplify-stage input.
    Invoke-Test "default and explicit output paths encoding and input preservation are correct" {
        $inputPath = Join-Path $temporaryDirectory "sample.simplified.openapi.json"
        Copy-Item -LiteralPath (Join-Path $fixtureDirectory "source.openapi.json") -Destination $inputPath
        [byte[]]$inputBefore = [System.IO.File]::ReadAllBytes($inputPath)
        & $normalizeScriptPath -InputPath $inputPath
        $outputPath = Join-Path $temporaryDirectory "sample.simplified.oneof-normalized.openapi.json"
        Assert-True ([System.IO.File]::Exists($outputPath)) "Default normalized output was not created."
        Assert-BytesEqual $inputBefore ([System.IO.File]::ReadAllBytes($inputPath)) "Normalizer modified its input."
        [byte[]]$outputBytes = [System.IO.File]::ReadAllBytes($outputPath)
        $hasBom = $outputBytes.Length -ge 3 -and $outputBytes[0] -eq 0xEF -and $outputBytes[1] -eq 0xBB -and $outputBytes[2] -eq 0xBF
        Assert-True (-not $hasBom) "Normalized output contains a BOM."
        Assert-Equal 10 $outputBytes[$outputBytes.Length - 1] "Normalized output lacks a trailing LF."

        $explicitPath = Join-Path $temporaryDirectory "explicit-output.json"
        & $normalizeScriptPath -InputPath $inputPath -OutputPath $explicitPath
        Assert-True ([System.IO.File]::Exists($explicitPath)) "Explicit normalized output was not created."
        Assert-BytesEqual $outputBytes ([System.IO.File]::ReadAllBytes($explicitPath)) "Explicit output differed from default output."
    }

    # An already-normalized artifact should be safe to rerun in place with zero
    # transformations, stable bytes, and no repeated filename marker.
    Invoke-Test "normalization is byte-idempotent and avoids repeated suffixes" {
        $inputPath = Join-Path $temporaryDirectory "already.oneof-normalized.openapi.json"
        Copy-Item -LiteralPath (Join-Path $fixtureDirectory "expected.oneof-normalized.openapi.json") -Destination $inputPath
        Assert-Equal ([System.IO.Path]::GetFullPath($inputPath)) (Get-DefaultNormalizedOpenApiOneOfPath $inputPath) "Already-normalized default path changed."
        $null = Invoke-OpenApiOneOfNormalizer -InputPath $inputPath
        [byte[]]$firstRun = [System.IO.File]::ReadAllBytes($inputPath)
        $null = Invoke-OpenApiOneOfNormalizer -InputPath $inputPath
        Assert-BytesEqual $firstRun ([System.IO.File]::ReadAllBytes($inputPath)) "Normalization was not byte-idempotent."
    }

    # File validation belongs at the public CLI boundary and must reject invalid
    # inputs before normalization or output creation.
    Invoke-Test "missing directory and invalid JSON inputs fail clearly" {
        Assert-Throws {
            Invoke-OpenApiOneOfNormalizer -InputPath (Join-Path $temporaryDirectory "missing.json")
        } "*cannot find path*"
        Assert-Throws {
            Invoke-OpenApiOneOfNormalizer -InputPath $temporaryDirectory
        } "*is not a file*"

        $invalidPath = Join-Path $temporaryDirectory "invalid.json"
        [System.IO.File]::WriteAllText($invalidPath, "{ invalid", [System.Text.UTF8Encoding]::new($false))
        Assert-Throws {
            Invoke-OpenApiOneOfNormalizer -InputPath $invalidPath
        } "*Could not parse OpenAPI JSON*"
    }
}
finally {
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