using Avalonia.Input.Platform;

namespace ClipboardManager.Services.Interfaces;

public interface IPlatformServicesAccessor
{
        IClipboard? Clipboard { get; }
}