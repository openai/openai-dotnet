<#
.SYNOPSIS
    Verifies the top-level Responses OpenAPI phases chain through the published alias.
#>

[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$specificationScriptsDirectory = Join-Path $PSScriptRoot ".."
. (Join-Path $specificationScriptsDirectory "pipeline" "_ResponsesOpenApiPipeline.Helpers.ps1")
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

$temporaryDirectory = Join-Path ([System.IO.Path]::GetTempPath()) "responses-openapi-pipeline-$([Guid]::NewGuid().ToString('N'))"
$null = [System.IO.Directory]::CreateDirectory($temporaryDirectory)

try {
    $sourcePath = Join-Path $PSScriptRoot "fixtures" "Generate-ResponsesOpenApi" "source.openapi.json"
    $outputDirectory = Join-Path $temporaryDirectory "output"
    $paths = Get-ResponsesOpenApiArtifactPaths -OutputDirectory $outputDirectory
    [byte[]]$sourceBytes = [System.IO.File]::ReadAllBytes($sourcePath)

    Invoke-TopLevelPhase `
        -ScriptPath (Join-Path $specificationScriptsDirectory "Generate-ResponsesOpenApi.ps1") `
        -Arguments @("-SpecificationPath", $sourcePath, "-OutputDirectory", $outputDirectory)
    Assert-BytesEqual ([System.IO.File]::ReadAllBytes($paths.Componentized)) ([System.IO.File]::ReadAllBytes($paths.Final)) "Generate did not publish its componentized output."
    [byte[]]$componentizedBytes = [System.IO.File]::ReadAllBytes($paths.Componentized)

    Invoke-TopLevelPhase `
        -ScriptPath (Join-Path $specificationScriptsDirectory "Inherit-ResponsesOpenApi.ps1") `
        -Arguments @("-OutputDirectory", $outputDirectory)
    Assert-BytesEqual $componentizedBytes ([System.IO.File]::ReadAllBytes($paths.Componentized)) "Inherit modified the retained componentized artifact."
    Assert-BytesEqual ([System.IO.File]::ReadAllBytes($paths.DiscriminatorInheritedSorted)) ([System.IO.File]::ReadAllBytes($paths.Final)) "Inherit did not publish its sorted output."
    [byte[]]$inheritedBytes = [System.IO.File]::ReadAllBytes($paths.DiscriminatorInheritedSorted)

    Invoke-TopLevelPhase `
        -ScriptPath (Join-Path $specificationScriptsDirectory "Finalize-ResponsesOpenApi.ps1") `
        -Arguments @("-OutputDirectory", $outputDirectory)
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

    Write-Host "[PASS] Generate, Inherit, and Finalize chain through responses.openapi.json"
}
finally {
    if ([System.IO.Directory]::Exists($temporaryDirectory)) {
        [System.IO.Directory]::Delete($temporaryDirectory, $true)
    }
}
