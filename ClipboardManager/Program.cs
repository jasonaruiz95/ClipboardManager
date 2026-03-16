using Avalonia;
using System;
using System.IO;

namespace ClipboardManager;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        var logPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            "clipboard_log.txt"
        );

        try
        {
            File.WriteAllText(logPath, $"Working dir: {Directory.GetCurrentDirectory()}\n");
            File.AppendAllText(logPath, $"App base: {AppContext.BaseDirectory}\n");

            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            File.AppendAllText(logPath, $"CRASH: {ex}\n");
        }
    }
    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}