<#
.SYNOPSIS
    Verifies generic top-level OpenAPI phases chain through feature aliases.
#>

[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$specificationScriptsDirectory = Join-Path $PSScriptRoot ".."
. (Join-Path $specificationScriptsDirectory "pipeline" "_OpenApiPipeline.Helpers.ps1")
. (Join-Path $specificationScriptsDirectory "pipeline" "_OpenApiDocument.Helpers.ps1")

function Assert-True {
    param([bool]$Condition, [string]$Message)
    if (-not $Condition) { throw $Message }
}

function Assert-BytesEqual {
    param([byte[]]$Expected, [byte[]]$Actual, [string]$Message)
    if (-not [System.Linq.Enumerable]::SequenceEqual($Expected, $Actual)) {
        throw $Message
    }
}

function Assert-FeatureArtifactLayout {
    param([object]$Paths)

    $numberedPaths = @(
        $Paths.Extracted,
        $Paths.Omitted,
        $Paths.Inlined,
        $Paths.Flattened,
        $Paths.Simplified,
        $Paths.RecursiveSimplified,
        $Paths.TypeEnumSplit,
        $Paths.OneOfDiscriminated,
        $Paths.OneOfNormalized,
        $Paths.OneOfFlattened,
        $Paths.SingletonTypeWrapped,
        $Paths.Sorted,
        $Paths.Componentized,
        $Paths.DiscriminatorInherited,
        $Paths.DiscriminatorInheritedSorted,
        $Paths.Deduplicated
    )
    for ($index = 0; $index -lt $numberedPaths.Count; $index++) {
        $path = $numberedPaths[$index]
        Assert-True ([System.IO.File]::Exists($path)) "Numbered artifact was not created: $path"
        $expectedPrefix = "$($Paths.FeatureArea).$(($index + 1).ToString('00'))."
        Assert-True ([System.IO.Path]::GetFileName($path).StartsWith($expectedPrefix, [System.StringComparison]::Ordinal)) "Artifact '$path' has the wrong sequence number."
    }
    foreach ($stablePath in @($Paths.DefaultComponentMap, $Paths.PolicyComponentMap, $Paths.Final)) {
        Assert-True ([System.IO.File]::Exists($stablePath)) "Stable artifact was not created: $stablePath"
        Assert-True ([System.IO.Path]::GetFileName($stablePath) -notmatch '^.+\.\d{2}\.') "Stable artifact was unexpectedly numbered: $stablePath"
    }
    Assert-True (@(Get-ChildItem -LiteralPath $Paths.OutputDirectory -File).Count -eq 19) "Feature '$($Paths.FeatureArea)' did not contain exactly 19 artifacts."
}

function Invoke-TopLevelPhase {
    param(
        [string]$ScriptPath,
        [string[]]$Arguments
    )

    $pwshPath = (Get-Process -Id $PID).Path
    $output = & $pwshPath -NoLogo -NoProfile -NonInteractive -File $ScriptPath @Arguments 2>&1
    if ($LASTEXITCODE -ne 0) {
        throw "$(Split-Path $ScriptPath -Leaf) failed with exit code $LASTEXITCODE.`n$($output -join [Environment]::NewLine)"
    }
}

$temporaryDirectory = Join-Path ([System.IO.Path]::GetTempPath()) "openapi-pipeline-$([Guid]::NewGuid().ToString('N'))"
$null = [System.IO.Directory]::CreateDirectory($temporaryDirectory)

try {
    $sourcePath = Join-Path $PSScriptRoot "fixtures" "Generate-OpenApi" "source.openapi.json"
    $outputRoot = Join-Path $temporaryDirectory "output"
    $paths = Get-OpenApiArtifactPaths -FeatureArea "responses" -OutputRoot $outputRoot
    $null = [System.IO.Directory]::CreateDirectory($paths.OutputDirectory)
    $null = Write-OpenApiJsonFile -Document ([ordered]@{ version = [long]1; groups = @() }) -Path $paths.PolicyComponentMap
    [byte[]]$sourceBytes = [System.IO.File]::ReadAllBytes($sourcePath)

    Invoke-TopLevelPhase `
        -ScriptPath (Join-Path $specificationScriptsDirectory "Generate-OpenApi.ps1") `
        -Arguments @("-FeatureArea", "responses", "-OperationId", "createResponse", "-SpecificationPath", $sourcePath, "-OutputRoot", $outputRoot)
    Assert-BytesEqual ([System.IO.File]::ReadAllBytes($paths.Componentized)) ([System.IO.File]::ReadAllBytes($paths.Final)) "Generate did not publish its componentized output."
    [byte[]]$componentizedBytes = [System.IO.File]::ReadAllBytes($paths.Componentized)

    Invoke-TopLevelPhase `
        -ScriptPath (Join-Path $specificationScriptsDirectory "Inherit-OpenApi.ps1") `
        -Arguments @("-FeatureArea", "responses", "-OutputRoot", $outputRoot)
    Assert-BytesEqual $componentizedBytes ([System.IO.File]::ReadAllBytes($paths.Componentized)) "Inherit modified the retained componentized artifact."
    Assert-BytesEqual ([System.IO.File]::ReadAllBytes($paths.DiscriminatorInheritedSorted)) ([System.IO.File]::ReadAllBytes($paths.Final)) "Inherit did not publish its sorted output."
    [byte[]]$inheritedBytes = [System.IO.File]::ReadAllBytes($paths.DiscriminatorInheritedSorted)

    Invoke-TopLevelPhase `
        -ScriptPath (Join-Path $specificationScriptsDirectory "Finalize-OpenApi.ps1") `
        -Arguments @("-FeatureArea", "responses", "-OutputRoot", $outputRoot)
    Assert-BytesEqual $componentizedBytes ([System.IO.File]::ReadAllBytes($paths.Componentized)) "Finalize modified the retained componentized artifact."
    Assert-BytesEqual $inheritedBytes ([System.IO.File]::ReadAllBytes($paths.DiscriminatorInheritedSorted)) "Finalize modified the retained inherited artifact."
    Assert-BytesEqual ([System.IO.File]::ReadAllBytes($paths.Deduplicated)) ([System.IO.File]::ReadAllBytes($paths.Final)) "Finalize did not publish its deduplicated output."
    Assert-BytesEqual $sourceBytes ([System.IO.File]::ReadAllBytes($sourcePath)) "The top-level pipeline modified its source specification."

    foreach ($artifactPath in @($paths.DefaultComponentMap, $paths.Deduplicated, $paths.Final)) {
        Assert-True ([System.IO.File]::Exists($artifactPath)) "The chained pipeline did not produce '$artifactPath'."
    }
    $document = Read-OpenApiDocument -Path $paths.Final
    Assert-OpenApiReferencesResolve -Document $document
    Assert-AdditionalOpenApiReferencesResolve -Document $document
    Assert-FeatureArtifactLayout -Paths $paths

    [byte[]]$responsesFinalBytes = [System.IO.File]::ReadAllBytes($paths.Final)
    $alternateSourcePath = Join-Path $temporaryDirectory "alternate.openapi.json"
    $alternateSource = Get-Content -LiteralPath $sourcePath -Raw -Encoding utf8 |
        ConvertFrom-Json -AsHashtable -Depth 100
    $alternateSource.paths["/responses"].post.operationId = "createAlternate"
    $alternateSource.paths["/alternate"] = $alternateSource.paths["/responses"]
    $null = $alternateSource.paths.Remove("/responses")
    $null = Write-OpenApiJsonFile -Document $alternateSource -Path $alternateSourcePath
    $alternatePaths = Get-OpenApiArtifactPaths -FeatureArea "alternate" -OutputRoot $outputRoot
    $null = [System.IO.Directory]::CreateDirectory($alternatePaths.OutputDirectory)
    $null = Write-OpenApiJsonFile -Document ([ordered]@{ version = [long]1; groups = @() }) -Path $alternatePaths.PolicyComponentMap

    Invoke-TopLevelPhase `
        -ScriptPath (Join-Path $specificationScriptsDirectory "Generate-OpenApi.ps1") `
        -Arguments @("-FeatureArea", "alternate", "-OperationId", "createAlternate", "-SpecificationPath", $alternateSourcePath, "-OutputRoot", $outputRoot)
    Invoke-TopLevelPhase `
        -ScriptPath (Join-Path $specificationScriptsDirectory "Inherit-OpenApi.ps1") `
        -Arguments @("-FeatureArea", "alternate", "-OutputRoot", $outputRoot)
    Invoke-TopLevelPhase `
        -ScriptPath (Join-Path $specificationScriptsDirectory "Finalize-OpenApi.ps1") `
        -Arguments @("-FeatureArea", "alternate", "-OutputRoot", $outputRoot)

    Assert-True ([System.IO.File]::Exists($alternatePaths.Final)) "Second feature alias was not created."
    Assert-FeatureArtifactLayout -Paths $alternatePaths
    Assert-BytesEqual $responsesFinalBytes ([System.IO.File]::ReadAllBytes($paths.Final)) "Running the second feature changed the Responses alias."
    Assert-True (-not [System.IO.File]::Exists((Join-Path $outputRoot "responses.openapi.json"))) "An artifact was written directly beneath the output root."

    Write-Host "[PASS] Generic phases chain through isolated feature aliases"
}
finally {
    if ([System.IO.Directory]::Exists($temporaryDirectory)) {
        [System.IO.Directory]::Delete($temporaryDirectory, $true)
    }
}
