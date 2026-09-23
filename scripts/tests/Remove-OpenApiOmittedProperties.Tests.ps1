<#
.SYNOPSIS
    Runs focused tests for scripts/Remove-OpenApiOmittedProperties.ps1.
#>

[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$extractScriptPath = Join-Path $PSScriptRoot ".." "Extract-OpenApiRoute.ps1"
$removeScriptPath = Join-Path $PSScriptRoot ".." "Remove-OpenApiOmittedProperties.ps1"
. $extractScriptPath
. $removeScriptPath
. (Join-Path $PSScriptRoot "_OpenApiRouteTest.Helpers.ps1")
Initialize-OpenApiRouteTestState

$temporaryDirectory = Join-Path ([System.IO.Path]::GetTempPath()) "remove-openapi-properties-$([Guid]::NewGuid().ToString('N'))"
$null = [System.IO.Directory]::CreateDirectory($temporaryDirectory)

try {
    $context = New-OpenApiRouteTestContext
    $fixture = $context.Fixture
    $fixtureCases = $context.Cases
    $operationEntry = $context.OperationEntry
    $extracted = (New-ExtractedOpenApiDocument `
        -Document $fixture `
        -OperationEntry $operationEntry).Document
    $omission = ConvertTo-OmittedOpenApiPropertiesDocument -Document $extracted
    $omitted = $omission.Document

    Invoke-Test "omission predicate is configured and case-sensitive" {
        foreach ($name in @("example", "examples", "title", "description", "default", "x-foo")) {
            Assert-True (Test-IsOmittedOpenApiProperty -Name $name) "'$name' should be omitted."
        }
        foreach ($name in @("Title", "Description", "Default", "Example", "Examples", "X-foo")) {
            Assert-True (-not (Test-IsOmittedOpenApiProperty -Name $name)) "'$name' should be retained."
        }
        Assert-True (-not (Test-IsOmittedOpenApiProperty -Name "title" -ParentBreadcrumb "#/info")) "info.title should be retained."
        Assert-True (-not (Test-IsOmittedOpenApiProperty -Name "default" -ParentBreadcrumb "#/paths/~1test/post/responses")) "Response default key should be retained."
        Assert-True (-not (Test-IsOmittedOpenApiProperty -Name "description" -ParentObject ([ordered]@{}) -ParentBreadcrumb "#/paths/~1test/post/responses/default")) "Response description should be retained."
        foreach ($name in @("title", "description", "default")) {
            Assert-True (-not (Test-IsOmittedOpenApiProperty -Name $name -ParentContext SchemaMap)) "Schema-map name '$name' should be retained."
        }
    }

    Invoke-Test "adding an exact name to the omission array needs no code changes" {
        $originalOmittedPropertyNames = @($script:OmittedPropertyNames)
        try {
            $script:OmittedPropertyNames = @($script:OmittedPropertyNames) + "temporaryOmit"
            $pruned = ConvertTo-PrunedOpenApiValue -Value (
                Get-TestFixtureCase -Cases $fixtureCases -Name "configuredOmission"
            )
            Assert-True (-not (Test-JsonProperty -Object $pruned -Name "temporaryOmit")) "Configured property remained."
            Assert-Equal "value" $pruned.keep "Unconfigured property was removed."
        }
        finally {
            $script:OmittedPropertyNames = $originalOmittedPropertyNames
        }
    }

    Invoke-Test "pruning preserves surviving object and array order" {
        $ordinary = Get-TestFixtureCase -Cases $fixtureCases -Name "pruningOrder"
        $pruned = ConvertTo-PrunedOpenApiValue -Value $ordinary
        Assert-Sequence `
            @("required", "properties", "type") `
            @((Get-JsonProperties -Value $pruned).Name) `
            "Pruning changed surviving object order."
        Assert-Sequence `
            @("untouched") `
            @((Get-JsonProperties -Value $pruned.properties).Name) `
            "Pruning changed a nested object."
    }

    Invoke-Test "schema reference metadata siblings are removed before inlining" {
        $alias = $omitted.components.schemas.Alias
        Assert-Equal "#/components/schemas/Cat" $alias.'$ref' "Omission changed the schema reference."
        Assert-True (-not (Test-JsonProperty -Object $alias -Name "description")) "Reference sibling description remained."
        $requiredA = $omitted.paths["/things"].post.requestBody.content["application/json"].schema.properties.requiredA
        Assert-Equal "#/components/schemas/Alias" $requiredA.'$ref' "Route reference was expanded during omission."
    }

    Invoke-Test "response reference descriptions are removed before inlining" {
        $responseFixture = Copy-JsonValue -Value $fixture
        $responseFixture.paths["/things"].post.responses["200"].description = "Discard response reference override"
        $responseOperation = Select-OpenApiOperation `
            -Catalog (Get-OpenApiOperationCatalog -Document $responseFixture) `
            -RequestedOperationId "createThing"
        $responseExtract = (New-ExtractedOpenApiDocument `
            -Document $responseFixture `
            -OperationEntry $responseOperation `
            -Mode Responses).Document
        $responseOmitted = (ConvertTo-OmittedOpenApiPropertiesDocument -Document $responseExtract).Document
        $response = $responseOmitted.paths["/things"].post.responses["200"]
        Assert-Equal "#/components/responses/Shared" $response.'$ref' "Response reference changed."
        Assert-True (-not (Test-JsonProperty -Object $response -Name "description")) "Response reference description remained."
        Assert-Equal "ok" $responseOmitted.components.responses.Shared.description "Concrete target description changed."
    }

    Invoke-Test "metadata names remain valid payload fields in schema maps" {
        $metadataFixture = Copy-JsonValue -Value $fixture
        $metadataFixture.paths["/things"].post.requestBody.content["application/json"].schema = `
            Get-TestFixtureCase -Cases $fixtureCases -Name "metadataAndPayloadNames"
        $metadataOperation = Select-OpenApiOperation `
            -Catalog (Get-OpenApiOperationCatalog -Document $metadataFixture) `
            -RequestedOperationId "createThing"
        $metadataExtract = (New-ExtractedOpenApiDocument `
            -Document $metadataFixture `
            -OperationEntry $metadataOperation `
            -Mode RequestBody).Document
        $metadataOmitted = (ConvertTo-OmittedOpenApiPropertiesDocument -Document $metadataExtract).Document
        $schema = $metadataOmitted.paths["/things"].post.requestBody.content["application/json"].schema
        Assert-Sequence @("type", "properties", "required") @((Get-JsonProperties -Value $schema).Name) "Schema metadata remained."
        Assert-Sequence @("title", "description", "default") @((Get-JsonProperties -Value $schema.properties).Name) "Payload field names changed."
        Assert-Sequence @("title", "description", "default") @($schema.required) "Required payload names changed."
        Assert-True (-not (Test-JsonProperty -Object $schema.properties.title -Name "description")) "Field description annotation remained."
        Assert-True (-not (Test-JsonProperty -Object $schema.properties.description -Name "title")) "Field title annotation remained."
        Assert-True (-not (Test-JsonProperty -Object $schema.properties.default -Name "description")) "Reference description annotation remained."
        Assert-Equal "#/components/schemas/A" $schema.properties.default.'$ref' "Payload field reference was expanded."
    }

    Invoke-Test "configured and x-prefixed properties are removed without mutating the source" {
        Assert-Equal 0 @(Find-OmittedOpenApiPropertyNames -Value $omitted).Count "Omitted properties remained."
        Assert-True ($omission.RemovedPropertyCount -gt 0) "Removed-property count was not reported."
        Assert-Equal "Extractor fixture" $omitted.info.title "Required info.title was removed."
        Assert-True (Test-JsonProperty -Object $fixture -Name "x-custom-root") "Source root extension was mutated."
        Assert-True (Test-JsonProperty -Object $fixture.info -Name "x-custom-info") "Source info extension was mutated."
        Assert-True (Test-JsonProperty -Object $fixture.paths["/things"].post -Name "x-hidden-ref") "Source operation extension was mutated."
        Assert-True (-not (Test-JsonProperty -Object $omitted.paths["/things"].post -Name "x-hidden-ref")) "Output operation extension remained."
        Assert-True (-not (Test-JsonProperty -Object $omitted.components.schemas.Cat -Name "title")) "Schema title remained."
        Assert-True (Test-JsonProperty -Object $omitted.components.schemas -Name "Unused") "Omission removed a component entry."
        Assert-True (Test-JsonProperty -Object $omitted.components.schemas -Name "ExampleOnly") "Omission removed an example-only component entry."
        Assert-True (Test-JsonProperty -Object $omitted.components.schemas -Name "ExamplesOnly") "Omission removed an examples-only component entry."

        $inputPath = Join-Path $temporaryDirectory "sample.extracted.openapi.json"
        $outputPath = Join-Path $temporaryDirectory "sample.extracted.omitted.openapi.json"
        $null = Write-OpenApiJsonFile -Document $extracted -Path $inputPath
        [byte[]]$inputBefore = [System.IO.File]::ReadAllBytes($inputPath)
        $null = Invoke-OpenApiOmittedPropertyRemoval -InputPath $inputPath
        Assert-True ([System.IO.File]::Exists($outputPath)) "Default omitted output was not created."
        Assert-True ([System.Linq.Enumerable]::SequenceEqual($inputBefore, [System.IO.File]::ReadAllBytes($inputPath))) "Omission modified its input file."
        [byte[]]$bytes = [System.IO.File]::ReadAllBytes($outputPath)
        $hasBom = $bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF
        Assert-True (-not $hasBom) "Omitted output contained a UTF-8 BOM."
    }
}
finally {
    if ([System.IO.Directory]::Exists($temporaryDirectory)) {
        [System.IO.Directory]::Delete($temporaryDirectory, $true)
    }
}

Complete-OpenApiRouteTests
