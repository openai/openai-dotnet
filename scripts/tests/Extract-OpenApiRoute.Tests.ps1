<#
.SYNOPSIS
    Runs focused tests for scripts/Extract-OpenApiRoute.ps1.

.DESCRIPTION
    Uses a small built-in assertion runner so the repository does not need an
    additional PowerShell test dependency. Behavior is exercised against a
    compact fixture so this focused suite remains fast and deterministic.
#>

[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$scriptPath = Join-Path $PSScriptRoot ".." "Extract-OpenApiRoute.ps1"
. $scriptPath
. (Join-Path $PSScriptRoot "_OpenApiTestFixture.Helpers.ps1")

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

# Recursively returns properties that violate the contextual production
# omission policy. Required OpenAPI members and schema-map field names are not
# reported merely because their names overlap omitted metadata keywords.
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

    if ($null -eq $Value) {
        return
    }
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
    if (-not (Test-JsonObjectValue -Value $Value)) {
        return
    }

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

$temporaryDirectory = Join-Path ([System.IO.Path]::GetTempPath()) "openapi-route-tests-$([Guid]::NewGuid().ToString('N'))"
$null = [System.IO.Directory]::CreateDirectory($temporaryDirectory)
$fixtureCases = Read-TestJsonFixture -RelativePath "Extract-OpenApiRoute/cases.json"

try {
    $fixture = Read-TestJsonFixture -RelativePath "Flatten-OpenApiAllOf/source.openapi.json"
    $catalog = Get-OpenApiOperationCatalog -Document $fixture
    $operationEntry = Select-OpenApiOperation `
        -Catalog $catalog `
        -RequestedOperationId "createThing"

    # The public command accepts exact operation selection plus the three
    # supported content projections. Route selection remains intentionally absent.
    Invoke-Test "public script exposes operationId and validated extraction modes" {
        $command = Get-Command $scriptPath
        Assert-True ($command.Parameters.ContainsKey("OperationId")) "OperationId parameter was missing."
        Assert-True ($command.Parameters.ContainsKey("Mode")) "Mode parameter was missing."
        Assert-True (-not $command.Parameters.ContainsKey("Route")) "Legacy Route parameter remained."
        $validateSet = @($command.Parameters["Mode"].Attributes | Where-Object {
            $_ -is [System.Management.Automation.ValidateSetAttribute]
        })
        Assert-Equal 1 $validateSet.Count "Mode did not have one ValidateSet contract."
        Assert-Sequence @("RequestBody", "Responses", "Both") @($validateSet[0].ValidValues) "Mode values were incorrect."
    }

    # The interactive menu and direct lookup share this catalog, so it must be
    # complete and deterministic.
    Invoke-Test "catalog includes path operations and sorts operationIds" {
        Assert-Equal 5 $catalog.Count "Catalog count was incorrect."
        Assert-Sequence `
            @("createBetaResponse", "createThing", "listFineTuningJobs", "listThings", "listZebraItems") `
            @($catalog.OperationId) `
            "OperationIds were not sorted."
    }

    # OperationId, rather than route, is the selection key because one path can
    # expose several HTTP operations.
    Invoke-Test "multiple operations on one route are independently selectable" {
        $sameRouteOperations = @($catalog | Where-Object Route -eq "/things")
        Assert-Sequence @("createThing", "listThings") @($sameRouteOperations.OperationId) "Same-route operations were not cataloged independently."
        Assert-Equal "post" $operationEntry.Method "OperationId selected the wrong HTTP method."
        Assert-Equal "/things" $operationEntry.Route "OperationId selected the wrong route."
    }

    # Duplicate operationIds would make direct and interactive selection
    # ambiguous, so extraction must reject the source document.
    Invoke-Test "duplicate operationIds fail clearly" {
        $duplicateFixture = Copy-JsonValue -Value $fixture
        $duplicateFixture.paths["/things"].get.operationId = "createThing"
        Assert-Throws `
            { Get-OpenApiOperationCatalog -Document $duplicateFixture } `
            "*operationId 'createThing' is not unique*"
    }

    # Omission affects both emitted JSON and dependency discovery; exact JSON
    # property-name casing and structural exceptions must therefore be explicit.
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

    # The omission list is intended as configuration, not a hard-coded set of
    # branches in the traversal implementation.
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

    # Extract owns omission but not canonical sorting, so surviving encounter
    # order must remain available to later stages.
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

    # Interactive extraction must recover from malformed/out-of-range input
    # without changing the displayed one-based numbering contract.
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

    # Explicit modes keep automation noninteractive, while an omitted mode uses
    # the same retrying one-based menu as operation selection.
    Invoke-Test "mode selection supports explicit values and an interactive menu" {
        $explicit = Select-OpenApiExtractionMode `
            -RequestedMode "responses" `
            -InputProvider { throw "Explicit mode unexpectedly prompted." }
        Assert-Equal "Responses" $explicit "Explicit mode was not canonicalized."

        $script:modeAnswers = [System.Collections.Generic.Queue[string]]::new()
        $script:modeAnswers.Enqueue("invalid")
        $script:modeAnswers.Enqueue("1")
        $interactive = Select-OpenApiExtractionMode `
            -InputProvider { param($Prompt) $script:modeAnswers.Dequeue() }
        Assert-Equal "RequestBody" $interactive "Interactive mode selected the wrong value."
        Assert-Equal "Request Body" (Get-OpenApiExtractionModeLabel -Mode $interactive) "Mode label was incorrect."
        Assert-Throws {
            Select-OpenApiExtractionMode -RequestedMode "Invalid"
        } "*does not belong to the set*"
    }

    # Component names may contain RFC 6901 reserved characters; incorrect
    # decoding would resolve or inline the wrong schema.
    Invoke-Test "JSON Pointer handles escaped slash and tilde tokens" {
        $pointerFixture = Get-TestFixtureCase -Cases $fixtureCases -Name "escapedPointer"
        $value = Resolve-LocalJsonPointer `
            -Document $pointerFixture `
            -Pointer "#/components/schemas/A~1B~0C"
        Assert-Equal "found" $value "Escaped pointer resolved incorrectly."
    }

    $sourceOperationBeforeModes = ConvertTo-Json `
        -InputObject $fixture.paths["/things"].post `
        -Compress `
        -Depth 100
    $inlined = New-ExtractedOpenApiDocument `
        -Document $fixture `
        -OperationEntry $operationEntry
    $explicitBoth = New-ExtractedOpenApiDocument `
        -Document $fixture `
        -OperationEntry $operationEntry `
        -Mode Both
    $requestBodyOnly = New-ExtractedOpenApiDocument `
        -Document $fixture `
        -OperationEntry $operationEntry `
        -Mode RequestBody
    $responsesOnly = New-ExtractedOpenApiDocument `
        -Document $fixture `
        -OperationEntry $operationEntry `
        -Mode Responses
    $lowercaseResponses = New-ExtractedOpenApiDocument `
        -Document $fixture `
        -OperationEntry $operationEntry `
        -Mode responses

    # The lower-level default remains Both so existing programmatic callers and
    # the complete extraction behavior retain their current contract.
    Invoke-Test "default extraction is structurally identical to explicit Both" {
        $defaultJson = ConvertTo-Json -InputObject $inlined.Document -Compress -Depth 100
        $explicitJson = ConvertTo-Json -InputObject $explicitBoth.Document -Compress -Depth 100
        Assert-Equal $defaultJson $explicitJson "Explicit Both changed the extracted document."
        Assert-Equal $inlined.DependencyCount $explicitBoth.DependencyCount "Explicit Both changed dependency count."
        Assert-Equal $inlined.CycleCount $explicitBoth.CycleCount "Explicit Both changed cycle count."
    }

    Invoke-Test "lower-level mode values follow case-insensitive PowerShell binding" {
        $canonicalJson = ConvertTo-Json -InputObject $responsesOnly.Document -Compress -Depth 100
        $lowercaseJson = ConvertTo-Json -InputObject $lowercaseResponses.Document -Compress -Depth 100
        Assert-Equal $canonicalJson $lowercaseJson "Lowercase Responses changed the projected document."
    }

    # Every mode builds an independent operation projection; none may replace
    # or remove sections on the parsed source used by later extractions.
    Invoke-Test "mode projections do not mutate the source operation" {
        $sourceOperationAfterModes = ConvertTo-Json `
            -InputObject $fixture.paths["/things"].post `
            -Compress `
            -Depth 100
        Assert-Equal $sourceOperationBeforeModes $sourceOperationAfterModes "A mode projection mutated the source operation."
    }

    # Path-level parameters apply to every operation, but sibling HTTP methods
    # must not leak into an operation-scoped document.
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

    # RequestBody mode preserves the request and all unrelated metadata, but
    # swaps responses in place for a conformant reference-free placeholder.
    Invoke-Test "RequestBody mode keeps metadata and emits only a placeholder response" {
        $pathItem = $requestBodyOnly.Document.paths["/things"]
        $operation = $pathItem.post
        Assert-True (Test-JsonProperty -Object $pathItem -Name "parameters") "Path-level parameters were removed."
        Assert-True (Test-JsonProperty -Object $operation -Name "requestBody") "Request body was removed."
        Assert-Sequence @("default") @((Get-JsonProperties -Value $operation.responses).Name) "Original responses remained."
        Assert-Sequence @("description") @((Get-JsonProperties -Value $operation.responses.default).Name) "Placeholder response shape was incorrect."
        Assert-Equal "Responses omitted by RequestBody extraction mode." $operation.responses.default.description "Placeholder description was incorrect."
        Assert-Sequence @("Things") @($operation.tags) "Operation tags changed."
        Assert-True (Test-JsonProperty -Object $operation -Name "security") "Operation security was removed."
        Assert-Sequence `
            @("operationId", "tags", "security", "requestBody", "responses") `
            @((Get-JsonProperties -Value $operation).Name) `
            "RequestBody mode changed surviving operation order."
    }

    # Responses mode removes only the direct operation requestBody. Path and
    # operation metadata stay intact, and request-only recursion no longer
    # contributes fallback schemas or cycle counts.
    Invoke-Test "Responses mode keeps metadata and excludes request-only dependencies" {
        $pathItem = $responsesOnly.Document.paths["/things"]
        $operation = $pathItem.post
        Assert-True (Test-JsonProperty -Object $pathItem -Name "parameters") "Path-level parameters were removed."
        Assert-True (-not (Test-JsonProperty -Object $operation -Name "requestBody")) "Request body remained."
        Assert-True (Test-JsonProperty -Object $operation -Name "responses") "Responses were removed."
        Assert-Sequence @("Things") @($operation.tags) "Operation tags changed."
        Assert-True (Test-JsonProperty -Object $operation -Name "security") "Operation security was removed."
        Assert-Equal 0 $responsesOnly.CycleCount "Request-only cycle remained in Responses mode."
        Assert-True (
            -not (Test-JsonProperty -Object $responsesOnly.Document.components -Name "schemas")
        ) "Request-only fallback schemas remained in Responses mode."
        Assert-True ($responsesOnly.DependencyCount -lt $inlined.DependencyCount) "Responses mode did not reduce retained dependencies."
    }

    # Filtering before expansion means invalid references on the omitted side
    # cannot fail extraction, while the same reference still fails when selected.
    Invoke-Test "selective modes do not traverse omitted-side references" {
        $invalidResponseFixture = Copy-JsonValue -Value $fixture
        $invalidResponseFixture.paths["/things"].post.responses["200"] = [ordered]@{
            '$ref' = "#/components/responses/MissingResponse"
        }
        $invalidResponseOperation = Select-OpenApiOperation `
            -Catalog (Get-OpenApiOperationCatalog -Document $invalidResponseFixture) `
            -RequestedOperationId "createThing"
        $requestResult = New-ExtractedOpenApiDocument `
            -Document $invalidResponseFixture `
            -OperationEntry $invalidResponseOperation `
            -Mode RequestBody
        Assert-Sequence @("default") @((Get-JsonProperties -Value $requestResult.Document.paths["/things"].post.responses).Name) "RequestBody mode retained the bad response."
        Assert-Throws {
            New-ExtractedOpenApiDocument `
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
        $responseResult = New-ExtractedOpenApiDocument `
            -Document $invalidRequestFixture `
            -OperationEntry $invalidRequestOperation `
            -Mode Responses
        Assert-True (-not (Test-JsonProperty -Object $responseResult.Document.paths["/things"].post -Name "requestBody")) "Responses mode retained the bad request."
        Assert-Throws {
            New-ExtractedOpenApiDocument `
                -Document $invalidRequestFixture `
                -OperationEntry $invalidRequestOperation `
                -Mode RequestBody
        } "*has no 'MissingRequest' property*"
    }

    # A selective mode must name the exact absent section rather than silently
    # producing an artifact that does not contain the requested content.
    Invoke-Test "selective modes fail when the requested section is absent" {
        $listOperation = Select-OpenApiOperation `
            -Catalog $catalog `
            -RequestedOperationId "listThings"
        Assert-Throws {
            New-ExtractedOpenApiDocument `
                -Document $fixture `
                -OperationEntry $listOperation `
                -Mode RequestBody
        } "*does not define a requestBody*"

        $missingResponsesFixture = Copy-JsonValue -Value $fixture
        $missingResponsesFixture.paths["/things"].post.Remove("responses")
        $missingResponsesOperation = Select-OpenApiOperation `
            -Catalog (Get-OpenApiOperationCatalog -Document $missingResponsesFixture) `
            -RequestedOperationId "createThing"
        Assert-Throws {
            New-ExtractedOpenApiDocument `
                -Document $missingResponsesFixture `
                -OperationEntry $missingResponsesOperation `
                -Mode Responses
        } "*does not define responses*"
    }

    # Inlined output should retain components only when recursion or copied
    # security requirements still need them.
    Invoke-Test "extraction keeps only cycle fallback and security components" {
        Assert-Sequence @("A") @((Get-JsonProperties -Value $inlined.Document.components.schemas).Name) "Fallback component selection was incorrect."
        Assert-Sequence `
            @("ApiKeyAuth", "AdminApiKeyAuth") `
            @((Get-JsonProperties -Value $inlined.Document.components.securitySchemes).Name) `
            "Security scheme selection was incorrect."
        Assert-Equal 3 $inlined.DependencyCount "Retained component count was incorrect."
        Assert-OpenApiReferencesResolve -Document $inlined.Document
    }

    # Top-level tag metadata is minimized to definitions referenced by the
    # selected operation.
    Invoke-Test "extraction keeps only selected operation tag definitions" {
        Assert-Sequence @("Things") @($inlined.Document.tags.name) "Tag minimization was incorrect."
    }

    # Ordinary references must inline completely; only a reference already on
    # the active expansion path may survive as the finite recursion back-edge.
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

    # Component mapping targets become invalid after inlining, so mappings must
    # identify the corresponding local composition branches.
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

    # OpenAPI 3.1 permits siblings next to $ref. Validating siblings are merged,
    # while configured metadata siblings are removed before the merge.
    Invoke-Test "schema reference metadata siblings are omitted from the inlined target" {
        $alias = $inlined.Document.paths["/things"].post.requestBody.content["application/json"].schema.properties.requiredA
        Assert-True (-not (Test-JsonProperty -Object $alias -Name "allOf")) "Reference sibling introduced allOf."
        Assert-Equal "object" $alias.type "Referenced schema type was lost."
        Assert-True (-not (Test-JsonProperty -Object $alias -Name "description")) "Reference sibling description remained."
        Assert-Equal "cat" $alias.properties.type.const "Referenced schema properties were lost."
    }

    # A Response Reference Object's description sibling is optional metadata,
    # but the concrete target response's description remains required.
    Invoke-Test "response reference description siblings do not replace required target descriptions" {
        $responseFixture = Copy-JsonValue -Value $fixture
        $responseFixture.paths["/things"].post.responses["200"].description = "Discard response reference override"
        $responseOperation = Select-OpenApiOperation `
            -Catalog (Get-OpenApiOperationCatalog -Document $responseFixture) `
            -RequestedOperationId "createThing"
        $responseResult = New-ExtractedOpenApiDocument `
            -Document $responseFixture `
            -OperationEntry $responseOperation `
            -Mode Responses
        Assert-Equal "ok" $responseResult.Document.paths["/things"].post.responses["200"].description "Required target response description changed."
    }

    # Annotation keywords are removed without deleting payload fields that use
    # the same names or references reachable through those field schemas.
    Invoke-Test "metadata names remain valid payload fields in schema maps" {
        $metadataFixture = Copy-JsonValue -Value $fixture
        $metadataFixture.paths["/things"].post.requestBody.content["application/json"].schema = `
            Get-TestFixtureCase -Cases $fixtureCases -Name "metadataAndPayloadNames"
        $metadataOperation = Select-OpenApiOperation `
            -Catalog (Get-OpenApiOperationCatalog -Document $metadataFixture) `
            -RequestedOperationId "createThing"
        $metadataResult = New-ExtractedOpenApiDocument `
            -Document $metadataFixture `
            -OperationEntry $metadataOperation `
            -Mode RequestBody
        $schema = $metadataResult.Document.paths["/things"].post.requestBody.content["application/json"].schema
        Assert-Sequence @("type", "properties", "required") @((Get-JsonProperties -Value $schema).Name) "Schema metadata remained."
        Assert-Sequence @("title", "description", "default") @((Get-JsonProperties -Value $schema.properties).Name) "Payload field names changed."
        Assert-Sequence @("title", "description", "default") @($schema.required) "Required payload names changed."
        Assert-True (-not (Test-JsonProperty -Object $schema.properties.title -Name "description")) "Field description annotation remained."
        Assert-True (-not (Test-JsonProperty -Object $schema.properties.description -Name "title")) "Field title annotation remained."
        Assert-True (-not (Test-JsonProperty -Object $schema.properties.default -Name "description")) "Reference description annotation remained."
        Assert-Equal "object" $schema.properties.default.type "Payload field reference was not expanded."
    }

    # Extract is orthogonal to source allOf composition: it expands references
    # inside those branches but leaves the composition itself for the dedicated
    # Flatten stage.
    Invoke-Test "extraction leaves source allOf intact while expanding references" {
        $preserved = $inlined.Document.paths["/things"].post.requestBody.content["application/json"].schema.properties.flattened
        Assert-True (Test-JsonProperty -Object $preserved -Name "allOf") "Inlined output removed source allOf."
        Assert-True (@(Find-JsonPropertyValues -Value $preserved -Name "allOf").Count -gt 1) "Nested source allOf was removed."
        Assert-Equal 0 @(Get-OpenApiReferenceOccurrences -Value $preserved | Where-Object Kind -eq "Reference").Count "Inlined allOf retained a non-cycle component reference."
    }

    # A component cycle cannot be expanded forever. Extract retains the true
    # back-edge as $ref and emits a minimal fallback component; source allOf
    # encountered along that fallback path must still reach Flatten unchanged.
    Invoke-Test "cycle fallback retains source allOf and its cycle reference" {
        $fallbackChild = $inlined.Document.components.schemas.A.properties.child
        Assert-True (Test-JsonProperty -Object $fallbackChild -Name "allOf") "Cycle fallback removed source allOf."
        Assert-True (@(Get-OpenApiReferenceOccurrences -Value $fallbackChild | Where-Object Kind -eq "Reference").Count -gt 0) "Cycle fallback lost its cycle reference."
    }

    # JSON Schema recursion expressed with $recursiveRef is intentional and
    # must remain finite rather than being expanded as an ordinary $ref.
    Invoke-Test "explicit recursive references remain finite" {
        $recursiveReferences = @(Find-JsonPropertyValues -Value $inlined.Document -Name '$recursiveRef')
        Assert-Sequence @("#") $recursiveReferences "Recursive references were changed or removed."
    }

    # Omission must affect only the copied output; mutating the parsed source
    # would make later extractions depend on test/execution order.
    Invoke-Test "configured and x-prefixed properties are removed without mutating the source" {
        Assert-Equal 0 @(Find-OmittedOpenApiPropertyNames -Value $inlined.Document).Count "Inlined omitted properties remained."
        Assert-Equal "Extractor fixture" $inlined.Document.info.title "Inlined info title was removed."
        Assert-True (-not (Test-JsonProperty -Object $inlined.Document.info -Name "description")) "Inlined info description remained."
        Assert-True (Test-JsonProperty -Object $fixture -Name "x-custom-root") "Source root extension was mutated."
        Assert-True (Test-JsonProperty -Object $fixture.info -Name "x-custom-info") "Source info extension was mutated."
        Assert-True (Test-JsonProperty -Object $fixture.paths["/things"].post -Name "x-hidden-ref") "Source operation extension was mutated."
        Assert-True (Test-JsonProperty -Object $fixture.paths["/things"].post -Name "example") "Source example was mutated."
        Assert-True (Test-JsonProperty -Object $fixture.paths["/things"].post -Name "examples") "Source examples were mutated."
        Assert-True (Test-JsonProperty -Object $fixture.components.schemas.Cat.properties.type -Name "x-stainless-const") "Nested source extension was mutated."
        Assert-True (-not (Test-JsonProperty -Object $inlined.Document.paths["/things"].post.responses["200"].content["application/json"].schema.oneOf[0] -Name "title")) "Schema title remained."
        Assert-Equal "ok" $inlined.Document.paths["/things"].post.responses["200"].description "Required response description was removed."
        Assert-True (-not (Test-JsonProperty -Object $inlined.Document.components.schemas -Name "Unused")) "Extraction followed a reference inside an extension."
        Assert-True (-not (Test-JsonProperty -Object $inlined.Document.components.schemas -Name "ExampleOnly")) "Extraction followed a reference inside example."
        Assert-True (-not (Test-JsonProperty -Object $inlined.Document.components.schemas -Name "ExamplesOnly")) "Extraction followed a reference inside examples."
    }

    # Canonical ordering belongs to Sort, so Extract must preserve schema,
    # property, and required-name encounter order after omission/inlining.
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

    # Extraction supports only resolvable local component references and should
    # report the failing reference rather than emit a dangling document.
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

    # OperationIds become filenames across platforms; sanitization collisions
    # need a stable suffix rather than accidental overwrites.
    Invoke-Test "default filenames are portable and collision-aware" {
        $ordinaryPath = Get-DefaultOutputPath `
            -OperationId "createThing" `
            -Catalog $catalog `
            -RepositoryRoot $temporaryDirectory
        Assert-Equal "createThing.inlined.openapi.json" (Split-Path $ordinaryPath -Leaf) "Ordinary operationId filename was incorrect."

        $requestPath = Get-DefaultOutputPath `
            -OperationId "createThing" `
            -Catalog $catalog `
            -RepositoryRoot $temporaryDirectory `
            -Mode RequestBody
        Assert-Equal "createThing.request-body.inlined.openapi.json" (Split-Path $requestPath -Leaf) "RequestBody filename was incorrect."

        $responsePath = Get-DefaultOutputPath `
            -OperationId "createThing" `
            -Catalog $catalog `
            -RepositoryRoot $temporaryDirectory `
            -Mode Responses
        Assert-Equal "createThing.responses.inlined.openapi.json" (Split-Path $responsePath -Leaf) "Responses filename was incorrect."

        $collisionCatalog = @(
            foreach ($operationId in $fixtureCases.collisionOperationIds) {
                [PSCustomObject]@{ OperationId = $operationId }
            }
        )
        $path = Get-DefaultOutputPath `
            -OperationId "foo?bar" `
            -Catalog $collisionCatalog `
            -RepositoryRoot $temporaryDirectory `
            -Mode Responses
        $name = Split-Path $path -Leaf
        Assert-True ($name -match '^foo-bar-[0-9a-f]{8}\.responses\.inlined\.openapi\.json$') "Collision-safe filename was unexpected: $name"
    }

    # Explicit modes and output paths make the public command suitable for
    # orchestration without either interactive prompt or mode-based renaming.
    Invoke-Test "public invocation supports explicit noninteractive modes and output paths" {
        $fixturePath = Join-Path $script:OpenApiTestFixtureRoot "Flatten-OpenApiAllOf/source.openapi.json"
        $outputPath = Join-Path $temporaryDirectory "explicit-response-output.json"
        Invoke-OpenApiOperationExtractor `
            -SpecificationPath $fixturePath `
            -OperationId "createThing" `
            -Mode Responses `
            -OutputPath $outputPath
        $written = Get-Content -LiteralPath $outputPath -Raw -Encoding utf8 |
            ConvertFrom-Json -AsHashtable -Depth 100
        Assert-True ([System.IO.File]::Exists($outputPath)) "Explicit output path was not created."
        Assert-True (-not (Test-JsonProperty -Object $written.paths["/things"].post -Name "requestBody")) "Explicit Responses invocation retained requestBody."
        Assert-True (Test-JsonProperty -Object $written.paths["/things"].post -Name "responses") "Explicit Responses invocation lost responses."
    }

    # Deterministic encoding and serialization keep generated artifacts
    # reviewable and prevent platform-specific byte churn.
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