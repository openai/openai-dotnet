<#
.SYNOPSIS
    Generates the complete default component map for an OpenAPI document.

.DESCRIPTION
    Analyzes a componentized OpenAPI components.schemas graph, validates the
    optional manually maintained component policy map, and generates a complete
    default map for every component not claimed by policy. The previous default
    map is never read; successful generation atomically replaces it.

.PARAMETER InputPath
    The componentized OpenAPI JSON document. Defaults to
    specification/openapi/responses.componentized.openapi.json.

.PARAMETER DefaultMapPath
    The generated JSON map. Defaults to
    specification/openapi/responses.component-default-map.json.

.PARAMETER PolicyMapPath
    The optional manually maintained policy map. Defaults to
    specification/openapi/responses.component-policy-map.json. A missing file
    means that no policy has been defined.

.EXAMPLE
    ./scripts/Generate-OpenApiComponentDefaultMap.ps1

.EXAMPLE
    ./scripts/Generate-OpenApiComponentDefaultMap.ps1 `
        ./artifacts/responses.componentized.openapi.json `
        -DefaultMapPath ./artifacts/responses.component-default-map.json `
        -PolicyMapPath ./artifacts/responses.component-policy-map.json
#>

#Requires -Version 7.0

[CmdletBinding()]
param(
    [Parameter(Position = 0)]
    [string]$InputPath,

    [Parameter()]
    [string]$DefaultMapPath,

    [Parameter()]
    [string]$PolicyMapPath
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$helperPath = Join-Path $PSScriptRoot "_OpenApiDocument.Helpers.ps1"
$componentMapHelperPath = Join-Path $PSScriptRoot "_OpenApiComponentDeduplication.Helpers.ps1"
foreach ($path in @($helperPath, $componentMapHelperPath)) {
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) {
        throw "The required helper script was not found: $path"
    }
    . $path
}

$script:ComponentMapRepositoryRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot ".."))
$script:DefaultComponentMapInputPath = Join-Path $script:ComponentMapRepositoryRoot "specification" "openapi" "responses.componentized.openapi.json"
$script:DefaultComponentDefaultMapPath = Join-Path $script:ComponentMapRepositoryRoot "specification" "openapi" "responses.component-default-map.json"
$script:DefaultComponentPolicyMapPath = Join-Path $script:ComponentMapRepositoryRoot "specification" "openapi" "responses.component-policy-map.json"

function Resolve-ComponentMapRepositoryPath {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    if ([System.IO.Path]::IsPathRooted($Path)) {
        return [System.IO.Path]::GetFullPath($Path)
    }
    return [System.IO.Path]::GetFullPath((Join-Path $script:ComponentMapRepositoryRoot $Path))
}

function Invoke-OpenApiComponentDefaultMapGeneration {
    param(
        [Parameter()]
        [string]$InputPath,

        [Parameter()]
        [string]$DefaultMapPath,

        [Parameter()]
        [string]$PolicyMapPath
    )

    if ([string]::IsNullOrWhiteSpace($InputPath)) {
        $InputPath = $script:DefaultComponentMapInputPath
    }
    else {
        $InputPath = Resolve-ComponentMapRepositoryPath -Path $InputPath
    }
    $resolvedInputPath = (Resolve-Path -LiteralPath $InputPath -ErrorAction Stop).ProviderPath
    if (-not [System.IO.File]::Exists($resolvedInputPath)) {
        throw "The componentized OpenAPI input path is not a file: $resolvedInputPath"
    }

    if ([string]::IsNullOrWhiteSpace($DefaultMapPath)) {
        $DefaultMapPath = $script:DefaultComponentDefaultMapPath
    }
    else {
        $DefaultMapPath = Resolve-ComponentMapRepositoryPath -Path $DefaultMapPath
    }
    if ([string]::IsNullOrWhiteSpace($PolicyMapPath)) {
        $PolicyMapPath = $script:DefaultComponentPolicyMapPath
    }
    else {
        $PolicyMapPath = Resolve-ComponentMapRepositoryPath -Path $PolicyMapPath
    }
    if (Test-Path -LiteralPath $PolicyMapPath -PathType Container) {
        throw "The component policy map path is not a file: $PolicyMapPath"
    }

    $document = Read-OpenApiDocument -Path $resolvedInputPath
    $null = Assert-OpenApiDiscriminatorMappings -Document $document
    $policyMap = if ([System.IO.File]::Exists($PolicyMapPath)) {
        Read-OpenApiComponentPolicyMap -Path $PolicyMapPath
    }
    else {
        $null
    }
    $plan = New-OpenApiComponentDefaultMapPlan -Document $document -PolicyMap $policyMap
    $writtenPath = Write-OpenApiJsonFile -Document $plan.DefaultMap -Path $DefaultMapPath

    Write-Host ""
    Write-Host "Input: $resolvedInputPath"
    Write-Host "Policy map: $(if ([System.IO.File]::Exists($PolicyMapPath)) { $PolicyMapPath } else { '<none>' })"
    Write-Host "Default map: $writtenPath"
    Write-Host "Components scanned: $($plan.ComponentCount)"
    Write-Host "Policy components: $($plan.PolicyComponentCount)"
    Write-Host "Default groups: $($plan.DefaultMap.groups.Count)"
    Write-Host "Singleton default groups: $($plan.SingletonDefaultGroupCount)"
    Write-Host "Partial default groups: $($plan.PartialDefaultGroupCount)"
    Write-Host "Logical output groups: $($plan.LogicalGroupCount)"
    Write-Host "Potential component removals: $($plan.RemovableComponentCount)"

    return [PSCustomObject]@{
        DefaultMap = $plan.DefaultMap
        DefaultMapPath = $writtenPath
        PolicyMapPath = if ([System.IO.File]::Exists($PolicyMapPath)) { $PolicyMapPath } else { $null }
        ComponentCount = $plan.ComponentCount
        PolicyComponentCount = $plan.PolicyComponentCount
        DefaultGroupCount = $plan.DefaultMap.groups.Count
        SingletonDefaultGroupCount = $plan.SingletonDefaultGroupCount
        PartialDefaultGroupCount = $plan.PartialDefaultGroupCount
        LogicalGroupCount = $plan.LogicalGroupCount
        RemovableComponentCount = $plan.RemovableComponentCount
    }
}

if ($MyInvocation.InvocationName -ne ".") {
    $null = Invoke-OpenApiComponentDefaultMapGeneration `
        -InputPath $InputPath `
        -DefaultMapPath $DefaultMapPath `
        -PolicyMapPath $PolicyMapPath
}
