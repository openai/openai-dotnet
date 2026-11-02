<#
.SYNOPSIS
    Runs focused tests for Normalize-OpenApiTypeArrays.ps1.
#>

[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$normalizerPath = Join-Path $PSScriptRoot ".." "pipeline" "Normalize-OpenApiTypeArrays.ps1"
. $normalizerPath
. (Join-Path $PSScriptRoot "_OpenApiTestFixture.Helpers.ps1")

$script:PassedTests = 0
$script:FailedTests = 0

function Invoke-Test {
    param([string]$Name, [scriptblock]$Test)
    try {
        & $Test
        $script:PassedTests++
        Write-Host "[PASS] $Name"
    }
    catch {
        $script:FailedTests++
        Write-Host "[FAIL] $Name`: $($_.Exception.Message)" -ForegroundColor Red
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

function Assert-Sequence {
    param([object[]]$Expected, [object[]]$Actual, [string]$Message)
    $expectedJson = ConvertTo-Json -InputObject @($Expected) -Compress -Depth 100
    $actualJson = ConvertTo-Json -InputObject @($Actual) -Compress -Depth 100
    if ($expectedJson -cne $actualJson) { throw "$Message Expected '$expectedJson', got '$actualJson'." }
}

function Assert-Throws {
    param([scriptblock]$Action, [string]$MessagePattern)
    try { & $Action }
    catch {
        if ($_.Exception.Message -notlike $MessagePattern) {
            throw "Expected '$MessagePattern', got '$($_.Exception.Message)'."
        }
        return
    }
    throw "Expected an error matching '$MessagePattern'."
}

$cases = Read-TestJsonFixture -RelativePath "Normalize-OpenApiTypeArrays/cases.schema.json"

Invoke-Test "nullable objects and enums become separate non-null and null branches" {
    $source = New-TestOpenApiDocument -Schema (Get-TestFixtureCase -Cases $cases -Name "nullableObject")
    $before = ConvertTo-Json -InputObject $source -Compress -Depth 100
    $result = ConvertTo-NormalizedOpenApiTypeArrayDocument -Document $source
    $schema = Get-TestResponseSchema -Document $result.Document
    Assert-Equal 2 $result.NormalizedTypeArrayCount "Normalized schema count was incorrect."
    Assert-Equal "object" $schema.anyOf[0].type "Object branch did not have a scalar type."
    Assert-Equal $false $schema.anyOf[0].additionalProperties "Object constraint was lost."
    Assert-Equal "null" $schema.anyOf[1].type "Object null branch was missing."
    $mode = $schema.anyOf[0].properties.mode
    Assert-Sequence @("explicit", "implicit") @($mode.anyOf[0].enum) "Non-null enum retained null or changed its values."
    Assert-Equal "null" $mode.anyOf[1].type "Mode null branch was missing."
    Assert-True (-not (Test-JsonProperty -Object $mode.anyOf[1] -Name "enum")) "Null branch redundantly declared a null enum."
    Assert-Equal $before (ConvertTo-Json -InputObject $source -Compress -Depth 100) "Normalization mutated its source."
    $second = ConvertTo-NormalizedOpenApiTypeArrayDocument -Document $result.Document
    Assert-Equal 0 $second.NormalizedTypeArrayCount "Second pass normalized more schemas."
    Assert-True (Test-JsonValuesEqual -Left $result.Document -Right $second.Document) "Second pass changed the document."
}

Invoke-Test "an enum excluding null does not gain a null branch" {
    $result = ConvertTo-NormalizedOpenApiTypeArrayDocument -Document (
        New-TestOpenApiDocument -Schema (Get-TestFixtureCase -Cases $cases -Name "enumWithoutNull")
    )
    $schema = Get-TestResponseSchema -Document $result.Document
    Assert-Equal "string" $schema.type "Finite enum did not reduce to a scalar string schema."
    Assert-Sequence @("explicit", "implicit") @($schema.enum) "Finite enum values changed."
    Assert-True (-not (Test-JsonProperty -Object $schema -Name "anyOf")) "An excluded null branch was introduced."
}

Invoke-Test "all JSON types expand in stable order with applicable constraints" {
    $result = ConvertTo-NormalizedOpenApiTypeArrayDocument -Document (
        New-TestOpenApiDocument -Schema (Get-TestFixtureCase -Cases $cases -Name "allTypes")
    )
    $schema = Get-TestResponseSchema -Document $result.Document
    Assert-Equal 3 $result.NormalizedTypeArrayCount "Copied common constraints changed the normalized source count."
    Assert-Sequence @("object", "array", "string", "number", "integer", "boolean", "null") @($schema.anyOf.type) "Type ordering changed or null was not last."
    Assert-Equal "Preserved annotation" $schema.title "Annotation was lost."
    Assert-Sequence @("ignored", "literal") @($schema.'x-note'.type) "Extension data changed."
    Assert-Sequence @("child") @($schema.anyOf[0].required) "Required-property membership changed."
    Assert-Equal $false $schema.anyOf[0].additionalProperties "Object constraint changed."
    Assert-Equal 1 $schema.anyOf[1].minItems "Array constraint changed."
    Assert-Equal "^example$" $schema.anyOf[2].pattern "String constraint changed."
    foreach ($index in @(3, 4)) { Assert-Equal 0 $schema.anyOf[$index].minimum "Numeric constraint changed." }
    foreach ($index in 1..6) { Assert-True (-not (Test-JsonProperty -Object $schema.anyOf[$index] -Name "properties")) "Object keyword leaked into another type."
    }
    foreach ($branch in $schema.anyOf) { Assert-Equal "custom-format" $branch.format "Format was dropped or treated as string-only." }
    Assert-NoOpenApiSchemaTypeArrays -Value $result.Document
}

Invoke-Test "number and integer remain overlapping anyOf alternatives without coercion" {
    $result = ConvertTo-NormalizedOpenApiTypeArrayDocument -Document (
        New-TestOpenApiDocument -Schema (Get-TestFixtureCase -Cases $cases -Name "overlappingNumbers")
    )
    $schema = Get-TestResponseSchema -Document $result.Document
    Assert-Sequence @("number", "integer") @($schema.anyOf.type) "Overlapping numeric types were not preserved."
    Assert-Sequence @([double]1, [double]1.5) @($schema.anyOf[0].enum) "Numeric enum was coerced or retained another JSON kind."
    Assert-Equal 1 $schema.anyOf[1].enum.Count "Fractional number was admitted to integer."
    Assert-Equal 1 $schema.anyOf[1].enum[0] "Mathematically integral JSON number did not satisfy integer."
    Assert-True (-not (Test-JsonProperty -Object $schema -Name "oneOf")) "Overlapping numeric types were changed to oneOf."
}

Invoke-Test "finite enum partitioning preserves object array string and boolean JSON kinds" {
    $sourceSchema = Get-TestFixtureCase -Cases $cases -Name "jsonKinds"
    $result = ConvertTo-NormalizedOpenApiTypeArrayDocument -Document (New-TestOpenApiDocument -Schema $sourceSchema)
    $schema = Get-TestResponseSchema -Document $result.Document
    for ($index = 0; $index -lt 4; $index++) {
        Assert-Equal 1 $schema.anyOf[$index].enum.Count "Enum values were not partitioned by kind."
        Assert-True (Test-OpenApiTypeArrayValuesEqual -Left $sourceSchema.enum[$index] -Right $schema.anyOf[$index].enum[0]) "An enum member changed its JSON value."
    }
    Assert-Equal "null" $schema.anyOf[4].type "Null was not isolated."
}

Invoke-Test "singleton arrays and const intersections reduce without widening" {
    foreach ($typeName in @("object", "array", "string", "number", "integer", "boolean", "null")) {
        $result = ConvertTo-NormalizedOpenApiTypeArrayDocument -Document (New-TestOpenApiDocument -Schema ([ordered]@{ type = @($typeName) }))
        Assert-Equal $typeName (Get-TestResponseSchema -Document $result.Document).type "Singleton array did not become a scalar type."
    }
    foreach ($value in @("explicit", $null)) {
        $schema = [ordered]@{ type = @("string", "null"); const = $value; enum = @("explicit", "implicit", $null) }
        $result = ConvertTo-NormalizedOpenApiTypeArrayDocument -Document (New-TestOpenApiDocument -Schema $schema)
        $normalized = Get-TestResponseSchema -Document $result.Document
        if ($null -eq $value) {
            Assert-Equal "null" $normalized.type "Null const did not exclude other types."
            Assert-True (-not (Test-JsonProperty -Object $normalized -Name "const")) "Null const was redundantly retained."
        }
        else {
            Assert-Equal "string" $normalized.type "String const did not exclude null."
            Assert-Equal "explicit" $normalized.const "Non-null const changed."
            Assert-Sequence @("explicit") @($normalized.enum) "Const and enum were not intersected."
        }
    }
    $impossible = [ordered]@{ type = @("string", "null"); const = "excluded"; enum = @("explicit") }
    $result = ConvertTo-NormalizedOpenApiTypeArrayDocument -Document (New-TestOpenApiDocument -Schema $impossible)
    Assert-Equal $false (Get-TestResponseSchema -Document $result.Document) "Unsatisfiable constraints became unrestricted or an empty union."
}

Invoke-Test "existing applicators remain conjunctive including on the null branch" {
    $schema = [ordered]@{
        type = @("string", "null")
        allOf = @([ordered]@{ type = "string" })
        not = [ordered]@{ const = "excluded" }
        if = [ordered]@{ const = "example" }
        then = [ordered]@{ enum = @("example") }
        else = [ordered]@{ not = [ordered]@{ const = "other" } }
    }
    $result = ConvertTo-NormalizedOpenApiTypeArrayDocument -Document (New-TestOpenApiDocument -Schema $schema)
    foreach ($branch in (Get-TestResponseSchema -Document $result.Document).anyOf) {
        Assert-Equal "string" $branch.allOf[0].type "Existing allOf was bypassed."
        Assert-Equal "excluded" $branch.not.const "Existing not was lost."
        Assert-Equal "example" $branch.if.const "Existing if was lost."
        Assert-Equal "example" $branch.then.enum[0] "Existing then was lost."
        Assert-Equal "other" $branch.else.not.const "Existing else was lost."
    }
}

Invoke-Test "schema contexts include maps items definitions and conditionals but exclude literals" {
    $nullable = [ordered]@{ type = @("string", "null") }
    $schema = [ordered]@{
        type = "object"
        properties = [ordered]@{ type = $nullable }
        additionalProperties = $nullable
        '$defs' = [ordered]@{ Value = $nullable }
        if = $nullable
        then = $nullable
        else = $nullable
        prefixItems = @($nullable)
        example = [ordered]@{ type = @("not-a-type", "literal") }
        default = [ordered]@{ type = @("literal") }
    }
    $source = New-TestOpenApiDocument -Schema $schema
    $source["components"] = [ordered]@{ schemas = [ordered]@{ Existing = $nullable } }
    $result = ConvertTo-NormalizedOpenApiTypeArrayDocument -Document $source
    Assert-Equal 8 $result.NormalizedTypeArrayCount "Schema contexts were missed or literal data was interpreted as schemas."
    Assert-NoOpenApiSchemaTypeArrays -Value $result.Document
    $normalized = Get-TestResponseSchema -Document $result.Document
    Assert-Sequence @("not-a-type", "literal") @($normalized.example.type) "Example data changed."
    Assert-Sequence @("literal") @($normalized.default.type) "Default data changed."
}

Invoke-Test "malformed type arrays and enums fail with schema breadcrumbs" {
    foreach ($entry in $cases.invalidTypes.GetEnumerator()) {
        $source = New-TestOpenApiDocument -Schema ([ordered]@{ type = $entry.Value })
        Assert-Throws { ConvertTo-NormalizedOpenApiTypeArrayDocument -Document $source } "*#/paths/~1test/post/responses/200/content/application~1json/schema/type*"
    }
    foreach ($enum in @("not-an-array", @(), @("duplicate", "duplicate"), @([long]1, [double]1))) {
        $source = New-TestOpenApiDocument -Schema ([ordered]@{ type = @("string", "number", "null"); enum = $enum })
        Assert-Throws { ConvertTo-NormalizedOpenApiTypeArrayDocument -Document $source } "*schema/enum*"
    }
}

Invoke-Test "scope-sensitive candidates and inherited URI scopes fail without moving anchors" {
    foreach ($keyword in @('$id', '$anchor', '$dynamicAnchor', '$recursiveAnchor')) {
        $schema = Get-TestFixtureCase -Cases $cases -Name "nullableObject"
        $schema[$keyword] = "scope"
        Assert-Throws {
            ConvertTo-NormalizedOpenApiTypeArrayDocument -Document (New-TestOpenApiDocument -Schema $schema)
        } "*scope-sensitive keyword '$keyword'*"
    }
    $inherited = [ordered]@{ '$id' = "urn:scope"; type = "object"; properties = [ordered]@{ child = [ordered]@{ type = @("string", "null") } } }
    Assert-Throws {
        ConvertTo-NormalizedOpenApiTypeArrayDocument -Document (New-TestOpenApiDocument -Schema $inherited)
    } "*inherits URI scope*"
}

Invoke-Test "nested descendants and self-references relocate while root references remain whole" {
    $root = "#/paths/~1test/post/responses/200/content/application~1json/schema"
    $schema = Get-TestFixtureCase -Cases $cases -Name "nullableObject"
    $schema.properties["root_alias"] = [ordered]@{ '$ref' = $root }
    $schema.properties["mode_alias"] = [ordered]@{ '$ref' = "$root/properties/mode" }
    $schema.properties["enum_alias"] = [ordered]@{ '$dynamicRef' = "$root/properties/mode/enum/1" }
    $schema["example"] = [ordered]@{ type = @("object", "null"); '$ref' = "$root/properties/mode" }
    $result = ConvertTo-NormalizedOpenApiTypeArrayDocument -Document (New-TestOpenApiDocument -Schema $schema)
    $normalized = Get-TestResponseSchema -Document $result.Document
    Assert-Equal $root $normalized.anyOf[0].properties.root_alias.'$ref' "Root reference targeted only one branch."
    Assert-Equal "$root/anyOf/0/properties/mode" $normalized.anyOf[0].properties.mode_alias.'$ref' "Nested schema reference did not relocate."
    Assert-Equal "$root/anyOf/0/properties/mode/anyOf/0/enum/1" $normalized.anyOf[0].properties.enum_alias.'$dynamicRef' "Nested enum entry did not relocate."
    Assert-Equal "$root/properties/mode" $normalized.example.'$ref' "Literal example reference changed."
    Assert-Sequence @("object", "null") @($normalized.example.type) "Literal type array changed."
}

Invoke-Test "existing anyOf is retained conjunctively and its original pointer moves once" {
    $root = "#/paths/~1test/post/responses/200/content/application~1json/schema"
    $schema = [ordered]@{
        type = @("object", "null")
        properties = [ordered]@{ alias = [ordered]@{ '$ref' = "$root/anyOf/0" } }
        anyOf = @([ordered]@{ type = "object" })
    }
    $result = ConvertTo-NormalizedOpenApiTypeArrayDocument -Document (New-TestOpenApiDocument -Schema $schema)
    $normalized = Get-TestResponseSchema -Document $result.Document
    Assert-Equal "$root/anyOf/0/anyOf/0" $normalized.anyOf[0].properties.alias.'$ref' "Existing composition pointer moved repeatedly."
    Assert-Equal "object" $normalized.anyOf[0].anyOf[0].type "Existing object composition was overwritten."
    Assert-Equal "object" $normalized.anyOf[1].anyOf[0].type "Null branch bypassed the existing composition."
}

Invoke-Test "references to filtered enum entries fail instead of changing values" {
    $root = "#/paths/~1test/post/responses/200/content/application~1json/schema"
    $schema = Get-TestFixtureCase -Cases $cases -Name "nullableObject"
    $schema.properties["alias"] = [ordered]@{ '$ref' = "$root/properties/mode/enum/2" }
    Assert-Throws {
        ConvertTo-NormalizedOpenApiTypeArrayDocument -Document (New-TestOpenApiDocument -Schema $schema)
    } "*Cannot preserve reference*normalization removes that target*"
}

Invoke-Test "mapping targets escaped paths and recursive references follow the retained object branch" {
    $root = "#/paths/~1test/post/responses/200/content/application~1json/schema"
    $schema = [ordered]@{
        type = "object"
        properties = [ordered]@{
            'a/b~c' = [ordered]@{ type = @("null", "object"); properties = [ordered]@{ value = [ordered]@{ type = @("string", "null") } } }
            alias = [ordered]@{ '$recursiveRef' = "$root/properties/a~1b~0c/properties/value" }
            owner = [ordered]@{ discriminator = [ordered]@{ propertyName = "kind"; mapping = [ordered]@{ example = "$root/properties/a~1b~0c/properties/value" } } }
            external = [ordered]@{ '$ref' = "https://example.test/schema.json" }
        }
    }
    $result = ConvertTo-NormalizedOpenApiTypeArrayDocument -Document (New-TestOpenApiDocument -Schema $schema)
    $normalized = Get-TestResponseSchema -Document $result.Document
    $expected = "$root/properties/a~1b~0c/anyOf/0/properties/value"
    Assert-Equal $expected $normalized.properties.alias.'$recursiveRef' "Escaped recursive reference did not relocate."
    Assert-Equal $expected $normalized.properties.owner.discriminator.mapping.example "Discriminator mapping did not relocate."
    Assert-Equal "https://example.test/schema.json" $normalized.properties.external.'$ref' "External reference changed or required fetching."
}

Invoke-Test "shared schema subtrees use a deterministic canonical reference target" {
    $root = "#/paths/~1test/post/responses/200/content/application~1json/schema"
    $schema = [ordered]@{
        type = @("object", "null")
        '$defs' = [ordered]@{ Value = [ordered]@{ type = @("string", "null") } }
        properties = [ordered]@{ value = [ordered]@{ '$ref' = "$root/`$defs/Value" } }
    }
    $result = ConvertTo-NormalizedOpenApiTypeArrayDocument -Document (New-TestOpenApiDocument -Schema $schema)
    $normalized = Get-TestResponseSchema -Document $result.Document
    Assert-Equal 2 $result.NormalizedTypeArrayCount "Copied definitions were counted twice."
    Assert-Equal "$root/anyOf/0/`$defs/Value" $normalized.anyOf[0].properties.value.'$ref' "Definition reference did not use the first retained copy."
    Assert-NoOpenApiSchemaTypeArrays -Value $result.Document
}

Invoke-Test "CLI paths encoding failure atomicity and byte idempotence follow stage conventions" {
    $directory = Join-Path ([System.IO.Path]::GetTempPath()) "normalize-type-arrays-$([Guid]::NewGuid().ToString('N'))"
    $null = [System.IO.Directory]::CreateDirectory($directory)
    try {
        $inputPath = Join-Path $directory "sample.openapi.json"
        $source = New-TestOpenApiDocument -Schema (Get-TestFixtureCase -Cases $cases -Name "nullableObject")
        $null = Write-OpenApiJsonFile -Document $source -Path $inputPath
        $sourceHash = (Get-FileHash -LiteralPath $inputPath).Hash
        $first = Invoke-OpenApiTypeArrayNormalizer -InputPath $inputPath 6>$null
        $output = Join-Path $directory "sample.type-arrays-normalized.openapi.json"
        $bytes = [System.IO.File]::ReadAllBytes($output)
        Assert-Equal 10 $bytes[-1] "Output did not end with LF."
        Assert-True (-not ($bytes.Length -ge 3 -and $bytes[0] -eq 239 -and $bytes[1] -eq 187 -and $bytes[2] -eq 191)) "Output contained a BOM."
        Assert-True (-not ([System.Text.Encoding]::UTF8.GetString($bytes).Contains("`r"))) "Output contained CRLF newlines."
        Assert-Equal $sourceHash (Get-FileHash -LiteralPath $inputPath).Hash "Input file changed."
        $outputHash = (Get-FileHash -LiteralPath $output).Hash
        $second = Invoke-OpenApiTypeArrayNormalizer -InputPath $output 6>$null
        Assert-Equal 0 $second.NormalizedTypeArrayCount "Second CLI pass normalized more schemas."
        Assert-Equal $outputHash (Get-FileHash -LiteralPath $output).Hash "Second CLI pass changed bytes."
        $explicit = Join-Path $directory "explicit.json"
        $null = Invoke-OpenApiTypeArrayNormalizer -InputPath $inputPath -OutputPath $explicit 6>$null
        Assert-Equal $outputHash (Get-FileHash -LiteralPath $explicit).Hash "Explicit output differed."
        $invalid = New-TestOpenApiDocument -Schema ([ordered]@{ type = @("invalid") })
        $null = Write-OpenApiJsonFile -Document $invalid -Path $inputPath
        Assert-Throws { Invoke-OpenApiTypeArrayNormalizer -InputPath $inputPath -OutputPath $output } "*unsupported JSON Schema type*"
        Assert-Equal $outputHash (Get-FileHash -LiteralPath $output).Hash "Failed CLI invocation overwrote existing output."
        Assert-Throws { Invoke-OpenApiTypeArrayNormalizer -InputPath (Join-Path $directory "missing.json") } "*cannot find path*"
        Assert-Throws { Invoke-OpenApiTypeArrayNormalizer -InputPath $directory } "*is not a file*"
        [System.IO.File]::WriteAllText($inputPath, "{ invalid")
        Assert-Throws { Invoke-OpenApiTypeArrayNormalizer -InputPath $inputPath } "*Could not parse OpenAPI JSON*"
        Assert-Equal (Join-Path $directory "plain.type-arrays-normalized.json") (Get-DefaultNormalizedOpenApiTypeArrayPath -InputPath (Join-Path $directory "plain.json")) "Plain JSON suffix was incorrect."
    }
    finally {
        [System.IO.Directory]::Delete($directory, $true)
    }
}

Invoke-Test "single-pass relocation does not reapply a source prefix inside its destination" {
    $relocations = [System.Collections.Generic.Dictionary[string, string]]::new([System.StringComparer]::Ordinal)
    $relocations["#/schema/anyOf"] = "#/schema/anyOf/0/anyOf"
    Assert-Equal "#/schema/anyOf/0/anyOf/1" (Get-RelocatedOpenApiPointer -Pointer "#/schema/anyOf/1" -Relocations $relocations -SinglePass) "Single-pass relocation repeated a move."
    $relocations.Clear()
    $relocations["#/first"] = "#/second"
    $relocations["#/second"] = "#/third"
    Assert-Equal "#/third/value" (Get-RelocatedOpenApiPointer -Pointer "#/first/value" -Relocations $relocations) "Default iterative relocation changed."
    Assert-Equal "#/second/value" (Get-RelocatedOpenApiPointer -Pointer "#/first/value" -Relocations $relocations -SinglePass) "Single-pass mode followed a second source prefix."
}

Invoke-Test "the type-array postcondition ignores literals and reports schema breadcrumbs" {
    $source = New-TestOpenApiDocument -Schema (Get-TestFixtureCase -Cases $cases -Name "nullableObject")
    Assert-Throws { Assert-NoOpenApiSchemaTypeArrays -Value $source } "*#/paths/~1test/post/responses/200/content/application~1json/schema/type*Normalize-OpenApiTypeArrays*"
    $literalOnly = New-TestOpenApiDocument -Schema ([ordered]@{ type = "string"; example = [ordered]@{ type = @("object", "null") } })
    Assert-NoOpenApiSchemaTypeArrays -Value $literalOnly
}

Write-Host ""
Write-Host "Passed: $script:PassedTests"
Write-Host "Failed: $script:FailedTests"
if ($script:FailedTests -gt 0) { exit 1 }