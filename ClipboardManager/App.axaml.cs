using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using System.IO;
using Avalonia.Markup.Xaml;
using ClipboardManager.Services;
using ClipboardManager.Services.Interfaces;
using ClipboardManager.ViewModels;
using ClipboardManager.Views;
using Microsoft.Extensions.DependencyInjection;

namespace ClipboardManager;

public partial class App : Application
{
    public static IServiceProvider? Services { get; private set; }

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

            services.AddSingleton<IPlatformServicesAccessor>(
                new DefaultPlatformServiceAccessor(desktop));

            // Fixed: absolute path for SQLite
            var dbFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Library", "Application Support", "ClipboardManager"
            );
            Directory.CreateDirectory(dbFolder);
            var dbConfig = new DatabaseConfig
            {
                Backend = DatabaseBackend.Sqlite,
                SqlitePath = Path.Combine(dbFolder, "clipboard.db")
            };
            services.AddSingleton(dbConfig);

            services.AddSingleton<IClipboardRepository>(provider =>
            {
                var cfg = provider.GetRequiredService<DatabaseConfig>();
                return new SqliteClipboardRepository(cfg.SqlitePath);
            });

            services.AddSingleton<IClipboardMonitorService>(provider =>
            {
                var accessor = provider.GetRequiredService<IPlatformServicesAccessor>();
                return new ClipboardMonitorService(accessor.Clipboard);
            });

            services.AddSingleton<IClipboardService, ClipboardService>();
            services.AddHttpClient<ISyncService, SyncService>();

            // Fixed: register all ViewModels
            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<HomePageViewModel>();
            services.AddTransient<SettingsViewModel>();

            var provider = services.BuildServiceProvider();
            Services = provider;

            var homePageVm = provider.GetRequiredService<HomePageViewModel>();
            var settingsVm = provider.GetRequiredService<SettingsViewModel>();

            desktop.MainWindow = new MainWindow(homePageVm, settingsVm);

            var monitor = provider.GetRequiredService<IClipboardMonitorService>();
            monitor.ClipboardChanged += homePageVm.OnClipboardChanged;
            monitor.Start();

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