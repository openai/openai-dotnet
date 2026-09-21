<#
.SYNOPSIS
    Runs focused tests for scripts/Flatten-OpenApiAllOf.ps1.

.DESCRIPTION
    Uses the repository's dependency-free assertion style to validate allOf
    flattening, pointer relocation, deterministic file output, idempotence, and
    byte-for-byte compatibility with the pre-split extractor behavior.
#>

[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$scriptDirectory = Join-Path $PSScriptRoot ".."
$flattenScriptPath = Join-Path $scriptDirectory "Flatten-OpenApiAllOf.ps1"
$extractScriptPath = Join-Path $scriptDirectory "Extract-OpenApiRoute.ps1"
$simplifyScriptPath = Join-Path $scriptDirectory "Simplify-OpenApiStringVariants.ps1"
$sortScriptPath = Join-Path $scriptDirectory "Sort-OpenApiDocument.ps1"
. $flattenScriptPath

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

function New-TestOpenApiDocument {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Schema
    )

    return [ordered]@{
        openapi = "3.1.0"
        info = [ordered]@{ title = "Flatten fixture"; version = "1.0.0" }
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
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document
    )

    return $Document.paths["/test"].post.responses["200"].content["application/json"].schema
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

function Find-OpenApiAllOf {
    param(
        [Parameter(Mandatory = $true)]
        [AllowNull()]
        [object]$Value
    )

    if ($null -eq $Value) {
        return
    }
    if (Test-JsonArrayValue -Value $Value) {
        foreach ($item in $Value) {
            Find-OpenApiAllOf -Value $item
        }
        return
    }
    if (-not (Test-JsonObjectValue -Value $Value)) {
        return
    }

    foreach ($property in (Get-JsonProperties -Value $Value)) {
        if ([string]::Equals($property.Name, "allOf", [System.StringComparison]::Ordinal)) {
            $property.Value
        }
        Find-OpenApiAllOf -Value $property.Value
    }
}

$repositoryRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot ".." ".."))
$fixtureDirectory = Join-Path $PSScriptRoot "fixtures" "Flatten-OpenApiAllOf"
$temporaryDirectory = Join-Path ([System.IO.Path]::GetTempPath()) "flatten-openapi-allof-$([Guid]::NewGuid().ToString('N'))"
$null = [System.IO.Directory]::CreateDirectory($temporaryDirectory)

try {
    Invoke-Test "dot-sourcing defines functions without executing the CLI" {
        Assert-True ($null -ne (Get-Command ConvertTo-FlattenedOpenApiDocument -ErrorAction SilentlyContinue)) "Conversion function was not defined."
        Assert-True ($null -ne (Get-Command Invoke-OpenApiAllOfFlattener -ErrorAction SilentlyContinue)) "CLI function was not defined."
    }

    Invoke-Test "simple and nested allOf schemas are flattened" {
        $schema = [ordered]@{
            description = "Outer description"
            allOf = @(
                [ordered]@{
                    type = "object"
                    properties = [ordered]@{
                        text = [ordered]@{ type = "string" }
                        type = [ordered]@{ type = "string" }
                    }
                    required = @("text", "type")
                },
                [ordered]@{
                    allOf = @(
                        [ordered]@{
                            type = "object"
                            properties = [ordered]@{ optional = [ordered]@{ type = "boolean" } }
                        },
                        [ordered]@{ type = "object"; nullable = $true }
                    )
                }
            )
        }
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

    Invoke-Test "compatible duplicate properties and enums are merged" {
        $source = New-TestOpenApiDocument -Schema ([ordered]@{
            allOf = @(
                [ordered]@{
                    type = "object"
                    properties = [ordered]@{
                        status = [ordered]@{ type = "string"; enum = @("in_progress", "completed", "incomplete") }
                    }
                },
                [ordered]@{
                    type = "object"
                    properties = [ordered]@{
                        status = [ordered]@{ enum = @("completed", "incomplete", "failed") }
                    }
                }
            )
        })
        $result = ConvertTo-FlattenedOpenApiDocument -Document $source
        $status = (Get-TestResponseSchema -Document $result.Document).properties.status
        Assert-Sequence @("completed", "incomplete") @($status.enum) "Enum intersection was incorrect."
    }

    Invoke-Test "incompatible property definitions fail with a breadcrumb" {
        $source = New-TestOpenApiDocument -Schema ([ordered]@{
            allOf = @(
                [ordered]@{ properties = [ordered]@{ duplicate = [ordered]@{ type = "string" } } },
                [ordered]@{ properties = [ordered]@{ duplicate = [ordered]@{ type = "integer" } } }
            )
        })
        Assert-Throws { ConvertTo-FlattenedOpenApiDocument -Document $source } "*duplicate*"
    }

    Invoke-Test "incompatible validating keywords fail" {
        $source = New-TestOpenApiDocument -Schema ([ordered]@{
            allOf = @(
                [ordered]@{ type = "object" },
                [ordered]@{ type = "array" }
            )
        })
        Assert-Throws { ConvertTo-FlattenedOpenApiDocument -Document $source } "*keyword 'type'*"
    }

    Invoke-Test "disjoint enum constraints fail" {
        $source = New-TestOpenApiDocument -Schema ([ordered]@{
            allOf = @(
                [ordered]@{ enum = @("completed") },
                [ordered]@{ enum = @("failed") }
            )
        })
        Assert-Throws { ConvertTo-FlattenedOpenApiDocument -Document $source } "*no common values*"
    }

    Invoke-Test "empty and non-object allOf branches fail" {
        Assert-Throws {
            ConvertTo-FlattenedOpenApiDocument -Document (
                New-TestOpenApiDocument -Schema ([ordered]@{ allOf = @() })
            )
        } "*must contain at least one*"
        Assert-Throws {
            ConvertTo-FlattenedOpenApiDocument -Document (
                New-TestOpenApiDocument -Schema ([ordered]@{ allOf = @("invalid") })
            )
        } "*must be an object definition*"
    }

    Invoke-Test "local pointers under allOf branches are relocated" {
        $source = New-TestOpenApiDocument -Schema ([ordered]@{
            allOf = @(
                [ordered]@{
                    type = "object"
                    properties = [ordered]@{
                        kind = [ordered]@{ type = "string"; enum = @("a") }
                        child = [ordered]@{ '$ref' = "#/paths/~1test/post/responses/200/content/application~1json/schema/allOf/0/properties/kind" }
                    }
                },
                [ordered]@{ type = "object" }
            )
        })
        $result = ConvertTo-FlattenedOpenApiDocument -Document $source
        $flattened = Get-TestResponseSchema -Document $result.Document
        Assert-Equal `
            "#/paths/~1test/post/responses/200/content/application~1json/schema/properties/kind" `
            $flattened.properties.child.'$ref' `
            "Local reference was not relocated."
        Assert-OpenApiReferencesResolve -Document $result.Document
    }

    Invoke-Test "escaped JSON Pointer segments are relocated safely" {
        $source = New-TestOpenApiDocument -Schema ([ordered]@{
            allOf = @(
                [ordered]@{
                    type = "object"
                    properties = [ordered]@{
                        "a/b~c" = [ordered]@{ type = "string" }
                        child = [ordered]@{ '$ref' = "#/paths/~1test/post/responses/200/content/application~1json/schema/allOf/0/properties/a~1b~0c" }
                    }
                },
                [ordered]@{ type = "object" }
            )
        })
        $result = ConvertTo-FlattenedOpenApiDocument -Document $source
        $flattened = Get-TestResponseSchema -Document $result.Document
        Assert-Equal `
            "#/paths/~1test/post/responses/200/content/application~1json/schema/properties/a~1b~0c" `
            $flattened.properties.child.'$ref' `
            "Escaped local reference was not relocated."
        Assert-OpenApiReferencesResolve -Document $result.Document
    }

    Invoke-Test "discriminator mapping pointers under allOf branches are relocated" {
        $source = New-TestOpenApiDocument -Schema ([ordered]@{
            allOf = @(
                [ordered]@{
                    oneOf = @(
                        [ordered]@{ type = "object"; properties = [ordered]@{ kind = [ordered]@{ const = "a" } } },
                        [ordered]@{ type = "object"; properties = [ordered]@{ kind = [ordered]@{ const = "b" } } }
                    )
                    discriminator = [ordered]@{
                        propertyName = "kind"
                        mapping = [ordered]@{
                            a = "#/paths/~1test/post/responses/200/content/application~1json/schema/allOf/0/oneOf/0"
                            b = "#/paths/~1test/post/responses/200/content/application~1json/schema/allOf/0/oneOf/1"
                        }
                    }
                },
                [ordered]@{ type = "object" }
            )
        })
        $result = ConvertTo-FlattenedOpenApiDocument -Document $source
        $mapping = (Get-TestResponseSchema -Document $result.Document).discriminator.mapping
        Assert-Equal "#/paths/~1test/post/responses/200/content/application~1json/schema/oneOf/0" $mapping.a "First mapping was not relocated."
        Assert-Equal "#/paths/~1test/post/responses/200/content/application~1json/schema/oneOf/1" $mapping.b "Second mapping was not relocated."
        Assert-OpenApiReferencesResolve -Document $result.Document
    }

    Invoke-Test "conflicting pointer relocations fail clearly" {
        $relocations = [System.Collections.Generic.Dictionary[string, string]]::new(
            [System.StringComparer]::Ordinal
        )
        Add-OpenApiPointerRelocation -Relocations $relocations -SourcePrefix "#/source" -DestinationPrefix "#/first"
        Assert-Throws {
            Add-OpenApiPointerRelocation -Relocations $relocations -SourcePrefix "#/source" -DestinationPrefix "#/second"
        } "*conflicting relocation destinations*"
    }

    Invoke-Test "relocated pointers must resolve in the flattened document" {
        $source = New-TestOpenApiDocument -Schema ([ordered]@{
            allOf = @(
                [ordered]@{
                    type = "object"
                    properties = [ordered]@{
                        child = [ordered]@{ '$ref' = "#/paths/~1test/post/responses/200/content/application~1json/schema/allOf/0/properties/missing" }
                    }
                },
                [ordered]@{ type = "object" }
            )
        })
        Assert-Throws { ConvertTo-FlattenedOpenApiDocument -Document $source } "*has no 'missing' property*"
    }

    Invoke-Test "compact four-stage pipeline matches the pre-split golden bytes" {
        $sourcePath = Join-Path $fixtureDirectory "source.openapi.json"
        $goldenPath = Join-Path $fixtureDirectory "expected.flattened.openapi.json"
        $intermediatePath = Join-Path $temporaryDirectory "things.inlined.openapi.json"
        $flattenedPath = Join-Path $temporaryDirectory "things.inlined.flattened.openapi.json"
        $simplifiedPath = Join-Path $temporaryDirectory "things.inlined.flattened.simplified.openapi.json"
        $sortedPath = Join-Path $temporaryDirectory "things.inlined.flattened.simplified.sorted.openapi.json"
        & $extractScriptPath `
            -SpecificationPath $sourcePath `
            -OperationId "createThing" `
            -OutputPath $intermediatePath
        $intermediate = Get-Content -LiteralPath $intermediatePath -Raw | ConvertFrom-Json -Depth 100
        Assert-True (@(Find-OpenApiAllOf -Value $intermediate).Count -gt 0) "Extractor intermediate did not retain allOf."
        $null = Invoke-OpenApiAllOfFlattener -InputPath $intermediatePath -OutputPath $flattenedPath
        & $simplifyScriptPath -InputPath $flattenedPath -OutputPath $simplifiedPath
        & $sortScriptPath -InputPath $simplifiedPath -OutputPath $sortedPath
        Assert-BytesEqual -ExpectedPath $goldenPath -ActualPath $sortedPath -Message "Pipeline output changed."
    }

    Invoke-Test "flattening is byte-idempotent" {
        $goldenPath = Join-Path $fixtureDirectory "expected.flattened.openapi.json"
        $outputPath = Join-Path $temporaryDirectory "idempotent.openapi.json"
        $null = Invoke-OpenApiAllOfFlattener -InputPath $goldenPath -OutputPath $outputPath
        Assert-BytesEqual -ExpectedPath $goldenPath -ActualPath $outputPath -Message "Flattening was not idempotent."
    }

    Invoke-Test "default output path creates a sibling flattened file" {
        $inputPath = Join-Path $temporaryDirectory "sample.inlined.openapi.json"
        Copy-Item -LiteralPath (Join-Path $fixtureDirectory "expected.flattened.openapi.json") -Destination $inputPath
        $expectedPath = Join-Path $temporaryDirectory "sample.inlined.flattened.openapi.json"
        $null = Invoke-OpenApiAllOfFlattener -InputPath $inputPath
        Assert-True ([System.IO.File]::Exists($expectedPath)) "Default flattened output was not created."
        Assert-BytesEqual -ExpectedPath $inputPath -ActualPath $expectedPath -Message "Default output changed already-flat input."
    }

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

    Invoke-Test "standalone invocation writes the requested output" {
        $inputPath = Join-Path $temporaryDirectory "standalone.inlined.openapi.json"
        $outputPath = Join-Path $temporaryDirectory "standalone.flattened.openapi.json"
        Copy-Item -LiteralPath (Join-Path $fixtureDirectory "expected.flattened.openapi.json") -Destination $inputPath
        & $flattenScriptPath -InputPath $inputPath -OutputPath $outputPath
        Assert-True ([System.IO.File]::Exists($outputPath)) "Standalone invocation did not create output."
        Assert-BytesEqual -ExpectedPath $inputPath -ActualPath $outputPath -Message "Standalone invocation changed already-flat input."
    }

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
