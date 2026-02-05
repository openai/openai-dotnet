[CmdletBinding()]
param(
    [string]$Path
)

# If this is not a real path, use the git root so the tool can find both
# the snippet source files (in /tests) and the markdown files (README.md at root).
if (-not $Path) {
    $_gitRoot = git rev-parse --show-toplevel 2>$null

    if ((-not $_gitRoot) -or ($LASTEXITCODE -ne 0)) {
        Write-Error "Could not determine the git root directory. Please specify the path to the project file."
        exit 1
    }

    $Path = (Resolve-Path -LiteralPath $_gitRoot).Path
}

# Update snippets
Write-Host "Updating snippets in $Path"

dotnet tool restore
dotnet tool run snippet-generator -b $Path