#!/usr/bin/env pwsh

<#
.DESCRIPTION
Creates a pull request to update the @typespec/http-client-csharp dependency in the OpenAI SDK for .NET repository.
This script follows the pattern used by the TypeSpec repository for creating PRs in downstream repositories.

.PARAMETER PackageVersion
The version of the @typespec/http-client-csharp package to update to.

.PARAMETER AuthToken
A GitHub personal access token for authentication.

.PARAMETER BranchName
The name of the branch to create in the repository.

.PARAMETER RepoPath
The path to the local repository. Defaults to current directory.

.EXAMPLE
# Update to a specific version
./Submit-GeneratorUpdatePr.ps1 -PackageVersion "1.0.0-alpha.20250625.4" -AuthToken "ghp_xxxx"
#>
[CmdletBinding(SupportsShouldProcess = $true)]
param(
  [Parameter(Mandatory = $true)]
  [string]$PackageVersion,

  [Parameter(Mandatory = $true)]
  [string]$AuthToken,

  [Parameter(Mandatory = $false)]
  [string]$BranchName = "typespec/update-http-client-csharp-$PackageVersion",

  [Parameter(Mandatory = $false)]
  [string]$RepoPath = "."
)

# Set up variables for the PR
$RepoOwner = "openai"
$RepoName = "openai-dotnet"
$BaseBranch = "main"
$PRBranch = $BranchName

function Write-Log {
    param([string]$Message)
    Write-Host "$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss'): $Message" -ForegroundColor Green
}

function Write-Warning-Log {
    param([string]$Message)
    Write-Host "$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss'): WARNING: $Message" -ForegroundColor Yellow
}

function Write-Error-Log {
    param([string]$Message)
    Write-Host "$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss'): ERROR: $Message" -ForegroundColor Red
}

Write-Log "Starting TypeSpec generator update process"
Write-Log "Target version: $PackageVersion"
Write-Log "Repository: $RepoOwner/$RepoName"
Write-Log "Branch: $PRBranch"

try {
    Push-Location $RepoPath

    # Get current version from package.json files
    $openAiPackageJsonPath = "package.json"
    
    if (-not (Test-Path $openAiPackageJsonPath)) {
        throw "OpenAI package.json not found at: $openAiPackageJsonPath"
    }
    
    # Read current versions
    $openAiPackageJson = Get-Content $openAiPackageJsonPath -Raw | ConvertFrom-Json
    
    $currentVersion = $openAiPackageJson.dependencies.'@typespec/http-client-csharp'
    
    Write-Log "Current OpenAI version: $currentVersion"
    
    # Check if update is needed
    if ($currentVersion -eq $PackageVersion) {
        Write-Log "No update needed. Already at version: $PackageVersion"
        return
    }
    
    Write-Log "Update needed: $currentVersion -> $PackageVersion"
    
    # Create a new branch
    Write-Log "Creating branch: $PRBranch"
    git checkout -b $PRBranch
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to create branch: $PRBranch"
    }
    
    # Update OpenAI package.json
    Write-Log "Updating OpenAI package.json"
    $openAiPackageJson.dependencies.'@typespec/http-client-csharp' = $PackageVersion
    $openAiPackageJson | ConvertTo-Json -Depth 10 | Set-Content -Path $openAiPackageJsonPath

    # Update Microsoft.TypeSpec.Generator.ClientModel version in csproj files
    $openAiCsprojPath = "codegen/generator/src/OpenAI.Library.Plugin.csproj"
    
    Write-Log "Updating Microsoft.TypeSpec.Generator.ClientModel version in csproj files"
    
    # Update OpenAI csproj
    if (Test-Path $openAiCsprojPath) {
        $openAiCsproj = Get-Content $openAiCsprojPath -Raw
        $openAiCsproj = $openAiCsproj -replace '(<PackageReference Include="Microsoft\.TypeSpec\.Generator\.ClientModel" Version=")[^"]*(")', "`${1}$PackageVersion`${2}"
        Set-Content -Path $openAiCsprojPath -Value $openAiCsproj
        Write-Log "Updated OpenAI csproj: $openAiCsprojPath"
    } else {
        Write-Warning-Log "OpenAI csproj not found at: $openAiCsprojPath"
    }
    
    # Install dependencies from root (using workspaces)
    Write-Log "Installing dependencies from root (using npm workspaces)"
    npm install
    if ($LASTEXITCODE -ne 0) {
        throw "npm install failed"
    }
    
    # Build OpenAI plugin
    Write-Log "Building OpenAI plugin"
    Push-Location "."
    npm run clean && npm run build
    if ($LASTEXITCODE -ne 0) {
        Write-Warning-Log "OpenAI plugin build failed, but continuing..."
    }
    Pop-Location
    
    # Regenerate OpenAI SDK code
    Write-Log "Regenerating OpenAI SDK code"
    Push-Location "."
    try {
        pwsh .scripts/Invoke-CodeGen.ps1
    } catch {
        Write-Warning-Log "OpenAI code generation failed: $_"
    }
    Pop-Location
    
    # Check if there are changes to commit
    $gitStatus = git status --porcelain
    if (-not $gitStatus) {
        Write-Log "No changes detected. Skipping commit and PR creation."
        return
    }
    
    # Configure git
    git config --local user.email "action@github.com"
    git config --local user.name "GitHub Action"
    
    # Add and commit changes
    Write-Log "Adding and committing changes"
    git add package.json
    git add codegen/generator/src/OpenAI.Library.Plugin.csproj
    git add package-lock.json
    git add ./ # Add any generated code changes
    
    $commitMessage = @"
Update @typespec/http-client-csharp to $PackageVersion

- Updated @typespec/http-client-csharp from $currentVersion to $PackageVersion
- Updated Microsoft.TypeSpec.Generator.ClientModel from $currentVersion to $PackageVersion
- Regenerated OpenAI SDK code with new generator version
- Updated centrally managed package-lock.json file with new dependency versions
"@
    
    git commit -m $commitMessage
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to commit changes"
    }
    
    # Push the branch
    Write-Log "Pushing branch to remote"
    git push origin $PRBranch
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to push branch"
    }
    
    # Create PR using GitHub CLI
    Write-Log "Creating PR using GitHub CLI"
    $env:GH_TOKEN = $AuthToken
    
    $prTitle = "Update @typespec/http-client-csharp to $PackageVersion"
    $prBody = @"
This PR automatically updates the TypeSpec HTTP client C# generator version and regenerates the SDK code.

## Changes
- Updated ``@typespec/http-client-csharp`` from ``$currentVersion`` to ``$PackageVersion``
- Updated ``Microsoft.TypeSpec.Generator.ClientModel`` from ``$currentVersion`` to ``$PackageVersion``
- Updated OpenAI plugin package.json file
- Updated OpenAI plugin csproj file
- Regenerated OpenAI SDK code using the new generator version
- Updated centrally managed package-lock.json file with new dependency versions

## Details
- Generator package: [@typespec/http-client-csharp](https://www.npmjs.com/package/@typespec/http-client-csharp)
- Version update: ``$currentVersion`` → ``$PackageVersion``

## Testing
Please run the existing test suites to ensure the generated code works correctly:
- Build and test the OpenAI SDK
- Verify API compatibility and functionality

## Notes
This PR was created automatically by the **Update TypeSpec Generator Version** workflow. The workflow runs weekly and when manually triggered to keep the generator version current with the latest TypeSpec improvements and fixes.

If there are any issues with the generated code, please review the [TypeSpec release notes](https://github.com/microsoft/typespec/releases) for breaking changes or new features that may require manual adjustments.
"@
    
    $prUrl = gh pr create --title $prTitle --body $prBody --base $BaseBranch --head $PRBranch 2>&1
    
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to create PR using gh CLI: $prUrl"
    }
    
    Write-Log "Successfully created PR: $prUrl"
    
} catch {
    Write-Error-Log "Error creating PR: $_"
    exit 1
} finally {
    Pop-Location
}