using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using ContactToVCard.Services;
using ContactToVCard.ViewModels;
using ContactToVCard.Views;

namespace ContactToVCard;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = new MainWindow();
            var filePickerService = new AvaloniaFilePickerService(mainWindow);
            var convertContactService = new ConvertContactService();
            
            mainWindow.DataContext = new MainWindowViewModel(filePickerService, convertContactService);

            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}