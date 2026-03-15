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
        AddClipboardEntry();
    }

    public async Task AddClipboardEntry()
    {
        var topLevel = TopLevel.GetTopLevel(Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null);
        var text = await topLevel.Clipboard.TryGetTextAsync();
        
        var entry = new ClipboardEntry(text);
        if (!ClipboardEntries.Contains(entry))
        {
            ClipboardEntries.Add(entry);
        }
    }
    
    public async Task AddClipboardEntry(string entryText)
    {
        var topLevel = TopLevel.GetTopLevel(Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null);
        //var text = await topLevel.Clipboard.TryGetTextAsync();
        
        var entry = new ClipboardEntry(entryText);
        if (!ClipboardEntries.Contains(entry))
        {
            ClipboardEntries.Add(entry);
        }
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

    // public bool Equals(ClipboardEntry? other)
    // {
    //     if (other is null) return false;
    //     return Text == other.Text;
    //     
    // }

    public override bool Equals(object obj)
    {
        if (obj is ClipboardEntry other)
        {
            return Text == other.Text;
        }
        return false;
    }
    
}