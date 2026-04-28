using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Material.Icons;
using static RosKomBank.Mobile.UiHelpers;

namespace RosKomBank.Mobile.Views;

public class LoginView : UserControl
{
    private readonly ShellView _shell;
    private readonly TextBox _phone;
    private readonly TextBox _pin;
    private readonly TextBlock _err;

    public LoginView(ShellView shell)
    {
        _shell = shell;
        Background = Rgb(8, 18, 45);

        // Top hero band with gradient
        var hero = new Border
        {
            Height = 220,
            Background = Gradient(Color.FromRgb(0, 80, 200), Color.FromRgb(0, 200, 150), 135),
            CornerRadius = new CornerRadius(0, 0, 32, 32),
        };
        var heroPanel = new StackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
        };
        var logo = new Border
        {
            Width = 78,
            Height = 78,
            CornerRadius = new CornerRadius(39),
            Background = Argb(50, 255, 255, 255),
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 0, 0, 12),
            Child = new TextBlock
            {
                Text = "₽",
                FontSize = 40,
                FontWeight = FontWeight.Bold,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            },
        };
        heroPanel.Children.Add(logo);
        heroPanel.Children.Add(Tb("РосКомБанк", 28, Brushes.White, FontWeight.Bold, HorizontalAlignment.Center));
        heroPanel.Children.Add(Tb("Мобильный банк", 13, Argb(220, 255, 255, 255), null, HorizontalAlignment.Center, new Thickness(0, 4, 0, 0)));
        hero.Child = heroPanel;

        // Form card
        var card = new Border
        {
            Background = Rgb(13, 27, 62),
            CornerRadius = new CornerRadius(20),
            Padding = new Thickness(24),
            Margin = new Thickness(20, -36, 20, 20),
            BoxShadow = new BoxShadows(new BoxShadow
            {
                Blur = 30,
                OffsetY = 8,
                Color = Color.FromArgb(115, 0, 60, 180),
            }),
        };
        var form = new StackPanel();
        form.Children.Add(Tb("Вход в банк", 20, Brushes.White, FontWeight.Bold, margin: new Thickness(0, 0, 0, 6)));
        form.Children.Add(Tb("Введите телефон и PIN-код", 12, Rgb(130, 165, 215), margin: new Thickness(0, 0, 0, 18)));

        form.Children.Add(MakeLabel("Номер телефона"));
        _phone = MakeInput("79001234567");
        form.Children.Add(WrapField(MaterialIconKind.Phone, _phone));

        form.Children.Add(MakeLabel("PIN-код"));
        _pin = MakeInput("1234");
        _pin.PasswordChar = '●';
        _pin.MaxLength = 4;
        form.Children.Add(WrapField(MaterialIconKind.Lock, _pin));

        _err = Tb("", 12, Rgb(255, 90, 90), halign: HorizontalAlignment.Center, margin: new Thickness(0, 12, 0, 0), wrap: true);
        _err.IsVisible = false;
        form.Children.Add(_err);

        var loginBtn = GradBtn("ВОЙТИ В БАНК", Color.FromRgb(0, 110, 230), Color.FromRgb(0, 190, 160));
        loginBtn.Margin = new Thickness(0, 18, 0, 10);
        loginBtn.Click += DoLogin;
        form.Children.Add(loginBtn);

        var regBtn = new Button
        {
            Content = "Нет аккаунта? Зарегистрироваться",
            Background = Brushes.Transparent,
            Foreground = Rgb(0, 170, 255),
            BorderThickness = new Thickness(0),
            FontSize = 13,
            HorizontalAlignment = HorizontalAlignment.Center,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            Padding = new Thickness(8),
        };
        regBtn.Click += (_, _) => _shell.ShowRegister();
        form.Children.Add(regBtn);

        card.Child = form;

        var root = new DockPanel { LastChildFill = true };
        DockPanel.SetDock(hero, Dock.Top);
        root.Children.Add(hero);

        var scroll = new ScrollViewer
        {
            HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Disabled,
            VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
            Content = card,
            Margin = new Thickness(0, 0, 0, 0),
        };
        root.Children.Add(scroll);

        Content = root;
    }

    internal static TextBlock MakeLabel(string t) => Tb(t, 11, Rgb(130, 165, 215), margin: new Thickness(2, 10, 0, 6));

    internal static TextBox MakeInput(string text = "", string watermark = "") => new()
    {
        Background = Brushes.Transparent,
        Foreground = Brushes.White,
        BorderThickness = new Thickness(0),
        FontSize = 15,
        VerticalAlignment = VerticalAlignment.Center,
        Text = text,
        Watermark = watermark,
        Padding = new Thickness(0),
        CaretBrush = Brushes.White,
        SelectionBrush = Argb(120, 0, 170, 255),
    };

    internal static Border WrapField(MaterialIconKind icon, Control ctrl)
    {
        var b = new Border
        {
            Background = Rgb(22, 42, 88),
            CornerRadius = new CornerRadius(14),
            Padding = new Thickness(14, 12),
            Height = 50,
        };
        var sp = new StackPanel { Orientation = Orientation.Horizontal };
        sp.Children.Add(Icon(icon, Rgb(0, 140, 255), 20, new Thickness(0, 0, 12, 0)));
        ctrl.HorizontalAlignment = HorizontalAlignment.Stretch;
        sp.Children.Add(ctrl);
        b.Child = sp;
        return b;
    }

    private void DoLogin(object? sender, RoutedEventArgs e)
    {
        var phone = (_phone.Text ?? "").Trim().Replace("+", "").Replace("-", "").Replace(" ", "");
        var pin = _pin.Text ?? "";
        var u = Store.FindUserByPhonePin(phone, pin);
        if (u == null)
        {
            _err.Text = "Неверный номер или PIN";
            _err.IsVisible = true;
            return;
        }
        App.CurrentUser = u;
        _shell.ShowMain();
    }
}
