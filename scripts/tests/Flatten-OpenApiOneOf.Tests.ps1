<#
.SYNOPSIS
    Runs focused tests for scripts/Flatten-OpenApiOneOf.ps1.
#>

[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$scriptDirectory = Join-Path $PSScriptRoot ".."
$flattenScriptPath = Join-Path $scriptDirectory "Flatten-OpenApiOneOf.ps1"
. $flattenScriptPath
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

function New-OneOfLeaf {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Value
    )

    return [ordered]@{
        type = "object"
        required = @("kind")
        properties = [ordered]@{
            kind = [ordered]@{
                type = "string"
                enum = @($Value)
            }
        }
    }
}

function New-TransparentOneOfSchema {
    $discriminator = [ordered]@{ propertyName = "kind" }
    $wrapper = [ordered]@{
        type = "object"
        discriminator = Copy-JsonValue -Value $discriminator
        oneOf = @(
            (New-OneOfLeaf -Value "system_message")
            (New-OneOfLeaf -Value "user_message")
        )
    }
    return [ordered]@{
        discriminator = $discriminator
        oneOf = @(
            (New-OneOfLeaf -Value "alpha")
            $wrapper
            (New-OneOfLeaf -Value "omega")
        )
    }
}

function New-GroupedOneOfSchema {
    $base = "#/paths/~1test/post/responses/200/content/application~1json/schema"
    $discriminator = [ordered]@{
        propertyName = "kind"
        mapping = [ordered]@{
            input = "$base/oneOf/0/oneOf/0"
            output = "$base/oneOf/1/oneOf/1"
        }
    }
    return [ordered]@{
        discriminator = $discriminator
        oneOf = @(
            [ordered]@{
                discriminator = [ordered]@{ propertyName = "kind" }
                oneOf = @(
                    (New-OneOfLeaf -Value "input_text")
                    (New-OneOfLeaf -Value "input_image")
                    (New-OneOfLeaf -Value "input_file")
                )
            },
            [ordered]@{
                discriminator = [ordered]@{ propertyName = "kind" }
                oneOf = @(
                    (New-OneOfLeaf -Value "output_text")
                    (New-OneOfLeaf -Value "refusal")
                )
            }
        )
    }
}

function New-MatchingOneOfDocument {
    return Read-TestJsonFixture -RelativePath "Flatten-OpenApiOneOf/source.openapi.json"
}

$fixtureDirectory = Join-Path $PSScriptRoot "fixtures" "Flatten-OpenApiOneOf"
$temporaryDirectory = Join-Path ([System.IO.Path]::GetTempPath()) "flatten-openapi-oneof-$([Guid]::NewGuid().ToString('N'))"
$null = [System.IO.Directory]::CreateDirectory($temporaryDirectory)

try {
    Invoke-Test "dot-sourcing defines flattener functions without running the CLI" {
        Assert-True ($null -ne (Get-Command ConvertTo-FlattenedOpenApiOneOfDocument -ErrorAction SilentlyContinue)) "Conversion function was not defined."
        Assert-True ($null -ne (Get-Command Invoke-OpenApiOneOfFlattener -ErrorAction SilentlyContinue)) "CLI function was not defined."
    }

    Invoke-Test "matching safe siblings flatten to the golden without mutating input" {
        $source = New-MatchingOneOfDocument
        $expected = Read-OpenApiDocument -Path (Join-Path $fixtureDirectory "expected.oneof-flattened.openapi.json")
        $sourceBefore = ConvertTo-Json -InputObject $source -Compress -Depth 100
        $result = ConvertTo-FlattenedOpenApiOneOfDocument -Document $source
        Assert-Equal 1 $result.FlattenedOneOfCount "Unexpected wrapper count."
        Assert-Equal 2 $result.PromotedBranchCount "Unexpected promoted branch count."
        Assert-Equal 7 $result.RelocationCount "Unexpected relocation count."
        Assert-JsonEqual $expected $result.Document "Flattened fixture did not match its golden."
        Assert-Equal $sourceBefore (ConvertTo-Json -InputObject $source -Compress -Depth 100) "Conversion mutated its input."

        $schema = Get-TestResponseSchema -Document $result.Document
        Assert-Sequence @("alpha", "system_message", "user_message", "omega") @(
            $schema.oneOf | ForEach-Object { $_.properties.kind.enum[0] }
        ) "Promoted branch order changed."
        Assert-Sequence @("shared", "label") @($schema.required) "Shared required constraint changed."
        Assert-Equal $true $schema.additionalProperties "Shared additionalProperties changed."
    }

    Invoke-Test "the original transparent wrapper shape remains eligible" {
        $source = New-TestOpenApiDocument -Schema (New-TransparentOneOfSchema)
        $result = ConvertTo-FlattenedOpenApiOneOfDocument -Document $source
        $schema = Get-TestResponseSchema -Document $result.Document
        Assert-Equal 1 $result.FlattenedOneOfCount "Transparent wrapper was not flattened."
        Assert-Equal 4 $schema.oneOf.Count "Transparent wrapper emitted the wrong branch count."
        Assert-Sequence @("alpha", "system_message", "user_message", "omega") @(
            $schema.oneOf | ForEach-Object { $_.properties.kind.enum[0] }
        ) "Transparent wrapper changed branch order."
    }

    Invoke-Test "composition-only wrapper groups flatten into one discriminated union" {
        $source = New-TestOpenApiDocument -Schema (New-GroupedOneOfSchema)
        $result = ConvertTo-FlattenedOpenApiOneOfDocument -Document $source
        $schema = Get-TestResponseSchema -Document $result.Document
        Assert-Equal 2 $result.FlattenedOneOfCount "Grouped wrapper count was incorrect."
        Assert-Equal 5 $result.PromotedBranchCount "Grouped promotion count was incorrect."
        Assert-Sequence @("input_text", "input_image", "input_file", "output_text", "refusal") @(
            $schema.oneOf | ForEach-Object { $_.properties.kind.enum[0] }
        ) "Grouped flattening changed leaf order."
        Assert-Equal `
            "#/paths/~1test/post/responses/200/content/application~1json/schema/oneOf/0" `
            $schema.discriminator.mapping.input `
            "Input mapping was not relocated."
        Assert-Equal `
            "#/paths/~1test/post/responses/200/content/application~1json/schema/oneOf/4" `
            $schema.discriminator.mapping.output `
            "Output mapping was not relocated."
        Assert-True (-not @($schema.oneOf | Where-Object {
            Test-JsonProperty -Object $_ -Name "oneOf"
        }).Count) "Grouped flattening retained a nested wrapper."

        $second = ConvertTo-FlattenedOpenApiOneOfDocument -Document $result.Document
        Assert-Equal 0 $second.FlattenedOneOfCount "Second grouped pass flattened a wrapper."
        Assert-JsonEqual $result.Document $second.Document "Grouped flattening was not structurally idempotent."
    }

    Invoke-Test "const and multi-value enum leaves flatten" {
        $candidate = New-TransparentOneOfSchema
        $firstProperty = $candidate.oneOf[1].oneOf[0].properties.kind
        $null = $firstProperty.Remove("enum")
        $firstProperty["const"] = "system_message"
        $candidate.oneOf[1].oneOf[1].properties.kind.enum = @("user_message", "assistant_message")

        $result = ConvertTo-FlattenedOpenApiOneOfDocument `
            -Document (New-TestOpenApiDocument -Schema $candidate)
        Assert-Equal 1 $result.FlattenedOneOfCount "Finite const/enum wrapper was not flattened."
        Assert-Equal 4 (Get-TestResponseSchema $result.Document).oneOf.Count "Finite const/enum wrapper emitted the wrong branch count."
    }

    Invoke-Test "duplicate or optional recursive discriminator values remain nested" {
        $duplicate = New-TransparentOneOfSchema
        $duplicate.oneOf[1].oneOf[0].properties.kind.enum = @("alpha")
        Assert-Equal 0 (ConvertTo-FlattenedOpenApiOneOfDocument `
            -Document (New-TestOpenApiDocument -Schema $duplicate)).FlattenedOneOfCount "Cross-level duplicate was flattened."

        $optional = New-TransparentOneOfSchema
        $null = $optional.oneOf[1].oneOf[0].Remove("required")
        Assert-Equal 0 (ConvertTo-FlattenedOpenApiOneOfDocument `
            -Document (New-TestOpenApiDocument -Schema $optional)).FlattenedOneOfCount "Optional discriminator leaf was flattened."
    }

    Invoke-Test "mapping differences do not block structural flattening" {
        $candidate = New-TransparentOneOfSchema
        $candidate.discriminator["mapping"] = [ordered]@{
            alpha = "#/paths/~1test/post/responses/200/content/application~1json/schema/oneOf/0"
        }
        $candidate.oneOf[1].discriminator["mapping"] = [ordered]@{
            system_message = "#/paths/~1test/post/responses/200/content/application~1json/schema/oneOf/1/oneOf/0"
        }

        $result = ConvertTo-FlattenedOpenApiOneOfDocument `
            -Document (New-TestOpenApiDocument -Schema $candidate)
        Assert-Equal 1 $result.FlattenedOneOfCount "Mapping differences prevented structural flattening."
    }

    Invoke-Test "object member order is insignificant for matching siblings" {
        $source = New-MatchingOneOfDocument
        $schema = Get-TestResponseSchema -Document $source
        $wrapper = $schema.oneOf[1]
        $reorderedProperties = [ordered]@{}
        foreach ($name in @("sharedAlias", "label", "shared")) {
            $reorderedProperties[$name] = $wrapper.properties[$name]
        }
        $wrapper.properties = $reorderedProperties
        $result = ConvertTo-FlattenedOpenApiOneOfDocument -Document $source
        Assert-Equal 1 $result.FlattenedOneOfCount "Object key reordering prevented flattening."
    }

    Invoke-Test "unequal missing extra and reordered-array siblings remain nested" {
        $cases = @(
            [PSCustomObject]@{ Name = "unequal value"; Mutate = { param($wrapper) $wrapper.additionalProperties = $false } },
            [PSCustomObject]@{ Name = "missing sibling"; Mutate = { param($wrapper) $null = $wrapper.Remove("required") } },
            [PSCustomObject]@{ Name = "extra sibling"; Mutate = { param($wrapper) $wrapper["minProperties"] = 1 } },
            [PSCustomObject]@{ Name = "array order"; Mutate = { param($wrapper) $wrapper.required = @("label", "shared") } }
        )
        foreach ($case in $cases) {
            $source = New-MatchingOneOfDocument
            $schema = Get-TestResponseSchema -Document $source
            & $case.Mutate $schema.oneOf[1]
            $result = ConvertTo-FlattenedOpenApiOneOfDocument -Document $source
            Assert-Equal 0 $result.FlattenedOneOfCount "Near miss '$($case.Name)' was flattened."
            Assert-Equal 3 (Get-TestResponseSchema -Document $result.Document).oneOf.Count "Near miss '$($case.Name)' changed shape."
        }
    }

    Invoke-Test "location-sensitive keywords and extensions remain nested" {
        foreach ($propertyName in @('$id', '$anchor', '$dynamicAnchor', '$recursiveAnchor', 'x-test', 'X-Test')) {
            $source = New-MatchingOneOfDocument
            $schema = Get-TestResponseSchema -Document $source
            $schema[$propertyName] = "blocked"
            $schema.oneOf[1][$propertyName] = "blocked"
            $result = ConvertTo-FlattenedOpenApiOneOfDocument -Document $source
            Assert-Equal 0 $result.FlattenedOneOfCount "Unsafe sibling '$propertyName' was flattened."
        }

        $source = New-MatchingOneOfDocument
        $schema = Get-TestResponseSchema -Document $source
        $schema.properties.shared["x-nested"] = $true
        $schema.oneOf[1].properties.shared["x-nested"] = $true
        $result = ConvertTo-FlattenedOpenApiOneOfDocument -Document $source
        Assert-Equal 0 $result.FlattenedOneOfCount "Nested extension was flattened."
    }

    Invoke-Test "mismatched discriminator and malformed leaves remain nested" {
        $source = New-MatchingOneOfDocument
        $schema = Get-TestResponseSchema -Document $source
        $schema.oneOf[1].discriminator.propertyName = "other"
        Assert-Equal 0 (ConvertTo-FlattenedOpenApiOneOfDocument -Document $source).FlattenedOneOfCount "Mismatched discriminator was flattened."

        $source = New-MatchingOneOfDocument
        $schema = Get-TestResponseSchema -Document $source
        $null = $schema.oneOf[1].oneOf[0].properties.kind.Remove("enum")
        Assert-Equal 0 (ConvertTo-FlattenedOpenApiOneOfDocument -Document $source).FlattenedOneOfCount "Malformed leaf was flattened."

        $source = New-MatchingOneOfDocument
        $schema = Get-TestResponseSchema -Document $source
        $schema.oneOf[1] = [ordered]@{ '$ref' = "#/paths/~1test/post/responses/200/content/application~1json/schema/oneOf/0" }
        $schema.discriminator = [ordered]@{ propertyName = "kind" }
        $null = $schema.properties.Remove("sharedAlias")
        Assert-Equal 0 (ConvertTo-FlattenedOpenApiOneOfDocument -Document $source).FlattenedOneOfCount "Reference wrapper was flattened."
    }

    Invoke-Test "a matching union outside a matching parent remains unchanged" {
        $sourceDocument = New-MatchingOneOfDocument
        $wrapper = Copy-JsonValue -Value (Get-TestResponseSchema -Document $sourceDocument).oneOf[1]
        $wrapper.discriminator = [ordered]@{ propertyName = "kind" }
        $null = $wrapper.properties.Remove("sharedAlias")
        $before = ConvertTo-Json -InputObject $wrapper -Compress -Depth 100
        $result = ConvertTo-FlattenedOpenApiOneOfDocument -Document (New-TestOpenApiDocument -Schema $wrapper)
        Assert-Equal 0 $result.FlattenedOneOfCount "Standalone union was flattened."
        Assert-Equal $before (ConvertTo-Json -InputObject (Get-TestResponseSchema $result.Document) -Compress -Depth 100) "Standalone union changed."
    }

    Invoke-Test "multiple and recursively nested wrappers flatten in encounter order" {
        $base = "#/paths/~1test/post/responses/200/content/application~1json/schema"
        $discriminator = [ordered]@{
            propertyName = "kind"
            mapping = [ordered]@{
                first = "$base/oneOf/0/oneOf/0"
                nested = "$base/oneOf/0/oneOf/1/oneOf/1"
                second = "$base/oneOf/1/oneOf/1"
                shifted = "$base/oneOf/2"
            }
        }
        $nested = [ordered]@{
            type = "object"
            discriminator = Copy-JsonValue $discriminator
            oneOf = @((New-OneOfLeaf "b"), (New-OneOfLeaf "c"))
        }
        $first = [ordered]@{
            type = "object"
            discriminator = Copy-JsonValue $discriminator
            oneOf = @((New-OneOfLeaf "a"), $nested)
        }
        $second = [ordered]@{
            type = "object"
            discriminator = Copy-JsonValue $discriminator
            oneOf = @((New-OneOfLeaf "d"), (New-OneOfLeaf "e"))
        }
        $schema = [ordered]@{
            discriminator = $discriminator
            oneOf = @($first, $second, (New-OneOfLeaf "f"))
        }
        $result = ConvertTo-FlattenedOpenApiOneOfDocument -Document (New-TestOpenApiDocument $schema)
        $actual = Get-TestResponseSchema -Document $result.Document
        Assert-Equal 3 $result.FlattenedOneOfCount "Recursive wrapper count was incorrect."
        Assert-Equal 6 $result.PromotedBranchCount "Recursive promotion count was incorrect."
        Assert-Sequence @("a", "b", "c", "d", "e", "f") @(
            $actual.oneOf | ForEach-Object { $_.properties.kind.enum[0] }
        ) "Recursive flattening changed encounter order."
        Assert-Sequence `
            @("$base/oneOf/0", "$base/oneOf/2", "$base/oneOf/4", "$base/oneOf/5") `
            @(
                $actual.discriminator.mapping.first,
                $actual.discriminator.mapping.nested,
                $actual.discriminator.mapping.second,
                $actual.discriminator.mapping.shifted
            ) `
            "Recursive flattening did not relocate cross-level mappings."
    }

    Invoke-Test "escaped pointer tokens relocate promoted branches" {
        $propertyName = "a/b~c"
        $encodedName = "a~1b~0c"
        $base = "#/paths/~1test/post/responses/200/content/application~1json/schema/properties/$encodedName"
        $candidate = New-TransparentOneOfSchema
        $candidate.discriminator["mapping"] = [ordered]@{ inner = "$base/oneOf/1/oneOf/0" }
        $candidate.oneOf[1].discriminator = Copy-JsonValue $candidate.discriminator
        $encodedAliasTarget = "#" + "$base/oneOf/1/oneOf/1".Substring(1).Replace("/", "%2F")
        $root = [ordered]@{
            type = "object"
            properties = [ordered]@{
                $propertyName = $candidate
                alias = [ordered]@{ '$ref' = $encodedAliasTarget }
            }
        }
        $result = ConvertTo-FlattenedOpenApiOneOfDocument -Document (New-TestOpenApiDocument $root)
        $properties = (Get-TestResponseSchema $result.Document).properties
        Assert-Equal "$base/oneOf/1" $properties[$propertyName].discriminator.mapping.inner "Escaped mapping was not relocated."
        Assert-Equal "$base/oneOf/2" $properties.alias.'$ref' "Escaped reference was not relocated."
    }

    Invoke-Test "references to removed wrapper locations fail clearly" {
        $base = "#/paths/~1test/post/responses/200/content/application~1json/schema/oneOf/1"
        $encodedBase = "#" + $base.Substring(1).Replace("/", "%2F")
        foreach ($target in @($base, $encodedBase, "$base/oneOf", "$base/type")) {
            foreach ($referenceName in @('$ref', '$dynamicRef', '$recursiveRef')) {
                $source = New-MatchingOneOfDocument
                $schema = Get-TestResponseSchema -Document $source
                $schema.properties["blocked"] = [ordered]@{}
                $schema.properties.blocked[$referenceName] = $target
                $schema.oneOf[1].properties = Copy-JsonValue $schema.properties
                Assert-Throws {
                    ConvertTo-FlattenedOpenApiOneOfDocument -Document $source
                } "*targets removed wrapper location '$target'*"
            }
        }
    }

    Invoke-Test "postcondition rejects an eligible matching wrapper" {
        Assert-Throws {
            Assert-NoFlattenableOpenApiOneOf -Value (New-MatchingOneOfDocument)
        } "*retained a matching nested oneOf wrapper*"
    }

    Invoke-Test "default and explicit output paths encoding and input preservation are correct" {
        $inputPath = Join-Path $temporaryDirectory "sample.oneof-normalized.openapi.json"
        Copy-Item -LiteralPath (Join-Path $fixtureDirectory "source.openapi.json") -Destination $inputPath
        [byte[]]$inputBefore = [System.IO.File]::ReadAllBytes($inputPath)
        $null = Invoke-OpenApiOneOfFlattener -InputPath $inputPath
        $outputPath = Join-Path $temporaryDirectory "sample.oneof-normalized.oneof-flattened.openapi.json"
        Assert-True ([System.IO.File]::Exists($outputPath)) "Default flattened output was not created."
        Assert-BytesEqual $inputBefore ([System.IO.File]::ReadAllBytes($inputPath)) "Flattener modified its input."
        [byte[]]$outputBytes = [System.IO.File]::ReadAllBytes($outputPath)
        $hasBom = $outputBytes.Length -ge 3 -and $outputBytes[0] -eq 0xEF -and $outputBytes[1] -eq 0xBB -and $outputBytes[2] -eq 0xBF
        Assert-True (-not $hasBom) "Flattened output contains a BOM."
        Assert-Equal 10 $outputBytes[$outputBytes.Length - 1] "Flattened output lacks a trailing LF."

        $explicitPath = Join-Path $temporaryDirectory "explicit-output.json"
        $null = Invoke-OpenApiOneOfFlattener -InputPath $inputPath -OutputPath $explicitPath
        Assert-BytesEqual $outputBytes ([System.IO.File]::ReadAllBytes($explicitPath)) "Explicit output differed from default output."
    }

    Invoke-Test "flattening is byte-idempotent and avoids repeated suffixes" {
        $inputPath = Join-Path $temporaryDirectory "already.oneof-flattened.openapi.json"
        Copy-Item -LiteralPath (Join-Path $fixtureDirectory "expected.oneof-flattened.openapi.json") -Destination $inputPath
        Assert-Equal ([System.IO.Path]::GetFullPath($inputPath)) (Get-DefaultFlattenedOpenApiOneOfPath $inputPath) "Already-flattened default path changed."
        $firstResult = Invoke-OpenApiOneOfFlattener -InputPath $inputPath
        [byte[]]$firstRun = [System.IO.File]::ReadAllBytes($inputPath)
        $secondResult = Invoke-OpenApiOneOfFlattener -InputPath $inputPath
        Assert-Equal 0 $firstResult.FlattenedOneOfCount "Golden output was not already flat."
        Assert-Equal 0 $secondResult.FlattenedOneOfCount "Second pass flattened a wrapper."
        Assert-BytesEqual $firstRun ([System.IO.File]::ReadAllBytes($inputPath)) "Flattening was not byte-idempotent."
    }

    Invoke-Test "missing directory and invalid JSON inputs fail clearly" {
        Assert-Throws {
            Invoke-OpenApiOneOfFlattener -InputPath (Join-Path $temporaryDirectory "missing.json")
        } "*cannot find path*"
        Assert-Throws {
            Invoke-OpenApiOneOfFlattener -InputPath $temporaryDirectory
        } "*is not a file*"

        $invalidPath = Join-Path $temporaryDirectory "invalid.json"
        [System.IO.File]::WriteAllText($invalidPath, "{ invalid", [System.Text.UTF8Encoding]::new($false))
        Assert-Throws {
            Invoke-OpenApiOneOfFlattener -InputPath $invalidPath
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