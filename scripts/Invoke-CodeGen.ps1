[CmdletBinding(DefaultParameterSetName = 'GitHub')]
param(
    [Parameter(Mandatory = $true, ParameterSetName = 'GitHub')]
    [string]$GitHubOwner,

    [Parameter(Mandatory = $true, ParameterSetName = 'GitHub')]
    [string]$GitHubRepository,

    [Parameter(Mandatory = $false, ParameterSetName = 'GitHub')]
    [string]$CommitHash = "fa5b2820354fa4fb62636f1ea6abd8a5a6d39bf7",

    [Parameter(Mandatory = $false, ParameterSetName = 'GitHub')]
    [string]$GitHubToken,

    [Parameter(Mandatory = $true, ParameterSetName = 'Local')]
    [string]$LocalRepositoryPath,

    [Parameter(Mandatory = $false)]
    [switch]$Force
)

function Invoke-ScriptWithLogging {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [scriptblock]$Script
    )

    $scriptString = $Script | Out-String
    Write-Host "--------------------------------------------------------------------------------`n> $scriptString"
    & $Script
    Write-Host ""
}

function Get-GitHubApiHeaders {
    param(
        [Parameter(Mandatory = $false)]
        [string]$GitHubToken
    )

    $headers = @{
        'Accept' = 'application/vnd.github+json'
        'X-GitHub-Api-Version' = '2022-11-28'
    }

    if ($GitHubToken) {
        $headers.Authorization = "Bearer $GitHubToken"
    }

    return $headers
}

function Get-TempDownloadPath {
    $tempPath = [System.IO.Path]::GetTempPath()
    return Join-Path $tempPath ([System.IO.Path]::GetRandomFileName())
}

function Test-GitHubRepoExists {
    param(
        [Parameter(Mandatory = $true)]
        [string]$GitHubOwner,

        [Parameter(Mandatory = $true)]
        [string]$GitHubRepository,

        [Parameter(Mandatory = $false)]
        [string]$GitHubToken
    )

    $apiUrl = "https://api.github.com/repos/$GitHubOwner/$GitHubRepository"
    try {
        $headers = Get-GitHubApiHeaders -GitHubToken $GitHubToken
        Invoke-RestMethod -Uri $apiUrl -Method Get -Headers $headers -ErrorAction Stop
        return $true
    }
    catch {
        Write-Warning "Repository check failed: $_"
        return $false
    }
}

function Get-GitHubRepoContent {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$GitHubOwner,

        [Parameter(Mandatory = $true)]
        [string]$GitHubRepository,

        [Parameter(Mandatory = $true)]
        [string]$CommitHash,

        [Parameter(Mandatory = $false)]
        [string]$GitHubToken,

        [Parameter(Mandatory = $true)]
        [string]$SubdirectoryPath,

        [Parameter(Mandatory = $true)]
        [string]$Destination
    )

    # Validate if the repository exists
    if (-not (Test-GitHubRepoExists -GitHubOwner $GitHubOwner -GitHubRepository $GitHubRepository -GitHubToken $GitHubToken)) {
        Write-Error "Repository '$GitHubOwner/$GitHubRepository' does not exist or is not accessible."
        return $false
    }

    # Create temporary directory for download
    $downloadPath = Get-TempDownloadPath
    New-Item -ItemType Directory -Path $downloadPath -Force | Out-Null

    try {
        # Construct the download URL using GitHub API endpoint
        $archiveUrl = "https://api.github.com/repos/$GitHubOwner/$GitHubRepository/zipball/$CommitHash"
        $zipPath = Join-Path $downloadPath "repo.zip"
        
        # Get GitHub API headers with optional token
        $headers = Get-GitHubApiHeaders -GitHubToken $GitHubToken

        # Download the repository
        Write-Host "Downloading repository archive from $GitHubOwner/$GitHubRepository @ $CommitHash..."
        Write-Host ""
        Invoke-WebRequest -Uri $archiveUrl -OutFile $zipPath -Headers $headers

        # Extract the contents
        Write-Host "Extracting repository contents..."
        Write-Host ""
        Expand-Archive -Path $zipPath -DestinationPath $downloadPath

        # Get the extracted folder name (it will be repository-commithash)
        $extractedFolder = Get-ChildItem -Path $downloadPath -Directory | Select-Object -First 1

        # Create the destination directory if it doesn't exist
        New-Item -ItemType Directory -Path $Destination -Force | Out-Null

        $sourcePath = Join-Path $extractedFolder.FullName $SubdirectoryPath
        if (-not (Test-Path $sourcePath)) {
            Write-Error "Specified path '$SubdirectoryPath' does not exist in the repository."
            return $false
        }

        # Copy the contents directly to destination, preserving only the internal structure
        if (Test-Path $sourcePath -PathType Container) {
            Copy-Item -Path "$sourcePath\*" -Destination $Destination -Recurse -Force
        }
        else {
            Copy-Item -Path $sourcePath -Destination $Destination -Force
        }

        Write-Host "Downloaded repository contents to: $Destination"
        Write-Host ""
        return $true
    }
    catch {
        Write-Error "An error occurred: $_"
        return $false
    }
    finally {
        # Cleanup temporary files
        if (Test-Path $downloadPath) {
            Remove-Item -Path $downloadPath -Recurse -Force
        }
    }
}

function Get-LocalRepoContent {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$LocalRepositoryPath,

        [Parameter(Mandatory = $true)]
        [string]$SubdirectoryPath,

        [Parameter(Mandatory = $true)]
        [string]$Destination
    )

    try {
        $sourcePath = Join-Path $LocalRepositoryPath $SubdirectoryPath

        if (-not (Test-Path $sourcePath)) {
            Write-Error "Specified path '$SubdirectoryPath' does not exist in the local repository at: $LocalRepositoryPath"
            return $false
        }

        # Create the destination directory if it doesn't exist
        New-Item -ItemType Directory -Path $Destination -Force | Out-Null

        # Copy the contents directly to destination, preserving only the internal structure
        if (Test-Path $sourcePath -PathType Container) {
            Copy-Item -Path "$sourcePath\*" -Destination $Destination -Recurse -Force
        }
        else {
            Copy-Item -Path $sourcePath -Destination $Destination -Force
        }

        Write-Host "Copied repository contents to: $Destination"
        Write-Host ""
        return $true
    }
    catch {
        Write-Error "An error occurred: $_"
        return $false
    }
}

$repoRootPath = Join-Path $PSScriptRoot .. -Resolve
$specificationFolderPath = Join-Path $repoRootPath "specification"
$baseSpecificationFolderPath = Join-Path $repoRootPath "specification\base"
$codegenFolderPath = Join-Path $repoRootPath "codegen"

$scriptStartTime = Get-Date

$shouldDownload = $true
if (Test-Path $baseSpecificationFolderPath) {
    Write-Host "Base specification already exists at: $baseSpecificationFolderPath"
    Write-Host ""

    if ($Force) {
        Write-Host "Overwriting existing base specification..."
        Write-Host ""
        Remove-Item -Path $baseSpecificationFolderPath -Recurse -Force
    }
    else {
        $shouldDownload = $false
    }
}

if ($shouldDownload) {
    Write-Host "Retrieving base specification..."
    Write-Host ""

    if ($PSCmdlet.ParameterSetName -eq 'GitHub') {
        $success = Get-GitHubRepoContent -GitHubOwner $GitHubOwner `
            -GitHubRepository $GitHubRepository `
            -CommitHash $CommitHash `
            -GitHubToken $GitHubToken `
            -SubdirectoryPath "openai-in-typespec" `
            -Destination $baseSpecificationFolderPath
    }
    elseif ($PSCmdlet.ParameterSetName -eq 'Local') {
        $success = Get-LocalRepoContent -LocalRepositoryPath $LocalRepositoryPath `
            -SubdirectoryPath "openai-in-typespec" `
            -Destination $baseSpecificationFolderPath
    }

    if (-not $success) {
        Write-Error "Failed to get repository contents."
        throw "Repository content retrieval failed."
    }
}

Push-Location $repoRootPath

try {
    Invoke-ScriptWithLogging { npm ci }
    Invoke-ScriptWithLogging { npm run build -w $codegenFolderPath }

    Set-Location $specificationFolderPath
    Invoke-ScriptWithLogging { npm exec --no -- tsp compile . }
}
finally {
    Pop-Location
}

$scriptElapsed = $(Get-Date) - $scriptStartTime
$scriptElapsedSeconds = [math]::Round($scriptElapsed.TotalSeconds, 1)
$scriptName = $MyInvocation.MyCommand.Name

Write-Host "${scriptName} completed. Time: ${scriptElapsedSeconds}s"
Write-Host ""