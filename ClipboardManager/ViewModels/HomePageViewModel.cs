using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;
using ClipboardManager.Services;
using ClipboardManager.Services.Interfaces;
using ClipboardManager.Views;

namespace ClipboardManager.ViewModels;

public class HomePageViewModel : ViewModelBase
{
    private IClipboard _clipboard;


    public ObservableCollection<ClipboardEntry> ClipboardEntries { get; set; } =
        new ();
        
    public HomePageViewModel()
    {
        
    }
    
    public async Task InitClipboardManager()
    {
        var topLevel = TopLevel.GetTopLevel(Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null);
        var text = await topLevel.Clipboard.TryGetTextAsync();
        
        ClipboardEntries.Add(new ClipboardEntry(text));
    }
}


public class ClipboardEntry
{
    public DateTime Time { get; set; }
    public string Text { get; set; }

    public ClipboardEntry(string text)
    {
        Text = text;
        Time = DateTime.Now;
    }
}