<#
.SYNOPSIS
    Runs focused tests for scripts/Wrap-OpenApiSingletonTypeEnums.ps1.
#>

[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$wrapScriptPath = Join-Path $PSScriptRoot ".." "Wrap-OpenApiSingletonTypeEnums.ps1"
. $wrapScriptPath
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

function New-TestSingletonTypeObject {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Value,

        [Parameter()]
        [switch]$WithName
    )

    $required = [System.Collections.Generic.List[string]]::new()
    $required.Add("type")
    $properties = [ordered]@{
        type = [ordered]@{
            type = "string"
            enum = @($Value)
        }
    }
    if ($WithName) {
        $required.Add("name")
        $properties["name"] = [ordered]@{ type = "string"; minLength = 1 }
    }
    return [ordered]@{
        type = "object"
        title = "preserved"
        required = [object[]]$required.ToArray()
        properties = $properties
        additionalProperties = $false
        'x-test' = [ordered]@{ note = "preserved" }
    }
}

function Get-TestSchemaPointer {
    return "#/paths/~1test/post/responses/200/content/application~1json/schema"
}

$schemaCases = Read-TestJsonFixture -RelativePath "Wrap-OpenApiSingletonTypeEnums/cases.schema.json"
$temporaryDirectory = Join-Path ([System.IO.Path]::GetTempPath()) "wrap-openapi-singleton-types-$([Guid]::NewGuid().ToString('N'))"
$null = [System.IO.Directory]::CreateDirectory($temporaryDirectory)

try {
    Invoke-Test "dot-sourcing defines wrapper functions without running the CLI" {
        Assert-True ($null -ne (Get-Command ConvertTo-WrappedOpenApiSingletonTypeEnumDocument -ErrorAction SilentlyContinue)) "Conversion function was not defined."
        Assert-True ($null -ne (Get-Command Invoke-OpenApiSingletonTypeEnumWrapper -ErrorAction SilentlyContinue)) "CLI function was not defined."
    }

    Invoke-Test "ToolChoice and ContainerUnion shapes group all typed object branches" {
        $source = New-TestOpenApiDocument -Schema (
            Get-TestFixtureCase -Cases $schemaCases -Name "productionShapes"
        )
        $sourceBefore = ConvertTo-Json -InputObject $source -Compress -Depth 100
        $result = ConvertTo-WrappedOpenApiSingletonTypeEnumDocument -Document $source
        $properties = (Get-TestResponseSchema -Document $result.Document).properties

        Assert-Equal 2 $result.GroupedOneOfCount "Grouped oneOf count was incorrect."
        Assert-Equal 4 $result.GroupedBranchCount "Grouped branch count was incorrect."
        Assert-Equal 0 $result.SingletonWrapperCount "Unexpected singleton wrapper count."
        Assert-Equal 2 $properties.toolChoice.oneOf.Count "ToolChoice outer branch count was incorrect."
        Assert-Equal "string" $properties.toolChoice.oneOf[0].type "ToolChoice scalar branch moved."
        Assert-Equal "type" $properties.toolChoice.oneOf[1].discriminator.propertyName "ToolChoice wrapper discriminator was incorrect."
        Assert-Sequence `
            @("allowed_tools", "file_search", "function") `
            @($properties.toolChoice.oneOf[1].oneOf | ForEach-Object { $_.properties.type.enum[0] }) `
            "ToolChoice object branch order changed."
        Assert-Equal 2 $properties.container.oneOf.Count "Container outer branch count was incorrect."
        Assert-Equal "string" $properties.container.oneOf[0].type "Container string branch moved."
        Assert-Sequence @("auto") @($properties.container.oneOf[1].oneOf | ForEach-Object { $_.properties.type.enum[0] }) "Container singleton wrapper was incorrect."
        Assert-Equal $sourceBefore (ConvertTo-Json -InputObject $source -Compress -Depth 100) "Conversion mutated its input."
    }

    Invoke-Test "interleaved branches preserve both relative orders and relocate pointers" {
        $root = Get-TestSchemaPointer
        $schema = [ordered]@{
            properties = [ordered]@{
                mappingOwner = [ordered]@{
                    type = "object"
                    discriminator = [ordered]@{
                        propertyName = "kind"
                        mapping = [ordered]@{
                            trailing = "$root/oneOf/3"
                        }
                    }
                }
            }
            oneOf = @(
                (New-TestSingletonTypeObject -Value "alpha"),
                [ordered]@{ type = "string" },
                (New-TestSingletonTypeObject -Value "beta"),
                [ordered]@{
                    type = "object"
                    properties = [ordered]@{
                        alias = [ordered]@{ '$ref' = "$root/oneOf/3" }
                    }
                }
            )
        }
        $result = ConvertTo-WrappedOpenApiSingletonTypeEnumDocument -Document (
            New-TestOpenApiDocument -Schema $schema
        )
        $output = Get-TestResponseSchema -Document $result.Document

        Assert-Equal 3 $output.oneOf.Count "Interleaved output branch count was incorrect."
        Assert-Sequence @("alpha", "beta") @($output.oneOf[0].oneOf | ForEach-Object { $_.properties.type.enum[0] }) "Grouped relative order changed."
        Assert-Equal "string" $output.oneOf[1].type "Nonqualifying middle branch moved incorrectly."
        Assert-Equal "$root/oneOf/2" $output.oneOf[2].properties.alias.'$ref' "Trailing branch pointer was not relocated."
        Assert-Equal "$root/oneOf/2" $output.properties.mappingOwner.discriminator.mapping.trailing "Discriminator mapping target was not relocated."
    }

    Invoke-Test "qualifying schemas in ordinary and composition positions receive singleton wrappers" {
        $root = Get-TestSchemaPointer
        $schema = [ordered]@{
            type = "object"
            properties = [ordered]@{
                standalone = New-TestSingletonTypeObject -Value "standalone" -WithName
                nullable = [ordered]@{
                    anyOf = @(
                        (New-TestSingletonTypeObject -Value "approximate"),
                        [ordered]@{ type = "null" }
                    )
                }
                intersection = [ordered]@{
                    allOf = @(
                        (New-TestSingletonTypeObject -Value "intersected"),
                        [ordered]@{ type = "object" }
                    )
                }
                list = [ordered]@{
                    type = "array"
                    items = New-TestSingletonTypeObject -Value "item"
                }
                rootAlias = [ordered]@{ '$ref' = "$root/properties/standalone" }
                childAlias = [ordered]@{ '$ref' = "$root/properties/standalone/properties/name" }
                dynamicAlias = [ordered]@{ '$dynamicRef' = "$root/properties/standalone/properties/name" }
                recursiveAlias = [ordered]@{ '$recursiveRef' = "$root/properties/standalone/properties/name" }
            }
        }
        $result = ConvertTo-WrappedOpenApiSingletonTypeEnumDocument -Document (
            New-TestOpenApiDocument -Schema $schema
        )
        $properties = (Get-TestResponseSchema -Document $result.Document).properties

        Assert-Equal 4 $result.SingletonWrapperCount "Singleton wrapper count was incorrect."
        Assert-Sequence @("standalone") @($properties.standalone.oneOf | ForEach-Object { $_.properties.type.enum[0] }) "Standalone wrapper was incorrect."
        Assert-Equal 2 $properties.nullable.anyOf.Count "Nullable anyOf changed shape."
        Assert-Sequence @("approximate") @($properties.nullable.anyOf[0].oneOf | ForEach-Object { $_.properties.type.enum[0] }) "Nullable branch was not wrapped."
        Assert-Equal "null" $properties.nullable.anyOf[1].type "Nullable null branch changed."
        Assert-Sequence @("intersected") @($properties.intersection.allOf[0].oneOf | ForEach-Object { $_.properties.type.enum[0] }) "allOf branch was not wrapped."
        Assert-Sequence @("item") @($properties.list.items.oneOf | ForEach-Object { $_.properties.type.enum[0] }) "Array item was not wrapped."
        Assert-Equal "$root/properties/standalone" $properties.rootAlias.'$ref' "Wrapper root reference changed."
        foreach ($name in @("childAlias", "dynamicAlias", "recursiveAlias")) {
            $referenceName = if ($name -ceq "childAlias") { '$ref' } elseif ($name -ceq "dynamicAlias") { '$dynamicRef' } else { '$recursiveRef' }
            Assert-Equal "$root/properties/standalone/oneOf/0/properties/name" $properties[$name][$referenceName] "Descendant reference '$name' was not relocated."
        }
    }

    Invoke-Test "valid discriminated oneOf leaves remain direct while nested candidates wrap" {
        $alpha = New-TestSingletonTypeObject -Value "alpha"
        $alpha.properties["nested"] = New-TestSingletonTypeObject -Value "nested"
        $schema = [ordered]@{
            discriminator = [ordered]@{ propertyName = "type" }
            oneOf = @(
                $alpha,
                (New-TestSingletonTypeObject -Value "beta")
            )
        }
        $result = ConvertTo-WrappedOpenApiSingletonTypeEnumDocument -Document (
            New-TestOpenApiDocument -Schema $schema
        )
        $output = Get-TestResponseSchema -Document $result.Document

        Assert-Equal 0 $result.GroupedOneOfCount "Satisfied parent was regrouped."
        Assert-Equal 1 $result.SingletonWrapperCount "Nested candidate was not wrapped exactly once."
        Assert-Equal "alpha" $output.oneOf[0].properties.type.enum[0] "Satisfied alpha leaf was wrapped."
        Assert-Equal "beta" $output.oneOf[1].properties.type.enum[0] "Satisfied beta leaf was wrapped."
        Assert-Sequence @("nested") @($output.oneOf[0].properties.nested.oneOf | ForEach-Object { $_.properties.type.enum[0] }) "Nested candidate wrapper was incorrect."
    }

    Invoke-Test "enum-only near misses remain unchanged and const must agree" {
        $nearMisses = New-TestOpenApiDocument -Schema (
            Get-TestFixtureCase -Cases $schemaCases -Name "nearMisses"
        )
        $before = ConvertTo-Json -InputObject $nearMisses -Compress -Depth 100
        $result = ConvertTo-WrappedOpenApiSingletonTypeEnumDocument -Document $nearMisses
        Assert-Equal 0 $result.GeneratedWrapperCount "A near miss was wrapped."
        Assert-Equal $before (ConvertTo-Json -InputObject $result.Document -Compress -Depth 100) "A near miss or literal changed."

        $consistent = New-TestSingletonTypeObject -Value "consistent"
        $consistent.properties.type["const"] = "consistent"
        $consistentResult = ConvertTo-WrappedOpenApiSingletonTypeEnumDocument -Document (
            New-TestOpenApiDocument -Schema $consistent
        )
        Assert-Equal 1 $consistentResult.SingletonWrapperCount "Consistent const sibling prevented wrapping."

        $conflicting = New-TestSingletonTypeObject -Value "enum_value"
        $conflicting.properties.type["const"] = "other"
        Assert-Throws {
            ConvertTo-WrappedOpenApiSingletonTypeEnumDocument -Document (
                New-TestOpenApiDocument -Schema $conflicting
            )
        } "*const*must equal singleton enum value*"
    }

    Invoke-Test "late-stage invariant and incompatible discriminator failures are explicit" {
        $eligibleParent = [ordered]@{
            oneOf = @(
                (New-TestSingletonTypeObject -Value "alpha"),
                (New-TestSingletonTypeObject -Value "beta")
            )
        }
        Assert-Throws {
            ConvertTo-WrappedOpenApiSingletonTypeEnumDocument -Document (
                New-TestOpenApiDocument -Schema $eligibleParent
            )
        } "*remains eligible for direct type discriminator inference*"

        $incompatible = Copy-JsonValue -Value $eligibleParent
        $incompatible["discriminator"] = [ordered]@{ propertyName = "kind" }
        Assert-Throws {
            ConvertTo-WrappedOpenApiSingletonTypeEnumDocument -Document (
                New-TestOpenApiDocument -Schema $incompatible
            )
        } "*incompatible discriminator propertyName 'kind'*"

    }

    Invoke-Test "duplicate singleton values receive separate valid wrappers" {
        $duplicate = [ordered]@{
            oneOf = @(
                [ordered]@{ type = "string" },
                (New-TestSingletonTypeObject -Value "same"),
                (New-TestSingletonTypeObject -Value "same")
            )
        }
        $result = ConvertTo-WrappedOpenApiSingletonTypeEnumDocument -Document (
            New-TestOpenApiDocument -Schema $duplicate
        )
        $output = Get-TestResponseSchema -Document $result.Document
        Assert-Equal 3 $output.oneOf.Count "Duplicate-value fallback changed the outer branch count."
        Assert-Equal 2 $result.SingletonWrapperCount "Duplicate-value branches were not wrapped separately."
        foreach ($index in @(1, 2)) {
            Assert-Equal "type" $output.oneOf[$index].discriminator.propertyName "Duplicate-value wrapper discriminator was incorrect."
            Assert-Sequence @("same") @($output.oneOf[$index].oneOf | ForEach-Object { $_.properties.type.enum[0] }) "Duplicate-value wrapper leaf was incorrect."
        }
    }

    Invoke-Test "scope-sensitive candidates fail before output conversion" {
        foreach ($keyword in @('$id', '$anchor', '$dynamicAnchor', '$recursiveAnchor')) {
            $candidate = New-TestSingletonTypeObject -Value "blocked"
            $candidate.properties.type[$keyword] = "blocked"
            Assert-Throws {
                ConvertTo-WrappedOpenApiSingletonTypeEnumDocument -Document (
                    New-TestOpenApiDocument -Schema $candidate
                )
            } "*scope-sensitive keyword '$keyword'*"
        }
    }

    Invoke-Test "postcondition and transformation are structurally idempotent" {
        $source = New-TestOpenApiDocument -Schema (New-TestSingletonTypeObject -Value "alpha")
        Assert-Throws {
            Assert-NoUnwrappedOpenApiSingletonTypeEnums -Value $source
        } "*retained required singleton type enum*"

        $first = ConvertTo-WrappedOpenApiSingletonTypeEnumDocument -Document $source
        $second = ConvertTo-WrappedOpenApiSingletonTypeEnumDocument -Document $first.Document
        Assert-Equal 0 $second.GeneratedWrapperCount "Second pass generated wrappers."
        Assert-JsonEqual $first.Document $second.Document "Second pass changed the document."
    }

    Invoke-Test "CLI paths encoding atomicity and byte idempotence are correct" {
        $sourcePath = Join-Path $temporaryDirectory "sample.oneof-flattened.openapi.json"
        $null = Write-OpenApiJsonFile `
            -Document (New-TestOpenApiDocument -Schema (New-TestSingletonTypeObject -Value "alpha")) `
            -Path $sourcePath
        [byte[]]$sourceBefore = [System.IO.File]::ReadAllBytes($sourcePath)
        $first = Invoke-OpenApiSingletonTypeEnumWrapper -InputPath $sourcePath
        $outputPath = Join-Path $temporaryDirectory "sample.oneof-flattened.singleton-type-wrapped.openapi.json"

        Assert-True ([System.IO.File]::Exists($outputPath)) "Default wrapped output was not created."
        Assert-BytesEqual $sourceBefore ([System.IO.File]::ReadAllBytes($sourcePath)) "Wrapper modified its input."
        [byte[]]$outputBytes = [System.IO.File]::ReadAllBytes($outputPath)
        $hasBom = $outputBytes.Length -ge 3 -and $outputBytes[0] -eq 0xEF -and $outputBytes[1] -eq 0xBB -and $outputBytes[2] -eq 0xBF
        Assert-True (-not $hasBom) "Wrapped output contains a BOM."
        Assert-Equal 10 $outputBytes[$outputBytes.Length - 1] "Wrapped output lacks a trailing LF."
        Assert-Equal 1 $first.SingletonWrapperCount "First CLI wrapper count was incorrect."

        $explicitPath = Join-Path $temporaryDirectory "explicit.json"
        $null = Invoke-OpenApiSingletonTypeEnumWrapper -InputPath $sourcePath -OutputPath $explicitPath
        Assert-BytesEqual $outputBytes ([System.IO.File]::ReadAllBytes($explicitPath)) "Explicit output differed from default output."
        Assert-Equal ([System.IO.Path]::GetFullPath($outputPath)) (Get-DefaultWrappedOpenApiSingletonTypeEnumPath $outputPath) "Wrapper marker repeated."

        $null = Invoke-OpenApiSingletonTypeEnumWrapper -InputPath $outputPath
        [byte[]]$firstInPlace = [System.IO.File]::ReadAllBytes($outputPath)
        $second = Invoke-OpenApiSingletonTypeEnumWrapper -InputPath $outputPath
        Assert-Equal 0 $second.GeneratedWrapperCount "Second in-place pass generated wrappers."
        Assert-BytesEqual $firstInPlace ([System.IO.File]::ReadAllBytes($outputPath)) "In-place wrapping changed bytes."

        $unsafePath = Join-Path $temporaryDirectory "unsafe.json"
        $unsafe = New-TestSingletonTypeObject -Value "unsafe"
        $unsafe['$anchor'] = "blocked"
        $null = Write-OpenApiJsonFile -Document (New-TestOpenApiDocument -Schema $unsafe) -Path $unsafePath
        [System.IO.File]::WriteAllText($explicitPath, "preserved", [System.Text.UTF8Encoding]::new($false))
        [byte[]]$preserved = [System.IO.File]::ReadAllBytes($explicitPath)
        Assert-Throws {
            Invoke-OpenApiSingletonTypeEnumWrapper -InputPath $unsafePath -OutputPath $explicitPath
        } "*scope-sensitive keyword*"
        Assert-BytesEqual $preserved ([System.IO.File]::ReadAllBytes($explicitPath)) "Failed wrapping changed existing output."

        Assert-Throws { Invoke-OpenApiSingletonTypeEnumWrapper -InputPath (Join-Path $temporaryDirectory "missing.json") } "*cannot find path*"
        Assert-Throws { Invoke-OpenApiSingletonTypeEnumWrapper -InputPath $temporaryDirectory } "*is not a file*"
        $invalidPath = Join-Path $temporaryDirectory "invalid.json"
        [System.IO.File]::WriteAllText($invalidPath, "{ invalid", [System.Text.UTF8Encoding]::new($false))
        Assert-Throws { Invoke-OpenApiSingletonTypeEnumWrapper -InputPath $invalidPath } "*Could not parse OpenAPI JSON*"
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
