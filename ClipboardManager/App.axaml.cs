using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using ClipboardManager.Services;
using ClipboardManager.Services.Interfaces;
using ClipboardManager.ViewModels;
using ClipboardManager.Views;
using Microsoft.Extensions.DependencyInjection;

namespace ClipboardManager;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        DisableAvaloniaDataAnnotationValidation();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var services = new ServiceCollection();

            // Platform accessor — wraps the desktop lifetime so services can
            // reach IClipboard without taking a direct Window dependency.
            services.AddSingleton<IPlatformServicesAccessor>(
                new DefaultPlatformServiceAccessor(desktop));

            // After the existing IPlatformServicesAccessor registration, add:
            var dbConfig = new DatabaseConfig
            {
                Backend = DatabaseBackend.Sqlite,
                SqlitePath = "clipboard.db"
            };
            services.AddSingleton(dbConfig);

            services.AddSingleton<IClipboardRepository>(provider =>
            {
                var cfg = provider.GetRequiredService<DatabaseConfig>();
                return new SqliteClipboardRepository(cfg.SqlitePath);
            });
            
            // Clipboard monitor — resolved lazily via a factory so that
            // IClipboard is only accessed after the MainWindow is assigned.
            services.AddSingleton<IClipboardMonitorService>(provider =>
            {
                var accessor = provider.GetRequiredService<IPlatformServicesAccessor>();
                return new ClipboardMonitorService(accessor.Clipboard);
            });

            services.AddTransient<MainWindowViewModel>();
// Add alongside the other service registrations:
            services.AddSingleton<IClipboardService, ClipboardService>();
            services.AddHttpClient<ISyncService, SyncService>();
            var provider = services.BuildServiceProvider();

            // 1. Create the window and assign DataContext
            var vm = provider.GetRequiredService<HomePageViewModel>();
            desktop.MainWindow = new MainWindow
            {
                DataContext = vm,
            };

            // 2. Now that MainWindow exists, IClipboard is available —
            //    resolve and start the monitor.
            var monitor = provider.GetRequiredService<IClipboardMonitorService>();
            monitor.ClipboardChanged += vm.OnClipboardChanged;
            monitor.Start();

            // 3. Stop the monitor cleanly when the app exits.
            // Replace the existing desktop.Exit line with:
            desktop.Exit += async (_, _) =>
            {
                monitor.Stop();
                if (provider.GetRequiredService<IClipboardRepository>() is IAsyncDisposable d)
                    await d.DisposeAsync();
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        foreach (var plugin in dataValidationPluginsToRemove)
            BindingPlugins.DataValidators.Remove(plugin);
    }
}