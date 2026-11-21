namespace Intento.Infrastructure;

/// <summary>Production implementation backed by the local file system.</summary>
public sealed class PhysicalActivityFileSystem : IActivityFileSystem
{
    public bool FileExists(string path)
    {
        try
        {
            _ = File.GetAttributes(path);
            return true;
        }
        catch (FileNotFoundException)
        {
            return false;
        }
        catch (DirectoryNotFoundException)
        {
            return false;
        }
    }

    public void CreateDirectory(string path) => Directory.CreateDirectory(path);

    public Task<string> ReadAllTextAsync(string path) => File.ReadAllTextAsync(path);

    public Task WriteAllTextAsync(string path, string contents) => File.WriteAllTextAsync(path, contents);

    public void ReplaceFile(string sourcePath, string destinationPath) =>
        File.Replace(sourcePath, destinationPath, destinationBackupFileName: null);

    public void MoveFile(string sourcePath, string destinationPath) =>
        File.Move(sourcePath, destinationPath);

    public void DeleteFile(string path) => File.Delete(path);
}
