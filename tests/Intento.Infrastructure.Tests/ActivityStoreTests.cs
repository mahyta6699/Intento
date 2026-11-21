using Intento.Core.Models;
using Intento.Infrastructure;

namespace Intento.Infrastructure.Tests;

public sealed class ActivityStoreTests
{
    [Fact]
    public async Task SaveAndLoad_RoundTripsAllActivityAndItemTypes()
    {
        using var directory = new TemporaryDirectory();
        var path = Path.Combine(directory.Path, "Intento", "activities.json");
        var store = new ActivityStore(filePath: path);
        var activities = new[]
        {
            new Activity
            {
                Id = "job-1",
                Name = "Job",
                Icon = "💼",
                Items =
                [
                    new WorkspaceItem { Name = "Editor", Type = WorkspaceItemType.Application, Target = @"C:\Tools\editor.exe" },
                    new WorkspaceItem { Name = "Docs", Type = WorkspaceItemType.Url, Target = "https://example.com/docs" },
                    new WorkspaceItem { Name = "Project", Type = WorkspaceItemType.Folder, Target = @"C:\Projects\Intento" }
                ]
            },
            new Activity { Id = "learn-1", Name = "Learning", Icon = "📚" }
        };

        await store.SaveAsync(activities);
        var loaded = await store.LoadAsync();

        Assert.Equal(2, loaded.Count);
        Assert.Equal(activities[0].Id, loaded[0].Id);
        Assert.Equal(activities[0].Name, loaded[0].Name);
        Assert.Equal(activities[0].Icon, loaded[0].Icon);
        Assert.Equal(activities[0].Items.Count, loaded[0].Items.Count);
        for (var index = 0; index < activities[0].Items.Count; index++)
        {
            Assert.Equal(activities[0].Items[index].Name, loaded[0].Items[index].Name);
            Assert.Equal(activities[0].Items[index].Type, loaded[0].Items[index].Type);
            Assert.Equal(activities[0].Items[index].Target, loaded[0].Items[index].Target);
        }
        Assert.Equal(activities[1].Id, loaded[1].Id);
    }

    [Fact]
    public async Task LoadAsync_WhenFileIsMissing_ReturnsEmptyList()
    {
        using var directory = new TemporaryDirectory();
        var store = new ActivityStore(filePath: Path.Combine(directory.Path, "missing.json"));

        var loaded = await store.LoadAsync();

        Assert.Empty(loaded);
    }

    [Fact]
    public async Task LoadAsync_WhenJsonIsMalformed_ThrowsControlledErrorAndPreservesFile()
    {
        using var directory = new TemporaryDirectory();
        var path = Path.Combine(directory.Path, "activities.json");
        const string invalidJson = "{ definitely-not-json";
        await File.WriteAllTextAsync(path, invalidJson);
        var store = new ActivityStore(filePath: path);

        var exception = await Assert.ThrowsAsync<ActivityStoreException>(() => store.LoadAsync());

        Assert.Equal(path, exception.FilePath);
        Assert.Equal(invalidJson, await File.ReadAllTextAsync(path));
    }

    [Fact]
    public async Task LoadAsync_WhenPathIsInaccessible_ThrowsControlledError()
    {
        var fileSystem = new FailingFileSystem { ReadFailure = new UnauthorizedAccessException("denied") };
        var store = new ActivityStore(fileSystem, Path.Combine(Path.GetTempPath(), "Intento-tests", "activities.json"));

        await Assert.ThrowsAsync<ActivityStoreException>(() => store.LoadAsync());
    }

    [Fact]
    public async Task SaveAsync_WhenReplacementFails_KeepsLastValidFile()
    {
        using var directory = new TemporaryDirectory();
        var path = Path.Combine(directory.Path, "activities.json");
        const string previousJson = "[{\"Name\":\"Existing\"}]";
        await File.WriteAllTextAsync(path, previousJson);
        var fileSystem = new FailingFileSystem { ReplaceFailure = new IOException("disk error") };
        var store = new ActivityStore(fileSystem, path);

        await Assert.ThrowsAsync<ActivityStoreException>(() =>
            store.SaveAsync([new Activity { Name = "Replacement" }]));

        Assert.Equal(previousJson, await File.ReadAllTextAsync(path));
    }

    [Fact]
    public void DefaultPath_IsUnderLocalApplicationData()
    {
        var store = new ActivityStore();
        var expectedRoot = Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
        var actualPath = Path.GetFullPath(store.FilePath);

        Assert.StartsWith(expectedRoot + Path.DirectorySeparatorChar, actualPath, StringComparison.OrdinalIgnoreCase);
        Assert.EndsWith(Path.Combine("Intento", "activities.json"), actualPath, StringComparison.OrdinalIgnoreCase);
    }

    private sealed class TemporaryDirectory : IDisposable
    {
        public TemporaryDirectory()
        {
            Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Intento.Tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path);
        }

        public string Path { get; }

        public void Dispose()
        {
            if (Directory.Exists(Path))
                Directory.Delete(Path, recursive: true);
        }
    }

    private sealed class FailingFileSystem : IActivityFileSystem
    {
        public Exception? ReadFailure { get; init; }
        public Exception? ReplaceFailure { get; init; }

        public bool FileExists(string path) => true;
        public void CreateDirectory(string path) { }
        public Task<string> ReadAllTextAsync(string path) =>
            ReadFailure is null ? Task.FromResult("[]") : Task.FromException<string>(ReadFailure);
        public Task WriteAllTextAsync(string path, string contents) => Task.CompletedTask;
        public void ReplaceFile(string sourcePath, string destinationPath)
        {
            if (ReplaceFailure is not null)
                throw ReplaceFailure;
        }
        public void MoveFile(string sourcePath, string destinationPath) { }
        public void DeleteFile(string path) { }
    }
}
