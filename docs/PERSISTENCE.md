# Activity persistence

Intento stores activity configuration as a JSON array at:

`%LocalAppData%/Intento/activities.json`

The file is per-user and is deliberately not stored beside the executable, so installed or portable application updates do not overwrite user configuration.

## Compatibility

The persisted shape remains the existing `List<Activity>` JSON format. Property names (`Id`, `Name`, `Icon`, `Items`, `Type`, and `Target`) and numeric `WorkspaceItemType` values are unchanged:

- `0`: Application
- `1`: Url
- `2`: Folder

No migration is required for existing valid activity JSON files.

## Read behavior

- A missing file represents an empty activity collection.
- A valid JSON `null` value also resolves to an empty collection.
- Malformed JSON and expected file-system failures are surfaced as `ActivityStoreException`, including the affected path and original exception.
- A malformed file is never silently replaced or deleted; keep it intact for manual recovery.

The UI catches persistence exceptions and presents an actionable error instead of allowing an unhandled storage exception to terminate the app.

## Write behavior

Writes are serialized to a uniquely named temporary file in the destination directory. Once the complete temporary file has been written:

- an existing destination is replaced with `File.Replace`;
- a new destination is created by moving the temporary file into place.

Keeping the temporary file on the same volume allows the final operation to be atomic on supported Windows file systems. If serialization, writing, or replacement fails, the previous destination is not intentionally modified. Temporary-file cleanup is best-effort and never masks the original failure.

## Testability

`IActivityFileSystem` isolates file access from the repository logic. Production uses `PhysicalActivityFileSystem`; tests can inject a fake implementation and/or a temporary file path through the `ActivityStore` constructor.

The persistence test suite covers round-trip data for multiple activities and every workspace item type, missing files, malformed JSON recovery, inaccessible reads, failed replacement, and the resolved Local AppData path.

Run tests on a .NET 10 SDK:

```powershell
dotnet test tests/Intento.Infrastructure.Tests/Intento.Infrastructure.Tests.csproj
```
