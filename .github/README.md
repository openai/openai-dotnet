The workflows in this repository try to follow existing, basic samples with little customization.

## main.yml
We use a standard dotnet build/test/pack workflow  
https://docs.github.com/en/actions/automating-builds-and-tests/building-and-testing-net

- Build the solution using the dotnet cli
  - Strong name the assemblies using a key stored in the repository  
    https://github.com/dotnet/runtime/blob/main/docs/project/strong-name-signing.md
- Test the built libraries
  - Use a repository secret to hold the OpenAI token used for live testing  
    https://docs.github.com/en/actions/security-guides/using-secrets-in-github-actions
- Package the built libraries
- Publish the package as a GitHub Release
- Publish the package to a GitHub NuGet registry  
  https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-nuget-registry
- Publish a single build artifact containing test results and a nuget package