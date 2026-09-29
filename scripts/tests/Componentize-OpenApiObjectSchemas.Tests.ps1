<#
.SYNOPSIS
    Runs focused tests for scripts/Componentize-OpenApiObjectSchemas.ps1.

.DESCRIPTION
    Exercises candidate discovery, semantic naming, pointer relocation,
    component materialization, input validation, and structural/byte idempotence.
    The suite is dependency-free so it can run directly under PowerShell 7:

        pwsh ./scripts/tests/Componentize-OpenApiObjectSchemas.Tests.ps1
#>

[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$scriptDirectory = Join-Path $PSScriptRoot ".."
$componentizeScriptPath = Join-Path $scriptDirectory "Componentize-OpenApiObjectSchemas.ps1"

# Dot-source the command under test so its pure conversion functions can be
# exercised without file I/O. The script's invocation guard must prevent its CLI
# entry point from running during this load; the first test locks that contract.
. $componentizeScriptPath
. (Join-Path $PSScriptRoot "_OpenApiTestFixture.Helpers.ps1")

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

function Assert-Equal {
    param([AllowNull()][object]$Expected, [AllowNull()][object]$Actual, [string]$Message)
    if ($Expected -ne $Actual) { throw "$Message Expected '$Expected', got '$Actual'." }
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

function Assert-JsonEqual {
    param([object]$Expected, [object]$Actual, [string]$Message)
    if (-not (Test-JsonValuesEqual -Left $Expected -Right $Actual)) {
        throw $Message
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

# The compact fixture intentionally combines one root request object, nested
# properties, additionalProperties, repeated oneOf branches, an existing schema
# root, source-authored refs/mappings, and literal lookalikes. This gives the
# golden test broad coverage while the smaller mutation tests isolate decisions.
$fixtureDirectory = Join-Path $PSScriptRoot "fixtures" "Componentize-OpenApiObjectSchemas"
$sourceFixturePath = Join-Path $fixtureDirectory "source.openapi.json"
$expectedFixturePath = Join-Path $fixtureDirectory "expected.componentized.openapi.json"
$temporaryDirectory = Join-Path ([System.IO.Path]::GetTempPath()) "componentize-openapi-$([Guid]::NewGuid().ToString('N'))"
$null = [System.IO.Directory]::CreateDirectory($temporaryDirectory)

try {
    # Tests consume the script as a function library; loading it must not write
    # the canonical production artifact.
    Invoke-Test "dot-sourcing exposes componentizer functions without running the CLI" {
        Assert-True ($null -ne (Get-Command ConvertTo-ComponentizedOpenApiDocument -ErrorAction SilentlyContinue)) "Conversion function was not defined."
        Assert-True ($null -ne (Get-Command Invoke-OpenApiObjectSchemaComponentizer -ErrorAction SilentlyContinue)) "CLI function was not defined."
        Assert-Equal `
            ([System.IO.Path]::GetFullPath("./specification/openapi/responses.inlined.flattened.simplified.recursive-simplified.type-enum-split.oneof-discriminated.oneof-normalized.oneof-flattened.sorted.openapi.json")) `
            $script:DefaultComponentizerInputPath `
            "Canonical input path was incorrect."
        Assert-Equal `
            ([System.IO.Path]::GetFullPath("./specification/openapi/responses.componentized.openapi.json")) `
            $script:DefaultComponentizerOutputPath `
            "Canonical output path was incorrect."
    }

    Invoke-Test "union marker concatenates with the final semantic part" {
        $empty = [System.Collections.Generic.List[string]]::new()
        Add-OpenApiUnionNameSuffix -Parts $empty
        Assert-Sequence @("Union") @($empty) "Empty union name did not use its fallback."

        $ordinary = [System.Collections.Generic.List[string]]::new([string[]]@("Conversation"))
        Add-OpenApiUnionNameSuffix -Parts $ordinary
        Assert-Sequence @("ConversationUnion") @($ordinary) "Union marker was not concatenated."

        $alreadyMarked = [System.Collections.Generic.List[string]]::new([string[]]@("InputUnion"))
        Add-OpenApiUnionNameSuffix -Parts $alreadyMarked
        Assert-Sequence @("InputUnion") @($alreadyMarked) "Union marker was appended twice."
    }

    # Discovery is schema-aware and parent-first: component roots and literal
    # lookalikes are excluded, while a direct oneOf owner precedes its branches.
    Invoke-Test "candidate discovery finds each inline schema occurrence exactly once" {
        $source = Read-OpenApiDocument -Path $sourceFixturePath
        $candidates = @(Get-InlineOpenApiSchemaCandidates -Document $source)
        Assert-Equal 8 $candidates.Count "Candidate count was incorrect."
        Assert-Sequence `
            @("Object", "Object", "Object", "Object", "OneOf", "Object", "Object", "Object") `
            @($candidates.Kind) `
            "Candidate kinds or encounter order were incorrect."
        Assert-Sequence `
            @(0, 1, 2, 1, 1, 2, 2, 0) `
            @($candidates.CandidateDepth) `
            "Candidate nesting depths were incorrect."
        Assert-Equal 1 @($candidates | Where-Object IsNonDiscriminatedUnion).Count "Non-discriminated union count was incorrect."
        Assert-True (@($candidates | Where-Object Kind -eq "OneOf")[0].IsNonDiscriminatedUnion) "oneOf candidate was not classified as a non-discriminated union."
        Assert-Equal 3 @($candidates | Where-Object { $null -ne $_.TypeEnumValue }).Count "Singleton enum count was incorrect."
        Assert-True (-not ($candidates.Pointer -ccontains "#/components/schemas/ExistingRoot")) "Existing component root was rediscovered."
    }

    # The golden locks one-component-per-occurrence semantics, deterministic
    # names, nested reference chains, relocation, and existing component order.
    Invoke-Test "compact fixture matches its componentized golden" {
        $source = Read-OpenApiDocument -Path $sourceFixturePath
        $expected = Read-OpenApiDocument -Path $expectedFixturePath
        $sourceBefore = ConvertTo-Json -InputObject $source -Compress -Depth 100
        $result = ConvertTo-ComponentizedOpenApiDocument -Document $source
        Assert-Equal 7 $result.DiscoveredObjectSchemaCount "Discovery statistic was incorrect."
        Assert-Equal 1 $result.DiscoveredOneOfSchemaCount "oneOf discovery statistic was incorrect."
        Assert-Equal 8 $result.DiscoveredSchemaCount "Total discovery statistic was incorrect."
        Assert-Equal 8 $result.GeneratedComponentCount "Generated statistic was incorrect."
        Assert-Equal 8 $result.RelocationCount "Relocation statistic was incorrect."
        Assert-JsonEqual $expected $result.Document "Componentized fixture did not match its golden."
        Assert-Equal $sourceBefore (ConvertTo-Json -InputObject $source -Compress -Depth 100) "Conversion mutated its input."
    }

    # Names combine underscore-separated operation/property context and direct
    # singleton type enums; only colliding bases receive numeric suffixes.
    Invoke-Test "component names are semantic union-aware and suffix only collisions" {
        $source = Read-OpenApiDocument -Path $sourceFixturePath
        $result = ConvertTo-ComponentizedOpenApiDocument -Document $source
        $schemas = $result.Document.components.schemas
        Assert-Sequence @(
            "ExistingRoot",
            "CreateWidget_Request",
            "CreateWidget_Request_Child_TypeOfChildKind",
            "CreateWidget_Request_Child_TypeOfChildKind_1",
            "CreateWidget_Request_List",
            "CreateWidget_Request_VariantsUnion",
            "CreateWidget_Request_VariantsUnion_TypeOfAlpha",
            "CreateWidget_Request_VariantsUnion_TypeOfAlpha_1",
            "ExistingRoot_Nested"
        ) @((Get-JsonProperties -Value $schemas).Name) "Generated component names or order were incorrect."
    }

    # A direct oneOf without a discriminator is visibly a union. A unique object
    # branch receives a shape role instead of an opaque numeric suffix.
    Invoke-Test "non-discriminated oneOf owners and unique shapes use union roles" {
        $source = Read-TestJsonFixture `
            -RelativePath "Componentize-OpenApiObjectSchemas/source.openapi.json"
        $source.paths["/widgets"].post.requestBody.content["application/json"].schema.properties["toolChoice"] = [ordered]@{
            oneOf = @(
                [ordered]@{
                    type = "string"
                    enum = @("auto", "none")
                },
                [ordered]@{
                    type = "object"
                    properties = [ordered]@{
                        type = [ordered]@{
                            type = "string"
                            enum = @("file_search", "web_search")
                        }
                    }
                }
            )
        }

        $result = ConvertTo-ComponentizedOpenApiDocument -Document $source
        $wrapper = @($result.Candidates | Where-Object {
            $_.Pointer.EndsWith("/properties/toolChoice", [System.StringComparison]::Ordinal)
        })
        $branch = @($result.Candidates | Where-Object {
            $_.Pointer.EndsWith("/properties/toolChoice/oneOf/1", [System.StringComparison]::Ordinal)
        })
        Assert-Equal 1 $wrapper.Count "Direct oneOf owner was not discovered exactly once."
        Assert-Equal "OneOf" $wrapper[0].Kind "Direct oneOf owner had the wrong candidate kind."
        Assert-True $wrapper[0].IsNonDiscriminatedUnion "Direct oneOf owner was not classified as a union."
        Assert-Equal "CreateWidget_Request_ToolChoiceUnion" $wrapper[0].Name "Direct oneOf owner omitted its union marker."
        Assert-Equal "CreateWidget_Request_ToolChoiceUnion_Object" $branch[0].Name "Unique object branch omitted its shape role."
        Assert-Equal `
            "#/components/schemas/CreateWidget_Request_ToolChoiceUnion" `
            $result.Document.components.schemas.CreateWidget_Request.properties.toolChoice.'$ref' `
            "Direct oneOf owner was not replaced by its component reference."
        Assert-Equal `
            "#/components/schemas/CreateWidget_Request_ToolChoiceUnion_Object" `
            $result.Document.components.schemas.CreateWidget_Request_ToolChoiceUnion.oneOf[1].'$ref' `
            "Componentized oneOf did not reference its object branch."
    }

    # Structural-only composition children inherit the complete semantic parent
    # name, including an ancestor's TypeOf label, before collision suffixing.
    Invoke-Test "structural-only children retain their semantic parent name" {
        $source = Read-TestJsonFixture `
            -RelativePath "Componentize-OpenApiObjectSchemas/source.openapi.json"
        $source.paths["/widgets"].post.requestBody.content["application/json"].schema.properties["input"] = [ordered]@{
            type = "object"
            required = @("type", "content")
            properties = [ordered]@{
                type = [ordered]@{ type = "string"; enum = @("message") }
                content = [ordered]@{
                    type = "array"
                    items = [ordered]@{
                        oneOf = @(
                            [ordered]@{ oneOf = @([ordered]@{ type = "string" }, [ordered]@{ type = "null" }) },
                            [ordered]@{ oneOf = @([ordered]@{ type = "number" }, [ordered]@{ type = "boolean" }) }
                        )
                    }
                }
            }
        }

        $result = ConvertTo-ComponentizedOpenApiDocument -Document $source
        $contentPointer = "#/paths/~1widgets/post/requestBody/content/application~1json/schema/properties/input/properties/content/items"
        $contentCandidates = @($result.Candidates | Where-Object {
            $_.Pointer.StartsWith($contentPointer, [System.StringComparison]::Ordinal)
        })
        Assert-Sequence @(
            "CreateWidget_Request_Input_TypeOfMessage_ContentUnion",
            "CreateWidget_Request_Input_TypeOfMessage_ContentUnion_Alternative1Union",
            "CreateWidget_Request_Input_TypeOfMessage_ContentUnion_Alternative2Union"
        ) @($contentCandidates.Name) "Structural-only children lost inherited semantic context."
    }

    Invoke-Test "array item and additional-properties unions use storage roles" {
        $source = Read-TestJsonFixture `
            -RelativePath "Componentize-OpenApiObjectSchemas/source.openapi.json"
        $root = $source.paths["/widgets"].post.requestBody.content["application/json"].schema
        $root.properties["input"] = [ordered]@{
            oneOf = @(
                [ordered]@{ type = "string" },
                [ordered]@{
                    type = "array"
                    items = [ordered]@{ type = "object"; properties = [ordered]@{} }
                }
            )
        }
        $root.properties["metadata"] = [ordered]@{
            type = "object"
            additionalProperties = [ordered]@{
                oneOf = @(
                    [ordered]@{ type = "string" },
                    [ordered]@{ type = "number" }
                )
            }
        }

        $result = ConvertTo-ComponentizedOpenApiDocument -Document $source
        $names = @($result.Candidates.Name)
        Assert-True ($names -ccontains "CreateWidget_Request_InputUnion") "Input union omitted its marker."
        Assert-True ($names -ccontains "CreateWidget_Request_InputUnion_ArrayItem") "Array item omitted its role."
        Assert-True ($names -ccontains "CreateWidget_Request_Metadata_ValueUnion") "Dictionary value union omitted its role."
    }

    # The candidate is the complete Schema Object, so siblings of oneOf move into
    # the component together. Direct oneOf eligibility wins over type=object and
    # prevents the same source occurrence from producing two components.
    Invoke-Test "oneOf components preserve sibling schema keywords" {
        $source = Read-TestJsonFixture `
            -RelativePath "Componentize-OpenApiObjectSchemas/source.openapi.json"
        $source.paths["/widgets"].post.requestBody.content["application/json"].schema.properties["hybrid"] = [ordered]@{
            type = "object"
            description = "A constrained union."
            maxProperties = 2
            discriminator = [ordered]@{ propertyName = "type" }
            oneOf = @(
                [ordered]@{
                    type = "object"
                    required = @("type")
                    properties = [ordered]@{
                        type = [ordered]@{ type = "string"; enum = @("first") }
                    }
                },
                [ordered]@{
                    type = "object"
                    required = @("type")
                    properties = [ordered]@{
                        type = [ordered]@{ type = "string"; enum = @("second") }
                    }
                }
            )
        }

        $result = ConvertTo-ComponentizedOpenApiDocument -Document $source
        $ownerPointer = "#/paths/~1widgets/post/requestBody/content/application~1json/schema/properties/hybrid"
        $owners = @($result.Candidates | Where-Object Pointer -eq $ownerPointer)
        Assert-Equal 1 $owners.Count "Object/oneOf schema produced duplicate candidates."
        Assert-Equal "OneOf" $owners[0].Kind "Direct oneOf did not take precedence over type=object."
        Assert-True (-not $owners[0].IsNonDiscriminatedUnion) "Discriminated oneOf was classified as a non-discriminated union."
        Assert-Sequence @('$ref') @((Get-JsonProperties -Value $result.Document.components.schemas.CreateWidget_Request.properties.hybrid).Name) "Inline oneOf siblings remained beside the generated ref."
        $component = $result.Document.components.schemas.CreateWidget_Request_Hybrid
        Assert-Equal "object" $component.type "Type sibling did not move with oneOf."
        Assert-Equal "A constrained union." $component.description "Description sibling did not move with oneOf."
        Assert-Equal 2 $component.maxProperties "Constraint sibling did not move with oneOf."
        Assert-Equal "type" $component.discriminator.propertyName "Discriminator sibling did not move with oneOf."
        Assert-Equal 2 $component.oneOf.Count "oneOf branches changed during componentization."
    }

    # Mapping keys are payload discriminator values, not generated component
    # names. Materialization happens after branch components receive stable refs.
    Invoke-Test "discriminator mappings use accepted branch values" {
        $source = Read-TestJsonFixture `
            -RelativePath "Componentize-OpenApiObjectSchemas/source.openapi.json"
        $source.paths["/widgets"].post.requestBody.content["application/json"].schema.properties["mappedChoice"] = [ordered]@{
            discriminator = [ordered]@{
                propertyName = "kind"
                mapping = [ordered]@{
                    red = "#/paths/~1widgets/post/requestBody/content/application~1json/schema/properties/mappedChoice/oneOf/0"
                }
            }
            oneOf = @(
                [ordered]@{
                    type = "object"
                    required = @("kind")
                    properties = [ordered]@{
                        kind = [ordered]@{ type = "string"; enum = @("red") }
                    }
                },
                [ordered]@{
                    type = "object"
                    required = @("kind")
                    properties = [ordered]@{
                        kind = [ordered]@{ type = "string"; enum = @("blue") }
                    }
                }
            )
        }

        $result = ConvertTo-ComponentizedOpenApiDocument -Document $source
        $choice = $result.Document.components.schemas.CreateWidget_Request_MappedChoice
        Assert-Sequence @("red", "blue") @((Get-JsonProperties -Value $choice.discriminator.mapping).Name) "Mapping keys were not accepted discriminator values."
        Assert-Sequence `
            @(
                "#/components/schemas/CreateWidget_Request_MappedChoice_1",
                "#/components/schemas/CreateWidget_Request_MappedChoice_2"
            ) `
            @((Get-JsonProperties -Value $choice.discriminator.mapping).Value) `
            "Mapping targets did not follow branch encounter order."
        Assert-Equal 1 $result.DiscriminatorCount "Discriminator count was incorrect."
        Assert-Equal 1 $result.ChangedDiscriminatorMappingCount "Partial mapping was not completed."
        Assert-Equal 2 $result.DiscriminatorMappingEntryCount "Mapping entry count was incorrect."
    }

    # Mapping cardinality follows accepted values, so a multi-value enum creates
    # aliases to one branch while const contributes one key to another branch.
    Invoke-Test "multi-value enum and const mappings are exhaustive" {
        $source = Read-TestJsonFixture `
            -RelativePath "Componentize-OpenApiObjectSchemas/source.openapi.json"
        $source.paths["/widgets"].post.requestBody.content["application/json"].schema.properties["finiteChoice"] = [ordered]@{
            discriminator = [ordered]@{ propertyName = "kind" }
            oneOf = @(
                [ordered]@{
                    type = "object"
                    required = @("kind")
                    properties = [ordered]@{
                        kind = [ordered]@{ type = "string"; enum = @("red", "crimson") }
                    }
                },
                [ordered]@{
                    type = "object"
                    required = @("kind")
                    properties = [ordered]@{
                        kind = [ordered]@{ type = "string"; const = "blue" }
                    }
                }
            )
        }

        $result = ConvertTo-ComponentizedOpenApiDocument -Document $source
        $mapping = $result.Document.components.schemas.CreateWidget_Request_FiniteChoice.discriminator.mapping
        Assert-Sequence @("red", "crimson", "blue") @((Get-JsonProperties -Value $mapping).Name) "Finite mapping keys were incomplete or reordered."
        Assert-Sequence `
            @(
                "#/components/schemas/CreateWidget_Request_FiniteChoice_1",
                "#/components/schemas/CreateWidget_Request_FiniteChoice_1",
                "#/components/schemas/CreateWidget_Request_FiniteChoice_2"
            ) `
            @((Get-JsonProperties -Value $mapping).Value) `
            "Finite mapping targets were incorrect."
    }

    # Locks the production input-content shape that motivated this contract: generated
    # TypeOf names are targets, while snake_case payload values remain the keys.
    Invoke-Test "input content mappings use singleton type enum values" {
        $source = Read-TestJsonFixture `
            -RelativePath "Componentize-OpenApiObjectSchemas/source.openapi.json"
        $branches = [System.Collections.Generic.List[object]]::new()
        foreach ($value in @("input_text", "input_image", "input_file")) {
            $branches.Add([ordered]@{
                type = "object"
                required = @("type")
                properties = [ordered]@{
                    type = [ordered]@{ type = "string"; enum = @($value) }
                }
            })
        }
        $source.paths["/widgets"].post.requestBody.content["application/json"].schema.properties["inputContent"] = [ordered]@{
            discriminator = [ordered]@{ propertyName = "type" }
            oneOf = [object[]]$branches.ToArray()
        }

        $result = ConvertTo-ComponentizedOpenApiDocument -Document $source
        $mapping = $result.Document.components.schemas.CreateWidget_Request_InputContent.discriminator.mapping
        Assert-Sequence @("input_text", "input_image", "input_file") @((Get-JsonProperties -Value $mapping).Name) "Input-content mapping keys were incorrect."
        Assert-Sequence @(
            "#/components/schemas/CreateWidget_Request_InputContent_TypeOfInputText",
            "#/components/schemas/CreateWidget_Request_InputContent_TypeOfInputImage",
            "#/components/schemas/CreateWidget_Request_InputContent_TypeOfInputFile"
        ) @((Get-JsonProperties -Value $mapping).Value) "Input-content mapping targets were incorrect."
    }

    # anyOf uses the same alternative contract. allOf does not list alternatives,
    # so it requires an explicit exhaustive map and is never inferred.
    Invoke-Test "anyOf mappings infer while allOf mappings remain explicit" {
        $schemas = [ordered]@{}
        $schemas.Red = [ordered]@{
            type = "object"
            required = @("kind")
            properties = [ordered]@{ kind = [ordered]@{ type = "string"; enum = @("red") } }
        }
        $schemas.Blue = [ordered]@{
            type = "object"
            required = @("kind")
            properties = [ordered]@{ kind = [ordered]@{ type = "string"; const = "blue" } }
        }
        $schemas.AnyChoice = [ordered]@{
            discriminator = [ordered]@{ propertyName = "kind" }
            anyOf = @(
                [ordered]@{ '$ref' = "#/components/schemas/Red" },
                [ordered]@{ '$ref' = "#/components/schemas/Blue" }
            )
        }
        $schemas.AllChoice = [ordered]@{
            discriminator = [ordered]@{
                propertyName = "kind"
                mapping = [ordered]@{
                    red = "Red"
                    blue = "#/components/schemas/Blue"
                }
            }
            allOf = @([ordered]@{ '$ref' = "#/components/schemas/Red" })
        }
        $document = [ordered]@{
            openapi = "3.1.0"
            info = [ordered]@{ title = "mapping"; version = "1" }
            paths = [ordered]@{}
            components = [ordered]@{ schemas = $schemas }
        }

        $result = ConvertTo-ComponentizedOpenApiDocument -Document $document
        Assert-Sequence @("red", "blue") @((Get-JsonProperties -Value $result.Document.components.schemas.AnyChoice.discriminator.mapping).Name) "anyOf mapping was not inferred."
        Assert-Sequence `
            @("#/components/schemas/Red", "#/components/schemas/Blue") `
            @((Get-JsonProperties -Value $result.Document.components.schemas.AllChoice.discriminator.mapping).Value) `
            "allOf mapping targets were not canonicalized."

        $missing = Copy-JsonValue -Value $document
        $null = $missing.components.schemas.AllChoice.discriminator.Remove("mapping")
        Assert-Throws {
            ConvertTo-ComponentizedOpenApiDocument -Document $missing
        } "*allOf discriminator*requires a nonempty explicit mapping*"
    }

    # Invalid metadata fails before output: keys must be accepted, targets must
    # match the owning branch, values must be disjoint, and every branch must be
    # an object component with a required finite discriminator property.
    Invoke-Test "invalid discriminator mappings and alternatives fail clearly" {
        function New-InvalidDiscriminatorSource {
            $source = Read-TestJsonFixture `
                -RelativePath "Componentize-OpenApiObjectSchemas/source.openapi.json"
            $source.paths["/widgets"].post.requestBody.content["application/json"].schema.properties["invalidChoice"] = [ordered]@{
                discriminator = [ordered]@{ propertyName = "kind" }
                oneOf = @(
                    [ordered]@{
                        type = "object"
                        required = @("kind")
                        properties = [ordered]@{ kind = [ordered]@{ type = "string"; enum = @("red") } }
                    },
                    [ordered]@{
                        type = "object"
                        required = @("kind")
                        properties = [ordered]@{ kind = [ordered]@{ type = "string"; enum = @("blue") } }
                    }
                )
            }
            return $source
        }

        $optional = New-InvalidDiscriminatorSource
        $null = $optional.paths["/widgets"].post.requestBody.content["application/json"].schema.properties.invalidChoice.oneOf[1].Remove("required")
        Assert-Throws { ConvertTo-ComponentizedOpenApiDocument -Document $optional } "*property 'kind' must be required*"

        $duplicate = New-InvalidDiscriminatorSource
        $duplicate.paths["/widgets"].post.requestBody.content["application/json"].schema.properties.invalidChoice.oneOf[1].properties.kind.enum = @("red")
        Assert-Throws { ConvertTo-ComponentizedOpenApiDocument -Document $duplicate } "*value 'red'*also accepted*"

        $extraAlias = New-InvalidDiscriminatorSource
        $extraAlias.paths["/widgets"].post.requestBody.content["application/json"].schema.properties.invalidChoice.discriminator["mapping"] = [ordered]@{ legacy = "#/paths/~1widgets/post/requestBody/content/application~1json/schema/properties/invalidChoice/oneOf/0" }
        Assert-Throws { ConvertTo-ComponentizedOpenApiDocument -Document $extraAlias } "*mapping key 'legacy'*not accepted*"

        $wrongTarget = New-InvalidDiscriminatorSource
        $wrongTarget.paths["/widgets"].post.requestBody.content["application/json"].schema.properties.invalidChoice.discriminator["mapping"] = [ordered]@{ red = "#/paths/~1widgets/post/requestBody/content/application~1json/schema/properties/invalidChoice/oneOf/1" }
        Assert-Throws { ConvertTo-ComponentizedOpenApiDocument -Document $wrongTarget } "*mapping key 'red'*instead of*"

        $primitive = New-InvalidDiscriminatorSource
        $primitive.paths["/widgets"].post.requestBody.content["application/json"].schema.properties.invalidChoice.oneOf[1] = [ordered]@{ type = "string"; enum = @("blue") }
        Assert-Throws { ConvertTo-ComponentizedOpenApiDocument -Document $primitive } "*alternative*must contain only a string `$ref*"
    }

    # A nested candidate inherits the containing candidate's semantic enum name,
    # not only literal source-pointer tokens around the nested occurrence. This
    # synthetic extension models a production shape like:
    #   variant(type=alpha) -> tools[] -> branch(type=function)
    # and proves `TypeOfAlpha`, which is absent from the raw child pointer, stays
    # in the child's generated name.
    Invoke-Test "nested names inherit enum-derived parent context" {
        $source = Read-TestJsonFixture `
            -RelativePath "Componentize-OpenApiObjectSchemas/source.openapi.json"
        $variant = $source.paths["/widgets"].post.requestBody.content["application/json"].schema.properties.variants.oneOf[0]
        $variant.properties["tools"] = [ordered]@{
            type = "array"
            items = [ordered]@{
                oneOf = @(
                    [ordered]@{
                        type = "object"
                        properties = [ordered]@{
                            type = [ordered]@{
                                type = "string"
                                enum = @("function")
                            }
                        }
                    }
                )
            }
        }

        $result = ConvertTo-ComponentizedOpenApiDocument -Document $source
        $nested = @($result.Candidates | Where-Object {
            $_.Pointer.EndsWith("/properties/tools/items/oneOf/0", [System.StringComparison]::Ordinal)
        })[0]
        Assert-Equal `
            "CreateWidget_Request_VariantsUnion_TypeOfAlpha_ToolsUnion_TypeOfFunction" `
            $nested.Name `
            "Nested component omitted its enum-derived parent name."
    }

    # Identical oneOf branches intentionally remain separate definitions with
    # distinct names and refs; no structural deduplication is permitted.
    Invoke-Test "structurally identical occurrences remain separate components" {
        $source = Read-OpenApiDocument -Path $sourceFixturePath
        $result = ConvertTo-ComponentizedOpenApiDocument -Document $source
        $schemas = $result.Document.components.schemas
        $first = $schemas.CreateWidget_Request_VariantsUnion_TypeOfAlpha
        $second = $schemas.CreateWidget_Request_VariantsUnion_TypeOfAlpha_1
        Assert-True (Test-JsonValuesEqual -Left $first -Right $second) "Duplicate fixture branches were not structurally equal."
        $variantRefs = $schemas.CreateWidget_Request_VariantsUnion.oneOf
        Assert-True (-not [string]::Equals($variantRefs[0].'$ref', $variantRefs[1].'$ref', [System.StringComparison]::Ordinal)) "Duplicate occurrences shared one component."
    }

    # Parent components reference nested components, while each generated root
    # remains the complete object definition that originally occupied its pointer.
    Invoke-Test "nested objects become component reference chains" {
        $source = Read-OpenApiDocument -Path $sourceFixturePath
        $result = ConvertTo-ComponentizedOpenApiDocument -Document $source
        $schemas = $result.Document.components.schemas
        Assert-Equal "#/components/schemas/CreateWidget_Request" $result.Document.paths["/widgets"].post.requestBody.content["application/json"].schema.'$ref' "Root schema was not replaced."
        Assert-Equal "object" $schemas.CreateWidget_Request.type "Root component lost its object type."
        Assert-Equal "#/components/schemas/CreateWidget_Request_Child_TypeOfChildKind" $schemas.CreateWidget_Request.properties.child.'$ref' "Child component ref was incorrect."
        Assert-Equal "#/components/schemas/CreateWidget_Request_Child_TypeOfChildKind_1" $schemas.CreateWidget_Request_Child_TypeOfChildKind.additionalProperties.'$ref' "Additional-properties ref was incorrect."
        Assert-Equal "object" $schemas.CreateWidget_Request_Child_TypeOfChildKind_1.type "Nested component lost its root definition."
        Assert-Equal 0 @(Get-InlineOpenApiObjectSchemaCandidates -Document $result.Document).Count "Componentized output retained inline object candidates."
    }

    # Existing source pointers follow moved definitions using longest-prefix
    # relocation, including refs to complete oneOf owners.
    Invoke-Test "references relocate to generated components" {
        $source = Read-OpenApiDocument -Path $sourceFixturePath
        $result = ConvertTo-ComponentizedOpenApiDocument -Document $source
        $schemas = $result.Document.components.schemas
        Assert-Equal "#/components/schemas/CreateWidget_Request_Child_TypeOfChildKind" $schemas.CreateWidget_Request.properties.childAlias.'$ref' "Existing schema ref was not relocated."
        Assert-Equal "#/components/schemas/CreateWidget_Request_VariantsUnion" $schemas.CreateWidget_Request.properties.variants.'$ref' "oneOf owner was not relocated."
        Assert-OpenApiReferencesResolve -Document $result.Document
    }

    # Components unrelated to schemas are preserved, and existing schema roots
    # stay roots while only their nested inline objects are promoted.
    Invoke-Test "existing component roots and security schemes are preserved" {
        $source = Read-OpenApiDocument -Path $sourceFixturePath
        $result = ConvertTo-ComponentizedOpenApiDocument -Document $source
        Assert-Equal "object" $result.Document.components.schemas.ExistingRoot.type "Existing schema root changed."
        Assert-Equal "#/components/schemas/ExistingRoot_Nested" $result.Document.components.schemas.ExistingRoot.properties.nested.'$ref' "Existing component nested object was not promoted."
        Assert-Equal "apiKey" $result.Document.components.securitySchemes.ApiKey.type "Security scheme type changed."
        Assert-Equal "x-api-key" $result.Document.components.securitySchemes.ApiKey.name "Security scheme name changed."
    }

    # Type arrays and example objects are not exact schema-context object
    # candidates and must survive unchanged.
    Invoke-Test "near misses and literal object lookalikes remain inline" {
        $source = Read-TestJsonFixture `
            -RelativePath "Componentize-OpenApiObjectSchemas/source.openapi.json"
        $source.paths["/widgets"].post.requestBody.content["application/json"].schema.properties.literal.example["oneOf"] = @(
            [ordered]@{ type = "object" }
        )
        $result = ConvertTo-ComponentizedOpenApiDocument -Document $source
        $root = $result.Document.components.schemas.CreateWidget_Request
        Assert-Sequence @("object", "null") @($root.properties.nearMiss.type) "Type-array near miss changed."
        Assert-Equal "object" $root.properties.literal.example.type "Literal object changed."
        Assert-Equal "object" $root.properties.literal.example.properties.ignored.type "Nested literal object changed."
        Assert-Equal 1 $root.properties.literal.example.oneOf.Count "Literal oneOf lookalike changed."
    }

    # Moving a schema that establishes JSON Schema URI or anchor scope can change
    # reference meaning. Every such keyword is rejected with its source pointer.
    Invoke-Test "scope-sensitive schema keywords fail explicitly" {
        foreach ($case in @(
            [PSCustomObject]@{ Keyword = '$id'; Value = "urn:test:root" },
            [PSCustomObject]@{ Keyword = '$anchor'; Value = "root" },
            [PSCustomObject]@{ Keyword = '$dynamicAnchor'; Value = "node" },
            [PSCustomObject]@{ Keyword = '$recursiveAnchor'; Value = $true }
        )) {
            $source = Read-TestJsonFixture `
                -RelativePath "Componentize-OpenApiObjectSchemas/source.openapi.json"
            $schema = $source.paths["/widgets"].post.requestBody.content["application/json"].schema
            $schema[$case.Keyword] = $case.Value
            Assert-Throws {
                ConvertTo-ComponentizedOpenApiDocument -Document $source
            } "*scope-sensitive keyword '$($case.Keyword)'*schema*$($case.Keyword)*"
        }

        $inherited = Read-TestJsonFixture `
            -RelativePath "Componentize-OpenApiObjectSchemas/source.openapi.json"
        $inherited.components.schemas.ExistingRoot['$id'] = "urn:test:existing"
        Assert-Throws {
            ConvertTo-ComponentizedOpenApiDocument -Document $inherited
        } "*inherits scope from `$id*ExistingRoot*`$id*"
    }

    # Readable names are intentionally not truncated, and a unique long name
    # does not acquire a numeric suffix.
    Invoke-Test "long readable component names are not truncated" {
        $source = Read-TestJsonFixture `
            -RelativePath "Componentize-OpenApiObjectSchemas/source.openapi.json"
        $longPropertyName = "segment_" + ("x" * 140)
        $source.paths["/widgets"].post.requestBody.content["application/json"].schema.properties[$longPropertyName] = [ordered]@{
            type = "object"
        }
        $result = ConvertTo-ComponentizedOpenApiDocument -Document $source
        $longCandidate = @($result.Candidates | Where-Object { $_.Pointer.EndsWith("/$longPropertyName", [System.StringComparison]::Ordinal) })[0]
        Assert-True ($longCandidate.Name.Length -gt 100) "Long component name was truncated."
        Assert-True (-not ($longCandidate.Name -match "_[0-9]+$")) "Unique long component received a numeric suffix."
        Assert-True ($longCandidate.Name.StartsWith("CreateWidget_Request_Segment", [System.StringComparison]::Ordinal)) "Long component sections were incorrect."
    }

    # Existing schema names participate in collision checks, causing only the
    # conflicting generated base name to acquire a suffix.
    Invoke-Test "existing component names are never overwritten" {
        $source = Read-TestJsonFixture `
            -RelativePath "Componentize-OpenApiObjectSchemas/source.openapi.json"
        $source.components.schemas["CreateWidget_Request"] = [ordered]@{ type = "string" }
        $result = ConvertTo-ComponentizedOpenApiDocument -Document $source
        Assert-Equal "string" $result.Document.components.schemas.CreateWidget_Request.type "Existing colliding component was overwritten."
        Assert-Equal "#/components/schemas/CreateWidget_Request_1" $result.Document.paths["/widgets"].post.requestBody.content["application/json"].schema.'$ref' "Generated name did not suffix the collision."
    }

    Invoke-Test "union collisions suffix generated names and preserve source names" {
        $document = [ordered]@{
            openapi = "3.1.0"
            info = [ordered]@{ title = "union names"; version = "1" }
            paths = [ordered]@{}
            components = [ordered]@{
                schemas = [ordered]@{
                    Legacy_Union = [ordered]@{ type = "string" }
                    Container_InputUnion = [ordered]@{ type = "string" }
                    Container = [ordered]@{
                        type = "object"
                        properties = [ordered]@{
                            input = [ordered]@{
                                oneOf = @(
                                    [ordered]@{ type = "string" },
                                    [ordered]@{ type = "number" }
                                )
                            }
                        }
                    }
                }
            }
        }

        $result = ConvertTo-ComponentizedOpenApiDocument -Document $document
        Assert-Equal "string" $result.Document.components.schemas.Legacy_Union.type "Source-authored underscore Union name changed."
        Assert-Equal "string" $result.Document.components.schemas.Container_InputUnion.type "Reserved concatenated Union name changed."
        Assert-Equal `
            "#/components/schemas/Container_InputUnion_1" `
            $result.Document.components.schemas.Container.properties.input.'$ref' `
            "Generated Union collision did not use the normal numeric suffix."
    }

    # Once every inline candidate has moved, a second conversion performs no
    # work and preserves the complete document structurally.
    Invoke-Test "componentization is structurally idempotent" {
        $source = Read-OpenApiDocument -Path $sourceFixturePath
        $first = ConvertTo-ComponentizedOpenApiDocument -Document $source
        $second = ConvertTo-ComponentizedOpenApiDocument -Document $first.Document
        Assert-Equal 0 $second.DiscoveredSchemaCount "Second pass rediscovered candidates."
        Assert-Equal 0 $second.DiscoveredObjectSchemaCount "Second pass rediscovered candidates."
        Assert-Equal 0 $second.DiscoveredOneOfSchemaCount "Second pass rediscovered oneOf candidates."
        Assert-Equal 0 $second.GeneratedComponentCount "Second pass generated components."
        Assert-JsonEqual $first.Document $second.Document "Second pass changed the componentized document."
    }

    # Default naming supports arbitrary sibling inputs and recognizes already
    # componentized paths without repeating the suffix.
    Invoke-Test "default output paths insert the componentized marker once" {
        $ordinary = Join-Path $temporaryDirectory "sample.openapi.json"
        $expected = Join-Path $temporaryDirectory "sample.componentized.openapi.json"
        Assert-Equal ([System.IO.Path]::GetFullPath($expected)) (Get-DefaultComponentizedOpenApiPath -InputPath $ordinary) "OpenAPI default output path was incorrect."
        Assert-Equal ([System.IO.Path]::GetFullPath($expected)) (Get-DefaultComponentizedOpenApiPath -InputPath $expected) "Componentized suffix repeated."
        Assert-Equal ([System.IO.Path]::GetFullPath((Join-Path $temporaryDirectory "sample.componentized.json"))) (Get-DefaultComponentizedOpenApiPath -InputPath (Join-Path $temporaryDirectory "sample.json")) "Plain JSON output path was incorrect."
    }

    # Public file handling preserves the source and emits deterministic UTF-8
    # bytes for default and explicit destinations. Byte checks are intentional:
    # these artifacts are committed, so BOM or newline drift creates noisy diffs
    # even when the parsed JSON is equivalent.
    Invoke-Test "CLI default and explicit outputs are deterministic and preserve input" {
        $inputPath = Join-Path $temporaryDirectory "sample.openapi.json"
        Copy-Item -LiteralPath $sourceFixturePath -Destination $inputPath
        [byte[]]$inputBefore = [System.IO.File]::ReadAllBytes($inputPath)
        $null = Invoke-OpenApiObjectSchemaComponentizer -InputPath $inputPath
        $defaultOutput = Join-Path $temporaryDirectory "sample.componentized.openapi.json"
        Assert-True ([System.IO.File]::Exists($defaultOutput)) "Default output was not created."
        Assert-BytesEqual $inputBefore ([System.IO.File]::ReadAllBytes($inputPath)) "Componentizer modified its input."
        [byte[]]$defaultBytes = [System.IO.File]::ReadAllBytes($defaultOutput)
        $hasBom = $defaultBytes.Length -ge 3 -and $defaultBytes[0] -eq 0xEF -and $defaultBytes[1] -eq 0xBB -and $defaultBytes[2] -eq 0xBF
        Assert-True (-not $hasBom) "Output contains a UTF-8 BOM."
        Assert-Equal 10 $defaultBytes[$defaultBytes.Length - 1] "Output lacks a trailing LF."

        $explicitOutput = Join-Path $temporaryDirectory "explicit.json"
        $null = Invoke-OpenApiObjectSchemaComponentizer `
            -InputPath $inputPath `
            -OutputPath $explicitOutput
        Assert-BytesEqual $defaultBytes ([System.IO.File]::ReadAllBytes($explicitOutput)) "Explicit output differed from default output."
    }

    # Reprocessing a componentized file in place is a byte no-op and reports no
    # newly generated components.
    Invoke-Test "CLI componentization is byte-idempotent in place" {
        $inputPath = Join-Path $temporaryDirectory "already.componentized.openapi.json"
        Copy-Item -LiteralPath $expectedFixturePath -Destination $inputPath
        $null = Invoke-OpenApiObjectSchemaComponentizer -InputPath $inputPath
        [byte[]]$firstRun = [System.IO.File]::ReadAllBytes($inputPath)
        $second = Invoke-OpenApiObjectSchemaComponentizer -InputPath $inputPath
        Assert-Equal 0 $second.GeneratedComponentCount "In-place second pass generated components."
        Assert-BytesEqual $firstRun ([System.IO.File]::ReadAllBytes($inputPath)) "In-place componentization changed bytes."
    }

    # Input validation belongs at the CLI boundary and must fail before writing
    # a partial componentized artifact.
    Invoke-Test "missing directory and invalid JSON inputs fail clearly" {
        Assert-Throws {
            Invoke-OpenApiObjectSchemaComponentizer -InputPath (Join-Path $temporaryDirectory "missing.json")
        } "*cannot find path*"
        Assert-Throws {
            Invoke-OpenApiObjectSchemaComponentizer -InputPath $temporaryDirectory
        } "*is not a file*"

        $invalidPath = Join-Path $temporaryDirectory "invalid.json"
        [System.IO.File]::WriteAllText($invalidPath, "{ invalid", [System.Text.UTF8Encoding]::new($false))
        Assert-Throws {
            Invoke-OpenApiObjectSchemaComponentizer -InputPath $invalidPath
        } "*Could not parse OpenAPI JSON*"

        $unsafe = Read-TestJsonFixture `
            -RelativePath "Componentize-OpenApiObjectSchemas/source.openapi.json"
        $unsafe.paths["/widgets"].post.requestBody.content["application/json"].schema['$anchor'] = "root"
        $unsafePath = Join-Path $temporaryDirectory "unsafe.json"
        $unsafeOutputPath = Join-Path $temporaryDirectory "unsafe-output.json"
        $null = Write-OpenApiJsonFile -Document $unsafe -Path $unsafePath
        [byte[]]$sentinel = [System.Text.Encoding]::UTF8.GetBytes("existing output`n")
        [System.IO.File]::WriteAllBytes($unsafeOutputPath, $sentinel)
        Assert-Throws {
            Invoke-OpenApiObjectSchemaComponentizer `
                -InputPath $unsafePath `
                -OutputPath $unsafeOutputPath
        } "*scope-sensitive keyword*`$anchor*"
        Assert-BytesEqual $sentinel ([System.IO.File]::ReadAllBytes($unsafeOutputPath)) "Failed componentization changed output bytes."
    }
}
finally {
    # Each suite run owns a GUID-named directory, making cleanup safe even when
    # multiple local or CI invocations execute concurrently.
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
