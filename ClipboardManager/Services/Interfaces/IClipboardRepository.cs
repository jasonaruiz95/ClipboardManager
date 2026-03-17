using System.Collections.Generic;
using System.Threading.Tasks;
using ClipboardManager.Models;

namespace ClipboardManager.Services.Interfaces;

public interface IClipboardRepository
{
    Task SaveEntryAsync(string content);
    Task<IReadOnlyList<string>> LoadAllAsync();
    Task<IReadOnlyList<ClipboardEntry>> LoadAllEntriesAsync();
    Task DeleteEntryAsync(string content);
    Task ClearAllAsync();
}