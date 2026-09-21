<#
.SYNOPSIS
    Runs focused tests for scripts/Sort-OpenApiDocument.ps1.
#>

[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$scriptDirectory = Join-Path $PSScriptRoot ".."
$sortScriptPath = Join-Path $scriptDirectory "Sort-OpenApiDocument.ps1"
$extractScriptPath = Join-Path $scriptDirectory "Extract-OpenApiRoute.ps1"
$flattenScriptPath = Join-Path $scriptDirectory "Flatten-OpenApiAllOf.ps1"
$simplifyScriptPath = Join-Path $scriptDirectory "Simplify-OpenApiStringVariants.ps1"
. $sortScriptPath

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
    param([AllowEmptyCollection()][object[]]$Expected, [AllowEmptyCollection()][object[]]$Actual, [string]$Message)
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
    param([string]$ExpectedPath, [string]$ActualPath, [string]$Message)
    $expected = [System.IO.File]::ReadAllBytes([System.IO.Path]::GetFullPath($ExpectedPath))
    $actual = [System.IO.File]::ReadAllBytes([System.IO.Path]::GetFullPath($ActualPath))
    if (-not [System.Linq.Enumerable]::SequenceEqual($expected, $actual)) {
        throw "$Message Expected SHA-256 '$((Get-FileHash $ExpectedPath).Hash)', got '$((Get-FileHash $ActualPath).Hash)'."
    }
}

function New-TestOpenApiDocument {
    param([object]$Schema)
    return [ordered]@{
        openapi = "3.1.0"
        info = [ordered]@{ title = "Sort fixture"; version = "1.0.0" }
        paths = [ordered]@{
            "/test" = [ordered]@{
                post = [ordered]@{
                    responses = [ordered]@{
                        "200" = [ordered]@{
                            description = "ok"
                            content = [ordered]@{
                                "application/json" = [ordered]@{ schema = $Schema }
                            }
                        }
                    }
                }
            }
        }
    }
}

function Get-TestResponseSchema {
    param([object]$Document)
    return $Document.paths["/test"].post.responses["200"].content["application/json"].schema
}

$repositoryRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot ".." ".."))
$fixtureDirectory = Join-Path $PSScriptRoot "fixtures" "Flatten-OpenApiAllOf"
$temporaryDirectory = Join-Path ([System.IO.Path]::GetTempPath()) "sort-openapi-$([Guid]::NewGuid().ToString('N'))"
$null = [System.IO.Directory]::CreateDirectory($temporaryDirectory)

try {
    Invoke-Test "dot-sourcing defines sorter functions without running the CLI" {
        Assert-True ($null -ne (Get-Command ConvertTo-SortedOpenApiDocument -ErrorAction SilentlyContinue)) "Sort conversion function was not defined."
        Assert-True ($null -ne (Get-Command Invoke-OpenApiDocumentSorter -ErrorAction SilentlyContinue)) "Sort CLI function was not defined."
    }

    Invoke-Test "schema keys properties required names and enums are canonicalized" {
        $source = New-TestOpenApiDocument -Schema ([ordered]@{
            zeta = $true
            properties = [ordered]@{
                optionalZ = [ordered]@{ nullable = $true; description = "Optional"; type = "string" }
                text = [ordered]@{ enum = @("zeta", "alpha", "middle"); type = "string" }
                type = [ordered]@{ type = "string" }
            }
            required = @("text", "type")
            default = $null
            description = "Root"
            type = "object"
        })
        $sorted = ConvertTo-SortedOpenApiDocument -Document $source
        $schema = Get-TestResponseSchema -Document $sorted
        Assert-Sequence @("type", "description", "required", "properties", "default", "zeta") @((Get-JsonProperties $schema).Name) "Schema keys were not canonicalized."
        Assert-Sequence @("type", "text") @($schema.required) "Required names were not canonicalized."
        Assert-Sequence @("type", "text", "optionalZ") @((Get-JsonProperties $schema.properties).Name) "Property names were not canonicalized."
        Assert-Sequence @("alpha", "middle", "zeta") @($schema.properties.text.enum) "Enum values were not canonicalized."
        Assert-Sequence @("type", "description", "nullable") @((Get-JsonProperties $schema.properties.optionalZ).Name) "Nested schema keys were not canonicalized."
    }

    Invoke-Test "enum sorting preserves JSON value types including null" {
        $source = New-TestOpenApiDocument -Schema ([ordered]@{
            type = "object"
            properties = [ordered]@{
                numbers = [ordered]@{ enum = @(2, 10, 1) }
                mixed = [ordered]@{ enum = @($null, "incomplete") }
                flag = [ordered]@{ enum = @($true) }
            }
        })
        $schema = Get-TestResponseSchema -Document (ConvertTo-SortedOpenApiDocument $source)
        Assert-Sequence @(1, 10, 2) @($schema.properties.numbers.enum) "Numeric enum order was incorrect."
        Assert-Equal "incomplete" $schema.properties.mixed.enum[0] "Mixed enum string order was incorrect."
        Assert-True ($null -eq $schema.properties.mixed.enum[1]) "Mixed enum null order was incorrect."
        Assert-Equal $true $schema.properties.flag.enum[0] "Boolean enum value changed."
    }

    Invoke-Test "non-schema objects non-enum arrays and allOf branch order are preserved" {
        $source = New-TestOpenApiDocument -Schema ([ordered]@{
            allOf = @(
                [ordered]@{ type = "string"; description = "first" },
                [ordered]@{ type = "string"; description = "second" }
            )
            customArray = @("zeta", "alpha")
        })
        $source.paths["/test"].post["custom"] = [ordered]@{
            required = $true
            type = "ordinary"
            description = "ordinary"
        }
        $sorted = ConvertTo-SortedOpenApiDocument $source
        $schema = Get-TestResponseSchema $sorted
        Assert-Equal "first" $schema.allOf[0].description "allOf branch order changed."
        Assert-Equal "second" $schema.allOf[1].description "allOf branch order changed."
        Assert-Sequence @("zeta", "alpha") @($schema.customArray) "Non-enum array order changed."
        Assert-Sequence @("responses", "custom") @((Get-JsonProperties $sorted.paths["/test"].post).Name) "Ordinary OpenAPI object order changed."
    }

    Invoke-Test "sort preserves configured and extension properties" {
        $source = New-TestOpenApiDocument -Schema ([ordered]@{
            type = "string"
            example = "kept"
            examples = @("also kept")
            "x-custom" = "kept"
        })
        $schema = Get-TestResponseSchema -Document (ConvertTo-SortedOpenApiDocument $source)
        Assert-Equal "kept" $schema.example "Sort omitted example."
        Assert-Equal "also kept" $schema.examples[0] "Sort omitted examples."
        Assert-Equal "kept" $schema."x-custom" "Sort omitted extension."
    }

    Invoke-Test "schema and required priority arrays are configurable" {
        $originalSchemaPriority = @($script:PrioritizedSchemaPropertyNames)
        $originalRequiredPriority = @($script:PrioritizedRequiredPropertyNames)
        try {
            $script:PrioritizedSchemaPropertyNames = @("description", "properties", "type")
            $script:PrioritizedRequiredPropertyNames = @("kind", "type", "kind")
            $source = New-TestOpenApiDocument -Schema ([ordered]@{
                required = @("text", "type", "kind")
                type = "object"
                properties = [ordered]@{
                    text = [ordered]@{ type = "string" }
                    type = [ordered]@{ type = "string" }
                    optional = [ordered]@{ type = "string" }
                    kind = [ordered]@{ type = "string" }
                }
                description = "configured"
            })
            $schema = Get-TestResponseSchema -Document (ConvertTo-SortedOpenApiDocument $source)
            Assert-Sequence @("description", "properties", "type", "required") @((Get-JsonProperties $schema).Name) "Schema priority configuration was ignored."
            Assert-Sequence @("kind", "type", "text") @($schema.required) "Required priority configuration was ignored."
            Assert-Sequence @("kind", "type", "text", "optional") @((Get-JsonProperties $schema.properties).Name) "Property priority configuration was ignored."
        }
        finally {
            $script:PrioritizedSchemaPropertyNames = $originalSchemaPriority
            $script:PrioritizedRequiredPropertyNames = $originalRequiredPriority
        }
    }

    Invoke-Test "non-string required names are rejected" {
        $source = New-TestOpenApiDocument -Schema ([ordered]@{
            type = "object"
            required = @("valid", 42)
            properties = [ordered]@{ valid = [ordered]@{ type = "string" } }
        })
        Assert-Throws { ConvertTo-SortedOpenApiDocument -Document $source } "*Every required name must be a string*"
    }

    Invoke-Test "compact four-stage pipeline matches the current golden bytes" {
        $sourcePath = Join-Path $fixtureDirectory "source.openapi.json"
        $goldenPath = Join-Path $fixtureDirectory "expected.flattened.openapi.json"
        $inlinedPath = Join-Path $temporaryDirectory "things.inlined.openapi.json"
        $flattenedPath = Join-Path $temporaryDirectory "things.inlined.flattened.openapi.json"
        $simplifiedPath = Join-Path $temporaryDirectory "things.inlined.flattened.simplified.openapi.json"
        $sortedPath = Join-Path $temporaryDirectory "things.inlined.flattened.simplified.sorted.openapi.json"
        & $extractScriptPath -SpecificationPath $sourcePath -OperationId "createThing" -OutputPath $inlinedPath
        & $flattenScriptPath -InputPath $inlinedPath -OutputPath $flattenedPath
        & $simplifyScriptPath -InputPath $flattenedPath -OutputPath $simplifiedPath
        & $sortScriptPath -InputPath $simplifiedPath -OutputPath $sortedPath
        Assert-BytesEqual -ExpectedPath $goldenPath -ActualPath $sortedPath -Message "Four-stage output changed."
    }

    Invoke-Test "Sort supports unflattened extracted output" {
        $sourcePath = Join-Path $fixtureDirectory "source.openapi.json"
        $inputPath = Join-Path $temporaryDirectory "things.inlined.openapi.json"
        $outputPath = Join-Path $temporaryDirectory "things.inlined.sorted.openapi.json"
        & $extractScriptPath -SpecificationPath $sourcePath -OperationId "createThing" -OutputPath $inputPath
        & $sortScriptPath -InputPath $inputPath -OutputPath $outputPath
        $sorted = Get-Content -LiteralPath $outputPath -Raw | ConvertFrom-Json -Depth 100
        Assert-OpenApiReferencesResolve -Document $sorted
        Assert-True ([System.IO.File]::Exists($outputPath)) "Sort did not support extracted output."
        $preservedAllOf = $sorted.paths.'/things'.post.requestBody.content.'application/json'.schema.properties.flattened.allOf
        Assert-True ($null -ne $preservedAllOf) "Sort flattened an allOf composition."
    }

    Invoke-Test "default naming standalone output encoding and input preservation are correct" {
        $inputPath = Join-Path $temporaryDirectory "sample.inlined.flattened.openapi.json"
        Copy-Item -LiteralPath (Join-Path $fixtureDirectory "expected.flattened.openapi.json") -Destination $inputPath
        [byte[]]$before = [System.IO.File]::ReadAllBytes($inputPath)
        & $sortScriptPath -InputPath $inputPath
        $outputPath = Join-Path $temporaryDirectory "sample.inlined.flattened.sorted.openapi.json"
        Assert-True ([System.IO.File]::Exists($outputPath)) "Default sorted output was not created."
        Assert-True ([System.Linq.Enumerable]::SequenceEqual($before, [System.IO.File]::ReadAllBytes($inputPath))) "Sort modified its input."
        [byte[]]$outputBytes = [System.IO.File]::ReadAllBytes($outputPath)
        $hasBom = $outputBytes.Length -ge 3 -and $outputBytes[0] -eq 0xEF -and $outputBytes[1] -eq 0xBB -and $outputBytes[2] -eq 0xBF
        Assert-True (-not $hasBom) "Sorted output contains a BOM."
        Assert-Equal 10 $outputBytes[$outputBytes.Length - 1] "Sorted output lacks a trailing LF."
    }

    Invoke-Test "sorting is byte-idempotent and avoids repeated suffixes" {
        $inputPath = Join-Path $temporaryDirectory "already.sorted.openapi.json"
        Copy-Item -LiteralPath (Join-Path $fixtureDirectory "expected.flattened.openapi.json") -Destination $inputPath
        [byte[]]$before = [System.IO.File]::ReadAllBytes($inputPath)
        Assert-Equal ([System.IO.Path]::GetFullPath($inputPath)) (Get-DefaultSortedOpenApiPath $inputPath) "Already-sorted default path changed."
        $null = Invoke-OpenApiDocumentSorter -InputPath $inputPath
        Assert-True ([System.Linq.Enumerable]::SequenceEqual($before, [System.IO.File]::ReadAllBytes($inputPath))) "Sorting was not byte-idempotent."
    }

    Invoke-Test "missing and invalid inputs fail clearly" {
        Assert-Throws { Invoke-OpenApiDocumentSorter -InputPath (Join-Path $temporaryDirectory "missing.json") } "*cannot find path*"
        $invalidPath = Join-Path $temporaryDirectory "invalid.json"
        [System.IO.File]::WriteAllText($invalidPath, "{ invalid", [System.Text.UTF8Encoding]::new($false))
        Assert-Throws { Invoke-OpenApiDocumentSorter -InputPath $invalidPath } "*Could not parse OpenAPI JSON*"
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
