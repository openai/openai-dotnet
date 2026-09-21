<#
.SYNOPSIS
    Runs focused tests for scripts/Simplify-OpenApiStringVariants.ps1.
#>

[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$scriptDirectory = Join-Path $PSScriptRoot ".."
$simplifyScriptPath = Join-Path $scriptDirectory "Simplify-OpenApiStringVariants.ps1"
$sortScriptPath = Join-Path $scriptDirectory "Sort-OpenApiDocument.ps1"
. $simplifyScriptPath

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

function New-TestOpenApiDocument {
    param([object]$Schema)
    return [ordered]@{
        openapi = "3.1.0"
        info = [ordered]@{ title = "String variant fixture"; version = "1.0.0" }
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

function New-SelectedShapeSchema {
    return [ordered]@{
        description = "Keep wrapper"
        anyOf = @(
            [ordered]@{
                description = "Drop nested wrapper"
                anyOf = @(
                    [ordered]@{ type = "string"; description = "Drop plain annotation" },
                    [ordered]@{ type = "string"; enum = @("zeta", "alpha", "shared"); default = "alpha" }
                )
            },
            [ordered]@{ type = "string"; enum = @("shared", "middle"); title = "Drop title" }
        )
    }
}

$fixtureDirectory = Join-Path $PSScriptRoot "fixtures" "Simplify-OpenApiStringVariants"
$temporaryDirectory = Join-Path ([System.IO.Path]::GetTempPath()) "simplify-openapi-$([Guid]::NewGuid().ToString('N'))"
$null = [System.IO.Directory]::CreateDirectory($temporaryDirectory)

try {
    Invoke-Test "dot-sourcing defines simplifier functions without running the CLI" {
        Assert-True ($null -ne (Get-Command ConvertTo-SimplifiedOpenApiStringVariantDocument -ErrorAction SilentlyContinue)) "Conversion function was not defined."
        Assert-True ($null -ne (Get-Command Invoke-OpenApiStringVariantSimplifier -ErrorAction SilentlyContinue)) "CLI function was not defined."
    }

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

    Invoke-Test "branch category order follows the first recursive leaf" {
        $source = New-TestOpenApiDocument -Schema ([ordered]@{
            anyOf = @(
                [ordered]@{ type = "string"; enum = @("first") },
                [ordered]@{
                    anyOf = @(
                        [ordered]@{ type = "string" },
                        [ordered]@{ type = "string"; enum = @("second") }
                    )
                }
            )
        })
        $schema = Get-TestResponseSchema -Document (
            ConvertTo-SimplifiedOpenApiStringVariantDocument -Document $source
        ).Document
        Assert-True (Test-JsonProperty -Object $schema.anyOf[0] -Name "enum") "Enum branch did not remain first."
        Assert-True (-not (Test-JsonProperty -Object $schema.anyOf[1] -Name "enum")) "Plain branch did not remain second."
        Assert-Sequence @("first", "second") @($schema.anyOf[0].enum) "Enum values changed order."
    }

    Invoke-Test "arbitrary nesting all-enum and all-plain trees retain anyOf" {
        $source = New-TestOpenApiDocument -Schema ([ordered]@{
            type = "object"
            properties = [ordered]@{
                enumOnly = [ordered]@{
                    anyOf = @(
                        [ordered]@{ anyOf = @([ordered]@{ anyOf = @(
                            [ordered]@{ type = "string"; enum = @("a") },
                            [ordered]@{ type = "string"; enum = @("b") }
                        ) }) }
                    )
                }
                plainOnly = [ordered]@{
                    anyOf = @(
                        [ordered]@{ type = "string" },
                        [ordered]@{ anyOf = @(
                            [ordered]@{ type = "string" },
                            [ordered]@{ type = "string" }
                        ) }
                    )
                }
            }
        })
        $result = ConvertTo-SimplifiedOpenApiStringVariantDocument -Document $source
        $properties = (Get-TestResponseSchema -Document $result.Document).properties
        Assert-Equal 2 $result.SimplifiedCount "Deep candidates were not both simplified."
        Assert-Equal 1 $properties.enumOnly.anyOf.Count "All-enum anyOf was removed."
        Assert-Sequence @("a", "b") @($properties.enumOnly.anyOf[0].enum) "All-enum values were incorrect."
        Assert-Equal 1 $properties.plainOnly.anyOf.Count "All-plain anyOf was removed."
        Assert-Sequence @("type") @((Get-JsonProperties $properties.plainOnly.anyOf[0]).Name) "All-plain branch was incorrect."
    }

    Invoke-Test "mixed unions preserve directly connected string subtrees" {
        foreach ($otherBranch in @(
            [ordered]@{ type = "null" },
            [ordered]@{ type = "object" },
            [ordered]@{ type = "number" }
        )) {
            $source = New-TestOpenApiDocument -Schema ([ordered]@{
                anyOf = @(
                    [ordered]@{ anyOf = @(
                        [ordered]@{ type = "string" },
                        [ordered]@{ type = "string"; enum = @("known") }
                    ) },
                    $otherBranch
                )
            })
            $result = ConvertTo-SimplifiedOpenApiStringVariantDocument -Document $source
            $schema = Get-TestResponseSchema -Document $result.Document
            Assert-Equal 0 $result.SimplifiedCount "A mixed union was simplified."
            Assert-Equal 2 $schema.anyOf[0].anyOf.Count "A mixed union's string subtree changed."
        }
    }

    Invoke-Test "independent child schemas inside mixed branches remain eligible" {
        $source = New-TestOpenApiDocument -Schema ([ordered]@{
            anyOf = @(
                [ordered]@{
                    type = "object"
                    properties = [ordered]@{ model = New-SelectedShapeSchema }
                },
                [ordered]@{ type = "null" }
            )
        })
        $result = ConvertTo-SimplifiedOpenApiStringVariantDocument -Document $source
        $model = (Get-TestResponseSchema -Document $result.Document).anyOf[0].properties.model
        Assert-Equal 1 $result.SimplifiedCount "Independent child candidate was not simplified."
        Assert-Equal 2 $model.anyOf.Count "Independent child output was incorrect."
    }

    Invoke-Test "unsupported constraints and invalid enums make complete trees ineligible" {
        $unsupportedLeaves = @(
            [ordered]@{ type = "string"; format = "uri" },
            [ordered]@{ type = "string"; pattern = "x" },
            [ordered]@{ type = "string"; minLength = 1 },
            [ordered]@{ type = "string"; maxLength = 2 },
            [ordered]@{ type = "string"; const = "x" },
            [ordered]@{ type = "string"; enum = @() },
            [ordered]@{ type = "string"; enum = @("x", 1) },
            [ordered]@{ '$ref' = "#/paths" }
        )
        foreach ($unsupportedLeaf in $unsupportedLeaves) {
            $schema = [ordered]@{
                anyOf = @(
                    [ordered]@{ anyOf = @([ordered]@{ type = "string" }, $unsupportedLeaf) },
                    [ordered]@{ type = "string"; enum = @("known") }
                )
            }
            $source = New-TestOpenApiDocument -Schema $schema
            $before = ConvertTo-Json -InputObject $source -Compress -Depth 100
            $result = ConvertTo-SimplifiedOpenApiStringVariantDocument -Document $source
            Assert-Equal 0 $result.SimplifiedCount "An unsupported string tree was simplified."
            Assert-Equal $before (ConvertTo-Json -InputObject $result.Document -Compress -Depth 100) "An unsupported string tree changed."
        }
    }

    Invoke-Test "already-flat unions and literal anyOf objects remain unchanged" {
        $schema = [ordered]@{
            anyOf = @(
                [ordered]@{ type = "string" },
                [ordered]@{ type = "string"; enum = @("known") }
            )
            example = [ordered]@{
                anyOf = @(
                    [ordered]@{ anyOf = @(
                        [ordered]@{ type = "string" },
                        [ordered]@{ type = "string"; enum = @("literal") }
                    ) }
                )
            }
        }
        $source = New-TestOpenApiDocument -Schema $schema
        $before = ConvertTo-Json -InputObject $source -Compress -Depth 100
        $result = ConvertTo-SimplifiedOpenApiStringVariantDocument -Document $source
        Assert-Equal 0 $result.SimplifiedCount "Flat or literal anyOf was simplified."
        Assert-Equal $before (ConvertTo-Json -InputObject $result.Document -Compress -Depth 100) "Flat or literal anyOf changed."
    }

    Invoke-Test "postcondition rejects an eligible nested string union" {
        $source = New-TestOpenApiDocument -Schema (New-SelectedShapeSchema)
        Assert-Throws {
            Assert-NoSimplifiableOpenApiStringVariants -Value $source
        } "*retained an eligible nested string anyOf*"
    }

    Invoke-Test "references to candidate roots remain valid" {
        $candidatePointer = "#/paths/~1test/post/responses/200/content/application~1json/schema/properties/model"
        $source = New-TestOpenApiDocument -Schema ([ordered]@{
            type = "object"
            properties = [ordered]@{
                model = New-SelectedShapeSchema
                alias = [ordered]@{ '$ref' = $candidatePointer }
            }
        })
        $result = ConvertTo-SimplifiedOpenApiStringVariantDocument -Document $source
        Assert-Equal 1 $result.SimplifiedCount "Root-targeted candidate was not simplified."
        Assert-Equal $candidatePointer (Get-TestResponseSchema -Document $result.Document).properties.alias.'$ref' "Root reference changed."
    }

    Invoke-Test "references into disappearing candidate branches fail before rewriting" {
        $candidatePointer = "#/paths/~1test/post/responses/200/content/application~1json/schema/properties/model"
        foreach ($referenceProperty in @('$ref', '$dynamicRef', '$recursiveRef')) {
            $alias = [ordered]@{}
            $alias[$referenceProperty] = "$candidatePointer/anyOf/0/anyOf/0"
            $source = New-TestOpenApiDocument -Schema ([ordered]@{
                type = "object"
                properties = [ordered]@{
                    model = New-SelectedShapeSchema
                    alias = $alias
                }
            })
            Assert-Throws {
                ConvertTo-SimplifiedOpenApiStringVariantDocument -Document $source
            } "*targets*inside the anyOf branches*"
        }

        $sourceWithMapping = New-TestOpenApiDocument -Schema ([ordered]@{
            type = "object"
            properties = [ordered]@{ model = New-SelectedShapeSchema }
            discriminator = [ordered]@{
                propertyName = "type"
                mapping = [ordered]@{
                    model = "$candidatePointer/anyOf/1"
                }
            }
        })
        Assert-Throws {
            ConvertTo-SimplifiedOpenApiStringVariantDocument -Document $sourceWithMapping
        } "*DiscriminatorMapping*targets*inside the anyOf branches*"
    }

    Invoke-Test "unresolved references and allOf input fail clearly" {
        $unresolved = New-TestOpenApiDocument -Schema ([ordered]@{ '$ref' = "#/components/schemas/Missing" })
        Assert-Throws {
            ConvertTo-SimplifiedOpenApiStringVariantDocument -Document $unresolved
        } "*Could not resolve Reference*"

        $unflattened = New-TestOpenApiDocument -Schema ([ordered]@{
            allOf = @([ordered]@{ type = "string" })
        })
        Assert-Throws {
            ConvertTo-SimplifiedOpenApiStringVariantDocument -Document $unflattened
        } "*requires allOf-flattened input*Flatten-OpenApiAllOf.ps1 first*"
    }

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

    Invoke-Test "simplification is byte-idempotent and avoids repeated suffixes" {
        $inputPath = Join-Path $temporaryDirectory "already.simplified.openapi.json"
        Copy-Item -LiteralPath (Join-Path $fixtureDirectory "expected.simplified.openapi.json") -Destination $inputPath
        Assert-Equal ([System.IO.Path]::GetFullPath($inputPath)) (Get-DefaultSimplifiedOpenApiPath $inputPath) "Already-simplified default path changed."
        $null = Invoke-OpenApiStringVariantSimplifier -InputPath $inputPath
        [byte[]]$firstRun = [System.IO.File]::ReadAllBytes($inputPath)
        $null = Invoke-OpenApiStringVariantSimplifier -InputPath $inputPath
        Assert-BytesEqual $firstRun ([System.IO.File]::ReadAllBytes($inputPath)) "Simplification was not byte-idempotent."
    }

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