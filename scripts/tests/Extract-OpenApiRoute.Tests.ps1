<#
.SYNOPSIS
    Runs focused tests for scripts/Extract-OpenApiRoute.ps1.

.DESCRIPTION
    Uses a small built-in assertion runner so the repository does not need an
    additional PowerShell test dependency. Most behavior is exercised against a
    compact fixture, followed by representative integration checks against the
    repository's current OpenAPI document.
#>

[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$scriptPath = Join-Path $PSScriptRoot ".." "Extract-OpenApiRoute.ps1"
. $scriptPath

$script:PassedTests = 0
$script:FailedTests = 0
$script:Failures = [System.Collections.Generic.List[string]]::new()

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
    param(
        [Parameter(Mandatory = $true)]
        [bool]$Condition,

        [Parameter(Mandatory = $true)]
        [string]$Message
    )

    if (-not $Condition) {
        throw $Message
    }
}

function Assert-Equal {
    param(
        [Parameter(Mandatory = $true)]
        [AllowNull()]
        [object]$Expected,

        [Parameter(Mandatory = $true)]
        [AllowNull()]
        [object]$Actual,

        [Parameter(Mandatory = $true)]
        [string]$Message
    )

    if ($Expected -ne $Actual) {
        throw "$Message Expected '$Expected', got '$Actual'."
    }
}

function Assert-Sequence {
    param(
        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [object[]]$Expected,

        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [object[]]$Actual,

        [Parameter(Mandatory = $true)]
        [string]$Message
    )

    $expectedText = $Expected -join "|"
    $actualText = $Actual -join "|"
    if (-not [string]::Equals($expectedText, $actualText, [System.StringComparison]::Ordinal)) {
        throw "$Message Expected '$expectedText', got '$actualText'."
    }
}

function Assert-Throws {
    param(
        [Parameter(Mandatory = $true)]
        [scriptblock]$Action,

        [Parameter(Mandatory = $true)]
        [string]$MessagePattern
    )

    try {
        & $Action
    }
    catch {
        if ($_.Exception.Message -notlike $MessagePattern) {
            throw "Expected error matching '$MessagePattern', got '$($_.Exception.Message)'."
        }
        return
    }

    throw "Expected an error matching '$MessagePattern', but no error was thrown."
}

# Recursively returns values of properties with an exact JSON property name.
function Find-JsonPropertyValues {
    param(
        [Parameter(Mandatory = $true)]
        [AllowNull()]
        [object]$Value,

        [Parameter(Mandatory = $true)]
        [string]$Name
    )

    if ($null -eq $Value) {
        return
    }
    if (Test-JsonArrayValue -Value $Value) {
        foreach ($item in $Value) {
            Find-JsonPropertyValues -Value $item -Name $Name
        }
        return
    }
    if (-not (Test-JsonObjectValue -Value $Value)) {
        return
    }

    foreach ($property in (Get-JsonProperties -Value $Value)) {
        if ([string]::Equals($property.Name, $Name, [System.StringComparison]::Ordinal)) {
            $property.Value
        }
        Find-JsonPropertyValues -Value $property.Value -Name $Name
    }
}

# Recursively returns every JSON property name selected by the production
# omission configuration, regardless of where it appears in the document.
function Find-OmittedOpenApiPropertyNames {
    param(
        [Parameter(Mandatory = $true)]
        [AllowNull()]
        [object]$Value
    )

    if ($null -eq $Value) {
        return
    }
    if (Test-JsonArrayValue -Value $Value) {
        foreach ($item in $Value) {
            Find-OmittedOpenApiPropertyNames -Value $item
        }
        return
    }
    if (-not (Test-JsonObjectValue -Value $Value)) {
        return
    }

    foreach ($property in (Get-JsonProperties -Value $Value)) {
        if (Test-IsOmittedOpenApiProperty -Name $property.Name) {
            $property.Name
        }
        Find-OmittedOpenApiPropertyNames -Value $property.Value
    }
}

# Recursively validates top-level key order for schema objects while leaving
# ordinary OpenAPI containers outside the assertion.
function Assert-OpenApiSchemaKeyOrder {
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

    if ($null -eq $Value) {
        return
    }

    if (Test-JsonArrayValue -Value $Value) {
        $itemContext = $Context
        if ([string]::Equals($Context, "SchemaArray", [System.StringComparison]::Ordinal)) {
            $itemContext = "Schema"
        }
        for ($index = 0; $index -lt $Value.Count; $index++) {
            Assert-OpenApiSchemaKeyOrder `
                -Value $Value[$index] `
                -Context $itemContext `
                -Breadcrumb "$Breadcrumb/$index"
        }
        return
    }

    if (-not (Test-JsonObjectValue -Value $Value)) {
        return
    }
    if ([string]::Equals($Context, "SchemaArray", [System.StringComparison]::Ordinal)) {
        $Context = "Schema"
    }

    if ([string]::Equals($Context, "Schema", [System.StringComparison]::Ordinal)) {
        [string[]]$actualNames = @(Get-JsonProperties -Value $Value | ForEach-Object Name)
        $prioritizedNames = [System.Collections.Generic.List[string]]::new()
        $seenNames = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
        foreach ($propertyName in $script:PrioritizedSchemaPropertyNames) {
            if ($seenNames.Add($propertyName) -and
                (Test-JsonProperty -Object $Value -Name $propertyName)) {
                $prioritizedNames.Add($propertyName)
            }
        }
        [string[]]$remainingNames = @($actualNames | Where-Object {
            -not $seenNames.Contains($_)
        })
        [object[]]$expectedNames = @(
            $prioritizedNames
            (Sort-OrdinalStrings -Values $remainingNames)
        )
        Assert-Sequence `
            -Expected $expectedNames `
            -Actual $actualNames `
            -Message "Schema keys at '$Breadcrumb' were ordered incorrectly."

        $properties = Get-JsonPropertyValue -Object $Value -Name "properties"
        if (Test-JsonObjectValue -Value $properties) {
            $requiredNames = [System.Collections.Generic.HashSet[string]]::new(
                [System.StringComparer]::Ordinal
            )
            $required = Get-JsonPropertyValue -Object $Value -Name "required"
            if (Test-JsonArrayValue -Value $required) {
                foreach ($requiredName in $required) {
                    if ($requiredName -is [string]) {
                        $null = $requiredNames.Add($requiredName)
                    }
                }
            }
            [string[]]$actualPropertyNames = @(
                Get-JsonProperties -Value $properties | ForEach-Object Name
            )
            [string[]]$expectedPropertyNames = Get-OrderedSchemaPropertyNames `
                -PropertyNames $actualPropertyNames `
                -RequiredNames $requiredNames
            Assert-Sequence `
                -Expected $expectedPropertyNames `
                -Actual $actualPropertyNames `
                -Message "Properties at '$Breadcrumb/properties' were ordered incorrectly."
        }
    }

    foreach ($property in (Get-JsonProperties -Value $Value)) {
        $childContext = Get-OpenApiNormalizationChildContext `
            -ParentContext $Context `
            -PropertyName $property.Name `
            -PropertyValue $property.Value
        Assert-OpenApiSchemaKeyOrder `
            -Value $property.Value `
            -Context $childContext `
            -Breadcrumb "$Breadcrumb/$(ConvertTo-JsonPointerToken -Token $property.Name)"
    }
}

# The fixture includes unrelated entries so minimization can be asserted, not
# merely inferred from the absence of errors.
function New-TestOpenApiDocument {
    return [ordered]@{
        openapi = "3.1.0"
        info = [ordered]@{
            title = "Extractor fixture"
            version = "1.0.0"
            "x-oaiMeta" = [ordered]@{ ignored = $true }
            "x-custom-info" = "ignored"
        }
        servers = @([ordered]@{ url = "https://example.test/v1" })
        security = @([ordered]@{ ApiKeyAuth = @() })
        tags = @(
            [ordered]@{ name = "Things"; description = "Selected tag" },
            [ordered]@{ name = "Unused"; description = "Unselected tag" }
        )
        paths = [ordered]@{
            "/zebra/items" = [ordered]@{
                get = [ordered]@{ responses = [ordered]@{ "204" = [ordered]@{ description = "none" } } }
            }
            "/responses?beta=true" = [ordered]@{
                post = [ordered]@{ responses = [ordered]@{ "204" = [ordered]@{ description = "none" } } }
            }
            "/things" = [ordered]@{
                parameters = @(
                    [ordered]@{ name = "trace"; in = "header"; schema = [ordered]@{ type = "string" } }
                )
                get = [ordered]@{
                    tags = @("Things")
                    responses = [ordered]@{ "204" = [ordered]@{ description = "none" } }
                }
                post = [ordered]@{
                    tags = @("Things")
                    security = @([ordered]@{ AdminApiKeyAuth = @() })
                    example = [ordered]@{ '$ref' = "#/components/schemas/ExampleOnly" }
                    examples = [ordered]@{
                        hidden = [ordered]@{ '$ref' = "#/components/schemas/ExamplesOnly" }
                    }
                    requestBody = [ordered]@{
                        content = [ordered]@{
                            "application/json" = [ordered]@{
                                schema = [ordered]@{
                                    type = "object"
                                    properties = [ordered]@{
                                        optionalZ = [ordered]@{ type = "string" }
                                        requiredB = [ordered]@{ '$ref' = "#/components/schemas/A" }
                                        recursive = [ordered]@{ '$ref' = "#/components/schemas/Recursive" }
                                        requiredA = [ordered]@{ '$ref' = "#/components/schemas/Alias" }
                                        flattened = [ordered]@{
                                            description = "Outer description"
                                            allOf = @(
                                                [ordered]@{
                                                    type = "object"
                                                    description = "Base description"
                                                    nullable = $true
                                                    properties = [ordered]@{
                                                        optionalZ = [ordered]@{ type = "string" }
                                                        shared = [ordered]@{
                                                            description = "Shared property"
                                                            type = "boolean"
                                                        }
                                                        requiredB = [ordered]@{ type = "integer" }
                                                    }
                                                    required = @("requiredB")
                                                },
                                                [ordered]@{
                                                    allOf = @(
                                                        [ordered]@{
                                                            type = "object"
                                                            properties = [ordered]@{
                                                                optionalA = [ordered]@{ type = "string" }
                                                                requiredA = [ordered]@{ type = "string" }
                                                            }
                                                            required = @("requiredA")
                                                        },
                                                        [ordered]@{
                                                            type = "object"
                                                            description = "Nested description"
                                                            properties = [ordered]@{
                                                                requiredC = [ordered]@{ type = "boolean" }
                                                                shared = [ordered]@{
                                                                    type = "boolean"
                                                                    description = "Shared property"
                                                                }
                                                                optionalM = [ordered]@{ type = "number" }
                                                            }
                                                            required = @("requiredC", "requiredA")
                                                        }
                                                    )
                                                }
                                            )
                                        }
                                        type = [ordered]@{
                                            type = "string"
                                            description = "The request item type."
                                        }
                                    }
                                    required = @("requiredB", "requiredA", "type")
                                }
                            }
                        }
                    }
                    responses = [ordered]@{
                        "200" = [ordered]@{ '$ref' = "#/components/responses/Shared" }
                    }
                    "x-oaiMeta" = [ordered]@{ examples = "ignored" }
                    "x-hidden-ref" = [ordered]@{ '$ref' = "#/components/schemas/Unused" }
                }
            }
            "/fine_tuning/jobs" = [ordered]@{
                get = [ordered]@{ responses = [ordered]@{ "204" = [ordered]@{ description = "none" } } }
            }
        }
        webhooks = [ordered]@{
            ignored = [ordered]@{ post = [ordered]@{ responses = [ordered]@{} } }
        }
        components = [ordered]@{
            schemas = [ordered]@{
                A = [ordered]@{
                    type = "object"
                    properties = [ordered]@{
                        child = [ordered]@{ '$ref' = "#/components/schemas/B" }
                    }
                }
                B = [ordered]@{
                    allOf = @(
                        [ordered]@{
                            type = "object"
                            properties = [ordered]@{
                                parent = [ordered]@{ '$ref' = "#/components/schemas/A" }
                            }
                        },
                        [ordered]@{
                            type = "object"
                            properties = [ordered]@{
                                cycleOptional = [ordered]@{ type = "string" }
                                cycleRequired = [ordered]@{ type = "boolean" }
                                type = [ordered]@{ type = "string" }
                            }
                            required = @("cycleRequired", "type")
                        }
                    )
                    "x-oaiMeta" = [ordered]@{ ignored = $true }
                }
                Cat = [ordered]@{
                    type = "object"
                    title = "Cat schema"
                    example = [ordered]@{ type = "cat" }
                    examples = @([ordered]@{ type = "cat" })
                    properties = [ordered]@{
                        type = [ordered]@{
                            const = "cat"
                            "x-stainless-const" = $true
                        }
                    }
                }
                Dog = [ordered]@{
                    type = "object"
                    properties = [ordered]@{ type = [ordered]@{ const = "dog" } }
                }
                Pet = [ordered]@{
                    oneOf = @(
                        [ordered]@{ '$ref' = "#/components/schemas/Cat" },
                        [ordered]@{ '$ref' = "#/components/schemas/Dog" }
                    )
                    discriminator = [ordered]@{
                        propertyName = "type"
                        mapping = [ordered]@{
                            cat = "#/components/schemas/Cat"
                            dog = "#/components/schemas/Dog"
                        }
                    }
                }
                Alias = [ordered]@{
                    '$ref' = "#/components/schemas/Cat"
                    description = "A reference with a sibling"
                }
                Recursive = [ordered]@{
                    '$recursiveAnchor' = $true
                    type = "object"
                    properties = [ordered]@{
                        children = [ordered]@{
                            type = "array"
                            items = [ordered]@{ '$recursiveRef' = "#" }
                        }
                    }
                }
                ExampleOnly = [ordered]@{ type = "string" }
                ExamplesOnly = [ordered]@{ type = "string" }
                Unused = [ordered]@{ type = "string" }
            }
            responses = [ordered]@{
                Shared = [ordered]@{
                    description = "ok"
                    content = [ordered]@{
                        "application/json" = [ordered]@{
                            schema = [ordered]@{ '$ref' = "#/components/schemas/Pet" }
                        }
                    }
                }
                Unused = [ordered]@{ description = "unused" }
            }
            securitySchemes = [ordered]@{
                ApiKeyAuth = [ordered]@{ type = "http"; scheme = "bearer" }
                AdminApiKeyAuth = [ordered]@{ type = "http"; scheme = "bearer" }
                UnusedAuth = [ordered]@{ type = "http"; scheme = "bearer" }
            }
        }
        "x-oaiMeta" = [ordered]@{ ignored = $true }
        "x-custom-root" = [ordered]@{ ignored = $true }
    }
}

$repositoryRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot ".." ".."))
$temporaryDirectory = Join-Path ([System.IO.Path]::GetTempPath()) "openapi-route-tests-$([Guid]::NewGuid().ToString('N'))"
$null = [System.IO.Directory]::CreateDirectory($temporaryDirectory)

try {
    $fixture = New-TestOpenApiDocument
    $catalog = Get-OpenApiRouteCatalog -Document $fixture
    $routeEntry = Select-OpenApiRoute -Catalog $catalog -RequestedRoute "/things"

    Invoke-Test "catalog includes only paths and sorts normalized feature areas" {
        Assert-Equal 4 $catalog.Count "Catalog count was incorrect."
        Assert-Sequence `
            @("Fine Tuning", "Responses", "Things", "Zebra") `
            @($catalog.FeatureArea) `
            "Feature areas were not normalized and sorted."
    }

    Invoke-Test "query suffixes do not change the feature area" {
        $entry = $catalog | Where-Object Route -eq "/responses?beta=true"
        Assert-Equal "Responses" $entry.FeatureArea "Query-suffixed route was grouped incorrectly."
    }

    Invoke-Test "omission predicate is configured and case-sensitive" {
        foreach ($name in @("example", "examples", "x-foo")) {
            Assert-True (Test-IsOmittedOpenApiProperty -Name $name) "'$name' should be omitted."
        }
        foreach ($name in @("title", "Title", "Example", "Examples", "X-foo")) {
            Assert-True (-not (Test-IsOmittedOpenApiProperty -Name $name)) "'$name' should be retained."
        }
    }

    Invoke-Test "adding an exact name to the omission array needs no code changes" {
        $originalOmittedPropertyNames = @($script:OmittedPropertyNames)
        try {
            $script:OmittedPropertyNames = @($script:OmittedPropertyNames) + "temporaryOmit"
            $normalized = ConvertTo-NormalizedOpenApiValue -Value ([ordered]@{
                keep = "value"
                temporaryOmit = "removed"
            })
            Assert-True (-not (Test-JsonProperty -Object $normalized -Name "temporaryOmit")) "Configured property remained."
            Assert-Equal "value" $normalized.keep "Unconfigured property was removed."
        }
        finally {
            $script:OmittedPropertyNames = $originalOmittedPropertyNames
        }
    }

    Invoke-Test "schema keys use configured priority then alphabetical order" {
        $schema = [ordered]@{
            zeta = $true
            properties = [ordered]@{
                value = [ordered]@{
                    nullable = $true
                    default = "text"
                    description = "Nested value"
                    type = "string"
                }
            }
            items = [ordered]@{
                nullable = $true
                description = "Array item"
                type = "integer"
                default = 1
            }
            required = @("value")
            default = $null
            description = "Root schema"
            type = "object"
            enum = @("first", "second")
        }

        $normalized = ConvertTo-NormalizedOpenApiValue -Value $schema -Context Schema
        Assert-Sequence `
            @("type", "description", "required", "properties", "default", "enum", "items", "zeta") `
            @((Get-JsonProperties -Value $normalized).Name) `
            "Root schema keys were ordered incorrectly."
        Assert-Sequence `
            @("type", "description", "default", "nullable") `
            @((Get-JsonProperties -Value $normalized.properties.value).Name) `
            "Property schema keys were ordered incorrectly."
        Assert-Sequence `
            @("type", "description", "default", "nullable") `
            @((Get-JsonProperties -Value $normalized.items).Name) `
            "Items schema keys were ordered incorrectly."
    }

    Invoke-Test "schema priority array can be reordered without code changes" {
        $originalPriority = @($script:PrioritizedSchemaPropertyNames)
        try {
            $script:PrioritizedSchemaPropertyNames = @("description", "properties", "type")
            $normalized = ConvertTo-NormalizedOpenApiValue `
                -Value ([ordered]@{
                    required = @("value")
                    type = "object"
                    properties = [ordered]@{ value = [ordered]@{ type = "string" } }
                    description = "Configured"
                    default = $null
                }) `
                -Context Schema
            Assert-Sequence `
                @("description", "properties", "type", "default", "required") `
                @((Get-JsonProperties -Value $normalized).Name) `
                "Customized schema priority order was not applied."
        }
        finally {
            $script:PrioritizedSchemaPropertyNames = $originalPriority
        }
    }

    Invoke-Test "required property priorities apply only to required names" {
        $requiredSchema = ConvertTo-NormalizedOpenApiValue `
            -Value ([ordered]@{
                type = "object"
                properties = [ordered]@{
                    text = [ordered]@{ type = "string" }
                    type = [ordered]@{ type = "string" }
                    alpha = [ordered]@{ type = "string" }
                }
                required = @("text", "type")
            }) `
            -Context Schema
        Assert-Sequence `
            @("type", "text", "alpha") `
            @((Get-JsonProperties -Value $requiredSchema.properties).Name) `
            "Required type was not promoted."

        $optionalSchema = ConvertTo-NormalizedOpenApiValue `
            -Value ([ordered]@{
                type = "object"
                properties = [ordered]@{
                    type = [ordered]@{ type = "string" }
                    text = [ordered]@{ type = "string" }
                    alpha = [ordered]@{ type = "string" }
                }
                required = @("text")
            }) `
            -Context Schema
        Assert-Sequence `
            @("text", "alpha", "type") `
            @((Get-JsonProperties -Value $optionalSchema.properties).Name) `
            "Optional type was incorrectly promoted."
    }

    Invoke-Test "required property priority is configurable case-sensitive and duplicate-safe" {
        $originalPriority = @($script:PrioritizedRequiredPropertyNames)
        try {
            $script:PrioritizedRequiredPropertyNames = @("kind", "type", "kind", "missing")
            $normalized = ConvertTo-NormalizedOpenApiValue `
                -Value ([ordered]@{
                    type = "object"
                    properties = [ordered]@{
                        text = [ordered]@{ type = "string" }
                        type = [ordered]@{ type = "string" }
                        alpha = [ordered]@{ type = "string" }
                        kind = [ordered]@{ type = "string" }
                    }
                    required = @("text", "type", "kind")
                }) `
                -Context Schema
            Assert-Sequence `
                @("kind", "type", "text", "alpha") `
                @((Get-JsonProperties -Value $normalized.properties).Name) `
                "Customized required-property priority was not applied."

            $caseSensitiveRequired = [System.Collections.Generic.HashSet[string]]::new(
                [System.StringComparer]::Ordinal
            )
            foreach ($name in @("text", "Type", "type")) {
                $null = $caseSensitiveRequired.Add($name)
            }
            $caseSensitiveOrder = Get-OrderedSchemaPropertyNames `
                -PropertyNames @("text", "Type", "type", "alpha") `
                -RequiredNames $caseSensitiveRequired
            Assert-Sequence `
                @("type", "Type", "text", "alpha") `
                @($caseSensitiveOrder) `
                "Required-property priority matching was not case-sensitive."
        }
        finally {
            $script:PrioritizedRequiredPropertyNames = $originalPriority
        }
    }

    Invoke-Test "allOf merging applies required property priorities" {
        $merged = Merge-AllOfSchemaDefinitions `
            -Definitions @(
                [ordered]@{
                    properties = [ordered]@{
                        text = [ordered]@{ type = "string" }
                        optional = [ordered]@{ type = "string" }
                    }
                    required = @("text")
                },
                [ordered]@{
                    properties = [ordered]@{
                        type = [ordered]@{ type = "string" }
                    }
                    required = @("type")
                }
            ) `
            -DefinitionBreadcrumbs @("#/allOf/0", "#/allOf/1") `
            -Breadcrumb "#"
        Assert-Sequence `
            @("type", "text", "optional") `
            @((Get-JsonProperties -Value $merged.properties).Name) `
            "allOf merge did not apply required-property priority."
    }

    Invoke-Test "schema maps arrays and schema-valued keywords propagate ordering" {
        $schema = [ordered]@{
            oneOf = @(
                [ordered]@{ description = "Branch"; type = "string"; default = "value" }
            )
            additionalProperties = [ordered]@{
                nullable = $true
                description = "Additional value"
                type = "integer"
            }
            '$defs' = [ordered]@{
                Nested = [ordered]@{
                    properties = [ordered]@{ id = [ordered]@{ type = "string" } }
                    required = @("id")
                    description = "Definition"
                    type = "object"
                }
            }
        }

        $normalized = ConvertTo-NormalizedOpenApiValue -Value $schema -Context Schema
        Assert-Sequence `
            @("`$defs", "additionalProperties", "oneOf") `
            @((Get-JsonProperties -Value $normalized).Name) `
            "Composition-only schema keys were not sorted alphabetically."
        Assert-Sequence `
            @("type", "description", "default") `
            @((Get-JsonProperties -Value $normalized.oneOf[0]).Name) `
            "Composition branch schema keys were ordered incorrectly."
        Assert-Sequence `
            @("type", "description", "nullable") `
            @((Get-JsonProperties -Value $normalized.additionalProperties).Name) `
            "Additional-properties schema keys were ordered incorrectly."
        Assert-Sequence `
            @("type", "description", "required", "properties") `
            @((Get-JsonProperties -Value $normalized.'$defs'.Nested).Name) `
            "Definition-map schema keys were ordered incorrectly."
    }

    Invoke-Test "ordinary OpenAPI objects preserve their key order" {
        $ordinary = [ordered]@{
            required = $true
            properties = [ordered]@{ untouched = "value" }
            type = "ordinary"
            description = "Not a schema"
            default = "value"
        }
        $normalized = ConvertTo-NormalizedOpenApiValue -Value $ordinary -Context OpenApi
        Assert-Sequence `
            @("required", "properties", "type", "description", "default") `
            @((Get-JsonProperties -Value $normalized).Name) `
            "Ordinary OpenAPI object order changed."
        Assert-Sequence `
            @("untouched") `
            @((Get-JsonProperties -Value $normalized.properties).Name) `
            "Ordinary properties map was treated as a schema map."
    }

    Invoke-Test "numbered menu retries invalid input and remains one-based" {
        $script:menuAnswers = [System.Collections.Generic.Queue[string]]::new()
        $script:menuAnswers.Enqueue("invalid")
        $script:menuAnswers.Enqueue("0")
        $script:menuAnswers.Enqueue("2")
        $selection = Read-NumberedSelection `
            -Title "Fixture menu" `
            -Options @("first", "second") `
            -GetLabel { param($Option) $Option } `
            -InputProvider { param($Prompt) $script:menuAnswers.Dequeue() }
        Assert-Equal "second" $selection "Menu selected the wrong item."
    }

    Invoke-Test "JSON Pointer handles escaped slash and tilde tokens" {
        $pointerFixture = [ordered]@{ components = [ordered]@{ schemas = [ordered]@{ "A/B~C" = "found" } } }
        $value = Resolve-LocalJsonPointer `
            -Document $pointerFixture `
            -Pointer "#/components/schemas/A~1B~0C"
        Assert-Equal "found" $value "Escaped pointer resolved incorrectly."
    }

    $exact = New-ExtractedOpenApiDocument -Document $fixture -RouteEntry $routeEntry -Mode Exact

    Invoke-Test "Exact mode preserves one complete path item" {
        Assert-Sequence @("/things") @((Get-JsonProperties -Value $exact.Document.paths).Name) "Wrong paths were emitted."
        Assert-Sequence `
            @("parameters", "get", "post") `
            @((Get-JsonProperties -Value $exact.Document.paths["/things"]).Name) `
            "Path-item order changed."
        Assert-True (Test-JsonProperty -Object $exact.Document.paths["/things"] -Name "parameters") "Path-level parameters were dropped."
        Assert-True (Test-JsonProperty -Object $exact.Document.paths["/things"] -Name "get") "GET operation was dropped."
        Assert-True (Test-JsonProperty -Object $exact.Document.paths["/things"] -Name "post") "POST operation was dropped."
    }

    Invoke-Test "Exact mode copies only transitive and security components" {
        Assert-Sequence `
            @("A", "B", "Cat", "Dog", "Pet", "Alias", "Recursive") `
            @((Get-JsonProperties -Value $exact.Document.components.schemas).Name) `
            "Exact schema closure was incorrect."
        Assert-Sequence @("Shared") @((Get-JsonProperties -Value $exact.Document.components.responses).Name) "Exact response closure was incorrect."
        Assert-Sequence `
            @("ApiKeyAuth", "AdminApiKeyAuth") `
            @((Get-JsonProperties -Value $exact.Document.components.securitySchemes).Name) `
            "Security scheme selection was incorrect."
        Assert-Equal 10 $exact.DependencyCount "Exact dependency count was incorrect."
        Assert-OpenApiReferencesResolve -Document $exact.Document
    }

    Invoke-Test "Exact output keeps only used tag definitions" {
        Assert-Sequence @("Things") @($exact.Document.tags.name) "Tag minimization was incorrect."
    }

    $inlined = New-ExtractedOpenApiDocument -Document $fixture -RouteEntry $routeEntry -Mode Inlined

    Invoke-Test "Inlined mode retains only the true cycle back-edge" {
        Assert-Equal 1 $inlined.CycleCount "Cycle count was incorrect."
        $ordinaryReferences = @(Get-OpenApiReferenceOccurrences -Value $inlined.Document | Where-Object Kind -eq "Reference")
        Assert-True ($ordinaryReferences.Count -gt 0) "Expected cycle reference was not retained."
        foreach ($occurrence in $ordinaryReferences) {
            Assert-Equal "#/components/schemas/A" $occurrence.Pointer "A non-cycle reference survived inlining."
        }
        Assert-Sequence @("A") @((Get-JsonProperties -Value $inlined.Document.components.schemas).Name) "Fallback component selection was incorrect."
        Assert-OpenApiReferencesResolve -Document $inlined.Document
    }

    Invoke-Test "Inlined mode rewrites discriminator mappings to local branches" {
        $mapping = $inlined.Document.paths["/things"].post.responses["200"].content["application/json"].schema.discriminator.mapping
        Assert-Equal `
            "#/paths/~1things/post/responses/200/content/application~1json/schema/oneOf/0" `
            $mapping.cat `
            "Cat mapping was not rewritten."
        Assert-Equal `
            "#/paths/~1things/post/responses/200/content/application~1json/schema/oneOf/1" `
            $mapping.dog `
            "Dog mapping was not rewritten."
    }

    Invoke-Test "Inlined schema reference siblings are flattened" {
        $alias = $inlined.Document.paths["/things"].post.requestBody.content["application/json"].schema.properties.requiredA
        Assert-True (-not (Test-JsonProperty -Object $alias -Name "allOf")) "Reference sibling introduced allOf."
        Assert-Equal "object" $alias.type "Referenced schema type was lost."
        Assert-Equal "A reference with a sibling" $alias.description "Reference sibling description was lost."
        Assert-Equal "cat" $alias.properties.type.const "Referenced schema properties were lost."
        Assert-Sequence `
            @("type", "description", "properties", "title") `
            @((Get-JsonProperties -Value $alias).Name) `
            "Inlined reference schema keys were ordered incorrectly."
    }

    Invoke-Test "Inlined mode recursively flattens allOf and combines properties" {
        $flattened = $inlined.Document.paths["/things"].post.requestBody.content["application/json"].schema.properties.flattened
        Assert-Equal 0 @(Find-JsonPropertyValues -Value $inlined.Document -Name "allOf").Count "Inlined output retained allOf."
        Assert-Equal "object" $flattened.type "Flattened type was lost."
        Assert-Equal $true $flattened.nullable "Flattened nullable constraint was lost."
        Assert-Equal "Outer description" $flattened.description "Containing description did not take precedence."
        Assert-Sequence `
            @("requiredA", "requiredB", "requiredC") `
            @($flattened.required) `
            "Combined required names were not deduplicated and sorted."
        Assert-Sequence `
            @("requiredA", "requiredB", "requiredC", "optionalA", "optionalM", "optionalZ", "shared") `
            @((Get-JsonProperties -Value $flattened.properties).Name) `
            "Flattened properties were not ordered required-first and alphabetically."
        Assert-Equal 1 @((Get-JsonProperties -Value $flattened.properties) | Where-Object Name -eq "shared").Count "Identical properties were not deduplicated."
        Assert-Sequence `
            @("type", "description", "required", "properties", "nullable") `
            @((Get-JsonProperties -Value $flattened).Name) `
            "Flattened schema keys were ordered incorrectly."
    }

    Invoke-Test "Inlined cycle fallback components are flattened" {
        $fallbackChild = $inlined.Document.components.schemas.A.properties.child
        Assert-True (-not (Test-JsonProperty -Object $fallbackChild -Name "allOf")) "Cycle fallback retained allOf."
        Assert-Sequence `
            @("type", "cycleRequired", "cycleOptional", "parent") `
            @((Get-JsonProperties -Value $fallbackChild.properties).Name) `
            "Cycle fallback properties were not merged and ordered."
        Assert-Sequence `
            @("type", "required", "properties") `
            @((Get-JsonProperties -Value $fallbackChild).Name) `
            "Cycle fallback schema keys were ordered incorrectly."
    }

    Invoke-Test "Exact mode preserves source allOf" {
        Assert-True (@(Find-JsonPropertyValues -Value $exact.Document -Name "allOf").Count -gt 0) "Exact mode unexpectedly flattened allOf."
    }

    Invoke-Test "allOf flattening rejects incompatible property definitions" {
        $activeReferences = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
        $cycleReferences = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
        $conflictingSchema = [ordered]@{
            allOf = @(
                [ordered]@{ properties = [ordered]@{ duplicate = [ordered]@{ type = "string" } } },
                [ordered]@{ properties = [ordered]@{ duplicate = [ordered]@{ type = "integer" } } }
            )
        }
        Assert-Throws `
            { Expand-OpenApiReferences -Value $conflictingSchema -Document $fixture -ActiveReferences $activeReferences -CycleReferences $cycleReferences -Breadcrumb "#/conflict" } `
            "*duplicate*"
    }

    Invoke-Test "allOf flattening rejects incompatible validating keywords" {
        $activeReferences = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
        $cycleReferences = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
        $conflictingSchema = [ordered]@{
            allOf = @(
                [ordered]@{ type = "object" },
                [ordered]@{ type = "array" }
            )
        }
        Assert-Throws `
            { Expand-OpenApiReferences -Value $conflictingSchema -Document $fixture -ActiveReferences $activeReferences -CycleReferences $cycleReferences -Breadcrumb "#/conflict" } `
            "*type*"
    }

    Invoke-Test "allOf flattening intersects compatible enum constraints" {
        $activeReferences = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
        $cycleReferences = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
        $refinedSchema = [ordered]@{
            allOf = @(
                [ordered]@{ enum = @("in_progress", "completed", "incomplete") },
                [ordered]@{ enum = @("completed", "incomplete", "failed") }
            )
        }
        $expanded = Expand-OpenApiReferences `
            -Value $refinedSchema `
            -Document $fixture `
            -ActiveReferences $activeReferences `
            -CycleReferences $cycleReferences `
            -Breadcrumb "#/enum-refinement"
        Assert-Sequence `
            @("completed", "incomplete") `
            @($expanded.enum) `
            "Compatible enum constraints were not intersected."
    }

    Invoke-Test "allOf flattening rejects disjoint enum constraints" {
        $activeReferences = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
        $cycleReferences = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
        $conflictingSchema = [ordered]@{
            allOf = @(
                [ordered]@{ enum = @("completed") },
                [ordered]@{ enum = @("failed") }
            )
        }
        Assert-Throws `
            { Expand-OpenApiReferences -Value $conflictingSchema -Document $fixture -ActiveReferences $activeReferences -CycleReferences $cycleReferences -Breadcrumb "#/enum-conflict" } `
            "*no common values*"
    }

    Invoke-Test "allOf flattening rejects empty and non-object branches" {
        $activeReferences = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
        $cycleReferences = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
        Assert-Throws `
            { Expand-OpenApiReferences -Value ([ordered]@{ allOf = @() }) -Document $fixture -ActiveReferences $activeReferences -CycleReferences $cycleReferences -Breadcrumb "#/empty" } `
            "*allOf*"
        Assert-Throws `
            { Expand-OpenApiReferences -Value ([ordered]@{ allOf = @("invalid") }) -Document $fixture -ActiveReferences $activeReferences -CycleReferences $cycleReferences -Breadcrumb "#/invalid" } `
            "*allOf*"
    }

    Invoke-Test "explicit recursive references remain finite" {
        $recursiveReferences = @(Find-JsonPropertyValues -Value $inlined.Document -Name '$recursiveRef')
        Assert-Sequence @("#") $recursiveReferences "Recursive references were changed or removed."
    }

    Invoke-Test "configured and x-prefixed properties are removed without mutating the source" {
        Assert-Equal 0 @(Find-OmittedOpenApiPropertyNames -Value $exact.Document).Count "Exact omitted properties remained."
        Assert-Equal 0 @(Find-OmittedOpenApiPropertyNames -Value $inlined.Document).Count "Inlined omitted properties remained."
        Assert-Equal "Extractor fixture" $exact.Document.info.title "Exact info title was removed."
        Assert-Equal "Extractor fixture" $inlined.Document.info.title "Inlined info title was removed."
        Assert-True (Test-JsonProperty -Object $fixture -Name "x-custom-root") "Source root extension was mutated."
        Assert-True (Test-JsonProperty -Object $fixture.info -Name "x-custom-info") "Source info extension was mutated."
        Assert-True (Test-JsonProperty -Object $fixture.paths["/things"].post -Name "x-hidden-ref") "Source operation extension was mutated."
        Assert-True (Test-JsonProperty -Object $fixture.paths["/things"].post -Name "example") "Source example was mutated."
        Assert-True (Test-JsonProperty -Object $fixture.paths["/things"].post -Name "examples") "Source examples were mutated."
        Assert-True (Test-JsonProperty -Object $fixture.components.schemas.Cat.properties.type -Name "x-stainless-const") "Nested source extension was mutated."
        Assert-Equal "Cat schema" $inlined.Document.paths["/things"].post.responses["200"].content["application/json"].schema.oneOf[0].title "Schema title was removed."
        Assert-True (-not (Test-JsonProperty -Object $exact.Document.components.schemas -Name "Unused")) "Exact mode followed a reference inside an extension."
        Assert-True (-not (Test-JsonProperty -Object $exact.Document.components.schemas -Name "ExampleOnly")) "Exact mode followed a reference inside example."
        Assert-True (-not (Test-JsonProperty -Object $exact.Document.components.schemas -Name "ExamplesOnly")) "Exact mode followed a reference inside examples."
    }

    Invoke-Test "schema properties are prioritized-required then alphabetical" {
        $properties = $inlined.Document.paths["/things"].post.requestBody.content["application/json"].schema.properties
        Assert-Sequence `
            @("type", "requiredA", "requiredB", "flattened", "optionalZ", "recursive") `
            @((Get-JsonProperties -Value $properties).Name) `
            "Schema properties were ordered incorrectly."
        Assert-Sequence `
            @("requiredB", "requiredA", "type") `
            @($inlined.Document.paths["/things"].post.requestBody.content["application/json"].schema.required) `
            "Required array order changed."
        Assert-Sequence `
            @("optionalZ", "requiredB", "recursive", "requiredA", "flattened", "type") `
            @((Get-JsonProperties -Value $fixture.paths["/things"].post.requestBody.content["application/json"].schema.properties).Name) `
            "Source property order was mutated."
        Assert-Sequence `
            @("type", "requiredA", "requiredB", "flattened", "optionalZ", "recursive") `
            @((Get-JsonProperties -Value $exact.Document.paths["/things"].post.requestBody.content["application/json"].schema.properties).Name) `
            "Exact schema properties were ordered incorrectly."
        Assert-Sequence `
            @("type", "required", "properties") `
            @((Get-JsonProperties -Value $inlined.Document.paths["/things"].post.requestBody.content["application/json"].schema).Name) `
            "Inlined request schema keys were ordered incorrectly."
        Assert-Sequence `
            @("type", "properties", "title") `
            @((Get-JsonProperties -Value $exact.Document.components.schemas.Cat).Name) `
            "Exact component schema keys were ordered incorrectly."
    }

    Invoke-Test "external and unresolved references fail with context" {
        Assert-Throws `
            { Get-TransitiveComponentReferences -Document $fixture -Value ([ordered]@{ '$ref' = "https://example.test/schema.json" }) } `
            "*external*"
        Assert-Throws `
            { Get-TransitiveComponentReferences -Document $fixture -Value ([ordered]@{ '$ref' = "#/components/schemas/Missing" }) } `
            "*has no 'Missing' property*"
    }

    Invoke-Test "default filenames are portable and collision-aware" {
        $collisionCatalog = @(
            [PSCustomObject]@{ Route = "/foo?bar" },
            [PSCustomObject]@{ Route = "/foo/bar" }
        )
        $path = Get-DefaultOutputPath `
            -Route "/foo?bar" `
            -Mode Exact `
            -Catalog $collisionCatalog `
            -RepositoryRoot $temporaryDirectory
        $name = Split-Path $path -Leaf
        Assert-True ($name -match '^foo-bar-[0-9a-f]{8}\.exact\.openapi\.json$') "Collision-safe filename was unexpected: $name"
    }

    Invoke-Test "JSON output is deterministic UTF-8 without a BOM" {
        $firstPath = Join-Path $temporaryDirectory "first.json"
        $secondPath = Join-Path $temporaryDirectory "second.json"
        $null = Write-OpenApiJsonFile -Document $inlined.Document -Path $firstPath
        $null = Write-OpenApiJsonFile -Document $inlined.Document -Path $secondPath
        [byte[]]$firstBytes = [System.IO.File]::ReadAllBytes($firstPath)
        [byte[]]$secondBytes = [System.IO.File]::ReadAllBytes($secondPath)
        Assert-True ([System.Linq.Enumerable]::SequenceEqual($firstBytes, $secondBytes)) "Repeated serialization differed."
        $hasBom = $firstBytes.Length -ge 3 -and $firstBytes[0] -eq 0xEF -and $firstBytes[1] -eq 0xBB -and $firstBytes[2] -eq 0xBF
        Assert-True (-not $hasBom) "Output contained a UTF-8 BOM."
        $null = Get-Content -LiteralPath $firstPath -Raw -Encoding utf8 | ConvertFrom-Json -Depth 100
    }

    $livePath = Join-Path $repositoryRoot "specification" "openapi" "openapi.json"
    $liveDocument = Read-OpenApiDocument -Path $livePath
    $liveCatalog = Get-OpenApiRouteCatalog -Document $liveDocument

    Invoke-Test "live catalog has one entry per paths property" {
        $livePathCount = @(Get-JsonProperties -Value $liveDocument.paths).Count
        Assert-Equal $livePathCount $liveCatalog.Count "Live catalog omitted or added routes."
    }

    Invoke-Test "live audio route extracts in Exact and Inlined modes" {
        $audioRoute = Select-OpenApiRoute -Catalog $liveCatalog -RequestedRoute "/audio/speech"
        $audioExact = New-ExtractedOpenApiDocument -Document $liveDocument -RouteEntry $audioRoute -Mode Exact
        $audioInlined = New-ExtractedOpenApiDocument -Document $liveDocument -RouteEntry $audioRoute -Mode Inlined
        Assert-OpenApiReferencesResolve -Document $audioExact.Document
        Assert-OpenApiReferencesResolve -Document $audioInlined.Document
        Assert-Equal 1 @(Get-JsonProperties -Value $audioExact.Document.paths).Count "Exact audio output had the wrong path count."
        Assert-Equal 0 @(Get-OpenApiReferenceOccurrences -Value $audioInlined.Document | Where-Object Kind -eq "Reference").Count "A non-cyclic audio reference survived inlining."
        Assert-Equal 0 @(Find-OmittedOpenApiPropertyNames -Value $audioExact.Document).Count "Exact audio output retained omitted properties."
        Assert-Equal 0 @(Find-OmittedOpenApiPropertyNames -Value $audioInlined.Document).Count "Inlined audio output retained omitted properties."
        Assert-Equal "OpenAI API" $audioExact.Document.info.title "Exact live title was removed."
        Assert-Equal "OpenAI API" $audioInlined.Document.info.title "Inlined live title was removed."
        Assert-OpenApiSchemaKeyOrder -Value $audioExact.Document
        Assert-OpenApiSchemaKeyOrder -Value $audioInlined.Document
    }

    Invoke-Test "live top-level and nested allOf schemas are flattened" {
        foreach ($route in @(
            "/organization/admin_api_keys",
            "/chat/completions/{completion_id}/messages"
        )) {
            $routeEntry = Select-OpenApiRoute -Catalog $liveCatalog -RequestedRoute $route
            $routeInlined = New-ExtractedOpenApiDocument `
                -Document $liveDocument `
                -RouteEntry $routeEntry `
                -Mode Inlined
            Assert-Equal `
                0 `
                @(Find-JsonPropertyValues -Value $routeInlined.Document -Name "allOf").Count `
                "Route '$route' retained allOf."
            Assert-OpenApiReferencesResolve -Document $routeInlined.Document
        }
    }

    Invoke-Test "live recursive filter remains finite when inlined" {
        $searchRoute = Select-OpenApiRoute `
            -Catalog $liveCatalog `
            -RequestedRoute "/vector_stores/{vector_store_id}/search"
        $searchInlined = New-ExtractedOpenApiDocument `
            -Document $liveDocument `
            -RouteEntry $searchRoute `
            -Mode Inlined
        $recursiveReferences = @(Find-JsonPropertyValues -Value $searchInlined.Document -Name '$recursiveRef')
        Assert-True ($recursiveReferences.Count -gt 0) "Live recursive filter lost its recursive reference."
        foreach ($reference in $recursiveReferences) {
            Assert-Equal "#" $reference "Live recursive pointer changed."
        }
        Assert-Equal 0 @(Get-OpenApiReferenceOccurrences -Value $searchInlined.Document | Where-Object Kind -eq "Reference").Count "Live recursive filter retained an ordinary reference."
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