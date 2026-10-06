<#
.SYNOPSIS
    Runs focused tests for scripts/specification/pipeline/Simplify-OpenApiStringVariants.ps1.
#>

[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$scriptDirectory = Join-Path $PSScriptRoot ".." "pipeline"
$simplifyScriptPath = Join-Path $scriptDirectory "Simplify-OpenApiStringVariants.ps1"
$sortScriptPath = Join-Path $scriptDirectory "Sort-OpenApiDocument.ps1"
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
        throw "$Message"
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

function New-SelectedShapeSchema {
    return Read-TestJsonFixture `
        -RelativePath "Simplify-OpenApiStringVariants/selected-shape.schema.json"
}

$fixtureDirectory = Join-Path $PSScriptRoot "fixtures" "Simplify-OpenApiStringVariants"
$schemaCases = Read-TestJsonFixture -RelativePath "Simplify-OpenApiStringVariants/cases.schema.json"
$temporaryDirectory = Join-Path ([System.IO.Path]::GetTempPath()) "simplify-openapi-$([Guid]::NewGuid().ToString('N'))"
$null = [System.IO.Directory]::CreateDirectory($temporaryDirectory)

try {
    # Focused tests and orchestration load stage functions directly; dot-sourcing
    # must not trigger command-line file generation.
    Invoke-Test "dot-sourcing defines simplifier functions without running the CLI" {
        Assert-True ($null -ne (Get-Command ConvertTo-SimplifiedOpenApiStringVariantDocument -ErrorAction SilentlyContinue)) "Conversion function was not defined."
        Assert-True ($null -ne (Get-Command Invoke-OpenApiStringVariantSimplifier -ErrorAction SilentlyContinue)) "CLI function was not defined."
    }

    # Simplify owns structural consolidation and first-seen deduplication, while
    # canonical enum ordering remains the downstream Sort stage's job.
    Invoke-Test "nested plain and enum branches simplify without sorting" {
        $schema = New-SelectedShapeSchema
        $source = New-TestOpenApiDocument -Schema $schema
        $sourceBefore = ConvertTo-Json -InputObject $source -Compress -Depth 100
        $result = ConvertTo-SimplifiedOpenApiStringVariantDocument -Document $source
        $simplified = Get-TestResponseSchema -Document $result.Document

        Assert-Equal 1 $result.SimplifiedCount "Unexpected simplification count."
        Assert-Equal 1 $result.DeduplicatedEnumValueCount "Unexpected duplicate enum count."
        Assert-Equal "Keep wrapper" $simplified.description "Wrapper description changed."
        Assert-Equal 2 $simplified.anyOf.Count "Unexpected output branch count."
        Assert-Sequence @("type") @((Get-JsonProperties $simplified.anyOf[0]).Name) "Plain branch was not canonical."
        Assert-Sequence @("type", "enum") @((Get-JsonProperties $simplified.anyOf[1]).Name) "Enum branch retained annotations."
        Assert-Sequence @("zeta", "alpha", "shared", "middle") @($simplified.anyOf[1].enum) "Merged enum order was incorrect."
        Assert-Equal $sourceBefore (ConvertTo-Json -InputObject $source -Compress -Depth 100) "Conversion mutated its input."
    }

    # Plain and enum output branches must retain category encounter order so the
    # semantic stage does not perform hidden canonicalization.
    Invoke-Test "branch category order follows the first recursive leaf" {
        $source = New-TestOpenApiDocument -Schema (
            Get-TestFixtureCase -Cases $schemaCases -Name "branchCategoryOrder"
        )
        $schema = Get-TestResponseSchema -Document (
            ConvertTo-SimplifiedOpenApiStringVariantDocument -Document $source
        ).Document
        Assert-True (Test-JsonProperty -Object $schema.anyOf[0] -Name "enum") "Enum branch did not remain first."
        Assert-True (-not (Test-JsonProperty -Object $schema.anyOf[1] -Name "enum")) "Plain branch did not remain second."
        Assert-Sequence @("first", "second") @($schema.anyOf[0].enum) "Enum values changed order."
    }

    # Recursive nesting depth and single-category unions should normalize to a
    # stable branch shape without deleting the containing composition keyword.
    Invoke-Test "arbitrary nesting all-enum and all-plain trees retain anyOf" {
        $source = New-TestOpenApiDocument -Schema (
            Get-TestFixtureCase -Cases $schemaCases -Name "arbitraryNesting"
        )
        $result = ConvertTo-SimplifiedOpenApiStringVariantDocument -Document $source
        $properties = (Get-TestResponseSchema -Document $result.Document).properties
        Assert-Equal 2 $result.SimplifiedCount "Deep candidates were not both simplified."
        Assert-Equal 1 $properties.enumOnly.anyOf.Count "All-enum anyOf was removed."
        Assert-Sequence @("a", "b") @($properties.enumOnly.anyOf[0].enum) "All-enum values were incorrect."
        Assert-Equal 1 $properties.plainOnly.anyOf.Count "All-plain anyOf was removed."
        Assert-Sequence @("type") @((Get-JsonProperties $properties.plainOnly.anyOf[0]).Name) "All-plain branch was incorrect."
    }

    # A string-only subtree inside a mixed union cannot be promoted independently
    # without changing the outer union's documented branch boundaries.
    Invoke-Test "mixed unions preserve directly connected string subtrees" {
        foreach ($otherType in @("null", "object", "number")) {
            $schema = Get-TestFixtureCase -Cases $schemaCases -Name "mixedUnion"
            $schema.anyOf[1].type = $otherType
            $source = New-TestOpenApiDocument -Schema $schema
            $result = ConvertTo-SimplifiedOpenApiStringVariantDocument -Document $source
            $schema = Get-TestResponseSchema -Document $result.Document
            Assert-Equal 0 $result.SimplifiedCount "A mixed union was simplified."
            Assert-Equal 2 $schema.anyOf[0].anyOf.Count "A mixed union's string subtree changed."
        }
    }

    # Suppression applies only to the connected mixed composition; separate
    # child schemas should still receive their own simplification pass.
    Invoke-Test "independent child schemas inside mixed branches remain eligible" {
        $schema = Get-TestFixtureCase -Cases $schemaCases -Name "independentChild"
        $schema.anyOf[0].properties.model = New-SelectedShapeSchema
        $source = New-TestOpenApiDocument -Schema $schema
        $result = ConvertTo-SimplifiedOpenApiStringVariantDocument -Document $source
        $model = (Get-TestResponseSchema -Document $result.Document).anyOf[0].properties.model
        Assert-Equal 1 $result.SimplifiedCount "Independent child candidate was not simplified."
        Assert-Equal 2 $model.anyOf.Count "Independent child output was incorrect."
    }

    # Constraints beyond type/enum cannot be safely merged by this narrow stage,
    # so any unsupported leaf protects the complete connected tree.
    Invoke-Test "unsupported constraints and invalid enums make complete trees ineligible" {
        $unsupportedLeaves = $schemaCases.unsupportedLeaves
        foreach ($unsupportedLeaf in $unsupportedLeaves) {
            $schema = Get-TestFixtureCase -Cases $schemaCases -Name "unsupportedTree"
            $schema.anyOf[0].anyOf[1] = Copy-JsonValue -Value $unsupportedLeaf
            $source = New-TestOpenApiDocument -Schema $schema
            $before = ConvertTo-Json -InputObject $source -Compress -Depth 100
            $result = ConvertTo-SimplifiedOpenApiStringVariantDocument -Document $source
            Assert-Equal 0 $result.SimplifiedCount "An unsupported string tree was simplified."
            Assert-Equal $before (ConvertTo-Json -InputObject $result.Document -Compress -Depth 100) "An unsupported string tree changed."
        }
    }

    # The stage targets nested schema unions only; flat unions and lookalike
    # objects in literal/example contexts are outside its ownership.
    Invoke-Test "already-flat unions and literal anyOf objects remain unchanged" {
        $schema = Get-TestFixtureCase -Cases $schemaCases -Name "alreadyFlatLiteral"
        $source = New-TestOpenApiDocument -Schema $schema
        $before = ConvertTo-Json -InputObject $source -Compress -Depth 100
        $result = ConvertTo-SimplifiedOpenApiStringVariantDocument -Document $source
        Assert-Equal 0 $result.SimplifiedCount "Flat or literal anyOf was simplified."
        Assert-Equal $before (ConvertTo-Json -InputObject $result.Document -Compress -Depth 100) "Flat or literal anyOf changed."
    }

    # The postcondition guards against traversal regressions that silently leave
    # a schema the stage claims it can simplify.
    Invoke-Test "postcondition rejects an eligible nested string union" {
        $source = New-TestOpenApiDocument -Schema (New-SelectedShapeSchema)
        Assert-Throws {
            Assert-NoSimplifiableOpenApiStringVariants -Value $source
        } "*retained an eligible nested string anyOf*"
    }

    # The candidate object remains at the same pointer even though its internal
    # branches are consolidated, so root references should survive unchanged.
    Invoke-Test "references to candidate roots remain valid" {
        $candidatePointer = "#/paths/~1test/post/responses/200/content/application~1json/schema/properties/model"
        $schema = Get-TestFixtureCase -Cases $schemaCases -Name "rootReference"
        $schema.properties.model = New-SelectedShapeSchema
        $source = New-TestOpenApiDocument -Schema $schema
        $result = ConvertTo-SimplifiedOpenApiStringVariantDocument -Document $source
        Assert-Equal 1 $result.SimplifiedCount "Root-targeted candidate was not simplified."
        Assert-Equal $candidatePointer (Get-TestResponseSchema -Document $result.Document).properties.alias.'$ref' "Root reference changed."
    }

    # Many-to-one branch merging has no semantics-preserving destination for an
    # external pointer into a removed branch, so normalization must fail safely.
    Invoke-Test "references into disappearing candidate branches fail before rewriting" {
        $candidatePointer = "#/paths/~1test/post/responses/200/content/application~1json/schema/properties/model"
        foreach ($referenceProperty in @('$ref', '$dynamicRef', '$recursiveRef')) {
            $alias = [ordered]@{}
            $alias[$referenceProperty] = "$candidatePointer/anyOf/0/anyOf/0"
            $schema = Get-TestFixtureCase -Cases $schemaCases -Name "rootReference"
            $schema.properties.model = New-SelectedShapeSchema
            $schema.properties.alias = $alias
            $source = New-TestOpenApiDocument -Schema $schema
            Assert-Throws {
                ConvertTo-SimplifiedOpenApiStringVariantDocument -Document $source
            } "*targets*inside the anyOf branches*"
        }

        $mappingSchema = Get-TestFixtureCase -Cases $schemaCases -Name "mappingReference"
        $mappingSchema.properties.model = New-SelectedShapeSchema
        $sourceWithMapping = New-TestOpenApiDocument -Schema $mappingSchema
        Assert-Throws {
            ConvertTo-SimplifiedOpenApiStringVariantDocument -Document $sourceWithMapping
        } "*DiscriminatorMapping*targets*inside the anyOf branches*"
    }

    # Simplify expects a valid, allOf-free prior-stage document and should report
    # contract violations before attempting union consolidation.
    Invoke-Test "unresolved references and allOf input fail clearly" {
        $unresolved = New-TestOpenApiDocument -Schema (
            Get-TestFixtureCase -Cases $schemaCases -Name "unresolvedReference"
        )
        Assert-Throws {
            ConvertTo-SimplifiedOpenApiStringVariantDocument -Document $unresolved
        } "*Could not resolve Reference*"

        $unflattened = New-TestOpenApiDocument -Schema (
            Get-TestFixtureCase -Cases $schemaCases -Name "unflattened"
        )
        Assert-Throws {
            ConvertTo-SimplifiedOpenApiStringVariantDocument -Document $unflattened
        } "*requires allOf-flattened input*Flatten-OpenApiAllOf.ps1 first*"
    }

    # Compact goldens lock both the semantic intermediate and the downstream
    # canonical result without invoking the production-size pipeline.
    Invoke-Test "compact fixture matches simplified and sorted goldens" {
        $source = Read-OpenApiDocument -Path (Join-Path $fixtureDirectory "source.openapi.json")
        $expectedSimplified = Read-OpenApiDocument -Path (Join-Path $fixtureDirectory "expected.simplified.openapi.json")
        $expectedSorted = Read-OpenApiDocument -Path (Join-Path $fixtureDirectory "expected.simplified.sorted.openapi.json")
        $result = ConvertTo-SimplifiedOpenApiStringVariantDocument -Document $source
        Assert-JsonEqual $expectedSimplified $result.Document "Simplified fixture did not match its golden."

        . $sortScriptPath
        $sorted = ConvertTo-SortedOpenApiDocument -Document $result.Document
        Assert-JsonEqual $expectedSorted $sorted "Sorted fixture did not match its golden."
    }

    # CLI output-path choices must produce identical deterministic bytes and
    # never mutate the previous stage's artifact.
    Invoke-Test "default and explicit output paths encoding and input preservation are correct" {
        $inputPath = Join-Path $temporaryDirectory "sample.inlined.flattened.openapi.json"
        Copy-Item -LiteralPath (Join-Path $fixtureDirectory "source.openapi.json") -Destination $inputPath
        [byte[]]$inputBefore = [System.IO.File]::ReadAllBytes($inputPath)
        & $simplifyScriptPath -InputPath $inputPath
        $outputPath = Join-Path $temporaryDirectory "sample.inlined.flattened.simplified.openapi.json"

        Assert-True ([System.IO.File]::Exists($outputPath)) "Default simplified output was not created."
        Assert-BytesEqual $inputBefore ([System.IO.File]::ReadAllBytes($inputPath)) "Simplifier modified its input."
        [byte[]]$outputBytes = [System.IO.File]::ReadAllBytes($outputPath)
        $hasBom = $outputBytes.Length -ge 3 -and $outputBytes[0] -eq 0xEF -and $outputBytes[1] -eq 0xBB -and $outputBytes[2] -eq 0xBF
        Assert-True (-not $hasBom) "Simplified output contains a BOM."
        Assert-Equal 10 $outputBytes[$outputBytes.Length - 1] "Simplified output lacks a trailing LF."

        $explicitPath = Join-Path $temporaryDirectory "explicit-output.json"
        & $simplifyScriptPath -InputPath $inputPath -OutputPath $explicitPath
        Assert-True ([System.IO.File]::Exists($explicitPath)) "Explicit simplified output was not created."
        Assert-BytesEqual $outputBytes ([System.IO.File]::ReadAllBytes($explicitPath)) "Explicit output differed from default output."
    }

    # Reprocessing normalized output should be a byte no-op and should not grow
    # the filename marker repeatedly.
    Invoke-Test "simplification is byte-idempotent and avoids repeated suffixes" {
        $inputPath = Join-Path $temporaryDirectory "already.simplified.openapi.json"
        Copy-Item -LiteralPath (Join-Path $fixtureDirectory "expected.simplified.openapi.json") -Destination $inputPath
        Assert-Equal ([System.IO.Path]::GetFullPath($inputPath)) (Get-DefaultSimplifiedOpenApiPath $inputPath) "Already-simplified default path changed."
        $null = Invoke-OpenApiStringVariantSimplifier -InputPath $inputPath
        [byte[]]$firstRun = [System.IO.File]::ReadAllBytes($inputPath)
        $null = Invoke-OpenApiStringVariantSimplifier -InputPath $inputPath
        Assert-BytesEqual $firstRun ([System.IO.File]::ReadAllBytes($inputPath)) "Simplification was not byte-idempotent."
    }

    # Public file handling should reject absent paths, directories, and malformed
    # JSON without leaving generated artifacts behind.
    Invoke-Test "missing directory and invalid JSON inputs fail clearly" {
        Assert-Throws {
            Invoke-OpenApiStringVariantSimplifier -InputPath (Join-Path $temporaryDirectory "missing.json")
        } "*cannot find path*"
        Assert-Throws {
            Invoke-OpenApiStringVariantSimplifier -InputPath $temporaryDirectory
        } "*is not a file*"

        $invalidPath = Join-Path $temporaryDirectory "invalid.json"
        [System.IO.File]::WriteAllText($invalidPath, "{ invalid", [System.Text.UTF8Encoding]::new($false))
        Assert-Throws {
            Invoke-OpenApiStringVariantSimplifier -InputPath $invalidPath
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