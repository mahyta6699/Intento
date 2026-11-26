namespace Intento.Core.Models;

public sealed class Activity
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = "⚡";
    // Optional local image selected by the user. Existing JSON files remain compatible.
    public string? ImagePath { get; set; }
    public List<WorkspaceItem> Items { get; set; } = [];
}

public sealed class WorkspaceItem
{
    public string Name { get; set; } = string.Empty;
    public WorkspaceItemType Type { get; set; }
    public string Target { get; set; } = string.Empty;
}

public enum WorkspaceItemType
{
    Application,
    Url,
    Folder
}
