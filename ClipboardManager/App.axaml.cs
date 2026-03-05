using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
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
            // If you use CommunityToolkit, line below is needed to remove Avalonia data validation.
            // Without this line you will get duplicate validations from both Avalonia and CT
            BindingPlugins.DataValidators.RemoveAt(0);

            // Register all the services needed for the application to run
            
            

            
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var services = new ServiceCollection();
                services.AddSingleton<IPlatformServicesAccessor>(new DefaultPlatformServiceAccessor(desktop));
                services.AddTransient<MainWindowViewModel>();
                
                var provider = services.BuildServiceProvider();
                
                var vm = provider.GetRequiredService<MainWindowViewModel>();
                
                DisableAvaloniaDataAnnotationValidation();
                desktop.MainWindow = new MainWindow
                {
                    DataContext = vm,
                };
            }
            
    
            base.OnFrameworkInitializationCompleted();
        }
        

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}