using System;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using Material.Icons;
using Material.Icons.Avalonia;
using static RosKomBank.Mobile.UiHelpers;

namespace RosKomBank.Mobile.Views;

public class MainView : UserControl
{
    private readonly ShellView _shell;
    private User _user;
    private StackPanel _content = null!;
    private TextBlock _balText = null!;
    private TextBlock _titleText = null!;
    private Border _drawer = null!;
    private Border _scrim = null!;
    private string _currentSection = "dashboard";

    public MainView(ShellView shell)
    {
        _shell = shell;
        _user = App.CurrentUser!;
        Background = Rgb(8, 18, 45);
        Build();
    }

    private void Build()
    {
        var dock = new DockPanel { LastChildFill = true };

        // Top bar (app bar)
        var topBar = BuildTopBar();
        DockPanel.SetDock(topBar, Dock.Top);
        dock.Children.Add(topBar);

        // Balance hero card under top bar
        var hero = BuildHero();
        DockPanel.SetDock(hero, Dock.Top);
        dock.Children.Add(hero);

        // Bottom nav
        var bottom = BuildBottomNav();
        DockPanel.SetDock(bottom, Dock.Bottom);
        dock.Children.Add(bottom);

        // Scrollable content
        _content = new StackPanel { Margin = new Thickness(16, 6, 16, 16) };
        var scroll = new ScrollViewer
        {
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            Content = _content,
        };
        dock.Children.Add(scroll);

        // Drawer overlay (left slide-in)
        _scrim = new Border
        {
            Background = Argb(140, 0, 0, 0),
            IsVisible = false,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
        };
        _scrim.PointerPressed += (_, _) => CloseDrawer();

        _drawer = BuildDrawer();
        _drawer.IsVisible = false;

        var rootGrid = new Grid();
        rootGrid.Children.Add(dock);
        rootGrid.Children.Add(_scrim);
        rootGrid.Children.Add(_drawer);
        Content = rootGrid;

        ShowDashboard();
    }

    private Border BuildTopBar()
    {
        var bar = new Border
        {
            Background = Rgb(11, 23, 55),
            Padding = new Thickness(8, 6),
            Height = 56,
        };
        var g = new Grid();
        g.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        g.ColumnDefinitions.Add(new ColumnDefinition());
        g.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var menuBtn = new Button
        {
            Content = Icon(MaterialIconKind.Menu, Brushes.White, 24),
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0),
            Padding = new Thickness(12),
            VerticalAlignment = VerticalAlignment.Center,
            CornerRadius = new CornerRadius(22),
        };
        menuBtn.Click += (_, _) => OpenDrawer();
        Grid.SetColumn(menuBtn, 0);

        _titleText = Tb("РосКомБанк", 18, Brushes.White, FontWeight.SemiBold,
            HorizontalAlignment.Center,
            new Thickness(0, 0, 0, 0));
        _titleText.VerticalAlignment = VerticalAlignment.Center;
        Grid.SetColumn(_titleText, 1);

        var bell = new Button
        {
            Content = Icon(MaterialIconKind.BellOutline, Brushes.White, 22),
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0),
            Padding = new Thickness(12),
            VerticalAlignment = VerticalAlignment.Center,
            CornerRadius = new CornerRadius(22),
        };
        bell.Click += (_, _) => _shell.Snack("Новых уведомлений нет");
        Grid.SetColumn(bell, 2);

        g.Children.Add(menuBtn);
        g.Children.Add(_titleText);
        g.Children.Add(bell);
        bar.Child = g;
        return bar;
    }

    private Border BuildHero()
    {
        var hero = new Border
        {
            Background = Gradient(Color.FromRgb(0, 80, 200), Color.FromRgb(0, 170, 150), 135),
            CornerRadius = new CornerRadius(0, 0, 24, 24),
            Padding = new Thickness(20, 14, 20, 22),
        };
        var s = new StackPanel();
        s.Children.Add(Tb("Добрый день, " + (_user.FullName.Split(' ').ElementAtOrDefault(1) ?? _user.FullName) + "!",
            14, Argb(220, 255, 255, 255), FontWeight.Medium));
        s.Children.Add(Tb("Доступно на счёте", 11, Argb(170, 255, 255, 255), null, null, new Thickness(0, 12, 0, 2)));
        _balText = Tb(Fmt(_user.Balance), 28, Brushes.White, FontWeight.Bold);
        s.Children.Add(_balText);
        hero.Child = s;
        return hero;
    }

    private Border BuildBottomNav()
    {
        var bar = new Border
        {
            Background = Rgb(11, 23, 55),
            Height = 64,
            BorderBrush = Rgb(22, 46, 100),
            BorderThickness = new Thickness(0, 1, 0, 0),
        };
        var g = new Grid();
        for (int i = 0; i < 5; i++) g.ColumnDefinitions.Add(new ColumnDefinition());

        var items = new (MaterialIconKind ic, string label, string key, Action act)[]
        {
            (MaterialIconKind.ViewDashboard, "Главная", "dashboard", ShowDashboard),
            (MaterialIconKind.CreditCard, "Карта", "cards", ShowCards),
            (MaterialIconKind.Send, "Перевод", "transfer", ShowTransfer),
            (MaterialIconKind.History, "История", "history", ShowHistory),
            (MaterialIconKind.Menu, "Ещё", "more", () => OpenDrawer()),
        };
        for (int i = 0; i < items.Length; i++)
        {
            var (ic, label, key, act) = items[i];
            var b = MakeBottomNavItem(ic, label, key, act);
            Grid.SetColumn(b, i);
            g.Children.Add(b);
        }
        bar.Child = g;
        return bar;
    }

    private Button MakeBottomNavItem(MaterialIconKind ic, string label, string key, Action act)
    {
        var btn = new Button
        {
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0),
            Padding = new Thickness(0),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center,
            CornerRadius = new CornerRadius(0),
            Tag = key,
        };
        var s = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center };
        var iconCtl = Icon(ic, Rgb(120, 155, 210), 22);
        iconCtl.HorizontalAlignment = HorizontalAlignment.Center;
        s.Children.Add(iconCtl);
        s.Children.Add(Tb(label, 10, Rgb(120, 155, 210), null, HorizontalAlignment.Center, new Thickness(0, 4, 0, 0)));
        btn.Content = s;
        btn.Click += (_, _) => act();
        return btn;
    }

    private void HighlightBottom()
    {
        if (Content is not Grid root) return;
        if (root.Children.OfType<DockPanel>().FirstOrDefault() is not DockPanel dp) return;
        var bottom = dp.Children.OfType<Border>().LastOrDefault(b => b.Height == 64);
        if (bottom?.Child is not Grid g) return;
        foreach (var btn in g.Children.OfType<Button>())
        {
            var key = btn.Tag as string;
            var active = key == _currentSection || (key == "more" && IsExtraSection(_currentSection));
            if (btn.Content is StackPanel sp)
            {
                foreach (var c in sp.Children)
                {
                    if (c is MaterialIcon mi) mi.Foreground = active ? Rgb(0, 170, 255) : Rgb(120, 155, 210);
                    if (c is TextBlock tb)
                    {
                        tb.Foreground = active ? Rgb(0, 170, 255) : Rgb(120, 155, 210);
                        tb.FontWeight = active ? FontWeight.Bold : FontWeight.Normal;
                    }
                }
            }
        }
    }

    private static bool IsExtraSection(string key) => key is "matcap" or "credit" or "fines" or "taxes" or "profile";

    private Border BuildDrawer()
    {
        var d = new Border
        {
            Width = 280,
            Background = Rgb(11, 23, 55),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Stretch,
            BorderBrush = Rgb(22, 46, 105),
            BorderThickness = new Thickness(0, 0, 1, 0),
            Padding = new Thickness(0),
            BoxShadow = new BoxShadows(new BoxShadow
            {
                Blur = 30,
                OffsetX = 4,
                Color = Color.FromArgb(180, 0, 0, 0),
            }),
        };
        var sp = new StackPanel();

        // Header
        var hdr = new Border
        {
            Padding = new Thickness(20, 28, 20, 20),
            Background = Gradient(Color.FromRgb(0, 80, 200), Color.FromRgb(0, 170, 150), 135),
        };
        var hp = new StackPanel();
        var ava = new Border
        {
            Width = 56,
            Height = 56,
            CornerRadius = new CornerRadius(28),
            Background = Argb(70, 255, 255, 255),
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(0, 0, 0, 10),
            Child = Tb(GetInitials(_user.FullName), 22, Brushes.White, FontWeight.Bold, HorizontalAlignment.Center)
        };
        if (ava.Child is TextBlock t) t.VerticalAlignment = VerticalAlignment.Center;
        hp.Children.Add(ava);
        hp.Children.Add(Tb(_user.FullName, 16, Brushes.White, FontWeight.Bold));
        hp.Children.Add(Tb("+7 " + (_user.Phone.Length > 1 ? _user.Phone[1..] : _user.Phone), 12, Argb(220, 255, 255, 255), margin: new Thickness(0, 2, 0, 0)));
        hdr.Child = hp;
        sp.Children.Add(hdr);

        // Items
        var items = new (MaterialIconKind ic, string label, string key, Action act)[]
        {
            (MaterialIconKind.ViewDashboard, "Главная", "dashboard", ShowDashboard),
            (MaterialIconKind.CreditCard, "Счёт и карта", "cards", ShowCards),
            (MaterialIconKind.Send, "Переводы", "transfer", ShowTransfer),
            (MaterialIconKind.History, "История", "history", ShowHistory),
            (MaterialIconKind.Baby, "Мат. капитал", "matcap", ShowMatCapital),
            (MaterialIconKind.FileDocument, "Кредиты", "credit", ShowCredit),
            (MaterialIconKind.AlertCircle, "Штрафы", "fines", ShowFines),
            (MaterialIconKind.Cash, "Налоги", "taxes", ShowTaxes),
            (MaterialIconKind.Account, "Профиль", "profile", ShowProfile),
        };
        foreach (var (ic, label, _, act) in items)
        {
            var btn = MakeDrawerItem(ic, label, () => { CloseDrawer(); act(); });
            sp.Children.Add(btn);
        }
        sp.Children.Add(new Border { Background = Rgb(22, 46, 100), Height = 1, Margin = new Thickness(12, 8) });
        var logoutBtn = MakeDrawerItem(MaterialIconKind.Logout, "Выйти", () =>
        {
            CloseDrawer();
            App.CurrentUser = null;
            _shell.ShowLogin();
        }, danger: true);
        sp.Children.Add(logoutBtn);

        var scroll = new ScrollViewer { Content = sp, VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
        d.Child = scroll;
        return d;
    }

    private Button MakeDrawerItem(MaterialIconKind ic, string label, Action act, bool danger = false)
    {
        var btn = new Button
        {
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Left,
            CornerRadius = new CornerRadius(10),
            Margin = new Thickness(8, 1),
            Padding = new Thickness(12, 12),
        };
        var s = new StackPanel { Orientation = Orientation.Horizontal };
        s.Children.Add(Icon(ic, danger ? Rgb(255, 90, 90) : Rgb(0, 158, 255), 22, new Thickness(0, 0, 14, 0)));
        s.Children.Add(Tb(label, 14, danger ? Rgb(255, 110, 110) : Brushes.White, FontWeight.Medium));
        btn.Content = s;
        btn.Click += (_, _) => act();
        return btn;
    }

    private void OpenDrawer()
    {
        _drawer.IsVisible = true;
        _scrim.IsVisible = true;
    }

    private void CloseDrawer()
    {
        _drawer.IsVisible = false;
        _scrim.IsVisible = false;
    }

    private static string GetInitials(string fullName)
        => string.Join("", fullName.Split(' ').Take(2).Select(w => w.Length > 0 ? w[0].ToString() : "")).ToUpper();

    private void ResetAndSet(string key, string title)
    {
        _currentSection = key;
        _titleText.Text = title;
        _content.Children.Clear();
        Reload();
        HighlightBottom();
    }

    private void Reload()
    {
        var u = Store.FindUserById(_user.Id);
        if (u != null)
        {
            _user = u;
            App.CurrentUser = u;
            _balText.Text = Fmt(u.Balance);
        }
    }

    private static bool TryMoney(string s, out decimal v)
    {
        v = 0;
        return decimal.TryParse(s.Replace(",", ".").Replace(" ", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out v);
    }

    private static TextBlock Sec(string t) => Tb(t, 18, Brushes.White, FontWeight.Bold, margin: new Thickness(0, 14, 0, 10));

    // ============================ DASHBOARD ============================

    private void ShowDashboard()
    {
        ResetAndSet("dashboard", "Главная");

        // Card grid (2 per row)
        var wp = new UniformGrid { Columns = 2, Margin = new Thickness(0, 0, 0, 6) };
        wp.Children.Add(MakeCard("Баланс", Fmt(_user.Balance), MaterialIconKind.Wallet, Color.FromRgb(0, 90, 210), Color.FromRgb(0, 170, 145)));
        wp.Children.Add(MakeCard("Кредит", Fmt(_user.CreditBalance), MaterialIconKind.CreditCard, Color.FromRgb(165, 32, 32), Color.FromRgb(205, 86, 0)));
        wp.Children.Add(MakeCard("Мат. капитал", Fmt(_user.MatCapitalBalance), MaterialIconKind.Baby, Color.FromRgb(86, 0, 185), Color.FromRgb(0, 86, 195)));
        wp.Children.Add(MakeCard("Долг налоги", Fmt(_user.TaxDebt), MaterialIconKind.FileDocument, Color.FromRgb(185, 86, 0), Color.FromRgb(205, 145, 0)));
        _content.Children.Add(wp);

        _content.Children.Add(Sec("Быстрые действия"));
        var qp = new UniformGrid { Columns = 4, Margin = new Thickness(0, 0, 0, 8) };
        qp.Children.Add(MakeQuick(MaterialIconKind.Send, "Перевод", ShowTransfer));
        qp.Children.Add(MakeQuick(MaterialIconKind.Plus, "Пополнить", ShowCards));
        qp.Children.Add(MakeQuick(MaterialIconKind.AlertCircle, "Штрафы", ShowFines));
        qp.Children.Add(MakeQuick(MaterialIconKind.FileDocument, "Кредит", ShowCredit));
        _content.Children.Add(qp);

        _content.Children.Add(Sec("Последние операции"));
        var txs = Store.UserTxs(_user.Id).Take(8).ToList();
        if (txs.Count == 0)
            _content.Children.Add(Tb("Нет операций", 13, Rgb(105, 135, 185)));
        else foreach (var tx in txs) _content.Children.Add(MakeTx(tx));
    }

    private static Border MakeCard(string title, string val, MaterialIconKind ic, Color c1, Color c2)
    {
        var card = new Border
        {
            Background = Gradient(c1, c2, 135),
            CornerRadius = new CornerRadius(16),
            Padding = new Thickness(14),
            Margin = new Thickness(4, 4, 4, 4),
            Height = 120,
            BoxShadow = new BoxShadows(new BoxShadow
            {
                Blur = 14,
                OffsetY = 4,
                Color = Color.FromArgb(85, c1.R, c1.G, c1.B),
            }),
        };
        var s = new StackPanel();
        var ib = new Border
        {
            Width = 36,
            Height = 36,
            CornerRadius = new CornerRadius(10),
            Background = Argb(60, 255, 255, 255),
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(0, 0, 0, 7),
            Child = Icon(ic, Brushes.White, 19),
        };
        s.Children.Add(ib);
        s.Children.Add(Tb(title, 11, Argb(195, 255, 255, 255)));
        s.Children.Add(Tb(val, 15, Brushes.White, FontWeight.Bold, margin: new Thickness(0, 2, 0, 0), wrap: true));
        card.Child = s;
        return card;
    }

    private Border MakeQuick(MaterialIconKind ic, string label, Action act)
    {
        var b = new Border
        {
            Margin = new Thickness(3),
            CornerRadius = new CornerRadius(14),
            Background = Rgb(17, 36, 80),
            Padding = new Thickness(8, 12),
        };
        var sp = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center };
        var ic2 = Icon(ic, Rgb(0, 158, 255), 26);
        ic2.HorizontalAlignment = HorizontalAlignment.Center;
        sp.Children.Add(ic2);
        sp.Children.Add(Tb(label, 11, Brushes.White, halign: HorizontalAlignment.Center, margin: new Thickness(0, 6, 0, 0)));
        b.Child = sp;
        b.PointerPressed += (_, _) => act();
        return b;
    }

    private Border MakeTx(Transaction tx)
    {
        var row = new Border
        {
            Background = Rgb(15, 30, 67),
            CornerRadius = new CornerRadius(12),
            Padding = new Thickness(13, 10),
            Margin = new Thickness(0, 4, 0, 4),
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
            Width = 40,
            Height = 40,
            CornerRadius = new CornerRadius(10),
            Background = new SolidColorBrush(bg),
            Margin = new Thickness(0, 0, 12, 0),
            VerticalAlignment = VerticalAlignment.Center,
            Child = Icon(ik, Brushes.White, 20),
        };
        Grid.SetColumn(ib, 0);
        var ts = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
        ts.Children.Add(Tb(tx.Description, 13, Brushes.White));
        ts.Children.Add(Tb(tx.CreatedAt.ToString("dd.MM.yyyy HH:mm"), 11, Rgb(100, 135, 190)));
        Grid.SetColumn(ts, 1);
        var ac = tx.Amount >= 0 ? Color.FromRgb(0, 205, 115) : Color.FromRgb(255, 80, 80);
        var at = Tb((tx.Amount >= 0 ? "+" : "") + Fmt(tx.Amount), 13, new SolidColorBrush(ac), FontWeight.Bold);
        at.VerticalAlignment = VerticalAlignment.Center;
        Grid.SetColumn(at, 2);
        g.Children.Add(ib);
        g.Children.Add(ts);
        g.Children.Add(at);
        row.Child = g;
        return row;
    }

    // ============================ SHARED FORM HELPERS ============================

    private static TextBox MakeFInput(string watermark = "")
    {
        return new TextBox
        {
            Background = Rgb(20, 40, 85),
            Foreground = Brushes.White,
            BorderBrush = Rgb(38, 76, 175),
            BorderThickness = new Thickness(1),
            Height = 46,
            FontSize = 14,
            Padding = new Thickness(12, 0),
            VerticalContentAlignment = VerticalAlignment.Center,
            CaretBrush = Brushes.White,
            Watermark = watermark,
            CornerRadius = new CornerRadius(10),
        };
    }

    private static Button MakePrimary(string text, MaterialIconKind ic)
    {
        var btn = new Button
        {
            Height = 50,
            Foreground = Brushes.White,
            FontSize = 14,
            FontWeight = FontWeight.SemiBold,
            BorderThickness = new Thickness(0),
            CornerRadius = new CornerRadius(12),
            Background = Gradient(Color.FromRgb(0, 100, 220), Color.FromRgb(0, 170, 150), 0),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center,
        };
        var sp = new StackPanel { Orientation = Orientation.Horizontal };
        sp.Children.Add(Icon(ic, Brushes.White, 18, new Thickness(0, 0, 8, 0)));
        sp.Children.Add(Tb(text, 14, Brushes.White, FontWeight.SemiBold));
        btn.Content = sp;
        return btn;
    }

    private static Border CardBox(StackPanel inner, Thickness? margin = null)
        => new()
        {
            Background = Rgb(15, 30, 67),
            CornerRadius = new CornerRadius(16),
            Padding = new Thickness(18),
            Margin = margin ?? new Thickness(0, 0, 0, 14),
            Child = inner,
        };

    private static TextBlock FieldLabel(string t, double topMargin = 0)
        => Tb(t, 12, Rgb(130, 165, 215), margin: new Thickness(0, topMargin, 0, 6));

    // ============================ CARDS ============================

    private void ShowCards()
    {
        ResetAndSet("cards", "Счёт и карта");
        // Card visual
        var cv = new Border
        {
            Height = 200,
            CornerRadius = new CornerRadius(18),
            Background = Gradient(Color.FromRgb(0, 80, 200), Color.FromRgb(0, 170, 150), 135),
            Margin = new Thickness(0, 0, 0, 16),
            BoxShadow = new BoxShadows(new BoxShadow
            {
                Blur = 20,
                OffsetY = 6,
                Color = Color.FromArgb(120, 0, 100, 200),
            }),
        };
        var cg = new Grid { Margin = new Thickness(20) };
        for (int i = 0; i < 4; i++) cg.RowDefinitions.Add(new RowDefinition());

        var top = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
        top.Children.Add(Tb("РосКомБанк", 13, Brushes.White, FontWeight.Bold));
        Grid.SetRow(top, 0);

        var nm = new TextBlock
        {
            Text = "••••  ••••  ••••  4567",
            Foreground = Brushes.White,
            FontSize = 18,
            FontWeight = FontWeight.Bold,
            Margin = new Thickness(0, 18, 0, 0),
            FontFamily = new FontFamily("Courier New,monospace"),
        };
        Grid.SetRow(nm, 1);

        var br = new StackPanel { Margin = new Thickness(0, 8, 0, 0) };
        br.Children.Add(Tb("БАЛАНС", 9, Argb(165, 255, 255, 255)));
        br.Children.Add(Tb(Fmt(_user.Balance), 22, Brushes.White, FontWeight.Bold));
        Grid.SetRow(br, 2);

        var bot = new Grid { Margin = new Thickness(0, 8, 0, 0) };
        bot.ColumnDefinitions.Add(new ColumnDefinition());
        bot.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        var ow = new StackPanel();
        ow.Children.Add(Tb("ДЕРЖАТЕЛЬ", 9, Argb(150, 255, 255, 255)));
        ow.Children.Add(Tb(_user.FullName.ToUpper(), 11, Brushes.White));
        Grid.SetColumn(ow, 0);
        var vl = new StackPanel { HorizontalAlignment = HorizontalAlignment.Right };
        vl.Children.Add(Tb("ДО", 9, Argb(150, 255, 255, 255)));
        vl.Children.Add(Tb(_user.CreatedAt.AddYears(5).ToString("MM/yy"), 11, Brushes.White));
        Grid.SetColumn(vl, 1);
        bot.Children.Add(ow);
        bot.Children.Add(vl);
        Grid.SetRow(bot, 3);

        cg.Children.Add(top);
        cg.Children.Add(nm);
        cg.Children.Add(br);
        cg.Children.Add(bot);
        cv.Child = cg;
        _content.Children.Add(cv);

        // Details card
        _content.Children.Add(Sec("Реквизиты"));
        var dsp = new StackPanel();
        AddDetailRow(dsp, "Номер счёта", "40817810" + _user.Id.ToString().PadLeft(8, '0'));
        AddDetailRow(dsp, "БИК", "044525225");
        AddDetailRow(dsp, "Банк", "АО «РосКомБанк»");
        AddDetailRow(dsp, "Корр. счёт", "30101810400000000225");
        _content.Children.Add(CardBox(dsp));

        // Top up card
        _content.Children.Add(Sec("Пополнить счёт"));
        var ps = new StackPanel();
        ps.Children.Add(FieldLabel("Сумма пополнения"));
        var ai = MakeFInput("Введите сумму");
        ps.Children.Add(ai);
        var bb = MakePrimary("Пополнить", MaterialIconKind.Plus);
        bb.Margin = new Thickness(0, 12, 0, 0);
        bb.Click += (_, _) =>
        {
            if (!TryMoney(ai.Text ?? "", out var v) || v <= 0) { _shell.Snack("Введите корректную сумму"); return; }
            var u = Store.FindUserById(_user.Id)!;
            u.Balance += v;
            Store.AddTransaction(new Transaction
            {
                UserId = _user.Id,
                Type = "topup",
                Amount = v,
                Description = "Пополнение счёта",
                Status = "success",
                CreatedAt = DateTime.Now
            });
            _shell.Snack("Счёт пополнен на " + Fmt(v));
            ShowCards();
        };
        ps.Children.Add(bb);
        _content.Children.Add(CardBox(ps));
    }

    private static void AddDetailRow(StackPanel ds, string l, string v)
    {
        var g = new Grid { Margin = new Thickness(0, 5, 0, 5) };
        g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(140) });
        g.ColumnDefinitions.Add(new ColumnDefinition());
        g.Children.Add(Tb(l, 12, Rgb(105, 135, 190)));
        var vt = Tb(v, 12, Brushes.White);
        Grid.SetColumn(vt, 1);
        vt.TextWrapping = TextWrapping.Wrap;
        g.Children.Add(vt);
        ds.Children.Add(g);
        ds.Children.Add(new Border { Background = Rgb(22, 46, 100), Height = 1, Margin = new Thickness(0, 1, 0, 1) });
    }

    // ============================ TRANSFER ============================

    private void ShowTransfer()
    {
        ResetAndSet("transfer", "Перевод");
        var fs = new StackPanel();
        fs.Children.Add(FieldLabel("Номер телефона получателя"));
        var ri = MakeFInput("79XXXXXXXXX");
        fs.Children.Add(ri);
        fs.Children.Add(FieldLabel("Сумма", 12));
        var ai = MakeFInput("Сумма");
        fs.Children.Add(ai);
        fs.Children.Add(FieldLabel("Комментарий", 12));
        var ci = MakeFInput("Необязательно");
        fs.Children.Add(ci);

        var err = Tb("", 12, Rgb(255, 90, 90), wrap: true);
        err.Margin = new Thickness(0, 10, 0, 0);
        err.IsVisible = false;
        fs.Children.Add(err);

        var btn = MakePrimary("Перевести", MaterialIconKind.Send);
        btn.Margin = new Thickness(0, 14, 0, 0);
        btn.Click += (_, _) =>
        {
            err.IsVisible = false;
            var phone = (ri.Text ?? "").Trim().Replace("+", "").Replace("-", "").Replace(" ", "");
            if (!TryMoney(ai.Text ?? "", out var v) || v <= 0) { err.Text = "Введите корректную сумму"; err.IsVisible = true; return; }
            if (v > _user.Balance) { err.Text = "Недостаточно средств"; err.IsVisible = true; return; }
            if (phone == _user.Phone) { err.Text = "Нельзя переводить самому себе"; err.IsVisible = true; return; }
            var rec = Store.FindUserByPhone(phone);
            if (rec == null) { err.Text = "Получатель не найден"; err.IsVisible = true; return; }
            var sen = Store.FindUserById(_user.Id)!;
            sen.Balance -= v;
            rec.Balance += v;
            var desc = string.IsNullOrEmpty((ci.Text ?? "").Trim()) ? "Перевод: " + rec.FullName : ci.Text!.Trim();
            Store.AddTransaction(new Transaction { UserId = _user.Id, Type = "transfer_out", Amount = -v, Description = desc, Status = "success", RecipientPhone = phone, CreatedAt = DateTime.Now });
            Store.AddTransaction(new Transaction { UserId = rec.Id, Type = "transfer_in", Amount = v, Description = "Перевод от " + sen.FullName, Status = "success", RecipientPhone = _user.Phone, CreatedAt = DateTime.Now });
            _shell.Snack("Переведено " + Fmt(v) + " → " + rec.FullName);
            ShowTransfer();
        };
        fs.Children.Add(btn);
        _content.Children.Add(CardBox(fs));
    }

    // ============================ HISTORY ============================

    private void ShowHistory()
    {
        ResetAndSet("history", "История");
        var txs = Store.UserTxs(_user.Id).ToList();
        if (txs.Count == 0)
        {
            _content.Children.Add(Tb("История пуста", 14, Rgb(105, 135, 185), margin: new Thickness(0, 16, 0, 0)));
            return;
        }
        foreach (var g in txs.GroupBy(t => t.CreatedAt.ToString("MMMM yyyy", new CultureInfo("ru-RU"))))
        {
            _content.Children.Add(Tb(g.Key.ToUpper(), 11, Rgb(65, 105, 170), FontWeight.Bold, margin: new Thickness(0, 14, 0, 6)));
            foreach (var tx in g) _content.Children.Add(MakeTx(tx));
        }
    }

    // ============================ MAT CAPITAL ============================

    private void ShowMatCapital()
    {
        ResetAndSet("matcap", "Мат. капитал");
        var ic = new Border
        {
            Background = Gradient(Color.FromRgb(72, 0, 170), Color.FromRgb(0, 78, 190), 135),
            CornerRadius = new CornerRadius(16),
            Padding = new Thickness(18),
            Margin = new Thickness(0, 0, 0, 14),
        };
        var iss = new StackPanel();
        iss.Children.Add(Tb("Остаток сертификата", 12, Argb(185, 255, 255, 255)));
        iss.Children.Add(Tb(Fmt(_user.MatCapitalBalance), 26, Brushes.White, FontWeight.Bold, margin: new Thickness(0, 4, 0, 4)));
        iss.Children.Add(Tb(_user.HasMatCapital ? "✓ Сертификат активен" : "✗ Не оформлен", 12,
            _user.HasMatCapital ? Rgb(170, 255, 200) : Rgb(255, 170, 170)));
        ic.Child = iss;
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
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(14, 12),
                Margin = new Thickness(0, 0, 0, 6),
            };
            var rg = new Grid();
            rg.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40) });
            rg.ColumnDefinitions.Add(new ColumnDefinition());
            rg.Children.Add(new TextBlock { Text = em, FontSize = 22, VerticalAlignment = VerticalAlignment.Center });
            var rt = new StackPanel { VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0, 0, 0) };
            rt.Children.Add(Tb(t, 13, Brushes.White, FontWeight.Medium));
            rt.Children.Add(Tb(d, 11, Rgb(105, 135, 190)));
            Grid.SetColumn(rt, 1);
            rg.Children.Add(rt);
            row.Child = rg;
            _content.Children.Add(row);
        }

        if (!_user.HasMatCapital) return;

        _content.Children.Add(Sec("Подать заявку"));
        var fs = new StackPanel();
        fs.Children.Add(FieldLabel("Направление"));
        var combo = new ComboBox
        {
            Height = 46,
            FontSize = 14,
            Background = Rgb(20, 40, 85),
            Foreground = Brushes.White,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            CornerRadius = new CornerRadius(10),
            ItemsSource = new[] { "Жильё", "Образование", "Пенсия матери", "Адаптация" },
            SelectedIndex = 0,
        };
        fs.Children.Add(combo);
        fs.Children.Add(FieldLabel("Сумма", 12));
        var ai = MakeFInput("Сумма");
        fs.Children.Add(ai);
        var btn = MakePrimary("Подать заявку", MaterialIconKind.Send);
        btn.Margin = new Thickness(0, 12, 0, 0);
        btn.Click += (_, _) =>
        {
            if (!TryMoney(ai.Text ?? "", out var v) || v <= 0) { _shell.Snack("Введите сумму"); return; }
            if (v > _user.MatCapitalBalance) { _shell.Snack("Превышает остаток"); return; }
            var u = Store.FindUserById(_user.Id)!;
            u.MatCapitalBalance -= v;
            Store.AddTransaction(new Transaction
            {
                UserId = _user.Id,
                Type = "matcap",
                Amount = -v,
                Description = "Мат. капитал: " + combo.SelectedItem,
                Status = "pending",
                CreatedAt = DateTime.Now,
            });
            _shell.Snack("Заявка подана!");
            ShowMatCapital();
        };
        fs.Children.Add(btn);
        _content.Children.Add(CardBox(fs));
    }

    // ============================ CREDIT ============================

    private void ShowCredit()
    {
        ResetAndSet("credit", "Кредиты");

        if (_user.CreditBalance > 0)
        {
            var ac = new Border
            {
                Background = Gradient(Color.FromRgb(165, 28, 28), Color.FromRgb(205, 86, 0), 135),
                CornerRadius = new CornerRadius(16),
                Padding = new Thickness(18),
                Margin = new Thickness(0, 0, 0, 14),
            };
            var acs = new StackPanel();
            acs.Children.Add(Tb("Активный кредит", 12, Argb(185, 255, 255, 255)));
            acs.Children.Add(Tb(Fmt(_user.CreditBalance), 26, Brushes.White, FontWeight.Bold, margin: new Thickness(0, 4, 0, 12)));
            var dg = new UniformGrid { Columns = 3, Margin = new Thickness(0, 0, 0, 8) };
            dg.Children.Add(MakeMini("Платёж", Fmt(_user.MonthlyPayment)));
            dg.Children.Add(MakeMini("Следующий", _user.NextPaymentDate?.ToString("dd.MM.yy") ?? "-"));
            dg.Children.Add(MakeMini("Ставка", "14.5%"));
            acs.Children.Add(dg);
            var pb = MakePrimary("Внести платёж", MaterialIconKind.CashFast);
            pb.Margin = new Thickness(0, 14, 0, 0);
            pb.Click += (_, _) =>
            {
                if (_user.Balance < _user.MonthlyPayment) { _shell.Snack("Недостаточно средств"); return; }
                var u = Store.FindUserById(_user.Id)!;
                var pay = Math.Min(u.MonthlyPayment, u.CreditBalance);
                u.Balance -= pay;
                u.CreditBalance -= pay;
                if (u.CreditBalance <= 0) { u.CreditBalance = 0; u.NextPaymentDate = null; }
                else u.NextPaymentDate = DateTime.Now.AddMonths(1);
                Store.AddTransaction(new Transaction { UserId = _user.Id, Type = "credit", Amount = -pay, Description = "Платёж по кредиту", Status = "success", CreatedAt = DateTime.Now });
                _shell.Snack("Платёж " + Fmt(pay) + " внесён");
                ShowCredit();
            };
            acs.Children.Add(pb);
            ac.Child = acs;
            _content.Children.Add(ac);
            return;
        }

        // No active credit -> apply form
        var fs = new StackPanel();
        fs.Children.Add(FieldLabel("Сумма кредита"));
        var ai = MakeFInput("До 1 000 000 ₽");
        fs.Children.Add(ai);
        fs.Children.Add(FieldLabel("Срок", 12));
        var rates = new[] { 0.145m, 0.160m, 0.185m, 0.210m };
        var mons = new[] { 12, 24, 36, 60 };
        var combo = new ComboBox
        {
            Height = 46,
            FontSize = 14,
            Background = Rgb(20, 40, 85),
            Foreground = Brushes.White,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            CornerRadius = new CornerRadius(10),
            ItemsSource = new[] { "12 мес. (14.5%)", "24 мес. (16.0%)", "36 мес. (18.5%)", "60 мес. (21.0%)" },
            SelectedIndex = 0,
        };
        fs.Children.Add(combo);
        var cr = new Border
        {
            Background = Rgb(9, 20, 52),
            CornerRadius = new CornerRadius(12),
            Padding = new Thickness(14),
            Margin = new Thickness(0, 12, 0, 0),
            IsVisible = false,
        };
        var cs = new StackPanel();
        cr.Child = cs;
        fs.Children.Add(cr);
        var calcBtn = MakePrimary("Рассчитать", MaterialIconKind.Calculator);
        calcBtn.Margin = new Thickness(0, 12, 0, 0);
        calcBtn.Click += (_, _) =>
        {
            if (!TryMoney(ai.Text ?? "", out var v) || v <= 0) { _shell.Snack("Введите сумму"); return; }
            var r = rates[combo.SelectedIndex] / 12;
            var n = mons[combo.SelectedIndex];
            var pay = v * r * (decimal)Math.Pow((double)(1 + r), n) / ((decimal)Math.Pow((double)(1 + r), n) - 1);
            cs.Children.Clear();
            cs.Children.Add(Tb("Ежемесячный платёж: " + Fmt(Math.Round(pay, 2)), 14, Brushes.White, margin: new Thickness(0, 0, 0, 4), wrap: true));
            cs.Children.Add(Tb("Переплата: " + Fmt(Math.Round(pay * n - v, 2)), 13, Rgb(255, 155, 50), wrap: true));
            cs.Children.Add(Tb("Итого: " + Fmt(Math.Round(pay * n, 2)), 13, Rgb(135, 190, 255), wrap: true));
            cr.IsVisible = true;
        };
        fs.Children.Add(calcBtn);
        var applyBtn = MakePrimary("Оформить кредит", MaterialIconKind.FileDocumentCheck);
        applyBtn.Margin = new Thickness(0, 8, 0, 0);
        applyBtn.Click += (_, _) =>
        {
            if (!TryMoney(ai.Text ?? "", out var v) || v <= 0 || v > 1000000) { _shell.Snack("Сумма от 1 до 1 000 000 ₽"); return; }
            var r = rates[combo.SelectedIndex] / 12;
            var n = mons[combo.SelectedIndex];
            var pay = v * r * (decimal)Math.Pow((double)(1 + r), n) / ((decimal)Math.Pow((double)(1 + r), n) - 1);
            var u = Store.FindUserById(_user.Id)!;
            u.Balance += v;
            u.CreditBalance = v;
            u.CreditLimit = v;
            u.MonthlyPayment = Math.Round(pay, 2);
            u.NextPaymentDate = DateTime.Now.AddMonths(1);
            Store.AddTransaction(new Transaction { UserId = _user.Id, Type = "credit", Amount = v, Description = "Выдача кредита " + n + " мес.", Status = "success", CreatedAt = DateTime.Now });
            _shell.Snack("Кредит " + Fmt(v) + " зачислен!");
            ShowCredit();
        };
        fs.Children.Add(applyBtn);
        _content.Children.Add(CardBox(fs));
    }

    private static StackPanel MakeMini(string lbl, string val)
    {
        var s = new StackPanel { Margin = new Thickness(0, 0, 8, 0) };
        s.Children.Add(Tb(lbl, 10, Argb(155, 255, 255, 255)));
        s.Children.Add(Tb(val, 12, Brushes.White, FontWeight.Bold, wrap: true));
        return s;
    }

    // ============================ FINES ============================

    private void ShowFines()
    {
        ResetAndSet("fines", "Штрафы");
        var fines = Store.UserFines(_user.Id).ToList();
        var unpaid = fines.Where(f => !f.IsPaid).Sum(f => f.Amount);
        var sc = new Border
        {
            Background = unpaid > 0
                ? Gradient(Color.FromRgb(165, 42, 0), Color.FromRgb(205, 105, 0), 135)
                : Gradient(Color.FromRgb(0, 105, 50), Color.FromRgb(0, 160, 85), 135),
            CornerRadius = new CornerRadius(14),
            Padding = new Thickness(16),
            Margin = new Thickness(0, 0, 0, 12),
        };
        var ss = new StackPanel { Orientation = Orientation.Horizontal };
        ss.Children.Add(new MaterialIcon
        {
            Kind = unpaid > 0 ? MaterialIconKind.AlertCircle : MaterialIconKind.CheckCircle,
            Foreground = Brushes.White,
            Width = 32,
            Height = 32,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, 0, 12, 0),
        });
        var st = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
        st.Children.Add(Tb(unpaid > 0 ? "Неоплачено: " + Fmt(unpaid) : "Все штрафы оплачены", 14, Brushes.White, FontWeight.Bold, wrap: true));
        st.Children.Add(Tb("Записей: " + fines.Count, 11, Argb(185, 255, 255, 255)));
        ss.Children.Add(st);
        sc.Child = ss;
        _content.Children.Add(sc);

        foreach (var fine in fines)
        {
            var fb = new Border
            {
                Background = Rgb(15, 30, 67),
                CornerRadius = new CornerRadius(13),
                Padding = new Thickness(14, 12),
                Margin = new Thickness(0, 0, 0, 6),
                BorderBrush = fine.IsPaid ? Rgb(0, 95, 47) : Rgb(188, 70, 0),
                BorderThickness = new Thickness(0, 0, 0, 2),
            };
            var fi = new StackPanel();
            fi.Children.Add(Tb(fine.Description, 13, Brushes.White, FontWeight.Medium, wrap: true));
            fi.Children.Add(Tb("№ " + fine.FineNumber, 10, Rgb(105, 135, 190), margin: new Thickness(0, 2, 0, 0)));
            fi.Children.Add(Tb("Выдан: " + fine.IssuedAt.ToString("dd.MM.yyyy") + (fine.IsPaid ? " | Оплачен: " + fine.PaidAt?.ToString("dd.MM.yyyy") : ""),
                11, Rgb(105, 135, 190)));
            var foot = new Grid { Margin = new Thickness(0, 8, 0, 0) };
            foot.ColumnDefinitions.Add(new ColumnDefinition());
            foot.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            var amtTxt = Tb(Fmt(fine.Amount), 16, fine.IsPaid ? Rgb(0, 195, 95) : Rgb(255, 95, 48), FontWeight.Bold);
            amtTxt.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(amtTxt, 0);
            foot.Children.Add(amtTxt);
            if (!fine.IsPaid)
            {
                var payBtn = new Button
                {
                    Content = "Оплатить",
                    Background = Gradient(Color.FromRgb(0, 100, 220), Color.FromRgb(0, 170, 150), 0),
                    Foreground = Brushes.White,
                    BorderThickness = new Thickness(0),
                    CornerRadius = new CornerRadius(10),
                    FontSize = 12,
                    FontWeight = FontWeight.SemiBold,
                    Padding = new Thickness(16, 8),
                    Height = 36,
                };
                var fid = fine.Id;
                var famt = fine.Amount;
                payBtn.Click += (_, _) =>
                {
                    if (_user.Balance < famt) { _shell.Snack("Недостаточно средств"); return; }
                    var f = Store.Fines.FirstOrDefault(x => x.Id == fid);
                    if (f == null) return;
                    f.IsPaid = true;
                    f.PaidAt = DateTime.Now;
                    var u = Store.FindUserById(_user.Id)!;
                    u.Balance -= famt;
                    Store.AddTransaction(new Transaction { UserId = _user.Id, Type = "fine", Amount = -famt, Description = "Штраф: " + f.Description, Status = "success", CreatedAt = DateTime.Now });
                    _shell.Snack("Штраф " + Fmt(famt) + " оплачен");
                    ShowFines();
                };
                Grid.SetColumn(payBtn, 1);
                foot.Children.Add(payBtn);
            }
            else
            {
                var paid = Tb("✓ Оплачен", 12, Rgb(0, 195, 95));
                paid.VerticalAlignment = VerticalAlignment.Center;
                Grid.SetColumn(paid, 1);
                foot.Children.Add(paid);
            }
            fi.Children.Add(foot);
            fb.Child = fi;
            _content.Children.Add(fb);
        }

        _content.Children.Add(Sec("Добавить штраф"));
        var asx = new StackPanel();
        asx.Children.Add(FieldLabel("Номер постановления"));
        var ni = MakeFInput("18 цифр");
        asx.Children.Add(ni);
        asx.Children.Add(FieldLabel("Описание", 12));
        var di = MakeFInput("Описание нарушения");
        asx.Children.Add(di);
        asx.Children.Add(FieldLabel("Сумма", 12));
        var fi2 = MakeFInput("Сумма штрафа");
        asx.Children.Add(fi2);
        var addBtn = MakePrimary("Добавить", MaterialIconKind.Plus);
        addBtn.Margin = new Thickness(0, 12, 0, 0);
        addBtn.Click += (_, _) =>
        {
            if (!TryMoney(fi2.Text ?? "", out var v) || v <= 0 || string.IsNullOrEmpty((ni.Text ?? "").Trim())) { _shell.Snack("Заполните все поля"); return; }
            Store.AddFine(new Fine
            {
                UserId = _user.Id,
                FineNumber = (ni.Text ?? "").Trim(),
                Description = (di.Text ?? "").Trim(),
                Amount = v,
                IsPaid = false,
                IssuedAt = DateTime.Now,
            });
            _shell.Snack("Штраф добавлен");
            ShowFines();
        };
        asx.Children.Add(addBtn);
        _content.Children.Add(CardBox(asx));
    }

    // ============================ TAXES ============================

    private void ShowTaxes()
    {
        ResetAndSet("taxes", "Налоги");
        var tc = new Border
        {
            Background = _user.TaxDebt > 0
                ? Gradient(Color.FromRgb(150, 72, 0), Color.FromRgb(195, 130, 0), 135)
                : Gradient(Color.FromRgb(0, 95, 47), Color.FromRgb(0, 150, 76), 135),
            CornerRadius = new CornerRadius(15),
            Padding = new Thickness(18),
            Margin = new Thickness(0, 0, 0, 14),
        };
        var ts = new StackPanel();
        ts.Children.Add(Tb("Задолженность по налогам", 12, Argb(185, 255, 255, 255)));
        ts.Children.Add(Tb(Fmt(_user.TaxDebt), 26, Brushes.White, FontWeight.Bold, margin: new Thickness(0, 4, 0, 4)));
        ts.Children.Add(Tb(_user.TaxDebt > 0 ? "⚠ Требуется оплата" : "✓ Задолженностей нет", 12, Brushes.White, wrap: true));
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
                CornerRadius = new CornerRadius(11),
                Padding = new Thickness(14, 11),
                Margin = new Thickness(0, 0, 0, 6),
            };
            var rg = new Grid();
            rg.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(44) });
            rg.ColumnDefinitions.Add(new ColumnDefinition());
            rg.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            var ib = new Border
            {
                Width = 36,
                Height = 36,
                CornerRadius = new CornerRadius(9),
                Background = Rgb(22, 48, 120),
                VerticalAlignment = VerticalAlignment.Center,
                Child = Icon(icon, Rgb(0, 148, 255), 18),
            };
            var tt = new StackPanel { VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(10, 0, 0, 0) };
            tt.Children.Add(Tb(t, 13, Brushes.White, FontWeight.Medium));
            Grid.SetColumn(tt, 1);
            var rt = Tb(r, 12, Rgb(135, 165, 220));
            rt.VerticalAlignment = VerticalAlignment.Center;
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
            var ps = new StackPanel();
            ps.Children.Add(Tb("К оплате: " + Fmt(_user.TaxDebt), 14, Brushes.White, margin: new Thickness(0, 0, 0, 12), wrap: true));
            var pay = MakePrimary("Оплатить задолженность", MaterialIconKind.CashFast);
            pay.Click += (_, _) =>
            {
                if (_user.Balance < _user.TaxDebt) { _shell.Snack("Недостаточно средств"); return; }
                var u = Store.FindUserById(_user.Id)!;
                var debt = u.TaxDebt;
                u.Balance -= debt;
                u.TaxDebt = 0;
                Store.AddTransaction(new Transaction
                {
                    UserId = _user.Id, Type = "tax", Amount = -debt, Description = "Оплата налоговой задолженности",
                    Status = "success", CreatedAt = DateTime.Now,
                });
                _shell.Snack("Налоговая задолженность погашена!");
                ShowTaxes();
            };
            ps.Children.Add(pay);
            _content.Children.Add(CardBox(ps));
        }
    }

    // ============================ PROFILE ============================

    private void ShowProfile()
    {
        ResetAndSet("profile", "Профиль");

        var initials = GetInitials(_user.FullName);
        var av = new Border
        {
            Width = 80,
            Height = 80,
            CornerRadius = new CornerRadius(40),
            Background = Gradient(Color.FromRgb(0, 120, 255), Color.FromRgb(0, 200, 150), 45),
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 4, 0, 14),
            Child = new TextBlock
            {
                Text = initials,
                FontSize = 28,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontWeight = FontWeight.Bold,
            },
        };
        _content.Children.Add(av);
        _content.Children.Add(Tb(_user.FullName, 17, Brushes.White, FontWeight.Bold, halign: HorizontalAlignment.Center, wrap: true));
        _content.Children.Add(Tb("Клиент с " + _user.CreatedAt.ToString("MMMM yyyy", new CultureInfo("ru-RU")),
            12, Rgb(130, 165, 215), halign: HorizontalAlignment.Center, margin: new Thickness(0, 4, 0, 16)));

        var ps = new StackPanel();
        AddDetailRow(ps, "Телефон", "+7 " + (_user.Phone.Length > 1 ? _user.Phone[1..] : _user.Phone));
        AddDetailRow(ps, "Email", _user.Email);
        AddDetailRow(ps, "Паспорт", _user.PassportNumber);
        AddDetailRow(ps, "ИНН", _user.INN);
        AddDetailRow(ps, "Адрес", _user.Address);
        _content.Children.Add(CardBox(ps));

        _content.Children.Add(Sec("Сменить PIN"));
        var pf = new StackPanel();
        TextBox MakePin()
        {
            return new TextBox
            {
                MaxLength = 4,
                Background = Rgb(20, 40, 85),
                Foreground = Brushes.White,
                BorderBrush = Rgb(38, 76, 175),
                Height = 46,
                FontSize = 14,
                Margin = new Thickness(0, 0, 0, 10),
                PasswordChar = '●',
                Padding = new Thickness(12, 0),
                CornerRadius = new CornerRadius(10),
            };
        }
        pf.Children.Add(FieldLabel("Текущий PIN"));
        var op = MakePin();
        pf.Children.Add(op);
        pf.Children.Add(FieldLabel("Новый PIN"));
        var np = MakePin();
        pf.Children.Add(np);
        pf.Children.Add(FieldLabel("Подтвердите"));
        var cp2 = MakePin();
        pf.Children.Add(cp2);
        var chBtn = MakePrimary("Изменить PIN", MaterialIconKind.Lock);
        chBtn.Margin = new Thickness(0, 6, 0, 0);
        chBtn.Click += (_, _) =>
        {
            if ((op.Text ?? "") != _user.PinHash) { _shell.Snack("Неверный текущий PIN"); return; }
            if ((np.Text ?? "") != (cp2.Text ?? "")) { _shell.Snack("Новые PIN не совпадают"); return; }
            var newPin = np.Text ?? "";
            if (newPin.Length != 4 || !newPin.All(char.IsDigit)) { _shell.Snack("PIN — 4 цифры"); return; }
            var u = Store.FindUserById(_user.Id)!;
            u.PinHash = newPin;
            _shell.Snack("PIN изменён");
        };
        pf.Children.Add(chBtn);
        _content.Children.Add(CardBox(pf));

        _content.Children.Add(Sec("Безопасность"));
        var logoutPanel = new StackPanel();
        var logoutBtn = new Button
        {
            Content = "Выйти из аккаунта",
            Background = Argb(40, 255, 80, 80),
            Foreground = Rgb(255, 110, 110),
            BorderThickness = new Thickness(1),
            BorderBrush = Argb(120, 255, 80, 80),
            CornerRadius = new CornerRadius(12),
            Height = 50,
            FontSize = 14,
            FontWeight = FontWeight.SemiBold,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Center,
        };
        logoutBtn.Click += (_, _) =>
        {
            App.CurrentUser = null;
            _shell.ShowLogin();
        };
        logoutPanel.Children.Add(logoutBtn);
        _content.Children.Add(CardBox(logoutPanel));
    }
}
