<#
.SYNOPSIS
    Runs focused tests for scripts/specification/Generate-OpenApi.ps1.
#>

[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$scriptPath = Join-Path $PSScriptRoot ".." "Generate-OpenApi.ps1"
. $scriptPath
. (Join-Path $PSScriptRoot "_OpenApiTestFixture.Helpers.ps1")
. (Join-Path $PSScriptRoot ".." "pipeline" "_OpenApiDocument.Helpers.ps1")

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
    $document = Read-TestJsonFixture -RelativePath "Generate-OpenApi/source.openapi.json"
    $properties = $document.paths["/responses"].post.requestBody.content["application/json"].schema.allOf[0].properties
    $properties["splitChoice"] = [ordered]@{
        oneOf = @(
            [ordered]@{ type = "string"; enum = @("auto") },
            [ordered]@{
                type = "object"
                properties = [ordered]@{
                    type = [ordered]@{ type = "string"; enum = @("alpha", "beta") }
                }
            },
            [ordered]@{
                type = "object"
                required = @("type")
                properties = [ordered]@{
                    type = [ordered]@{ type = "string"; enum = @("gamma") }
                }
            }
        )
    }
    $properties["tools"] = [ordered]@{
        type = "array"
        items = [ordered]@{
            discriminator = [ordered]@{ propertyName = "type" }
            oneOf = @(
                [ordered]@{
                    type = "object"
                    required = @("type")
                    properties = [ordered]@{
                        type = [ordered]@{ type = "string"; enum = @("web_search_preview") }
                    }
                },
                [ordered]@{
                    type = "object"
                    required = @("type")
                    properties = [ordered]@{
                        type = [ordered]@{ type = "string"; enum = @("function") }
                    }
                },
                [ordered]@{
                    type = "object"
                    required = @("type")
                    properties = [ordered]@{
                        type = [ordered]@{ type = "string"; enum = @("apply_patch") }
                    }
                }
            )
        }
    }
    Write-TestJson -Document $document -Path $Path
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

$temporaryDirectory = Join-Path ([System.IO.Path]::GetTempPath()) "generate-openapi-$([Guid]::NewGuid().ToString('N'))"
$null = [System.IO.Directory]::CreateDirectory($temporaryDirectory)
$featureArea = "responses"

try {
    # Tests and tooling need orchestration helpers without paying for generation
    # as a side effect of loading the script.
    Invoke-Test "dot-sourcing defines orchestration functions without running the pipeline" {
        Assert-True ($null -ne (Get-Command Invoke-OpenApiGeneration -ErrorAction SilentlyContinue)) "Generation function was not defined."
        Assert-True ($null -ne (Get-Command Get-OpenApiArtifactPaths -ErrorAction SilentlyContinue)) "Artifact path function was not defined."
        Assert-True ($null -ne (Get-Command Publish-OpenApiArtifact -ErrorAction SilentlyContinue)) "Shared publisher was not defined."
        Assert-True (-not (Get-Command Invoke-OpenApiGeneration).Parameters.ContainsKey("ResetDeduplicationMap")) "Generation still exposes map reset policy."
        Assert-True (-not [System.IO.Directory]::Exists((Join-Path $temporaryDirectory "unexpected"))) "Dot-sourcing unexpectedly generated output."
    }

    # The parameterless production command must read and write the repository's
    # canonical specification/openapi location.
    Invoke-Test "production defaults target specification openapi" {
        $expectedDirectory = Join-Path $script:RepositoryRoot "specification" "openapi"
        Assert-Equal ([System.IO.Path]::GetFullPath($expectedDirectory)) ([System.IO.Path]::GetFullPath($script:DefaultOutputRoot)) "Default output root was incorrect."
        Assert-Equal ([System.IO.Path]::GetFullPath((Join-Path $expectedDirectory "openapi.json"))) ([System.IO.Path]::GetFullPath($script:DefaultSpecificationPath)) "Default source path was incorrect."
    }

    Invoke-Test "feature areas are validated and scoped beneath the output root" {
        $outputRoot = Join-Path $temporaryDirectory "feature-paths"
        $paths = Get-OpenApiArtifactPaths -FeatureArea "chat-completions" -OutputRoot $outputRoot
        Assert-Equal "chat-completions" ([System.IO.Path]::GetFileName($paths.OutputDirectory)) "Feature directory was incorrect."
        Assert-Equal "chat-completions.openapi.json" ([System.IO.Path]::GetFileName($paths.Final)) "Feature alias name was incorrect."
        foreach ($invalid in @("Responses", "chat_completions", "../responses", "responses/other", "-responses")) {
            Assert-Throws {
                Get-OpenApiArtifactPaths -FeatureArea $invalid -OutputRoot $outputRoot
            } "*must be lowercase kebab-case*"
        }
    }

    Invoke-Test "CLI requires feature area and operation id" {
        $pwshPath = (Get-Process -Id $PID).Path
        $output = & $pwshPath `
            -NoLogo `
            -NoProfile `
            -NonInteractive `
            -File $scriptPath `
            -OperationId "createResponse" `
            2>&1
        Assert-True ($LASTEXITCODE -ne 0) "Generate CLI accepted a missing feature area."
        Assert-True (($output -join "`n") -like "*FeatureArea is required*") "Missing feature area error was unclear."

        $output = & $pwshPath `
            -NoLogo `
            -NoProfile `
            -NonInteractive `
            -File $scriptPath `
            -FeatureArea $featureArea `
            2>&1
        Assert-True ($LASTEXITCODE -ne 0) "Generate CLI accepted a missing operation id."
        Assert-True (($output -join "`n") -like "*OperationId is required*") "Missing operation id error was unclear."
    }

    Invoke-Test "generation forwards Responses and Both extraction modes" {
        $sourcePath = Join-Path $PSScriptRoot "fixtures" "Generate-OpenApi" "source.openapi.json"
        foreach ($mode in @("Responses", "Both")) {
            $modeFeature = "mode-$($mode.ToLowerInvariant())"
            $modeRoot = Join-Path $temporaryDirectory "mode-forwarding"
            $result = Invoke-OpenApiGeneration `
                -FeatureArea $modeFeature `
                -OperationId "createResponse" `
                -Mode $mode `
                -SpecificationPath $sourcePath `
                -OutputRoot $modeRoot `
                6>$null
            $extracted = Read-TestJson -Path $result.Extracted
            Assert-True ($extracted.paths.'/responses'.post.PSObject.Properties.Name -ccontains "responses") "Mode '$mode' omitted responses."
            Assert-Sequence @("200") @($extracted.paths.'/responses'.post.responses.PSObject.Properties.Name) "Mode '$mode' retained the wrong responses."
            $hasRequestBody = $extracted.paths.'/responses'.post.PSObject.Properties.Name -ccontains "requestBody"
            Assert-Equal ($mode -eq "Both") $hasRequestBody "Mode '$mode' request-body projection was incorrect."
        }
    }

    # Orchestration invokes stages by filename, so a rename/removal should fail
    # this focused contract test rather than surface during generation.
    Invoke-Test "required stage scripts exist at the expected paths" {
        foreach ($stageScriptName in @(
            "Extract-OpenApiRoute.ps1",
            "Remove-OpenApiOmittedProperties.ps1",
            "Inline-OpenApiReferences.ps1",
            "Normalize-OpenApiTypeArrays.ps1",
            "Flatten-OpenApiAllOf.ps1",
            "Simplify-OpenApiStringVariants.ps1",
            "Simplify-OpenApiRecursiveSchemas.ps1",
            "Require-OpenApiTypeProperties.ps1",
            "Split-OpenApiMultiValueTypeEnums.ps1",
            "Add-OpenApiOneOfDiscriminators.ps1",
            "Normalize-OpenApiOneOf.ps1",
            "Flatten-OpenApiOneOf.ps1",
            "Wrap-OpenApiSingletonTypeEnums.ps1",
            "Sort-OpenApiDocument.ps1",
            "Componentize-OpenApiObjectSchemas.ps1"
        )) {
            $stageScriptPath = Join-Path (Split-Path $scriptPath -Parent) "pipeline" $stageScriptName
            Assert-True (Test-Path -LiteralPath $stageScriptPath -PathType Leaf) "Required stage script was not found: $stageScriptPath"
        }
    }

    # Intermediate filenames are an audit trail of the pipeline, while the final
    # consumer-facing artifact intentionally has the short stable name.
    Invoke-Test "artifact paths retain every stage and use the short final name" {
        $outputRoot = Join-Path $temporaryDirectory "artifact-paths"
        $paths = Get-OpenApiArtifactPaths -FeatureArea $featureArea -OutputRoot $outputRoot
        Assert-Sequence @(
            "responses.01.route-extracted.openapi.json",
            "responses.02.properties-omitted.openapi.json",
            "responses.03.references-inlined.openapi.json",
            "responses.04.type-arrays-normalized.openapi.json",
            "responses.05.allof-flattened.openapi.json",
            "responses.06.string-variants-simplified.openapi.json",
            "responses.07.recursive-schemas-simplified.openapi.json",
            "responses.08.type-properties-required.openapi.json",
            "responses.09.multi-value-type-enums-split.openapi.json",
            "responses.10.oneof-discriminators-added.openapi.json",
            "responses.11.oneof-normalized.openapi.json",
            "responses.12.nested-oneof-flattened.openapi.json",
            "responses.13.singleton-type-enums-wrapped.openapi.json",
            "responses.14.document-sorted.openapi.json",
            "responses.15.schemas-componentized.openapi.json",
            "responses.16.discriminator-inheritance-refactored.openapi.json",
            "responses.17.inherited-document-sorted.openapi.json",
            "responses.component-default-map.json",
            "responses.component-policy-map.json",
            "responses.18.components-deduplicated.openapi.json",
            "responses.openapi.json"
        ) @(
            [System.IO.Path]::GetFileName($paths.Extracted),
            [System.IO.Path]::GetFileName($paths.Omitted),
            [System.IO.Path]::GetFileName($paths.Inlined),
            [System.IO.Path]::GetFileName($paths.TypeArraysNormalized),
            [System.IO.Path]::GetFileName($paths.Flattened),
            [System.IO.Path]::GetFileName($paths.Simplified),
            [System.IO.Path]::GetFileName($paths.RecursiveSimplified),
            [System.IO.Path]::GetFileName($paths.TypePropertiesRequired),
            [System.IO.Path]::GetFileName($paths.TypeEnumSplit),
            [System.IO.Path]::GetFileName($paths.OneOfDiscriminated),
            [System.IO.Path]::GetFileName($paths.OneOfNormalized),
            [System.IO.Path]::GetFileName($paths.OneOfFlattened),
            [System.IO.Path]::GetFileName($paths.SingletonTypeWrapped),
            [System.IO.Path]::GetFileName($paths.Sorted),
            [System.IO.Path]::GetFileName($paths.Componentized),
            [System.IO.Path]::GetFileName($paths.DiscriminatorInherited),
            [System.IO.Path]::GetFileName($paths.DiscriminatorInheritedSorted),
            [System.IO.Path]::GetFileName($paths.DefaultComponentMap),
            [System.IO.Path]::GetFileName($paths.PolicyComponentMap),
            [System.IO.Path]::GetFileName($paths.Deduplicated),
            [System.IO.Path]::GetFileName($paths.Final)
        ) "Artifact names were incorrect."
        $numberedPaths = @(
            $paths.Extracted,
            $paths.Omitted,
            $paths.Inlined,
            $paths.TypeArraysNormalized,
            $paths.Flattened,
            $paths.Simplified,
            $paths.RecursiveSimplified,
            $paths.TypePropertiesRequired,
            $paths.TypeEnumSplit,
            $paths.OneOfDiscriminated,
            $paths.OneOfNormalized,
            $paths.OneOfFlattened,
            $paths.SingletonTypeWrapped,
            $paths.Sorted,
            $paths.Componentized,
            $paths.DiscriminatorInherited,
            $paths.DiscriminatorInheritedSorted,
            $paths.Deduplicated
        )
        Assert-Sequence (1..18 | ForEach-Object { $_.ToString("00") }) @(
            $numberedPaths | ForEach-Object {
                $fileName = [System.IO.Path]::GetFileName($_)
                if ($fileName -notmatch '^responses\.(\d{2})\.') {
                    throw "Artifact '$fileName' does not have a two-digit sequence number."
                }
                $Matches[1]
            }
        ) "Artifact sequence numbers were not contiguous."
        foreach ($path in @(
            $paths.Componentized,
            $paths.DiscriminatorInherited,
            $paths.DiscriminatorInheritedSorted,
            $paths.DefaultComponentMap,
            $paths.PolicyComponentMap,
            $paths.Deduplicated
        )) {
            Assert-Equal `
                ([System.IO.Path]::GetFullPath((Join-Path $outputRoot $featureArea))) `
                ([System.IO.Path]::GetDirectoryName($path)) `
                "Post-processing artifact was not rooted in the feature directory."
        }
        Assert-True (-not [System.IO.Directory]::Exists($paths.OutputDirectory)) "Path construction created the feature directory."
    }

    # One compact end-to-end test owns stage ordering, intermediate persistence,
    # semantic handoffs, source immutability, and the componentization boundary.
    Invoke-Test "pipeline persists every stage through componentization and publishes the alias" {
        $sourcePath = Join-Path $temporaryDirectory "openapi.json"
        $outputRoot = Join-Path $temporaryDirectory "generated"
        Write-TestSpecification -Path $sourcePath
        [byte[]]$sourceBefore = [System.IO.File]::ReadAllBytes($sourcePath)
        $expectedPaths = Get-OpenApiArtifactPaths -FeatureArea $featureArea -OutputRoot $outputRoot
        $null = [System.IO.Directory]::CreateDirectory($expectedPaths.OutputDirectory)
        foreach ($downstreamPath in @($expectedPaths.DiscriminatorInherited, $expectedPaths.DiscriminatorInheritedSorted, $expectedPaths.DefaultComponentMap, $expectedPaths.PolicyComponentMap, $expectedPaths.Deduplicated)) {
            [System.IO.File]::WriteAllText($downstreamPath, "downstream-sentinel", [System.Text.UTF8Encoding]::new($false))
        }
        [System.IO.File]::WriteAllText($expectedPaths.Final, "stale-alias", [System.Text.UTF8Encoding]::new($false))
        $pwshPath = (Get-Process -Id $PID).Path
        & $pwshPath `
            -NoLogo `
            -NoProfile `
            -NonInteractive `
            -File $scriptPath `
            -FeatureArea $featureArea `
            -OperationId "createResponse" `
            -SpecificationPath $sourcePath `
            -OutputRoot $outputRoot
        if ($LASTEXITCODE -ne 0) {
            throw "Generate CLI failed with exit code $LASTEXITCODE."
        }
        $paths = Get-OpenApiArtifactPaths -FeatureArea $featureArea -OutputRoot $outputRoot

        foreach ($artifactPath in @(
            $paths.Extracted,
            $paths.Omitted,
            $paths.Inlined,
            $paths.TypeArraysNormalized,
            $paths.Flattened,
            $paths.Simplified,
            $paths.RecursiveSimplified,
            $paths.TypePropertiesRequired,
            $paths.TypeEnumSplit,
            $paths.OneOfDiscriminated,
            $paths.OneOfNormalized,
            $paths.OneOfFlattened,
            $paths.SingletonTypeWrapped,
            $paths.Sorted,
            $paths.Componentized,
            $paths.Final
        )) {
            Assert-True ([System.IO.File]::Exists($artifactPath)) "Artifact was not created: $artifactPath"
        }
        Assert-True ([System.Linq.Enumerable]::SequenceEqual($sourceBefore, [System.IO.File]::ReadAllBytes($sourcePath))) "Pipeline modified its source specification."
        foreach ($downstreamPath in @($paths.DiscriminatorInherited, $paths.DiscriminatorInheritedSorted, $paths.DefaultComponentMap, $paths.PolicyComponentMap, $paths.Deduplicated)) {
            Assert-Equal "downstream-sentinel" ([System.IO.File]::ReadAllText($downstreamPath)) "Generation modified downstream artifact '$downstreamPath'."
        }
        Assert-BytesEqual ([System.IO.File]::ReadAllBytes($paths.Componentized)) ([System.IO.File]::ReadAllBytes($paths.Final)) "Published alias differs from componentized output."
        Assert-True (-not [System.IO.File]::Exists((Join-Path $outputRoot "responses.openapi.json"))) "Generation wrote an artifact directly beneath the output root."

        $extracted = Read-TestJson -Path $paths.Extracted
        $omitted = Read-TestJson -Path $paths.Omitted
        $inlined = Read-TestJson -Path $paths.Inlined
        $typeArraysNormalized = Read-TestJson -Path $paths.TypeArraysNormalized
        $flattened = Read-TestJson -Path $paths.Flattened
        $simplified = Read-TestJson -Path $paths.Simplified
        $recursiveSimplified = Read-TestJson -Path $paths.RecursiveSimplified
        $typePropertiesRequired = Read-TestJson -Path $paths.TypePropertiesRequired
        $typeEnumSplit = Read-TestJson -Path $paths.TypeEnumSplit
        $oneOfDiscriminated = Read-TestJson -Path $paths.OneOfDiscriminated
        $oneOfNormalized = Read-TestJson -Path $paths.OneOfNormalized
        $oneOfFlattened = Read-TestJson -Path $paths.OneOfFlattened
        $singletonTypeWrapped = Read-TestJson -Path $paths.SingletonTypeWrapped
        $sorted = Read-TestJson -Path $paths.Sorted
        $componentized = Read-OpenApiDocument -Path $paths.Componentized
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

        $inlinedOptions = $inlinedSchema.allOf[0].properties.prompt_cache_options
        Assert-Sequence @("object", "null") @($inlinedOptions.type) "Inlining normalized type arrays too early."
        $normalizedProperties = $typeArraysNormalized.paths.'/responses'.post.requestBody.content.'application/json'.schema.allOf[0].properties
        Assert-Sequence @("object", "null") @($normalizedProperties.prompt_cache_options.anyOf.type) "Stage 04 did not normalize the nullable object."
        Assert-Sequence @("explicit", "implicit") @($normalizedProperties.prompt_cache_options.anyOf[0].properties.mode.anyOf[0].enum) "Stage 04 retained null in the non-null enum."
        Assert-Sequence @("integer", "number") @($normalizedProperties.mixed_value.anyOf.type) "Stage 04 changed overlapping type alternatives."
        Assert-NoOpenApiSchemaTypeArrays -Value $typeArraysNormalized

        $simplifiedModel = $simplified.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.model
        $simplifiedOutputs = $simplified.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.outputs
        $simplifiedEvents = $simplified.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.events
        $simplifiedFilters = $simplified.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.filters
        $recursiveSimplifiedFilters = $recursiveSimplified.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.filters
        $recursiveSimplifiedFormat = $recursiveSimplified.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.format
        $recursiveSimplifiedGrouped = $recursiveSimplified.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.grouped
        $recursiveSimplifiedNested = $recursiveSimplified.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.nested
        $recursiveSimplifiedSplitChoice = $recursiveSimplified.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.splitChoice
        $requiredNested = $typePropertiesRequired.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.nested
        $requiredSplitChoice = $typePropertiesRequired.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.splitChoice
        $splitChoice = $typeEnumSplit.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.splitChoice
        $discriminatedFormat = $oneOfDiscriminated.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.format
        $discriminatedGrouped = $oneOfDiscriminated.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.grouped
        $discriminatedSplitChoice = $oneOfDiscriminated.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.splitChoice
        $normalizedOutputs = $oneOfNormalized.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.outputs
        $normalizedEvents = $oneOfNormalized.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.events
        $normalizedFilters = $oneOfNormalized.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.filters
        $normalizedFormat = $oneOfNormalized.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.format
        $normalizedGrouped = $oneOfNormalized.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.grouped
        $normalizedNested = $oneOfNormalized.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.nested
        $flattenedGrouped = $oneOfFlattened.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.grouped
        $flattenedNested = $oneOfFlattened.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.nested
        $flattenedSplitChoice = $oneOfFlattened.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.splitChoice
        $wrappedSplitChoice = $singletonTypeWrapped.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.splitChoice
        $wrappedTools = $singletonTypeWrapped.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.tools
        $sortedModel = $sorted.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.model
        $sortedEvents = $sorted.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.events
        $sortedFilters = $sorted.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.filters
        $sortedGrouped = $sorted.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.grouped
        $sortedNested = $sorted.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.nested
        $sortedTools = $sorted.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.tools
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
        Assert-True (-not ($recursiveSimplifiedNested.oneOf[0].PSObject.Properties.Name -ccontains "required")) "Recursive Simplify stage required the optional alpha type early."
        Assert-True (-not ($recursiveSimplifiedNested.oneOf[2].PSObject.Properties.Name -ccontains "required")) "Recursive Simplify stage required the optional omega type early."
        Assert-True (-not ($recursiveSimplifiedSplitChoice.oneOf[1].PSObject.Properties.Name -ccontains "required")) "Recursive Simplify stage required the optional multi-value type early."
        Assert-Sequence @("alpha", "beta") @($recursiveSimplifiedSplitChoice.oneOf[1].properties.type.enum) "Recursive Simplify stage changed the multi-value type enum early."
        Assert-Sequence @("type") @($requiredNested.oneOf[0].required) "Type requirement stage did not require the alpha discriminator property."
        Assert-Sequence @("type") @($requiredNested.oneOf[2].required) "Type requirement stage did not require the omega discriminator property."
        Assert-Sequence @("type") @($requiredSplitChoice.oneOf[1].required) "Type requirement stage did not require the multi-value discriminator property."
        Assert-Sequence @("type") @($requiredSplitChoice.oneOf[2].required) "Type requirement stage changed the already-required discriminator property."
        Assert-Equal 4 $splitChoice.oneOf.Count "Type enum split stage emitted the wrong branch count."
        Assert-Sequence @("alpha", "beta", "gamma") @($splitChoice.oneOf[1..3] | ForEach-Object { $_.properties.type.enum[0] }) "Type enum split stage emitted the wrong singleton branches."
        Assert-True (-not ($discriminatedSplitChoice.PSObject.Properties.Name -ccontains "discriminator")) "Discriminator stage added metadata to a mixed scalar/object union."
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
        Assert-Equal 4 $flattenedSplitChoice.oneOf.Count "oneOf Flatten stage changed the mixed splitChoice union."
        Assert-Equal 2 $wrappedSplitChoice.oneOf.Count "Singleton wrapper stage emitted the wrong splitChoice outer branch count."
        Assert-Equal "type" $wrappedSplitChoice.oneOf[1].discriminator.propertyName "Singleton wrapper stage used the wrong discriminator."
        Assert-Sequence @("alpha", "beta", "gamma") @($wrappedSplitChoice.oneOf[1].oneOf | ForEach-Object { $_.properties.type.enum[0] }) "Singleton wrapper stage changed splitChoice branch order."
        Assert-Sequence @("web_search_preview", "function", "apply_patch") @(
            $wrappedTools.items.oneOf | ForEach-Object { $_.properties.type.enum[0] }
        ) "A pre-sort stage changed tools branch order."
        Assert-True ($sortedEvents.PSObject.Properties.Name -ccontains "anyOf") "Sort stage lost the retained anyOf."
        Assert-Equal "type" $sortedGrouped.discriminator.propertyName "Sort stage lost the grouped discriminator."
        Assert-Equal 5 $sortedGrouped.oneOf.Count "Sort stage changed grouped branches."
        Assert-Equal 2 $sortedNested.oneOf.Count "Singleton wrapper stage did not group the newly required type alternatives."
        $sortedNestedTypeGroups = @($sortedNested.oneOf | Where-Object {
            $_.PSObject.Properties.Name -ccontains "discriminator"
        })
        Assert-Equal 1 $sortedNestedTypeGroups.Count "Sorted nested union did not contain one type discriminator group."
        Assert-Sequence @("alpha", "omega") @(
            $sortedNestedTypeGroups[0].oneOf | ForEach-Object { $_.properties.type.enum[0] }
        ) "Newly required singleton discriminator properties were not grouped together."
        Assert-Sequence @("apply_patch", "function", "web_search_preview") @(
            $sortedTools.items.oneOf | ForEach-Object { $_.properties.type.enum[0] }
        ) "Sort stage did not order tools by discriminator value."
        Assert-Sequence @("alpha", "middle", "zeta") @($sortedModel.anyOf[1].enum) "Sort stage did not canonicalize the enum."
        Assert-Equal $true $sortedFilters.anyOf[0].additionalProperties "Sort stage lost the free-form recursive replacement."

        foreach ($stageDocument in @($oneOfDiscriminated, $oneOfFlattened, $singletonTypeWrapped)) {
            $inputItem = $stageDocument.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.input_item
            Assert-Equal "type" $inputItem.discriminator.propertyName "A generation stage lost the outer discriminator."
            Assert-Equal "role" $inputItem.oneOf[0].discriminator.propertyName "A generation stage lost the nested role discriminator."
            Assert-Sequence @("message") @($inputItem.oneOf[0].properties.type.enum) "A generation stage lost the intermediate discriminator value."
        }
        $inputItemName = "createResponse_request_object_InputItem_discriminatedObject"
        $messageName = "${inputItemName}_typeOfMessage"
        $inputItemComponent = Get-JsonPropertyValue -Object $componentized.components.schemas -Name $inputItemName
        $messageComponent = Get-JsonPropertyValue -Object $componentized.components.schemas -Name $messageName
        Assert-True ($null -ne $messageComponent) "Generation omitted the typeOfMessage hierarchy component."
        Assert-Equal "#/components/schemas/$messageName" $inputItemComponent.discriminator.mapping.message "Generation split the message and tool-call hierarchies."
        Assert-Equal "role" $messageComponent.discriminator.propertyName "Componentized Message lost role routing."

        $optionsName = "createResponse_request_object_PromptCacheOptions_object"
        $requestComponent = Get-JsonPropertyValue -Object $componentized.components.schemas -Name "createResponse_request_object"
        $optionsComponent = Get-JsonPropertyValue -Object $componentized.components.schemas -Name $optionsName
        Assert-Equal "object" $optionsComponent.type "Nullable component did not become a non-null object definition."
        Assert-Equal "#/components/schemas/$optionsName" $requestComponent.properties.prompt_cache_options.anyOf[0].'$ref' "Nullable property did not reference its object component."
        Assert-Equal "null" $requestComponent.properties.prompt_cache_options.anyOf[1].type "Nullable property lost null."
        Assert-Equal "#/components/schemas/${optionsName}_Mode_enum" $optionsComponent.properties.mode.anyOf[0].'$ref' "Nullable mode did not reference a non-null enum component."
        Assert-NoOpenApiSchemaTypeArrays -Value $componentized

        $sortedBreakpoint = $sorted.paths.'/responses'.post.requestBody.content.'application/json'.schema.properties.prompt_cache_breakpoint
        Assert-Sequence @("explicit") @($sortedBreakpoint.properties.mode.enum) "Singleton enum was extracted before componentization."
        $breakpointName = "createResponse_request_object_PromptCacheBreakpoint_object"
        $breakpoint = Get-JsonPropertyValue -Object $componentized.components.schemas -Name $breakpointName
        Assert-Equal "#/components/schemas/${breakpointName}_Mode_enum" $breakpoint.properties.mode.'$ref' "Componentization did not extract the singleton breakpoint mode."
        Assert-Sequence @("mode") @($breakpoint.required) "Singleton extraction changed required mode."
        Assert-Equal $false $breakpoint.additionalProperties "Singleton extraction changed the breakpoint object constraint."

        $splitWrapperName = "createResponse_request_object_SplitChoice_union_discriminatedObject"
        Assert-True (Test-JsonProperty -Object $componentized.components.schemas -Name $splitWrapperName) "Componentization omitted the splitChoice discriminator wrapper."
        Assert-True (Test-JsonProperty -Object $componentized.components.schemas -Name "${splitWrapperName}_typeOfAlpha") "Componentization omitted the alpha split variant."
        Assert-True (Test-JsonProperty -Object $componentized.components.schemas -Name "${splitWrapperName}_typeOfBeta") "Componentization omitted the beta split variant."
        Assert-True (Test-JsonProperty -Object $componentized.components.schemas -Name "${splitWrapperName}_typeOfGamma") "Componentization omitted the existing singleton variant."
        $componentizedSplitWrapper = Get-JsonPropertyValue -Object $componentized.components.schemas -Name $splitWrapperName
        Assert-True (Test-JsonProperty -Object $componentizedSplitWrapper -Name "oneOf") "Componentized discriminator wrapper did not retain oneOf for the inheritance phase."
        Assert-OpenApiReferencesResolve -Document $componentized
        Assert-AdditionalOpenApiReferencesResolve -Document $componentized
        $null = Assert-OpenApiDiscriminatorMappings -Document $componentized
    }

    # Orchestration should validate its source before launching any stage or
    # creating a partial artifact chain.
    Invoke-Test "missing and non-file specifications fail clearly" {
        $guardOutputRoot = Join-Path $temporaryDirectory "source-guard"
        $guardPaths = Get-OpenApiArtifactPaths -FeatureArea $featureArea -OutputRoot $guardOutputRoot
        $null = [System.IO.Directory]::CreateDirectory($guardPaths.OutputDirectory)
        [System.IO.File]::WriteAllText($guardPaths.DefaultComponentMap, "preserve", [System.Text.UTF8Encoding]::new($false))
        [System.IO.File]::WriteAllText($guardPaths.Final, "published", [System.Text.UTF8Encoding]::new($false))
        [byte[]]$guardBytes = [System.IO.File]::ReadAllBytes($guardPaths.DefaultComponentMap)
        [byte[]]$publishedBytes = [System.IO.File]::ReadAllBytes($guardPaths.Final)
        Assert-Throws {
            Invoke-OpenApiGeneration `
                -FeatureArea $featureArea `
                -OperationId "createResponse" `
                -SpecificationPath (Join-Path $temporaryDirectory "missing.json") `
                -OutputRoot $guardOutputRoot
        } "*cannot find path*"
        Assert-BytesEqual $guardBytes ([System.IO.File]::ReadAllBytes($guardPaths.DefaultComponentMap)) "Early failure changed the existing default map."
        Assert-BytesEqual $publishedBytes ([System.IO.File]::ReadAllBytes($guardPaths.Final)) "Early failure changed the published alias."
        Assert-Throws {
            Invoke-OpenApiGeneration `
                -FeatureArea $featureArea `
                -OperationId "createResponse" `
                -SpecificationPath $temporaryDirectory `
                -OutputRoot (Join-Path $temporaryDirectory "directory-output")
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