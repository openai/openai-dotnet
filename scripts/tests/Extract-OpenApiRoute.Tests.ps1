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
                get = [ordered]@{ operationId = "listZebraItems"; responses = [ordered]@{ "204" = [ordered]@{ description = "none" } } }
            }
            "/responses?beta=true" = [ordered]@{
                post = [ordered]@{ operationId = "createBetaResponse"; responses = [ordered]@{ "204" = [ordered]@{ description = "none" } } }
            }
            "/things" = [ordered]@{
                parameters = @(
                    [ordered]@{ name = "trace"; in = "header"; schema = [ordered]@{ type = "string" } }
                )
                get = [ordered]@{
                    operationId = "listThings"
                    tags = @("Things")
                    responses = [ordered]@{ "204" = [ordered]@{ description = "none" } }
                }
                post = [ordered]@{
                    operationId = "createThing"
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
                get = [ordered]@{ operationId = "listFineTuningJobs"; responses = [ordered]@{ "204" = [ordered]@{ description = "none" } } }
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
                            enum = @("zeta", "alpha", "middle")
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

$temporaryDirectory = Join-Path ([System.IO.Path]::GetTempPath()) "openapi-route-tests-$([Guid]::NewGuid().ToString('N'))"
$null = [System.IO.Directory]::CreateDirectory($temporaryDirectory)

try {
    $fixture = New-TestOpenApiDocument
    $catalog = Get-OpenApiOperationCatalog -Document $fixture
    $operationEntry = Select-OpenApiOperation `
        -Catalog $catalog `
        -RequestedOperationId "createThing"

    Invoke-Test "public script exposes operationId without route or mode parameters" {
        $command = Get-Command $scriptPath
        Assert-True ($command.Parameters.ContainsKey("OperationId")) "OperationId parameter was missing."
        Assert-True (-not $command.Parameters.ContainsKey("Route")) "Legacy Route parameter remained."
        Assert-True (-not $command.Parameters.ContainsKey("Mode")) "Legacy Mode parameter remained."
    }

    Invoke-Test "catalog includes path operations and sorts operationIds" {
        Assert-Equal 5 $catalog.Count "Catalog count was incorrect."
        Assert-Sequence `
            @("createBetaResponse", "createThing", "listFineTuningJobs", "listThings", "listZebraItems") `
            @($catalog.OperationId) `
            "OperationIds were not sorted."
    }

    Invoke-Test "multiple operations on one route are independently selectable" {
        $sameRouteOperations = @($catalog | Where-Object Route -eq "/things")
        Assert-Sequence @("createThing", "listThings") @($sameRouteOperations.OperationId) "Same-route operations were not cataloged independently."
        Assert-Equal "post" $operationEntry.Method "OperationId selected the wrong HTTP method."
        Assert-Equal "/things" $operationEntry.Route "OperationId selected the wrong route."
    }

    Invoke-Test "duplicate operationIds fail clearly" {
        $duplicateFixture = Copy-JsonValue -Value $fixture
        $duplicateFixture.paths["/things"].get.operationId = "createThing"
        Assert-Throws `
            { Get-OpenApiOperationCatalog -Document $duplicateFixture } `
            "*operationId 'createThing' is not unique*"
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
            $pruned = ConvertTo-PrunedOpenApiValue -Value ([ordered]@{
                keep = "value"
                temporaryOmit = "removed"
            })
            Assert-True (-not (Test-JsonProperty -Object $pruned -Name "temporaryOmit")) "Configured property remained."
            Assert-Equal "value" $pruned.keep "Unconfigured property was removed."
        }
        finally {
            $script:OmittedPropertyNames = $originalOmittedPropertyNames
        }
    }

    Invoke-Test "schema definition merging preserves first-seen order" {
        $merged = Merge-OpenApiSchemaDefinitions `
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
            @("text", "optional", "type") `
            @((Get-JsonProperties -Value $merged.properties).Name) `
            "Schema merge changed property encounter order."
        Assert-Sequence `
            @("text", "type") `
            @($merged.required) `
            "Schema merge changed required-name encounter order."
    }

    Invoke-Test "pruning preserves surviving object and array order" {
        $ordinary = [ordered]@{
            required = $true
            properties = [ordered]@{ untouched = "value" }
            type = "ordinary"
            example = "removed"
            description = "Not a schema"
            default = "value"
        }
        $pruned = ConvertTo-PrunedOpenApiValue -Value $ordinary
        Assert-Sequence `
            @("required", "properties", "type", "description", "default") `
            @((Get-JsonProperties -Value $pruned).Name) `
            "Pruning changed surviving object order."
        Assert-Sequence `
            @("untouched") `
            @((Get-JsonProperties -Value $pruned.properties).Name) `
            "Pruning changed a nested object."
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

    $inlined = New-ExtractedOpenApiDocument `
        -Document $fixture `
        -OperationEntry $operationEntry

    Invoke-Test "extraction retains path metadata and only the selected operation" {
        Assert-Sequence @("/things") @((Get-JsonProperties -Value $inlined.Document.paths).Name) "Wrong paths were emitted."
        Assert-Sequence `
            @("parameters", "post") `
            @((Get-JsonProperties -Value $inlined.Document.paths["/things"]).Name) `
            "Path-item order changed."
        Assert-True (Test-JsonProperty -Object $inlined.Document.paths["/things"] -Name "parameters") "Path-level parameters were dropped."
        Assert-True (-not (Test-JsonProperty -Object $inlined.Document.paths["/things"] -Name "get")) "Unselected GET operation remained."
        Assert-Equal "createThing" $inlined.Document.paths["/things"].post.operationId "Selected operationId changed."
    }

    Invoke-Test "extraction keeps only cycle fallback and security components" {
        Assert-Sequence @("A") @((Get-JsonProperties -Value $inlined.Document.components.schemas).Name) "Fallback component selection was incorrect."
        Assert-Sequence `
            @("ApiKeyAuth", "AdminApiKeyAuth") `
            @((Get-JsonProperties -Value $inlined.Document.components.securitySchemes).Name) `
            "Security scheme selection was incorrect."
        Assert-Equal 3 $inlined.DependencyCount "Retained component count was incorrect."
        Assert-OpenApiReferencesResolve -Document $inlined.Document
    }

    Invoke-Test "extraction keeps only selected operation tag definitions" {
        Assert-Sequence @("Things") @($inlined.Document.tags.name) "Tag minimization was incorrect."
    }

    Invoke-Test "extraction retains only the true cycle back-edge" {
        Assert-Equal 1 $inlined.CycleCount "Cycle count was incorrect."
        $ordinaryReferences = @(Get-OpenApiReferenceOccurrences -Value $inlined.Document | Where-Object Kind -eq "Reference")
        Assert-True ($ordinaryReferences.Count -gt 0) "Expected cycle reference was not retained."
        foreach ($occurrence in $ordinaryReferences) {
            Assert-Equal "#/components/schemas/A" $occurrence.Pointer "A non-cycle reference survived inlining."
        }
        Assert-Sequence @("A") @((Get-JsonProperties -Value $inlined.Document.components.schemas).Name) "Fallback component selection was incorrect."
        Assert-OpenApiReferencesResolve -Document $inlined.Document
    }

    Invoke-Test "extraction rewrites discriminator mappings to local branches" {
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
    }

    Invoke-Test "extraction preserves source allOf while expanding references" {
        $preserved = $inlined.Document.paths["/things"].post.requestBody.content["application/json"].schema.properties.flattened
        Assert-True (Test-JsonProperty -Object $preserved -Name "allOf") "Inlined output removed source allOf."
        Assert-True (@(Find-JsonPropertyValues -Value $preserved -Name "allOf").Count -gt 1) "Nested source allOf was removed."
        Assert-Equal 0 @(Get-OpenApiReferenceOccurrences -Value $preserved | Where-Object Kind -eq "Reference").Count "Inlined allOf retained a non-cycle component reference."
    }

    Invoke-Test "Inlined cycle fallback components preserve source allOf" {
        $fallbackChild = $inlined.Document.components.schemas.A.properties.child
        Assert-True (Test-JsonProperty -Object $fallbackChild -Name "allOf") "Cycle fallback removed source allOf."
        Assert-True (@(Get-OpenApiReferenceOccurrences -Value $fallbackChild | Where-Object Kind -eq "Reference").Count -gt 0) "Cycle fallback lost its cycle reference."
    }

    Invoke-Test "explicit recursive references remain finite" {
        $recursiveReferences = @(Find-JsonPropertyValues -Value $inlined.Document -Name '$recursiveRef')
        Assert-Sequence @("#") $recursiveReferences "Recursive references were changed or removed."
    }

    Invoke-Test "configured and x-prefixed properties are removed without mutating the source" {
        Assert-Equal 0 @(Find-OmittedOpenApiPropertyNames -Value $inlined.Document).Count "Inlined omitted properties remained."
        Assert-Equal "Extractor fixture" $inlined.Document.info.title "Inlined info title was removed."
        Assert-True (Test-JsonProperty -Object $fixture -Name "x-custom-root") "Source root extension was mutated."
        Assert-True (Test-JsonProperty -Object $fixture.info -Name "x-custom-info") "Source info extension was mutated."
        Assert-True (Test-JsonProperty -Object $fixture.paths["/things"].post -Name "x-hidden-ref") "Source operation extension was mutated."
        Assert-True (Test-JsonProperty -Object $fixture.paths["/things"].post -Name "example") "Source example was mutated."
        Assert-True (Test-JsonProperty -Object $fixture.paths["/things"].post -Name "examples") "Source examples were mutated."
        Assert-True (Test-JsonProperty -Object $fixture.components.schemas.Cat.properties.type -Name "x-stainless-const") "Nested source extension was mutated."
        Assert-Equal "Cat schema" $inlined.Document.paths["/things"].post.responses["200"].content["application/json"].schema.oneOf[0].title "Schema title was removed."
        Assert-True (-not (Test-JsonProperty -Object $inlined.Document.components.schemas -Name "Unused")) "Extraction followed a reference inside an extension."
        Assert-True (-not (Test-JsonProperty -Object $inlined.Document.components.schemas -Name "ExampleOnly")) "Extraction followed a reference inside example."
        Assert-True (-not (Test-JsonProperty -Object $inlined.Document.components.schemas -Name "ExamplesOnly")) "Extraction followed a reference inside examples."
    }

    Invoke-Test "Extract preserves schema encounter order after pruning" {
        $properties = $inlined.Document.paths["/things"].post.requestBody.content["application/json"].schema.properties
        Assert-Sequence `
            @("optionalZ", "requiredB", "recursive", "requiredA", "flattened", "type") `
            @((Get-JsonProperties -Value $properties).Name) `
            "Inlined schema property encounter order changed."
        Assert-Sequence `
            @("requiredB", "requiredA", "type") `
            @($inlined.Document.paths["/things"].post.requestBody.content["application/json"].schema.required) `
            "Inlined required encounter order changed."
        Assert-Sequence `
            @("requiredB", "requiredA", "type") `
            @($fixture.paths["/things"].post.requestBody.content["application/json"].schema.required) `
            "Source required array was mutated."
        Assert-Sequence `
            @("optionalZ", "requiredB", "recursive", "requiredA", "flattened", "type") `
            @((Get-JsonProperties -Value $fixture.paths["/things"].post.requestBody.content["application/json"].schema.properties).Name) `
            "Source property order was mutated."
        Assert-Sequence `
            @("type", "properties", "required") `
            @((Get-JsonProperties -Value $inlined.Document.paths["/things"].post.requestBody.content["application/json"].schema).Name) `
            "Inlined schema-key encounter order changed."
    }

    Invoke-Test "external and unresolved references fail with context" {
        foreach ($reference in @("https://example.test/schema.json", "#/components/schemas/Missing")) {
            $invalidFixture = Copy-JsonValue -Value $fixture
            $invalidFixture.paths["/things"].post.requestBody.content["application/json"].schema = [ordered]@{ '$ref' = $reference }
            $invalidCatalog = Get-OpenApiOperationCatalog -Document $invalidFixture
            $invalidOperation = Select-OpenApiOperation `
                -Catalog $invalidCatalog `
                -RequestedOperationId "createThing"
            Assert-Throws `
                { New-ExtractedOpenApiDocument -Document $invalidFixture -OperationEntry $invalidOperation } `
                $(if ($reference.StartsWith("https", [System.StringComparison]::Ordinal)) { "*external*" } else { "*has no 'Missing' property*" })
        }
    }

    Invoke-Test "default filenames are portable and collision-aware" {
        $ordinaryPath = Get-DefaultOutputPath `
            -OperationId "createThing" `
            -Catalog $catalog `
            -RepositoryRoot $temporaryDirectory
        Assert-Equal "createThing.inlined.openapi.json" (Split-Path $ordinaryPath -Leaf) "Ordinary operationId filename was incorrect."

        $collisionCatalog = @(
            [PSCustomObject]@{ OperationId = "foo?bar" },
            [PSCustomObject]@{ OperationId = "foo/bar" }
        )
        $path = Get-DefaultOutputPath `
            -OperationId "foo?bar" `
            -Catalog $collisionCatalog `
            -RepositoryRoot $temporaryDirectory
        $name = Split-Path $path -Leaf
        Assert-True ($name -match '^foo-bar-[0-9a-f]{8}\.inlined\.openapi\.json$') "Collision-safe filename was unexpected: $name"
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