using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Input.Platform;
using Avalonia.Threading;
using ClipboardManager.Services.Interfaces;

namespace ClipboardManager.Services;

/// <summary>
/// Polls the clipboard every 500 ms on a background task and raises
/// <see cref="ClipboardChanged"/> (on the UI thread) whenever a new,
/// distinct text entry is detected.
/// </summary>
public sealed class ClipboardMonitorService : IClipboardMonitorService
{
    private readonly IClipboard _clipboard;
    private readonly TimeSpan _pollInterval;

    private CancellationTokenSource? _cts;
    private string? _lastText;

    public event EventHandler<string>? ClipboardChanged;

    public ClipboardMonitorService(IClipboard clipboard, TimeSpan? pollInterval = null)
    {
        _clipboard = clipboard;
        _pollInterval = pollInterval ?? TimeSpan.FromMilliseconds(500);
    }

    public void Start()
    {
        if (_cts is not null)
            return; // already running

        _cts = new CancellationTokenSource();
        Task.Run(() => PollLoop(_cts.Token));
    }

    public void Stop()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

    private async Task PollLoop(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(_pollInterval, ct);

                // IClipboard must be accessed on the UI thread
                await Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    var text = await _clipboard.GetTextAsync();

                    if (!string.IsNullOrWhiteSpace(text) && text != _lastText)
                    {
                        _lastText = text;
                        ClipboardChanged?.Invoke(this, text);
                    }
                });
            }
            catch (TaskCanceledException)
            {
                break; // normal shutdown
            }
            catch (Exception ex)
            {
                // Log and continue — a transient clipboard error should never crash the loop
                Console.WriteLine($"[ClipboardMonitor] Poll error: {ex.Message}");
            }
        }
    }

    public void Dispose() => Stop();
}