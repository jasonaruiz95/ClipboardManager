using ClipboardManager.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ClipboardManager.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IPlatformServicesAccessor _platformServicesAccessor;
    [ObservableProperty] 
    private ViewModelBase _currentPage;
    
    public MainWindowViewModel(IPlatformServicesAccessor platformServicesAccessor)
    {
        // _clipboardService = clipboardService;
        _platformServicesAccessor = platformServicesAccessor;
        _currentPage = new HomePageViewModel();
    }
    
    
}