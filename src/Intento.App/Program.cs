using Microsoft.UI.Xaml;

namespace Intento.App;

public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        Application.Start(p =>
        {
            var context = new DispatcherQueueSynchronizationContext(Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread());
            SynchronizationContext.SetSynchronizationContext(context);
            _ = new App();
        });
    }
}
