# OpenAI with .NET 10 - Getting Started Guide

This readme shows you how to run each OpenAI based sample (.cs) file in this folder directly without a project or additional setup using the latest .NET 10 features.

## Prerequisites

### 1. Install .NET 10

#### Option A: Using package manager - Recommended

- Windows

    ```powershell
    # Install the .NET 10 SDK
    winget install --id Microsoft.DotNet.SDK.10 --exact
    ```

- macOS

    ```bash
    # Install the .NET 10 SDK
    brew install --cask dotnet-sdk
    ```

    If Homebrew installs a newer major version instead of a compatible .NET 10 SDK, use the manual download option below.

#### Option B: Manual download

1. Visit the [.NET 10 Download Page](https://dotnet.microsoft.com/download/dotnet/10.0)
1. Download and install: **.NET SDK 10.0** (required for development and `dotnet run`)

### 2. Verify installation

After installation, verify that the required SDK is available:

```powershell
# Check installed SDKs
dotnet --list-sdks
```

The output should include version 10.0.400 or a compatible later SDK, for example:

```text
10.0.400
```

## Setup

### 1. Clone the repository

```powershell
git clone https://github.com/openai/openai-dotnet.git
cd openai-dotnet
```

### 2. Set your OpenAI API key

You need an OpenAI API key to run the samples. Get one from [OpenAI's API platform](https://platform.openai.com/api-keys).

#### Temporary (Current session only)

```bash
# bash/zsh
export OPENAI_API_KEY="your-api-key-here"
```

```powershell
# PowerShell
$env:OPENAI_API_KEY = "your-api-key-here"
```

#### Permanent options

**Option A: Using System Properties (GUI)**

1. Press `Win + R`, type `sysdm.cpl`, press Enter
2. Click "Environment Variables"
3. Under "User variables", click "New"
4. Variable name: `OPENAI_API_KEY`
5. Variable value: Your API key

**Option B: Using PowerShell (Permanent)**

```powershell
[Environment]::SetEnvironmentVariable("OPENAI_API_KEY", "your-api-key-here", "User")
```

**Option C: Using Command Prompt as Administrator**

```cmd
setx OPENAI_API_KEY "your-api-key-here"
```

**Option D: Using bash/zsh**

```bash
# bash
echo 'export OPENAI_API_KEY=\"your-api-key-here\"' >> ~/.bashrc
source ~/.bashrc
```

```bash
# zsh
echo 'export OPENAI_API_KEY=\"your-api-key-here\"' >> ~/.zshrc
source ~/.zshrc
```

### 3. Verify environment variable

```bash
# bash/zsh
echo $OPENAI_API_KEY
```

```powershell
# PowerShell
echo $env:OPENAI_API_KEY
```

## Running the samples

The samples use .NET 10's file-based apps feature, which runs a C# file without a project file. Each `.cs` file under the `docs` folder is a standalone application.

### 1. Navigate to the docs directory

```powershell
cd docs
```

### 2. Run a sample

This repository uses central package management, while the standalone samples specify their own package versions. Disable central package management for the file-based app when running a sample:

```powershell
# Example: Run the simple chat prompt sample
dotnet run -p:ManagePackageVersionsCentrally=false quickstart/responses/developer_quickstart.cs

# Run other samples
dotnet run -p:ManagePackageVersionsCentrally=false guides/text/responses/responses_simpleprompt.cs
dotnet run -p:ManagePackageVersionsCentrally=false guides/text/responses/responses_roles.cs
```

### 3. Expected output

When you run `developer_quickstart.cs`, you should see output similar to:

```text
Under a velvet-purple sky, a gentle unicorn named Luna sprinkled stardust over the dreaming forest, filling every heart with peaceful, magical dreams.
```

## Sample file structure

The sample directories are organized as follows:

```text
docs/
├── README.md                           # Basic usage instructions
├── guides/
│   ├── images-vision/
│   ├── mcp/
│   ├── streaming-responses/
│   ├── text/
│   │   ├── chat/
│   │   │   └── ...                     # Chat handling samples
│   │   └── responses/
│   │       └── ...                     # Response handling samples
│   ├── tools/
│   ├── tools-connectors-mcp/
│   └── tools-web-search/
├── overview/
│   └── responses/
└── quickstart/
    └── responses/
        └── ...                         # Response handling samples
```

## Understanding the file-based app format

Each sample file contains special directives at the top:

```csharp
// SAMPLE: Description of what this sample does
#:package OpenAI@2.*             // NuGet package reference
#:property PublishAot=false      // Build properties

using OpenAI.Responses;          // Regular C# code follows

// Your application code here...
```

## Troubleshooting

### Problem: `dotnet --list-sdks` does not include 10.0.400 or a compatible later .NET 10 SDK

- **Solution**: Install the .NET 10 **SDK** (not just the runtime). The `global.json` file in the repository root requires version 10.0.400 or a compatible later SDK.

### Problem: "Couldn't find a project to run"

- **Solution**: Make sure you're running the command from the `docs` directory and providing the correct path to the `.cs` file.

### Problem: "The property directive needs to have two parts"

- **Solution**: The property directive format should be `#:property PropertyName=PropertyValue`, using an equals sign.

### Problem: API errors

- **Solution**:
  - Verify your `OPENAI_API_KEY` environment variable is set correctly
  - Check that your API key is valid and has sufficient credits
  - Ensure the model used by the sample is supported and available to your account

### Problem: Build errors about missing packages

- **Solution**: The package directives restore dependencies automatically when you run a sample. If restore fails, verify that you can access the NuGet sources configured by the repository and retry the same `dotnet run` command.

## Additional resources

- [OpenAI .NET SDK Documentation](https://github.com/openai/openai-dotnet)
- [.NET 10 Documentation](https://learn.microsoft.com/dotnet/core/whats-new/dotnet-10/overview)
- [OpenAI API Documentation](https://platform.openai.com/docs)
- [File-based apps in .NET 10](https://learn.microsoft.com/dotnet/core/sdk/file-based-apps)

## Next steps

Once you have the basic samples working, you can:

1. **Explore other samples** in the `guides/text/` directory
2. **Modify the prompts** in the sample files to experiment with different outputs
3. **Create your own samples** following the same file-based app format
4. **Integrate the OpenAI SDK** into your own .NET applications

Happy coding with OpenAI and .NET 10! 🚀
