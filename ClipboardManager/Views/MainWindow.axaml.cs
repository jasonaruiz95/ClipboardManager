using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Windowing;
using ClipboardManager.ViewModels;

namespace ClipboardManager.Views;

public partial class MainWindow : AppWindow
{
    private readonly HomePageView _homePage;
    private readonly SettingsView _settingsPage;

    public MainWindow(HomePageViewModel homePageVm, SettingsViewModel settingsVm)
    {
        InitializeComponent();

        _homePage = new HomePageView { DataContext = homePageVm };
        _settingsPage = new SettingsView { DataContext = settingsVm };

        PageContent.Content = _homePage;
    }

    private void NavView_SelectionChanged(object? sender, NavigationViewSelectionChangedEventArgs e)
    {
        if (e.IsSettingsSelected)
            PageContent.Content = _settingsPage;
        else
            PageContent.Content = _homePage;
    }
}