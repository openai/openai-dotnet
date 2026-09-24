#Requires -Version 7.0

# Dependency-free harness shared by the mapping and application suites. Keeping
# these assertions local lets the PowerShell transformation tests run in CI
# without importing Pester or participating in the recorded service-test system.
. (Join-Path $PSScriptRoot "_OpenApiTestFixture.Helpers.ps1")

# State is reset once per suite because both suites dot-source this same file in
# their own PowerShell process.
function Initialize-OpenApiComponentDeduplicationTestState {
    $script:PassedTests = 0
    $script:FailedTests = 0
    $script:Failures = [System.Collections.Generic.List[string]]::new()
}

# Record failures and continue so one run reports every independent scenario.
# Complete-OpenApiComponentDeduplicationTests converts the aggregate into the
# process exit code expected by CI.
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

# Sequence equality is serialized deliberately: these tests care about stable
# group/member/component ordering, not only set membership.
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

function Assert-BytesEqual {
    param([byte[]]$Expected, [byte[]]$Actual, [string]$Message)
    if (-not [System.Linq.Enumerable]::SequenceEqual($Expected, $Actual)) {
        throw $Message
    }
}

# Centralizes the compact graph fixture used by both mapping reconciliation and
# deduplication application tests.
function Get-TestDeduplicationFixturePath {
    return Join-Path $script:OpenApiTestFixtureRoot "Deduplicate-OpenApiComponents/source.componentized.openapi.json"
}

# These wrappers keep scenario setup terse while still exercising the same public
# helper pipeline used by the production updater.
function New-TestDeduplicationAnalysis {
    param([object]$Document)
    return Get-OpenApiComponentDeduplicationAnalysis -Document $Document
}

function New-TestDeduplicationMapping {
    param([object]$Document)
    $analysis = New-TestDeduplicationAnalysis -Document $Document
    return (Update-OpenApiComponentDeduplicationMapping -Analysis $analysis).Mapping
}

# Print one summary and fail the process only after all scenarios have run.
function Complete-OpenApiComponentDeduplicationTests {
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
}
