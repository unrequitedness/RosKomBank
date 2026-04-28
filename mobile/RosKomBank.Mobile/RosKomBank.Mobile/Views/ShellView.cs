using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using static RosKomBank.Mobile.UiHelpers;

namespace RosKomBank.Mobile.Views;

/// <summary>
/// Root mobile shell. Hosts a ContentControl whose content is swapped between
/// LoginView / RegisterView / MainView, plus a transient snackbar overlay.
/// </summary>
public class ShellView : UserControl
{
    private readonly ContentControl _content;
    private readonly Border _snack;
    private readonly TextBlock _snackText;
    private DispatcherTimer? _snackTimer;

    public ShellView()
    {
        Background = Rgb(8, 18, 45);

        var grid = new Grid();
        _content = new ContentControl();
        grid.Children.Add(_content);

        _snack = new Border
        {
            Background = Rgb(0, 110, 230),
            CornerRadius = new CornerRadius(12),
            Padding = new Thickness(18, 12),
            Margin = new Thickness(20, 0, 20, 30),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Bottom,
            IsVisible = false,
            BoxShadow = new BoxShadows(new BoxShadow
            {
                Blur = 16,
                OffsetY = 4,
                Color = Color.FromArgb(120, 0, 100, 255),
            }),
        };
        _snackText = Tb("", 13, Brushes.White, FontWeight.SemiBold, HorizontalAlignment.Center, wrap: true);
        _snack.Child = _snackText;
        grid.Children.Add(_snack);

        Content = grid;
    }

    public void ShowLogin()
    {
        _content.Content = new LoginView(this);
    }

    public void ShowRegister()
    {
        _content.Content = new RegisterView(this);
    }

    public void ShowMain()
    {
        _content.Content = new MainView(this);
    }

    public void Snack(string text, int seconds = 2)
    {
        _snackText.Text = text;
        _snack.IsVisible = true;
        _snackTimer?.Stop();
        _snackTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(seconds) };
        _snackTimer.Tick += (_, _) =>
        {
            _snack.IsVisible = false;
            _snackTimer?.Stop();
        };
        _snackTimer.Start();
    }
}
