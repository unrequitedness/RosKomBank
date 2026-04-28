using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Material.Icons;
using Material.Icons.Avalonia;

namespace RosKomBank.Mobile;

public static class UiHelpers
{
    private static readonly CultureInfo Ru = new("ru-RU");

    public static string Fmt(decimal v) => v.ToString("N2", Ru) + " ₽";

    /// <summary>
    /// Approximation of WPF's <c>new LinearGradientBrush(c1, c2, angleDegrees)</c>.
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

    public static TextBlock Tb(string text, double size = 13, IBrush? fg = null, FontWeight? w = null, HorizontalAlignment? halign = null, Thickness? margin = null, bool wrap = false)
        => new()
        {
            Text = text,
            FontSize = size,
            Foreground = fg ?? Brushes.White,
            FontWeight = w ?? FontWeight.Normal,
            HorizontalAlignment = halign ?? HorizontalAlignment.Left,
            Margin = margin ?? new Thickness(0),
            TextWrapping = wrap ? TextWrapping.Wrap : TextWrapping.NoWrap,
        };

    public static Button GradBtn(string text, Color c1, Color c2, double height = 50, double angle = 0)
        => new()
        {
            Height = height,
            Foreground = Brushes.White,
            FontSize = 15,
            FontWeight = FontWeight.Bold,
            BorderThickness = new Thickness(0),
            Content = text,
            CornerRadius = new CornerRadius(14),
            Background = Gradient(c1, c2, angle),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center,
        };
}
