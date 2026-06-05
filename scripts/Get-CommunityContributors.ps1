<#
.SYNOPSIS
    Identifies community contributors since the last release.

.DESCRIPTION
    Looks at commits pushed to a given branch since the last OpenAI_*.*.* release tag,
    prints commits authored by community contributors (non-collaborators), and returns
    a deduplicated list of their GitHub usernames.

    Requires the GitHub CLI (gh) to be installed and authenticated via `gh auth login`.

.PARAMETER Owner
    The GitHub repository owner. Defaults to "openai".

.PARAMETER Repository
    The GitHub repository name. Defaults to "openai-dotnet".

.PARAMETER Branch
    The branch to compare against the last release tag. Defaults to "main".

.EXAMPLE
    ./scripts/Get-CommunityContributors.ps1

.EXAMPLE
    ./scripts/Get-CommunityContributors.ps1 -Owner "openai" -Repository "openai-dotnet" -Branch "main"

.EXAMPLE
    $contributors = ./scripts/Get-CommunityContributors.ps1
    $contributors | ForEach-Object { Write-Host "Thank you, $_!" }
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory = $false)]
    [string]$Owner = "openai",

    [Parameter(Mandatory = $false)]
    [string]$Repository = "openai-dotnet",

    [Parameter(Mandatory = $false)]
    [string]$Branch = "main",

    [Parameter(Mandatory = $false)]
    [string[]]$ExcludeUsers = @("github-actions[bot]", "Copilot")
)

$ErrorActionPreference = "Stop"

# --- Helper functions ---

function Write-Log {
    param([string]$Message)
    Write-Host "$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss'): $Message"
}

function Write-Section {
    param([string]$Message)
    Write-Host ""
    Write-Host $Message -ForegroundColor Cyan
    Write-Host ("-" * $Message.Length) -ForegroundColor Cyan
}

# --- Phase 1: Verify gh CLI is available ---

$ghCommand = Get-Command gh -ErrorAction SilentlyContinue
if (-not $ghCommand) {
    throw "The GitHub CLI (gh) is not installed or not in PATH. Install it from https://cli.github.com/"
}

# --- Phase 2: Find the latest release tag ---

Write-Section "Finding latest release tag"

$tagsJson = gh api "repos/$Owner/$Repository/tags" --paginate 2>&1
if ($LASTEXITCODE -ne 0) {
    throw "Failed to fetch tags: $tagsJson"
}

$tags = $tagsJson | ConvertFrom-Json
$releaseTags = $tags | Where-Object { $_.name -match '^OpenAI_\d+\.\d+\.\d+$' }

if (-not $releaseTags -or $releaseTags.Count -eq 0) {
    throw "No release tags matching 'OpenAI_*.*.*' found in $Owner/$Repository"
}

# Sort by semantic version (descending) to find the latest
$latestTag = $releaseTags | Sort-Object {
    $version = $_.name -replace '^OpenAI_', ''
    $parts = $version.Split('.')
    [int]$parts[0] * 1000000 + [int]$parts[1] * 1000 + [int]$parts[2]
} -Descending | Select-Object -First 1

Write-Log "Latest release tag: $($latestTag.name)"

# --- Phase 3: Get commits since the tag ---

Write-Section "Fetching commits on '$Branch' since '$($latestTag.name)'"

$compareJson = gh api "repos/$Owner/$Repository/compare/$($latestTag.name)...$Branch" --paginate 2>&1
if ($LASTEXITCODE -ne 0) {
    throw "Failed to compare commits: $compareJson"
}

$compareData = $compareJson | ConvertFrom-Json

$totalCommits = $compareData.total_commits
Write-Log "Total commits since last release: $totalCommits"

if ($totalCommits -gt 250) {
    Write-Host "WARNING: GitHub compare API returns at most 250 commits. Only the first 250 will be analyzed." -ForegroundColor Yellow
}

$commits = $compareData.commits
if (-not $commits -or $commits.Count -eq 0) {
    Write-Log "No commits found since last release."
    return @()
}

# --- Phase 4: Identify unique authors and check collaborator status ---

Write-Section "Checking collaborator status for commit authors"

# Collect unique usernames from commits (skip null authors and excluded users)
$authorCommits = @{}  # username -> list of commits
foreach ($commit in $commits) {
    $login = $commit.author.login
    if (-not $login) {
        continue
    }
    if ($ExcludeUsers -contains $login) {
        continue
    }
    if (-not $authorCommits.ContainsKey($login)) {
        $authorCommits[$login] = @()
    }
    $authorCommits[$login] += $commit
}

Write-Log "Found $($authorCommits.Count) unique author(s) across $($commits.Count) commits"

# Check each unique author against the collaborators API
$communityContributors = @()
$collaborators = @()

foreach ($username in $authorCommits.Keys) {
    # gh api returns exit code 0 for 204 (is collaborator) and non-zero for 404 (not collaborator)
    $null = gh api "repos/$Owner/$Repository/collaborators/$username" 2>&1
    if ($LASTEXITCODE -eq 0) {
        $collaborators += $username
        Write-Host "  [collaborator] $username" -ForegroundColor DarkGray
    }
    else {
        $communityContributors += $username
        Write-Host "  [community]    $username" -ForegroundColor Green
    }
}

# --- Phase 5: Output ---

Write-Section "Community contributor commits"

if ($communityContributors.Count -eq 0) {
    Write-Log "No community contributor commits found since '$($latestTag.name)'."
    return @()
}

foreach ($username in $communityContributors) {
    foreach ($commit in $authorCommits[$username]) {
        $sha = $commit.sha.Substring(0, 7)
        $message = $commit.commit.message.Split("`n")[0]  # first line only
        Write-Host "  $sha " -ForegroundColor Yellow -NoNewline
        Write-Host "($username) " -ForegroundColor Green -NoNewline
        Write-Host $message
    }
}

Write-Section "Community contributors ($($communityContributors.Count) unique)"

foreach ($username in $communityContributors) {
    Write-Host "  - $username" -ForegroundColor Green
}

Write-Host ""

# Return deduplicated username list to the pipeline
return [string[]]$communityContributors
