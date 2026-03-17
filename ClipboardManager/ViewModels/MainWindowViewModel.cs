using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using ClipboardManager.Services.Interfaces;
using ClipboardManager.Services;

namespace ClipboardManager.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IClipboardRepository _repo;
    private readonly IClipboardService _clipboard;
    private readonly ISyncService _syncService;
    private readonly DatabaseConfig _config;

    public ObservableCollection<string> ClipboardHistory { get; } = new();

    public IAsyncRelayCommand<string> CopyEntryCommand { get; }
    public IAsyncRelayCommand<string> DeleteEntryCommand { get; }
    public IAsyncRelayCommand SyncCommand { get; }

    public MainWindowViewModel(IClipboardRepository repo, IClipboardService clipboard, ISyncService syncService, DatabaseConfig config)
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