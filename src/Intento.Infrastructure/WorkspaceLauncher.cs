using Intento.Core.Models;
using DiagnosticsProcess = System.Diagnostics.Process;
using DiagnosticsProcessStartInfo = System.Diagnostics.ProcessStartInfo;

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
                    case WorkspaceItemType.Application:
                    case WorkspaceItemType.Folder:
                        DiagnosticsProcess.Start(new DiagnosticsProcessStartInfo(item.Target)
                        {
                            UseShellExecute = true
                        });
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
