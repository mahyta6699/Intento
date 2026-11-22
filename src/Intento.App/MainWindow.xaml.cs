using Intento.Core.Models;
using Intento.Infrastructure;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace Intento.App;

public sealed partial class MainWindow : Window
{
    private readonly ActivityStore _store = new();
    private readonly WorkspaceLauncher _launcher = new();
    private List<Activity> _activities = [];
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
            _activities = (await _store.LoadAsync()).ToList();
            ActivitiesRepeater.ItemsSource = _activities;
            AddActivityButton.IsEnabled = true;
        }
        catch (ActivityStoreException exception)
        {
            _activities = [];
            ActivitiesRepeater.ItemsSource = _activities;

            // Fail closed: adding an activity would otherwise replace a corrupt or unreadable file.
            AddActivityButton.IsEnabled = false;
            await ShowPersistenceErrorAsync(exception);
        }
    }

    private async void AddActivity_Click(object sender, RoutedEventArgs e)
    {
        var nameInput = new TextBox
        {
            PlaceholderText = "Activity name",
            MinWidth = 280
        };

        var dialog = new ContentDialog
        {
            Title = "Create activity",
            Content = nameInput,
            PrimaryButtonText = "Create",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = ((FrameworkElement)Content).XamlRoot
        };

        var result = await dialog.ShowAsync();
        var name = nameInput.Text.Trim();
        if (result != ContentDialogResult.Primary || string.IsNullOrWhiteSpace(name))
            return;

        var activity = new Activity { Name = name, Icon = "⚡" };
        var updatedActivities = _activities.Append(activity).ToList();

        try
        {
            await _store.SaveAsync(updatedActivities);
            _activities = updatedActivities;
            ActivitiesRepeater.ItemsSource = _activities;
        }
        catch (ActivityStoreException exception)
        {
            await ShowPersistenceErrorAsync(exception);
        }
    }

    private void ActivityButton_RightTapped(object sender, RightTappedRoutedEventArgs e)
    {
        if (sender is not Button { Tag: Activity activity } button)
            return;

        var menu = new MenuFlyout();
        var deleteItem = new MenuFlyoutItem
        {
            Text = "Delete",
            Tag = activity
        };
        deleteItem.Click += DeleteActivity_Click;
        menu.Items.Add(deleteItem);
        menu.ShowAt(button, e.GetPosition(button));
        e.Handled = true;
    }

    private async void DeleteActivity_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuFlyoutItem { Tag: Activity activity })
            return;

        var updatedActivities = _activities
            .Where(existing => existing.Id != activity.Id)
            .ToList();

        try
        {
            await _store.SaveAsync(updatedActivities);
            _activities = updatedActivities;
            ActivitiesRepeater.ItemsSource = _activities;
        }
        catch (ActivityStoreException exception)
        {
            await ShowPersistenceErrorAsync(exception);
        }
    }

    private async void ActivityButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: Activity activity })
            await _launcher.LaunchAsync(activity);
    }

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
}
