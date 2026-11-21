using System.Text.Json;
using Intento.Core.Models;

namespace Intento.Infrastructure;

/// <summary>
/// Persists activities as the existing JSON array format in the current user's Local AppData.
/// </summary>
public sealed class ActivityStore
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    private readonly IActivityFileSystem _fileSystem;

    public ActivityStore(IActivityFileSystem? fileSystem = null, string? filePath = null)
    {
        _fileSystem = fileSystem ?? new PhysicalActivityFileSystem();
        FilePath = filePath ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Intento",
            "activities.json");
    }

    /// <summary>The resolved persistence path; exposed to make location behavior verifiable.</summary>
    public string FilePath { get; }

    public async Task<IReadOnlyList<Activity>> LoadAsync()
    {
        try
        {
            if (!_fileSystem.FileExists(FilePath))
                return [];

            var json = await _fileSystem.ReadAllTextAsync(FilePath);
            return JsonSerializer.Deserialize<List<Activity>>(json) ?? [];
        }
        catch (JsonException exception)
        {
            // Never overwrite or move a malformed file: it may be the only recoverable copy.
            throw new ActivityStoreException(
                "The activities file contains invalid JSON. The original file was preserved.",
                FilePath,
                exception);
        }
        catch (Exception exception) when (IsExpectedStorageException(exception))
        {
            throw new ActivityStoreException("Activities could not be read. Check file access and try again.", FilePath, exception);
        }
    }

    public async Task SaveAsync(IEnumerable<Activity> activities)
    {
        ArgumentNullException.ThrowIfNull(activities);

        var directory = Path.GetDirectoryName(FilePath)
            ?? throw new InvalidOperationException("The activities file path must include a directory.");
        var temporaryPath = Path.Combine(directory, $".activities.{Guid.NewGuid():N}.tmp");

        try
        {
            // Materialize before touching disk so enumeration errors cannot affect the current file.
            var snapshot = activities.ToList();
            var json = JsonSerializer.Serialize(snapshot, JsonOptions);

            _fileSystem.CreateDirectory(directory);
            await _fileSystem.WriteAllTextAsync(temporaryPath, json);

            if (_fileSystem.FileExists(FilePath))
                _fileSystem.ReplaceFile(temporaryPath, FilePath);
            else
                _fileSystem.MoveFile(temporaryPath, FilePath);
        }
        catch (Exception exception) when (IsExpectedStorageException(exception))
        {
            throw new ActivityStoreException(
                "Activities could not be saved. The last valid activities file was left untouched.",
                FilePath,
                exception);
        }
        finally
        {
            // A failed write/replace may leave a temporary file; cleanup must not hide the real error.
            try
            {
                if (_fileSystem.FileExists(temporaryPath))
                    _fileSystem.DeleteFile(temporaryPath);
            }
            catch (Exception exception) when (IsExpectedStorageException(exception))
            {
                // Best-effort cleanup only.
            }
        }
    }

    private static bool IsExpectedStorageException(Exception exception) =>
        exception is IOException
            or UnauthorizedAccessException
            or System.Security.SecurityException
            or JsonException
            or NotSupportedException;
}
