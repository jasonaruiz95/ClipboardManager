using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;
using ClipboardManager.Models;
using ClipboardManager.Services;
using ClipboardManager.Services.Interfaces;
using ClipboardManager.Views;
using CommunityToolkit.Mvvm.Input;

namespace ClipboardManager.ViewModels;

public class HomePageViewModel : ViewModelBase
{

    private readonly IClipboardRepository _repo;
    private readonly IClipboardService _clipboard;
    private readonly ISyncService _syncService;
    private readonly DatabaseConfig _config;

    public ObservableCollection<string> ClipboardHistory { get; } = new();

    public IAsyncRelayCommand<string> CopyEntryCommand { get; }
    public IAsyncRelayCommand<string> DeleteEntryCommand { get; }
    public IAsyncRelayCommand SyncCommand { get; }

    public ObservableCollection<ClipboardEntry> ClipboardEntries { get; set; } =
        new ();
        
    public HomePageViewModel(IClipboardRepository repo, IClipboardService clipboard, ISyncService syncService, DatabaseConfig config)
    {
        _repo = repo;
        _clipboard = clipboard;
        _syncService = syncService;
        _config = config;

        CopyEntryCommand = new AsyncRelayCommand<string>(CopyEntryAsync);
        DeleteEntryCommand = new AsyncRelayCommand<string>(DeleteEntryAsync);
        SyncCommand = new AsyncRelayCommand(SyncAsync);

        _ = LoadHistoryAsync();

        if (_config.Backend == DatabaseBackend.ApiSync)
        {
            _ = StartAutoSync();
        }
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
    
    private async Task StartAutoSync()
    {
        while (true)
        {
            await Task.Delay(TimeSpan.FromMinutes(5));
            await SyncAsync();
        }
    }

    private async Task SyncAsync()
    {
        await _syncService.SyncAsync();
        await LoadHistoryAsync();
    }

    private async Task LoadHistoryAsync()
    {
        var entries = await _repo.LoadAllAsync();
        ClipboardHistory.Clear();
        foreach (var entry in entries)
            ClipboardHistory.Add(entry);
    }

    public async void OnClipboardChanged(object? sender, string content)
    {
        if (string.IsNullOrWhiteSpace(content) || ClipboardHistory.Contains(content))
            return;

        ClipboardHistory.Insert(0, content);
        await _repo.SaveEntryAsync(content);
        
        if (_config.Backend == DatabaseBackend.ApiSync)
        {
            await _syncService.SyncAsync();
        }
    }

    private async Task CopyEntryAsync(string? content)
    {
        if (string.IsNullOrWhiteSpace(content)) return;
        await _clipboard.SetTextAsync(content);
    }

    private async Task DeleteEntryAsync(string? content)
    {
        if (string.IsNullOrWhiteSpace(content)) return;

        ClipboardHistory.Remove(content);
        await _repo.DeleteEntryAsync(content);
    }
}


