using System;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;
using ClipboardManager.Services.Interfaces;

namespace ClipboardManager.Services;

/// <summary>
/// Retrieves Avalonia platform services from the classic desktop lifetime.
/// The <see cref="IClipboard"/> instance is taken from the main window once
/// the window has been created.
/// </summary>
public sealed class DefaultPlatformServiceAccessor : IPlatformServicesAccessor
{
    private readonly IClassicDesktopStyleApplicationLifetime _lifetime;

    public DefaultPlatformServiceAccessor(IClassicDesktopStyleApplicationLifetime lifetime)
    {
        _lifetime = lifetime;
    }

    /// <summary>
    /// Returns the clipboard associated with the application's main window.
    /// Call this only after <c>desktop.MainWindow</c> has been assigned.
    /// </summary>
    public IClipboard Clipboard =>
        _lifetime.MainWindow?.Clipboard
        ?? throw new InvalidOperationException(
            "MainWindow has not been assigned yet. " +
            "Resolve IClipboard only after the window is created.");
}