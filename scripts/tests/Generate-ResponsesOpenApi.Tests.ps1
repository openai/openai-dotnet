<#
.SYNOPSIS
    Runs focused tests for scripts/Generate-ResponsesOpenApi.ps1.
#>

[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$scriptPath = Join-Path $PSScriptRoot ".." "Generate-ResponsesOpenApi.ps1"
. $scriptPath
. (Join-Path $PSScriptRoot "_OpenApiTestFixture.Helpers.ps1")
. (Join-Path $PSScriptRoot ".." "_OpenApiDocument.Helpers.ps1")
. (Join-Path $PSScriptRoot ".." "_OpenApiComponentDeduplication.Helpers.ps1")

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

function Write-TestSpecification {
    param([string]$Path)
    Copy-Item `
        -LiteralPath (Join-Path $script:OpenApiTestFixtureRoot "Generate-ResponsesOpenApi/source.openapi.json") `
        -Destination $Path
}

function Read-TestJson {
    param([string]$Path)
    return Get-Content -LiteralPath $Path -Raw -Encoding utf8 | ConvertFrom-Json -Depth 100
}

function Write-TestJson {
    param([object]$Document, [string]$Path)
    $json = ConvertTo-Json -InputObject $Document -Depth 100
    [System.IO.File]::WriteAllText(
        $Path,
        "$json`n",
        [System.Text.UTF8Encoding]::new($false)
    )
}

function Write-DuplicateTestSpecification {
    param([string]$Path)
    $document = Read-TestJsonFixture -RelativePath "Generate-ResponsesOpenApi/source.openapi.json"
    $properties = $document.paths["/responses"].post.requestBody.content["application/json"].schema.allOf[0].properties
    foreach ($name in @("duplicateA", "duplicateB")) {
        $properties[$name] = [ordered]@{
            type = "object"
            required = @("value")
            properties = [ordered]@{
                value = [ordered]@{ type = "string" }
            }
        }
    }
    Write-TestJson -Document $document -Path $Path
}

$temporaryDirectory = Join-Path ([System.IO.Path]::GetTempPath()) "generate-responses-openapi-$([Guid]::NewGuid().ToString('N'))"
$null = [System.IO.Directory]::CreateDirectory($temporaryDirectory)

try {
    # Tests and tooling need orchestration helpers without paying for generation
    # as a side effect of loading the script.
    Invoke-Test "dot-sourcing defines orchestration functions without running the pipeline" {
        Assert-True ($null -ne (Get-Command Invoke-ResponsesOpenApiGeneration -ErrorAction SilentlyContinue)) "Generation function was not defined."
        Assert-True ($null -ne (Get-Command Get-ResponsesOpenApiArtifactPaths -ErrorAction SilentlyContinue)) "Artifact path function was not defined."
        Assert-True ($null -ne (Get-Command Reset-ResponsesOpenApiDeduplicationMap -ErrorAction SilentlyContinue)) "Map reset function was not defined."
        Assert-True (-not [System.IO.Directory]::Exists((Join-Path $temporaryDirectory "unexpected"))) "Dot-sourcing unexpectedly generated output."
    }

    # The parameterless production command must read and write the repository's
    # canonical specification/openapi location.
    Invoke-Test "production defaults target specification openapi" {
        $expectedDirectory = Join-Path $script:RepositoryRoot "specification" "openapi"
        Assert-Equal ([System.IO.Path]::GetFullPath($expectedDirectory)) ([System.IO.Path]::GetFullPath($script:DefaultOutputDirectory)) "Default output directory was incorrect."
        Assert-Equal ([System.IO.Path]::GetFullPath((Join-Path $expectedDirectory "openapi.json"))) ([System.IO.Path]::GetFullPath($script:DefaultSpecificationPath)) "Default source path was incorrect."
    }

    # Orchestration invokes stages by filename, so a rename/removal should fail
    # this focused contract test rather than surface during generation.
    Invoke-Test "required stage scripts exist at the expected paths" {
        foreach ($stageScriptName in @(
            "Extract-OpenApiRoute.ps1",
            "Remove-OpenApiOmittedProperties.ps1",
            "Inline-OpenApiReferences.ps1",
            "Flatten-OpenApiAllOf.ps1",
            "Simplify-OpenApiStringVariants.ps1",
            "Simplify-OpenApiRecursiveSchemas.ps1",
            "Add-OpenApiOneOfDiscriminators.ps1",
            "Normalize-OpenApiOneOf.ps1",
            "Flatten-OpenApiOneOf.ps1",
            "Sort-OpenApiDocument.ps1",
            "Componentize-OpenApiObjectSchemas.ps1",
            "Update-OpenApiComponentDeduplicationMap.ps1",
            "Deduplicate-OpenApiComponents.ps1"
        )) {
            $stageScriptPath = Join-Path (Split-Path $scriptPath -Parent) $stageScriptName
            Assert-True (Test-Path -LiteralPath $stageScriptPath -PathType Leaf) "Required stage script was not found: $stageScriptPath"
        }
    }

    # Intermediate filenames are an audit trail of the pipeline, while the final
    # consumer-facing artifact intentionally has the short stable name.
    Invoke-Test "artifact paths retain every stage and use the short final name" {
        $outputDirectory = Join-Path $temporaryDirectory "artifact-paths"
        $paths = Get-ResponsesOpenApiArtifactPaths -OutputDirectory $outputDirectory
        Assert-Sequence @(
            "responses.extracted.openapi.json",
            "responses.extracted.omitted.openapi.json",
            "responses.inlined.openapi.json",
            "responses.inlined.flattened.openapi.json",
            "responses.inlined.flattened.simplified.openapi.json",
            "responses.inlined.flattened.simplified.recursive-simplified.openapi.json",
            "responses.inlined.flattened.simplified.recursive-simplified.oneof-discriminated.openapi.json",
            "responses.inlined.flattened.simplified.recursive-simplified.oneof-discriminated.oneof-normalized.openapi.json",
            "responses.inlined.flattened.simplified.recursive-simplified.oneof-discriminated.oneof-normalized.oneof-flattened.openapi.json",
            "responses.inlined.flattened.simplified.recursive-simplified.oneof-discriminated.oneof-normalized.oneof-flattened.sorted.openapi.json",
            "responses.componentized.openapi.json",
            "responses.component-deduplication-map.json",
            "responses.componentized.deduplicated.openapi.json",
            "responses.openapi.json"
        ) @(
            [System.IO.Path]::GetFileName($paths.Extracted),
            [System.IO.Path]::GetFileName($paths.Omitted),
            [System.IO.Path]::GetFileName($paths.Inlined),
            [System.IO.Path]::GetFileName($paths.Flattened),
            [System.IO.Path]::GetFileName($paths.Simplified),
            [System.IO.Path]::GetFileName($paths.RecursiveSimplified),
            [System.IO.Path]::GetFileName($paths.OneOfDiscriminated),
            [System.IO.Path]::GetFileName($paths.OneOfNormalized),
            [System.IO.Path]::GetFileName($paths.OneOfFlattened),
            [System.IO.Path]::GetFileName($paths.Sorted),
            [System.IO.Path]::GetFileName($paths.Componentized),
            [System.IO.Path]::GetFileName($paths.DeduplicationMap),
            [System.IO.Path]::GetFileName($paths.Deduplicated),
            [System.IO.Path]::GetFileName($paths.Final)
        ) "Artifact names were incorrect."
        foreach ($path in @(
            $paths.Componentized,
            $paths.DeduplicationMap,
            $paths.Deduplicated
        )) {
            Assert-Equal `
                ([System.IO.Path]::GetFullPath($outputDirectory)) `
                ([System.IO.Path]::GetDirectoryName($path)) `
                "Post-processing artifact was not rooted in the requested output directory."
        }
    }

    # One compact end-to-end test owns stage ordering, intermediate persistence,
    # semantic handoffs, source immutability, and final alias identity.
    Invoke-Test "pipeline persists every stage and publishes the deduplicated final alias" {
        $sourcePath = Join-Path $temporaryDirectory "openapi.json"
        $outputDirectory = Join-Path $temporaryDirectory "generated"
        Write-TestSpecification -Path $sourcePath
        [byte[]]$sourceBefore = [System.IO.File]::ReadAllBytes($sourcePath)
        $obsoleteSortedPath = Join-Path $outputDirectory "responses.inlined.flattened.simplified.recursive-simplified.oneof-discriminated.oneof-normalized.sorted.openapi.json"
        $null = [System.IO.Directory]::CreateDirectory($outputDirectory)
        [System.IO.File]::WriteAllText($obsoleteSortedPath, "stale", [System.Text.UTF8Encoding]::new($false))
        $paths = Invoke-ResponsesOpenApiGeneration `
            -SpecificationPath $sourcePath `
            -OutputDirectory $outputDirectory

        foreach ($artifactPath in @(
            $paths.Extracted,
            $paths.Omitted,
            $paths.Inlined,
            $paths.Flattened,
            $paths.Simplified,
            $paths.RecursiveSimplified,
            $paths.OneOfDiscriminated,
            $paths.OneOfNormalized,
            $paths.OneOfFlattened,
            $paths.Sorted,
            $paths.Componentized,
            $paths.DeduplicationMap,
            $paths.Deduplicated,
            $paths.Final
        )) {
            Assert-True ([System.IO.File]::Exists($artifactPath)) "Artifact was not created: $artifactPath"
        }
        Assert-True ([System.Linq.Enumerable]::SequenceEqual($sourceBefore, [System.IO.File]::ReadAllBytes($sourcePath))) "Pipeline modified its source specification."
        Assert-True ([System.Linq.Enumerable]::SequenceEqual([System.IO.File]::ReadAllBytes($paths.Deduplicated), [System.IO.File]::ReadAllBytes($paths.Final))) "Final output differs from the retained deduplicated artifact."
        Assert-True (-not [System.IO.File]::Exists($obsoleteSortedPath)) "Pipeline retained the obsolete sorted artifact."

        $extracted = Read-TestJson -Path $paths.Extracted
        $omitted = Read-TestJson -Path $paths.Omitted
        $inlined = Read-TestJson -Path $paths.Inlined
        $flattened = Read-TestJson -Path $paths.Flattened
        $simplified = Read-TestJson -Path $paths.Simplified
        $recursiveSimplified = Read-TestJson -Path $paths.RecursiveSimplified
        $oneOfDiscriminated = Read-TestJson -Path $paths.OneOfDiscriminated
        $oneOfNormalized = Read-TestJson -Path $paths.OneOfNormalized
        $oneOfFlattened = Read-TestJson -Path $paths.OneOfFlattened
        $sorted = Read-TestJson -Path $paths.Sorted
        $componentized = Read-OpenApiDocument -Path $paths.Componentized
        $mapping = Read-TestJson -Path $paths.DeduplicationMap
        $deduplicated = Read-OpenApiDocument -Path $paths.Deduplicated
        Assert-Sequence @("/responses") @($extracted.paths.PSObject.Properties.Name) "Extract stage retained unrelated routes."
        Assert-Equal "Discard API description" $extracted.info.description "Extract stage removed metadata."
        $extractedModel = $extracted.paths.'/responses'.post.requestBody.content.'application/json'.schema.allOf[0].properties.model
        Assert-Equal "Discard model title" $extractedModel.title "Extract stage removed schema title metadata."
        Assert-Equal "discard-model-default" $extractedModel.default "Extract stage removed schema default metadata."
        Assert-True (-not ($omitted.info.PSObject.Properties.Name -ccontains "description")) "Omission stage retained info.description."
        $omittedModel = $omitted.paths.'/responses'.post.requestBody.content.'application/json'.schema.allOf[0].properties.model
        Assert-True (-not ($omittedModel.PSObject.Properties.Name -ccontains "title")) "Omission stage retained schema title metadata."
        Assert-True (-not ($omittedModel.PSObject.Properties.Name -ccontains "description")) "Omission stage retained schema description metadata."
        Assert-True (-not ($omittedModel.PSObject.Properties.Name -ccontains "default")) "Omission stage retained schema default metadata."
        Assert-Equal `
            (ConvertTo-Json -InputObject $omitted -Compress -Depth 100) `
            (ConvertTo-Json -InputObject $inlined -Compress -Depth 100) `
            "Reference-free compact fixture changed during inlining."
        Assert-Sequence @("/responses") @($inlined.paths.PSObject.Properties.Name) "Inline stage retained unrelated routes."
        Assert-Equal "Responses pipeline fixture" $inlined.info.title "Extract stage removed required info.title."
        Assert-True (-not ($inlined.info.PSObject.Properties.Name -ccontains "description")) "Extract stage retained info.description."
        Assert-Sequence @("default") @($inlined.paths.'/responses'.post.responses.PSObject.Properties.Name) "Extract stage lost the RequestBody response placeholder."
        Assert-Equal "Responses omitted by RequestBody extraction mode." $inlined.paths.'/responses'.post.responses.default.description "Extract stage changed the required placeholder description."
        $inlinedSchema = $inlined.paths.'/responses'.post.requestBody.content.'application/json'.schema
        $flattenedSchema = $flattened.paths.'/responses'.post.requestBody.content.'application/json'.schema
        Assert-True ($inlinedSchema.PSObject.Properties.Name -ccontains "allOf") "Extract stage did not retain allOf."
        $inlinedModel = $inlinedSchema.allOf[0].properties.model
        Assert-True (-not ($inlinedModel.PSObject.Properties.Name -ccontains "title")) "Extract stage retained schema title metadata."
        Assert-True (-not ($inlinedModel.PSObject.Properties.Name -ccontains "description")) "Extract stage retained schema description metadata."
        Assert-True (-not ($inlinedModel.PSObject.Properties.Name -ccontains "default")) "Extract stage retained schema default metadata."
        Assert-True (-not ($flattenedSchema.PSObject.Properties.Name -ccontains "allOf")) "Flatten stage retained allOf."

        $simplifiedModel = $simplified.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.model
        $simplifiedOutputs = $simplified.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.outputs
        $simplifiedEvents = $simplified.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.events
        $simplifiedFilters = $simplified.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.filters
        $recursiveSimplifiedFilters = $recursiveSimplified.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.filters
        $recursiveSimplifiedFormat = $recursiveSimplified.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.format
        $recursiveSimplifiedGrouped = $recursiveSimplified.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.grouped
        $discriminatedFormat = $oneOfDiscriminated.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.format
        $discriminatedGrouped = $oneOfDiscriminated.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.grouped
        $normalizedOutputs = $oneOfNormalized.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.outputs
        $normalizedEvents = $oneOfNormalized.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.events
        $normalizedFilters = $oneOfNormalized.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.filters
        $normalizedFormat = $oneOfNormalized.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.format
        $normalizedGrouped = $oneOfNormalized.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.grouped
        $normalizedNested = $oneOfNormalized.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.nested
        $flattenedGrouped = $oneOfFlattened.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.grouped
        $flattenedNested = $oneOfFlattened.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.nested
        $sortedModel = $sorted.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.model
        $sortedEvents = $sorted.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.events
        $sortedFilters = $sorted.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.filters
        $sortedGrouped = $sorted.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.grouped
        $sortedNested = $sorted.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.nested
        Assert-Equal 2 $simplifiedModel.anyOf.Count "Simplify stage emitted the wrong branch count."
        Assert-Sequence @("zeta", "alpha", "middle") @($simplifiedModel.anyOf[1].enum) "Simplify stage did not preserve encounter order."
        Assert-True ($simplifiedOutputs.PSObject.Properties.Name -ccontains "discriminator") "Simplify stage removed the orphan before Normalize."
        Assert-True ($simplifiedEvents.PSObject.Properties.Name -ccontains "anyOf") "Simplify stage promoted the union before Normalize."
        Assert-True ($simplifiedFilters.anyOf[0].anyOf[1].PSObject.Properties.Name -ccontains '$recursiveAnchor') "String Simplify stage removed the recursive anchor."
        Assert-Equal 2 $recursiveSimplifiedFilters.anyOf.Count "Recursive Simplify stage changed the nullable wrapper."
        Assert-Sequence @("type", "additionalProperties") @($recursiveSimplifiedFilters.anyOf[0].PSObject.Properties.Name) "Recursive Simplify stage emitted the wrong free-form shape."
        Assert-Equal $true $recursiveSimplifiedFilters.anyOf[0].additionalProperties "Recursive Simplify stage did not allow free-form properties."
        Assert-Equal "null" $recursiveSimplifiedFilters.anyOf[1].type "Recursive Simplify stage lost the nullable branch."
        Assert-True (-not ($recursiveSimplifiedFormat.PSObject.Properties.Name -ccontains "discriminator")) "Recursive Simplify stage added the discriminator early."
        Assert-True (-not ($recursiveSimplifiedGrouped.PSObject.Properties.Name -ccontains "discriminator")) "Recursive Simplify stage added the grouped discriminator early."
        Assert-Sequence @("discriminator", "oneOf") @($discriminatedFormat.PSObject.Properties.Name) "oneOf discriminator stage emitted the wrong member order."
        Assert-Equal "type" $discriminatedFormat.discriminator.propertyName "oneOf discriminator stage used the wrong propertyName."
        Assert-Sequence @("text", "json_schema", "json_object") @($discriminatedFormat.oneOf | ForEach-Object { $_.properties.type.enum[0] }) "oneOf discriminator stage changed branch values."
        Assert-Equal "type" $discriminatedGrouped.discriminator.propertyName "Grouped parent discriminator was not inferred."
        Assert-Equal 2 $discriminatedGrouped.oneOf.Count "Discriminator stage flattened grouped wrappers early."
        Assert-Equal 3 $discriminatedGrouped.oneOf[0].oneOf.Count "Discriminator stage changed the grouped input wrapper."
        Assert-Equal 2 $discriminatedGrouped.oneOf[1].oneOf.Count "Discriminator stage changed the grouped output wrapper."
        Assert-True (-not ($normalizedOutputs.PSObject.Properties.Name -ccontains "discriminator")) "Normalize stage retained the outer orphan."
        Assert-True (-not ($normalizedOutputs.items.PSObject.Properties.Name -ccontains "discriminator")) "Invalid nested discriminator remained."
        Assert-True ($normalizedEvents.PSObject.Properties.Name -ccontains "anyOf") "Normalize stage changed a non-inferable anyOf."
        Assert-True (-not ($normalizedEvents.PSObject.Properties.Name -ccontains "oneOf")) "Normalize stage promoted a non-inferable anyOf."
        Assert-True (-not ($normalizedEvents.PSObject.Properties.Name -ccontains "discriminator")) "Normalize stage retained a non-inferable discriminator."
        Assert-True ($normalizedFilters.PSObject.Properties.Name -ccontains "anyOf") "Normalize stage changed the nullable recursive wrapper."
        Assert-Equal "type" $normalizedFormat.discriminator.propertyName "Normalize stage removed the inferred discriminator."
        Assert-Equal "type" $normalizedGrouped.discriminator.propertyName "Normalize stage removed the grouped discriminator."
        Assert-Equal 2 $normalizedGrouped.oneOf.Count "Normalize stage flattened grouped wrappers early."
        Assert-Equal 3 $normalizedNested.oneOf.Count "Normalize stage changed the nested outer union."
        Assert-Equal 2 $normalizedNested.oneOf[1].oneOf.Count "Normalize stage removed the matching wrapper early."
        Assert-True (-not ($normalizedNested.PSObject.Properties.Name -ccontains "discriminator")) "Invalid nested parent discriminator remained."
        Assert-Equal "type" $flattenedGrouped.discriminator.propertyName "oneOf Flatten stage removed the grouped discriminator."
        Assert-Sequence @("input_text", "input_image", "input_file", "output_text", "refusal") @(
            $flattenedGrouped.oneOf | ForEach-Object { $_.properties.type.enum[0] }
        ) "oneOf Flatten stage changed grouped leaf order."
        Assert-Equal 0 @($flattenedGrouped.oneOf | Where-Object {
            $_.PSObject.Properties.Name -ccontains "oneOf"
        }).Count "oneOf Flatten stage retained a grouped wrapper."
        Assert-Equal 3 $flattenedNested.oneOf.Count "oneOf Flatten stage changed a non-discriminated union."
        Assert-Equal 2 $flattenedNested.oneOf[1].oneOf.Count "oneOf Flatten stage flattened a non-discriminated wrapper."
        Assert-Sequence @("shared") @($flattenedNested.required) "oneOf Flatten stage changed shared constraints."
        Assert-True ($flattenedNested.oneOf[1].PSObject.Properties.Name -ccontains "oneOf") "oneOf Flatten stage lost the retained wrapper."
        Assert-True ($sortedEvents.PSObject.Properties.Name -ccontains "anyOf") "Sort stage lost the retained anyOf."
        Assert-Equal "type" $sortedGrouped.discriminator.propertyName "Sort stage lost the grouped discriminator."
        Assert-Equal 5 $sortedGrouped.oneOf.Count "Sort stage changed grouped branches."
        Assert-Equal 3 $sortedNested.oneOf.Count "Sort stage changed retained oneOf branches."
        Assert-Sequence @("alpha", "middle", "zeta") @($sortedModel.anyOf[1].enum) "Sort stage did not canonicalize the enum."
        Assert-Equal $true $sortedFilters.anyOf[0].additionalProperties "Sort stage lost the free-form recursive replacement."

        Assert-Equal 1 $mapping.version "Deduplication map version was incorrect."
        Assert-True ($mapping.PSObject.Properties.Name -ccontains "groups") "Deduplication map omitted groups."
        Assert-OpenApiReferencesResolve -Document $componentized
        Assert-AdditionalOpenApiReferencesResolve -Document $componentized
        $null = Assert-OpenApiDiscriminatorMappings -Document $componentized
        Assert-OpenApiReferencesResolve -Document $deduplicated
        Assert-AdditionalOpenApiReferencesResolve -Document $deduplicated
        $null = Assert-OpenApiDiscriminatorMappings -Document $deduplicated
        $componentizedCount = @(Get-JsonProperties -Value (Get-OpenApiComponentSchemas -Document $componentized)).Count
        $deduplicatedCount = @(Get-JsonProperties -Value (Get-OpenApiComponentSchemas -Document $deduplicated)).Count
        Assert-True ($deduplicatedCount -le $componentizedCount) "Deduplication increased the component count."
    }

    Invoke-Test "map reset deletes only files" {
        $mapPath = Join-Path $temporaryDirectory "reset-map.json"
        [System.IO.File]::WriteAllText($mapPath, "map", [System.Text.UTF8Encoding]::new($false))
        Reset-ResponsesOpenApiDeduplicationMap -Path $mapPath
        Assert-True (-not [System.IO.File]::Exists($mapPath)) "Map reset did not delete the file."

        $directoryPath = Join-Path $temporaryDirectory "map-directory"
        $null = [System.IO.Directory]::CreateDirectory($directoryPath)
        Assert-Throws {
            Reset-ResponsesOpenApiDeduplicationMap -Path $directoryPath
        } "*path is not a file*"
        Assert-True ([System.IO.Directory]::Exists($directoryPath)) "Map reset removed a directory."
    }

    Invoke-Test "reviewed mapping is preserved unless reset is requested" {
        $sourcePath = Join-Path $temporaryDirectory "duplicates.openapi.json"
        $outputDirectory = Join-Path $temporaryDirectory "mapping-policy"
        Write-DuplicateTestSpecification -Path $sourcePath

        $paths = Invoke-ResponsesOpenApiGeneration `
            -SpecificationPath $sourcePath `
            -OutputDirectory $outputDirectory `
            -ResetDeduplicationMap `
            6>$null
        $mapping = Read-TestJson -Path $paths.DeduplicationMap
        $group = @($mapping.groups | Where-Object {
            @($_.members) -ccontains "CreateResponse_Request_DuplicateA"
        })[0]
        Assert-True ($null -ne $group) "Duplicate fixture did not create a mapping group."
        $defaultCanonicalName = $group.canonicalName
        $group.canonicalName = "ReviewedDuplicate"
        Write-TestJson -Document $mapping -Path $paths.DeduplicationMap
        [byte[]]$reviewedMappingBytes = [System.IO.File]::ReadAllBytes($paths.DeduplicationMap)

        $ordinaryPaths = Invoke-ResponsesOpenApiGeneration `
            -SpecificationPath $sourcePath `
            -OutputDirectory $outputDirectory `
            6>$null
        Assert-BytesEqual `
            $reviewedMappingBytes `
            ([System.IO.File]::ReadAllBytes($ordinaryPaths.DeduplicationMap)) `
            "Ordinary generation rewrote reviewed mapping bytes."
        $preservedMapping = Read-TestJson -Path $ordinaryPaths.DeduplicationMap
        $preservedGroup = @($preservedMapping.groups | Where-Object {
            @($_.members) -ccontains "CreateResponse_Request_DuplicateA"
        })[0]
        Assert-Equal "ReviewedDuplicate" $preservedGroup.canonicalName "Ordinary generation discarded the reviewed canonical name."
        $preservedDocument = Read-TestJson -Path $ordinaryPaths.Deduplicated
        Assert-Equal `
            "#/components/schemas/ReviewedDuplicate" `
            $preservedDocument.components.schemas.CreateResponse_Request.properties.duplicateA.'$ref' `
            "Deduplicated output did not apply the reviewed canonical name."

        $resetPaths = Invoke-ResponsesOpenApiGeneration `
            -SpecificationPath $sourcePath `
            -OutputDirectory $outputDirectory `
            -ResetDeduplicationMap `
            6>$null
        $resetMapping = Read-TestJson -Path $resetPaths.DeduplicationMap
        $resetGroup = @($resetMapping.groups | Where-Object {
            @($_.members) -ccontains "CreateResponse_Request_DuplicateA"
        })[0]
        Assert-Equal $defaultCanonicalName $resetGroup.canonicalName "Reset did not restore the first-encounter canonical name."
        $resetDocument = Read-TestJson -Path $resetPaths.Deduplicated
        Assert-Equal `
            "#/components/schemas/$defaultCanonicalName" `
            $resetDocument.components.schemas.CreateResponse_Request.properties.duplicateA.'$ref' `
            "Reset deduplicated output did not apply the default canonical name."
        [byte[]]$resetMappingBytes = [System.IO.File]::ReadAllBytes($resetPaths.DeduplicationMap)
        [byte[]]$resetDeduplicatedBytes = [System.IO.File]::ReadAllBytes($resetPaths.Deduplicated)

        $secondResetPaths = Invoke-ResponsesOpenApiGeneration `
            -SpecificationPath $sourcePath `
            -OutputDirectory $outputDirectory `
            -ResetDeduplicationMap `
            6>$null
        Assert-BytesEqual $resetMappingBytes ([System.IO.File]::ReadAllBytes($secondResetPaths.DeduplicationMap)) "Repeated reset changed default mapping bytes."
        Assert-BytesEqual $resetDeduplicatedBytes ([System.IO.File]::ReadAllBytes($secondResetPaths.Deduplicated)) "Repeated reset changed deduplicated bytes."

        $staleMapping = Read-TestJson -Path $secondResetPaths.DeduplicationMap
        $staleGroup = @($staleMapping.groups | Where-Object {
            @($_.members) -ccontains "CreateResponse_Request_DuplicateA"
        })[0]
        $staleGroup.members[1] = "MissingReviewedComponent"
        Write-TestJson -Document $staleMapping -Path $secondResetPaths.DeduplicationMap
        [byte[]]$staleBytes = [System.IO.File]::ReadAllBytes($secondResetPaths.DeduplicationMap)
        Assert-Throws {
            Invoke-ResponsesOpenApiGeneration `
                -SpecificationPath $sourcePath `
                -OutputDirectory $outputDirectory `
                6>$null
        } "*missing from the componentized source*"
        Assert-BytesEqual $staleBytes ([System.IO.File]::ReadAllBytes($secondResetPaths.DeduplicationMap)) "Failed ordinary generation changed the stale map."
    }

    # Final publication must replace stale bytes atomically without requiring a
    # second expensive pipeline run in this test.
    Invoke-Test "atomic copy replaces a stale final alias" {
        $sourcePath = Join-Path $temporaryDirectory "copy-source.json"
        $finalPath = Join-Path $temporaryDirectory "responses.openapi.json"
        [System.IO.File]::WriteAllText($sourcePath, "fresh", [System.Text.UTF8Encoding]::new($false))
        [System.IO.File]::WriteAllText($finalPath, "stale", [System.Text.UTF8Encoding]::new($false))
        $writtenPath = Copy-OpenApiFileAtomically `
            -SourcePath $sourcePath `
            -DestinationPath $finalPath
        Assert-Equal ([System.IO.Path]::GetFullPath($finalPath)) $writtenPath "Atomic copy returned the wrong path."
        Assert-True ([System.Linq.Enumerable]::SequenceEqual([System.IO.File]::ReadAllBytes($sourcePath), [System.IO.File]::ReadAllBytes($finalPath))) "Stale final alias was not atomically replaced."
    }

    # Orchestration should validate its source before launching any stage or
    # creating a partial artifact chain.
    Invoke-Test "missing and non-file specifications fail clearly" {
        $guardOutputDirectory = Join-Path $temporaryDirectory "reset-guard"
        $guardPaths = Get-ResponsesOpenApiArtifactPaths -OutputDirectory $guardOutputDirectory
        $null = [System.IO.Directory]::CreateDirectory($guardOutputDirectory)
        [System.IO.File]::WriteAllText($guardPaths.DeduplicationMap, "preserve", [System.Text.UTF8Encoding]::new($false))
        [byte[]]$guardBytes = [System.IO.File]::ReadAllBytes($guardPaths.DeduplicationMap)
        Assert-Throws {
            Invoke-ResponsesOpenApiGeneration `
                -SpecificationPath (Join-Path $temporaryDirectory "missing.json") `
                -OutputDirectory $guardOutputDirectory `
                -ResetDeduplicationMap
        } "*cannot find path*"
        Assert-BytesEqual $guardBytes ([System.IO.File]::ReadAllBytes($guardPaths.DeduplicationMap)) "Early failure deleted the existing map."
        Assert-Throws {
            Invoke-ResponsesOpenApiGeneration `
                -SpecificationPath $temporaryDirectory `
                -OutputDirectory (Join-Path $temporaryDirectory "directory-output")
        } "*is not a file*"
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