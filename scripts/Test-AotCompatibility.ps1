$repoRootPath = Join-Path $PSScriptRoot .. -Resolve
$expectedWarningsFilePath = Join-Path -Path $repoRootPath "src\ExpectedWarnings.txt"
$libraryCsprojFilePath = Join-Path $repoRootPath "src\OpenAI.csproj"
$packageName = "OpenAI"

Write-Host "Creating test app..."

$tempFolderPath = Join-Path $repoRootPath "\TempAotCompatibility"
New-Item -ItemType Directory -Path $tempFolderPath | Out-Null
Push-Location $tempFolderPath

$tempCsprojFilePath = Join-Path $tempFolderPath "AotCompatibility.csproj"
$tempCsprojFileContent = @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <IsTestSupportProject>true</IsTestSupportProject>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="$libraryCsprojFilePath" />
  </ItemGroup>

  <PropertyGroup>
    <PublishAot>true</PublishAot>
    <TrimmerSingleWarn>false</TrimmerSingleWarn>
  </PropertyGroup>

  <ItemGroup>
    <TrimmerRootAssembly Include="$packageName" />
  </ItemGroup>
</Project>
"@
$tempCsprojFileContent | Set-Content -Path $tempCsprojFilePath

$tempProgramFilePath = Join-Path $tempFolderPath "Program.cs"
$tempProgramFileContent = @"
using $packageName;
using System;
namespace AotCompatibility
{
    internal class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Hello, World!");
        }
    }
}
"@
$tempProgramFileContent | Set-Content -Path $tempProgramFilePath

try {
    Write-Host
    Write-Host "Publishing test app..."

    dotnet clean AotCompatibility.csproj | Out-Null
    dotnet restore AotCompatibility.csproj | Out-Null
    $publishOutput = dotnet publish AotCompatibility.csproj -nodeReuse:false /p:UseSharedCompilation=false /p:ExposeExperimentalFeatures=true

    if ($LASTEXITCODE -eq 0) {
        Write-Host
        Write-Host "Parsing the result for any reported warnings..."
        $actualWarningCount = 0
        foreach ($line in $($publishOutput -split "`r`n")) {
            if ($line -like "*analysis warning IL*")
            {
                $actualWarningCount += 1
            }
        }

        Write-Host "Found $actualWarningCount warnings reported while publishing."

        $expectedWarningCount = 0
        $unexpectedWarningCount = 0

        if (Test-Path $expectedWarningsFilePath -PathType Leaf) {
            # Read the contents of the file and store each line in an array
            $expectedWarnings = Get-Content -Path $expectedWarningsFullPath
            $expectedWarningCount = $expectedWarnings.Count

            ### Comparing expected warnings to the publish output ###
            Write-Host "Found a list of expected warnings containing $expectedWarningCount expected warnings."
            Write-Host "Comparing the reported warnings against the list of expected warnings..."
            $unexpectedWarnings = $publishOutput -split "`n" | select-string -pattern 'IL\d+' | select-string -pattern '##' -notmatch | select-string -pattern $expectedWarnings -notmatch
            $unexpectedWarningCount = $unexpectedWarnings.Count
        } 
        else {
            # If no correct expected warnings were provided, check that there are no warnings reported.
            Write-Host "Could not find a list of expected warnings, therefore assuming that all reported warnings are unexpected."
            $unexpectedWarnings = $publishOutput -split "`n" | select-string -pattern 'IL\d+' | select-string -pattern '##' -notmatch
            $unexpectedWarningCount = $unexpectedWarnings.Count
        }

        if ($unexpectedWarningCount -gt 0) {
            Write-Host
            Write-Host "Found $unexpectedWarningCount unexpected warnings."
            foreach ($unexpectedWarning in $unexpectedWarnings) {
                Write-Host
                Write-Host $unexpectedWarning
            }
        }
        else {
            Write-Host
            Write-Host "No unexpected warnings found."
        }

        if (($expectedWarningCount -gt 0) -and ($expectedWarningCount -ne $actualWarningCount)) {
            Write-Host
            throw "The number of expected warnings ($expectedWarningCount) was different than the number of reported warnings ($actualWarningCount).`nFor help with this check, please see https://github.com/Azure/azure-sdk-for-net/tree/main/doc/dev/AotRegressionChecks.md."
        }
        elseif ($unexpectedWarningCount -gt 0) {
            Write-Host
            throw "The number of unexpected warnings ($unexpectedWarningCount) was greater than zero.`nFor help with this check, please see https://github.com/Azure/azure-sdk-for-net/tree/main/doc/dev/AotRegressionChecks.md."
        }
    }
    else {
        Write-Host
        Write-Host $publishOutput

        Write-Host
        throw "Publishing failed.`nFor help with this check, please see https://github.com/Azure/azure-sdk-for-net/tree/main/doc/dev/AotRegressionChecks.md."
    }
}
finally {
    Write-Host
    Write-Host "Deleting test app and cleaning up..."
    Pop-Location
    Remove-Item -Path $tempFolderPath -Recurse -Force
}