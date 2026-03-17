using Avalonia.Controls;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Windowing;
using ClipboardManager.Views;

namespace ClipboardManager.Views;

public partial class MainWindow : AppWindow
{
    private readonly HomePageView _homePage = new();
    private readonly SettingsView _settingsPage = new();

    public MainWindow()
    {
        InitializeComponent();
        // Show home page by default
        PageContent.Content = _homePage;
    }

    private void NavView_SelectionChanged(object? sender, NavigationViewSelectionChangedEventArgs e)
    {
        if (e.IsSettingsSelected)
        {
            PageContent.Content = _settingsPage;
        }
        else
        {
            PageContent.Content = _homePage;
        }
    }
}