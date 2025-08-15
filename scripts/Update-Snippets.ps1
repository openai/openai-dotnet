[CmdletBinding()]
param(
    [string]$Path
)

# If this is not a real path, assume snippets should be refreshed in a relative
# path to the OpenAI.csproj file from the git root.
if (-not $Path) {
    $_gitRoot = git rev-parse --show-toplevel 2>$null

    if ((-not $_gitRoot) -or ($LASTEXITCODE -ne 0)) {
        Write-Error "Could not determine the git root directory. Please specify the path to the project file."
        exit 1
    }

    $Path = Join-Path -Path $_gitRoot -ChildPath '/src/OpenAI.csproj'
    $Path = (Resolve-Path -LiteralPath $Path).Path
}

# Update snippets
Write-Host "Updating snippets in $Path"

dotnet tool restore
dotnet tool run snippet-generator -b $Path $additionalArgs