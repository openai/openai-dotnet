<#
.SYNOPSIS
    Runs focused tests for scripts/specification/pipeline/Split-OpenApiMultiValueTypeEnums.ps1.
#>

[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$splitScriptPath = Join-Path $PSScriptRoot ".." "pipeline" "Split-OpenApiMultiValueTypeEnums.ps1"
. $splitScriptPath
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

function New-TestTypeEnumCandidate {
    param(
        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [object[]]$Values
    )

    $schema = [ordered]@{
        type = "object"
        title = "preserved"
        required = @("type", "name")
        properties = [ordered]@{
            type = [ordered]@{
                type = "string"
                enum = [object[]]$Values
            }
            name = [ordered]@{
                type = "string"
                minLength = 1
            }
        }
        additionalProperties = $false
        'x-test' = [ordered]@{ note = "preserved" }
    }
    return $schema
}

function Get-TestSchemaPointer {
    return "#/paths/~1test/post/responses/200/content/application~1json/schema"
}

$schemaCases = Read-TestJsonFixture -RelativePath "Split-OpenApiMultiValueTypeEnums/cases.schema.json"
$temporaryDirectory = Join-Path ([System.IO.Path]::GetTempPath()) "split-openapi-type-enums-$([Guid]::NewGuid().ToString('N'))"
$null = [System.IO.Directory]::CreateDirectory($temporaryDirectory)

try {
    Invoke-Test "dot-sourcing defines split functions without running the CLI" {
        Assert-True ($null -ne (Get-Command ConvertTo-SplitOpenApiMultiValueTypeEnumDocument -ErrorAction SilentlyContinue)) "Conversion function was not defined."
        Assert-True ($null -ne (Get-Command Invoke-OpenApiMultiValueTypeEnumSplitter -ErrorAction SilentlyContinue)) "CLI function was not defined."
    }

    Invoke-Test "direct oneOf and anyOf branches expand in enum encounter order" {
        $schema = Get-TestFixtureCase -Cases $schemaCases -Name "directCompositions"
        $source = New-TestOpenApiDocument -Schema $schema
        $sourceBefore = ConvertTo-Json -InputObject $source -Compress -Depth 100
        $result = ConvertTo-SplitOpenApiMultiValueTypeEnumDocument -Document $source
        $output = Get-TestResponseSchema -Document $result.Document

        Assert-Equal 2 $result.SplitSchemaCount "Split count was incorrect."
        Assert-Equal 9 $result.GeneratedVariantCount "Variant count was incorrect."
        Assert-Equal 2 $result.ExpandedCompositionBranchCount "Expanded branch count was incorrect."
        Assert-Equal 0 $result.WrappedSchemaCount "Unexpected wrapped schema count."
        Assert-Equal 9 $output.properties.choice.oneOf.Count "ToolChoice-like branch count was incorrect."
        Assert-Sequence `
            @("file_search", "web_search_preview", "computer", "computer_use_preview", "computer_use", "image_generation", "code_interpreter") `
            @($output.properties.choice.oneOf[1..7] | ForEach-Object { $_.properties.type.enum[0] }) `
            "ToolChoice-like enum order changed."
        Assert-Sequence @("first", "second") @($output.properties.fallback.anyOf[0..1] | ForEach-Object { $_.properties.type.enum[0] }) "anyOf enum order changed."
        foreach ($variant in $output.properties.choice.oneOf[1..7]) {
            Assert-Equal 1 $variant.properties.type.enum.Count "Variant did not retain a singleton enum array."
            Assert-Equal "preserved" $variant.title "Variant annotation changed."
            Assert-Equal 1 $variant.properties.name.minLength "Variant constraints changed."
            Assert-Equal $false $variant.additionalProperties "Variant additionalProperties changed."
            Assert-Equal "preserved" $variant.'x-test'.note "Variant extension changed."
        }
        Assert-Equal $sourceBefore (ConvertTo-Json -InputObject $source -Compress -Depth 100) "Conversion mutated its input."
    }

    Invoke-Test "standalone allOf and component candidates become transparent oneOf wrappers" {
        $schema = [ordered]@{
            type = "object"
            properties = [ordered]@{
                standalone = New-TestTypeEnumCandidate -Values @("alpha", "beta")
                intersection = [ordered]@{
                    allOf = @(
                        (New-TestTypeEnumCandidate -Values @("left", "right")),
                        [ordered]@{ type = "object"; properties = [ordered]@{ value = [ordered]@{ type = "string" } } }
                    )
                }
            }
        }
        $source = New-TestOpenApiDocument -Schema $schema
        $source.components = [ordered]@{
            schemas = [ordered]@{
                Existing = New-TestTypeEnumCandidate -Values @("one", "two")
            }
        }
        $result = ConvertTo-SplitOpenApiMultiValueTypeEnumDocument -Document $source
        $output = Get-TestResponseSchema -Document $result.Document

        Assert-Equal 3 $result.SplitSchemaCount "Wrapped split count was incorrect."
        Assert-Equal 3 $result.WrappedSchemaCount "Wrapped schema count was incorrect."
        Assert-Sequence @("oneOf") @((Get-JsonProperties -Value $output.properties.standalone).Name) "Standalone wrapper keys were incorrect."
        Assert-Sequence @("alpha", "beta") @($output.properties.standalone.oneOf | ForEach-Object { $_.properties.type.enum[0] }) "Standalone variants were incorrect."
        Assert-Sequence @("left", "right") @($output.properties.intersection.allOf[0].oneOf | ForEach-Object { $_.properties.type.enum[0] }) "allOf branch was not wrapped."
        Assert-Sequence @("one", "two") @($result.Document.components.schemas.Existing.oneOf | ForEach-Object { $_.properties.type.enum[0] }) "Component root was not wrapped."
    }

    Invoke-Test "optional and malformed near misses remain unchanged" {
        $schema = Get-TestFixtureCase -Cases $schemaCases -Name "nearMissesAndLiterals"
        $source = New-TestOpenApiDocument -Schema $schema
        $before = ConvertTo-Json -InputObject $source -Compress -Depth 100
        $result = ConvertTo-SplitOpenApiMultiValueTypeEnumDocument -Document $source
        Assert-Equal 0 $result.SplitSchemaCount "A near miss was split."
        Assert-Equal $before (ConvertTo-Json -InputObject $result.Document -Compress -Depth 100) "A near miss or literal changed."
    }

    Invoke-Test "mixed and duplicate multi-value enums fail clearly" {
        foreach ($case in @(
            [PSCustomObject]@{ Values = [object[]]@("alpha", 2); Pattern = "*enum/1*must be a string*" },
            [PSCustomObject]@{ Values = [object[]]@("alpha", "alpha"); Pattern = "*contains duplicate value 'alpha'*" }
        )) {
            $source = New-TestOpenApiDocument -Schema (New-TestTypeEnumCandidate -Values $case.Values)
            Assert-Throws {
                ConvertTo-SplitOpenApiMultiValueTypeEnumDocument -Document $source
            } $case.Pattern
        }
    }

    Invoke-Test "nested candidates are transformed once before outer variants are copied" {
        $outer = New-TestTypeEnumCandidate -Values @("outer_a", "outer_b")
        $outer.properties["nested"] = New-TestTypeEnumCandidate -Values @("inner_a", "inner_b")
        $source = New-TestOpenApiDocument -Schema $outer
        $result = ConvertTo-SplitOpenApiMultiValueTypeEnumDocument -Document $source
        $schema = Get-TestResponseSchema -Document $result.Document

        Assert-Equal 2 $result.SplitSchemaCount "Nested source occurrence count was incorrect."
        Assert-Equal 4 $result.GeneratedVariantCount "Nested source variant count was incorrect."
        Assert-Equal 2 $result.WrappedSchemaCount "Nested wrapped count was incorrect."
        Assert-Equal 2 $schema.oneOf.Count "Outer candidate did not split."
        foreach ($outerVariant in $schema.oneOf) {
            Assert-Sequence @("inner_a", "inner_b") @($outerVariant.properties.nested.oneOf | ForEach-Object { $_.properties.type.enum[0] }) "Nested variants were not copied into each outer variant."
        }
    }

    Invoke-Test "scope-sensitive candidate subtrees fail before splitting" {
        foreach ($keyword in @('$id', '$anchor', '$dynamicAnchor', '$recursiveAnchor')) {
            $candidate = New-TestTypeEnumCandidate -Values @("alpha", "beta")
            $candidate.properties.name[$keyword] = "blocked"
            Assert-Throws {
                ConvertTo-SplitOpenApiMultiValueTypeEnumDocument -Document (
                    New-TestOpenApiDocument -Schema $candidate
                )
            } "*scope-sensitive keyword '$keyword'*properties/name*"
        }
    }

    Invoke-Test "shifted sibling pointers relocate and ambiguous candidate targets fail" {
        $candidate = New-TestTypeEnumCandidate -Values @("alpha", "beta")
        $schema = [ordered]@{
            type = "object"
            properties = [ordered]@{
                choice = [ordered]@{
                    oneOf = @(
                        $candidate,
                        [ordered]@{ type = "object"; properties = [ordered]@{ value = [ordered]@{ type = "string" } } }
                    )
                }
                alias = [ordered]@{
                    '$ref' = "$(Get-TestSchemaPointer)/properties/choice/oneOf/1"
                }
            }
        }
        $result = ConvertTo-SplitOpenApiMultiValueTypeEnumDocument -Document (
            New-TestOpenApiDocument -Schema $schema
        )
        $output = Get-TestResponseSchema -Document $result.Document
        Assert-Equal "$(Get-TestSchemaPointer)/properties/choice/oneOf/2" $output.properties.alias.'$ref' "Shifted sibling reference was not relocated."
        Assert-Equal 1 $result.RelocationCount "Relocation count was incorrect."

        $schema.properties.alias.'$ref' = "$(Get-TestSchemaPointer)/properties/choice/oneOf/0"
        Assert-Throws {
            ConvertTo-SplitOpenApiMultiValueTypeEnumDocument -Document (
                New-TestOpenApiDocument -Schema $schema
            )
        } "*reference*targets ambiguous location*"

        $wrapped = [ordered]@{
            type = "object"
            properties = [ordered]@{
                target = New-TestTypeEnumCandidate -Values @("alpha", "beta")
                rootAlias = [ordered]@{ '$ref' = "$(Get-TestSchemaPointer)/properties/target" }
            }
        }
        $wrappedResult = ConvertTo-SplitOpenApiMultiValueTypeEnumDocument -Document (
            New-TestOpenApiDocument -Schema $wrapped
        )
        Assert-Equal "$(Get-TestSchemaPointer)/properties/target" (Get-TestResponseSchema $wrappedResult.Document).properties.rootAlias.'$ref' "Wrapped root reference changed."
        $wrapped.properties["descendantAlias"] = [ordered]@{ '$ref' = "$(Get-TestSchemaPointer)/properties/target/properties/name" }
        Assert-Throws {
            ConvertTo-SplitOpenApiMultiValueTypeEnumDocument -Document (
                New-TestOpenApiDocument -Schema $wrapped
            )
        } "*reference*targets ambiguous location*"
    }

    Invoke-Test "postcondition and conversion are structurally idempotent" {
        $source = New-TestOpenApiDocument -Schema (New-TestTypeEnumCandidate -Values @("alpha", "beta"))
        Assert-Throws {
            Assert-NoSplittableOpenApiMultiValueTypeEnums -Document $source
        } "*retained eligible multi-value type enum*"

        $first = ConvertTo-SplitOpenApiMultiValueTypeEnumDocument -Document $source
        $second = ConvertTo-SplitOpenApiMultiValueTypeEnumDocument -Document $first.Document
        Assert-Equal 0 $second.SplitSchemaCount "Second pass split a schema."
        Assert-JsonEqual $first.Document $second.Document "Second pass changed the document."
    }

    Invoke-Test "default paths deterministic encoding and byte idempotence are correct" {
        $sourcePath = Join-Path $temporaryDirectory "sample.recursive-simplified.openapi.json"
        $source = New-TestOpenApiDocument -Schema (New-TestTypeEnumCandidate -Values @("alpha", "beta"))
        $null = Write-OpenApiJsonFile -Document $source -Path $sourcePath
        [byte[]]$sourceBefore = [System.IO.File]::ReadAllBytes($sourcePath)

        $first = Invoke-OpenApiMultiValueTypeEnumSplitter -InputPath $sourcePath
        $outputPath = Join-Path $temporaryDirectory "sample.recursive-simplified.type-enum-split.openapi.json"
        Assert-True ([System.IO.File]::Exists($outputPath)) "Default split output was not created."
        Assert-BytesEqual $sourceBefore ([System.IO.File]::ReadAllBytes($sourcePath)) "Splitter modified its input."
        [byte[]]$outputBytes = [System.IO.File]::ReadAllBytes($outputPath)
        $hasBom = $outputBytes.Length -ge 3 -and $outputBytes[0] -eq 0xEF -and $outputBytes[1] -eq 0xBB -and $outputBytes[2] -eq 0xBF
        Assert-True (-not $hasBom) "Split output contains a BOM."
        Assert-Equal 10 $outputBytes[$outputBytes.Length - 1] "Split output lacks a trailing LF."
        Assert-Equal 1 $first.SplitSchemaCount "First CLI split count was incorrect."

        $explicitPath = Join-Path $temporaryDirectory "explicit.json"
        $null = Invoke-OpenApiMultiValueTypeEnumSplitter -InputPath $sourcePath -OutputPath $explicitPath
        Assert-BytesEqual $outputBytes ([System.IO.File]::ReadAllBytes($explicitPath)) "Explicit output differed from default output."
        Assert-Equal ([System.IO.Path]::GetFullPath($outputPath)) (Get-DefaultSplitOpenApiMultiValueTypeEnumPath -InputPath $outputPath) "Split marker repeated."

        $null = Invoke-OpenApiMultiValueTypeEnumSplitter -InputPath $outputPath
        [byte[]]$firstInPlace = [System.IO.File]::ReadAllBytes($outputPath)
        $second = Invoke-OpenApiMultiValueTypeEnumSplitter -InputPath $outputPath
        Assert-Equal 0 $second.SplitSchemaCount "Second in-place pass split a schema."
        Assert-BytesEqual $firstInPlace ([System.IO.File]::ReadAllBytes($outputPath)) "In-place split changed bytes."
    }

    Invoke-Test "unsafe failures preserve output and invalid inputs fail clearly" {
        $sourcePath = Join-Path $temporaryDirectory "unsafe.openapi.json"
        $candidate = New-TestTypeEnumCandidate -Values @("alpha", "beta")
        $candidate['$anchor'] = "blocked"
        $null = Write-OpenApiJsonFile -Document (New-TestOpenApiDocument -Schema $candidate) -Path $sourcePath
        $outputPath = Join-Path $temporaryDirectory "preserved.json"
        [System.IO.File]::WriteAllText($outputPath, "preserved", [System.Text.UTF8Encoding]::new($false))
        [byte[]]$before = [System.IO.File]::ReadAllBytes($outputPath)
        Assert-Throws {
            Invoke-OpenApiMultiValueTypeEnumSplitter -InputPath $sourcePath -OutputPath $outputPath
        } "*scope-sensitive keyword*"
        Assert-BytesEqual $before ([System.IO.File]::ReadAllBytes($outputPath)) "Failed split changed existing output."

        Assert-Throws {
            Invoke-OpenApiMultiValueTypeEnumSplitter -InputPath (Join-Path $temporaryDirectory "missing.json")
        } "*cannot find path*"
        Assert-Throws {
            Invoke-OpenApiMultiValueTypeEnumSplitter -InputPath $temporaryDirectory
        } "*is not a file*"
        $invalidPath = Join-Path $temporaryDirectory "invalid.json"
        [System.IO.File]::WriteAllText($invalidPath, "{ invalid", [System.Text.UTF8Encoding]::new($false))
        Assert-Throws {
            Invoke-OpenApiMultiValueTypeEnumSplitter -InputPath $invalidPath
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
