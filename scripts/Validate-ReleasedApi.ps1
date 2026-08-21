<#
.SYNOPSIS
    Validates that api/released matches the API listings from the recorded release.

.DESCRIPTION
    Reads api/api-version.txt, resolves the corresponding OpenAI_<version> tag,
    and compares the release-time API listings with the current api/released
    directory. Tags created before the API directory split are read from api;
    newer tags are read from api/in-progress.

    The comparison is performed in place using Git blob IDs. The script reads
    the previous release's file paths and blob IDs directly from the local tag,
    then computes the blob IDs Git would assign to the current working-tree
    files. It does not clone the repository, switch branches, check out the tag,
    or copy tagged files. The release tag and its referenced Git objects must be
    available in the local repository.
#>

[CmdletBinding()]
param(
    [Parameter()]
    [string]$RepositoryRoot = (Join-Path $PSScriptRoot ".." -Resolve)
)

$ErrorActionPreference = "Stop"

# Keep Git invocation and error handling consistent. Returning an array is
# intentional: callers can distinguish an empty result from a single output line.
function Invoke-Git {
    param(
        [Parameter(Mandatory = $true)]
        [string[]]$Arguments
    )

    $output = @(& git -C $RepositoryRoot @Arguments 2>&1)
    if ($LASTEXITCODE -ne 0) {
        $details = ($output | ForEach-Object { $_.ToString() }) -join [Environment]::NewLine
        throw "git $($Arguments -join ' ') failed with exit code $LASTEXITCODE.$([Environment]::NewLine)$details"
    }

    return @($output | ForEach-Object { $_.ToString() })
}

# cat-file checks both commits and paths within a tag without checking anything
# out or modifying the working tree. The script uses this to determine whether a
# release tag contains the newer api/in-progress directory or the older api/<tfm>
# structure. Because one of those paths is expected to be absent, this helper
# returns a Boolean instead of throwing for a missing object.
function Test-GitObject {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Object
    )

    & git -C $RepositoryRoot cat-file -e $Object 2>$null
    return $LASTEXITCODE -eq 0
}

$RepositoryRoot = (Resolve-Path $RepositoryRoot).Path
$versionPath = Join-Path $RepositoryRoot "api" "api-version.txt"
$releasedDirectory = Join-Path $RepositoryRoot "api" "released"

if (-not (Test-Path $versionPath -PathType Leaf)) {
    throw "API version file was not found: $versionPath"
}

$apiVersion = (Get-Content $versionPath -Raw).Trim()
if ($apiVersion -notmatch '^\d+\.\d+\.\d+(?:-[0-9A-Za-z.-]+)?$') {
    throw "API version '$apiVersion' is empty or is not a valid release version."
}

if (-not (Test-Path $releasedDirectory -PathType Container)) {
    throw "Released API directory was not found: $releasedDirectory"
}

# api-version.txt is the source of truth for the archived snapshot. Resolve the
# fully qualified tag reference as a commit so a similarly named branch cannot
# satisfy the check. A shallow checkout may not contain this older tag reference
# and commit locally even when the tag exists on the remote; in that case, fail
# with a message telling the caller to make the repository tags available.
$releaseTag = "OpenAI_$apiVersion"
$tagReference = "refs/tags/$releaseTag"
if (-not (Test-GitObject "$tagReference^{commit}")) {
    throw "Release tag '$releaseTag' was not found. Ensure the repository tags are available."
}

# The in-progress/released split was introduced after 2.13.0. Newer release tags
# store their release-time listings under api/in-progress, while older tags store
# the same listings directly under api. Detect the tree shape rather than
# hard-coding a version boundary so the fallback naturally expires.
$taggedSourceRoot = if (Test-GitObject "${releaseTag}:api/in-progress") {
    "api/in-progress"
} else {
    "api"
}

# Read each tagged .cs entry into a map where the key is its path below the API
# listing root and the value is Git's blob ID for its contents. For example,
# api/net8.0/OpenAI.Chat.net8.0.cs in a legacy tag and
# api/in-progress/net8.0/OpenAI.Chat.net8.0.cs in a newer tag both become the key
# net8.0/OpenAI.Chat.net8.0.cs. The blob ID is later compared with the Git hash of
# the corresponding api/released file, so no tagged files need to be checked out
# or copied to a temporary directory.
$taggedFiles = [System.Collections.Generic.Dictionary[string, string]]::new([System.StringComparer]::Ordinal)

# `git ls-tree` reads directory entries directly from the release tag:
#   - `-r` recursively includes files in target-framework subdirectories.
#   - `--full-tree` prints each path from the repository root rather than relative
#     to the path argument.
#   - `--format` prints the blob ID, a tab (`%x09`), and the repository path.
#   - `$releaseTag` identifies the Git tree to inspect.
#   - `--` ends Git's options, so `$taggedSourceRoot` is interpreted only as the
#     path within that tree whose entries should be listed.
# A resulting line resembles:
#   abc123...<tab>api/in-progress/net8.0/OpenAI.Chat.net8.0.cs
# The loop below splits that line at the tab into the blob ID and path.
$treeLines = Invoke-Git @(
    "ls-tree",
    "-r",
    "--full-tree",
    "--format=%(objectname)%x09%(path)",
    $releaseTag,
    "--",
    $taggedSourceRoot
)

foreach ($line in $treeLines) {
    $parts = $line -split "`t", 2
    if ($parts.Count -ne 2 -or -not $parts[1].EndsWith(".cs", [System.StringComparison]::OrdinalIgnoreCase)) {
        continue
    }

    $taggedRelativePath = $parts[1].Substring($taggedSourceRoot.Length).TrimStart("/")
    if (-not $taggedFiles.TryAdd($taggedRelativePath, $parts[0])) {
        throw "Duplicate tagged API listing path was found: $taggedRelativePath"
    }
}

if ($taggedFiles.Count -eq 0) {
    throw "No API listings were found under '$taggedSourceRoot' in tag '$releaseTag'."
}

# Build the matching map for the current files under api/released. Each key is
# the path below api/released, such as net8.0/OpenAI.Chat.net8.0.cs, and each
# value is the blob ID Git would assign if that working-tree file were committed.
# `git hash-object --path` applies the attributes and clean filters configured for
# the repository path before calculating the ID. For example, a file checked out
# with CRLF can therefore match the tag's LF blob when Git is configured to
# normalize line endings; any difference in the normalized file content still
# produces a different blob ID.
$releasedFiles = [System.Collections.Generic.Dictionary[string, string]]::new([System.StringComparer]::Ordinal)
foreach ($file in Get-ChildItem $releasedDirectory -Filter "*.cs" -File -Recurse) {
    $releasedRelativePath = [System.IO.Path]::GetRelativePath($releasedDirectory, $file.FullName).Replace("\", "/")
    $repositoryPath = [System.IO.Path]::GetRelativePath($RepositoryRoot, $file.FullName).Replace("\", "/")
    $hashOutput = @(Invoke-Git @("hash-object", "--path=$repositoryPath", "--", $file.FullName))
    if ($hashOutput.Count -ne 1) {
        throw "Expected one Git hash for '$repositoryPath' but received $($hashOutput.Count)."
    }
    $hash = $hashOutput[0]

    if (-not $releasedFiles.TryAdd($releasedRelativePath, $hash)) {
        throw "Duplicate released API listing path was found: $releasedRelativePath"
    }
}

if ($releasedFiles.Count -eq 0) {
    throw "No API listings were found under '$releasedDirectory'."
}

# The dictionary keys align files that occupy the same relative path in the tag
# and api/released. First compare all keys to find listings missing from or added
# to api/released. Then, for keys present in both maps, compare the blob IDs to
# find files whose normalized contents differ. Keeping these checks separate
# ensures a missing or extra file cannot be hidden by the content comparison.
$missingFiles = @($taggedFiles.Keys | Where-Object { -not $releasedFiles.ContainsKey($_) } | Sort-Object)
$unexpectedFiles = @($releasedFiles.Keys | Where-Object { -not $taggedFiles.ContainsKey($_) } | Sort-Object)
$modifiedFiles = @(
    $taggedFiles.Keys |
        Where-Object { $releasedFiles.ContainsKey($_) -and $releasedFiles[$_] -ne $taggedFiles[$_] } |
        Sort-Object
)

if ($missingFiles.Count -gt 0 -or $unexpectedFiles.Count -gt 0 -or $modifiedFiles.Count -gt 0) {
    Write-Error "Released API listings do not match tag '$releaseTag' ($taggedSourceRoot)." -ErrorAction Continue

    if ($missingFiles.Count -gt 0) {
        Write-Host "Missing released API listings:"
        $missingFiles | ForEach-Object { Write-Host "  - $_" }
    }
    if ($unexpectedFiles.Count -gt 0) {
        Write-Host "Unexpected released API listings:"
        $unexpectedFiles | ForEach-Object { Write-Host "  - $_" }
    }
    if ($modifiedFiles.Count -gt 0) {
        Write-Host "Modified released API listings:"
        $modifiedFiles | ForEach-Object { Write-Host "  - $_" }
    }

    exit 1
}

Write-Host "Released API listings match '$releaseTag' ($taggedSourceRoot)." -ForegroundColor Green