using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;
using Avalonia.Platform.Storage;

namespace ClipboardManager.Services.Interfaces;

public interface IPlatformServicesAccessor
{
    IClipboard? Clipboard { get; }
}

class DefaultPlatformServiceAccessor : IPlatformServicesAccessor
{
    readonly IClassicDesktopStyleApplicationLifetime _desktop;

    public DefaultPlatformServiceAccessor(IClassicDesktopStyleApplicationLifetime desktop)
    {
        _desktop = desktop;
    }

    public IClipboard? Clipboard => _desktop.MainWindow?.Clipboard;
}