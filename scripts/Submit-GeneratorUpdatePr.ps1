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
# Track if any warnings were encountered during execution
$WarningsEncountered = $false
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
    # Set the global warning flag to track that warnings occurred
    $script:WarningsEncountered = $true
}

function Write-Error-Log {
    param([string]$Message)
    Write-Host "$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss'): ERROR: $Message" -ForegroundColor Red
}

# Function to get package dependencies using npm view
function Get-PackageDependencies {
    param([string]$PackageName, [string]$PackageVersion)
    
    Write-Log "Fetching dependencies for $PackageName version $PackageVersion"
    
    try {
        # Properly format the package specification for npm view
        $packageSpec = "$PackageName@$PackageVersion"
        
        # Run npm view command and parse JSON output
        $npmViewOutput = & npm view $packageSpec devDependencies --json 2>&1
        
        # Check if there was an error
        if ($LASTEXITCODE -ne 0) {
            Write-Warning-Log "Failed to get dependencies for $PackageName version $PackageVersion. Error: $npmViewOutput"
            return $null
        }
        
        # Parse the JSON output
        $dependencies = $npmViewOutput | ConvertFrom-Json
        
        return $dependencies
    }
    catch {
        Write-Warning-Log "Error fetching dependencies for $PackageName version $PackageVersion $_"
        return $null
    }
}

$InjectedDependencies = @(
    '@azure-tools/typespec-client-generator-core',
    '@azure-tools/typespec-azure-core',
    '@typespec/http',
    '@typespec/openapi'
)

Write-Log "Starting TypeSpec generator update process"
Write-Log "Target version: $PackageVersion"
Write-Log "Repository: $RepoOwner/$RepoName"
Write-Log "Branch: $PRBranch"

try {
    Push-Location $RepoPath

    # Get current version from package.json files
    $openAiPackageJsonPath = "codegen/package.json"
    
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
    # Fetch dependencies of the http-client-csharp package
    $httpClientDependencies = Get-PackageDependencies -PackageName '@typespec/http-client-csharp' -PackageVersion $PackageVersion
    
    # Update the injected dependencies in the package.json
    if ($httpClientDependencies -ne $null) {
        Write-Log "Updating injected dependencies in OpenAI package.json"
        
        foreach ($dependency in $InjectedDependencies) {
            if ($httpClientDependencies.PSObject.Properties.Name -contains $dependency) {
                $dependencyVersion = $httpClientDependencies.$dependency
                Write-Log "Updating $dependency to version $dependencyVersion"
                
                # Update the dependency in the package.json
                if ($openAiPackageJson.dependencies.PSObject.Properties.Name -contains $dependency) {
                    $openAiPackageJson.dependencies.$dependency = $dependencyVersion
                    Write-Log "Updated $dependency to version $dependencyVersion"
                } else {
                    Write-Warning-Log "Dependency $dependency not found in package.json"
                }
            } else {
                Write-Warning-Log "Dependency $dependency not found in @typespec/http-client-csharp version $PackageVersion"
            }
        }
    } else {
        Write-Warning-Log "Could not fetch dependencies for @typespec/http-client-csharp version $PackageVersion"
    }
    

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
    
    # Delete previous package-lock.json
    Write-Log "Deleting previous package-lock.json"
    if (Test-Path "package-lock.json") {
        Remove-Item -Path "package-lock.json" -Force
    }

    # Install dependencies from root directory (using workspaces)
    Write-Log "Installing dependencies from root directory"
    npm install
    if ($LASTEXITCODE -ne 0) {
        throw "npm install failed"
    }
    
    # Build OpenAI plugin
    Write-Log "Building OpenAI plugin"
    Push-Location "codegen"
    npm run clean && npm run build
    if ($LASTEXITCODE -ne 0) {
        Write-Warning-Log "OpenAI plugin build failed, but continuing..."
    }
    Pop-Location
    
    # Regenerate OpenAI SDK code
    Write-Log "Regenerating OpenAI SDK code"
    Push-Location "."
    try {
        pwsh scripts/Invoke-CodeGen.ps1
    } catch {
        Write-Warning-Log "OpenAI code generation failed: $_"
    }
    Pop-Location

     # Export the API
    Write-Log "Updating API"
    Push-Location "."
    try {
        pwsh scripts/Export-Api.ps1
    } catch {
        Write-Warning-Log "Exporting API failed: $_"
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
    git add codegen/package.json
    git add codegen/generator/src/OpenAI.Library.Plugin.csproj
    git add api
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
    
    # Update PR title if warnings were encountered
    $prTitle = "Update @typespec/http-client-csharp to $PackageVersion"
    if ($WarningsEncountered) {
        $prTitle = "Succeeded with Issues: $prTitle"
    }
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
    # If warnings were encountered, make the script exit with non-zero code
    # This will mark the GitHub Action step as failed but still create the PR
    if ($WarningsEncountered) {
        Write-Warning-Log "Warnings were encountered during execution. PR was created but marking step as failed."
        exit 1
    }
    
} catch {
    Write-Error-Log "Error creating PR: $_"
    exit 1
} finally {
    Pop-Location
}
