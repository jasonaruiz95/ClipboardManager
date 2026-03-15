using System;
using System.Collections.ObjectModel;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ClipboardManager.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    
    /// <summary>
    /// Clipboard entries in reverse-chronological order (newest at index 0).
    /// Bind your ListBox/ItemsControl to this in your View.
    /// </summary>
    public ObservableCollection<string> ClipboardHistory { get; } = new();

    /// <summary>
    /// Called by App.axaml.cs when the ClipboardMonitorService detects a new entry.
    /// Already marshalled onto the UI thread by ClipboardMonitorService.
    /// </summary>
    public void OnClipboardChanged(object? sender, string newText)
    {
        ClipboardHistory.Insert(0, newText);
    }
}