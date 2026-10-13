<#
.SYNOPSIS
    Runs focused tests for scripts/specification/Inherit-OpenApi.ps1.
#>

[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$scriptPath = Join-Path $PSScriptRoot ".." "Inherit-OpenApi.ps1"
. $scriptPath
. (Join-Path $PSScriptRoot ".." "pipeline" "_OpenApiDocument.Helpers.ps1")

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

function New-InheritTopLevelTestDocument {
    return [ordered]@{
        openapi = "3.1.0"
        info = [ordered]@{ title = "test"; version = "1.0.0" }
        paths = [ordered]@{
            "/test" = [ordered]@{
                get = [ordered]@{
                    responses = [ordered]@{
                        "200" = [ordered]@{ description = "ok" }
                    }
                }
            }
        }
        components = [ordered]@{
            schemas = [ordered]@{
                Pet = [ordered]@{
                    discriminator = [ordered]@{
                        propertyName = "kind"
                        mapping = [ordered]@{ cat = "#/components/schemas/Cat" }
                    }
                    oneOf = @([ordered]@{ '$ref' = "#/components/schemas/Cat" })
                }
                Cat = [ordered]@{
                    type = "object"
                    required = @("kind")
                    properties = [ordered]@{
                        kind = [ordered]@{ type = "string"; enum = @("cat") }
                    }
                }
            }
        }
    }
}

$temporaryDirectory = Join-Path ([System.IO.Path]::GetTempPath()) "inherit-openapi-$([Guid]::NewGuid().ToString('N'))"
$null = [System.IO.Directory]::CreateDirectory($temporaryDirectory)
$featureArea = "responses"

try {
    Invoke-Test "dot-sourcing defines inheritance without running it" {
        Assert-True ($null -ne (Get-Command Invoke-OpenApiInheritance -ErrorAction SilentlyContinue)) "Inheritance function was not defined."
        Assert-True ($null -ne (Get-Command Publish-OpenApiArtifact -ErrorAction SilentlyContinue)) "Shared publisher was not defined."
        Assert-True (-not [System.IO.Directory]::Exists((Join-Path $temporaryDirectory "unexpected"))) "Dot-sourcing unexpectedly ran inheritance."
    }

    Invoke-Test "production defaults and inheritance stages are correct" {
        $expectedDirectory = Join-Path $script:RepositoryRoot "specification" "openapi"
        Assert-Equal ([System.IO.Path]::GetFullPath($expectedDirectory)) ([System.IO.Path]::GetFullPath($script:DefaultOutputRoot)) "Default output root was incorrect."
        foreach ($stageScriptName in @("Refactor-OpenApiDiscriminatedInheritance.ps1", "Sort-OpenApiDocument.ps1")) {
            $stagePath = Join-Path $script:PipelineDirectory $stageScriptName
            Assert-True (Test-Path -LiteralPath $stagePath -PathType Leaf) "Required stage script was not found: $stagePath"
        }
        $paths = Get-OpenApiArtifactPaths -FeatureArea $featureArea -OutputRoot $expectedDirectory
        Assert-Equal "responses.14.discriminator-inheritance-refactored.openapi.json" ([System.IO.Path]::GetFileName($paths.DiscriminatorInherited)) "Inherited artifact name was incorrect."
        Assert-Equal "responses.15.inherited-document-sorted.openapi.json" ([System.IO.Path]::GetFileName($paths.DiscriminatorInheritedSorted)) "Sorted inherited artifact name was incorrect."
    }

    Invoke-Test "default alias input produces retained artifacts and inherited alias" {
        $outputRoot = Join-Path $temporaryDirectory "default"
        $paths = Get-OpenApiArtifactPaths -FeatureArea $featureArea -OutputRoot $outputRoot
        $null = [System.IO.Directory]::CreateDirectory($paths.OutputDirectory)
        $null = Write-OpenApiJsonFile -Document (New-InheritTopLevelTestDocument) -Path $paths.Final
        foreach ($downstreamPath in @($paths.DefaultComponentMap, $paths.PolicyComponentMap, $paths.Deduplicated)) {
            [System.IO.File]::WriteAllText($downstreamPath, "sentinel", [System.Text.UTF8Encoding]::new($false))
        }

        $result = Invoke-OpenApiInheritance -FeatureArea $featureArea -OutputRoot $outputRoot 6>$null
        foreach ($artifactPath in @($result.DiscriminatorInherited, $result.DiscriminatorInheritedSorted, $result.Final)) {
            Assert-True ([System.IO.File]::Exists($artifactPath)) "Inheritance artifact was not created: $artifactPath"
        }
        Assert-BytesEqual ([System.IO.File]::ReadAllBytes($result.DiscriminatorInheritedSorted)) ([System.IO.File]::ReadAllBytes($result.Final)) "Published alias differs from inherited-sorted output."
        $document = Get-Content -LiteralPath $result.Final -Raw -Encoding utf8 | ConvertFrom-Json -AsHashtable -Depth 100
        Assert-True (-not $document.components.schemas.Pet.Contains("oneOf")) "Published base retained oneOf."
        Assert-Equal "#/components/schemas/Pet" $document.components.schemas.Cat.allOf[0].'$ref' "Published child did not inherit its base."
        foreach ($downstreamPath in @($paths.DefaultComponentMap, $paths.PolicyComponentMap, $paths.Deduplicated)) {
            Assert-Equal "sentinel" ([System.IO.File]::ReadAllText($downstreamPath)) "Inheritance modified downstream artifact '$downstreamPath'."
        }

        [byte[]]$inheritedBytes = [System.IO.File]::ReadAllBytes($result.DiscriminatorInherited)
        [byte[]]$sortedBytes = [System.IO.File]::ReadAllBytes($result.DiscriminatorInheritedSorted)
        [byte[]]$aliasBytes = [System.IO.File]::ReadAllBytes($result.Final)
        $null = Invoke-OpenApiInheritance -FeatureArea $featureArea -OutputRoot $outputRoot 6>$null
        Assert-BytesEqual $inheritedBytes ([System.IO.File]::ReadAllBytes($result.DiscriminatorInherited)) "Repeated inheritance changed retained bytes."
        Assert-BytesEqual $sortedBytes ([System.IO.File]::ReadAllBytes($result.DiscriminatorInheritedSorted)) "Repeated inheritance changed sorted bytes."
        Assert-BytesEqual $aliasBytes ([System.IO.File]::ReadAllBytes($result.Final)) "Repeated inheritance changed alias bytes."
    }

    Invoke-Test "explicit input supports cross-directory inheritance" {
        $inputDirectory = Join-Path $temporaryDirectory "source"
        $outputRoot = Join-Path $temporaryDirectory "cross-directory"
        $null = [System.IO.Directory]::CreateDirectory($inputDirectory)
        $inputPath = Join-Path $inputDirectory "responses.openapi.json"
        $null = Write-OpenApiJsonFile -Document (New-InheritTopLevelTestDocument) -Path $inputPath
        [byte[]]$inputBytes = [System.IO.File]::ReadAllBytes($inputPath)

        $result = Invoke-OpenApiInheritance `
            -FeatureArea $featureArea `
            -InputPath $inputPath `
            -OutputRoot $outputRoot `
            6>$null
        Assert-BytesEqual $inputBytes ([System.IO.File]::ReadAllBytes($inputPath)) "Cross-directory inheritance modified its input."
        Assert-True ([System.IO.File]::Exists($result.DiscriminatorInheritedSorted)) "Cross-directory sorted output was not created."
        Assert-BytesEqual ([System.IO.File]::ReadAllBytes($result.DiscriminatorInheritedSorted)) ([System.IO.File]::ReadAllBytes($result.Final)) "Cross-directory alias differs from retained output."
    }

    Invoke-Test "failed inheritance preserves existing alias" {
        $outputRoot = Join-Path $temporaryDirectory "failure"
        $paths = Get-OpenApiArtifactPaths -FeatureArea $featureArea -OutputRoot $outputRoot
        $null = [System.IO.Directory]::CreateDirectory($paths.OutputDirectory)
        $invalid = New-InheritTopLevelTestDocument
        $invalid.components.schemas.Pet["description"] = "unsupported"
        $null = Write-OpenApiJsonFile -Document $invalid -Path $paths.Final
        [byte[]]$before = [System.IO.File]::ReadAllBytes($paths.Final)

        Assert-Throws {
            Invoke-OpenApiInheritance -FeatureArea $featureArea -OutputRoot $outputRoot 6>$null
        } "*unsupported sibling*"
        Assert-BytesEqual $before ([System.IO.File]::ReadAllBytes($paths.Final)) "Failed inheritance changed the alias."
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
