#!/usr/bin/env pwsh

<#
.DESCRIPTION
Synchronizes the local snapshot of the OpenAI REST API specification.

Downloads the specification, skips the run when the content is byte-for-byte what was processed
last time, rotates the current snapshot into the previous slot, and runs the spec processor to
split the specification into per-feature specifications and generate a diff report.

The same script backs both the scheduled workflow and local runs, so what CI does and what a
maintainer can reproduce on their own machine are the same code path.

.PARAMETER SourceUrl
The specification to download. Defaults to the URL recorded in the processor configuration.

.PARAMETER SourceRef
The branch, tag, or commit in the upstream OpenAPI repository to snapshot. Defaults to the upstream
default branch. Use this to take an advance copy of a change that has not shipped yet. Whatever is
given is resolved to a commit and the download is pinned to it, so the snapshot and its recorded
provenance cannot disagree, and so a push to upstream partway through a run cannot produce a
snapshot that is half of one revision and half of another.

.PARAMETER Force
Process the specification even when its content matches what was processed last time.

.PARAMETER DryRun
Write the results to a temporary directory and leave the committed snapshot untouched. Use this to
preview a change or to test the tooling.

.PARAMETER RepoPath
The path to the local repository. Defaults to the repository containing this script.

.EXAMPLE
# Take the scheduled snapshot
./Sync-OpenAISpec.ps1

.EXAMPLE
# Preview an in-progress upstream change without touching the snapshot
./Sync-OpenAISpec.ps1 -SourceRef "some-feature-branch" -DryRun
#>
[CmdletBinding(SupportsShouldProcess = $true)]
param(
  [Parameter(Mandatory = $false)]
  [string]$SourceUrl = "",

  [Parameter(Mandatory = $false)]
  [string]$SourceRef = "main",

  [Parameter(Mandatory = $false)]
  [switch]$Force,

  [Parameter(Mandatory = $false)]
  [switch]$DryRun,

  [Parameter(Mandatory = $false)]
  [string]$RepoPath = (Split-Path -Parent $PSScriptRoot)
)

$UpstreamOwner = "openai"
$UpstreamRepo = "openai-openapi"
$UpstreamSpecPath = "openapi.yaml"

function Write-Log {
    param([string]$Message)
    Write-Host "$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss'): $Message" -ForegroundColor Green
}

function Write-WarningLog {
    param([string]$Message)
    Write-Host "$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss'): WARNING: $Message" -ForegroundColor Yellow
}

function Write-ErrorLog {
    param([string]$Message)
    Write-Host "$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss'): ERROR: $Message" -ForegroundColor Red
}

# Resolves a branch, tag, or commit in the upstream OpenAPI repository to a full commit SHA, so
# that an advance copy can be pinned to an exact upstream state.
function Get-UpstreamCommitSha {
    param([string]$Ref)

    $uri = "https://api.github.com/repos/$UpstreamOwner/$UpstreamRepo/commits/$Ref"
    $headers = @{ "Accept" = "application/vnd.github+json" }

    # An available token only raises the rate limit; the upstream repository is public.

    if ($env:GH_TOKEN) {
        $headers["Authorization"] = "Bearer $($env:GH_TOKEN)"
    }

    try {
        return (Invoke-RestMethod -Uri $uri -Headers $headers -Method Get).sha
    }
    catch {
        throw "Failed to resolve upstream ref '$Ref': $_"
    }
}

# Reads the content hash recorded by the last processed snapshot, if there is one. The states here
# mirror the ones the processor uses, so that both halves of the pipeline interpret a metadata file
# the same way:
#
#   Missing            - no file, which is the legitimate bootstrap case, so the caller proceeds
#   Legacy             - an older schema, readable, but reseed before trusting the no-op decision
#   Malformed          - unparseable, or carrying no content hash; corruption, so stop
#   UnsupportedVersion - written by a newer tool than this one; stop rather than reinterpret
#
# Only Missing returns without a hash. Everything else either produces one or throws, because a
# hash that cannot be read is not the same as a hash that does not match, and treating it that way
# would rotate a good baseline out of the way on the strength of a corrupt file.
function Get-RecordedContentHash {
    param([string]$SnapshotPath)

    $metadataPath = Join-Path $SnapshotPath ".spec-metadata.json"
    $currentSchemaVersion = 1

    if (-not (Test-Path $metadataPath)) {
        Write-Log "No snapshot metadata at ${metadataPath}; treating this as a first run"
        return $null
    }

    try {
        $metadata = Get-Content $metadataPath -Raw | ConvertFrom-Json
    }
    catch {
        throw "The snapshot metadata at ${metadataPath} could not be parsed: $_"
    }

    $schemaVersion = if ($null -ne $metadata.schemaVersion) { [int]$metadata.schemaVersion } else { 0 }

    if ($schemaVersion -gt $currentSchemaVersion) {
        throw "The snapshot metadata at ${metadataPath} uses schema version ${schemaVersion}, and this script understands ${currentSchemaVersion}. Update the tooling rather than reinterpreting it."
    }

    if ($schemaVersion -lt $currentSchemaVersion) {
        Write-Log "WARNING: The snapshot metadata at ${metadataPath} uses schema version ${schemaVersion}, before the current shape of ${currentSchemaVersion}. Reseed the snapshot to bring it forward."
    }

    if (-not $metadata.sourceContentHash) {
        throw "The snapshot metadata at ${metadataPath} records no source content hash. Reseed the snapshot before running again."
    }

    return $metadata.sourceContentHash
}

# Reads the identity of the processing that produced the snapshot, if there is one. A snapshot that
# predates the field returns $null, which reads as "cannot be shown to match" rather than as a match.
function Get-RecordedProcessingIdentity {
    param([string]$SnapshotPath)

    $metadataPath = Join-Path $SnapshotPath ".spec-metadata.json"

    if (-not (Test-Path $metadataPath)) {
        return $null
    }

    try {
        $metadata = Get-Content $metadataPath -Raw | ConvertFrom-Json
    }
    catch {
        throw "The snapshot metadata at ${metadataPath} could not be parsed: $_"
    }

    if (-not $metadata.processingIdentity) {
        return $null
    }

    return $metadata.processingIdentity
}

# Asks the tool what behavior it would bring to a run. There is one implementation of what the
# identity means and it lives in the tool, because a second one here would be a second thing to keep
# in step, and the whole point is that the two cannot disagree.
function Get-CurrentProcessingFingerprint {
    param([string]$ToolProject, [string]$ConfigPath)

    $output = & dotnet run --project $ToolProject --configuration Release -- --config $ConfigPath identity --fingerprint-only

    if ($LASTEXITCODE -ne 0) {
        throw "Could not determine the processor's identity (exit code $LASTEXITCODE)"
    }

    $fingerprint = ($output | Where-Object { $_ -match '^[0-9a-f]{64}$' } | Select-Object -Last 1)

    if (-not $fingerprint) {
        throw "The processor did not report a usable identity fingerprint"
    }

    return $fingerprint.Trim()
}

# Decides whether freshly processed output differs from the committed snapshot in a way worth
# publishing.
#
# A forced run bypasses the input check on purpose, so without this a diagnostic run would open a
# pull request whose entire content is a moved timestamp. The comparison is over the generated
# specification files and the metadata with the run timestamp removed, because those are the only
# things a reviewer would be reading. The report is derived from the same inputs, so it cannot move
# on its own without one of them moving first.
function Test-MeaningfulChange {
    param([string]$StagingPath, [string]$CurrentPath)

    if (-not (Test-Path (Join-Path $CurrentPath ".spec-metadata.json"))) {
        return $true
    }

    $staged = Get-ChildItem -Path $StagingPath -Filter *.yml -File | Sort-Object Name
    $committed = Get-ChildItem -Path $CurrentPath -Filter *.yml -File | Sort-Object Name

    if ($staged.Count -ne $committed.Count) {
        return $true
    }

    for ($index = 0; $index -lt $staged.Count; $index++) {
        if ($staged[$index].Name -ne $committed[$index].Name) {
            return $true
        }

        if ((Get-FileHash $staged[$index].FullName).Hash -ne (Get-FileHash $committed[$index].FullName).Hash) {
            return $true
        }
    }

    # Everything in the metadata except the moment the run happened is a statement about what was
    # processed, so a difference in any of it is a real difference.

    $stagedMeta = Get-Content (Join-Path $StagingPath ".spec-metadata.json") -Raw | ConvertFrom-Json
    $committedMeta = Get-Content (Join-Path $CurrentPath ".spec-metadata.json") -Raw | ConvertFrom-Json

    $stagedMeta.PSObject.Properties.Remove("processedAt")
    $committedMeta.PSObject.Properties.Remove("processedAt")

    return (($stagedMeta | ConvertTo-Json -Depth 20 -Compress) -ne ($committedMeta | ConvertTo-Json -Depth 20 -Compress))
}

try {
    Push-Location $RepoPath

    $specRoot = Join-Path $RepoPath "specification/openai"
    $currentPath = Join-Path $specRoot "current"
    $previousPath = Join-Path $specRoot "previous"
    $toolProject = Join-Path $RepoPath "specification/tools/OpenAI.SpecProcessor/OpenAI.SpecProcessor.csproj"
    $configPath = Join-Path $RepoPath "specification/tools/openai-spec-processor.json"

    Write-Log "Starting OpenAI specification sync"

    # Work out what to download. An explicit URL is an escape hatch and is taken as given; otherwise
    # the ref is resolved to a commit and the download is pinned to it.

    $commitSha = $null

    if ($SourceUrl) {
        $resolvedUrl = $SourceUrl
    }
    else {
        $commitSha = Get-UpstreamCommitSha -Ref $SourceRef
        $resolvedUrl = "https://raw.githubusercontent.com/$UpstreamOwner/$UpstreamRepo/$commitSha/$UpstreamSpecPath"

        Write-Log "Upstream: $UpstreamOwner/$UpstreamRepo@$SourceRef"
        Write-Log "Resolved commit: $commitSha"
    }

    Write-Log "Source: $resolvedUrl"

    # Download first, because the content itself is what determines whether there is anything to
    # do. Hashing the downloaded document is exact and works for any source, including ones that
    # expose no commit of their own.

    $downloadPath = Join-Path ([System.IO.Path]::GetTempPath()) "openai-rest-$([System.Guid]::NewGuid()).yml"

    try {
        Write-Log "Downloading specification"
        Invoke-WebRequest -Uri $resolvedUrl -OutFile $downloadPath

        if (-not (Test-Path $downloadPath)) {
            throw "Specification download did not produce a file at $downloadPath"
        }

        $contentHash = (Get-FileHash -Path $downloadPath -Algorithm SHA256).Hash.ToLowerInvariant()
        Write-Log "Content hash: $contentHash"

        $recordedHash = Get-RecordedContentHash -SnapshotPath $currentPath

        # Two things decide whether a run has anything to do: the bytes that came in, and the
        # behavior that would process them. Comparing only the bytes assumes the tool would produce
        # the same output it did last month, which stops being true the moment the feature map, the
        # exclusions, the sanitizer, or the comparison scope is edited. That month would skip, and
        # the snapshot would go on describing itself with a taxonomy the tool no longer uses.

        $recordedIdentity = Get-RecordedProcessingIdentity -SnapshotPath $currentPath
        $currentIdentity = Get-CurrentProcessingFingerprint -ToolProject $toolProject -ConfigPath $configPath

        $sourceUnchanged = ($recordedHash -eq $contentHash)
        $behaviorUnchanged = ($recordedIdentity -and ($recordedIdentity.fingerprint -eq $currentIdentity))

        if ($recordedHash -and (-not $behaviorUnchanged)) {
            if (-not $recordedIdentity) {
                Write-Log "The snapshot records no processing identity, so it cannot be shown to match this build. Regenerating."
            }
            else {
                Write-Log "The processor's behavior has changed since the snapshot was taken. Regenerating."
            }
        }

        if ($sourceUnchanged -and $behaviorUnchanged -and (-not $Force)) {
            Write-Log "The specification and the processor are both unchanged since the last run. Nothing to do."

            if ($env:GITHUB_OUTPUT) {
                "changed=false" | Out-File -FilePath $env:GITHUB_OUTPUT -Append
                "content-hash=$contentHash" | Out-File -FilePath $env:GITHUB_OUTPUT -Append
            }

            return
        }

        # Choose the destination. Processing always writes to a staging directory first. The
        # committed snapshot is only touched once the processor has succeeded, so a failure part
        # way through cannot leave the repository without a valid baseline to compare against next
        # time.

        $stagingPath = Join-Path ([System.IO.Path]::GetTempPath()) "openai-spec-sync-$contentHash"
        $processingSucceeded = $false

        Remove-Item -Path $stagingPath -Recurse -Force -ErrorAction SilentlyContinue
        New-Item -ItemType Directory -Force -Path $stagingPath | Out-Null

        # Compare against the committed snapshot itself. Before one exists, the seeded baseline in
        # the previous slot stands in for it.

        $baselinePath = if (Test-Path (Join-Path $currentPath ".spec-metadata.json")) { $currentPath } else { $previousPath }

        if ($DryRun) {
            Write-Log "Dry run. Writing to: $stagingPath"
        }

        New-Item -ItemType Directory -Force -Path $baselinePath | Out-Null

        # Run the processor. It owns the split, validation, and diff, all of which must be exact
        # and repeatable.

        if ($PSCmdlet.ShouldProcess($stagingPath, "Process specification snapshot")) {
            Write-Log "Running the specification processor"

            $processorArgs = @(
                "--config", $configPath,
                "preprocess",
                "--new-spec", $downloadPath,
                "--previous-spec", $baselinePath,
                "--output", $stagingPath,
                "--report", $stagingPath
            )

            $processorArgs += @("--source-url", $resolvedUrl)

            if ($commitSha) {
                $processorArgs += @("--commit-sha", $commitSha)
            }

            & dotnet run --project $toolProject --configuration Release -- @processorArgs

            if ($LASTEXITCODE -ne 0) {
                throw "Specification processing failed with exit code $LASTEXITCODE"
            }
        }

        # Processing succeeded. Now decide whether the result is worth publishing, which is not the
        # same question as whether the run succeeded.
        #
        # The no-op check earlier compares the *inputs*. This compares the *outputs*, and it is what
        # stops a forced run from opening a pull request that moves nothing but timestamps. It also
        # covers the case where upstream genuinely changed but the change lands entirely in something
        # the snapshot does not carry, an excluded path or a stripped documentation key, so the
        # generated files come out identical anyway.

        $meaningfulChange = Test-MeaningfulChange -StagingPath $stagingPath -CurrentPath $currentPath

        if ($DryRun) {
            $outputPath = $stagingPath
        }
        elseif (-not $meaningfulChange) {
            $outputPath = $currentPath
            Write-Log "Processing produced output identical to the committed snapshot. Nothing to publish."
        }
        else {
            $outputPath = $currentPath

            if ($PSCmdlet.ShouldProcess($currentPath, "Publish the processed snapshot")) {
                if (Test-Path (Join-Path $currentPath ".spec-metadata.json")) {
                    Write-Log "Rotating current snapshot into previous"

                    New-Item -ItemType Directory -Force -Path $previousPath | Out-Null
                    Remove-Item -Path (Join-Path $previousPath "*") -Recurse -Force -ErrorAction SilentlyContinue
                    Copy-Item -Path (Join-Path $currentPath "*") -Destination $previousPath -Recurse -Force
                }
                else {
                    Write-Log "No existing snapshot to rotate; the seeded baseline stays in place"
                }

                New-Item -ItemType Directory -Force -Path $currentPath | Out-Null
                Remove-Item -Path (Join-Path $currentPath "*") -Recurse -Force -ErrorAction SilentlyContinue
                Copy-Item -Path (Join-Path $stagingPath "*") -Destination $currentPath -Recurse -Force
            }
        }

        $processingSucceeded = $true
    }
    finally {
        Remove-Item -Path $downloadPath -Force -ErrorAction SilentlyContinue

        # Clear the staging directory however this run ended, so a failure does not leave a partial
        # snapshot behind in temp. A *successful* dry run is the one exception, since its staged
        # output is the deliverable and the path was reported to the caller. A failed dry run is
        # cleaned up like any other failure, so a broken local experiment cannot accumulate in temp
        # or be mistaken for valid output.

        $keepStaging = $DryRun -and $processingSucceeded

        if ($stagingPath -and (-not $keepStaging)) {
            Remove-Item -Path $stagingPath -Recurse -Force -ErrorAction SilentlyContinue
        }
    }

    Write-Log "Specification processed into: $outputPath"

    if ($env:GITHUB_OUTPUT) {
        "changed=$($meaningfulChange.ToString().ToLowerInvariant())" | Out-File -FilePath $env:GITHUB_OUTPUT -Append
        "content-hash=$contentHash" | Out-File -FilePath $env:GITHUB_OUTPUT -Append
        "output-path=$outputPath" | Out-File -FilePath $env:GITHUB_OUTPUT -Append
    }

    if ($DryRun) {
        Write-Log "Dry run complete. The committed snapshot was not modified."
    }
    else {
        Write-Log "Sync complete."
    }
}
catch {
    Write-ErrorLog "Specification sync failed: $_"
    exit 1
}
finally {
    Pop-Location
}
