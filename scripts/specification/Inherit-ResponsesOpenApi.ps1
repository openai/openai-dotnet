<#
.SYNOPSIS
    Refactors the Responses OpenAPI document into discriminator inheritance.

.DESCRIPTION
    Reads responses.openapi.json by default, refactors componentized
    discriminator oneOf schemas into base/derived allOf inheritance, canonically
    sorts the result, retains both stage artifacts, and atomically republishes
    the inherited-sorted document as responses.openapi.json.

.PARAMETER InputPath
    The componentized Responses OpenAPI input. Defaults to responses.openapi.json
    in OutputDirectory.

.PARAMETER OutputDirectory
    Directory receiving retained stage artifacts and responses.openapi.json.
    Defaults to specification/openapi.

.EXAMPLE
    ./scripts/specification/Inherit-ResponsesOpenApi.ps1

.EXAMPLE
    ./scripts/specification/Inherit-ResponsesOpenApi.ps1 `
        -InputPath ./artifacts/source/responses.openapi.json `
        -OutputDirectory ./artifacts/inherited
#>

#Requires -Version 7.0

[CmdletBinding()]
param(
    [Parameter()]
    [string]$InputPath,

    [Parameter()]
    [string]$OutputDirectory
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

. (Join-Path $PSScriptRoot "pipeline" "_ResponsesOpenApiPipeline.Helpers.ps1")

$script:RepositoryRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot ".." ".."))
$script:PipelineDirectory = Join-Path $PSScriptRoot "pipeline"
$script:DefaultOutputDirectory = Join-Path $script:RepositoryRoot "specification" "openapi"

function Invoke-ResponsesOpenApiInheritance {
    param(
        [Parameter()]
        [string]$InputPath,

        [Parameter(Mandatory = $true)]
        [string]$OutputDirectory
    )

    $paths = Get-ResponsesOpenApiArtifactPaths -OutputDirectory $OutputDirectory
    if ([string]::IsNullOrWhiteSpace($InputPath)) {
        $InputPath = $paths.Final
    }
    $resolvedInputPath = (Resolve-Path -LiteralPath $InputPath -ErrorAction Stop).ProviderPath
    if (-not [System.IO.File]::Exists($resolvedInputPath)) {
        throw "The componentized Responses OpenAPI input path is not a file: $resolvedInputPath"
    }

    $stageScriptNames = @(
        "Refactor-OpenApiDiscriminatedInheritance.ps1"
        "Sort-OpenApiDocument.ps1"
    )
    $stageScriptPaths = [ordered]@{}
    foreach ($stageScriptName in $stageScriptNames) {
        $stageScriptPath = Join-Path $script:PipelineDirectory $stageScriptName
        if (-not (Test-Path -LiteralPath $stageScriptPath -PathType Leaf)) {
            throw "The OpenAPI stage script was not found: $stageScriptPath"
        }
        $stageScriptPaths[$stageScriptName] = $stageScriptPath
    }

    $null = & $stageScriptPaths["Refactor-OpenApiDiscriminatedInheritance.ps1"] `
        -InputPath $resolvedInputPath `
        -OutputPath $paths.DiscriminatorInherited
    $null = & $stageScriptPaths["Sort-OpenApiDocument.ps1"] `
        -InputPath $paths.DiscriminatorInherited `
        -OutputPath $paths.DiscriminatorInheritedSorted

    foreach ($artifactPath in @($paths.DiscriminatorInherited, $paths.DiscriminatorInheritedSorted)) {
        if (-not [System.IO.File]::Exists($artifactPath)) {
            throw "The Responses OpenAPI inheritance pipeline did not produce '$artifactPath'."
        }
    }
    $null = Publish-ResponsesOpenApiArtifact `
        -SourcePath $paths.DiscriminatorInheritedSorted `
        -DestinationPath $paths.Final

    Write-Host ""
    Write-Host "Responses OpenAPI inheritance complete."
    Write-Host "Input: $resolvedInputPath"
    Write-Host "Inherited output: $($paths.DiscriminatorInherited)"
    Write-Host "Inherited sorted output: $($paths.DiscriminatorInheritedSorted)"
    Write-Host "Published output: $($paths.Final)"

    return $paths
}

if ($MyInvocation.InvocationName -ne ".") {
    if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
        $OutputDirectory = $script:DefaultOutputDirectory
    }
    $null = Invoke-ResponsesOpenApiInheritance `
        -InputPath $InputPath `
        -OutputDirectory $OutputDirectory
}
