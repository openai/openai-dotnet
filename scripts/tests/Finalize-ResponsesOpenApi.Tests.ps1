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
    $document = Read-TestJsonFixture -RelativePath "Deduplicate-OpenApiComponents/source.componentized.openapi.json"
    $properties = $document.paths["/test"].post.responses["200"].content["application/json"].schema.properties
    $null = $properties.Remove("cycle")
    foreach ($name in @("CycleA", "CycleB", "CycleAncestor")) {
        $null = $document.components.schemas.Remove($name)
    }
    $null = Write-OpenApiJsonFile -Document $document -Path $paths.Componentized
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
        Assert-True ($null -ne (Get-Command Copy-OpenApiFileAtomically -ErrorAction SilentlyContinue)) "Atomic copy function was not defined."
        Assert-True (-not (Get-Command Invoke-ResponsesOpenApiFinalization).Parameters.ContainsKey("ResetDeduplicationMap")) "Finalization still exposes reset policy."
        Assert-True (-not [System.IO.Directory]::Exists((Join-Path $temporaryDirectory "unexpected"))) "Dot-sourcing unexpectedly finalized output."
    }

    Invoke-Test "production defaults and downstream stages are correct" {
        $expectedDirectory = Join-Path $script:RepositoryRoot "specification" "openapi"
        Assert-Equal ([System.IO.Path]::GetFullPath($expectedDirectory)) ([System.IO.Path]::GetFullPath($script:DefaultOutputDirectory)) "Default output directory was incorrect."
        foreach ($stageScriptName in @(
            "Generate-OpenApiComponentDefaultMap.ps1",
            "Deduplicate-OpenApiComponents.ps1"
        )) {
            $stageScriptPath = Join-Path (Split-Path $scriptPath -Parent) $stageScriptName
            Assert-True (Test-Path -LiteralPath $stageScriptPath -PathType Leaf) "Required stage script was not found: $stageScriptPath"
        }

        $paths = Get-ResponsesOpenApiArtifactPaths -OutputDirectory $expectedDirectory
        Assert-Equal "responses.componentized.openapi.json" ([System.IO.Path]::GetFileName($paths.Componentized)) "Componentized input name was incorrect."
        Assert-Equal "responses.component-default-map.json" ([System.IO.Path]::GetFileName($paths.DefaultComponentMap)) "Default map name was incorrect."
        Assert-Equal "responses.component-policy-map.json" ([System.IO.Path]::GetFileName($paths.PolicyComponentMap)) "Policy map name was incorrect."
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
        foreach ($artifactPath in @($result.DefaultComponentMap, $result.Deduplicated, $result.Final)) {
            Assert-True ([System.IO.File]::Exists($artifactPath)) "Finalization artifact was not created: $artifactPath"
        }
        Assert-BytesEqual $componentizedBytes ([System.IO.File]::ReadAllBytes($result.Componentized)) "Finalization modified its componentized input."
        Assert-BytesEqual ([System.IO.File]::ReadAllBytes($result.Deduplicated)) ([System.IO.File]::ReadAllBytes($result.Final)) "Final alias differs from the retained deduplicated output."

        $mapping = Read-FinalizationJson -Path $result.DefaultComponentMap
        Assert-Equal 1 $mapping.version "Default map version was incorrect."
        Assert-Equal 4 $mapping.groups.Count "Default map group count was incorrect."
        Assert-True (-not [System.IO.File]::Exists($result.PolicyComponentMap)) "Finalization created a policy map."
        $document = Read-FinalizationJson -Path $result.Deduplicated
        Assert-Equal 4 @(Get-JsonProperties -Value $document.components.schemas).Count "Deduplicated component count was incorrect."
        Assert-Equal "#/components/schemas/LeafA/properties/value" $document.paths["/test"].post.responses["200"].content["application/json"].schema.properties.descendantAlias.'$ref' "Default leaf id was not applied."
        Assert-OpenApiReferencesResolve -Document $document
        Assert-AdditionalOpenApiReferencesResolve -Document $document

        [byte[]]$defaultBytes = [System.IO.File]::ReadAllBytes($result.DefaultComponentMap)
        [byte[]]$deduplicatedBytes = [System.IO.File]::ReadAllBytes($result.Deduplicated)
        [byte[]]$finalBytes = [System.IO.File]::ReadAllBytes($result.Final)
        $null = Invoke-ResponsesOpenApiFinalization -OutputDirectory $outputDirectory 6>$null
        Assert-BytesEqual $defaultBytes ([System.IO.File]::ReadAllBytes($result.DefaultComponentMap)) "Repeated finalization changed default-map bytes."
        Assert-BytesEqual $deduplicatedBytes ([System.IO.File]::ReadAllBytes($result.Deduplicated)) "Repeated finalization changed deduplicated bytes."
        Assert-BytesEqual $finalBytes ([System.IO.File]::ReadAllBytes($result.Final)) "Repeated finalization changed final bytes."
    }

    Invoke-Test "default edits are discarded while policy edits persist" {
        $outputDirectory = Join-Path $temporaryDirectory "reviewed"
        $paths = Initialize-FinalizationOutput -OutputDirectory $outputDirectory
        $null = Invoke-ResponsesOpenApiFinalization -OutputDirectory $outputDirectory 6>$null
        [byte[]]$componentizedBytes = [System.IO.File]::ReadAllBytes($paths.Componentized)

        $defaultMap = Read-FinalizationJson -Path $paths.DefaultComponentMap
        $leafGroup = @($defaultMap.groups | Where-Object { $_.members -ccontains "LeafA" })[0]
        $leafGroup.id = "DiscardedEdit"
        $null = Write-OpenApiJsonFile -Document $defaultMap -Path $paths.DefaultComponentMap

        $null = Invoke-ResponsesOpenApiFinalization -OutputDirectory $outputDirectory 6>$null
        Assert-BytesEqual $componentizedBytes ([System.IO.File]::ReadAllBytes($paths.Componentized)) "Map-only finalization changed the componentized input."
        $regeneratedMap = Read-FinalizationJson -Path $paths.DefaultComponentMap
        Assert-Equal "LeafA" (@($regeneratedMap.groups | Where-Object { $_.members -ccontains "LeafA" })[0].id) "Finalization preserved a manual default edit."

        $policy = [ordered]@{
            version = [long]1
            groups = @([ordered]@{
                id = "SharedLeaf"
                type = "duplicate"
                representative = "LeafA"
                fingerprint = (@($regeneratedMap.groups | Where-Object { $_.members -ccontains "LeafA" })[0].fingerprint)
                members = @("LeafA", "LeafB")
            })
        }
        $null = Write-OpenApiJsonFile -Document $policy -Path $paths.PolicyComponentMap
        [byte[]]$policyBytes = [System.IO.File]::ReadAllBytes($paths.PolicyComponentMap)
        $null = Invoke-ResponsesOpenApiFinalization -OutputDirectory $outputDirectory 6>$null
        Assert-BytesEqual $policyBytes ([System.IO.File]::ReadAllBytes($paths.PolicyComponentMap)) "Finalization rewrote policy bytes."
        $reviewedDocument = Read-FinalizationJson -Path $paths.Deduplicated
        Assert-True (Test-JsonProperty -Object $reviewedDocument.components.schemas -Name "SharedLeaf") "Policy id was not used as the retained component name."
        Assert-Equal "#/components/schemas/SharedLeaf/properties/value" $reviewedDocument.paths["/test"].post.responses["200"].content["application/json"].schema.properties.descendantAlias.'$ref' "Policy id was not used as the reference target."
    }

    Invoke-Test "invalid policy preserves policy generated map and published outputs" {
        $outputDirectory = Join-Path $temporaryDirectory "invalid-policy"
        $paths = Initialize-FinalizationOutput -OutputDirectory $outputDirectory
        $null = Invoke-ResponsesOpenApiFinalization -OutputDirectory $outputDirectory 6>$null
        [byte[]]$defaultBytes = [System.IO.File]::ReadAllBytes($paths.DefaultComponentMap)
        [byte[]]$deduplicatedBytes = [System.IO.File]::ReadAllBytes($paths.Deduplicated)
        [byte[]]$finalBytes = [System.IO.File]::ReadAllBytes($paths.Final)
        [System.IO.File]::WriteAllText($paths.PolicyComponentMap, "invalid", [System.Text.UTF8Encoding]::new($false))
        [byte[]]$policyBytes = [System.IO.File]::ReadAllBytes($paths.PolicyComponentMap)

        Assert-Throws {
            Invoke-ResponsesOpenApiFinalization -OutputDirectory $outputDirectory 6>$null
        } "*Could not parse component policy map JSON*"
        Assert-BytesEqual $policyBytes ([System.IO.File]::ReadAllBytes($paths.PolicyComponentMap)) "Failed finalization changed policy bytes."
        Assert-BytesEqual $defaultBytes ([System.IO.File]::ReadAllBytes($paths.DefaultComponentMap)) "Failed finalization changed default bytes."
        Assert-BytesEqual $deduplicatedBytes ([System.IO.File]::ReadAllBytes($paths.Deduplicated)) "Failed finalization changed deduplicated bytes."
        Assert-BytesEqual $finalBytes ([System.IO.File]::ReadAllBytes($paths.Final)) "Failed finalization changed final bytes."
    }

    Invoke-Test "componentized input preflight protects component maps" {
        foreach ($case in @("missing", "directory")) {
            $outputDirectory = Join-Path $temporaryDirectory "preflight-$case"
            $null = [System.IO.Directory]::CreateDirectory($outputDirectory)
            $paths = Get-ResponsesOpenApiArtifactPaths -OutputDirectory $outputDirectory
            [System.IO.File]::WriteAllText($paths.DefaultComponentMap, "default", [System.Text.UTF8Encoding]::new($false))
            [System.IO.File]::WriteAllText($paths.PolicyComponentMap, "policy", [System.Text.UTF8Encoding]::new($false))
            if ($case -eq "directory") {
                $null = [System.IO.Directory]::CreateDirectory($paths.Componentized)
            }

            $expectedMessage = if ($case -eq "missing") { "*was not found*" } else { "*is not a file*" }
            Assert-Throws {
                Invoke-ResponsesOpenApiFinalization -OutputDirectory $outputDirectory
            } $expectedMessage
            Assert-Equal "default" ([System.IO.File]::ReadAllText($paths.DefaultComponentMap)) "Input preflight changed the default map for '$case'."
            Assert-Equal "policy" ([System.IO.File]::ReadAllText($paths.PolicyComponentMap)) "Input preflight changed the policy map for '$case'."
        }
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
