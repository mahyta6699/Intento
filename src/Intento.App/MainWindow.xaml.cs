using Intento.Core.Models;
using Intento.Infrastructure;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Intento.App;

public sealed partial class MainWindow : Window
{
    private readonly ActivityStore _store = new();
    private readonly WorkspaceLauncher _launcher = new();

    public MainWindow()
    {
        InitializeComponent();
        Title = "Intento";
        Loaded += async (_, _) => await LoadActivitiesAsync();
    }

    private async Task LoadActivitiesAsync()
    {
        var activities = await _store.LoadAsync();
        if (activities.Count == 0)
        {
            activities = CreateDefaults();
            await _store.SaveAsync(activities);
        }

        ActivitiesRepeater.ItemsSource = activities;
    }

    private async void ActivityButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: Activity activity })
            await _launcher.LaunchAsync(activity);
    }

    private async void Reload_Click(object sender, RoutedEventArgs e) => await LoadActivitiesAsync();

    private void Exit_Click(object sender, RoutedEventArgs e) => Close();

    private static IReadOnlyList<Activity> CreateDefaults() =>
    [
        new Activity { Name = "Job", Icon = "💼" },
        new Activity { Name = "Learning", Icon = "📚" },
        new Activity { Name = "Research", Icon = "🔬" },
        new Activity { Name = "Workout", Icon = "🏋️" }
    ];
}
