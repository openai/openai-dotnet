# API Listings

The current `.cs` files under `api/unreleased/` are **auto-generated** and must not be modified manually.

The `api/` root also contains checked-in metadata such as `ga-apis.yaml` and versioned API baselines.

To regenerate them, run:

```powershell
./scripts/Export-Api.ps1
```

The most common reason to regenerate these files is a change to the public API surface (adding, removing, or renaming public types or members). In that case, modify the relevant source files first and then re-run the script above.
