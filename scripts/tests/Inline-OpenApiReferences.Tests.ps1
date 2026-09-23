<#
.SYNOPSIS
    Runs focused tests for scripts/Inline-OpenApiReferences.ps1.
#>

[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

. (Join-Path $PSScriptRoot ".." "Extract-OpenApiRoute.ps1")
. (Join-Path $PSScriptRoot ".." "Remove-OpenApiOmittedProperties.ps1")
. (Join-Path $PSScriptRoot ".." "Inline-OpenApiReferences.ps1")
. (Join-Path $PSScriptRoot "_OpenApiRouteTest.Helpers.ps1")
Initialize-OpenApiRouteTestState

function ConvertTo-TestInlinedDocument {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document,

        [Parameter(Mandatory = $true)]
        [object]$OperationEntry,

        [Parameter()]
        [ValidateSet("RequestBody", "Responses", "Both")]
        [string]$Mode = "Both"
    )

    $extracted = (New-ExtractedOpenApiDocument `
        -Document $Document `
        -OperationEntry $OperationEntry `
        -Mode $Mode).Document
    $omitted = (ConvertTo-OmittedOpenApiPropertiesDocument -Document $extracted).Document
    return ConvertTo-InlinedOpenApiDocument -Document $omitted
}

$context = New-OpenApiRouteTestContext
$fixture = $context.Fixture
$operationEntry = $context.OperationEntry
$inlining = ConvertTo-TestInlinedDocument `
    -Document $fixture `
    -OperationEntry $operationEntry
$inlined = $inlining.Document

Invoke-Test "selective modes do not traverse omitted-side references" {
    $invalidResponseFixture = Copy-JsonValue -Value $fixture
    $invalidResponseFixture.paths["/things"].post.responses["200"] = [ordered]@{
        '$ref' = "#/components/responses/MissingResponse"
    }
    $invalidResponseOperation = Select-OpenApiOperation `
        -Catalog (Get-OpenApiOperationCatalog -Document $invalidResponseFixture) `
        -RequestedOperationId "createThing"
    $requestResult = ConvertTo-TestInlinedDocument `
        -Document $invalidResponseFixture `
        -OperationEntry $invalidResponseOperation `
        -Mode RequestBody
    Assert-Sequence @("default") @((Get-JsonProperties -Value $requestResult.Document.paths["/things"].post.responses).Name) "RequestBody mode retained the bad response."
    Assert-Throws {
        ConvertTo-TestInlinedDocument `
            -Document $invalidResponseFixture `
            -OperationEntry $invalidResponseOperation `
            -Mode Responses
    } "*has no 'MissingResponse' property*"

    $invalidRequestFixture = Copy-JsonValue -Value $fixture
    $invalidRequestFixture.paths["/things"].post.requestBody.content["application/json"].schema = [ordered]@{
        '$ref' = "#/components/schemas/MissingRequest"
    }
    $invalidRequestOperation = Select-OpenApiOperation `
        -Catalog (Get-OpenApiOperationCatalog -Document $invalidRequestFixture) `
        -RequestedOperationId "createThing"
    $responseResult = ConvertTo-TestInlinedDocument `
        -Document $invalidRequestFixture `
        -OperationEntry $invalidRequestOperation `
        -Mode Responses
    Assert-True (-not (Test-JsonProperty -Object $responseResult.Document.paths["/things"].post -Name "requestBody")) "Responses mode retained the bad request."
    Assert-Throws {
        ConvertTo-TestInlinedDocument `
            -Document $invalidRequestFixture `
            -OperationEntry $invalidRequestOperation `
            -Mode RequestBody
    } "*has no 'MissingRequest' property*"
}

Invoke-Test "inlining keeps only cycle fallback and security components" {
    Assert-Sequence @("A") @((Get-JsonProperties -Value $inlined.components.schemas).Name) "Fallback component selection was incorrect."
    Assert-Sequence `
        @("ApiKeyAuth", "AdminApiKeyAuth") `
        @((Get-JsonProperties -Value $inlined.components.securitySchemes).Name) `
        "Security scheme selection was incorrect."
    Assert-Equal 3 $inlining.DependencyCount "Retained component count was incorrect."
    Assert-OpenApiReferencesResolve -Document $inlined
}

Invoke-Test "inlining retains only the true cycle back-edge" {
    Assert-Equal 1 $inlining.CycleCount "Cycle count was incorrect."
    $ordinaryReferences = @(Get-OpenApiReferenceOccurrences -Value $inlined | Where-Object Kind -eq "Reference")
    Assert-True ($ordinaryReferences.Count -gt 0) "Expected cycle reference was not retained."
    foreach ($occurrence in $ordinaryReferences) {
        Assert-Equal "#/components/schemas/A" $occurrence.Pointer "A non-cycle reference survived inlining."
    }
    Assert-Sequence @("A") @((Get-JsonProperties -Value $inlined.components.schemas).Name) "Fallback component selection was incorrect."
}

Invoke-Test "inlining rewrites discriminator mappings to local branches" {
    $mapping = $inlined.paths["/things"].post.responses["200"].content["application/json"].schema.discriminator.mapping
    Assert-Equal `
        "#/paths/~1things/post/responses/200/content/application~1json/schema/oneOf/0" `
        $mapping.cat `
        "Cat mapping was not rewritten."
    Assert-Equal `
        "#/paths/~1things/post/responses/200/content/application~1json/schema/oneOf/1" `
        $mapping.dog `
        "Dog mapping was not rewritten."
}

Invoke-Test "inlining leaves source allOf intact while expanding references" {
    $preserved = $inlined.paths["/things"].post.requestBody.content["application/json"].schema.properties.flattened
    Assert-True (Test-JsonProperty -Object $preserved -Name "allOf") "Inlined output removed source allOf."
    Assert-True (@(Find-JsonPropertyValues -Value $preserved -Name "allOf").Count -gt 1) "Nested source allOf was removed."
    Assert-Equal 0 @(Get-OpenApiReferenceOccurrences -Value $preserved | Where-Object Kind -eq "Reference").Count "Inlined allOf retained a non-cycle component reference."
}

Invoke-Test "cycle fallback retains source allOf and its cycle reference" {
    $fallbackChild = $inlined.components.schemas.A.properties.child
    Assert-True (Test-JsonProperty -Object $fallbackChild -Name "allOf") "Cycle fallback removed source allOf."
    Assert-True (@(Get-OpenApiReferenceOccurrences -Value $fallbackChild | Where-Object Kind -eq "Reference").Count -gt 0) "Cycle fallback lost its cycle reference."
}

Invoke-Test "explicit recursive references remain finite" {
    $recursiveReferences = @(Find-JsonPropertyValues -Value $inlined -Name '$recursiveRef')
    Assert-Sequence @("#") $recursiveReferences "Recursive references were changed or removed."
}

Invoke-Test "external and unresolved references fail with context" {
    foreach ($reference in @("https://example.test/schema.json", "#/components/schemas/Missing")) {
        $invalidFixture = Copy-JsonValue -Value $fixture
        $invalidFixture.paths["/things"].post.requestBody.content["application/json"].schema = [ordered]@{ '$ref' = $reference }
        $invalidOperation = Select-OpenApiOperation `
            -Catalog (Get-OpenApiOperationCatalog -Document $invalidFixture) `
            -RequestedOperationId "createThing"
        Assert-Throws {
            ConvertTo-TestInlinedDocument `
                -Document $invalidFixture `
                -OperationEntry $invalidOperation
        } $(if ($reference.StartsWith("https", [System.StringComparison]::Ordinal)) { "*external*" } else { "*has no 'Missing' property*" })
    }
}

Complete-OpenApiRouteTests
