using Avalonia.Controls;
using Avalonia.Platform;
using ClipboardManager.Services.Interfaces;
using ClipboardManager.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace ClipboardManager.Services;

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection collection)
    {
        collection.AddSingleton<IClipboardService, ClipboardService>();
        collection.AddTransient<MainWindowViewModel>();
        collection.AddTransient<HomePageViewModel>();
    }
}