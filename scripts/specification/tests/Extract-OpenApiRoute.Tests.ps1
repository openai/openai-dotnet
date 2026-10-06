<#
.SYNOPSIS
    Runs focused tests for scripts/specification/pipeline/Extract-OpenApiRoute.ps1.

.DESCRIPTION
    Uses a small built-in assertion runner so the repository does not need an
    additional PowerShell test dependency. Behavior is exercised against a
    compact fixture so this focused suite remains fast and deterministic.
#>

[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$scriptPath = Join-Path $PSScriptRoot ".." "pipeline" "Extract-OpenApiRoute.ps1"
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
    $extracted = New-ExtractedOpenApiDocument `
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
        $defaultJson = ConvertTo-Json -InputObject $extracted.Document -Compress -Depth 100
        $explicitJson = ConvertTo-Json -InputObject $explicitBoth.Document -Compress -Depth 100
        Assert-Equal $defaultJson $explicitJson "Explicit Both changed the extracted document."
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
        Assert-Sequence @("/things") @((Get-JsonProperties -Value $extracted.Document.paths).Name) "Wrong paths were emitted."
        Assert-Sequence `
            @("parameters", "post") `
            @((Get-JsonProperties -Value $extracted.Document.paths["/things"]).Name) `
            "Path-item order changed."
        Assert-True (Test-JsonProperty -Object $extracted.Document.paths["/things"] -Name "parameters") "Path-level parameters were dropped."
        Assert-True (-not (Test-JsonProperty -Object $extracted.Document.paths["/things"] -Name "get")) "Unselected GET operation remained."
        Assert-Equal "createThing" $extracted.Document.paths["/things"].post.operationId "Selected operationId changed."
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
            @("operationId", "tags", "security", "example", "examples", "requestBody", "responses", "x-oaiMeta", "x-hidden-ref") `
            @((Get-JsonProperties -Value $operation).Name) `
            "RequestBody mode changed surviving operation order."
    }

    # Responses mode removes only the direct operation requestBody while path
    # and operation metadata and the self-contained component registry remain.
    Invoke-Test "Responses mode keeps metadata and excludes request body" {
        $pathItem = $responsesOnly.Document.paths["/things"]
        $operation = $pathItem.post
        Assert-True (Test-JsonProperty -Object $pathItem -Name "parameters") "Path-level parameters were removed."
        Assert-True (-not (Test-JsonProperty -Object $operation -Name "requestBody")) "Request body remained."
        Assert-True (Test-JsonProperty -Object $operation -Name "responses") "Responses were removed."
        Assert-Sequence @("Things") @($operation.tags) "Operation tags changed."
        Assert-True (Test-JsonProperty -Object $operation -Name "security") "Operation security was removed."
        Assert-Equal $fixture.components.schemas.Count $responsesOnly.Document.components.schemas.Count "Extraction did not retain the complete schema registry."
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

    # Top-level tag metadata is minimized to definitions referenced by the
    # selected operation.
    Invoke-Test "extraction keeps only selected operation tag definitions" {
        Assert-Sequence @("Things") @($extracted.Document.tags.name) "Tag minimization was incorrect."
    }

    # Extract preserves source encounter order; canonical ordering belongs to Sort.
    Invoke-Test "Extract preserves schema encounter order" {
        $properties = $extracted.Document.paths["/things"].post.requestBody.content["application/json"].schema.properties
        Assert-Sequence `
            @("optionalZ", "requiredB", "recursive", "requiredA", "flattened", "type") `
            @((Get-JsonProperties -Value $properties).Name) `
            "Extracted schema property encounter order changed."
        Assert-Sequence `
            @("requiredB", "requiredA", "type") `
            @($extracted.Document.paths["/things"].post.requestBody.content["application/json"].schema.required) `
            "Extracted required encounter order changed."
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
            @((Get-JsonProperties -Value $extracted.Document.paths["/things"].post.requestBody.content["application/json"].schema).Name) `
            "Extracted schema-key encounter order changed."
    }

    # OperationIds become filenames across platforms; sanitization collisions
    # need a stable suffix rather than accidental overwrites.
    Invoke-Test "default filenames are portable and collision-aware" {
        $ordinaryPath = Get-DefaultOutputPath `
            -OperationId "createThing" `
            -Catalog $catalog `
            -RepositoryRoot $temporaryDirectory
        Assert-Equal "createThing.extracted.openapi.json" (Split-Path $ordinaryPath -Leaf) "Ordinary operationId filename was incorrect."

        $requestPath = Get-DefaultOutputPath `
            -OperationId "createThing" `
            -Catalog $catalog `
            -RepositoryRoot $temporaryDirectory `
            -Mode RequestBody
        Assert-Equal "createThing.request-body.extracted.openapi.json" (Split-Path $requestPath -Leaf) "RequestBody filename was incorrect."

        $responsePath = Get-DefaultOutputPath `
            -OperationId "createThing" `
            -Catalog $catalog `
            -RepositoryRoot $temporaryDirectory `
            -Mode Responses
        Assert-Equal "createThing.responses.extracted.openapi.json" (Split-Path $responsePath -Leaf) "Responses filename was incorrect."

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
        Assert-True ($name -match '^foo-bar-[0-9a-f]{8}\.responses\.extracted\.openapi\.json$') "Collision-safe filename was unexpected: $name"
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
        $null = Write-OpenApiJsonFile -Document $extracted.Document -Path $firstPath
        $null = Write-OpenApiJsonFile -Document $extracted.Document -Path $secondPath
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