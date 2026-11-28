using Intento.Core.Models;
using Intento.Infrastructure;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using WinRT.Interop;
using DiagnosticsProcess = System.Diagnostics.Process;
using DiagnosticsProcessStartInfo = System.Diagnostics.ProcessStartInfo;

namespace Intento.App;

public sealed partial class MainWindow : Window
{
    private readonly ActivityStore _store = new();
    private readonly WorkspaceLauncher _launcher = new();
    private const string ReadmeUrl = "https://github.com/mahyta6699/Intento/blob/main/README.md";
    private List<Activity> _activities = [];
    private bool _activitiesLoaded;

    private static readonly string[] ActivityStickers =
        ["⚡", "💼", "📚", "🔬", "🏋️", "🎨", "🎯", "💡", "🌱", "🚀", "🎵", "🧘", "💻", "✍️", "☕", "🧠"];

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
        var activity = new Activity { Name = "New group", Icon = "⚡" };
        var updated = _activities.Append(activity).ToList();
        try
        {
            await _store.SaveAsync(updated);
            _activities = updated;
            RefreshActivities();
            await ShowActivityEditorAsync(activity);
        }
        catch (ActivityStoreException exception) { await ShowPersistenceErrorAsync(exception); }
    }

    private async void EditActivity_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: Activity activity })
            await ShowActivityEditorAsync(activity);
    }

    private async void DeleteActivity_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: Activity activity }) return;
        var confirm = new ContentDialog
        {
            Title = $"Delete “{activity.Name}”?",
            Content = "This group and its saved items will be removed.",
            PrimaryButtonText = "Delete",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Close,
            XamlRoot = ((FrameworkElement)Content).XamlRoot
        };
        if (await confirm.ShowAsync() != ContentDialogResult.Primary) return;
        var updated = _activities.Where(x => x.Id != activity.Id).ToList();
        try
        {
            await _store.SaveAsync(updated);
            _activities = updated;
            RefreshActivities();
        }
        catch (ActivityStoreException exception) { await ShowPersistenceErrorAsync(exception); }
    }

    private void RefreshActivities() => ActivitiesRepeater.ItemsSource = _activities.ToList();

    private async Task ShowActivityEditorAsync(Activity activity)
    {
        var root = new StackPanel { Spacing = 12, MinWidth = 480, MaxWidth = 680 };
        var groupName = new TextBox { Header = "Group name", Text = activity.Name, MinWidth = 300 };
        var selectedImagePath = activity.ImagePath;
        var stickerPicker = new ComboBox { Header = "Choose a sticker", Width = 220 };
        foreach (var sticker in ActivityStickers)
            stickerPicker.Items.Add(new ComboBoxItem { Content = sticker, Tag = sticker, FontSize = 22 });
        var currentSticker = ActivityStickers.Contains(activity.Icon) ? activity.Icon : "⚡";
        stickerPicker.SelectedIndex = Array.IndexOf(ActivityStickers, currentSticker);
        var imageStatus = new TextBlock
        {
            Text = string.IsNullOrWhiteSpace(selectedImagePath) ? "No custom image selected" : System.IO.Path.GetFileName(selectedImagePath),
            VerticalAlignment = VerticalAlignment.Center,
            TextTrimming = TextTrimming.CharacterEllipsis
        };
        var chooseImage = new Button { Content = "Choose image from computer" };
        chooseImage.Click += async (_, _) =>
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".bmp");
            picker.FileTypeFilter.Add(".gif");
            InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(this));
            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                var imageFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(
                    "Images", CreationCollisionOption.OpenIfExists);
                var savedImage = await file.CopyAsync(
                    imageFolder, file.Name, NameCollisionOption.GenerateUniqueName);
                selectedImagePath = savedImage.Path;
                imageStatus.Text = savedImage.Name;
            }
        };
        var clearImage = new Button { Content = "Use sticker instead" };
        clearImage.Click += (_, _) =>
        {
            selectedImagePath = null;
            imageStatus.Text = "No custom image selected";
        };
        root.Children.Add(groupName);
        root.Children.Add(stickerPicker);
        root.Children.Add(new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            Children = { chooseImage, clearImage }
        });
        root.Children.Add(imageStatus);

        var rowsPanel = new StackPanel { Spacing = 8 };
        var rows = new List<ItemEditorRow>();

        void AddItemRow(WorkspaceItem? item = null)
        {
            var row = new Grid { ColumnSpacing = 8 };
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(105) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var type = new ComboBox { MinWidth = 105, VerticalAlignment = VerticalAlignment.Center };
            type.Items.Add(new ComboBoxItem { Content = "File / App", Tag = "Application" });
            type.Items.Add(new ComboBoxItem { Content = "Folder", Tag = "Folder" });
            type.Items.Add(new ComboBoxItem { Content = "Website", Tag = "Url" });
            type.SelectedIndex = item?.Type switch
            {
                WorkspaceItemType.Folder => 1,
                WorkspaceItemType.Url => 2,
                _ => 0
            };
            var name = new TextBox { PlaceholderText = "Display name", Text = item?.Name ?? "", VerticalAlignment = VerticalAlignment.Center };
            var target = new TextBox { PlaceholderText = "File path or website URL", Text = item?.Target ?? "", VerticalAlignment = VerticalAlignment.Center };
            var browse = new Button { Content = "Browse", VerticalAlignment = VerticalAlignment.Center };
            var remove = new Button
            {
                Content = "✕",
                Foreground = new SolidColorBrush(Colors.Red),
                VerticalAlignment = VerticalAlignment.Center,
                Padding = new Thickness(6, 2, 6, 2)
            };

            async Task BrowseAsync()
            {
                var selected = (type.SelectedItem as ComboBoxItem)?.Tag?.ToString();
                if (selected == "Url") return;
                if (selected == "Folder")
                {
                    var picker = new FolderPicker();
                    picker.FileTypeFilter.Add("*");
                    InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(this));
                    var folder = await picker.PickSingleFolderAsync();
                    if (folder != null)
                    {
                        target.Text = folder.Path;
                        if (string.IsNullOrWhiteSpace(name.Text)) name.Text = folder.Name;
                    }
                }
                else
                {
                    var picker = new FileOpenPicker();
                    picker.FileTypeFilter.Add("*");
                    InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(this));
                    var file = await picker.PickSingleFileAsync();
                    if (file != null)
                    {
                        target.Text = file.Path;
                        if (string.IsNullOrWhiteSpace(name.Text)) name.Text = System.IO.Path.GetFileNameWithoutExtension(file.Name);
                    }
                }
            }

            browse.Click += async (_, _) => await BrowseAsync();
            type.SelectionChanged += (_, _) =>
            {
                browse.Visibility = ((type.SelectedItem as ComboBoxItem)?.Tag?.ToString() == "Url")
                    ? Visibility.Collapsed : Visibility.Visible;
                target.PlaceholderText = ((type.SelectedItem as ComboBoxItem)?.Tag?.ToString()) switch
                {
                    "Folder" => "Folder path",
                    "Url" => "https://example.com",
                    _ => "Application or file path"
                };
            };
            browse.Visibility = type.SelectedIndex == 2 ? Visibility.Collapsed : Visibility.Visible;

            Grid.SetColumn(type, 0);
            Grid.SetColumn(name, 1);
            Grid.SetColumn(target, 2);
            Grid.SetColumn(browse, 3);
            Grid.SetColumn(remove, 4);
            row.Children.Add(type);
            row.Children.Add(name);
            row.Children.Add(target);
            row.Children.Add(browse);
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

        foreach (var item in activity.Items) AddItemRow(item);
        var addItemButton = new Button
        {
            Content = "+ Add item",
            HorizontalAlignment = HorizontalAlignment.Center
        };
        addItemButton.Click += (_, _) => AddItemRow();

        root.Children.Add(new TextBlock { Text = "Items in this group", FontSize = 16, Margin = new Thickness(0, 8, 0, 4) });
        root.Children.Add(rowsPanel);
        root.Children.Add(addItemButton);

        var dialog = new ContentDialog
        {
            Title = "Edit group",
            Content = new ScrollViewer
            {
                Content = root,
                MaxHeight = 600,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto
            },
            PrimaryButtonText = "Save",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = ((FrameworkElement)Content).XamlRoot
        };

        var originalName = activity.Name;
        var originalIcon = activity.Icon;
        var originalImage = activity.ImagePath;
        var originalItems = activity.Items;
        if (await dialog.ShowAsync() != ContentDialogResult.Primary) return;

        var updatedItems = new List<WorkspaceItem>();
        foreach (var row in rows)
        {
            var displayName = row.Name.Text.Trim();
            var target = row.Target.Text.Trim();
            if (string.IsNullOrWhiteSpace(displayName) || string.IsNullOrWhiteSpace(target))
            {
                await ShowValidationErrorAsync("برای هر مورد، نام و مسیر فایل، پوشه یا آدرس سایت را وارد کنید.");
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
            updatedItems.Add(new WorkspaceItem { Name = displayName, Target = target, Type = itemType });
        }

        activity.Name = groupName.Text.Trim();
        if (string.IsNullOrWhiteSpace(activity.Name))
        {
            await ShowValidationErrorAsync("نام گروه نمی‌تواند خالی باشد.");
            activity.Name = originalName;
            return;
        }
        activity.Icon = (stickerPicker.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "⚡";
        activity.ImagePath = selectedImagePath;
        activity.Items = updatedItems;
        try
        {
            await _store.SaveAsync(_activities);
            RefreshActivities();
        }
        catch (ActivityStoreException exception)
        {
            activity.Name = originalName;
            activity.Icon = originalIcon;
            activity.ImagePath = originalImage;
            activity.Items = originalItems;
            await ShowPersistenceErrorAsync(exception);
        }
    }

    private void ActivityImage_Loaded(object sender, RoutedEventArgs e)
    {
        if (sender is not Image image || image.Tag is not Activity activity ||
            string.IsNullOrWhiteSpace(activity.ImagePath))
        {
            if (sender is Image emptyImage) emptyImage.Visibility = Visibility.Collapsed;
            return;
        }

        try
        {
            image.Source = new BitmapImage(new Uri(activity.ImagePath));
            if (image.Parent is StackPanel panel && panel.Children.Count > 1)
                panel.Children[1].Visibility = Visibility.Collapsed;
        }
        catch
        {
            image.Visibility = Visibility.Collapsed;
        }
    }

    private async void ActivityButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: Activity activity })
            await _launcher.LaunchAsync(activity);
    }

    private async void OpenConfigFile_Click(object sender, RoutedEventArgs e)
    {
        var configPath = _store.FilePath;
        try
        {
            // Create the file on first use so the editor has something to open.
            if (!File.Exists(configPath))
                await _store.SaveAsync(_activities);

            DiagnosticsProcess.Start(new DiagnosticsProcessStartInfo(configPath) { UseShellExecute = true });
        }
        catch (ActivityStoreException exception) { await ShowPersistenceErrorAsync(exception); }
        catch (Exception exception)
        {
            await ShowMessageAsync("Could not open the config file", $"{exception.Message}\n\nFile: {configPath}");
        }
    }

    private void Exit_Click(object sender, RoutedEventArgs e) => Application.Current.Exit();

    private async void About_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            DiagnosticsProcess.Start(new DiagnosticsProcessStartInfo(ReadmeUrl) { UseShellExecute = true });
        }
        catch (Exception exception)
        {
            await ShowMessageAsync("Could not open the browser", $"{exception.Message}\n\n{ReadmeUrl}");
        }
    }

    private async Task ShowMessageAsync(string title, string message)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            CloseButtonText = "OK",
            XamlRoot = ((FrameworkElement)Content).XamlRoot
        };
        await dialog.ShowAsync();
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

    private sealed record ItemEditorRow(Grid Container, ComboBox Type, TextBox Name, TextBox Target);
}
