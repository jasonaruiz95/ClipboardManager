using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ClipboardManager.Services.Interfaces;

namespace ClipboardManager.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IClipboardRepository _repo;

    public ObservableCollection<string> ClipboardHistory { get; } = new();

    public MainWindowViewModel(IClipboardRepository repo)
    {
        _repo = repo;
        _ = LoadHistoryAsync();
    }

    // Called once on startup — populates the list from SQLite
    private async Task LoadHistoryAsync()
    {
        var entries = await _repo.LoadAllAsync();
        foreach (var entry in entries)
            ClipboardHistory.Add(entry);
    }

    // Called by the monitor every time the clipboard changes
    public async void OnClipboardChanged(object? sender, string content)
    {
        if (string.IsNullOrWhiteSpace(content) || ClipboardHistory.Contains(content))
            return;

        ClipboardHistory.Insert(0, content);
        await _repo.SaveEntryAsync(content);
    }
}