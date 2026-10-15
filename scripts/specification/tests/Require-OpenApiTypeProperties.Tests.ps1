<#
.SYNOPSIS
    Runs focused tests for Require-OpenApiTypeProperties.ps1.
#>

[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$scriptPath = Join-Path $PSScriptRoot ".." "pipeline" "Require-OpenApiTypeProperties.ps1"
. $scriptPath
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
    param([object[]]$Expected, [object[]]$Actual, [string]$Message)
    $expectedJson = ConvertTo-Json -InputObject @($Expected) -Compress -Depth 100
    $actualJson = ConvertTo-Json -InputObject @($Actual) -Compress -Depth 100
    if (-not [string]::Equals($expectedJson, $actualJson, [System.StringComparison]::Ordinal)) {
        throw "$Message Expected '$expectedJson', got '$actualJson'."
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

function New-TypePropertyCandidate {
    param(
        [Parameter()]
        [AllowNull()]
        [object]$Required,

        [Parameter()]
        [object]$TypeProperty = ([ordered]@{ type = "string"; enum = @("example") }),

        [Parameter()]
        [switch]$OmitRequired
    )

    $schema = [ordered]@{ type = "object" }
    if (-not $OmitRequired) {
        $schema["required"] = $Required
    }
    $schema["properties"] = [ordered]@{
        id = [ordered]@{ type = "string" }
        type = $TypeProperty
    }
    return $schema
}

$temporaryDirectory = Join-Path ([System.IO.Path]::GetTempPath()) "require-openapi-type-properties-$([Guid]::NewGuid().ToString('N'))"
$null = [System.IO.Directory]::CreateDirectory($temporaryDirectory)

try {
    Invoke-Test "dot-sourcing defines functions without running the CLI" {
        Assert-True ($null -ne (Get-Command ConvertTo-OpenApiRequiredTypePropertiesDocument -ErrorAction SilentlyContinue)) "Conversion function was not defined."
        Assert-True ($null -ne (Get-Command Invoke-OpenApiTypePropertyRequirement -ErrorAction SilentlyContinue)) "CLI function was not defined."
    }

    Invoke-Test "eligible object schemas require type while preserving order" {
        $root = New-TypePropertyCandidate -OmitRequired
        $root.properties["append"] = New-TypePropertyCandidate -Required @("id") -TypeProperty ([ordered]@{ type = "integer" })
        $root.properties["already"] = New-TypePropertyCandidate -Required @("type", "id")
        $source = New-TestOpenApiDocument -Schema $root
        $sourceBefore = ConvertTo-Json -InputObject $source -Compress -Depth 100

        $result = ConvertTo-OpenApiRequiredTypePropertiesDocument -Document $source
        $schema = Get-TestResponseSchema -Document $result.Document
        Assert-Equal 2 $result.RequiredTypePropertyCount "Changed schema count was incorrect."
        Assert-Sequence @("type") @($schema.required) "Missing required array was not created."
        Assert-Sequence @("type", "required", "properties") @((Get-JsonProperties -Value $schema).Name) "Required was not inserted before properties."
        Assert-Sequence @("id", "type") @($schema.properties.append.required) "Type was not appended to required."
        Assert-Sequence @("type", "id") @($schema.properties.already.required) "Compliant required names changed."
        Assert-Equal $sourceBefore (ConvertTo-Json -InputObject $source -Compress -Depth 100) "Conversion mutated its source."
    }

    Invoke-Test "schema context transforms nested schemas but ignores literal lookalikes" {
        $candidate = New-TypePropertyCandidate -OmitRequired
        $root = [ordered]@{
            type = "object"
            properties = [ordered]@{
                nested = $candidate
                type = [ordered]@{ type = "object"; properties = [ordered]@{ type = [ordered]@{ type = "string" } } }
            }
            example = $candidate
        }
        $source = New-TestOpenApiDocument -Schema $root
        $source["x-schema-lookalike"] = $candidate

        $result = ConvertTo-OpenApiRequiredTypePropertiesDocument -Document $source
        $schema = Get-TestResponseSchema -Document $result.Document
        Assert-Sequence @("type") @($schema.properties.nested.required) "Nested schema was not transformed."
        Assert-Sequence @("type") @($schema.properties.type.required) "Eligible type property schema was not transformed independently."
        Assert-True (-not (Test-JsonProperty -Object $schema.example -Name "required")) "Schema-like example was transformed."
        Assert-True (-not (Test-JsonProperty -Object $result.Document.'x-schema-lookalike' -Name "required")) "OpenAPI lookalike was transformed."
    }

    Invoke-Test "ineligible schemas and exact casing remain unchanged" {
        $schema = [ordered]@{
            oneOf = @(
                [ordered]@{ properties = [ordered]@{ type = [ordered]@{ type = "string" } } },
                [ordered]@{ type = "string"; properties = [ordered]@{ type = [ordered]@{ type = "string" } } },
                [ordered]@{ type = "object"; properties = [ordered]@{ Type = [ordered]@{ type = "string" } } },
                [ordered]@{ type = "object"; properties = [ordered]@{} }
            )
        }
        $source = New-TestOpenApiDocument -Schema $schema
        $sourceJson = ConvertTo-Json -InputObject $source -Compress -Depth 100
        $result = ConvertTo-OpenApiRequiredTypePropertiesDocument -Document $source
        Assert-Equal 0 $result.RequiredTypePropertyCount "Ineligible schemas were changed."
        Assert-Equal $sourceJson (ConvertTo-Json -InputObject $result.Document -Compress -Depth 100) "Ineligible document changed."
    }

    Invoke-Test "malformed required values fail with breadcrumbs" {
        foreach ($required in @("type", @("id", 42))) {
            $source = New-TestOpenApiDocument -Schema (New-TypePropertyCandidate -Required $required)
            Assert-Throws {
                ConvertTo-OpenApiRequiredTypePropertiesDocument -Document $source
            } "*#/paths/~1test/post/responses/200/content/application~1json/schema/required*"
        }
    }

    Invoke-Test "conversion is structurally idempotent" {
        $first = ConvertTo-OpenApiRequiredTypePropertiesDocument -Document (
            New-TestOpenApiDocument -Schema (New-TypePropertyCandidate -Required @("id"))
        )
        $second = ConvertTo-OpenApiRequiredTypePropertiesDocument -Document $first.Document
        Assert-Equal 0 $second.RequiredTypePropertyCount "Second pass changed a compliant document."
        Assert-True (Test-JsonValuesEqual -Left $first.Document -Right $second.Document) "Second pass changed document structure."
    }

    Invoke-Test "default output naming inserts the marker once" {
        $ordinary = Join-Path $temporaryDirectory "sample.openapi.json"
        $expected = Join-Path $temporaryDirectory "sample.type-properties-required.openapi.json"
        Assert-Equal ([System.IO.Path]::GetFullPath($expected)) (Get-DefaultRequiredTypePropertiesOpenApiPath -InputPath $ordinary) "Default output path was incorrect."
        Assert-Equal ([System.IO.Path]::GetFullPath($expected)) (Get-DefaultRequiredTypePropertiesOpenApiPath -InputPath $expected) "Output marker repeated."
    }

    Invoke-Test "CLI output is deterministic and preserves input" {
        $inputPath = Join-Path $temporaryDirectory "sample.openapi.json"
        $document = New-TestOpenApiDocument -Schema (New-TypePropertyCandidate -Required @("id"))
        $null = Write-OpenApiJsonFile -Document $document -Path $inputPath
        [byte[]]$inputBytes = [System.IO.File]::ReadAllBytes($inputPath)

        $first = Invoke-OpenApiTypePropertyRequirement -InputPath $inputPath 6>$null
        $outputPath = Join-Path $temporaryDirectory "sample.type-properties-required.openapi.json"
        Assert-Equal 1 $first.RequiredTypePropertyCount "CLI changed count was incorrect."
        Assert-BytesEqual $inputBytes ([System.IO.File]::ReadAllBytes($inputPath)) "CLI modified its input."
        [byte[]]$outputBytes = [System.IO.File]::ReadAllBytes($outputPath)
        Assert-Equal 10 $outputBytes[$outputBytes.Length - 1] "Output lacks a trailing LF."
        $hasBom = $outputBytes.Length -ge 3 -and $outputBytes[0] -eq 0xEF -and $outputBytes[1] -eq 0xBB -and $outputBytes[2] -eq 0xBF
        Assert-True (-not $hasBom) "Output contains a UTF-8 BOM."

        $second = Invoke-OpenApiTypePropertyRequirement -InputPath $outputPath 6>$null
        Assert-Equal 0 $second.RequiredTypePropertyCount "Second CLI pass changed a compliant file."
        Assert-BytesEqual $outputBytes ([System.IO.File]::ReadAllBytes($outputPath)) "Second CLI pass changed output bytes."
    }

    Invoke-Test "missing non-file and invalid JSON inputs fail clearly" {
        Assert-Throws {
            Invoke-OpenApiTypePropertyRequirement -InputPath (Join-Path $temporaryDirectory "missing.json")
        } "*cannot find path*"
        Assert-Throws {
            Invoke-OpenApiTypePropertyRequirement -InputPath $temporaryDirectory
        } "*is not a file*"
        $invalidPath = Join-Path $temporaryDirectory "invalid.json"
        [System.IO.File]::WriteAllText($invalidPath, "{ invalid", [System.Text.UTF8Encoding]::new($false))
        Assert-Throws {
            Invoke-OpenApiTypePropertyRequirement -InputPath $invalidPath
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