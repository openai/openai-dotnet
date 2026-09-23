<#
.SYNOPSIS
    Runs the OpenAPI script test suites.

.DESCRIPTION
    Runs dependency-free tests against compact fixtures in isolated PowerShell
    processes and prints per-suite timings.

.EXAMPLE
    ./scripts/tests/Test-OpenApiScripts.ps1
#>

#Requires -Version 7.0

[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$env:CLIENTMODEL_TEST_MODE = "Playback"
$env:CLIENTMODEL_DISABLE_AUTO_RECORDING = "true"

$testNames = @(
    "Extract-OpenApiRoute.Tests.ps1"
    "Find-OpenApiOneOf.Tests.ps1"
    "Flatten-OpenApiAllOf.Tests.ps1"
    "Simplify-OpenApiStringVariants.Tests.ps1"
    "Simplify-OpenApiRecursiveSchemas.Tests.ps1"
    "Normalize-OpenApiOneOf.Tests.ps1"
    "Sort-OpenApiDocument.Tests.ps1"
    "Generate-ResponsesOpenApi.Tests.ps1"
    "Componentize-OpenApiObjectSchemas.Tests.ps1"
)

$pwshPath = (Get-Process -Id $PID).Path
$results = [System.Collections.Generic.List[object]]::new()
$failedTests = [System.Collections.Generic.List[string]]::new()
$totalStopwatch = [System.Diagnostics.Stopwatch]::StartNew()

Write-Host "OpenAPI script tests"
foreach ($testName in $testNames) {
    $testPath = Join-Path $PSScriptRoot $testName
    if (-not (Test-Path -LiteralPath $testPath -PathType Leaf)) {
        throw "The OpenAPI test script was not found: $testPath"
    }

    Write-Host ""
    Write-Host "=== $testName ==="
    $stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
    & $pwshPath -NoLogo -NoProfile -NonInteractive -File $testPath
    $exitCode = $LASTEXITCODE
    $stopwatch.Stop()

    $results.Add([PSCustomObject]@{
        Test = $testName
        Seconds = [Math]::Round($stopwatch.Elapsed.TotalSeconds, 2)
        Result = if ($exitCode -eq 0) { "Passed" } else { "Failed ($exitCode)" }
    })
    if ($exitCode -ne 0) {
        $failedTests.Add($testName)
        break
    }
}
$totalStopwatch.Stop()

Write-Host ""
$results | Format-Table -AutoSize | Out-Host
Write-Host "Total: $([Math]::Round($totalStopwatch.Elapsed.TotalSeconds, 2)) seconds"

if ($failedTests.Count -gt 0) {
    throw "OpenAPI tests failed: $($failedTests -join ', ')"
}
