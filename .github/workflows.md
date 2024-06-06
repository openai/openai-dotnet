The workflows in this repository try to follow existing, basic samples with little customization.

## main.yml
We use a standard dotnet build/test/pack workflow  
https://docs.github.com/en/actions/automating-builds-and-tests/building-and-testing-net

- Build the solution using the dotnet cli
  - Strong name the assemblies using a key stored in the repository  
    https://github.com/dotnet/runtime/blob/main/docs/project/strong-name-signing.md
- Test the built libraries
  - In a PR run, only local tests are run.
  - In a CI run, live tests are run using a repository secret containing an OpenAI token  
    https://docs.github.com/en/actions/security-guides/using-secrets-in-github-actions
- Package the built libraries
- Publish the package to a GitHub NuGet registry  
  https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-nuget-registry
- Publish a single build artifact containing test results and a nuget package

## release.yml
Releases are triggered by publishing a release in the GitHub repository.  The release workflow will:

- Build the solution using the dotnet cli
  - Strong name the assemblies using a key stored in the repository  
- Test the built libraries
  - Live tests are run using a repository secret containing an OpenAI token  
- Package the built libraries
- Publish the package to public NuGet registry  
- Publish a single build artifact containing test results and a nuget package
