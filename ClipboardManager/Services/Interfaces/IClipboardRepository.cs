using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClipboardManager.Services.Interfaces;

public interface IClipboardRepository
{
    Task SaveEntryAsync(string content);
    Task<IReadOnlyList<string>> LoadAllAsync();
    Task DeleteEntryAsync(string content);
    Task ClearAllAsync();
}