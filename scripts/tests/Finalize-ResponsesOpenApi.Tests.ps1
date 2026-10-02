<#
.SYNOPSIS
    Runs focused tests for scripts/Finalize-ResponsesOpenApi.ps1.
#>

[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$scriptPath = Join-Path $PSScriptRoot ".." "Finalize-ResponsesOpenApi.ps1"
. $scriptPath
. (Join-Path $PSScriptRoot ".." "_OpenApiDocument.Helpers.ps1")
. (Join-Path $PSScriptRoot "_OpenApiComponentDeduplicationTest.Helpers.ps1")
Initialize-OpenApiComponentDeduplicationTestState

$fixturePath = Get-TestDeduplicationFixturePath
$temporaryDirectory = Join-Path ([System.IO.Path]::GetTempPath()) "finalize-responses-openapi-$([Guid]::NewGuid().ToString('N'))"
$null = [System.IO.Directory]::CreateDirectory($temporaryDirectory)

function Initialize-FinalizationOutput {
    param([string]$OutputDirectory)

    $null = [System.IO.Directory]::CreateDirectory($OutputDirectory)
    $paths = Get-ResponsesOpenApiArtifactPaths -OutputDirectory $OutputDirectory
    [System.IO.File]::Copy($fixturePath, $paths.Componentized, $true)
    return $paths
}

function Read-FinalizationJson {
    param([string]$Path)

    return Get-Content -LiteralPath $Path -Raw -Encoding utf8 |
        ConvertFrom-Json -AsHashtable -Depth 100
}

try {
    Invoke-Test "dot-sourcing defines finalization without running it" {
        Assert-True ($null -ne (Get-Command Invoke-ResponsesOpenApiFinalization -ErrorAction SilentlyContinue)) "Finalization function was not defined."
        Assert-True ($null -ne (Get-Command Get-ResponsesOpenApiArtifactPaths -ErrorAction SilentlyContinue)) "Artifact path function was not defined."
        Assert-True ($null -ne (Get-Command Reset-ResponsesOpenApiDeduplicationMap -ErrorAction SilentlyContinue)) "Map reset function was not defined."
        Assert-True ($null -ne (Get-Command Copy-OpenApiFileAtomically -ErrorAction SilentlyContinue)) "Atomic copy function was not defined."
        Assert-True (-not [System.IO.Directory]::Exists((Join-Path $temporaryDirectory "unexpected"))) "Dot-sourcing unexpectedly finalized output."
    }

    Invoke-Test "production defaults and downstream stages are correct" {
        $expectedDirectory = Join-Path $script:RepositoryRoot "specification" "openapi"
        Assert-Equal ([System.IO.Path]::GetFullPath($expectedDirectory)) ([System.IO.Path]::GetFullPath($script:DefaultOutputDirectory)) "Default output directory was incorrect."
        foreach ($stageScriptName in @(
            "Update-OpenApiComponentDeduplicationMap.ps1",
            "Deduplicate-OpenApiComponents.ps1"
        )) {
            $stageScriptPath = Join-Path (Split-Path $scriptPath -Parent) $stageScriptName
            Assert-True (Test-Path -LiteralPath $stageScriptPath -PathType Leaf) "Required stage script was not found: $stageScriptPath"
        }

        $paths = Get-ResponsesOpenApiArtifactPaths -OutputDirectory $expectedDirectory
        Assert-Equal "responses.componentized.openapi.json" ([System.IO.Path]::GetFileName($paths.Componentized)) "Componentized input name was incorrect."
        Assert-Equal "responses.component-deduplication-map.json" ([System.IO.Path]::GetFileName($paths.DeduplicationMap)) "Mapping name was incorrect."
        Assert-Equal "responses.componentized.deduplicated.openapi.json" ([System.IO.Path]::GetFileName($paths.Deduplicated)) "Deduplicated output name was incorrect."
        Assert-Equal "responses.openapi.json" ([System.IO.Path]::GetFileName($paths.Final)) "Final output name was incorrect."
    }

    Invoke-Test "standalone finalization creates map deduplicated output and final alias" {
        $outputDirectory = Join-Path $temporaryDirectory "fresh"
        $paths = Initialize-FinalizationOutput -OutputDirectory $outputDirectory
        [byte[]]$componentizedBytes = [System.IO.File]::ReadAllBytes($paths.Componentized)

        $pwshPath = (Get-Process -Id $PID).Path
        & $pwshPath `
            -NoLogo `
            -NoProfile `
            -NonInteractive `
            -File $scriptPath `
            -OutputDirectory $outputDirectory
        if ($LASTEXITCODE -ne 0) {
            throw "Finalize CLI failed with exit code $LASTEXITCODE."
        }
        $result = Get-ResponsesOpenApiArtifactPaths -OutputDirectory $outputDirectory
        foreach ($artifactPath in @($result.DeduplicationMap, $result.Deduplicated, $result.Final)) {
            Assert-True ([System.IO.File]::Exists($artifactPath)) "Finalization artifact was not created: $artifactPath"
        }
        Assert-BytesEqual $componentizedBytes ([System.IO.File]::ReadAllBytes($result.Componentized)) "Finalization modified its componentized input."
        Assert-BytesEqual ([System.IO.File]::ReadAllBytes($result.Deduplicated)) ([System.IO.File]::ReadAllBytes($result.Final)) "Final alias differs from the retained deduplicated output."

        $mapping = Read-FinalizationJson -Path $result.DeduplicationMap
        Assert-Equal 1 $mapping.version "Mapping version was incorrect."
        Assert-Equal 3 $mapping.groups.Count "Mapping group count was incorrect."
        $document = Read-FinalizationJson -Path $result.Deduplicated
        Assert-Equal 7 @(Get-JsonProperties -Value $document.components.schemas).Count "Deduplicated component count was incorrect."
        Assert-Equal "#/components/schemas/LeafA/properties/value" $document.paths["/test"].post.responses["200"].content["application/json"].schema.properties.descendantAlias.'$ref' "Default leaf id was not applied."
        Assert-OpenApiReferencesResolve -Document $document
        Assert-AdditionalOpenApiReferencesResolve -Document $document
    }

    Invoke-Test "reviewed ids survive the map-only fast path and reset deterministically" {
        $outputDirectory = Join-Path $temporaryDirectory "reviewed"
        $paths = Initialize-FinalizationOutput -OutputDirectory $outputDirectory
        $null = Invoke-ResponsesOpenApiFinalization -OutputDirectory $outputDirectory 6>$null
        [byte[]]$componentizedBytes = [System.IO.File]::ReadAllBytes($paths.Componentized)

        $mapping = Read-FinalizationJson -Path $paths.DeduplicationMap
        $leafGroup = @($mapping.groups | Where-Object { $_.members -ccontains "LeafA" })[0]
        $leafGroup.id = "SharedLeaf"
        $null = Write-OpenApiJsonFile -Document $mapping -Path $paths.DeduplicationMap
        [byte[]]$reviewedMappingBytes = [System.IO.File]::ReadAllBytes($paths.DeduplicationMap)

        $null = Invoke-ResponsesOpenApiFinalization -OutputDirectory $outputDirectory 6>$null
        Assert-BytesEqual $componentizedBytes ([System.IO.File]::ReadAllBytes($paths.Componentized)) "Map-only finalization changed the componentized input."
        Assert-BytesEqual $reviewedMappingBytes ([System.IO.File]::ReadAllBytes($paths.DeduplicationMap)) "Map-only finalization rewrote the reviewed mapping."
        $reviewedDocument = Read-FinalizationJson -Path $paths.Deduplicated
        Assert-True (Test-JsonProperty -Object $reviewedDocument.components.schemas -Name "SharedLeaf") "Reviewed id was not used as the retained component name."
        Assert-Equal "#/components/schemas/SharedLeaf/properties/value" $reviewedDocument.paths["/test"].post.responses["200"].content["application/json"].schema.properties.descendantAlias.'$ref' "Reviewed id was not used as the reference target."

        $null = Invoke-ResponsesOpenApiFinalization -OutputDirectory $outputDirectory -ResetDeduplicationMap 6>$null
        $resetMapping = Read-FinalizationJson -Path $paths.DeduplicationMap
        $resetLeafGroup = @($resetMapping.groups | Where-Object { $_.members -ccontains "LeafA" })[0]
        Assert-Equal "LeafA" $resetLeafGroup.id "Reset did not restore the default id."
        [byte[]]$resetMappingBytes = [System.IO.File]::ReadAllBytes($paths.DeduplicationMap)
        [byte[]]$resetDeduplicatedBytes = [System.IO.File]::ReadAllBytes($paths.Deduplicated)
        [byte[]]$resetFinalBytes = [System.IO.File]::ReadAllBytes($paths.Final)

        $null = Invoke-ResponsesOpenApiFinalization -OutputDirectory $outputDirectory -ResetDeduplicationMap 6>$null
        Assert-BytesEqual $resetMappingBytes ([System.IO.File]::ReadAllBytes($paths.DeduplicationMap)) "Repeated reset changed mapping bytes."
        Assert-BytesEqual $resetDeduplicatedBytes ([System.IO.File]::ReadAllBytes($paths.Deduplicated)) "Repeated reset changed deduplicated bytes."
        Assert-BytesEqual $resetFinalBytes ([System.IO.File]::ReadAllBytes($paths.Final)) "Repeated reset changed final bytes."
        Assert-BytesEqual $componentizedBytes ([System.IO.File]::ReadAllBytes($paths.Componentized)) "Reset changed the componentized input."
    }

    Invoke-Test "stale mapping failure preserves map and published outputs" {
        $outputDirectory = Join-Path $temporaryDirectory "stale"
        $paths = Initialize-FinalizationOutput -OutputDirectory $outputDirectory
        $null = Invoke-ResponsesOpenApiFinalization -OutputDirectory $outputDirectory 6>$null

        $mapping = Read-FinalizationJson -Path $paths.DeduplicationMap
        $mapping.groups[0].members[1] = "MissingLeaf"
        $null = Write-OpenApiJsonFile -Document $mapping -Path $paths.DeduplicationMap
        [byte[]]$mappingBytes = [System.IO.File]::ReadAllBytes($paths.DeduplicationMap)
        [byte[]]$deduplicatedBytes = [System.IO.File]::ReadAllBytes($paths.Deduplicated)
        [byte[]]$finalBytes = [System.IO.File]::ReadAllBytes($paths.Final)

        Assert-Throws {
            Invoke-ResponsesOpenApiFinalization -OutputDirectory $outputDirectory 6>$null
        } "*missing from the componentized source*"
        Assert-BytesEqual $mappingBytes ([System.IO.File]::ReadAllBytes($paths.DeduplicationMap)) "Failed finalization changed the stale mapping."
        Assert-BytesEqual $deduplicatedBytes ([System.IO.File]::ReadAllBytes($paths.Deduplicated)) "Failed finalization changed the deduplicated output."
        Assert-BytesEqual $finalBytes ([System.IO.File]::ReadAllBytes($paths.Final)) "Failed finalization changed the final output."
    }

    Invoke-Test "componentized input preflight protects map reset" {
        foreach ($case in @("missing", "directory")) {
            $outputDirectory = Join-Path $temporaryDirectory "preflight-$case"
            $null = [System.IO.Directory]::CreateDirectory($outputDirectory)
            $paths = Get-ResponsesOpenApiArtifactPaths -OutputDirectory $outputDirectory
            [System.IO.File]::WriteAllText($paths.DeduplicationMap, "preserve", [System.Text.UTF8Encoding]::new($false))
            if ($case -eq "directory") {
                $null = [System.IO.Directory]::CreateDirectory($paths.Componentized)
            }

            $expectedMessage = if ($case -eq "missing") { "*was not found*" } else { "*is not a file*" }
            Assert-Throws {
                Invoke-ResponsesOpenApiFinalization -OutputDirectory $outputDirectory -ResetDeduplicationMap
            } $expectedMessage
            Assert-Equal "preserve" ([System.IO.File]::ReadAllText($paths.DeduplicationMap)) "Input preflight deleted the existing map for '$case'."
        }
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

    Invoke-Test "atomic copy replaces a stale final alias" {
        $sourcePath = Join-Path $temporaryDirectory "copy-source.json"
        $finalPath = Join-Path $temporaryDirectory "responses.openapi.json"
        [System.IO.File]::WriteAllText($sourcePath, "fresh", [System.Text.UTF8Encoding]::new($false))
        [System.IO.File]::WriteAllText($finalPath, "stale", [System.Text.UTF8Encoding]::new($false))
        $writtenPath = Copy-OpenApiFileAtomically -SourcePath $sourcePath -DestinationPath $finalPath
        Assert-Equal ([System.IO.Path]::GetFullPath($finalPath)) $writtenPath "Atomic copy returned the wrong path."
        Assert-BytesEqual ([System.IO.File]::ReadAllBytes($sourcePath)) ([System.IO.File]::ReadAllBytes($finalPath)) "Stale final alias was not atomically replaced."
    }
}
finally {
    if ([System.IO.Directory]::Exists($temporaryDirectory)) {
        [System.IO.Directory]::Delete($temporaryDirectory, $true)
    }
}

Complete-OpenApiComponentDeduplicationTests
