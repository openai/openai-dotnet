#Requires -Version 7.0

$script:OpenApiTestFixtureRoot = Join-Path $PSScriptRoot "fixtures"

function Read-TestJsonFixture {
    param(
        [Parameter(Mandatory = $true)]
        [string]$RelativePath
    )

    $path = Join-Path $script:OpenApiTestFixtureRoot $RelativePath
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) {
        throw "The test fixture was not found: $path"
    }

    return Get-Content -LiteralPath $path -Raw -Encoding utf8 |
        ConvertFrom-Json -AsHashtable -Depth 100
}

function New-TestOpenApiDocument {
    param(
        [Parameter(Mandatory = $true)]
        [AllowNull()]
        [object]$Schema
    )

    $document = Read-TestJsonFixture -RelativePath "_Shared/minimal.openapi.json"
    $document.paths["/test"].post.responses["200"].content["application/json"].schema = $Schema
    return $document
}

function Get-TestResponseSchema {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document
    )

    return $Document.paths["/test"].post.responses["200"].content["application/json"].schema
}

function Get-TestFixtureCase {
    param(
        [Parameter(Mandatory = $true)]
        [System.Collections.IDictionary]$Cases,

        [Parameter(Mandatory = $true)]
        [string]$Name
    )

    if (-not $Cases.Contains($Name)) {
        throw "Test fixture case '$Name' was not found."
    }

    return Copy-JsonValue -Value $Cases[$Name]
}
