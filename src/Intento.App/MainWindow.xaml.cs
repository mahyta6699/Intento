using Intento.Core.Models;
using Intento.Infrastructure;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Intento.App;

public sealed partial class MainWindow : Window
{
    private readonly ActivityStore _store = new();
    private readonly WorkspaceLauncher _launcher = new();
    private bool _activitiesLoaded;

    public MainWindow()
    {
        InitializeComponent();
        Title = "Intento";
        Activated += MainWindow_Activated;
    }

    private async void MainWindow_Activated(object sender, WindowActivatedEventArgs e)
    {
        if (_activitiesLoaded || e.WindowActivationState == WindowActivationState.Deactivated)
            return;

        _activitiesLoaded = true;
        await LoadActivitiesAsync();
    }

    private async Task LoadActivitiesAsync()
    {
        try
        {
            var activities = await _store.LoadAsync();
            if (activities.Count == 0)
            {
                activities = CreateDefaults();
                await _store.SaveAsync(activities);
            }

            ActivitiesRepeater.ItemsSource = activities;
        }
        catch (ActivityStoreException exception)
        {
            ActivitiesRepeater.ItemsSource = Array.Empty<Activity>();
            await ShowPersistenceErrorAsync(exception);
        }
    }

    private async void ActivityButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: Activity activity })
            await _launcher.LaunchAsync(activity);
    }

    private async void Reload_Click(object sender, RoutedEventArgs e) => await LoadActivitiesAsync();

    private void Exit_Click(object sender, RoutedEventArgs e) => Close();

    private async Task ShowPersistenceErrorAsync(ActivityStoreException exception)
    {
        var dialog = new ContentDialog
        {
            Title = "Could not access saved activities",
            Content = $"{exception.Message}\n\nFile: {exception.FilePath}",
            CloseButtonText = "OK",
            XamlRoot = ((FrameworkElement)Content).XamlRoot
        };

        await dialog.ShowAsync();
    }

    private static IReadOnlyList<Activity> CreateDefaults() =>
        new Activity[]
        {
            new() { Name = "Job", Icon = "💼" },
            new() { Name = "Learning", Icon = "📚" },
            new() { Name = "Research", Icon = "🔬" },
            new() { Name = "Workout", Icon = "🏋️" }
        };
}
