using System;

namespace ClipboardManager.Services.Interfaces;

public interface IClipboardMonitorService : IDisposable
{
    /// <summary>Fired on the UI thread whenever a new, distinct text entry is detected.</summary>
    event EventHandler<string> ClipboardChanged;

    void Start();
    void Stop();
}