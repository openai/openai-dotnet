<#
.SYNOPSIS
    Runs focused tests for scripts/specification/pipeline/Simplify-OpenApiRecursiveSchemas.ps1.
#>

[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$scriptDirectory = Join-Path $PSScriptRoot ".." "pipeline"
$simplifyScriptPath = Join-Path $scriptDirectory "Simplify-OpenApiRecursiveSchemas.ps1"
. $simplifyScriptPath
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

function Assert-FreeFormObjectSchema {
    param([object]$Schema, [string]$Message)
    # Replacement schemas must contain exactly these two members; preserving
    # annotations or constraints would violate the stage's minimal contract.
    Assert-Sequence @("type", "additionalProperties") @((Get-JsonProperties -Value $Schema).Name) "$Message Keys were incorrect."
    Assert-Equal "object" $Schema.type "$Message Type was incorrect."
    Assert-Equal $true $Schema.additionalProperties "$Message additionalProperties was incorrect."
}

$fixtureDirectory = Join-Path $PSScriptRoot "fixtures" "Simplify-OpenApiRecursiveSchemas"
$schemaCases = Read-TestJsonFixture -RelativePath "Simplify-OpenApiRecursiveSchemas/cases.schema.json"
$temporaryDirectory = Join-Path ([System.IO.Path]::GetTempPath()) "simplify-openapi-recursive-$([Guid]::NewGuid().ToString('N'))"
$null = [System.IO.Directory]::CreateDirectory($temporaryDirectory)

try {
    # Focused tests dot-source the stage as a function library. Loading it must
    # not run the command-line entry point or create output files.
    Invoke-Test "dot-sourcing defines recursive simplifier functions without running the CLI" {
        Assert-True ($null -ne (Get-Command ConvertTo-SimplifiedOpenApiRecursiveSchemaDocument -ErrorAction SilentlyContinue)) "Conversion function was not defined."
        Assert-True ($null -ne (Get-Command Invoke-OpenApiRecursiveSchemaSimplifier -ErrorAction SilentlyContinue)) "CLI function was not defined."
    }

    # An eligible anchor without a collapsible parent takes the branch-level
    # path and discards all of its original annotations and constraints.
    Invoke-Test "standalone anchored objects become exact free-form objects" {
        $source = New-TestOpenApiDocument -Schema (
            Get-TestFixtureCase -Cases $schemaCases -Name "standalone"
        )
        $sourceBefore = ConvertTo-Json -InputObject $source -Compress -Depth 100
        $result = ConvertTo-SimplifiedOpenApiRecursiveSchemaDocument -Document $source
        Assert-Equal 0 $result.CollapsedAnyOfCount "Unexpected anyOf collapse count."
        Assert-Equal 1 $result.ReplacedRecursiveSchemaCount "Unexpected individual replacement count."
        Assert-Equal 1 $result.RemovedRecursiveAnchorCount "Unexpected removed-anchor count."
        Assert-FreeFormObjectSchema (Get-TestResponseSchema -Document $result.Document) "Standalone replacement."
        Assert-Equal $sourceBefore (ConvertTo-Json -InputObject $source -Compress -Depth 100) "Conversion mutated its input."
    }

    # Parent-first precedence is the central rule: replacing the branch first
    # would leave a redundant object-only union in the output.
    Invoke-Test "all-object anyOf parents collapse before anchored branches" {
        $source = New-TestOpenApiDocument -Schema (
            Get-TestFixtureCase -Cases $schemaCases -Name "allObjectAnyOf"
        )
        $result = ConvertTo-SimplifiedOpenApiRecursiveSchemaDocument -Document $source
        Assert-Equal 1 $result.CollapsedAnyOfCount "The parent anyOf was not collapsed."
        Assert-Equal 0 $result.ReplacedRecursiveSchemaCount "A subsumed branch was counted individually."
        Assert-Equal 1 $result.RemovedRecursiveAnchorCount "Unexpected removed-anchor count."
        Assert-FreeFormObjectSchema (Get-TestResponseSchema -Document $result.Document) "Collapsed anyOf replacement."
    }

    # One parent replacement may remove several direct anchors, but it still
    # counts as one collapsed composition and zero individual replacements.
    Invoke-Test "parent collapse accounts for every subsumed direct anchor" {
        $source = New-TestOpenApiDocument -Schema (
            Get-TestFixtureCase -Cases $schemaCases -Name "multipleAnchors"
        )
        $result = ConvertTo-SimplifiedOpenApiRecursiveSchemaDocument -Document $source
        Assert-Equal 1 $result.CollapsedAnyOfCount "The multi-anchor anyOf was not collapsed once."
        Assert-Equal 0 $result.ReplacedRecursiveSchemaCount "Subsumed anchors were counted individually."
        Assert-Equal 2 $result.RemovedRecursiveAnchorCount "Not all subsumed anchors were counted."
    }

    # The production shape wraps an object-only union in a nullable union. Only
    # the inner candidate should collapse; nullability and outer annotations stay.
    Invoke-Test "nullable outer anyOf remains while its inner object union collapses" {
        $source = New-TestOpenApiDocument -Schema (
            Get-TestFixtureCase -Cases $schemaCases -Name "nullableWrapper"
        )
        $result = ConvertTo-SimplifiedOpenApiRecursiveSchemaDocument -Document $source
        $schema = Get-TestResponseSchema -Document $result.Document
        Assert-Equal 1 $result.CollapsedAnyOfCount "The inner anyOf was not collapsed."
        Assert-Equal "keep nullable wrapper" $schema.description "The outer annotation changed."
        Assert-Equal 2 $schema.anyOf.Count "The outer nullable anyOf changed shape."
        Assert-FreeFormObjectSchema $schema.anyOf[0] "Nullable object branch."
        Assert-Equal "null" $schema.anyOf[1].type "The nullable branch changed."
    }

    # Mixed anyOf and oneOf are outside the parent-collapse rule. Their wrappers
    # remain intact while the eligible anchored branch is simplified locally.
    Invoke-Test "mixed anyOf and oneOf replace only anchored branches" {
        foreach ($caseName in @("mixedAnyOf", "oneOf")) {
            $source = New-TestOpenApiDocument -Schema (
                Get-TestFixtureCase -Cases $schemaCases -Name $caseName
            )
            $result = ConvertTo-SimplifiedOpenApiRecursiveSchemaDocument -Document $source
            $schema = Get-TestResponseSchema -Document $result.Document
            Assert-Equal 0 $result.CollapsedAnyOfCount "Case '$caseName' collapsed its parent."
            Assert-Equal 1 $result.ReplacedRecursiveSchemaCount "Case '$caseName' did not replace its anchor."
            $compositionName = if ($caseName -ceq "oneOf") { "oneOf" } else { "anyOf" }
            Assert-FreeFormObjectSchema $schema[$compositionName][0] "Case '$caseName' branch replacement."
            Assert-Equal "keep $($caseName -eq 'oneOf' ? 'oneOf' : 'mixed') wrapper" $schema.description "Case '$caseName' lost its wrapper."
        }
    }

    # Parent eligibility checks direct branches only. An anchor under a branch's
    # properties must not cause the enclosing all-object anyOf to collapse.
    Invoke-Test "descendant anchors do not make an all-object parent collapsible" {
        $source = New-TestOpenApiDocument -Schema (
            Get-TestFixtureCase -Cases $schemaCases -Name "nestedAnchor"
        )
        $result = ConvertTo-SimplifiedOpenApiRecursiveSchemaDocument -Document $source
        $schema = Get-TestResponseSchema -Document $result.Document
        Assert-Equal 0 $result.CollapsedAnyOfCount "A non-direct anchor collapsed its parent."
        Assert-Equal 1 $result.ReplacedRecursiveSchemaCount "The descendant anchor was not replaced."
        Assert-Equal 2 $schema.anyOf.Count "The parent anyOf changed shape."
        Assert-FreeFormObjectSchema $schema.anyOf[0].properties.nested "Nested branch replacement."
    }

    # Exact JSON types and schema context matter: false/string anchors, nonobject
    # types, malformed unions, and anchor-shaped examples must all be preserved.
    Invoke-Test "near misses and literal lookalikes remain unchanged" {
        $source = New-TestOpenApiDocument -Schema (
            Get-TestFixtureCase -Cases $schemaCases -Name "nearMissesAndLiterals"
        )
        $before = ConvertTo-Json -InputObject $source -Compress -Depth 100
        $result = ConvertTo-SimplifiedOpenApiRecursiveSchemaDocument -Document $source
        Assert-Equal 0 $result.CollapsedAnyOfCount "A near miss collapsed."
        Assert-Equal 0 $result.ReplacedRecursiveSchemaCount "A near miss was replaced."
        Assert-Equal 0 $result.RemovedRecursiveAnchorCount "A near-miss anchor was counted."
        Assert-Equal $before (ConvertTo-Json -InputObject $result.Document -Compress -Depth 100) "A near miss or literal changed."
    }

    # The independent postcondition should identify both forms of unfinished
    # work: a standalone eligible anchor and a collapsible parent composition.
    Invoke-Test "postcondition rejects eligible recursive schemas and parent anyOf candidates" {
        $standalone = New-TestOpenApiDocument -Schema (
            Get-TestFixtureCase -Cases $schemaCases -Name "standalone"
        )
        Assert-Throws {
            Assert-NoSimplifiableOpenApiRecursiveSchemas -Value $standalone
        } "*retained an eligible recursive anchor*"

        $parent = New-TestOpenApiDocument -Schema (
            Get-TestFixtureCase -Cases $schemaCases -Name "allObjectAnyOf"
        )
        Assert-Throws {
            Assert-NoSimplifiableOpenApiRecursiveSchemas -Value $parent
        } "*retained a collapsible recursive anyOf*"
    }

    # Replacing a schema does not move its root, so references to that exact
    # pointer remain valid and require no relocation.
    Invoke-Test "references to candidate roots remain valid" {
        $schema = Get-TestFixtureCase -Cases $schemaCases -Name "rootReference"
        $targetPointer = "#/paths/~1test/post/responses/200/content/application~1json/schema/properties/target"
        $source = New-TestOpenApiDocument -Schema $schema
        $result = ConvertTo-SimplifiedOpenApiRecursiveSchemaDocument -Document $source
        $properties = (Get-TestResponseSchema -Document $result.Document).properties
        Assert-FreeFormObjectSchema $properties.target "Root-targeted replacement."
        Assert-Equal $targetPointer $properties.alias.'$ref' "The root reference changed."
    }

    # Descendants of a replacement disappear. Every supported local-reference
    # form must fail rather than silently retain a dangling pointer.
    Invoke-Test "references into disappearing candidate descendants fail clearly" {
        $targetPointer = "#/paths/~1test/post/responses/200/content/application~1json/schema/properties/target"
        foreach ($referenceProperty in @('$ref', '$dynamicRef', '$recursiveRef')) {
            $schema = Get-TestFixtureCase -Cases $schemaCases -Name "rootReference"
            $alias = [ordered]@{}
            $alias[$referenceProperty] = "$targetPointer/anyOf/0/properties/field"
            $schema.properties.alias = $alias
            $source = New-TestOpenApiDocument -Schema $schema
            Assert-Throws {
                ConvertTo-SimplifiedOpenApiRecursiveSchemaDocument -Document $source
            } "*Could not resolve*"
        }

        $schema = Get-TestFixtureCase -Cases $schemaCases -Name "rootReference"
        $schema.properties.alias = [ordered]@{
            type = "object"
            discriminator = [ordered]@{
                propertyName = "kind"
                mapping = [ordered]@{
                    target = "$targetPointer/anyOf/0"
                }
            }
        }
        Assert-Throws {
            ConvertTo-SimplifiedOpenApiRecursiveSchemaDocument -Document (
                New-TestOpenApiDocument -Schema $schema
            )
        } "*Could not resolve*"
    }

    # The compact golden covers parent collapse, individual replacement, mixed
    # unions, literal preservation, member order, and aggregate statistics.
    Invoke-Test "compact fixture matches its recursive-simplified golden" {
        $source = Read-OpenApiDocument -Path (Join-Path $fixtureDirectory "source.openapi.json")
        $expected = Read-OpenApiDocument -Path (Join-Path $fixtureDirectory "expected.recursive-simplified.openapi.json")
        $result = ConvertTo-SimplifiedOpenApiRecursiveSchemaDocument -Document $source
        Assert-Equal 1 $result.CollapsedAnyOfCount "Unexpected golden collapse count."
        Assert-Equal 2 $result.ReplacedRecursiveSchemaCount "Unexpected golden individual replacement count."
        Assert-Equal 3 $result.RemovedRecursiveAnchorCount "Unexpected golden removed-anchor count."
        Assert-JsonEqual $expected $result.Document "Recursive-simplified fixture did not match its golden."
    }

    # Public file handling must preserve the input, choose the documented
    # default suffix, and emit deterministic UTF-8 without a BOM.
    Invoke-Test "default and explicit output paths encoding and input preservation are correct" {
        $inputPath = Join-Path $temporaryDirectory "sample.simplified.openapi.json"
        Copy-Item -LiteralPath (Join-Path $fixtureDirectory "source.openapi.json") -Destination $inputPath
        [byte[]]$inputBefore = [System.IO.File]::ReadAllBytes($inputPath)
        & $simplifyScriptPath -InputPath $inputPath
        $outputPath = Join-Path $temporaryDirectory "sample.simplified.recursive-simplified.openapi.json"

        Assert-True ([System.IO.File]::Exists($outputPath)) "Default recursive-simplified output was not created."
        Assert-BytesEqual $inputBefore ([System.IO.File]::ReadAllBytes($inputPath)) "Simplifier modified its input."
        [byte[]]$outputBytes = [System.IO.File]::ReadAllBytes($outputPath)
        $hasBom = $outputBytes.Length -ge 3 -and $outputBytes[0] -eq 0xEF -and $outputBytes[1] -eq 0xBB -and $outputBytes[2] -eq 0xBF
        Assert-True (-not $hasBom) "Recursive-simplified output contains a BOM."
        Assert-Equal 10 $outputBytes[$outputBytes.Length - 1] "Recursive-simplified output lacks a trailing LF."

        $explicitPath = Join-Path $temporaryDirectory "explicit-output.json"
        & $simplifyScriptPath -InputPath $inputPath -OutputPath $explicitPath
        Assert-BytesEqual $outputBytes ([System.IO.File]::ReadAllBytes($explicitPath)) "Explicit output differed from default output."
    }

    # A completed artifact can be processed in place repeatedly with zero
    # transformations, stable bytes, and no duplicated filename suffix.
    Invoke-Test "simplification is byte-idempotent and avoids repeated suffixes" {
        $inputPath = Join-Path $temporaryDirectory "already.recursive-simplified.openapi.json"
        Copy-Item -LiteralPath (Join-Path $fixtureDirectory "expected.recursive-simplified.openapi.json") -Destination $inputPath
        Assert-Equal ([System.IO.Path]::GetFullPath($inputPath)) (Get-DefaultSimplifiedOpenApiRecursiveSchemaPath $inputPath) "Already-simplified default path changed."
        $null = Invoke-OpenApiRecursiveSchemaSimplifier -InputPath $inputPath
        [byte[]]$firstRun = [System.IO.File]::ReadAllBytes($inputPath)
        $null = Invoke-OpenApiRecursiveSchemaSimplifier -InputPath $inputPath
        Assert-BytesEqual $firstRun ([System.IO.File]::ReadAllBytes($inputPath)) "Simplification was not byte-idempotent."
    }

    # File-system and JSON failures belong at the public invocation boundary and
    # should surface before any partial output is written.
    Invoke-Test "missing directory and invalid JSON inputs fail clearly" {
        Assert-Throws {
            Invoke-OpenApiRecursiveSchemaSimplifier -InputPath (Join-Path $temporaryDirectory "missing.json")
        } "*cannot find path*"
        Assert-Throws {
            Invoke-OpenApiRecursiveSchemaSimplifier -InputPath $temporaryDirectory
        } "*is not a file*"

        $invalidPath = Join-Path $temporaryDirectory "invalid.json"
        [System.IO.File]::WriteAllText($invalidPath, "{ invalid", [System.Text.UTF8Encoding]::new($false))
        Assert-Throws {
            Invoke-OpenApiRecursiveSchemaSimplifier -InputPath $invalidPath
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
