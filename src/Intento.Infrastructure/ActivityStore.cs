using System.Text.Json;
using Intento.Core.Models;

namespace Intento.Infrastructure;

public sealed class ActivityStore
{
    private readonly string _filePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "Intento",
        "activities.json");

    public async Task<IReadOnlyList<Activity>> LoadAsync()
    {
        if (!File.Exists(_filePath))
            return [];

        await using var stream = File.OpenRead(_filePath);
        return await JsonSerializer.DeserializeAsync<List<Activity>>(stream) ?? [];
    }

    public async Task SaveAsync(IEnumerable<Activity> activities)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
        await using var stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, activities, new JsonSerializerOptions { WriteIndented = true });
    }
}
