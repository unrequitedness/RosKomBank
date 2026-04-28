using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Material.Icons;
using Material.Icons.Avalonia;
using static RosKomBank.UiHelpers;

namespace RosKomBank;

public class RegisterWindow : Window
{
    private TextBox _name = null!, _phone = null!, _email = null!, _pass = null!, _inn = null!, _addr = null!;
    private TextBox _pin = null!, _pin2 = null!;
    private TextBlock _err = null!;

    public RegisterWindow()
    {
        Title = "Регистрация";
        Width = 470;
        Height = 660;
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
            Margin = new Thickness(8),
        };
        var scroll = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
        var panel = new StackPanel { Margin = new Thickness(36, 18, 36, 24) };

        var closeBtn = new Button
        {
            Content = "✕",
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0),
            Foreground = Rgb(150, 150, 180),
            FontSize = 16,
            HorizontalAlignment = HorizontalAlignment.Right,
            Cursor = new Cursor(StandardCursorType.Hand),
        };
        closeBtn.Click += (_, _) => Close();
        panel.Children.Add(closeBtn);

        panel.Children.Add(new TextBlock
        {
            Text = "Открыть счёт",
            FontSize = 23,
            FontWeight = FontWeight.Bold,
            Foreground = Brushes.White,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 0, 0, 18),
        });

        _name = AddField(panel, MaterialIconKind.Account, "ФИО", "Иванов Иван Иванович");
        _phone = AddField(panel, MaterialIconKind.Phone, "Телефон", "79001234567");
        _email = AddField(panel, MaterialIconKind.Email, "Email", "example@mail.ru");
        _pass = AddField(panel, MaterialIconKind.CardAccountDetails, "Паспорт", "4521 123456");
        _inn = AddField(panel, MaterialIconKind.FileDocument, "ИНН", "772012345678");
        _addr = AddField(panel, MaterialIconKind.MapMarker, "Адрес", "г. Москва...");

        panel.Children.Add(MakeLbl("PIN-код (4 цифры)"));
        _pin = MakePin();
        panel.Children.Add(WrapCtrl(MaterialIconKind.Lock, _pin));
        panel.Children.Add(MakeLbl("Подтвердите PIN"));
        _pin2 = MakePin();
        panel.Children.Add(WrapCtrl(MaterialIconKind.Lock, _pin2));

        _err = new TextBlock
        {
            Foreground = Rgb(255, 80, 80),
            FontSize = 13,
            HorizontalAlignment = HorizontalAlignment.Center,
            IsVisible = false,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 8, 0, 0),
        };
        panel.Children.Add(_err);

        var regBtn = MakeGradBtn("ЗАРЕГИСТРИРОВАТЬСЯ");
        regBtn.Margin = new Thickness(0, 12, 0, 6);
        regBtn.Click += DoRegister;
        panel.Children.Add(regBtn);

        var backBtn = new Button
        {
            Content = "← Назад",
            Background = Brushes.Transparent,
            Foreground = Rgb(0, 170, 255),
            BorderThickness = new Thickness(0),
            FontSize = 13,
            Cursor = new Cursor(StandardCursorType.Hand),
            HorizontalAlignment = HorizontalAlignment.Center,
        };
        backBtn.Click += (_, _) => Close();
        panel.Children.Add(backBtn);

        scroll.Content = panel;
        root.Child = scroll;
        Content = root;

        PointerPressed += (_, e) =>
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed && e.Source is not TextBox)
            {
                try { BeginMoveDrag(e); } catch { }
            }
        };
    }

    private static TextBox MakePin() => new()
    {
        MaxLength = 4,
        Background = Brushes.Transparent,
        Foreground = Brushes.White,
        BorderThickness = new Thickness(0),
        FontSize = 15,
        VerticalAlignment = VerticalAlignment.Center,
        Width = 355,
        PasswordChar = '●',
        Padding = new Thickness(0),
    };

    private static TextBlock MakeLbl(string t) => new()
    {
        Text = t,
        Foreground = Rgb(130, 165, 215),
        FontSize = 12,
        Margin = new Thickness(0, 10, 0, 4),
    };

    private static TextBox AddField(StackPanel panel, MaterialIconKind icon, string label, string ph)
    {
        panel.Children.Add(MakeLbl(label));
        var tb = new TextBox
        {
            Background = Brushes.Transparent,
            Foreground = Rgb(110, 145, 195),
            BorderThickness = new Thickness(0),
            FontSize = 14,
            Width = 355,
            VerticalAlignment = VerticalAlignment.Center,
            Text = ph,
            Tag = "ph",
            Padding = new Thickness(0),
        };
        tb.GotFocus += (_, _) =>
        {
            if (tb.Tag?.ToString() == "ph") { tb.Text = ""; tb.Foreground = Brushes.White; tb.Tag = null; }
        };
        tb.LostFocus += (_, _) =>
        {
            if (string.IsNullOrEmpty(tb.Text)) { tb.Text = ph; tb.Foreground = Rgb(110, 145, 195); tb.Tag = "ph"; }
        };
        panel.Children.Add(WrapCtrl(icon, tb));
        return tb;
    }

    private static Border WrapCtrl(MaterialIconKind icon, Control ctrl)
    {
        var b = new Border
        {
            Background = Rgb(22, 42, 88),
            CornerRadius = new CornerRadius(11),
            Padding = new Thickness(12, 9, 12, 9),
        };
        var sp = new StackPanel { Orientation = Orientation.Horizontal };
        sp.Children.Add(Icon(icon, Rgb(0, 140, 255), 18, new Thickness(0, 0, 8, 0)));
        sp.Children.Add(ctrl);
        b.Child = sp;
        return b;
    }

    private static Button MakeGradBtn(string text) => new()
    {
        Height = 44,
        Foreground = Brushes.White,
        FontSize = 14,
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

    private static string Val(TextBox tb) => tb.Tag?.ToString() == "ph" ? "" : (tb.Text ?? "").Trim();

    private void DoRegister(object? sender, RoutedEventArgs e)
    {
        _err.IsVisible = false;
        var name = Val(_name);
        var phone = Val(_phone).Replace("+", "").Replace("-", "").Replace(" ", "");
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(phone))
        {
            _err.Text = "Заполните ФИО и телефон"; _err.IsVisible = true; return;
        }
        var pin = _pin.Text ?? "";
        var pin2 = _pin2.Text ?? "";
        if (pin.Length != 4 || !pin.All(char.IsDigit))
        {
            _err.Text = "PIN — 4 цифры"; _err.IsVisible = true; return;
        }
        if (pin != pin2)
        {
            _err.Text = "PIN-коды не совпадают"; _err.IsVisible = true; return;
        }
        try
        {
            using var db = new BankContext();
            if (db.Users.Any(u => u.Phone == phone))
            {
                _err.Text = "Такой телефон уже зарегистрирован"; _err.IsVisible = true; return;
            }
            db.Users.Add(new User
            {
                FullName = name,
                Phone = phone,
                Email = Val(_email),
                PassportNumber = Val(_pass),
                INN = Val(_inn),
                Address = Val(_addr),
                PinHash = pin,
                Balance = 0,
                CreatedAt = DateTime.Now,
            });
            db.SaveChanges();
            ShowInfo(this, "Готово", "Счёт открыт! Войдите с вашими данными.");
            Close();
        }
        catch (Exception ex)
        {
            _err.Text = "Ошибка: " + ex.Message; _err.IsVisible = true;
        }
    }
}
