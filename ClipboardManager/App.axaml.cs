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

            // Clipboard monitor — resolved lazily via a factory so that
            // IClipboard is only accessed after the MainWindow is assigned.
            services.AddSingleton<IClipboardMonitorService>(provider =>
            {
                var accessor = provider.GetRequiredService<IPlatformServicesAccessor>();
                return new ClipboardMonitorService(accessor.Clipboard);
            });

            services.AddTransient<MainWindowViewModel>();

            var provider = services.BuildServiceProvider();

            // 1. Create the window and assign DataContext
            var vm = provider.GetRequiredService<MainWindowViewModel>();
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
            desktop.Exit += (_, _) => monitor.Stop();
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