using Intento.Core.Models;
using Intento.Infrastructure;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

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
            AddActivityButton.IsEnabled = false;
            await ShowPersistenceErrorAsync(exception);
        }
    }

    private async void AddActivity_Click(object sender, RoutedEventArgs e)
    {
        var nameInput = new TextBox { PlaceholderText = "Group name", MinWidth = 300 };
        var dialog = new ContentDialog
        {
            Title = "Create group",
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
            await ShowActivityEditorAsync(activity, allowDelete: false);
        }
        catch (ActivityStoreException exception)
        {
            await ShowPersistenceErrorAsync(exception);
        }
    }

    private async void ActivityButton_RightTapped(object sender, RightTappedRoutedEventArgs e)
    {
        if (sender is Button { Tag: Activity activity })
        {
            e.Handled = true;
            await ShowActivityEditorAsync(activity, allowDelete: true);
        }
    }

    private async Task ShowActivityEditorAsync(Activity activity, bool allowDelete)
    {
        var root = new StackPanel { Spacing = 12, MinWidth = 460, MaxWidth = 620 };
        var rowsPanel = new StackPanel { Spacing = 8 };
        var rows = new List<ItemEditorRow>();

        void AddItemRow(WorkspaceItem? item = null)
        {
            var row = new Grid { ColumnSpacing = 8 };
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(115) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(38) });

            var type = new ComboBox { MinWidth = 110, VerticalAlignment = VerticalAlignment.Center };
            type.Items.Add(new ComboBoxItem { Content = "File / App", Tag = "Application" });
            type.Items.Add(new ComboBoxItem { Content = "Folder", Tag = "Folder" });
            type.Items.Add(new ComboBoxItem { Content = "Website", Tag = "Url" });
            type.SelectedIndex = item?.Type switch
            {
                WorkspaceItemType.Folder => 1,
                WorkspaceItemType.Url => 2,
                _ => 0
            };

            var name = new TextBox
            {
                PlaceholderText = "Display name",
                Text = item?.Name ?? "",
                VerticalAlignment = VerticalAlignment.Center
            };
            var target = new TextBox
            {
                PlaceholderText = "File path or website URL",
                Text = item?.Target ?? "",
                VerticalAlignment = VerticalAlignment.Center
            };
            var remove = new Button
            {
                Content = "✕",
                Foreground = new SolidColorBrush(Colors.Red),
                VerticalAlignment = VerticalAlignment.Center,
                Padding = new Thickness(6, 2, 6, 2),
            };

            Grid.SetColumn(type, 0);
            Grid.SetColumn(name, 1);
            Grid.SetColumn(target, 2);
            Grid.SetColumn(remove, 3);
            row.Children.Add(type);
            row.Children.Add(name);
            row.Children.Add(target);
            row.Children.Add(remove);

            var editorRow = new ItemEditorRow(row, type, name, target);
            rows.Add(editorRow);
            rowsPanel.Children.Add(row);
            remove.Click += (_, _) =>
            {
                rowsPanel.Children.Remove(row);
                rows.Remove(editorRow);
            };
        }

        foreach (var item in activity.Items)
            AddItemRow(item);

        var addItemButton = new Button
        {
            Content = "+",
            FontSize = 28,
            FontWeight = FontWeights.Normal,
            Foreground = new SolidColorBrush(Colors.Black),
            HorizontalAlignment = HorizontalAlignment.Center,
            Width = 56,
            Height = 48,
        };
        addItemButton.Click += (_, _) => AddItemRow();

        root.Children.Add(new TextBlock
        {
            Text = "Items in this group",
            FontSize = 16,
            FontWeight = FontWeights.SemiBold,
            Margin = new Thickness(0, 0, 0, 4)
        });
        root.Children.Add(rowsPanel);
        root.Children.Add(addItemButton);

        var deleteButton = new Button
        {
            Content = "Delete group",
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Foreground = new SolidColorBrush(Colors.Red),
            Visibility = allowDelete ? Visibility.Visible : Visibility.Collapsed
        };
        root.Children.Add(deleteButton);

        var dialog = new ContentDialog
        {
            Title = activity.Name,
            Content = new ScrollViewer
            {
                Content = root,
                MaxHeight = 520,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            },
            PrimaryButtonText = "Save",
            CloseButtonText = "Close",
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = ((FrameworkElement)Content).XamlRoot
        };

        var deleteRequested = false;
        deleteButton.Click += (_, _) =>
        {
            deleteRequested = true;
            dialog.Hide();
        };

        var result = await dialog.ShowAsync();
        if (deleteRequested)
        {
            var updated = _activities.Where(existing => existing.Id != activity.Id).ToList();
            try
            {
                await _store.SaveAsync(updated);
                _activities = updated;
                ActivitiesRepeater.ItemsSource = _activities;
            }
            catch (ActivityStoreException exception)
            {
                await ShowPersistenceErrorAsync(exception);
            }
            return;
        }

        if (result != ContentDialogResult.Primary)
            return;

        var updatedItems = new List<WorkspaceItem>();
        foreach (var row in rows)
        {
            var displayName = row.Name.Text.Trim();
            var target = row.Target.Text.Trim();
            if (string.IsNullOrWhiteSpace(displayName) || string.IsNullOrWhiteSpace(target))
            {
                await ShowValidationErrorAsync("برای هر مورد، نام دلخواه و آدرس فایل، پوشه یا سایت را وارد کنید.");
                return;
            }

            var selectedType = (row.Type.SelectedItem as ComboBoxItem)?.Tag?.ToString();
            var itemType = selectedType switch
            {
                "Folder" => WorkspaceItemType.Folder,
                "Url" => WorkspaceItemType.Url,
                _ => WorkspaceItemType.Application
            };

            if (itemType == WorkspaceItemType.Url &&
                (!Uri.TryCreate(target, UriKind.Absolute, out var uri) ||
                 (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)))
            {
                await ShowValidationErrorAsync($"آدرس سایت معتبر نیست: {target}");
                return;
            }

            updatedItems.Add(new WorkspaceItem
            {
                Name = displayName,
                Target = target,
                Type = itemType
            });
        }

        var originalItems = activity.Items;
        activity.Items = updatedItems;
        try
        {
            await _store.SaveAsync(_activities);
            ActivitiesRepeater.ItemsSource = _activities.ToList();
        }
        catch (ActivityStoreException exception)
        {
            activity.Items = originalItems;
            await ShowPersistenceErrorAsync(exception);
        }
    }

    private async void ActivityButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: Activity activity })
            await _launcher.LaunchAsync(activity);
    }

    private async Task ShowValidationErrorAsync(string message)
    {
        var dialog = new ContentDialog
        {
            Title = "Check the item details",
            Content = message,
            CloseButtonText = "OK",
            XamlRoot = ((FrameworkElement)Content).XamlRoot
        };
        await dialog.ShowAsync();
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

    private sealed record ItemEditorRow(
        Grid Container,
        ComboBox Type,
        TextBox Name,
        TextBox Target);
}
