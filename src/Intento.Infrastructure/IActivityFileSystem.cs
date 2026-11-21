namespace Intento.Infrastructure;

/// <summary>
/// File operations used by <see cref="ActivityStore"/>. Keeping this boundary injectable
/// allows persistence tests to use isolated temporary files and deterministic failures.
/// </summary>
public interface IActivityFileSystem
{
    bool FileExists(string path);
    void CreateDirectory(string path);
    Task<string> ReadAllTextAsync(string path);
    Task WriteAllTextAsync(string path, string contents);
    void ReplaceFile(string sourcePath, string destinationPath);
    void MoveFile(string sourcePath, string destinationPath);
    void DeleteFile(string path);
}
