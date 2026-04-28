using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Media;
using Material.Icons;
using Material.Icons.Avalonia;

namespace RosKomBank;

public static class UiHelpers
{
    /// <summary>
    /// Approximation of WPF's <c>new LinearGradientBrush(c1, c2, angleDegrees)</c>.
    /// In WPF, angle 0° is left→right, 90° is top→bottom (clockwise from +X axis, treating Y as down).
    /// </summary>
    public static LinearGradientBrush Gradient(Color c1, Color c2, double angleDegrees)
    {
        var rad = angleDegrees * Math.PI / 180.0;
        var x = Math.Cos(rad);
        var y = Math.Sin(rad);
        var sx = (1 - x) / 2;
        var sy = (1 - y) / 2;
        var ex = (1 + x) / 2;
        var ey = (1 + y) / 2;
        return new LinearGradientBrush
        {
            StartPoint = new RelativePoint(sx, sy, RelativeUnit.Relative),
            EndPoint = new RelativePoint(ex, ey, RelativeUnit.Relative),
            GradientStops =
            {
                new GradientStop(c1, 0),
                new GradientStop(c2, 1),
            },
        };
    }

    public static SolidColorBrush Rgb(byte r, byte g, byte b) => new(Color.FromRgb(r, g, b));
    public static SolidColorBrush Argb(byte a, byte r, byte g, byte b) => new(Color.FromArgb(a, r, g, b));

    public static MaterialIcon Icon(MaterialIconKind kind, IBrush fg, double size = 18, Thickness? margin = null)
        => new()
        {
            Kind = kind,
            Foreground = fg,
            Width = size,
            Height = size,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = margin ?? new Thickness(0),
        };

    public static void Shutdown()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime d)
            d.Shutdown();
    }

    public static void ShowInfo(Window? owner, string title, string text)
    {
        var w = new Window
        {
            Title = title,
            Width = 360,
            Height = 170,
            WindowStartupLocation = owner != null ? WindowStartupLocation.CenterOwner : WindowStartupLocation.CenterScreen,
            CanResize = false,
            SystemDecorations = SystemDecorations.BorderOnly,
            Background = Rgb(13, 27, 62),
        };
        var sp = new StackPanel { Margin = new Thickness(20) };
        sp.Children.Add(new TextBlock
        {
            Text = title,
            FontSize = 16,
            FontWeight = FontWeight.Bold,
            Foreground = Brushes.White,
            Margin = new Thickness(0, 0, 0, 8),
        });
        sp.Children.Add(new TextBlock
        {
            Text = text,
            FontSize = 13,
            Foreground = Rgb(180, 200, 230),
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 0, 0, 16),
        });
        var ok = new Button
        {
            Content = "OK",
            Width = 90,
            Height = 32,
            HorizontalAlignment = HorizontalAlignment.Right,
            Background = Rgb(0, 110, 230),
            Foreground = Brushes.White,
            CornerRadius = new CornerRadius(8),
            BorderThickness = new Thickness(0),
        };
        ok.Click += (_, _) => w.Close();
        sp.Children.Add(ok);
        w.Content = sp;
        if (owner != null) w.ShowDialog(owner);
        else w.Show();
    }
}
