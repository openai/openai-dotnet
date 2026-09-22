<#
.SYNOPSIS
    Runs focused tests for scripts/Generate-ResponsesOpenApi.ps1.
#>

[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$scriptPath = Join-Path $PSScriptRoot ".." "Generate-ResponsesOpenApi.ps1"
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

function Write-TestSpecification {
    param([string]$Path)
    Copy-Item `
        -LiteralPath (Join-Path $script:OpenApiTestFixtureRoot "Generate-ResponsesOpenApi/source.openapi.json") `
        -Destination $Path
}

function Read-TestJson {
    param([string]$Path)
    return Get-Content -LiteralPath $Path -Raw -Encoding utf8 | ConvertFrom-Json -Depth 100
}

$temporaryDirectory = Join-Path ([System.IO.Path]::GetTempPath()) "generate-responses-openapi-$([Guid]::NewGuid().ToString('N'))"
$null = [System.IO.Directory]::CreateDirectory($temporaryDirectory)

try {
    # Tests and tooling need orchestration helpers without paying for generation
    # as a side effect of loading the script.
    Invoke-Test "dot-sourcing defines orchestration functions without running the pipeline" {
        Assert-True ($null -ne (Get-Command Invoke-ResponsesOpenApiGeneration -ErrorAction SilentlyContinue)) "Generation function was not defined."
        Assert-True ($null -ne (Get-Command Get-ResponsesOpenApiArtifactPaths -ErrorAction SilentlyContinue)) "Artifact path function was not defined."
        Assert-True (-not [System.IO.Directory]::Exists((Join-Path $temporaryDirectory "unexpected"))) "Dot-sourcing unexpectedly generated output."
    }

    # The parameterless production command must read and write the repository's
    # canonical specification/openapi location.
    Invoke-Test "production defaults target specification openapi" {
        $expectedDirectory = Join-Path $script:RepositoryRoot "specification" "openapi"
        Assert-Equal ([System.IO.Path]::GetFullPath($expectedDirectory)) ([System.IO.Path]::GetFullPath($script:DefaultOutputDirectory)) "Default output directory was incorrect."
        Assert-Equal ([System.IO.Path]::GetFullPath((Join-Path $expectedDirectory "openapi.json"))) ([System.IO.Path]::GetFullPath($script:DefaultSpecificationPath)) "Default source path was incorrect."
    }

    # Orchestration invokes stages by filename, so a rename/removal should fail
    # this focused contract test rather than surface during generation.
    Invoke-Test "required stage scripts exist at the expected paths" {
        foreach ($stageScriptName in @(
            "Extract-OpenApiRoute.ps1",
            "Flatten-OpenApiAllOf.ps1",
            "Simplify-OpenApiStringVariants.ps1",
            "Normalize-OpenApiOneOf.ps1",
            "Sort-OpenApiDocument.ps1"
        )) {
            $stageScriptPath = Join-Path (Split-Path $scriptPath -Parent) $stageScriptName
            Assert-True (Test-Path -LiteralPath $stageScriptPath -PathType Leaf) "Required stage script was not found: $stageScriptPath"
        }
    }

    # Intermediate filenames are an audit trail of the pipeline, while the final
    # consumer-facing artifact intentionally has the short stable name.
    Invoke-Test "artifact paths retain every stage and use the short final name" {
        $outputDirectory = Join-Path $temporaryDirectory "artifact-paths"
        $paths = Get-ResponsesOpenApiArtifactPaths -OutputDirectory $outputDirectory
        Assert-Sequence @(
            "responses.inlined.openapi.json",
            "responses.inlined.flattened.openapi.json",
            "responses.inlined.flattened.simplified.openapi.json",
            "responses.inlined.flattened.simplified.oneof-normalized.openapi.json",
            "responses.inlined.flattened.simplified.oneof-normalized.sorted.openapi.json",
            "responses.openapi.json"
        ) @(
            [System.IO.Path]::GetFileName($paths.Inlined),
            [System.IO.Path]::GetFileName($paths.Flattened),
            [System.IO.Path]::GetFileName($paths.Simplified),
            [System.IO.Path]::GetFileName($paths.OneOfNormalized),
            [System.IO.Path]::GetFileName($paths.Sorted),
            [System.IO.Path]::GetFileName($paths.Final)
        ) "Artifact names were incorrect."
    }

    # One compact end-to-end test owns stage ordering, intermediate persistence,
    # semantic handoffs, source immutability, and final alias identity.
    Invoke-Test "pipeline persists every stage and creates a byte-identical final alias" {
        $sourcePath = Join-Path $temporaryDirectory "openapi.json"
        $outputDirectory = Join-Path $temporaryDirectory "generated"
        Write-TestSpecification -Path $sourcePath
        [byte[]]$sourceBefore = [System.IO.File]::ReadAllBytes($sourcePath)
        $paths = Invoke-ResponsesOpenApiGeneration `
            -SpecificationPath $sourcePath `
            -OutputDirectory $outputDirectory

        foreach ($artifactPath in @(
            $paths.Inlined,
            $paths.Flattened,
            $paths.Simplified,
            $paths.OneOfNormalized,
            $paths.Sorted,
            $paths.Final
        )) {
            Assert-True ([System.IO.File]::Exists($artifactPath)) "Artifact was not created: $artifactPath"
        }
        Assert-True ([System.Linq.Enumerable]::SequenceEqual($sourceBefore, [System.IO.File]::ReadAllBytes($sourcePath))) "Pipeline modified its source specification."
        Assert-True ([System.Linq.Enumerable]::SequenceEqual([System.IO.File]::ReadAllBytes($paths.Sorted), [System.IO.File]::ReadAllBytes($paths.Final))) "Final output differs from the retained Sort-stage artifact."

        $inlined = Read-TestJson -Path $paths.Inlined
        $flattened = Read-TestJson -Path $paths.Flattened
        $simplified = Read-TestJson -Path $paths.Simplified
        $oneOfNormalized = Read-TestJson -Path $paths.OneOfNormalized
        $sorted = Read-TestJson -Path $paths.Sorted
        Assert-Sequence @("/responses") @($inlined.paths.PSObject.Properties.Name) "Extract stage retained unrelated routes."
        $inlinedSchema = $inlined.paths.'/responses'.post.requestBody.content.'application/json'.schema
        $flattenedSchema = $flattened.paths.'/responses'.post.requestBody.content.'application/json'.schema
        Assert-True ($inlinedSchema.PSObject.Properties.Name -ccontains "allOf") "Extract stage did not retain allOf."
        Assert-True (-not ($flattenedSchema.PSObject.Properties.Name -ccontains "allOf")) "Flatten stage retained allOf."

        $simplifiedModel = $simplified.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.model
        $simplifiedOutputs = $simplified.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.outputs
        $simplifiedEvents = $simplified.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.events
        $normalizedOutputs = $oneOfNormalized.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.outputs
        $normalizedEvents = $oneOfNormalized.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.events
        $sortedModel = $sorted.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.model
        $sortedEvents = $sorted.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.events
        Assert-Equal 2 $simplifiedModel.anyOf.Count "Simplify stage emitted the wrong branch count."
        Assert-Sequence @("zeta", "alpha", "middle") @($simplifiedModel.anyOf[1].enum) "Simplify stage did not preserve encounter order."
        Assert-True ($simplifiedOutputs.PSObject.Properties.Name -ccontains "discriminator") "Simplify stage removed the orphan before Normalize."
        Assert-True ($simplifiedEvents.PSObject.Properties.Name -ccontains "anyOf") "Simplify stage promoted the union before Normalize."
        Assert-True (-not ($normalizedOutputs.PSObject.Properties.Name -ccontains "discriminator")) "Normalize stage retained the outer orphan."
        Assert-True ($normalizedOutputs.items.PSObject.Properties.Name -ccontains "discriminator") "Normalize stage removed the nested paired discriminator."
        Assert-True (-not ($normalizedEvents.PSObject.Properties.Name -ccontains "anyOf")) "Normalize stage retained the qualifying anyOf."
        Assert-True ($normalizedEvents.PSObject.Properties.Name -ccontains "oneOf") "Normalize stage did not emit oneOf."
        Assert-True ($normalizedEvents.PSObject.Properties.Name -ccontains "discriminator") "Normalize stage removed the promoted discriminator."
        Assert-True ($sortedEvents.PSObject.Properties.Name -ccontains "oneOf") "Sort stage lost the promoted oneOf."
        Assert-Sequence @("alpha", "middle", "zeta") @($sortedModel.anyOf[1].enum) "Sort stage did not canonicalize the enum."
    }

    # Final publication must replace stale bytes atomically without requiring a
    # second expensive pipeline run in this test.
    Invoke-Test "atomic copy replaces a stale final alias" {
        $sourcePath = Join-Path $temporaryDirectory "copy-source.json"
        $finalPath = Join-Path $temporaryDirectory "responses.openapi.json"
        [System.IO.File]::WriteAllText($sourcePath, "fresh", [System.Text.UTF8Encoding]::new($false))
        [System.IO.File]::WriteAllText($finalPath, "stale", [System.Text.UTF8Encoding]::new($false))
        $writtenPath = Copy-OpenApiFileAtomically `
            -SourcePath $sourcePath `
            -DestinationPath $finalPath
        Assert-Equal ([System.IO.Path]::GetFullPath($finalPath)) $writtenPath "Atomic copy returned the wrong path."
        Assert-True ([System.Linq.Enumerable]::SequenceEqual([System.IO.File]::ReadAllBytes($sourcePath), [System.IO.File]::ReadAllBytes($finalPath))) "Stale final alias was not atomically replaced."
    }

    # Orchestration should validate its source before launching any stage or
    # creating a partial artifact chain.
    Invoke-Test "missing and non-file specifications fail clearly" {
        Assert-Throws {
            Invoke-ResponsesOpenApiGeneration `
                -SpecificationPath (Join-Path $temporaryDirectory "missing.json") `
                -OutputDirectory (Join-Path $temporaryDirectory "missing-output")
        } "*cannot find path*"
        Assert-Throws {
            Invoke-ResponsesOpenApiGeneration `
                -SpecificationPath $temporaryDirectory `
                -OutputDirectory (Join-Path $temporaryDirectory "directory-output")
        } "*is not a file*"
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