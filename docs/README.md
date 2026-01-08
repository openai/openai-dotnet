# OpenAI with .NET 10 - Getting Started Guide

This readme shows you how to run each OpenAI based sample (.cs) file in this folder directly without a project or additional setup using the latest .NET 10 features.

## Prerequisites

### 1. Install .NET 10

#### Option A: Using package manager - Recommended

- Windows

    ```powershell
    # Install .NET 10 SDK Preview
    winget install Microsoft.DotNet.SDK.10
    ```

- Mac OS

    ```bash
    # Install .NET 10 SDK Preview
    brew tap isen-ng/dotnet-sdk-versions
    brew install --cask dotnet-sdk10
    ```

#### Option B: Manual download

1. Visit the [.NET 10 Download Page](https://dotnet.microsoft.com/download/dotnet/10.0)
1. Download and install: **.NET SDK 10.0** (required for development and `dotnet run`)

### 2. Verify installation

After installation, verify you have the correct versions:

```powershell
# Check installed SDKs
dotnet --list-sdks

# Check version from the docs directory (should show 10.x)
cd docs
dotnet --version
```

You should see output similar to:

```text
10.0.100
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

The samples use .NET 10's new single-file application feature. Each `.cs` file in the guides folder is a standalone application.

### 1. Navigate to the docs directory

```powershell
cd docs
```

### 2. Run a sample

```powershell
# Example: Run the simple chat prompt sample
dotnet run quickstart/responses/developer_quickstart.cs

# Run other samples
dotnet run guides/text/responses/responses_simpleprompt.cs
dotnet run guides/text/responses/responses_roles.cs
```

### 3. Expected output

When you run `developer_quickstart.cs`, you should see output similar to:

```text
Under a velvet-purple sky, a gentle unicorn named Luna sprinkled stardust over the dreaming forest, filling every heart with peaceful, magical dreams.
```

## Sample file structure

The samples are organized as follows:

```text
docs/
├── global.json                         # Specifies .NET 10 preview SDK
├── README.MD                           # Basic usage instructions
├── guides/
│   └── text/
│       ├── chat/
│           └── ...                     # Chat handling samples
│       └── responses/
│           └── ...                     # Response handling samples
├── quickstart/
│   └── responses/
│       └── ...                         # Response handling samples
```

## Understanding the single-file format

Each sample file contains special directives at the top:

```csharp
// SAMPLE: Description of what this sample does
#:package OpenAI@2.*             // NuGet package reference
#:property PublishAot=false      // Build properties

using OpenAI.Responses;          // Regular C# code follows

// Your application code here...
```

## Troubleshooting

### Problem: "No package found matching input criteria"

- **Solution**: The .NET 10 preview packages might not be available yet. Try installing from the official Microsoft download page instead.

### Problem: `dotnet --version` shows 9.x instead of 10.x

- **Solution**: You need to install the .NET 10 **SDK** (not just the runtime). The `global.json` file in the guides directory requires the SDK.

### Problem: "Couldn't find a project to run"

- **Solution**: Make sure you're running the command from the `docs/guides` directory and providing the correct path to the `.cs` file.

### Problem: "The property directive needs to have two parts"

- **Solution**: The property directive format should be `#:property PropertyName PropertyValue` (space-separated, not equals sign).

### Problem: API errors

- **Solution**:
  - Verify your `OPENAI_API_KEY` environment variable is set correctly
  - Check that your API key is valid and has sufficient credits
  - Ensure you're using a valid model name (e.g., "gpt-4", "gpt-3.5-turbo")

### Problem: Build errors about missing packages

- **Solution**: The package directives should automatically download dependencies. If not, try:

  ```powershell
  dotnet restore
  ```

## Additional resources

- [OpenAI .NET SDK Documentation](https://github.com/openai/openai-dotnet)
- [.NET 10 Preview Documentation](https://docs.microsoft.com/dotnet/core/whats-new/dotnet-10)
- [OpenAI API Documentation](https://platform.openai.com/docs)
- [Single-File Applications in .NET 10](https://devblogs.microsoft.com/dotnet/announcing-dotnet-run-app/)

## Next steps

Once you have the basic samples working, you can:

1. **Explore other samples** in the `text/` directory
2. **Modify the prompts** in the sample files to experiment with different outputs
3. **Create your own samples** following the same single-file format
4. **Integrate the OpenAI SDK** into your own .NET applications

Happy coding with OpenAI and .NET 10! 🚀
