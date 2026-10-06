<#
.SYNOPSIS
    Runs focused tests for scripts/specification/pipeline/Flatten-OpenApiAllOf.ps1.

.DESCRIPTION
    Uses the repository's dependency-free assertion style to validate allOf
    flattening, pointer relocation, deterministic file output, and idempotence.
#>

[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$scriptDirectory = Join-Path $PSScriptRoot ".." "pipeline"
$flattenScriptPath = Join-Path $scriptDirectory "Flatten-OpenApiAllOf.ps1"
. $flattenScriptPath
. (Join-Path $PSScriptRoot "_OpenApiTestFixture.Helpers.ps1")

$script:PassedTests = 0
$script:FailedTests = 0
$script:Failures = [System.Collections.Generic.List[string]]::new()

function Invoke-Test {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Name,

        [Parameter(Mandatory = $true)]
        [scriptblock]$Test
    )

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
    param(
        [Parameter(Mandatory = $true)]
        [bool]$Condition,

        [Parameter(Mandatory = $true)]
        [string]$Message
    )

    if (-not $Condition) {
        throw $Message
    }
}

function Assert-Equal {
    param(
        [Parameter(Mandatory = $true)]
        [AllowNull()]
        [object]$Expected,

        [Parameter(Mandatory = $true)]
        [AllowNull()]
        [object]$Actual,

        [Parameter(Mandatory = $true)]
        [string]$Message
    )

    if ($Expected -ne $Actual) {
        throw "$Message Expected '$Expected', got '$Actual'."
    }
}

function Assert-Sequence {
    param(
        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [object[]]$Expected,

        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [object[]]$Actual,

        [Parameter(Mandatory = $true)]
        [string]$Message
    )

    $expectedJson = ConvertTo-Json -InputObject @($Expected) -Compress -Depth 100
    $actualJson = ConvertTo-Json -InputObject @($Actual) -Compress -Depth 100
    if (-not [string]::Equals($expectedJson, $actualJson, [System.StringComparison]::Ordinal)) {
        throw "$Message Expected '$expectedJson', got '$actualJson'."
    }
}

function Assert-Throws {
    param(
        [Parameter(Mandatory = $true)]
        [scriptblock]$Action,

        [Parameter(Mandatory = $true)]
        [string]$MessagePattern
    )

    try {
        & $Action
    }
    catch {
        if ($_.Exception.Message -notlike $MessagePattern) {
            throw "Expected error matching '$MessagePattern', got '$($_.Exception.Message)'."
        }
        return
    }

    throw "Expected an error matching '$MessagePattern', but no error was thrown."
}

function Assert-BytesEqual {
    param(
        [Parameter(Mandatory = $true)]
        [string]$ExpectedPath,

        [Parameter(Mandatory = $true)]
        [string]$ActualPath,

        [Parameter(Mandatory = $true)]
        [string]$Message
    )

    $expected = [System.IO.File]::ReadAllBytes([System.IO.Path]::GetFullPath($ExpectedPath))
    $actual = [System.IO.File]::ReadAllBytes([System.IO.Path]::GetFullPath($ActualPath))
    if (-not [System.Linq.Enumerable]::SequenceEqual($expected, $actual)) {
        throw "$Message Expected SHA-256 '$((Get-FileHash $ExpectedPath).Hash)', got '$((Get-FileHash $ActualPath).Hash)'."
    }
}

$fixtureDirectory = Join-Path $PSScriptRoot "fixtures" "Flatten-OpenApiAllOf"
$schemaCases = Read-TestJsonFixture -RelativePath "Flatten-OpenApiAllOf/cases.schema.json"
$temporaryDirectory = Join-Path ([System.IO.Path]::GetTempPath()) "flatten-openapi-allof-$([Guid]::NewGuid().ToString('N'))"
$null = [System.IO.Directory]::CreateDirectory($temporaryDirectory)

try {
    # Stage functions are dot-sourced by focused tests and other tooling; doing
    # so must not perform file I/O or invoke the command entry point.
    Invoke-Test "dot-sourcing defines functions without executing the CLI" {
        Assert-True ($null -ne (Get-Command ConvertTo-FlattenedOpenApiDocument -ErrorAction SilentlyContinue)) "Conversion function was not defined."
        Assert-True ($null -ne (Get-Command Invoke-OpenApiAllOfFlattener -ErrorAction SilentlyContinue)) "CLI function was not defined."
    }

    # Flatten must recursively collapse every composition while preserving the
    # merged schema's first-seen property and required-name order.
    Invoke-Test "simple and nested allOf schemas are flattened" {
        $schema = Get-TestFixtureCase -Cases $schemaCases -Name "simpleNested"
        $source = New-TestOpenApiDocument -Schema $schema
        $sourceBefore = ConvertTo-Json -InputObject $source -Depth 100 -Compress
        $result = ConvertTo-FlattenedOpenApiDocument -Document $source
        $flattened = Get-TestResponseSchema -Document $result.Document

        Assert-Equal 2 $result.FlattenedCount "Wrong number of allOf compositions was flattened."
        Assert-NoOpenApiAllOf -Value $result.Document
        Assert-Equal "object" $flattened.type "Flattened type was lost."
        Assert-Equal "Outer description" $flattened.description "Containing description did not win."
        Assert-Equal $true $flattened.nullable "Nested constraint was lost."
        Assert-Sequence @("text", "type") @($flattened.required) "Required names did not preserve first-seen order."
        Assert-Sequence @("text", "type", "optional") @((Get-JsonProperties -Value $flattened.properties).Name) "Properties did not preserve first-seen order."
        Assert-Equal $sourceBefore (ConvertTo-Json -InputObject $source -Depth 100 -Compress) "Source document was mutated."
    }

    # Repeated compatible constraints should combine without weakening schema
    # validation or duplicating required names and enum values.
    Invoke-Test "compatible duplicate properties and enums are merged" {
        $source = New-TestOpenApiDocument -Schema (
            Get-TestFixtureCase -Cases $schemaCases -Name "compatibleDuplicates"
        )
        $result = ConvertTo-FlattenedOpenApiDocument -Document $source
        $status = (Get-TestResponseSchema -Document $result.Document).properties.status
        Assert-Sequence @("completed", "incomplete") @($status.enum) "Enum intersection was incorrect."
    }

    # Conflicting definitions cannot be flattened safely; the error must name
    # the exact property path that made the merge ambiguous.
    Invoke-Test "incompatible property definitions fail with a breadcrumb" {
        $source = New-TestOpenApiDocument -Schema (
            Get-TestFixtureCase -Cases $schemaCases -Name "incompatibleProperties"
        )
        Assert-Throws { ConvertTo-FlattenedOpenApiDocument -Document $source } "*duplicate*"
    }

    # Different validating keywords at the same merged schema location must be
    # rejected rather than resolved by arbitrary precedence.
    Invoke-Test "incompatible validating keywords fail" {
        $source = New-TestOpenApiDocument -Schema (
            Get-TestFixtureCase -Cases $schemaCases -Name "incompatibleKeywords"
        )
        Assert-Throws { ConvertTo-FlattenedOpenApiDocument -Document $source } "*keyword 'type'*"
    }

    # allOf means intersection, so enum constraints with no common value make
    # the schema unsatisfiable and should fail explicitly.
    Invoke-Test "disjoint enum constraints fail" {
        $source = New-TestOpenApiDocument -Schema (
            Get-TestFixtureCase -Cases $schemaCases -Name "disjointEnums"
        )
        Assert-Throws { ConvertTo-FlattenedOpenApiDocument -Document $source } "*no common values*"
    }

    # This flattener supports object schema definitions only and must reject
    # malformed/empty compositions before emitting partial output.
    Invoke-Test "empty and non-object allOf branches fail" {
        Assert-Throws {
            ConvertTo-FlattenedOpenApiDocument -Document (
                New-TestOpenApiDocument -Schema (
                    Get-TestFixtureCase -Cases $schemaCases -Name "emptyAllOf"
                )
            )
        } "*must contain at least one*"
        Assert-Throws {
            ConvertTo-FlattenedOpenApiDocument -Document (
                New-TestOpenApiDocument -Schema (
                    Get-TestFixtureCase -Cases $schemaCases -Name "nonObjectBranch"
                )
            )
        } "*must be an object definition*"
    }

    # Moving branch contents to the containing schema changes JSON Pointer
    # paths, so retained local references must follow the move.
    Invoke-Test "local pointers under allOf branches are relocated" {
        $source = New-TestOpenApiDocument -Schema (
            Get-TestFixtureCase -Cases $schemaCases -Name "localPointers"
        )
        $result = ConvertTo-FlattenedOpenApiDocument -Document $source
        $flattened = Get-TestResponseSchema -Document $result.Document
        Assert-Equal `
            "#/paths/~1test/post/responses/200/content/application~1json/schema/properties/kind" `
            $flattened.properties.child.'$ref' `
            "Local reference was not relocated."
        Assert-OpenApiReferencesResolve -Document $result.Document
    }

    # Relocation must preserve RFC 6901 escaping for property names containing
    # slash or tilde characters.
    Invoke-Test "escaped JSON Pointer segments are relocated safely" {
        $source = New-TestOpenApiDocument -Schema (
            Get-TestFixtureCase -Cases $schemaCases -Name "escapedPointers"
        )
        $result = ConvertTo-FlattenedOpenApiDocument -Document $source
        $flattened = Get-TestResponseSchema -Document $result.Document
        Assert-Equal `
            "#/paths/~1test/post/responses/200/content/application~1json/schema/properties/a~1b~0c" `
            $flattened.properties.child.'$ref' `
            "Escaped local reference was not relocated."
        Assert-OpenApiReferencesResolve -Document $result.Document
    }

    # Discriminator mappings are pointer-valued even though they are not $ref;
    # they must remain aligned with relocated schema branches.
    Invoke-Test "discriminator mapping pointers under allOf branches are relocated" {
        $source = New-TestOpenApiDocument -Schema (
            Get-TestFixtureCase -Cases $schemaCases -Name "discriminatorMappings"
        )
        $result = ConvertTo-FlattenedOpenApiDocument -Document $source
        $mapping = (Get-TestResponseSchema -Document $result.Document).discriminator.mapping
        Assert-Equal "#/paths/~1test/post/responses/200/content/application~1json/schema/oneOf/0" $mapping.a "First mapping was not relocated."
        Assert-Equal "#/paths/~1test/post/responses/200/content/application~1json/schema/oneOf/1" $mapping.b "Second mapping was not relocated."
        Assert-OpenApiReferencesResolve -Document $result.Document
    }

    # A source prefix cannot map to two destinations without making reference
    # rewriting nondeterministic.
    Invoke-Test "conflicting pointer relocations fail clearly" {
        $relocations = [System.Collections.Generic.Dictionary[string, string]]::new(
            [System.StringComparer]::Ordinal
        )
        Add-OpenApiPointerRelocation -Relocations $relocations -SourcePrefix "#/source" -DestinationPrefix "#/first"
        Assert-Throws {
            Add-OpenApiPointerRelocation -Relocations $relocations -SourcePrefix "#/source" -DestinationPrefix "#/second"
        } "*conflicting relocation destinations*"
    }

    # Successful textual relocation is insufficient; every rewritten pointer
    # must resolve against the final flattened structure.
    Invoke-Test "relocated pointers must resolve in the flattened document" {
        $source = New-TestOpenApiDocument -Schema (
            Get-TestFixtureCase -Cases $schemaCases -Name "unresolvedRelocation"
        )
        Assert-Throws { ConvertTo-FlattenedOpenApiDocument -Document $source } "*has no 'missing' property*"
    }

    # An already-flat artifact should be safe to reprocess without byte churn.
    Invoke-Test "flattening is byte-idempotent" {
        $goldenPath = Join-Path $fixtureDirectory "expected.flattened.openapi.json"
        $outputPath = Join-Path $temporaryDirectory "idempotent.openapi.json"
        $null = Invoke-OpenApiAllOfFlattener -InputPath $goldenPath -OutputPath $outputPath
        Assert-BytesEqual -ExpectedPath $goldenPath -ActualPath $outputPath -Message "Flattening was not idempotent."
    }

    # Default invocation must retain its input and derive a predictable sibling
    # artifact rather than overwriting the previous pipeline stage.
    Invoke-Test "default output path creates a sibling flattened file" {
        $inputPath = Join-Path $temporaryDirectory "sample.inlined.openapi.json"
        Copy-Item -LiteralPath (Join-Path $fixtureDirectory "expected.flattened.openapi.json") -Destination $inputPath
        $expectedPath = Join-Path $temporaryDirectory "sample.inlined.flattened.openapi.json"
        $null = Invoke-OpenApiAllOfFlattener -InputPath $inputPath
        Assert-True ([System.IO.File]::Exists($expectedPath)) "Default flattened output was not created."
        Assert-BytesEqual -ExpectedPath $inputPath -ActualPath $expectedPath -Message "Default output changed already-flat input."
    }

    # Reprocessing a flattened artifact should target the same path, not append
    # an unbounded sequence of `.flattened` markers.
    Invoke-Test "already-flattened default output does not repeat the suffix" {
        $inputPath = Join-Path $temporaryDirectory "already.flattened.openapi.json"
        Copy-Item -LiteralPath (Join-Path $fixtureDirectory "expected.flattened.openapi.json") -Destination $inputPath
        Assert-Equal `
            ([System.IO.Path]::GetFullPath($inputPath)) `
            (Get-DefaultFlattenedOpenApiPath -InputPath $inputPath) `
            "Already-flattened default path changed."
        [byte[]]$before = [System.IO.File]::ReadAllBytes($inputPath)
        $null = Invoke-OpenApiAllOfFlattener -InputPath $inputPath
        [byte[]]$after = [System.IO.File]::ReadAllBytes($inputPath)
        Assert-True ([System.Linq.Enumerable]::SequenceEqual($before, $after)) "Default idempotent rewrite changed bytes."
    }

    # The public script entry point must honor explicit destinations in addition
    # to exposing conversion functions for unit tests.
    Invoke-Test "standalone invocation writes the requested output" {
        $inputPath = Join-Path $temporaryDirectory "standalone.inlined.openapi.json"
        $outputPath = Join-Path $temporaryDirectory "standalone.flattened.openapi.json"
        Copy-Item -LiteralPath (Join-Path $fixtureDirectory "expected.flattened.openapi.json") -Destination $inputPath
        & $flattenScriptPath -InputPath $inputPath -OutputPath $outputPath
        Assert-True ([System.IO.File]::Exists($outputPath)) "Standalone invocation did not create output."
        Assert-BytesEqual -ExpectedPath $inputPath -ActualPath $outputPath -Message "Standalone invocation changed already-flat input."
    }

    # Stage output must follow the repository's deterministic encoding contract
    # while treating the prior artifact as immutable.
    Invoke-Test "file output is UTF-8 without BOM and leaves input unchanged" {
        $inputPath = Join-Path $temporaryDirectory "preserved.openapi.json"
        $outputPath = Join-Path $temporaryDirectory "preserved.flattened.openapi.json"
        Copy-Item -LiteralPath (Join-Path $fixtureDirectory "source.openapi.json") -Destination $inputPath
        [byte[]]$before = [System.IO.File]::ReadAllBytes($inputPath)
        $null = Invoke-OpenApiAllOfFlattener -InputPath $inputPath -OutputPath $outputPath
        [byte[]]$after = [System.IO.File]::ReadAllBytes($inputPath)
        Assert-True ([System.Linq.Enumerable]::SequenceEqual($before, $after)) "Input file was modified."
        [byte[]]$outputBytes = [System.IO.File]::ReadAllBytes($outputPath)
        $hasBom = $outputBytes.Length -ge 3 -and $outputBytes[0] -eq 0xEF -and $outputBytes[1] -eq 0xBB -and $outputBytes[2] -eq 0xBF
        Assert-True (-not $hasBom) "Flattened output contains a UTF-8 BOM."
        Assert-Equal 10 $outputBytes[$outputBytes.Length - 1] "Flattened output lacks a trailing LF."
    }

    # Path and JSON errors should stop before transformation and identify the
    # invalid input rather than producing an output artifact.
    Invoke-Test "missing and invalid inputs fail clearly" {
        Assert-Throws { Invoke-OpenApiAllOfFlattener -InputPath (Join-Path $temporaryDirectory "missing.json") } "*cannot find path*"
        $invalidPath = Join-Path $temporaryDirectory "invalid.json"
        [System.IO.File]::WriteAllText($invalidPath, "{ invalid", [System.Text.UTF8Encoding]::new($false))
        Assert-Throws { Invoke-OpenApiAllOfFlattener -InputPath $invalidPath } "*Could not parse OpenAPI JSON*"
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
