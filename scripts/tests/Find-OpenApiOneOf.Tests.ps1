<#
.SYNOPSIS
    Runs focused tests for scripts/Find-OpenApiOneOf.ps1.
#>

[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$scriptPath = Join-Path $PSScriptRoot ".." "Find-OpenApiOneOf.ps1"
$fixtureDirectory = Join-Path $PSScriptRoot "fixtures" "Find-OpenApiOneOf"

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
    param([scriptblock]$Action, [string]$MessagePattern = "*")
    try { & $Action }
    catch {
        if ($_.Exception.Message -notlike $MessagePattern) {
            throw "Expected error matching '$MessagePattern', got '$($_.Exception.Message)'."
        }
        return
    }
    throw "Expected an error matching '$MessagePattern', but no error was thrown."
}

function Write-TestJson {
    param(
        [string]$Path,
        [string[]]$Lines,
        [string]$NewLine = "`n",
        [bool]$WithByteOrderMark = $false
    )

    $contents = [string]::Join($NewLine, $Lines) + $NewLine
    $encoding = [System.Text.UTF8Encoding]::new($WithByteOrderMark)
    [System.IO.File]::WriteAllText($Path, $contents, $encoding)
}

$temporaryDirectory = Join-Path ([System.IO.Path]::GetTempPath()) "find-openapi-oneof-$([Guid]::NewGuid().ToString('N'))"
$null = [System.IO.Directory]::CreateDirectory($temporaryDirectory)

try {
    # Copilot/test sessions can invoke the analyzer repeatedly in one process;
    # its embedded C# type must be reusable rather than redefined.
    Invoke-Test "runs when the previous analyzer type is already loaded" {
        if ($null -eq ("OpenAI.Scripts.OpenApiOneOf.V3.OpenApiOneOfCategorizer" -as [type])) {
            Add-Type -TypeDefinition @'
namespace OpenAI.Scripts.OpenApiOneOf.V3
{
    public static class OpenApiOneOfCategorizer
    {
    }
}
'@
        }

        $fixturePath = Join-Path $fixtureDirectory "none.json"
        $actual = @(& $scriptPath $fixturePath)
        Assert-True ($actual -ccontains "Discriminator Reconciliation:") "The current analyzer did not run."
    }

    # The analyzer is used for schema reconciliation, so each oneOf category and
    # malformed discriminator detail must remain stable and auditable.
    Invoke-Test "classifies occurrences and prints counts with required details" {
        $fixturePath = Join-Path $fixtureDirectory "categories.json"

        $actual = @(& $scriptPath $fixturePath)
        Assert-Sequence @(
            "Discriminated (2):",
            "4: kind",
            "23: variant",
            "",
            "Potentially Discriminated (3):",
            10,
            29,
            58,
            "",
            "Other (7):",
            "15: enum, boolean, string, object, object",
            "36: object, array",
            "42: (unknown)",
            "47: (unknown)",
            "52: (unknown)",
            "55: (unknown)",
            "63: integer, boolean",
            "",
            "Discriminator Reconciliation:",
            "Structural discriminator properties: 4",
            "Counted by Discriminated oneOf: 2",
            "Paired with rejected oneOf: 1",
            "Paired with malformed discriminator: 1",
            "Without sibling oneOf: 0",
            "Exact discriminator string values: 0",
            "Exact quoted discriminator occurrences: 4",
            "",
            "Paired with rejected oneOf (1):",
            "35 -> oneOf 36: kind; branch 2 has type array",
            "",
            "Paired with malformed discriminator (1):",
            "32 -> oneOf 29: propertyName is number, expected string",
            "",
            "Without sibling oneOf (0):",
            "",
            "Exact discriminator string values (0):"
        ) $actual "Categorized output was incorrect."
    }

    # Structural discriminator keywords must be distinguished from identical
    # strings in enums, required arrays, and ordinary literal fields.
    Invoke-Test "reconciles unpaired discriminators and exact string values" {
        $fixturePath = Join-Path $fixtureDirectory "reconciliation.json"

        $actual = @(& $scriptPath $fixturePath)
        $reconciliationIndex = [Array]::IndexOf($actual, "Discriminator Reconciliation:")
        Assert-True ($reconciliationIndex -ge 0) "The reconciliation heading was missing."
        $reconciliation = @($actual[$reconciliationIndex..($actual.Count - 1)])
        Assert-Sequence @(
            "Discriminator Reconciliation:",
            "Structural discriminator properties: 2",
            "Counted by Discriminated oneOf: 0",
            "Paired with rejected oneOf: 1",
            "Paired with malformed discriminator: 0",
            "Without sibling oneOf: 1",
            "Exact discriminator string values: 4",
            "Exact quoted discriminator occurrences: 6",
            "",
            "Paired with rejected oneOf (1):",
            '13 -> oneOf 14: kind; branch 2 uses $recursiveRef without explicit type: object',
            "",
            "Paired with malformed discriminator (0):",
            "",
            "Without sibling oneOf (1):",
            "3: discriminator",
            "",
            "Exact discriminator string values (4):",
            "4: propertyName value (not a discriminator keyword)",
            "21: enum value (not a discriminator keyword)",
            "24: required name (not a discriminator keyword)",
            "26: property 'label' value (not a discriminator keyword)"
        ) $reconciliation "Discriminator reconciliation output was incorrect."
    }

    # Reported locations are consumed by humans, so line numbering must remain
    # one-based across LF, CRLF, and BOM-encoded files.
    Invoke-Test "preserves one-based lines for oneOf and discriminator tokens" {
        $lines = Get-Content -LiteralPath (Join-Path $fixtureDirectory "line-locations.json")
        $cases = @(
            @{ Name = "lf.json"; NewLine = "`n"; Bom = $false },
            @{ Name = "crlf.json"; NewLine = "`r`n"; Bom = $false },
            @{ Name = "crlf-bom.json"; NewLine = "`r`n"; Bom = $true }
        )

        foreach ($case in $cases) {
            $fixturePath = Join-Path $temporaryDirectory $case.Name
            Write-TestJson `
                -Path $fixturePath `
                -Lines $lines `
                -NewLine $case.NewLine `
                -WithByteOrderMark $case.Bom
            $actual = @(& $scriptPath $fixturePath)
            Assert-True ($actual -ccontains "4: kind") "The oneOf line was incorrect for $($case.Name)."
            Assert-True ($actual -ccontains "7: other") "The discriminator line was incorrect for $($case.Name)."
        }
    }

    # Empty categories should still produce the complete report shape, making
    # automation independent of whether matches exist.
    Invoke-Test "prints zero counts when the document has no oneOf properties" {
        $fixturePath = Join-Path $fixtureDirectory "none.json"

        $actual = @(& $scriptPath $fixturePath)
        Assert-Sequence @(
            "Discriminated (0):",
            "",
            "Potentially Discriminated (0):",
            "",
            "Other (0):",
            "",
            "Discriminator Reconciliation:",
            "Structural discriminator properties: 0",
            "Counted by Discriminated oneOf: 0",
            "Paired with rejected oneOf: 0",
            "Paired with malformed discriminator: 0",
            "Without sibling oneOf: 0",
            "Exact discriminator string values: 0",
            "Exact quoted discriminator occurrences: 0",
            "",
            "Paired with rejected oneOf (0):",
            "",
            "Paired with malformed discriminator (0):",
            "",
            "Without sibling oneOf (0):",
            "",
            "Exact discriminator string values (0):"
        ) $actual "Zero-count output was incorrect."
    }

    # File and parse failures should be diagnosed before the embedded analyzer
    # attempts classification.
    Invoke-Test "missing non-file and invalid JSON inputs fail clearly" {
        Assert-Throws { & $scriptPath (Join-Path $temporaryDirectory "missing.json") } "*cannot find path*"
        Assert-Throws { & $scriptPath $temporaryDirectory } "*is not a file*"

        $invalidPath = Join-Path $temporaryDirectory "invalid.json"
        [System.IO.File]::WriteAllText($invalidPath, "{", [System.Text.UTF8Encoding]::new($false))
        Assert-Throws { & $scriptPath $invalidPath }
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