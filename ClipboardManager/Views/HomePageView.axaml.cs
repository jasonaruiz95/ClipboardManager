using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ClipboardManager.ViewModels;
using System.Timers;
using Avalonia.Threading;

namespace ClipboardManager.Views;

public partial class HomePageView : UserControl
{
    private string _lastClipboardValue = "";
    private Timer _clipboardTimer;
    
    public HomePageView()
    {
        InitializeComponent();
        
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
  
        if (this.DataContext is HomePageViewModel viewModel)
        {
            viewModel.InitClipboardManager();
            StartClipboardPolling(viewModel);
        }
    }
    
    
    private void StartClipboardPolling(HomePageViewModel viewModel)
    {
        _clipboardTimer = new Timer(1000); // 1 seconds
        _clipboardTimer.Elapsed += (s, e) =>
        {
            var clipboardValue = GetClipboardValue(); // Implement this method
            if (clipboardValue != _lastClipboardValue)
            {
                _lastClipboardValue = clipboardValue;
                viewModel.AddClipboardEntry(clipboardValue);
            }
        };
        _clipboardTimer.Start();
    }

    private string GetClipboardValue()
    {
        // Use Avalonia clipboard API or your ClipboardService
        // Example:
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel != null)
        {
            var clipboard = topLevel.Clipboard;
            return clipboard.GetTextAsync().Result ?? "";
        }
        return "";
    }
    
    

   
}