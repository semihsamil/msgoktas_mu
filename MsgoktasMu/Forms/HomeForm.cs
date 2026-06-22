using MsgoktasMu.Core;
using MsgoktasMu.Data;

namespace MsgoktasMu.Forms;

internal sealed class HomeForm : Form
{
    private readonly Panel _content = new()
    {
        Dock = DockStyle.Fill,
        Padding = new Padding(28, 20, 28, 24),
        AutoScroll = true,
        BackColor = AppTheme.Background,
    };
    private readonly Label _sessionLabel = new()
    {
        Dock = DockStyle.Top,
        Height = 30,
        TextAlign = ContentAlignment.MiddleRight,
        ForeColor = AppTheme.TextMuted,
        Font = AppTheme.BodyFont,
        BackColor = AppTheme.SurfaceMuted,
        Padding = new Padding(0, 0, 16, 0),
    };
    private readonly ToolStripStatusLabel _statusLabel = new() { Text = "Hazır" };

    public HomeForm()
    {
        AppTheme.ApplyToForm(this);
        Text = "Mimar Sinan Göktaş — Şantiye Takip";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(1020, 680);
        ClientSize = new Size(1140, 760);

        MainMenuStrip = BuildMainMenu();
        var statusBar = new StatusStrip { BackColor = AppTheme.SurfaceMuted };
        statusBar.Items.Add(_statusLabel);

        var header = new Panel
        {
            Dock = DockStyle.Top,
            Height = 68,
            BackColor = AppTheme.Surface,
            Padding = new Padding(18, 14, 18, 10),
        };
        header.Paint += (_, e) =>
        {
            using var pen = new Pen(AppTheme.Border);
            e.Graphics.DrawLine(pen, 0, header.Height - 1, header.Width, header.Height - 1);
        };

        var brand = new Label
        {
            Text = "Mimar Sinan Göktaş",
            Dock = DockStyle.Left,
            Width = 280,
            Font = AppTheme.TitleFont,
            ForeColor = AppTheme.Accent,
            TextAlign = ContentAlignment.MiddleLeft,
        };

        var nav = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Padding = new Padding(8, 0, 0, 0),
        };
        nav.Controls.Add(MakeNavButton("Ana Sayfa", ShowHome));
        nav.Controls.Add(MakeNavButton("Genel Bilgiler", () => ShowFiles("general")));
        nav.Controls.Add(MakeNavButton("Raporlar", () => ShowFiles("reports")));
        nav.Controls.Add(MakeNavButton("Notlar", () => ShowFiles("notes")));
        nav.Controls.Add(MakeNavButton("Çizelge", () => ShowFiles("schedule")));
        nav.Controls.Add(MakeNavButton("Şantiyeler", ShowSites));
        nav.Controls.Add(MakeNavButton("Harita", ShowMap));
        nav.Controls.Add(MakeNavButton("İletişim", ShowContact));

        var right = new FlowLayoutPanel { Dock = DockStyle.Right, Width = 250, FlowDirection = FlowDirection.RightToLeft };
        var btnLogin = UiFactory.CreateButton("Giriş / Admin", primary: true, width: 118);
        btnLogin.Click += (_, _) => OpenLogin();
        var btnLogout = UiFactory.CreateButton("Çıkış", primary: false, width: 80);
        btnLogout.Click += (_, _) => Logout();
        right.Controls.Add(btnLogout);
        right.Controls.Add(btnLogin);

        header.Controls.Add(right);
        header.Controls.Add(nav);
        header.Controls.Add(brand);

        Controls.Add(_content);
        Controls.Add(_sessionLabel);
        Controls.Add(header);
        Controls.Add(statusBar);
        Controls.Add(MainMenuStrip);

        UpdateSessionUi();
        ShowHome();
    }

    private MenuStrip BuildMainMenu()
    {
        var menu = new MenuStrip { BackColor = AppTheme.Surface, Font = AppTheme.BodyFont };

        var file = new ToolStripMenuItem("Dosya");
        file.DropDownItems.Add("Çıkış", null, (_, _) => Close());

        var view = new ToolStripMenuItem("Görüntüle");
        view.DropDownItems.Add("Ana Sayfa", null, (_, _) => ShowHome());
        view.DropDownItems.Add("Genel Şantiye Bilgileri", null, (_, _) => ShowFiles("general"));
        view.DropDownItems.Add("Günlük Raporlar", null, (_, _) => ShowFiles("reports"));
        view.DropDownItems.Add("Bilgi Notları", null, (_, _) => ShowFiles("notes"));
        view.DropDownItems.Add("Şantiye Çizelgesi", null, (_, _) => ShowFiles("schedule"));
        view.DropDownItems.Add(new ToolStripSeparator());
        view.DropDownItems.Add("Şantiyeler", null, (_, _) => ShowSites());
        view.DropDownItems.Add("Konum Haritası", null, (_, _) => ShowMap());
        view.DropDownItems.Add("İletişim", null, (_, _) => ShowContact());

        var account = new ToolStripMenuItem("Hesap");
        account.DropDownItems.Add("Giriş Yap", null, (_, _) => OpenLogin());
        account.DropDownItems.Add("Çıkış Yap", null, (_, _) => Logout());

        var help = new ToolStripMenuItem("Yardım");
        help.DropDownItems.Add("Kullanım Kılavuzu", null, (_, _) => ShowHelp());
        help.DropDownItems.Add("Hakkında", null, (_, _) => ShowAbout());

        menu.Items.Add(file);
        menu.Items.Add(view);
        menu.Items.Add(account);
        menu.Items.Add(help);
        return menu;
    }

    private void SetStatus(string text) => _statusLabel.Text = text;

    private static void ShowHelp()
    {
        using var help = new HelpForm();
        help.ShowDialog();
    }

    private static void ShowAbout()
    {
        using var about = new AboutForm();
        about.ShowDialog();
    }

    private Button MakeNavButton(string text, Action action)
    {
        var btn = UiFactory.CreateButton(text, primary: false, width: 0, height: 34);
        btn.AutoSize = true;
        btn.MinimumSize = new Size(88, 34);
        AppTheme.StyleNavButton(btn);
        btn.Click += (_, _) => action();
        return btn;
    }

    private void UpdateSessionUi()
    {
        if (!AuthSession.IsLoggedIn)
        {
            _sessionLabel.Text = "  Ziyaretçi modu — dosyaları görüntüleyebilirsiniz";
            return;
        }

        var u = AuthSession.Current!;
        _sessionLabel.Text = $"  Oturum: {u.Username} ({AppConstants.RoleLabel(u.Role)})";

        if (u.Role == "admin")
        {
            using var admin = new AdminForm();
            admin.ShowDialog(this);
        }
        else
        {
            using var work = new WorkTrackingForm();
            work.ShowDialog(this);
        }

        AuthSession.Clear();
        _sessionLabel.Text = "  Ziyaretçi modu — dosyaları görüntüleyebilirsiniz";
    }

    private void OpenLogin()
    {
        using var login = new LoginForm();
        if (login.ShowDialog(this) == DialogResult.OK)
            UpdateSessionUi();
    }

    private void Logout()
    {
        AuthSession.Clear();
        ShowHome();
    }

    private void ClearContent() => _content.Controls.Clear();

    private void ShowHome()
    {
        ClearContent();
        SetStatus("Ana sayfa");

        var hero = new Panel
        {
            Dock = DockStyle.Top,
            Height = 150,
            BackColor = AppTheme.Accent,
            Padding = new Padding(24, 22, 24, 22),
        };
        hero.Controls.Add(new Label
        {
            Text = "Mimarlık ofisi — şantiye raporları, çizelgeler ve dosyalar bu programdan takip edilir.",
            Dock = DockStyle.Bottom,
            Height = 42,
            ForeColor = Color.FromArgb(255, 245, 225),
            Font = AppTheme.BodyFont,
        });
        hero.Controls.Add(new Label
        {
            Text = "Mimar Sinan Göktaş",
            Dock = DockStyle.Top,
            Height = 42,
            ForeColor = Color.White,
            Font = new Font("Georgia", 20F, FontStyle.Bold),
        });

        var cards = new FlowLayoutPanel
        {
            AutoSize = true,
            WrapContents = true,
            Padding = new Padding(4, 8, 4, 8),
            BackColor = AppTheme.Background,
        };
        cards.Controls.Add(MakeCard("Genel Bilgiler", "Şantiye listesi ve dosyalar", () => ShowFiles("general")));
        cards.Controls.Add(MakeCard("Günlük Raporlar", "Yüklenen rapor dosyaları", () => ShowFiles("reports")));
        cards.Controls.Add(MakeCard("Çizelge", "İş programı dosyaları", () => ShowFiles("schedule")));
        cards.Controls.Add(MakeCard("Harita", "Ofis konumu", ShowMap));

        var cardsSection = new Panel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(0, 20, 0, 24),
            BackColor = AppTheme.Background,
        };
        cards.Dock = DockStyle.Top;
        cardsSection.Controls.Add(cards);

        var about = AppTheme.CreateGroupBox("Hakkımızda");
        about.Dock = DockStyle.Top;
        about.Height = 110;
        about.Margin = new Padding(0, 0, 0, 0);
        about.Controls.Add(new Label
        {
            Dock = DockStyle.Fill,
            Text = "Sivas merkezli mimarlık ofisiyiz. Konut ve ticari yapı projeleri ile şantiye takip işlerinde çalışıyoruz.",
            ForeColor = AppTheme.Text,
            Font = AppTheme.BodyFont,
        });

        var services = AppTheme.CreateGroupBox("Hizmetler");
        services.Dock = DockStyle.Top;
        services.Height = 130;
        services.Padding = new Padding(14, 18, 14, 14);
        services.Controls.Add(new Label
        {
            Dock = DockStyle.Fill,
            Text = "Mimari Proje  •  İç Mekan  •  Şantiye Takibi  •  Danışmanlık",
            ForeColor = AppTheme.TextMuted,
            Font = AppTheme.BodyFont,
        });

        _content.Controls.Add(services);
        _content.Controls.Add(CreateSectionGap(18));
        _content.Controls.Add(about);
        _content.Controls.Add(CreateSectionGap(18));
        _content.Controls.Add(cardsSection);
        _content.Controls.Add(CreateSectionGap(22));
        _content.Controls.Add(hero);
    }

    private static Panel CreateSectionGap(int height) =>
        new Panel { Dock = DockStyle.Top, Height = height, BackColor = AppTheme.Background };

    private Panel MakeCard(string title, string desc, Action action)
    {
        var card = AppTheme.CreateCard(240, 118);
        var titleLbl = new Label
        {
            Text = title,
            Dock = DockStyle.Top,
            Height = 28,
            Font = AppTheme.SectionFont,
            ForeColor = AppTheme.Accent,
        };
        var descLbl = new Label
        {
            Text = desc,
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.TextMuted,
            Font = AppTheme.BodyFont,
            Padding = new Padding(0, 6, 0, 0),
        };
        card.Controls.Add(descLbl);
        card.Controls.Add(titleLbl);
        card.Click += (_, _) => action();
        foreach (Control c in card.Controls)
            c.Click += (_, _) => action();
        return card;
    }

    private Panel CreatePageHeader(string title, string description)
    {
        var panel = new Panel { Dock = DockStyle.Top, Height = 72, Margin = new Padding(0, 0, 0, 12) };
        panel.Controls.Add(new Label
        {
            Text = description,
            Dock = DockStyle.Bottom,
            Height = 24,
            ForeColor = AppTheme.TextMuted,
            Font = AppTheme.BodyFont,
        });
        panel.Controls.Add(new Label
        {
            Text = title,
            Dock = DockStyle.Top,
            Height = 34,
            Font = AppTheme.SectionFont,
            ForeColor = AppTheme.Accent,
        });
        return panel;
    }

    private void ShowFiles(string category)
    {
        ClearContent();
        SetStatus(AppConstants.CategoryTitle(category));

        var header = CreatePageHeader(
            AppConstants.CategoryTitle(category),
            category == "general"
                ? "Önce şantiye listesine bakın, ardından genel dosyaları inceleyin."
                : "Listeden dosyayı çift tıklayarak açabilirsiniz.");

        var filesHost = new Panel { Dock = DockStyle.Fill, BackColor = AppTheme.Surface, Padding = new Padding(8), BorderStyle = BorderStyle.FixedSingle };
        filesHost.Controls.Add(new FileManagerControl(category, allowManage: false) { Dock = DockStyle.Fill });

        if (category == "general")
        {
            var sitesBox = AppTheme.CreateGroupBox("Şantiyeler");
            sitesBox.Dock = DockStyle.Top;
            sitesBox.Height = 240;
            sitesBox.Controls.Add(BuildSitesPanel());
            _content.Controls.Add(filesHost);
            _content.Controls.Add(sitesBox);
        }
        else
        {
            _content.Controls.Add(filesHost);
        }

        _content.Controls.Add(header);
    }

    private Control BuildSitesPanel()
    {
        var list = new ListView
        {
            Dock = DockStyle.Fill,
            View = View.Details,
            FullRowSelect = true,
            BorderStyle = BorderStyle.None,
            Font = AppTheme.BodyFont,
        };
        list.Columns.Add("Şantiye Adı", 220);
        list.Columns.Add("Adres", 380);
        list.Columns.Add("İşlem", 90);

        foreach (var site in LocalDatabase.GetSites())
        {
            var item = new ListViewItem(site.Name) { Tag = site };
            item.SubItems.Add(site.Address);
            item.SubItems.Add("Görüntüle");
            list.Items.Add(item);
        }

        list.DoubleClick += (_, _) => OpenSite(list);
        return list;
    }

    private static void OpenSite(ListView list)
    {
        if (list.SelectedItems.Count == 0) return;
        var site = (Models.ConstructionSite)list.SelectedItems[0].Tag!;
        using var detail = new SiteDetailForm(site);
        detail.ShowDialog();
    }

    private void ShowSites()
    {
        ClearContent();
        SetStatus("Şantiyeler");
        var header = CreatePageHeader("Şantiyeler", "Detay görmek için satıra çift tıklayın.");
        var box = AppTheme.CreateGroupBox("Kayıtlı Şantiyeler");
        box.Dock = DockStyle.Fill;
        box.Controls.Add(BuildSitesPanel());
        _content.Controls.Add(box);
        _content.Controls.Add(header);
    }

    private void ShowMap()
    {
        ClearContent();
        SetStatus("Konum haritası");
        var settings = LocalDatabase.GetSettings();
        var header = CreatePageHeader("Konum Haritası", "Ofis konumu ve koordinat bilgileri");
        var box = AppTheme.CreateGroupBox("Konum Bilgisi");
        box.Dock = DockStyle.Top;
        box.Height = 180;
        box.Controls.Add(new Label
        {
            Dock = DockStyle.Fill,
            Text = $"Etiket: {settings.MapLabel}\r\nAdres: {settings.ContactAddress}\r\nEnlem: {settings.MapLat}   Boylam: {settings.MapLng}",
            Font = AppTheme.BodyFont,
            ForeColor = AppTheme.Text,
        });

        var btn = UiFactory.CreateButton("Google Haritada Aç", primary: true, width: 170);
        btn.Click += (_, _) =>
        {
            var url = $"https://www.google.com/maps?q={settings.MapLat},{settings.MapLng}";
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true });
        };
        var flow = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 48, Padding = new Padding(0, 8, 0, 0) };
        flow.Controls.Add(btn);

        _content.Controls.Add(flow);
        _content.Controls.Add(box);
        _content.Controls.Add(header);
    }

    private void ShowContact()
    {
        ClearContent();
        SetStatus("İletişim");
        var settings = LocalDatabase.GetSettings();
        var header = CreatePageHeader("İletişim", "Ofis iletişim bilgileri");
        var box = AppTheme.CreateGroupBox("Bize Ulaşın");
        box.Dock = DockStyle.Top;
        box.Height = 130;
        box.Controls.Add(new Label
        {
            Dock = DockStyle.Fill,
            Text = $"E-posta: {settings.ContactEmail}\r\nTelefon: {settings.ContactPhone}\r\nAdres: {settings.ContactAddress}",
            Font = AppTheme.BodyFont,
            ForeColor = AppTheme.Text,
        });
        _content.Controls.Add(box);
        _content.Controls.Add(header);
    }
}
