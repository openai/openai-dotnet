<#
.SYNOPSIS
    Runs focused tests for scripts/Normalize-OpenApiOneOf.ps1.
#>

[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$scriptDirectory = Join-Path $PSScriptRoot ".."
$normalizeScriptPath = Join-Path $scriptDirectory "Normalize-OpenApiOneOf.ps1"
$sortScriptPath = Join-Path $scriptDirectory "Sort-OpenApiDocument.ps1"
. $normalizeScriptPath

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

function New-TestOpenApiDocument {
    param([object]$Schema)
    return [ordered]@{
        openapi = "3.1.0"
        info = [ordered]@{ title = "oneOf fixture"; version = "1.0.0" }
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

$fixtureDirectory = Join-Path $PSScriptRoot "fixtures" "Normalize-OpenApiOneOf"
$temporaryDirectory = Join-Path ([System.IO.Path]::GetTempPath()) "normalize-openapi-oneof-$([Guid]::NewGuid().ToString('N'))"
$null = [System.IO.Directory]::CreateDirectory($temporaryDirectory)

try {
    Invoke-Test "dot-sourcing defines normalizer functions without running the CLI" {
        Assert-True ($null -ne (Get-Command ConvertTo-NormalizedOpenApiOneOfDocument -ErrorAction SilentlyContinue)) "Conversion function was not defined."
        Assert-True ($null -ne (Get-Command Invoke-OpenApiOneOfNormalizer -ErrorAction SilentlyContinue)) "CLI function was not defined."
    }

    Invoke-Test "line 7132 shape removes only the outer orphan" {
        $source = Read-OpenApiDocument -Path (Join-Path $fixtureDirectory "source.openapi.json")
        $expected = Read-OpenApiDocument -Path (Join-Path $fixtureDirectory "expected.oneof-normalized.openapi.json")
        $sourceBefore = ConvertTo-Json -InputObject $source -Compress -Depth 100
        $result = ConvertTo-NormalizedOpenApiOneOfDocument -Document $source
        Assert-Equal 1 $result.RemovedDiscriminatorCount "Unexpected removal count."
        Assert-JsonEqual $expected $result.Document "Normalized fixture did not match its golden."
        Assert-Equal $sourceBefore (ConvertTo-Json -InputObject $source -Compress -Depth 100) "Conversion mutated its input."

        $outputs = (Get-TestResponseSchema -Document $result.Document).properties.outputs
        Assert-True (-not (Test-JsonProperty -Object $outputs -Name "discriminator")) "Outer orphan remained."
        Assert-True (Test-JsonProperty -Object $outputs.items -Name "discriminator") "Nested paired discriminator was removed."
    }

    Invoke-Test "any exact direct oneOf sibling preserves discriminator" {
        $source = New-TestOpenApiDocument -Schema ([ordered]@{
            type = "object"
            properties = [ordered]@{
                nonEmpty = [ordered]@{
                    discriminator = [ordered]@{ propertyName = "kind" }
                    oneOf = @([ordered]@{ type = "object" })
                }
                empty = [ordered]@{
                    discriminator = [ordered]@{ propertyName = "kind" }
                    oneOf = @()
                }
                nullValue = [ordered]@{
                    discriminator = [ordered]@{ propertyName = "kind" }
                    oneOf = $null
                }
                malformed = [ordered]@{
                    discriminator = "malformed"
                    oneOf = "not an array"
                }
            }
        })
        $result = ConvertTo-NormalizedOpenApiOneOfDocument -Document $source
        Assert-Equal 0 $result.RemovedDiscriminatorCount "A direct pair was removed."
        foreach ($property in (Get-JsonProperties -Value (Get-TestResponseSchema $result.Document).properties)) {
            Assert-True (Test-JsonProperty -Object $property.Value -Name "discriminator") "Paired discriminator '$($property.Name)' was removed."
        }
    }

    Invoke-Test "property-map key is preserved while its schema is traversed" {
        $source = New-TestOpenApiDocument -Schema ([ordered]@{
            type = "object"
            properties = [ordered]@{
                discriminator = [ordered]@{
                    type = "object"
                    discriminator = [ordered]@{ propertyName = "kind" }
                    properties = [ordered]@{ value = [ordered]@{ type = "string" } }
                }
            }
        })
        $result = ConvertTo-NormalizedOpenApiOneOfDocument -Document $source
        $properties = (Get-TestResponseSchema $result.Document).properties
        Assert-Equal 1 $result.RemovedDiscriminatorCount "Nested schema keyword was not removed."
        Assert-True (Test-JsonProperty -Object $properties -Name "discriminator") "Property-map key was removed."
        Assert-True (-not (Test-JsonProperty -Object $properties.discriminator -Name "discriminator")) "Orphan inside the property schema remained."
    }

    Invoke-Test "literal values ordinary objects and case variants remain unchanged" {
        $source = New-TestOpenApiDocument -Schema ([ordered]@{
            type = "string"
            description = "mentions discriminator"
            enum = @("discriminator")
            Discriminator = [ordered]@{ propertyName = "kind" }
            example = [ordered]@{ discriminator = "literal member" }
        })
        $source.info["discriminator"] = "ordinary OpenAPI member"
        $before = ConvertTo-Json -InputObject $source -Compress -Depth 100
        $result = ConvertTo-NormalizedOpenApiOneOfDocument -Document $source
        Assert-Equal 0 $result.RemovedDiscriminatorCount "A protected occurrence was removed."
        Assert-Equal $before (ConvertTo-Json -InputObject $result.Document -Compress -Depth 100) "A protected occurrence changed."
    }

    Invoke-Test "nested schema orphans are removed independently in encounter order" {
        $source = New-TestOpenApiDocument -Schema ([ordered]@{
            type = "object"
            properties = [ordered]@{
                first = [ordered]@{ type = "string"; discriminator = $null; description = "first" }
                second = [ordered]@{
                    anyOf = @(
                        [ordered]@{ type = "string"; discriminator = "remove" },
                        [ordered]@{ type = "null" }
                    )
                }
                third = [ordered]@{
                    type = "array"
                    items = [ordered]@{ type = "string"; discriminator = [ordered]@{} }
                }
            }
        })
        $result = ConvertTo-NormalizedOpenApiOneOfDocument -Document $source
        $properties = (Get-TestResponseSchema $result.Document).properties
        Assert-Equal 3 $result.RemovedDiscriminatorCount "Nested orphan count was incorrect."
        Assert-Sequence @("type", "description") @((Get-JsonProperties $properties.first).Name) "First schema order changed."
        Assert-True (-not (Test-JsonProperty $properties.second.anyOf[0] "discriminator")) "Composition orphan remained."
        Assert-True (-not (Test-JsonProperty $properties.third.items "discriminator")) "Items orphan remained."
    }

    Invoke-Test "postcondition rejects an orphaned schema discriminator" {
        $source = New-TestOpenApiDocument -Schema ([ordered]@{
            type = "string"
            discriminator = [ordered]@{ propertyName = "kind" }
        })
        Assert-Throws {
            Assert-NoOrphanedOpenApiDiscriminators -Value $source
        } "*retained an orphaned discriminator*"
    }

    Invoke-Test "references to paired discriminators remain valid" {
        $targetPointer = "#/paths/~1test/post/responses/200/content/application~1json/schema/properties/target/discriminator"
        $source = New-TestOpenApiDocument -Schema ([ordered]@{
            type = "object"
            properties = [ordered]@{
                target = [ordered]@{
                    discriminator = [ordered]@{ propertyName = "kind" }
                    oneOf = @([ordered]@{ type = "object" })
                }
                alias = [ordered]@{ '$ref' = $targetPointer }
            }
        })
        $result = ConvertTo-NormalizedOpenApiOneOfDocument -Document $source
        Assert-Equal 0 $result.RemovedDiscriminatorCount "Paired target was removed."
        Assert-Equal $targetPointer (Get-TestResponseSchema $result.Document).properties.alias.'$ref' "Retained reference changed."
    }

    Invoke-Test "references into removed discriminator subtrees fail clearly" {
        $targetPointer = "#/paths/~1test/post/responses/200/content/application~1json/schema/properties/target/discriminator"
        foreach ($referenceName in @('$ref', '$dynamicRef', '$recursiveRef')) {
            $alias = [ordered]@{}
            $alias[$referenceName] = $targetPointer
            $source = New-TestOpenApiDocument -Schema ([ordered]@{
                type = "object"
                properties = [ordered]@{
                    target = [ordered]@{ type = "string"; discriminator = [ordered]@{ propertyName = "kind" } }
                    alias = $alias
                }
            })
            Assert-Throws {
                ConvertTo-NormalizedOpenApiOneOfDocument -Document $source
            } "*has no 'discriminator' property*"
        }
    }

    Invoke-Test "normalization preserves order and Sort canonicalizes afterward" {
        $source = Read-OpenApiDocument -Path (Join-Path $fixtureDirectory "source.openapi.json")
        $normalized = (ConvertTo-NormalizedOpenApiOneOfDocument -Document $source).Document
        $normalizedProperties = (Get-TestResponseSchema $normalized).properties
        Assert-Sequence `
            @("outputs", "discriminator", "emptyPair", "malformedPair", "caseVariant") `
            @((Get-JsonProperties $normalizedProperties).Name) `
            "Normalizer reordered properties."

        . $sortScriptPath
        $sorted = ConvertTo-SortedOpenApiDocument -Document $normalized
        Assert-Sequence `
            @("caseVariant", "discriminator", "emptyPair", "malformedPair", "outputs") `
            @((Get-JsonProperties (Get-TestResponseSchema $sorted).properties).Name) `
            "Sort did not canonicalize normalized properties."
    }

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

    Invoke-Test "normalization is byte-idempotent and avoids repeated suffixes" {
        $inputPath = Join-Path $temporaryDirectory "already.oneof-normalized.openapi.json"
        Copy-Item -LiteralPath (Join-Path $fixtureDirectory "expected.oneof-normalized.openapi.json") -Destination $inputPath
        Assert-Equal ([System.IO.Path]::GetFullPath($inputPath)) (Get-DefaultNormalizedOpenApiOneOfPath $inputPath) "Already-normalized default path changed."
        $null = Invoke-OpenApiOneOfNormalizer -InputPath $inputPath
        [byte[]]$firstRun = [System.IO.File]::ReadAllBytes($inputPath)
        $null = Invoke-OpenApiOneOfNormalizer -InputPath $inputPath
        Assert-BytesEqual $firstRun ([System.IO.File]::ReadAllBytes($inputPath)) "Normalization was not byte-idempotent."
    }

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