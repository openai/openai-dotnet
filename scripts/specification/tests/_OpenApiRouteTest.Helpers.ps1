#Requires -Version 7.0

. (Join-Path $PSScriptRoot "_OpenApiTestFixture.Helpers.ps1")

function Initialize-OpenApiRouteTestState {
    $script:PassedTests = 0
    $script:FailedTests = 0
    $script:Failures = [System.Collections.Generic.List[string]]::new()
}

function Invoke-Test {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Name,

        [Parameter(Mandatory = $true)]
        [scriptblock]$Test
    )

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
    param(
        [AllowNull()][object]$Expected,
        [AllowNull()][object]$Actual,
        [string]$Message
    )
    if ($Expected -ne $Actual) {
        throw "$Message Expected '$Expected', got '$Actual'."
    }
}

function Assert-Sequence {
    param(
        [AllowEmptyCollection()][object[]]$Expected,
        [AllowEmptyCollection()][object[]]$Actual,
        [string]$Message
    )
    $expectedText = $Expected -join "|"
    $actualText = $Actual -join "|"
    if (-not [string]::Equals($expectedText, $actualText, [System.StringComparison]::Ordinal)) {
        throw "$Message Expected '$expectedText', got '$actualText'."
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

function Find-JsonPropertyValues {
    param(
        [Parameter(Mandatory = $true)]
        [AllowNull()]
        [object]$Value,

        [Parameter(Mandatory = $true)]
        [string]$Name
    )

    if ($null -eq $Value) { return }
    if (Test-JsonArrayValue -Value $Value) {
        foreach ($item in $Value) {
            Find-JsonPropertyValues -Value $item -Name $Name
        }
        return
    }
    if (-not (Test-JsonObjectValue -Value $Value)) { return }

    foreach ($property in (Get-JsonProperties -Value $Value)) {
        if ([string]::Equals($property.Name, $Name, [System.StringComparison]::Ordinal)) {
            $property.Value
        }
        Find-JsonPropertyValues -Value $property.Value -Name $Name
    }
}

function Find-OmittedOpenApiPropertyNames {
    param(
        [Parameter(Mandatory = $true)]
        [AllowNull()]
        [object]$Value,

        [Parameter()]
        [ValidateSet("OpenApi", "Components", "SchemaMap", "SchemaArray", "Schema", "Literal")]
        [string]$Context = "OpenApi",

        [Parameter()]
        [string]$Breadcrumb = "#"
    )

    if ($null -eq $Value) { return }
    if (Test-JsonArrayValue -Value $Value) {
        $itemContext = $Context
        if ([string]::Equals($Context, "SchemaArray", [System.StringComparison]::Ordinal)) {
            $itemContext = "Schema"
        }
        for ($index = 0; $index -lt $Value.Count; $index++) {
            Find-OmittedOpenApiPropertyNames `
                -Value $Value[$index] `
                -Context $itemContext `
                -Breadcrumb "$Breadcrumb/$index"
        }
        return
    }
    if (-not (Test-JsonObjectValue -Value $Value)) { return }

    foreach ($property in (Get-JsonProperties -Value $Value)) {
        if (Test-IsOmittedOpenApiProperty `
            -Name $property.Name `
            -ParentObject $Value `
            -ParentContext $Context `
            -ParentBreadcrumb $Breadcrumb) {
            $property.Name
        }
        $childContext = Get-OpenApiSchemaChildContext `
            -ParentContext $Context `
            -PropertyName $property.Name `
            -PropertyValue $property.Value
        Find-OmittedOpenApiPropertyNames `
            -Value $property.Value `
            -Context $childContext `
            -Breadcrumb "$Breadcrumb/$(ConvertTo-JsonPointerToken -Token $property.Name)"
    }
}

function New-OpenApiRouteTestContext {
    $fixture = Read-TestJsonFixture -RelativePath "Flatten-OpenApiAllOf/source.openapi.json"
    $catalog = Get-OpenApiOperationCatalog -Document $fixture
    $operationEntry = Select-OpenApiOperation `
        -Catalog $catalog `
        -RequestedOperationId "createThing"
    return [PSCustomObject]@{
        Fixture = $fixture
        Cases = Read-TestJsonFixture -RelativePath "Extract-OpenApiRoute/cases.json"
        Catalog = $catalog
        OperationEntry = $operationEntry
    }
}

function Complete-OpenApiRouteTests {
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
