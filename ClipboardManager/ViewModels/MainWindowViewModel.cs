using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using ClipboardManager.Services.Interfaces;

namespace ClipboardManager.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IClipboardRepository _repo;
    private readonly IClipboardService _clipboard;

    public ObservableCollection<string> ClipboardHistory { get; } = new();

    public IAsyncRelayCommand<string> CopyEntryCommand { get; }
    public IAsyncRelayCommand<string> DeleteEntryCommand { get; }

    public MainWindowViewModel(IClipboardRepository repo, IClipboardService clipboard)
    {
        _repo = repo;
        _clipboard = clipboard;

        CopyEntryCommand = new AsyncRelayCommand<string>(CopyEntryAsync);
        DeleteEntryCommand = new AsyncRelayCommand<string>(DeleteEntryAsync);

        _ = LoadHistoryAsync();
    }

    private async Task LoadHistoryAsync()
    {
        var entries = await _repo.LoadAllAsync();
        foreach (var entry in entries)
            ClipboardHistory.Add(entry);
    }

    public async void OnClipboardChanged(object? sender, string content)
    {
        if (string.IsNullOrWhiteSpace(content) || ClipboardHistory.Contains(content))
            return;

        ClipboardHistory.Insert(0, content);
        await _repo.SaveEntryAsync(content);
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