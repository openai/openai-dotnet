# API Listings

The `.cs` files in the target framework subdirectories are **auto-generated** API listings and must not be modified manually.

To regenerate the in-progress listings from the current source, run:

```powershell
./scripts/Export-Api.ps1
```

To regenerate the released listings from the published package version recorded
in `api-version.txt`, run:

```powershell
./scripts/Export-Api.ps1 -Released
```

The most common reason to regenerate the API listings is to reflect a change in the public API surface (such as adding, removing, or renaming public types or members). In that case, modify the relevant source files first and then re-run the script above.

The API listings under `api/released/` reflect the latest released public API
surface. They are generated from the published NuGet package identified by
`api-version.txt`, using the repository's current GenAPI version and formatting
logic. They are therefore a current rendering of the released binary rather than
a byte-for-byte copy of the listings committed with the release tag.

Source API changes normally require regenerating only `api/in-progress`. Changes
to GenAPI, API formatting, splitting, or generated headers require regenerating
both directories. Released-package downloads and restore artifacts are created
under the ignored `artifacts/api/` directory and removed automatically.

API listings are split by target framework and namespace. For example, the `net10.0` listings are written to `api/in-progress/net10.0/` as files such as `OpenAI.net10.0.cs`, `OpenAI.Chat.net10.0.cs`, and `OpenAI.Responses.net10.0.cs`.
