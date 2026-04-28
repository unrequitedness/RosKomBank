using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using RosKomBank.Mobile.Views;

namespace RosKomBank.Mobile;

public partial class App : Application
{
    public static User? CurrentUser { get; set; }
    public static ShellView? Shell { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Store.SeedIfEmpty();

        Shell = new ShellView();
        Shell.ShowLogin();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new Window
            {
                Title = "РосКомБанк",
                Width = 420,
                Height = 760,
                Content = Shell,
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime sv)
        {
            sv.MainView = Shell;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
