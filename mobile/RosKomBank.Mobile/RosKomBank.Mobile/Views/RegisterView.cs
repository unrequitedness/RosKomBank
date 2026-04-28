using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Material.Icons;
using static RosKomBank.Mobile.UiHelpers;
using static RosKomBank.Mobile.Views.LoginView;

namespace RosKomBank.Mobile.Views;

public class RegisterView : UserControl
{
    private readonly ShellView _shell;
    private readonly TextBox _name, _phone, _email, _passport, _inn, _addr, _pin1, _pin2;
    private readonly TextBlock _err;

    public RegisterView(ShellView shell)
    {
        _shell = shell;
        Background = Rgb(8, 18, 45);

        var hero = new Border
        {
            Height = 130,
            Background = Gradient(Color.FromRgb(0, 80, 200), Color.FromRgb(0, 200, 150), 135),
            CornerRadius = new CornerRadius(0, 0, 28, 28),
        };
        var heroPanel = new Grid();
        heroPanel.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        heroPanel.ColumnDefinitions.Add(new ColumnDefinition());

        var back = new Button
        {
            Content = Icon(MaterialIconKind.ArrowLeft, Brushes.White, 26),
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0),
            Padding = new Thickness(14),
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness(8, 14, 0, 0),
        };
        back.Click += (_, _) => _shell.ShowLogin();
        Grid.SetColumn(back, 0);
        heroPanel.Children.Add(back);

        var titleStack = new StackPanel
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 6, 40, 0),
        };
        titleStack.Children.Add(Tb("Регистрация", 22, Brushes.White, FontWeight.Bold, HorizontalAlignment.Center));
        titleStack.Children.Add(Tb("Создайте новый аккаунт", 12, Argb(220, 255, 255, 255), null, HorizontalAlignment.Center, new Thickness(0, 4, 0, 0)));
        Grid.SetColumn(titleStack, 1);
        heroPanel.Children.Add(titleStack);
        hero.Child = heroPanel;

        var card = new Border
        {
            Background = Rgb(13, 27, 62),
            CornerRadius = new CornerRadius(20),
            Padding = new Thickness(22),
            Margin = new Thickness(18, -28, 18, 22),
            BoxShadow = new BoxShadows(new BoxShadow
            {
                Blur = 26,
                OffsetY = 8,
                Color = Color.FromArgb(110, 0, 60, 180),
            }),
        };
        var form = new StackPanel();

        form.Children.Add(MakeLabel("ФИО"));
        _name = MakeInput("", "Иванов Иван Иванович");
        form.Children.Add(WrapField(MaterialIconKind.Account, _name));

        form.Children.Add(MakeLabel("Телефон"));
        _phone = MakeInput("", "79001234567");
        form.Children.Add(WrapField(MaterialIconKind.Phone, _phone));

        form.Children.Add(MakeLabel("Email"));
        _email = MakeInput("", "user@mail.ru");
        form.Children.Add(WrapField(MaterialIconKind.Email, _email));

        form.Children.Add(MakeLabel("Паспорт"));
        _passport = MakeInput("", "1234 567890");
        form.Children.Add(WrapField(MaterialIconKind.IdentificationCard, _passport));

        form.Children.Add(MakeLabel("ИНН"));
        _inn = MakeInput("", "123456789012");
        form.Children.Add(WrapField(MaterialIconKind.Numeric, _inn));

        form.Children.Add(MakeLabel("Адрес"));
        _addr = MakeInput("", "г. Москва, ул. ...");
        form.Children.Add(WrapField(MaterialIconKind.Home, _addr));

        form.Children.Add(MakeLabel("PIN-код (4 цифры)"));
        _pin1 = MakeInput();
        _pin1.PasswordChar = '●';
        _pin1.MaxLength = 4;
        form.Children.Add(WrapField(MaterialIconKind.Lock, _pin1));

        form.Children.Add(MakeLabel("Повторите PIN-код"));
        _pin2 = MakeInput();
        _pin2.PasswordChar = '●';
        _pin2.MaxLength = 4;
        form.Children.Add(WrapField(MaterialIconKind.LockCheck, _pin2));

        _err = Tb("", 12, Rgb(255, 90, 90), halign: HorizontalAlignment.Center, margin: new Thickness(0, 12, 0, 0), wrap: true);
        _err.IsVisible = false;
        form.Children.Add(_err);

        var regBtn = GradBtn("СОЗДАТЬ АККАУНТ", Color.FromRgb(0, 110, 230), Color.FromRgb(0, 190, 160));
        regBtn.Margin = new Thickness(0, 18, 0, 6);
        regBtn.Click += DoRegister;
        form.Children.Add(regBtn);

        var backBtn = new Button
        {
            Content = "Уже есть аккаунт? Войти",
            Background = Brushes.Transparent,
            Foreground = Rgb(0, 170, 255),
            BorderThickness = new Thickness(0),
            FontSize = 13,
            HorizontalAlignment = HorizontalAlignment.Center,
            Padding = new Thickness(8),
        };
        backBtn.Click += (_, _) => _shell.ShowLogin();
        form.Children.Add(backBtn);

        card.Child = form;

        var dock = new DockPanel { LastChildFill = true };
        DockPanel.SetDock(hero, Dock.Top);
        dock.Children.Add(hero);
        var scroll = new ScrollViewer
        {
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            Content = card,
        };
        dock.Children.Add(scroll);
        Content = dock;
    }

    private void DoRegister(object? sender, RoutedEventArgs e)
    {
        _err.IsVisible = false;
        var name = (_name.Text ?? "").Trim();
        var phone = (_phone.Text ?? "").Trim().Replace("+", "").Replace("-", "").Replace(" ", "");
        var email = (_email.Text ?? "").Trim();
        var pass = (_passport.Text ?? "").Trim();
        var inn = (_inn.Text ?? "").Trim();
        var addr = (_addr.Text ?? "").Trim();
        var pin1 = _pin1.Text ?? "";
        var pin2 = _pin2.Text ?? "";

        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(phone)
            || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(pass)
            || string.IsNullOrWhiteSpace(inn) || string.IsNullOrWhiteSpace(addr))
        {
            ShowErr("Заполните все поля"); return;
        }
        if (pin1.Length != 4 || !int.TryParse(pin1, out _))
        {
            ShowErr("PIN — ровно 4 цифры"); return;
        }
        if (pin1 != pin2)
        {
            ShowErr("PIN-коды не совпадают"); return;
        }
        if (Store.FindUserByPhone(phone) != null)
        {
            ShowErr("Этот номер уже зарегистрирован"); return;
        }

        var u = new User
        {
            FullName = name,
            Phone = phone,
            Email = email,
            PassportNumber = pass,
            INN = inn,
            Address = addr,
            PinHash = pin1,
            Balance = 0,
            CreditBalance = 0,
            CreditLimit = 100000,
            MonthlyPayment = 0,
            MatCapitalBalance = 0,
            HasMatCapital = false,
            TaxDebt = 0,
            CreatedAt = DateTime.Now,
        };
        Store.AddUser(u);
        App.CurrentUser = u;
        _shell.Snack("Аккаунт создан!");
        _shell.ShowMain();
    }

    private void ShowErr(string text)
    {
        _err.Text = text;
        _err.IsVisible = true;
    }
}
