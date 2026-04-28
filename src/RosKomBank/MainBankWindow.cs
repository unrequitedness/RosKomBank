using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Material.Icons;
using Material.Icons.Avalonia;
using static RosKomBank.UiHelpers;

namespace RosKomBank;

public class MainBankWindow : Window
{
    private StackPanel _content = null!;
    private TextBlock _balText = null!;
    private Button? _activeNav;
    private User _user;
    private Grid _rootGrid = null!;

    public MainBankWindow()
    {
        Title = "РосКомБанк";
        _user = App.CurrentUser!;
        Width = 1180;
        Height = 780;
        MinWidth = 950;
        MinHeight = 600;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        SystemDecorations = SystemDecorations.None;
        TransparencyLevelHint = new[] { WindowTransparencyLevel.Transparent };
        Background = Brushes.Transparent;
        ExtendClientAreaToDecorationsHint = true;
        Build();
    }

    private void Build()
    {
        var root = new Border { Background = Rgb(8, 18, 45) };
        var mg = new Grid();
        _rootGrid = mg;
        mg.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(230) });
        mg.ColumnDefinitions.Add(new ColumnDefinition());

        var sidebar = BuildSidebar();
        Grid.SetColumn(sidebar, 0);

        var rg = new Grid();
        rg.RowDefinitions.Add(new RowDefinition { Height = new GridLength(54) });
        rg.RowDefinitions.Add(new RowDefinition());
        var topBar = BuildTopBar();
        Grid.SetRow(topBar, 0);
        var scroll = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
        _content = new StackPanel { Margin = new Thickness(26, 20, 26, 26) };
        scroll.Content = _content;
        Grid.SetRow(scroll, 1);
        rg.Children.Add(topBar);
        rg.Children.Add(scroll);
        Grid.SetColumn(rg, 1);

        mg.Children.Add(sidebar);
        mg.Children.Add(rg);
        root.Child = mg;
        Content = root;

        PointerPressed += (_, e) =>
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed
                && e.Source is Control src
                && src is not TextBox && src is not Button && src is not ComboBox)
            {
                try { BeginMoveDrag(e); } catch { }
            }
        };

        ShowDashboard();
    }

    private Border BuildTopBar()
    {
        var bar = new Border { Background = Rgb(11, 23, 55), Padding = new Thickness(20, 0, 14, 0) };
        var g = new Grid();
        g.ColumnDefinitions.Add(new ColumnDefinition());
        g.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        g.Children.Add(new TextBlock
        {
            Text = "Добрый день, " + (_user.FullName.Split(' ').ElementAtOrDefault(1) ?? _user.FullName) + "!",
            Foreground = Brushes.White,
            FontSize = 15,
            FontWeight = FontWeight.Medium,
            VerticalAlignment = VerticalAlignment.Center,
        });

        var rp = new StackPanel { Orientation = Orientation.Horizontal };
        rp.Children.Add(new TextBlock
        {
            Text = DateTime.Now.ToString("dd MMMM yyyy", new CultureInfo("ru-RU")),
            Foreground = Rgb(120, 155, 210),
            FontSize = 13,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, 0, 14, 0),
        });
        var minB = new Button
        {
            Content = "—",
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0),
            Foreground = Brushes.White,
            FontSize = 14,
            Cursor = new Cursor(StandardCursorType.Hand),
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, 0, 5, 0),
            Padding = new Thickness(8, 4),
        };
        minB.Click += (_, _) => WindowState = WindowState.Minimized;
        var clB = new Button
        {
            Content = "✕",
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0),
            Foreground = Rgb(220, 80, 80),
            FontSize = 15,
            Cursor = new Cursor(StandardCursorType.Hand),
            VerticalAlignment = VerticalAlignment.Center,
            Padding = new Thickness(8, 4),
        };
        clB.Click += (_, _) => Shutdown();
        rp.Children.Add(minB);
        rp.Children.Add(clB);
        Grid.SetColumn(rp, 1);
        g.Children.Add(rp);
        bar.Child = g;
        return bar;
    }

    private Border BuildSidebar()
    {
        var sb = new Border
        {
            Background = Gradient(Color.FromRgb(11, 23, 55), Color.FromRgb(8, 18, 45), 90),
            BorderBrush = Rgb(22, 46, 105),
            BorderThickness = new Thickness(0, 0, 1, 0),
        };
        var panel = new StackPanel();

        var la = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 20, 0, 16) };
        var lc = new Border
        {
            Width = 50,
            Height = 50,
            CornerRadius = new CornerRadius(25),
            Background = Gradient(Color.FromRgb(0, 120, 255), Color.FromRgb(0, 200, 150), 45),
            HorizontalAlignment = HorizontalAlignment.Center,
            Child = new TextBlock
            {
                Text = "₽",
                FontSize = 25,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            },
        };
        la.Children.Add(lc);
        la.Children.Add(new TextBlock
        {
            Text = "РосКомБанк",
            FontSize = 16,
            FontWeight = FontWeight.Bold,
            Foreground = Brushes.White,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 6, 0, 0),
        });
        panel.Children.Add(la);

        var bc = new Border
        {
            Background = Gradient(Color.FromRgb(0, 80, 200), Color.FromRgb(0, 160, 140), 135),
            CornerRadius = new CornerRadius(13),
            Padding = new Thickness(13, 10, 13, 10),
            Margin = new Thickness(11, 0, 11, 14),
        };
        var bs = new StackPanel();
        bs.Children.Add(new TextBlock
        {
            Text = "Основной счёт",
            FontSize = 10,
            Foreground = Argb(185, 255, 255, 255),
        });
        _balText = new TextBlock
        {
            Text = Fmt(_user.Balance),
            FontSize = 19,
            FontWeight = FontWeight.Bold,
            Foreground = Brushes.White,
        };
        bs.Children.Add(_balText);
        bs.Children.Add(new TextBlock
        {
            Text = "рублей",
            FontSize = 9,
            Foreground = Argb(155, 255, 255, 255),
        });
        bc.Child = bs;
        panel.Children.Add(bc);

        panel.Children.Add(new TextBlock
        {
            Text = "МЕНЮ",
            FontSize = 10,
            Foreground = Rgb(65, 100, 155),
            Margin = new Thickness(17, 0, 0, 5),
            FontWeight = FontWeight.Bold,
        });

        var navItems = new (MaterialIconKind, string, Action)[]
        {
            (MaterialIconKind.ViewDashboard, "Главная", ShowDashboard),
            (MaterialIconKind.CreditCard, "Счёт и карта", ShowCards),
            (MaterialIconKind.Send, "Переводы", ShowTransfer),
            (MaterialIconKind.History, "История", ShowHistory),
            (MaterialIconKind.Baby, "Мат. капитал", ShowMatCapital),
            (MaterialIconKind.FileDocument, "Кредиты", ShowCredit),
            (MaterialIconKind.AlertCircle, "Штрафы", ShowFines),
            (MaterialIconKind.Cash, "Налоги", ShowTaxes),
            (MaterialIconKind.Account, "Профиль", ShowProfile),
        };
        var first = true;
        foreach (var (icon, label, action) in navItems)
        {
            var btn = MakeNavBtn(icon, label, action);
            if (first) { NavOn(btn); _activeNav = btn; first = false; }
            panel.Children.Add(btn);
        }

        var lo = new Button
        {
            Height = 44,
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0),
            Cursor = new Cursor(StandardCursorType.Hand),
            Margin = new Thickness(9, 14, 9, 9),
            HorizontalContentAlignment = HorizontalAlignment.Left,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            CornerRadius = new CornerRadius(10),
        };
        var los = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(11, 0, 0, 0) };
        los.Children.Add(Icon(MaterialIconKind.Logout, Rgb(255, 80, 80), 19, new Thickness(0, 0, 9, 0)));
        los.Children.Add(new TextBlock
        {
            Text = "Выйти",
            Foreground = Rgb(255, 80, 80),
            FontSize = 13,
            VerticalAlignment = VerticalAlignment.Center,
        });
        lo.Content = los;
        lo.Click += (_, _) =>
        {
            App.CurrentUser = null;
            new LoginWindow().Show();
            Close();
        };
        panel.Children.Add(lo);

        sb.Child = panel;
        return sb;
    }

    private Button MakeNavBtn(MaterialIconKind icon, string label, Action action)
    {
        var btn = new Button
        {
            Height = 44,
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0),
            Cursor = new Cursor(StandardCursorType.Hand),
            Margin = new Thickness(9, 2, 9, 2),
            HorizontalContentAlignment = HorizontalAlignment.Left,
            VerticalContentAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            CornerRadius = new CornerRadius(10),
        };
        var sp = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(9, 0, 0, 0) };
        sp.Children.Add(Icon(icon, Rgb(85, 135, 215), 18, new Thickness(0, 0, 10, 0)));
        sp.Children.Add(new TextBlock
        {
            Text = label,
            Foreground = Rgb(165, 195, 250),
            FontSize = 13,
            VerticalAlignment = VerticalAlignment.Center,
        });
        btn.Content = sp;
        btn.Click += (_, _) =>
        {
            if (_activeNav != null) NavOff(_activeNav);
            NavOn(btn);
            _activeNav = btn;
            _content.Children.Clear();
            action();
        };
        return btn;
    }

    private void NavOn(Button b)
    {
        b.Background = Argb(52, 0, 130, 255);
        if (b.Content is StackPanel sp)
        {
            foreach (var c in sp.Children)
            {
                if (c is MaterialIcon pi) pi.Foreground = Rgb(0, 175, 255);
                if (c is TextBlock tb) { tb.Foreground = Brushes.White; tb.FontWeight = FontWeight.SemiBold; }
            }
        }
    }

    private void NavOff(Button b)
    {
        b.Background = Brushes.Transparent;
        if (b.Content is StackPanel sp)
        {
            foreach (var c in sp.Children)
            {
                if (c is MaterialIcon pi) pi.Foreground = Rgb(85, 135, 215);
                if (c is TextBlock tb) { tb.Foreground = Rgb(165, 195, 250); tb.FontWeight = FontWeight.Normal; }
            }
        }
    }

    private void Reload()
    {
        try
        {
            using var db = new BankContext();
            var u = db.Users.Find(_user.Id);
            if (u != null) { _user = u; App.CurrentUser = u; _balText.Text = Fmt(u.Balance); }
        }
        catch { }
    }

    private static string Fmt(decimal v) => v.ToString("N2", new CultureInfo("ru-RU")) + " ₽";

    private static bool TryMoney(string s, out decimal v)
    {
        v = 0;
        return decimal.TryParse(s.Replace(",", ".").Replace(" ", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out v);
    }

    private static TextBlock Sec(string t) => new()
    {
        Text = t,
        FontSize = 19,
        FontWeight = FontWeight.Bold,
        Foreground = Brushes.White,
        Margin = new Thickness(0, 4, 0, 13),
    };

    private static string IVal(TextBox tb) => tb.Tag?.ToString() == "ph" ? "" : (tb.Text ?? "").Trim();

    private static TextBox MakeInput(string ph)
    {
        var tb = new TextBox
        {
            Background = Rgb(20, 40, 85),
            Foreground = Rgb(105, 140, 190),
            BorderBrush = Rgb(38, 76, 175),
            BorderThickness = new Thickness(1),
            Height = 40,
            FontSize = 14,
            Padding = new Thickness(10, 0, 10, 0),
            VerticalContentAlignment = VerticalAlignment.Center,
            CaretBrush = Brushes.White,
            Text = ph,
            Tag = "ph",
            CornerRadius = new CornerRadius(6),
        };
        tb.GotFocus += (_, _) =>
        {
            if (tb.Tag?.ToString() == "ph") { tb.Text = ""; tb.Foreground = Brushes.White; tb.Tag = null; }
        };
        tb.LostFocus += (_, _) =>
        {
            if (string.IsNullOrEmpty(tb.Text)) { tb.Text = ph; tb.Foreground = Rgb(105, 140, 190); tb.Tag = "ph"; }
        };
        return tb;
    }

    private static Button MakeBtn(string text, MaterialIconKind icon)
    {
        var btn = new Button
        {
            Height = 41,
            Foreground = Brushes.White,
            FontSize = 13,
            FontWeight = FontWeight.SemiBold,
            BorderThickness = new Thickness(0),
            Cursor = new Cursor(StandardCursorType.Hand),
            CornerRadius = new CornerRadius(9),
            Background = Gradient(Color.FromRgb(0, 100, 220), Color.FromRgb(0, 170, 150), 0),
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center,
        };
        var sp = new StackPanel { Orientation = Orientation.Horizontal };
        sp.Children.Add(Icon(icon, Brushes.White, 16, new Thickness(0, 0, 7, 0)));
        sp.Children.Add(new TextBlock { Text = text, VerticalAlignment = VerticalAlignment.Center });
        btn.Content = sp;
        return btn;
    }

    private static Border MakeCard(string title, string val, MaterialIconKind icon, Color c1, Color c2, string? sub = null)
    {
        var card = new Border
        {
            Background = Gradient(c1, c2, 135),
            CornerRadius = new CornerRadius(15),
            Padding = new Thickness(17),
            Margin = new Thickness(0, 0, 13, 13),
            MinWidth = 180,
            BoxShadow = new BoxShadows(new BoxShadow
            {
                Blur = 16,
                Color = Color.FromArgb(85, c1.R, c1.G, c1.B),
            }),
        };
        var s = new StackPanel();
        var ib = new Border
        {
            Width = 40,
            Height = 40,
            CornerRadius = new CornerRadius(10),
            Background = Argb(50, 255, 255, 255),
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(0, 0, 0, 9),
            Child = Icon(icon, Brushes.White, 21),
        };
        s.Children.Add(ib);
        s.Children.Add(new TextBlock { Text = title, Foreground = Argb(185, 255, 255, 255), FontSize = 12 });
        s.Children.Add(new TextBlock
        {
            Text = val,
            Foreground = Brushes.White,
            FontSize = 19,
            FontWeight = FontWeight.Bold,
            Margin = new Thickness(0, 3, 0, 0),
        });
        if (sub != null)
        {
            s.Children.Add(new TextBlock
            {
                Text = sub,
                Foreground = Argb(165, 255, 255, 255),
                FontSize = 10,
            });
        }
        card.Child = s;
        return card;
    }

    private Border MakeTx(Transaction tx)
    {
        var row = new Border
        {
            Background = Rgb(15, 30, 67),
            CornerRadius = new CornerRadius(10),
            Padding = new Thickness(13, 9, 13, 9),
            Margin = new Thickness(0, 3, 0, 3),
        };
        var g = new Grid();
        g.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        g.ColumnDefinitions.Add(new ColumnDefinition());
        g.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        var bg = tx.Amount >= 0 ? Color.FromRgb(0, 105, 50) : Color.FromRgb(105, 22, 22);
        var ik = tx.Type switch
        {
            "transfer_in" => MaterialIconKind.ArrowDown,
            "transfer_out" => MaterialIconKind.ArrowUp,
            "matcap" => MaterialIconKind.Baby,
            "fine" => MaterialIconKind.AlertCircle,
            "tax" => MaterialIconKind.FileDocument,
            "topup" => MaterialIconKind.Plus,
            _ => MaterialIconKind.CreditCard,
        };
        var ib = new Border
        {
            Width = 36,
            Height = 36,
            CornerRadius = new CornerRadius(8),
            Background = new SolidColorBrush(bg),
            Margin = new Thickness(0, 0, 11, 0),
            VerticalAlignment = VerticalAlignment.Center,
            Child = Icon(ik, Brushes.White, 18),
        };
        Grid.SetColumn(ib, 0);
        var ts = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
        ts.Children.Add(new TextBlock { Text = tx.Description, Foreground = Brushes.White, FontSize = 13 });
        ts.Children.Add(new TextBlock
        {
            Text = tx.CreatedAt.ToString("dd.MM.yyyy HH:mm"),
            Foreground = Rgb(100, 135, 190),
            FontSize = 11,
        });
        Grid.SetColumn(ts, 1);
        var ac = tx.Amount >= 0 ? Color.FromRgb(0, 205, 115) : Color.FromRgb(255, 80, 80);
        var at = new TextBlock
        {
            Text = (tx.Amount >= 0 ? "+" : "") + Fmt(tx.Amount),
            Foreground = new SolidColorBrush(ac),
            FontSize = 14,
            FontWeight = FontWeight.Bold,
            VerticalAlignment = VerticalAlignment.Center,
        };
        Grid.SetColumn(at, 2);
        g.Children.Add(ib);
        g.Children.Add(ts);
        g.Children.Add(at);
        row.Child = g;
        return row;
    }

    private void Snack(string msg)
    {
        var sn = new Border
        {
            Background = Rgb(18, 48, 115),
            CornerRadius = new CornerRadius(10),
            Padding = new Thickness(17, 9, 17, 9),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Bottom,
            Margin = new Thickness(0, 0, 0, 26),
            BoxShadow = new BoxShadows(new BoxShadow
            {
                Blur = 16,
                Color = Color.FromArgb(107, 0, 0, 0),
            }),
            Child = new TextBlock { Text = msg, Foreground = Brushes.White, FontSize = 13 },
        };
        var ov = new Grid
        {
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            IsHitTestVisible = false,
        };
        ov.Children.Add(sn);
        Grid.SetColumnSpan(ov, 2);
        _rootGrid.Children.Add(ov);
        var ti = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2.5) };
        ti.Tick += (_, _) => { _rootGrid.Children.Remove(ov); ti.Stop(); };
        ti.Start();
    }

    private void ShowDashboard()
    {
        Reload();
        _content.Children.Add(Sec("Обзор счёта"));
        var wp = new WrapPanel();
        wp.Children.Add(MakeCard("Баланс", Fmt(_user.Balance), MaterialIconKind.Wallet, Color.FromRgb(0, 90, 210), Color.FromRgb(0, 170, 145)));
        wp.Children.Add(MakeCard("Кредит", Fmt(_user.CreditBalance), MaterialIconKind.CreditCard, Color.FromRgb(165, 32, 32), Color.FromRgb(205, 86, 0)));
        wp.Children.Add(MakeCard("Мат. капитал", Fmt(_user.MatCapitalBalance), MaterialIconKind.Baby, Color.FromRgb(86, 0, 185), Color.FromRgb(0, 86, 195)));
        wp.Children.Add(MakeCard("Долг по налогам", Fmt(_user.TaxDebt), MaterialIconKind.FileDocument, Color.FromRgb(185, 86, 0), Color.FromRgb(205, 145, 0)));
        _content.Children.Add(wp);

        _content.Children.Add(Sec("Последние операции"));
        try
        {
            using var db = new BankContext();
            var txs = db.Transactions.Where(t => t.UserId == _user.Id).OrderByDescending(t => t.CreatedAt).Take(6).ToList();
            if (!txs.Any())
                _content.Children.Add(new TextBlock { Text = "Нет операций", Foreground = Rgb(105, 135, 185), FontSize = 14 });
            else
                foreach (var tx in txs) _content.Children.Add(MakeTx(tx));
        }
        catch (Exception ex)
        {
            _content.Children.Add(new TextBlock { Text = "Ошибка: " + ex.Message, Foreground = Brushes.Red });
        }

        _content.Children.Add(Sec("Быстрые действия"));
        var qp = new WrapPanel();
        qp.Children.Add(MakeQuick(MaterialIconKind.Send, "Перевести", () => { _content.Children.Clear(); ShowTransfer(); }));
        qp.Children.Add(MakeQuick(MaterialIconKind.Plus, "Пополнить", () => { _content.Children.Clear(); ShowCards(); }));
        qp.Children.Add(MakeQuick(MaterialIconKind.AlertCircle, "Штрафы", () => { _content.Children.Clear(); ShowFines(); }));
        qp.Children.Add(MakeQuick(MaterialIconKind.FileDocument, "Кредит", () => { _content.Children.Clear(); ShowCredit(); }));
        _content.Children.Add(qp);
    }

    private static Border MakeQuick(MaterialIconKind icon, string label, Action action)
    {
        var b = new Border
        {
            Width = 100,
            Height = 90,
            CornerRadius = new CornerRadius(14),
            Background = Rgb(17, 36, 80),
            Margin = new Thickness(0, 0, 11, 11),
            Cursor = new Cursor(StandardCursorType.Hand),
        };
        var sp = new StackPanel { VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
        sp.Children.Add(new MaterialIcon
        {
            Kind = icon,
            Foreground = Rgb(0, 158, 255),
            Width = 27,
            Height = 27,
            HorizontalAlignment = HorizontalAlignment.Center,
        });
        sp.Children.Add(new TextBlock
        {
            Text = label,
            Foreground = Brushes.White,
            FontSize = 12,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 6, 0, 0),
        });
        b.Child = sp;
        b.PointerPressed += (_, _) => action();
        b.PointerEntered += (_, _) => b.Background = Rgb(26, 52, 110);
        b.PointerExited += (_, _) => b.Background = Rgb(17, 36, 80);
        return b;
    }

    private void ShowCards()
    {
        Reload();
        _content.Children.Add(Sec("Карта и счёт"));
        var cv = new Border
        {
            Width = 350,
            Height = 205,
            CornerRadius = new CornerRadius(18),
            Background = Gradient(Color.FromRgb(0, 80, 200), Color.FromRgb(0, 170, 150), 135),
            Margin = new Thickness(0, 0, 0, 20),
            HorizontalAlignment = HorizontalAlignment.Left,
            BoxShadow = new BoxShadows(new BoxShadow
            {
                Blur = 20,
                OffsetY = 4,
                Color = Color.FromArgb(107, 0, 100, 200),
            }),
        };
        var cg = new Grid { Margin = new Thickness(20) };
        for (int i = 0; i < 4; i++) cg.RowDefinitions.Add(new RowDefinition());
        var tr = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
        tr.Children.Add(new TextBlock { Text = "РосКомБанк", Foreground = Brushes.White, FontSize = 13, FontWeight = FontWeight.Bold });
        Grid.SetRow(tr, 0);
        var nm = new TextBlock
        {
            Text = "••••  ••••  ••••  4567",
            Foreground = Brushes.White,
            FontSize = 16,
            FontWeight = FontWeight.Bold,
            Margin = new Thickness(0, 18, 0, 0),
            FontFamily = new FontFamily("Courier New,monospace"),
        };
        Grid.SetRow(nm, 1);
        var br = new StackPanel { Margin = new Thickness(0, 7, 0, 0) };
        br.Children.Add(new TextBlock { Text = "БАЛАНС", Foreground = Argb(165, 255, 255, 255), FontSize = 9 });
        br.Children.Add(new TextBlock { Text = Fmt(_user.Balance), Foreground = Brushes.White, FontSize = 20, FontWeight = FontWeight.Bold });
        Grid.SetRow(br, 2);
        var bot = new Grid { Margin = new Thickness(0, 7, 0, 0) };
        bot.ColumnDefinitions.Add(new ColumnDefinition());
        bot.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        var ow = new StackPanel();
        ow.Children.Add(new TextBlock { Text = "ДЕРЖАТЕЛЬ", Foreground = Argb(150, 255, 255, 255), FontSize = 9 });
        ow.Children.Add(new TextBlock { Text = _user.FullName.ToUpper(), Foreground = Brushes.White, FontSize = 11 });
        Grid.SetColumn(ow, 0);
        var vl = new StackPanel { HorizontalAlignment = HorizontalAlignment.Right };
        vl.Children.Add(new TextBlock { Text = "ДО", Foreground = Argb(150, 255, 255, 255), FontSize = 9 });
        vl.Children.Add(new TextBlock { Text = _user.CreatedAt.AddYears(5).ToString("MM/yy"), Foreground = Brushes.White, FontSize = 11 });
        Grid.SetColumn(vl, 1);
        bot.Children.Add(ow);
        bot.Children.Add(vl);
        Grid.SetRow(bot, 3);
        cg.Children.Add(tr);
        cg.Children.Add(nm);
        cg.Children.Add(br);
        cg.Children.Add(bot);
        cv.Child = cg;
        _content.Children.Add(cv);

        _content.Children.Add(Sec("Реквизиты"));
        var det = new Border
        {
            Background = Rgb(15, 30, 67),
            CornerRadius = new CornerRadius(13),
            Padding = new Thickness(18),
            Margin = new Thickness(0, 0, 0, 18),
        };
        var ds = new StackPanel();
        void DR(string l, string v)
        {
            var g = new Grid { Margin = new Thickness(0, 5, 0, 5) };
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(160) });
            g.ColumnDefinitions.Add(new ColumnDefinition());
            g.Children.Add(new TextBlock { Text = l, Foreground = Rgb(105, 135, 190), FontSize = 13 });
            var vt = new TextBlock { Text = v, Foreground = Brushes.White, FontSize = 13 };
            Grid.SetColumn(vt, 1);
            g.Children.Add(vt);
            ds.Children.Add(g);
            ds.Children.Add(new Border { Background = Rgb(22, 46, 100), Height = 1, Margin = new Thickness(0, 1, 0, 1) });
        }
        DR("Номер счёта", "40817810" + _user.Id.ToString().PadLeft(8, '0'));
        DR("БИК", "044525225");
        DR("Банк", "АО «РосКомБанк»");
        DR("Корр. счёт", "30101810400000000225");
        det.Child = ds;
        _content.Children.Add(det);

        _content.Children.Add(Sec("Пополнить счёт"));
        var pb = new Border
        {
            Background = Rgb(15, 30, 67),
            CornerRadius = new CornerRadius(13),
            Padding = new Thickness(18),
            MaxWidth = 400,
            HorizontalAlignment = HorizontalAlignment.Left,
        };
        var ps = new StackPanel();
        ps.Children.Add(new TextBlock
        {
            Text = "Сумма пополнения",
            Foreground = Rgb(130, 165, 215),
            FontSize = 13,
            Margin = new Thickness(0, 0, 0, 6),
        });
        var ai = MakeInput("Введите сумму");
        ps.Children.Add(ai);
        var bb = MakeBtn("Пополнить", MaterialIconKind.Plus);
        bb.Margin = new Thickness(0, 9, 0, 0);
        bb.Click += (_, _) =>
        {
            if (!TryMoney(IVal(ai), out var v) || v <= 0) { Snack("Введите корректную сумму"); return; }
            try
            {
                using var db = new BankContext();
                var u = db.Users.Find(_user.Id)!;
                u.Balance += v;
                db.Transactions.Add(new Transaction { UserId = _user.Id, Type = "topup", Amount = v, Description = "Пополнение счёта", Status = "success", CreatedAt = DateTime.Now });
                db.SaveChanges();
                Reload();
                Snack("Счёт пополнен на " + Fmt(v));
                _content.Children.Clear();
                ShowCards();
            }
            catch (Exception ex) { Snack("Ошибка: " + ex.Message); }
        };
        ps.Children.Add(bb);
        pb.Child = ps;
        _content.Children.Add(pb);
    }

    private void ShowTransfer()
    {
        _content.Children.Add(Sec("Перевод средств"));
        var fb = new Border
        {
            Background = Rgb(15, 30, 67),
            CornerRadius = new CornerRadius(15),
            Padding = new Thickness(22),
            MaxWidth = 450,
            HorizontalAlignment = HorizontalAlignment.Left,
        };
        var fs = new StackPanel();
        fs.Children.Add(new TextBlock { Text = "Номер телефона получателя", Foreground = Rgb(130, 165, 215), FontSize = 13, Margin = new Thickness(0, 0, 0, 6) });
        var ri = MakeInput("79XXXXXXXXX");
        fs.Children.Add(ri);
        fs.Children.Add(new TextBlock { Text = "Сумма", Foreground = Rgb(130, 165, 215), FontSize = 13, Margin = new Thickness(0, 11, 0, 6) });
        var ai = MakeInput("Введите сумму");
        fs.Children.Add(ai);
        fs.Children.Add(new TextBlock { Text = "Комментарий", Foreground = Rgb(130, 165, 215), FontSize = 13, Margin = new Thickness(0, 11, 0, 6) });
        var ci = MakeInput("Необязательно");
        fs.Children.Add(ci);
        var err = new TextBlock
        {
            Foreground = Rgb(255, 80, 80),
            FontSize = 13,
            IsVisible = false,
            Margin = new Thickness(0, 8, 0, 0),
            TextWrapping = TextWrapping.Wrap,
        };
        fs.Children.Add(err);
        var btn = MakeBtn("Перевести", MaterialIconKind.Send);
        btn.Margin = new Thickness(0, 11, 0, 0);
        btn.Click += (_, _) =>
        {
            err.IsVisible = false;
            var phone = IVal(ri).Replace("+", "").Replace("-", "").Replace(" ", "");
            if (!TryMoney(IVal(ai), out var v) || v <= 0) { err.Text = "Введите корректную сумму"; err.IsVisible = true; return; }
            if (v > _user.Balance) { err.Text = "Недостаточно средств"; err.IsVisible = true; return; }
            if (phone == _user.Phone) { err.Text = "Нельзя переводить самому себе"; err.IsVisible = true; return; }
            try
            {
                using var db = new BankContext();
                var rec = db.Users.FirstOrDefault(u => u.Phone == phone);
                if (rec == null) { err.Text = "Получатель не найден"; err.IsVisible = true; return; }
                var sen = db.Users.Find(_user.Id)!;
                sen.Balance -= v;
                rec.Balance += v;
                var desc = string.IsNullOrEmpty(IVal(ci)) ? "Перевод: " + rec.FullName : IVal(ci);
                db.Transactions.Add(new Transaction { UserId = _user.Id, Type = "transfer_out", Amount = -v, Description = desc, Status = "success", RecipientPhone = phone, CreatedAt = DateTime.Now });
                db.Transactions.Add(new Transaction { UserId = rec.Id, Type = "transfer_in", Amount = v, Description = "Перевод от " + sen.FullName, Status = "success", RecipientPhone = _user.Phone, CreatedAt = DateTime.Now });
                db.SaveChanges();
                Reload();
                Snack("Переведено " + Fmt(v) + " → " + rec.FullName);
                _content.Children.Clear();
                ShowTransfer();
            }
            catch (Exception ex) { err.Text = "Ошибка: " + ex.Message; err.IsVisible = true; }
        };
        fs.Children.Add(btn);
        fb.Child = fs;
        _content.Children.Add(fb);
    }

    private void ShowHistory()
    {
        _content.Children.Add(Sec("История операций"));
        try
        {
            using var db = new BankContext();
            var txs = db.Transactions.Where(t => t.UserId == _user.Id).OrderByDescending(t => t.CreatedAt).ToList();
            if (!txs.Any())
            {
                _content.Children.Add(new TextBlock { Text = "История пуста", Foreground = Rgb(105, 135, 185), FontSize = 14 });
                return;
            }
            foreach (var g in txs.GroupBy(t => t.CreatedAt.ToString("MMMM yyyy", new CultureInfo("ru-RU"))))
            {
                _content.Children.Add(new TextBlock
                {
                    Text = g.Key.ToUpper(),
                    Foreground = Rgb(65, 105, 170),
                    FontSize = 11,
                    FontWeight = FontWeight.Bold,
                    Margin = new Thickness(0, 13, 0, 6),
                });
                foreach (var tx in g) _content.Children.Add(MakeTx(tx));
            }
        }
        catch (Exception ex)
        {
            _content.Children.Add(new TextBlock { Text = "Ошибка: " + ex.Message, Foreground = Brushes.Red });
        }
    }

    private void ShowMatCapital()
    {
        Reload();
        _content.Children.Add(Sec("Материнский капитал"));
        var ic = new Border
        {
            Background = Gradient(Color.FromRgb(72, 0, 170), Color.FromRgb(0, 78, 190), 135),
            CornerRadius = new CornerRadius(15),
            Padding = new Thickness(20),
            Margin = new Thickness(0, 0, 0, 16),
        };
        var is2 = new StackPanel();
        is2.Children.Add(new TextBlock { Text = "Остаток сертификата", Foreground = Argb(185, 255, 255, 255), FontSize = 13 });
        is2.Children.Add(new TextBlock
        {
            Text = Fmt(_user.MatCapitalBalance),
            Foreground = Brushes.White,
            FontSize = 28,
            FontWeight = FontWeight.Bold,
            Margin = new Thickness(0, 4, 0, 4),
        });
        is2.Children.Add(new TextBlock
        {
            Text = _user.HasMatCapital ? "✓ Сертификат активен" : "✗ Не оформлен",
            Foreground = _user.HasMatCapital ? Rgb(0, 205, 115) : Rgb(255, 95, 95),
            FontSize = 13,
        });
        ic.Child = is2;
        _content.Children.Add(ic);

        _content.Children.Add(Sec("Направления"));
        foreach (var (em, t, d) in new[]
        {
            ("🏠", "Жильё", "Покупка, строительство"),
            ("🎓", "Образование", "Дошкольное, вузовское"),
            ("👴", "Пенсия матери", "Накопительная часть"),
            ("♿", "Адаптация", "Для детей-инвалидов"),
        })
        {
            var row = new Border
            {
                Background = Rgb(15, 30, 67),
                CornerRadius = new CornerRadius(11),
                Padding = new Thickness(15, 11, 15, 11),
                Margin = new Thickness(0, 0, 0, 5),
            };
            var rg = new Grid();
            rg.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(42) });
            rg.ColumnDefinitions.Add(new ColumnDefinition());
            rg.Children.Add(new TextBlock { Text = em, FontSize = 24, VerticalAlignment = VerticalAlignment.Center });
            var rt = new StackPanel { VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0, 0, 0) };
            rt.Children.Add(new TextBlock { Text = t, Foreground = Brushes.White, FontSize = 13, FontWeight = FontWeight.Medium });
            rt.Children.Add(new TextBlock { Text = d, Foreground = Rgb(105, 135, 190), FontSize = 11 });
            Grid.SetColumn(rt, 1);
            rg.Children.Add(rt);
            row.Child = rg;
            _content.Children.Add(row);
        }
        if (!_user.HasMatCapital) return;

        _content.Children.Add(Sec("Подать заявку"));
        var fb = new Border
        {
            Background = Rgb(15, 30, 67),
            CornerRadius = new CornerRadius(13),
            Padding = new Thickness(18),
            MaxWidth = 430,
            HorizontalAlignment = HorizontalAlignment.Left,
        };
        var fs = new StackPanel();
        fs.Children.Add(new TextBlock { Text = "Направление", Foreground = Rgb(130, 165, 215), FontSize = 13, Margin = new Thickness(0, 0, 0, 6) });
        var combo = new ComboBox
        {
            Height = 37,
            FontSize = 13,
            Background = Rgb(20, 40, 85),
            Foreground = Brushes.White,
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };
        combo.ItemsSource = new[] { "Жильё", "Образование", "Пенсия матери", "Адаптация" };
        combo.SelectedIndex = 0;
        fs.Children.Add(combo);
        fs.Children.Add(new TextBlock { Text = "Сумма", Foreground = Rgb(130, 165, 215), FontSize = 13, Margin = new Thickness(0, 9, 0, 6) });
        var ai = MakeInput("Сумма");
        fs.Children.Add(ai);
        var btn = MakeBtn("Подать заявку", MaterialIconKind.Send);
        btn.Margin = new Thickness(0, 9, 0, 0);
        btn.Click += (_, _) =>
        {
            if (!TryMoney(IVal(ai), out var v) || v <= 0) { Snack("Введите сумму"); return; }
            if (v > _user.MatCapitalBalance) { Snack("Превышает остаток"); return; }
            try
            {
                using var db = new BankContext();
                var u = db.Users.Find(_user.Id)!;
                u.MatCapitalBalance -= v;
                db.Transactions.Add(new Transaction
                {
                    UserId = _user.Id,
                    Type = "matcap",
                    Amount = -v,
                    Description = "Мат. капитал: " + combo.SelectedItem,
                    Status = "pending",
                    CreatedAt = DateTime.Now,
                });
                db.SaveChanges();
                Reload();
                Snack("Заявка подана!");
                _content.Children.Clear();
                ShowMatCapital();
            }
            catch (Exception ex) { Snack("Ошибка: " + ex.Message); }
        };
        fs.Children.Add(btn);
        fb.Child = fs;
        _content.Children.Add(fb);
    }

    private void ShowCredit()
    {
        Reload();
        _content.Children.Add(Sec("Кредиты"));
        if (_user.CreditBalance > 0)
        {
            var ac = new Border
            {
                Background = Gradient(Color.FromRgb(165, 28, 28), Color.FromRgb(205, 86, 0), 135),
                CornerRadius = new CornerRadius(15),
                Padding = new Thickness(20),
                Margin = new Thickness(0, 0, 0, 16),
            };
            var acs = new StackPanel();
            acs.Children.Add(new TextBlock { Text = "Активный кредит", Foreground = Argb(185, 255, 255, 255), FontSize = 13 });
            acs.Children.Add(new TextBlock
            {
                Text = Fmt(_user.CreditBalance),
                Foreground = Brushes.White,
                FontSize = 26,
                FontWeight = FontWeight.Bold,
                Margin = new Thickness(0, 4, 0, 9),
            });
            var dg = new Grid();
            dg.ColumnDefinitions.Add(new ColumnDefinition());
            dg.ColumnDefinitions.Add(new ColumnDefinition());
            dg.ColumnDefinitions.Add(new ColumnDefinition());
            void CI(int col, string l, string v)
            {
                var ss = new StackPanel();
                ss.Children.Add(new TextBlock { Text = l, Foreground = Argb(155, 255, 255, 255), FontSize = 10 });
                ss.Children.Add(new TextBlock { Text = v, Foreground = Brushes.White, FontSize = 13, FontWeight = FontWeight.Bold });
                Grid.SetColumn(ss, col);
                dg.Children.Add(ss);
            }
            CI(0, "Ежемес. платёж", Fmt(_user.MonthlyPayment));
            CI(1, "Следующий", _user.NextPaymentDate?.ToString("dd.MM.yyyy") ?? "-");
            CI(2, "Ставка", "14.5%");
            acs.Children.Add(dg);
            var pb2 = MakeBtn("Внести платёж", MaterialIconKind.CashFast);
            pb2.Margin = new Thickness(0, 13, 0, 0);
            pb2.Width = 170;
            pb2.HorizontalAlignment = HorizontalAlignment.Left;
            pb2.Click += (_, _) =>
            {
                Reload();
                if (_user.Balance < _user.MonthlyPayment) { Snack("Недостаточно средств"); return; }
                try
                {
                    using var db = new BankContext();
                    var u = db.Users.Find(_user.Id)!;
                    var pay = Math.Min(u.MonthlyPayment, u.CreditBalance);
                    u.Balance -= pay;
                    u.CreditBalance -= pay;
                    if (u.CreditBalance <= 0) { u.CreditBalance = 0; u.NextPaymentDate = null; }
                    else u.NextPaymentDate = DateTime.Now.AddMonths(1);
                    db.Transactions.Add(new Transaction { UserId = _user.Id, Type = "credit", Amount = -pay, Description = "Платёж по кредиту", Status = "success", CreatedAt = DateTime.Now });
                    db.SaveChanges();
                    Reload();
                    Snack("Платёж " + Fmt(pay) + " внесён");
                    _content.Children.Clear();
                    ShowCredit();
                }
                catch (Exception ex) { Snack("Ошибка: " + ex.Message); }
            };
            acs.Children.Add(pb2);
            ac.Child = acs;
            _content.Children.Add(ac);
        }
        else
        {
            var fb = new Border
            {
                Background = Rgb(15, 30, 67),
                CornerRadius = new CornerRadius(15),
                Padding = new Thickness(22),
                MaxWidth = 450,
                HorizontalAlignment = HorizontalAlignment.Left,
            };
            var fs = new StackPanel();
            fs.Children.Add(new TextBlock { Text = "Сумма кредита", Foreground = Rgb(130, 165, 215), FontSize = 13, Margin = new Thickness(0, 0, 0, 6) });
            var ai = MakeInput("До 1 000 000 ₽");
            fs.Children.Add(ai);
            fs.Children.Add(new TextBlock { Text = "Срок", Foreground = Rgb(130, 165, 215), FontSize = 13, Margin = new Thickness(0, 11, 0, 6) });
            var combo = new ComboBox
            {
                Height = 37,
                FontSize = 13,
                Background = Rgb(20, 40, 85),
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            var rates = new[] { 0.145m, 0.160m, 0.185m, 0.210m };
            var mons = new[] { 12, 24, 36, 60 };
            combo.ItemsSource = new[] { "12 мес. (14.5%)", "24 мес. (16.0%)", "36 мес. (18.5%)", "60 мес. (21.0%)" };
            combo.SelectedIndex = 0;
            fs.Children.Add(combo);
            var cr = new Border
            {
                Background = Rgb(9, 20, 52),
                CornerRadius = new CornerRadius(9),
                Padding = new Thickness(13),
                Margin = new Thickness(0, 9, 0, 9),
                IsVisible = false,
            };
            var cs = new StackPanel();
            cr.Child = cs;
            fs.Children.Add(cr);
            var calcBtn = MakeBtn("Рассчитать", MaterialIconKind.Calculator);
            calcBtn.Margin = new Thickness(0, 5, 0, 5);
            calcBtn.Click += (_, _) =>
            {
                if (!TryMoney(IVal(ai), out var v) || v <= 0) { Snack("Введите сумму"); return; }
                var r = rates[combo.SelectedIndex] / 12;
                var n = mons[combo.SelectedIndex];
                var pay = v * r * (decimal)Math.Pow((double)(1 + r), n) / ((decimal)Math.Pow((double)(1 + r), n) - 1);
                cs.Children.Clear();
                cs.Children.Add(new TextBlock { Text = "Ежемесячный платёж: " + Fmt(Math.Round(pay, 2)), Foreground = Brushes.White, FontSize = 14, Margin = new Thickness(0, 0, 0, 3) });
                cs.Children.Add(new TextBlock { Text = "Переплата: " + Fmt(Math.Round(pay * n - v, 2)), Foreground = Rgb(255, 155, 50), FontSize = 13 });
                cs.Children.Add(new TextBlock { Text = "Итого: " + Fmt(Math.Round(pay * n, 2)), Foreground = Rgb(135, 190, 255), FontSize = 13 });
                cr.IsVisible = true;
            };
            fs.Children.Add(calcBtn);
            var applyBtn = MakeBtn("Оформить кредит", MaterialIconKind.FileDocumentCheck);
            applyBtn.Click += (_, _) =>
            {
                if (!TryMoney(IVal(ai), out var v) || v <= 0 || v > 1000000) { Snack("Сумма от 1 до 1 000 000 ₽"); return; }
                try
                {
                    var r = rates[combo.SelectedIndex] / 12;
                    var n = mons[combo.SelectedIndex];
                    var pay = v * r * (decimal)Math.Pow((double)(1 + r), n) / ((decimal)Math.Pow((double)(1 + r), n) - 1);
                    using var db = new BankContext();
                    var u = db.Users.Find(_user.Id)!;
                    u.Balance += v;
                    u.CreditBalance = v;
                    u.CreditLimit = v;
                    u.MonthlyPayment = Math.Round(pay, 2);
                    u.NextPaymentDate = DateTime.Now.AddMonths(1);
                    db.Transactions.Add(new Transaction { UserId = _user.Id, Type = "credit", Amount = v, Description = "Выдача кредита " + n + " мес.", Status = "success", CreatedAt = DateTime.Now });
                    db.SaveChanges();
                    Reload();
                    Snack("Кредит " + Fmt(v) + " зачислен!");
                    _content.Children.Clear();
                    ShowCredit();
                }
                catch (Exception ex) { Snack("Ошибка: " + ex.Message); }
            };
            fs.Children.Add(applyBtn);
            fb.Child = fs;
            _content.Children.Add(fb);
        }
    }

    private void ShowFines()
    {
        _content.Children.Add(Sec("Штрафы"));
        try
        {
            using var db = new BankContext();
            var fines = db.Fines.Where(f => f.UserId == _user.Id).OrderByDescending(f => f.IssuedAt).ToList();
            var unpaid = fines.Where(f => !f.IsPaid).Sum(f => f.Amount);
            var sc = new Border
            {
                Background = unpaid > 0
                    ? Gradient(Color.FromRgb(165, 42, 0), Color.FromRgb(205, 105, 0), 135)
                    : Gradient(Color.FromRgb(0, 105, 50), Color.FromRgb(0, 160, 85), 135),
                CornerRadius = new CornerRadius(13),
                Padding = new Thickness(17),
                Margin = new Thickness(0, 0, 0, 14),
            };
            var ss = new StackPanel { Orientation = Orientation.Horizontal };
            ss.Children.Add(new MaterialIcon
            {
                Kind = unpaid > 0 ? MaterialIconKind.AlertCircle : MaterialIconKind.CheckCircle,
                Foreground = Brushes.White,
                Width = 34,
                Height = 34,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 13, 0),
            });
            var st = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
            st.Children.Add(new TextBlock
            {
                Text = unpaid > 0 ? "Неоплачено: " + Fmt(unpaid) : "Все штрафы оплачены",
                Foreground = Brushes.White,
                FontSize = 15,
                FontWeight = FontWeight.Bold,
            });
            st.Children.Add(new TextBlock { Text = "Записей: " + fines.Count, Foreground = Argb(185, 255, 255, 255), FontSize = 12 });
            ss.Children.Add(st);
            sc.Child = ss;
            _content.Children.Add(sc);

            foreach (var fine in fines)
            {
                var fb = new Border
                {
                    Background = Rgb(15, 30, 67),
                    CornerRadius = new CornerRadius(12),
                    Padding = new Thickness(15, 11, 15, 11),
                    Margin = new Thickness(0, 0, 0, 6),
                    BorderBrush = fine.IsPaid ? Rgb(0, 95, 47) : Rgb(188, 70, 0),
                    BorderThickness = new Thickness(0, 0, 0, 2),
                };
                var fg = new Grid();
                fg.ColumnDefinitions.Add(new ColumnDefinition());
                fg.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                var fi = new StackPanel();
                fi.Children.Add(new TextBlock { Text = fine.Description, Foreground = Brushes.White, FontSize = 13, FontWeight = FontWeight.Medium });
                fi.Children.Add(new TextBlock { Text = "№ " + fine.FineNumber, Foreground = Rgb(105, 135, 190), FontSize = 10, Margin = new Thickness(0, 2, 0, 2) });
                fi.Children.Add(new TextBlock
                {
                    Text = "Выдан: " + fine.IssuedAt.ToString("dd.MM.yyyy") + (fine.IsPaid ? " | Оплачен: " + fine.PaidAt?.ToString("dd.MM.yyyy") : ""),
                    Foreground = Rgb(105, 135, 190),
                    FontSize = 11,
                });
                Grid.SetColumn(fi, 0);
                var fr = new StackPanel { VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Right };
                fr.Children.Add(new TextBlock
                {
                    Text = Fmt(fine.Amount),
                    Foreground = fine.IsPaid ? Rgb(0, 195, 95) : Rgb(255, 95, 48),
                    FontSize = 16,
                    FontWeight = FontWeight.Bold,
                    HorizontalAlignment = HorizontalAlignment.Right,
                });
                if (!fine.IsPaid)
                {
                    var payBtn = MakeBtn("Оплатить", MaterialIconKind.CashFast);
                    payBtn.Width = 110;
                    payBtn.Height = 32;
                    payBtn.FontSize = 12;
                    payBtn.Margin = new Thickness(0, 6, 0, 0);
                    var fid = fine.Id;
                    var famt = fine.Amount;
                    payBtn.Click += (_, _) =>
                    {
                        Reload();
                        if (_user.Balance < famt) { Snack("Недостаточно средств"); return; }
                        try
                        {
                            using var db2 = new BankContext();
                            var f = db2.Fines.Find(fid)!;
                            f.IsPaid = true;
                            f.PaidAt = DateTime.Now;
                            var u = db2.Users.Find(_user.Id)!;
                            u.Balance -= famt;
                            db2.Transactions.Add(new Transaction { UserId = _user.Id, Type = "fine", Amount = -famt, Description = "Штраф: " + f.Description, Status = "success", CreatedAt = DateTime.Now });
                            db2.SaveChanges();
                            Reload();
                            Snack("Штраф " + Fmt(famt) + " оплачен");
                            _content.Children.Clear();
                            ShowFines();
                        }
                        catch (Exception ex) { Snack("Ошибка: " + ex.Message); }
                    };
                    fr.Children.Add(payBtn);
                }
                else
                {
                    fr.Children.Add(new TextBlock { Text = "✓ Оплачен", Foreground = Rgb(0, 195, 95), FontSize = 12 });
                }
                Grid.SetColumn(fr, 1);
                fg.Children.Add(fi);
                fg.Children.Add(fr);
                fb.Child = fg;
                _content.Children.Add(fb);
            }
        }
        catch (Exception ex)
        {
            _content.Children.Add(new TextBlock { Text = "Ошибка: " + ex.Message, Foreground = Brushes.Red });
        }

        _content.Children.Add(Sec("Добавить штраф"));
        var ab = new Border
        {
            Background = Rgb(15, 30, 67),
            CornerRadius = new CornerRadius(13),
            Padding = new Thickness(18),
            MaxWidth = 430,
            HorizontalAlignment = HorizontalAlignment.Left,
        };
        var as2 = new StackPanel();
        as2.Children.Add(new TextBlock { Text = "Номер постановления", Foreground = Rgb(130, 165, 215), FontSize = 13, Margin = new Thickness(0, 0, 0, 6) });
        var ni = MakeInput("18 цифр");
        as2.Children.Add(ni);
        as2.Children.Add(new TextBlock { Text = "Описание", Foreground = Rgb(130, 165, 215), FontSize = 13, Margin = new Thickness(0, 9, 0, 6) });
        var di = MakeInput("Описание нарушения");
        as2.Children.Add(di);
        as2.Children.Add(new TextBlock { Text = "Сумма", Foreground = Rgb(130, 165, 215), FontSize = 13, Margin = new Thickness(0, 9, 0, 6) });
        var fi2 = MakeInput("Сумма штрафа");
        as2.Children.Add(fi2);
        var addBtn = MakeBtn("Добавить", MaterialIconKind.Plus);
        addBtn.Margin = new Thickness(0, 9, 0, 0);
        addBtn.Click += (_, _) =>
        {
            if (!TryMoney(IVal(fi2), out var v) || v <= 0 || string.IsNullOrEmpty(IVal(ni))) { Snack("Заполните все поля"); return; }
            try
            {
                using var db = new BankContext();
                db.Fines.Add(new Fine
                {
                    UserId = _user.Id,
                    FineNumber = IVal(ni),
                    Description = IVal(di),
                    Amount = v,
                    IsPaid = false,
                    IssuedAt = DateTime.Now,
                });
                db.SaveChanges();
                Snack("Штраф добавлен");
                _content.Children.Clear();
                ShowFines();
            }
            catch (Exception ex) { Snack("Ошибка: " + ex.Message); }
        };
        as2.Children.Add(addBtn);
        ab.Child = as2;
        _content.Children.Add(ab);
    }

    private void ShowTaxes()
    {
        Reload();
        _content.Children.Add(Sec("Налоги"));
        var tc = new Border
        {
            Background = _user.TaxDebt > 0
                ? Gradient(Color.FromRgb(150, 72, 0), Color.FromRgb(195, 130, 0), 135)
                : Gradient(Color.FromRgb(0, 95, 47), Color.FromRgb(0, 150, 76), 135),
            CornerRadius = new CornerRadius(14),
            Padding = new Thickness(18),
            Margin = new Thickness(0, 0, 0, 16),
        };
        var ts = new StackPanel();
        ts.Children.Add(new TextBlock { Text = "Задолженность по налогам", Foreground = Argb(185, 255, 255, 255), FontSize = 13 });
        ts.Children.Add(new TextBlock
        {
            Text = Fmt(_user.TaxDebt),
            Foreground = Brushes.White,
            FontSize = 26,
            FontWeight = FontWeight.Bold,
            Margin = new Thickness(0, 4, 0, 4),
        });
        ts.Children.Add(new TextBlock
        {
            Text = _user.TaxDebt > 0 ? "⚠ Требуется оплата" : "✓ Задолженностей нет",
            Foreground = Brushes.White,
            FontSize = 13,
        });
        tc.Child = ts;
        _content.Children.Add(tc);

        _content.Children.Add(Sec("Виды налогов"));
        foreach (var (icon, t, r) in new (MaterialIconKind, string, string)[]
        {
            (MaterialIconKind.AccountCash, "НДФЛ", "13%"),
            (MaterialIconKind.Home, "На имущество", "До 0.1%"),
            (MaterialIconKind.Car, "Транспортный", "По мощности"),
            (MaterialIconKind.MapMarker, "Земельный", "0.3%–1.5%"),
        })
        {
            var row = new Border
            {
                Background = Rgb(15, 30, 67),
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(13, 10, 13, 10),
                Margin = new Thickness(0, 0, 0, 5),
            };
            var rg = new Grid();
            rg.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(44) });
            rg.ColumnDefinitions.Add(new ColumnDefinition());
            rg.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            var ib = new Border
            {
                Width = 34,
                Height = 34,
                CornerRadius = new CornerRadius(8),
                Background = Rgb(22, 48, 120),
                VerticalAlignment = VerticalAlignment.Center,
                Child = Icon(icon, Rgb(0, 148, 255), 17),
            };
            var tt = new StackPanel { VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(9, 0, 0, 0) };
            tt.Children.Add(new TextBlock { Text = t, Foreground = Brushes.White, FontSize = 13, FontWeight = FontWeight.Medium });
            Grid.SetColumn(tt, 1);
            var rt = new TextBlock { Text = r, Foreground = Rgb(135, 165, 220), FontSize = 12, VerticalAlignment = VerticalAlignment.Center };
            Grid.SetColumn(rt, 2);
            rg.Children.Add(ib);
            rg.Children.Add(tt);
            rg.Children.Add(rt);
            row.Child = rg;
            _content.Children.Add(row);
        }

        if (_user.TaxDebt > 0)
        {
            _content.Children.Add(Sec("Оплатить"));
            var pb = new Border
            {
                Background = Rgb(15, 30, 67),
                CornerRadius = new CornerRadius(13),
                Padding = new Thickness(18),
                MaxWidth = 430,
                HorizontalAlignment = HorizontalAlignment.Left,
            };
            var ps = new StackPanel();
            ps.Children.Add(new TextBlock { Text = "К оплате: " + Fmt(_user.TaxDebt), Foreground = Brushes.White, FontSize = 15, Margin = new Thickness(0, 0, 0, 11) });
            var payBtn = MakeBtn("Оплатить задолженность", MaterialIconKind.CashFast);
            payBtn.Click += (_, _) =>
            {
                Reload();
                if (_user.Balance < _user.TaxDebt) { Snack("Недостаточно средств"); return; }
                try
                {
                    using var db = new BankContext();
                    var u = db.Users.Find(_user.Id)!;
                    var debt = u.TaxDebt;
                    u.Balance -= debt;
                    u.TaxDebt = 0;
                    db.Transactions.Add(new Transaction
                    {
                        UserId = _user.Id,
                        Type = "tax",
                        Amount = -debt,
                        Description = "Оплата налоговой задолженности",
                        Status = "success",
                        CreatedAt = DateTime.Now,
                    });
                    db.SaveChanges();
                    Reload();
                    Snack("Налоговая задолженность погашена!");
                    _content.Children.Clear();
                    ShowTaxes();
                }
                catch (Exception ex) { Snack("Ошибка: " + ex.Message); }
            };
            ps.Children.Add(payBtn);
            pb.Child = ps;
            _content.Children.Add(pb);
        }
    }

    private void ShowProfile()
    {
        Reload();
        _content.Children.Add(Sec("Профиль"));
        var initials = string.Join("", _user.FullName.Split(' ').Take(2).Select(w => w.Length > 0 ? w[0].ToString() : ""));
        var av = new Border
        {
            Width = 68,
            Height = 68,
            CornerRadius = new CornerRadius(34),
            Background = Gradient(Color.FromRgb(0, 120, 255), Color.FromRgb(0, 200, 150), 45),
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(0, 0, 0, 14),
            Child = new TextBlock
            {
                Text = initials,
                FontSize = 24,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontWeight = FontWeight.Bold,
            },
        };
        _content.Children.Add(av);

        var pc = new Border
        {
            Background = Rgb(15, 30, 67),
            CornerRadius = new CornerRadius(15),
            Padding = new Thickness(20),
            Margin = new Thickness(0, 0, 0, 16),
        };
        var ps = new StackPanel();
        void PF(string l, string v)
        {
            var g = new Grid { Margin = new Thickness(0, 6, 0, 6) };
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(175) });
            g.ColumnDefinitions.Add(new ColumnDefinition());
            g.Children.Add(new TextBlock { Text = l, Foreground = Rgb(105, 135, 190), FontSize = 13 });
            var vt = new TextBlock { Text = v, Foreground = Brushes.White, FontSize = 14, FontWeight = FontWeight.Medium };
            Grid.SetColumn(vt, 1);
            g.Children.Add(vt);
            ps.Children.Add(g);
            ps.Children.Add(new Border { Background = Rgb(22, 46, 100), Height = 1, Margin = new Thickness(0, 1, 0, 1) });
        }
        PF("ФИО", _user.FullName);
        PF("Телефон", "+7 " + (_user.Phone.Length > 1 ? _user.Phone[1..] : _user.Phone));
        PF("Email", _user.Email);
        PF("Паспорт", _user.PassportNumber);
        PF("ИНН", _user.INN);
        PF("Адрес", _user.Address);
        PF("Клиент с", _user.CreatedAt.ToString("dd MMMM yyyy", new CultureInfo("ru-RU")));
        pc.Child = ps;
        _content.Children.Add(pc);

        _content.Children.Add(Sec("Сменить PIN"));
        var pb2 = new Border
        {
            Background = Rgb(15, 30, 67),
            CornerRadius = new CornerRadius(13),
            Padding = new Thickness(18),
            MaxWidth = 390,
            HorizontalAlignment = HorizontalAlignment.Left,
        };
        var pf = new StackPanel();
        TextBox MakePin(string mar = "0,0,0,9")
        {
            return new TextBox
            {
                MaxLength = 4,
                Background = Rgb(20, 40, 85),
                Foreground = Brushes.White,
                BorderBrush = Rgb(38, 76, 175),
                Height = 37,
                FontSize = 14,
                Margin = new Thickness(0, 0, 0, 9),
                PasswordChar = '●',
                CornerRadius = new CornerRadius(6),
            };
        }
        pf.Children.Add(new TextBlock { Text = "Текущий PIN", Foreground = Rgb(130, 165, 215), FontSize = 13, Margin = new Thickness(0, 0, 0, 5) });
        var op = MakePin();
        pf.Children.Add(op);
        pf.Children.Add(new TextBlock { Text = "Новый PIN", Foreground = Rgb(130, 165, 215), FontSize = 13, Margin = new Thickness(0, 0, 0, 5) });
        var np = MakePin();
        pf.Children.Add(np);
        pf.Children.Add(new TextBlock { Text = "Подтвердите", Foreground = Rgb(130, 165, 215), FontSize = 13, Margin = new Thickness(0, 0, 0, 5) });
        var cp2 = MakePin();
        pf.Children.Add(cp2);
        var chBtn = MakeBtn("Изменить PIN", MaterialIconKind.Lock);
        chBtn.Click += (_, _) =>
        {
            if ((op.Text ?? "") != _user.PinHash) { Snack("Неверный текущий PIN"); return; }
            if ((np.Text ?? "") != (cp2.Text ?? "")) { Snack("Новые PIN не совпадают"); return; }
            var newPin = np.Text ?? "";
            if (newPin.Length != 4 || !newPin.All(char.IsDigit)) { Snack("PIN — 4 цифры"); return; }
            try
            {
                using var db = new BankContext();
                var u = db.Users.Find(_user.Id)!;
                u.PinHash = newPin;
                db.SaveChanges();
                Reload();
                Snack("PIN изменён");
            }
            catch (Exception ex) { Snack("Ошибка: " + ex.Message); }
        };
        pf.Children.Add(chBtn);
        pb2.Child = pf;
        _content.Children.Add(pb2);
    }
}
