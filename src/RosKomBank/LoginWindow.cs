using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Material.Icons;
using Material.Icons.Avalonia;
using static RosKomBank.UiHelpers;

namespace RosKomBank;

public class LoginWindow : Window
{
    private TextBox _phone = null!;
    private TextBox _pin = null!;
    private TextBlock _err = null!;

    public LoginWindow()
    {
        Title = "РосКомБанк";
        Width = 420;
        Height = 560;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        CanResize = false;
        SystemDecorations = SystemDecorations.None;
        TransparencyLevelHint = new[] { WindowTransparencyLevel.Transparent };
        Background = Brushes.Transparent;
        ExtendClientAreaToDecorationsHint = true;

        var root = new Border
        {
            CornerRadius = new CornerRadius(20),
            Background = Rgb(13, 27, 62),
            BoxShadow = new BoxShadows(new BoxShadow
            {
                Blur = 30,
                OffsetX = 0,
                OffsetY = 0,
                Color = Color.FromArgb(115, 0, 100, 255), // ~0.45 opacity
                Spread = 0,
            }),
            Margin = new Thickness(8),
        };

        var panel = new StackPanel { Margin = new Thickness(38, 20, 38, 28) };

        var closeBtn = MakeTopBtn("✕");
        closeBtn.HorizontalAlignment = HorizontalAlignment.Right;
        closeBtn.Click += (_, _) => Close();
        panel.Children.Add(closeBtn);

        var logo = new Border
        {
            Width = 72,
            Height = 72,
            CornerRadius = new CornerRadius(36),
            Background = Gradient(Color.FromRgb(0, 120, 255), Color.FromRgb(0, 200, 150), 45),
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 8, 0, 10),
            Child = new TextBlock
            {
                Text = "₽",
                FontSize = 36,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            },
        };
        panel.Children.Add(logo);

        panel.Children.Add(new TextBlock
        {
            Text = "РосКомБанк",
            FontSize = 26,
            FontWeight = FontWeight.Bold,
            Foreground = Brushes.White,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 0, 0, 4),
        });
        panel.Children.Add(new TextBlock
        {
            Text = "Онлайн-банк",
            FontSize = 13,
            Foreground = Rgb(130, 165, 215),
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 0, 0, 26),
        });

        panel.Children.Add(MakeLabel("Номер телефона"));
        _phone = new TextBox
        {
            Background = Brushes.Transparent,
            Foreground = Brushes.White,
            BorderThickness = new Thickness(0),
            FontSize = 15,
            VerticalAlignment = VerticalAlignment.Center,
            Width = 295,
            Text = "79001234567",
            Padding = new Thickness(0),
        };
        panel.Children.Add(WrapCtrl(MaterialIconKind.Phone, _phone));

        panel.Children.Add(MakeLabel("PIN-код"));
        _pin = new TextBox
        {
            Background = Brushes.Transparent,
            Foreground = Brushes.White,
            BorderThickness = new Thickness(0),
            FontSize = 15,
            VerticalAlignment = VerticalAlignment.Center,
            Width = 295,
            MaxLength = 4,
            PasswordChar = '●',
            Text = "1234",
            Padding = new Thickness(0),
        };
        panel.Children.Add(WrapCtrl(MaterialIconKind.Lock, _pin));

        _err = new TextBlock
        {
            Foreground = Rgb(255, 80, 80),
            FontSize = 13,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 8, 0, 0),
            IsVisible = false,
        };
        panel.Children.Add(_err);

        var loginBtn = MakeGradBtn("ВОЙТИ В БАНК");
        loginBtn.Margin = new Thickness(0, 16, 0, 8);
        loginBtn.Click += DoLogin;
        panel.Children.Add(loginBtn);

        var regBtn = new Button
        {
            Content = "Нет аккаунта? Зарегистрироваться",
            Background = Brushes.Transparent,
            Foreground = Rgb(0, 170, 255),
            BorderThickness = new Thickness(0),
            FontSize = 13,
            Cursor = new Cursor(StandardCursorType.Hand),
            HorizontalAlignment = HorizontalAlignment.Center,
        };
        regBtn.Click += async (_, _) => await new RegisterWindow().ShowDialog(this);
        panel.Children.Add(regBtn);

        root.Child = panel;
        Content = root;

        PointerPressed += (_, e) =>
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed && e.Source is not TextBox)
            {
                try { BeginMoveDrag(e); } catch { }
            }
        };
    }

    private static Button MakeTopBtn(string t) => new()
    {
        Content = t,
        Background = Brushes.Transparent,
        BorderThickness = new Thickness(0),
        Foreground = Rgb(150, 150, 180),
        FontSize = 16,
        Cursor = new Cursor(StandardCursorType.Hand),
        Padding = new Thickness(8, 4),
    };

    private static TextBlock MakeLabel(string t) => new()
    {
        Text = t,
        Foreground = Rgb(130, 165, 215),
        FontSize = 12,
        Margin = new Thickness(0, 12, 0, 4),
    };

    private static Border WrapCtrl(MaterialIconKind icon, Control ctrl)
    {
        var b = new Border
        {
            Background = Rgb(22, 42, 88),
            CornerRadius = new CornerRadius(12),
            Padding = new Thickness(13, 10, 13, 10),
        };
        var sp = new StackPanel { Orientation = Orientation.Horizontal };
        sp.Children.Add(Icon(icon, Rgb(0, 140, 255), 20, new Thickness(0, 0, 10, 0)));
        sp.Children.Add(ctrl);
        b.Child = sp;
        return b;
    }

    private static Button MakeGradBtn(string text) => new()
    {
        Height = 46,
        Foreground = Brushes.White,
        FontSize = 15,
        FontWeight = FontWeight.Bold,
        BorderThickness = new Thickness(0),
        Cursor = new Cursor(StandardCursorType.Hand),
        Content = text,
        CornerRadius = new CornerRadius(12),
        Background = Gradient(Color.FromRgb(0, 110, 230), Color.FromRgb(0, 190, 160), 0),
        HorizontalAlignment = HorizontalAlignment.Stretch,
        HorizontalContentAlignment = HorizontalAlignment.Center,
        VerticalContentAlignment = VerticalAlignment.Center,
    };

    private void DoLogin(object? sender, RoutedEventArgs e)
    {
        var phone = (_phone.Text ?? "").Trim().Replace("+", "").Replace("-", "").Replace(" ", "");
        try
        {
            using var db = new BankContext();
            var pin = _pin.Text ?? "";
            var user = db.Users.FirstOrDefault(u => u.Phone == phone && u.PinHash == pin);
            if (user == null)
            {
                _err.Text = "Неверный номер или PIN";
                _err.IsVisible = true;
                return;
            }
            App.CurrentUser = user;
            var main = new MainBankWindow();
            main.Show();
            Close();
        }
        catch (Exception ex)
        {
            _err.Text = "Ошибка: " + ex.Message;
            _err.IsVisible = true;
        }
    }
}
