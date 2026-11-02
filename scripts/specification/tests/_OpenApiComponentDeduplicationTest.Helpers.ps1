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

# This wrapper keeps scenario setup terse while exercising production analysis.
function New-TestDeduplicationAnalysis {
    param([object]$Document)
    return Get-OpenApiComponentDeduplicationAnalysis -Document $Document
}

function New-TestNestedInheritanceFamilies {
    param([switch]$AncestorReference)

    $document = Read-TestJsonFixture -RelativePath "_Shared/minimal.openapi.json"
    $schemas = [ordered]@{}
    foreach ($prefix in @("", "Other")) {
        $rootName = "${prefix}InputItem"
        $messageName = "${prefix}Message"
        $userName = "${prefix}UserMessage"
        $assistantName = "${prefix}AssistantMessage"
        $schemas[$rootName] = [ordered]@{
            type = "object"
            required = @("type")
            properties = [ordered]@{ type = [ordered]@{ type = "string" } }
            discriminator = [ordered]@{ propertyName = "type"; mapping = [ordered]@{ message = "#/components/schemas/$messageName" } }
        }
        $schemas[$messageName] = [ordered]@{
            type = "object"
            required = @("type", "role")
            properties = [ordered]@{ type = [ordered]@{ type = "string"; enum = @("message") }; role = [ordered]@{ type = "string" } }
            discriminator = [ordered]@{ propertyName = "role"; mapping = [ordered]@{ user = "#/components/schemas/$userName"; assistant = "#/components/schemas/$assistantName" } }
            allOf = @([ordered]@{ '$ref' = "#/components/schemas/$rootName" })
        }
        foreach ($role in @("user", "assistant")) {
            $leafName = if ($role -ceq "user") { $userName } else { $assistantName }
            $properties = [ordered]@{
                type = [ordered]@{ type = "string"; enum = @("message") }
                role = [ordered]@{ type = "string"; enum = @($role) }
                content = [ordered]@{ type = "string" }
            }
            if ($AncestorReference) {
                $properties["ancestor"] = [ordered]@{ '$ref' = "#/components/schemas/$rootName" }
            }
            $schemas[$leafName] = [ordered]@{
                type = "object"
                required = @("type", "role", "content")
                properties = $properties
                allOf = @([ordered]@{ '$ref' = "#/components/schemas/$messageName" })
            }
        }
    }
    $document["components"] = [ordered]@{ schemas = $schemas }
    $document.paths["/test"].post.responses["200"].content["application/json"].schema = [ordered]@{
        oneOf = @([ordered]@{ '$ref' = "#/components/schemas/InputItem" }, [ordered]@{ '$ref' = "#/components/schemas/OtherInputItem" })
    }
    return $document
}

function New-TestSingletonEnumComponents {
    $document = Read-TestJsonFixture -RelativePath "_Shared/minimal.openapi.json"
    $document["components"] = [ordered]@{ schemas = [ordered]@{
        ModeA = [ordered]@{ type = "string"; enum = @("explicit") }
        ModeB = [ordered]@{ type = "string"; enum = @("explicit") }
        ModeImplicit = [ordered]@{ type = "string"; enum = @("implicit") }
        ModeConstrained = [ordered]@{ type = "string"; enum = @("explicit"); maxLength = 32 }
    } }
    $document.paths["/test"].post.responses["200"].content["application/json"].schema = [ordered]@{
        type = "object"
        required = @("first", "second")
        properties = [ordered]@{
            first = [ordered]@{ '$ref' = "#/components/schemas/ModeA" }
            second = [ordered]@{ '$ref' = "#/components/schemas/ModeB" }
            different = [ordered]@{ '$ref' = "#/components/schemas/ModeImplicit" }
            constrained = [ordered]@{ '$ref' = "#/components/schemas/ModeConstrained" }
        }
    }
    return $document
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
