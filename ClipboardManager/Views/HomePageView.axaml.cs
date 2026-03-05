using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ClipboardManager.ViewModels;

namespace ClipboardManager.Views;

public partial class HomePageView : UserControl
{
    
    public HomePageView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
  
        if (this.DataContext is HomePageViewModel viewModel)
        {
            viewModel.InitClipboardManager();
        }
    }

   
}