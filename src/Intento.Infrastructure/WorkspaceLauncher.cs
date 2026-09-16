using Intento.Core.Models;
using System.Diagnostics;

namespace Intento.Infrastructure;

public sealed class WorkspaceLauncher
{
    public async Task LaunchAsync(Activity activity)
    {
        foreach (var item in activity.Items)
        {
            try
            {
                switch (item.Type)
                {
                    case WorkspaceItemType.Url:
                        Process.Start(new ProcessStartInfo(item.Target) { UseShellExecute = true });
                        break;
                    case WorkspaceItemType.Application:
                    case WorkspaceItemType.Folder:
                        Process.Start(new ProcessStartInfo(item.Target) { UseShellExecute = true });
                        break;
                }
            }
            catch
            {
                // MVP: continue launching remaining items when one target fails.
            }

            await Task.Yield();
        }
    }
}
