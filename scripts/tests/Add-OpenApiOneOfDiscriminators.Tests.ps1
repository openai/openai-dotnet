<#
.SYNOPSIS
    Runs focused tests for scripts/Add-OpenApiOneOfDiscriminators.ps1.
#>

[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$scriptDirectory = Join-Path $PSScriptRoot ".."
$adderScriptPath = Join-Path $scriptDirectory "Add-OpenApiOneOfDiscriminators.ps1"
. $adderScriptPath
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
    if (-not (Test-JsonValuesEqual -Left $Expected -Right $Actual)) {
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

$fixtureDirectory = Join-Path $PSScriptRoot "fixtures" "Add-OpenApiOneOfDiscriminators"
$sourceFixturePath = Join-Path $fixtureDirectory "source.openapi.json"
$expectedFixturePath = Join-Path $fixtureDirectory "expected.oneof-discriminated.openapi.json"
$schemaCases = Read-TestJsonFixture -RelativePath "Add-OpenApiOneOfDiscriminators/cases.schema.json"
$temporaryDirectory = Join-Path ([System.IO.Path]::GetTempPath()) "add-openapi-oneof-discriminators-$([Guid]::NewGuid().ToString('N'))"
$null = [System.IO.Directory]::CreateDirectory($temporaryDirectory)

try {
    # Focused tests load the stage as a library; dot-sourcing must not invoke
    # file generation.
    Invoke-Test "dot-sourcing defines discriminator functions without running the CLI" {
        Assert-True ($null -ne (Get-Command ConvertTo-OneOfDiscriminatedOpenApiDocument -ErrorAction SilentlyContinue)) "Conversion function was not defined."
        Assert-True ($null -ne (Get-Command Invoke-OpenApiOneOfDiscriminatorAdder -ErrorAction SilentlyContinue)) "CLI function was not defined."
    }

    # The production-like format union permits arbitrary extra branch properties
    # as long as each branch requires finite, disjoint string type values.
    Invoke-Test "eligible object oneOf gains a direct type discriminator" {
        $schema = Get-TestFixtureCase -Cases $schemaCases -Name "eligible"
        $candidate = Get-OpenApiOneOfDiscriminatorCandidate -Schema $schema
        Assert-True ($null -ne $candidate) "Eligible schema was not detected."
        Assert-Sequence @("first", "second") @($candidate.EnumValues) "Candidate enum values were incorrect."
        $source = New-TestOpenApiDocument -Schema $schema
        $sourceBefore = ConvertTo-Json -InputObject $source -Compress -Depth 100
        $result = ConvertTo-OneOfDiscriminatedOpenApiDocument -Document $source
        $converted = Get-TestResponseSchema -Document $result.Document
        Assert-Equal 1 $result.AddedDiscriminatorCount "Added discriminator count was incorrect."
        Assert-Equal "type" $converted.discriminator.propertyName "Discriminator propertyName was incorrect."
        Assert-Sequence @("discriminator", "oneOf") @((Get-JsonProperties -Value $converted).Name) "Discriminator was not inserted immediately before oneOf."
        Assert-Equal $sourceBefore (ConvertTo-Json -InputObject $source -Compress -Depth 100) "Conversion mutated its input."
    }

    # Finite routing values may come from const, multi-value enum, or their
    # intersection. Every accepted value must remain disjoint across branches.
    Invoke-Test "const and multi-value enum branches are eligible" {
        $schema = [ordered]@{
            oneOf = @(
                [ordered]@{
                    type = "object"
                    required = @("type")
                    properties = [ordered]@{
                        type = [ordered]@{ type = "string"; const = "first" }
                    }
                },
                [ordered]@{
                    type = "object"
                    required = @("type")
                    properties = [ordered]@{
                        type = [ordered]@{ type = "string"; enum = @("second", "third") }
                    }
                },
                [ordered]@{
                    type = "object"
                    required = @("type")
                    properties = [ordered]@{
                        type = [ordered]@{ type = "string"; const = "fourth"; enum = @("fourth", "unused") }
                    }
                }
            )
        }
        $candidate = Get-OpenApiOneOfDiscriminatorCandidate -Schema $schema
        Assert-True ($null -ne $candidate) "Finite const/enum schema was not detected."
        Assert-Sequence @("first", "second", "third", "fourth") @($candidate.EnumValues) "Finite discriminator values were incorrect."
        $result = ConvertTo-OneOfDiscriminatedOpenApiDocument -Document (New-TestOpenApiDocument -Schema $schema)
        Assert-Equal 1 $result.AddedDiscriminatorCount "Finite const/enum discriminator was not added."
    }

    # A compatible nested discriminated oneOf contributes its leaf values to the
    # outer union. The outer discriminator must survive so the flattening stage
    # can promote those leaves before mappings are materialized.
    Invoke-Test "existing discriminator accepts compatible nested oneOf branches" {
        $nested = [ordered]@{
            type = "object"
            discriminator = [ordered]@{ propertyName = "type" }
            oneOf = @(
                [ordered]@{
                    type = "object"
                    required = @("type")
                    properties = [ordered]@{
                        type = [ordered]@{ type = "string"; enum = @("nested_a", "nested_alias") }
                    }
                },
                [ordered]@{
                    type = "object"
                    required = @("type")
                    properties = [ordered]@{
                        type = [ordered]@{ type = "string"; const = "nested_b" }
                    }
                }
            )
        }
        $schema = [ordered]@{
            discriminator = [ordered]@{ propertyName = "type" }
            oneOf = @(
                $nested,
                [ordered]@{
                    type = "object"
                    required = @("type")
                    properties = [ordered]@{
                        type = [ordered]@{ type = "string"; enum = @("outer") }
                    }
                }
            )
        }

        Assert-True (Test-IsValidInlineOpenApiOneOfDiscriminator -Schema $schema) "Compatible nested discriminator was considered invalid."
        $candidate = Get-OpenApiOneOfDiscriminatorCandidate -Schema $schema -IgnoreExistingDiscriminator
        Assert-Sequence @("nested_a", "nested_alias", "nested_b", "outer") @($candidate.EnumValues) "Recursive discriminator values were incorrect."
        $source = New-TestOpenApiDocument -Schema $schema
        $before = ConvertTo-Json -InputObject $source -Compress -Depth 100
        $result = ConvertTo-OneOfDiscriminatedOpenApiDocument -Document $source
        Assert-Equal 0 $result.RemovedInvalidDiscriminatorCount "Compatible outer discriminator was removed."
        Assert-Equal $before (ConvertTo-Json -InputObject $result.Document -Compress -Depth 100) "Compatible nested discriminator schema changed."

        $inferred = Copy-JsonValue -Value $schema
        $null = $inferred.Remove("discriminator")
        $inferredResult = ConvertTo-OneOfDiscriminatedOpenApiDocument `
            -Document (New-TestOpenApiDocument -Schema $inferred)
        Assert-Equal 1 $inferredResult.AddedDiscriminatorCount "Compatible outer discriminator was not inferred."
        Assert-Equal "type" (Get-TestResponseSchema $inferredResult.Document).discriminator.propertyName "Inferred outer discriminator was incorrect."
    }

    # Composition-only wrappers do not need to repeat type=object when every
    # recursively collected leaf is an explicit required object alternative.
    Invoke-Test "composition-only wrappers infer one parent discriminator" {
        $leaf = {
            param([string]$Value)
            [ordered]@{
                type = "object"
                required = @("type")
                properties = [ordered]@{
                    type = [ordered]@{ type = "string"; const = $Value }
                }
            }
        }
        $schema = [ordered]@{
            oneOf = @(
                [ordered]@{
                    discriminator = [ordered]@{ propertyName = "type" }
                    oneOf = @(
                        (& $leaf "input_text")
                        (& $leaf "input_image")
                        (& $leaf "input_file")
                    )
                },
                [ordered]@{
                    discriminator = [ordered]@{ propertyName = "type" }
                    oneOf = @(
                        (& $leaf "output_text")
                        (& $leaf "refusal")
                    )
                }
            )
        }

        $candidate = Get-OpenApiOneOfDiscriminatorCandidate -Schema $schema
        Assert-Sequence `
            @("input_text", "input_image", "input_file", "output_text", "refusal") `
            @($candidate.EnumValues) `
            "Composition-only wrapper values were incorrect."
        $result = ConvertTo-OneOfDiscriminatedOpenApiDocument `
            -Document (New-TestOpenApiDocument -Schema $schema)
        $converted = Get-TestResponseSchema $result.Document
        Assert-Equal 1 $result.AddedDiscriminatorCount "Parent discriminator was not inferred exactly once."
        Assert-Equal "type" $converted.discriminator.propertyName "Parent discriminator property was incorrect."
        Assert-Equal 2 $converted.oneOf.Count "Inference flattened wrappers before the flatten stage."
        Assert-Equal 3 $converted.oneOf[0].oneOf.Count "Input wrapper changed during inference."
        Assert-Equal 2 $converted.oneOf[1].oneOf.Count "Output wrapper changed during inference."
    }

    Invoke-Test "explicit non-object wrapper types remain ineligible" {
        foreach ($case in @(
            [PSCustomObject]@{ Name = "string"; Value = "string" },
            [PSCustomObject]@{ Name = "array"; Value = "array" },
            [PSCustomObject]@{ Name = "null"; Value = "null" },
            [PSCustomObject]@{ Name = "type array"; Value = [object[]]@("object", "null") },
            [PSCustomObject]@{ Name = "explicit null"; Value = $null }
        )) {
            $wrapper = [ordered]@{
                type = $case.Value
                discriminator = [ordered]@{ propertyName = "type" }
                oneOf = @(
                    [ordered]@{
                        type = "object"
                        required = @("type")
                        properties = [ordered]@{
                            type = [ordered]@{ type = "string"; const = "first" }
                        }
                    }
                )
            }
            $schema = [ordered]@{
                oneOf = @(
                    $wrapper,
                    [ordered]@{
                        discriminator = [ordered]@{ propertyName = "type" }
                        oneOf = @(
                            [ordered]@{
                                type = "object"
                                required = @("type")
                                properties = [ordered]@{
                                    type = [ordered]@{ type = "string"; const = "second" }
                                }
                            }
                        )
                    }
                )
            }
            Assert-True `
                ($null -eq (Get-OpenApiOneOfDiscriminatorCandidate -Schema $schema)) `
                "Explicit wrapper type '$($case.Name)' was considered eligible."
        }
    }

    Invoke-Test "duplicate values across nested and outer branches reject the discriminator" {
        $nested = [ordered]@{
            type = "object"
            discriminator = [ordered]@{ propertyName = "type" }
            oneOf = @(
                [ordered]@{
                    type = "object"
                    required = @("type")
                    properties = [ordered]@{
                        type = [ordered]@{ type = "string"; const = "duplicate" }
                    }
                }
            )
        }
        $schema = [ordered]@{
            discriminator = [ordered]@{ propertyName = "type" }
            oneOf = @(
                $nested,
                [ordered]@{
                    type = "object"
                    required = @("type")
                    properties = [ordered]@{
                        type = [ordered]@{ type = "string"; enum = @("duplicate") }
                    }
                }
            )
        }

        Assert-True (-not (Test-IsValidInlineOpenApiOneOfDiscriminator -Schema $schema)) "Cross-level duplicate was considered valid."
        Assert-True ($null -eq (Get-OpenApiOneOfDiscriminatorCandidate -Schema $schema -IgnoreExistingDiscriminator)) "Cross-level duplicate was inferable."
        $result = ConvertTo-OneOfDiscriminatedOpenApiDocument `
            -Document (New-TestOpenApiDocument -Schema $schema)
        $converted = Get-TestResponseSchema $result.Document
        Assert-Equal 1 $result.RemovedInvalidDiscriminatorCount "Ambiguous outer discriminator was not removed."
        Assert-True (-not (Test-JsonProperty -Object $converted -Name "discriminator")) "Ambiguous outer discriminator remained."
    }

    # OpenAPI requires the discriminator field in every alternative. Optional
    # properties are left undiscriminated rather than producing invalid metadata.
    Invoke-Test "optional discriminator properties remain undiscriminated" {
        $schema = Get-TestFixtureCase -Cases $schemaCases -Name "eligible"
        $null = $schema.oneOf[1].Remove("required")
        Assert-True ($null -eq (Get-OpenApiOneOfDiscriminatorCandidate -Schema $schema)) "Optional discriminator property was considered eligible."
        $source = New-TestOpenApiDocument -Schema $schema
        $before = ConvertTo-Json -InputObject $source -Compress -Depth 100
        $result = ConvertTo-OneOfDiscriminatedOpenApiDocument -Document $source
        Assert-Equal 0 $result.AddedDiscriminatorCount "Optional discriminator received metadata."
        Assert-Equal $before (ConvertTo-Json -InputObject $result.Document -Compress -Depth 100) "Optional discriminator schema changed."
    }

    # Nested independent candidates are transformed in one immutable traversal.
    Invoke-Test "nested candidates are added independently" {
        $source = Read-OpenApiDocument -Path $sourceFixturePath
        $result = ConvertTo-OneOfDiscriminatedOpenApiDocument -Document $source
        $properties = (Get-TestResponseSchema -Document $result.Document).properties
        Assert-Equal 2 $result.AddedDiscriminatorCount "Nested candidate count was incorrect."
        Assert-Equal "type" $properties.format.discriminator.propertyName "Outer candidate was not discriminated."
        Assert-Equal "type" $properties.nested.properties.choice.discriminator.propertyName "Nested candidate was not discriminated."
    }

    # Existing valid metadata is preserved, while malformed or non-inferable
    # discriminators are removed without changing their oneOf validation.
    Invoke-Test "existing discriminators are validated and reconciled" {
        $valid = [ordered]@{
            discriminator = [ordered]@{ propertyName = "kind" }
            oneOf = @(
                [ordered]@{
                    type = "object"
                    required = @("kind")
                    properties = [ordered]@{ kind = [ordered]@{ type = "string"; const = "first" } }
                },
                [ordered]@{
                    type = "object"
                    required = @("kind")
                    properties = [ordered]@{ kind = [ordered]@{ type = "string"; enum = @("second") } }
                }
            )
        }
        $validSource = New-TestOpenApiDocument -Schema $valid
        $validBefore = ConvertTo-Json -InputObject $validSource -Compress -Depth 100
        $validResult = ConvertTo-OneOfDiscriminatedOpenApiDocument -Document $validSource
        Assert-Equal 0 $validResult.AddedDiscriminatorCount "Valid existing discriminator was replaced."
        Assert-Equal 0 $validResult.RemovedInvalidDiscriminatorCount "Valid existing discriminator was removed."
        Assert-Equal $validBefore (ConvertTo-Json -InputObject $validResult.Document -Compress -Depth 100) "Valid existing discriminator changed."

        foreach ($value in @(
            [ordered]@{ propertyName = "kind" },
            "malformed",
            $null
        )) {
            $schema = Get-TestFixtureCase -Cases $schemaCases -Name "eligible"
            $schema["discriminator"] = $value
            $source = New-TestOpenApiDocument -Schema $schema
            $result = ConvertTo-OneOfDiscriminatedOpenApiDocument -Document $source
            $converted = Get-TestResponseSchema -Document $result.Document
            Assert-Equal 1 $result.RemovedInvalidDiscriminatorCount "Invalid existing discriminator was not removed."
            Assert-Equal 1 $result.AddedDiscriminatorCount "Valid inferred discriminator did not replace invalid metadata."
            Assert-Equal "type" $converted.discriminator.propertyName "Invalid discriminator was not replaced with inferred type metadata."
            Assert-True (Test-JsonProperty -Object $converted -Name "oneOf") "Removing invalid metadata changed oneOf validation."
        }
    }

    # Duplicate discriminator values make branch selection ambiguous and are
    # deliberately skipped instead of producing misleading metadata.
    Invoke-Test "duplicate singleton enum values remain undiscriminated" {
        $source = New-TestOpenApiDocument -Schema (
            Get-TestFixtureCase -Cases $schemaCases -Name "duplicateEnums"
        )
        $before = ConvertTo-Json -InputObject $source -Compress -Depth 100
        $result = ConvertTo-OneOfDiscriminatedOpenApiDocument -Document $source
        Assert-Equal 0 $result.AddedDiscriminatorCount "Ambiguous union received a discriminator."
        Assert-Equal $before (ConvertTo-Json -InputObject $result.Document -Compress -Depth 100) "Ambiguous union changed."
    }

    # Every exact inference gate is conservative, invalid existing metadata is
    # removed, and example/literal objects remain outside schema traversal.
    Invoke-Test "near misses are preserved while invalid metadata is removed" {
        $source = New-TestOpenApiDocument -Schema (
            Get-TestFixtureCase -Cases $schemaCases -Name "nearMisses"
        )
        $result = ConvertTo-OneOfDiscriminatedOpenApiDocument -Document $source
        Assert-Equal 0 $result.AddedDiscriminatorCount "A near miss received a discriminator."
        Assert-Equal 2 $result.RemovedInvalidDiscriminatorCount "Invalid near-miss discriminators were not removed."
        $converted = Get-TestResponseSchema -Document $result.Document
        Assert-True (-not (Test-JsonProperty -Object $converted.properties.alreadyObject -Name "discriminator")) "Invalid object discriminator remained."
        Assert-True (-not (Test-JsonProperty -Object $converted.properties.alreadyScalar -Name "discriminator")) "Invalid scalar discriminator remained."
        Assert-True (Test-JsonProperty -Object $converted.properties.alreadyObject -Name "oneOf") "Near-miss oneOf changed."
        Assert-True (Test-JsonProperty -Object $converted -Name "example") "Literal example was removed."
        Assert-True (Test-JsonProperty -Object $converted.example -Name "oneOf") "Literal oneOf lookalike changed."
    }

    # The independent postcondition catches traversal regressions that would
    # silently leave a stage-owned candidate in output.
    Invoke-Test "postcondition rejects an eligible undiscriminated oneOf" {
        $source = New-TestOpenApiDocument -Schema (
            Get-TestFixtureCase -Cases $schemaCases -Name "eligible"
        )
        Assert-Throws {
            Assert-NoUndiscriminatedOpenApiOneOfCandidates -Value $source
        } "*retained an eligible undiscriminated oneOf*"
    }

    # The compact golden locks nested traversal, existing discriminator
    # protection, literal preservation, insertion order, and statistics.
    Invoke-Test "compact fixture matches its oneOf-discriminated golden" {
        $source = Read-OpenApiDocument -Path $sourceFixturePath
        $expected = Read-OpenApiDocument -Path $expectedFixturePath
        $result = ConvertTo-OneOfDiscriminatedOpenApiDocument -Document $source
        Assert-Equal 2 $result.AddedDiscriminatorCount "Golden discriminator count was incorrect."
        Assert-Equal 1 $result.RemovedInvalidDiscriminatorCount "Golden invalid discriminator removal count was incorrect."
        Assert-JsonEqual $expected $result.Document "oneOf-discriminated fixture did not match its golden."
    }

    # Adding metadata does not move schemas or change local reference targets.
    Invoke-Test "local references remain unchanged and resolvable" {
        $source = Read-TestJsonFixture `
            -RelativePath "Add-OpenApiOneOfDiscriminators/source.openapi.json"
        $source.paths["/test"].post.responses["200"].content["application/json"].schema.properties["alias"] = [ordered]@{
            '$ref' = "#/paths/~1test/post/responses/200/content/application~1json/schema/properties/format/oneOf/0"
        }
        $result = ConvertTo-OneOfDiscriminatedOpenApiDocument -Document $source
        $alias = (Get-TestResponseSchema -Document $result.Document).properties.alias
        Assert-Equal "#/paths/~1test/post/responses/200/content/application~1json/schema/properties/format/oneOf/0" $alias.'$ref' "Reference changed."
        Assert-OpenApiReferencesResolve -Document $result.Document
    }

    # Completed output is a structural no-op on a second pass.
    Invoke-Test "conversion is structurally idempotent" {
        $source = Read-OpenApiDocument -Path $sourceFixturePath
        $first = ConvertTo-OneOfDiscriminatedOpenApiDocument -Document $source
        $second = ConvertTo-OneOfDiscriminatedOpenApiDocument -Document $first.Document
        Assert-Equal 0 $second.AddedDiscriminatorCount "Second pass added a discriminator."
        Assert-JsonEqual $first.Document $second.Document "Second pass changed the document."
    }

    # Public output naming inserts the marker once for OpenAPI and ordinary JSON
    # files.
    Invoke-Test "default output paths insert the oneOf-discriminated marker once" {
        $ordinary = Join-Path $temporaryDirectory "sample.openapi.json"
        $expected = Join-Path $temporaryDirectory "sample.oneof-discriminated.openapi.json"
        Assert-Equal ([System.IO.Path]::GetFullPath($expected)) (Get-DefaultOneOfDiscriminatedOpenApiPath -InputPath $ordinary) "OpenAPI output path was incorrect."
        Assert-Equal ([System.IO.Path]::GetFullPath($expected)) (Get-DefaultOneOfDiscriminatedOpenApiPath -InputPath $expected) "Marker repeated."
        Assert-Equal ([System.IO.Path]::GetFullPath((Join-Path $temporaryDirectory "sample.oneof-discriminated.json"))) (Get-DefaultOneOfDiscriminatedOpenApiPath -InputPath (Join-Path $temporaryDirectory "sample.json")) "Plain JSON output path was incorrect."
    }

    # CLI output is deterministic UTF-8, preserves the source, and honors both
    # default and explicit destinations.
    Invoke-Test "CLI default and explicit outputs are deterministic and preserve input" {
        $inputPath = Join-Path $temporaryDirectory "sample.openapi.json"
        Copy-Item -LiteralPath $sourceFixturePath -Destination $inputPath
        [byte[]]$inputBefore = [System.IO.File]::ReadAllBytes($inputPath)
        $null = Invoke-OpenApiOneOfDiscriminatorAdder -InputPath $inputPath
        $defaultOutput = Join-Path $temporaryDirectory "sample.oneof-discriminated.openapi.json"
        Assert-True ([System.IO.File]::Exists($defaultOutput)) "Default output was not created."
        Assert-BytesEqual $inputBefore ([System.IO.File]::ReadAllBytes($inputPath)) "Adder modified its input."
        [byte[]]$defaultBytes = [System.IO.File]::ReadAllBytes($defaultOutput)
        $hasBom = $defaultBytes.Length -ge 3 -and $defaultBytes[0] -eq 0xEF -and $defaultBytes[1] -eq 0xBB -and $defaultBytes[2] -eq 0xBF
        Assert-True (-not $hasBom) "Output contains a UTF-8 BOM."
        Assert-Equal 10 $defaultBytes[$defaultBytes.Length - 1] "Output lacks a trailing LF."

        $explicitOutput = Join-Path $temporaryDirectory "explicit.json"
        $null = Invoke-OpenApiOneOfDiscriminatorAdder `
            -InputPath $inputPath `
            -OutputPath $explicitOutput
        Assert-BytesEqual $defaultBytes ([System.IO.File]::ReadAllBytes($explicitOutput)) "Explicit output differed from default output."
    }

    # An already-processed artifact rewrites in place without byte churn.
    Invoke-Test "CLI addition is byte-idempotent in place" {
        $inputPath = Join-Path $temporaryDirectory "already.oneof-discriminated.openapi.json"
        Copy-Item -LiteralPath $expectedFixturePath -Destination $inputPath
        $null = Invoke-OpenApiOneOfDiscriminatorAdder -InputPath $inputPath
        [byte[]]$firstRun = [System.IO.File]::ReadAllBytes($inputPath)
        $second = Invoke-OpenApiOneOfDiscriminatorAdder -InputPath $inputPath
        Assert-Equal 0 $second.AddedDiscriminatorCount "Second in-place pass added a discriminator."
        Assert-BytesEqual $firstRun ([System.IO.File]::ReadAllBytes($inputPath)) "In-place output changed bytes."
    }

    # Input validation belongs at the command boundary and must fail before any
    # partial artifact is published.
    Invoke-Test "missing directory and invalid JSON inputs fail clearly" {
        Assert-Throws {
            Invoke-OpenApiOneOfDiscriminatorAdder -InputPath (Join-Path $temporaryDirectory "missing.json")
        } "*cannot find path*"
        Assert-Throws {
            Invoke-OpenApiOneOfDiscriminatorAdder -InputPath $temporaryDirectory
        } "*is not a file*"

        $invalidPath = Join-Path $temporaryDirectory "invalid.json"
        [System.IO.File]::WriteAllText($invalidPath, "{ invalid", [System.Text.UTF8Encoding]::new($false))
        Assert-Throws {
            Invoke-OpenApiOneOfDiscriminatorAdder -InputPath $invalidPath
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
