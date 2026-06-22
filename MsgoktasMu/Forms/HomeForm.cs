using MsgoktasMu.Core;
using MsgoktasMu.Data;

namespace MsgoktasMu.Forms;

internal sealed class HomeForm : Form
{
    private readonly Panel _content = new() { Dock = DockStyle.Fill, Padding = new Padding(24), AutoScroll = true };
    private readonly Label _sessionLabel = new()
    {
        Dock = DockStyle.Top,
        Height = 28,
        TextAlign = ContentAlignment.MiddleRight,
        ForeColor = Color.DimGray,
    };

    public HomeForm()
    {
        Text = "Mimar Sinan Göktaş";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(980, 640);
        ClientSize = new Size(1100, 720);

        var header = new Panel { Dock = DockStyle.Top, Height = 56, Padding = new Padding(16, 12, 16, 0) };
        var brand = new Label
        {
            Text = "Mimar Sinan Göktaş",
            Dock = DockStyle.Left,
            Width = 260,
            Font = new Font("Georgia", 16F, FontStyle.Bold),
            ForeColor = Color.FromArgb(140, 98, 36),
        };

        var nav = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false };
        nav.Controls.Add(MakeNavButton("Ana Sayfa", ShowHome));
        nav.Controls.Add(MakeNavButton("Genel Şantiye Bilgileri", () => ShowFiles("general")));
        nav.Controls.Add(MakeNavButton("Günlük Raporlar", () => ShowFiles("reports")));
        nav.Controls.Add(MakeNavButton("Bilgi Notları", () => ShowFiles("notes")));
        nav.Controls.Add(MakeNavButton("Şantiye Çizelgesi", () => ShowFiles("schedule")));
        nav.Controls.Add(MakeNavButton("Şantiyeler", ShowSites));
        nav.Controls.Add(MakeNavButton("Harita", ShowMap));
        nav.Controls.Add(MakeNavButton("İletişim", ShowContact));

        var right = new FlowLayoutPanel { Dock = DockStyle.Right, Width = 260, FlowDirection = FlowDirection.RightToLeft };
        var btnLogin = UiFactory.CreateButton("Giriş / Admin", 120);
        btnLogin.Click += (_, _) => OpenLogin();
        var btnLogout = UiFactory.CreateButton("Çıkış", 80);
        btnLogout.Click += (_, _) => Logout();
        right.Controls.Add(btnLogout);
        right.Controls.Add(btnLogin);

        header.Controls.Add(right);
        header.Controls.Add(nav);
        header.Controls.Add(brand);

        Controls.Add(_content);
        Controls.Add(_sessionLabel);
        Controls.Add(header);

        UpdateSessionUi();
        ShowHome();
    }

    private Button MakeNavButton(string text, Action action)
    {
        var btn = UiFactory.CreateButton(text, 150, 32);
        btn.Click += (_, _) => action();
        return btn;
    }

    private void UpdateSessionUi()
    {
        if (!AuthSession.IsLoggedIn)
        {
            _sessionLabel.Text = "Ziyaretçi modu";
            return;
        }

        var u = AuthSession.Current!;
        _sessionLabel.Text = $"Oturum: {u.Username} ({AppConstants.RoleLabel(u.Role)})";

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
        _sessionLabel.Text = "Ziyaretçi modu";
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
        UpdateSessionUi();
        ShowHome();
    }

    private void ClearContent()
    {
        _content.Controls.Clear();
    }

    private void ShowHome()
    {
        ClearContent();
        var title = new Label
        {
            Text = "Mimar Sinan Göktaş",
            Dock = DockStyle.Top,
            Height = 40,
            Font = new Font("Segoe UI", 18F, FontStyle.Bold),
        };
        var lead = new Label
        {
            Text = "Mimarlık ofisi. Şantiye raporları, çizelgeler ve dosyalar bu programdan takip edilir.",
            Dock = DockStyle.Top,
            Height = 48,
            MaximumSize = new Size(900, 0),
            AutoSize = true,
        };

        var cards = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 150, AutoSize = true, WrapContents = true };
        cards.Controls.Add(MakeCard("Genel Bilgiler", "Şantiye listesi ve dosyalar", () => ShowFiles("general")));
        cards.Controls.Add(MakeCard("Günlük Raporlar", "Yüklenen rapor dosyaları", () => ShowFiles("reports")));
        cards.Controls.Add(MakeCard("Çizelge", "İş programı dosyaları", () => ShowFiles("schedule")));
        cards.Controls.Add(MakeCard("Harita", "Ofis konumu", ShowMap));

        var about = new GroupBox { Text = "Hakkımızda", Dock = DockStyle.Top, Height = 120, Padding = new Padding(12) };
        about.Controls.Add(new Label
        {
            Dock = DockStyle.Fill,
            Text = "Sivas merkezli mimarlık ofisiyiz. Konut ve ticari yapı projeleri ile şantiye takip işlerinde çalışıyoruz. Bu program; rapor, çizelge ve şantiye bilgilerinin takip edildiği masaüstü uygulamadır.",
        });

        var services = new GroupBox { Text = "Hizmetler", Dock = DockStyle.Top, Height = 150, Padding = new Padding(12) };
        services.Controls.Add(new Label
        {
            Dock = DockStyle.Fill,
            Text = "• Mimari Proje — Konut ve işyeri projelerinin hazırlanması\r\n• İç Mekan — Oda düzeni ve malzeme planlaması\r\n• Şantiye Takibi — Günlük rapor, çizelge ve dosya takibi\r\n• Danışmanlık — Ruhsat ve uygulama süreçlerinde destek",
        });

        _content.Controls.Add(services);
        _content.Controls.Add(about);
        _content.Controls.Add(cards);
        _content.Controls.Add(lead);
        _content.Controls.Add(title);
    }

    private Panel MakeCard(string title, string desc, Action action)
    {
        var card = new Panel
        {
            Width = 220,
            Height = 110,
            Margin = new Padding(0, 0, 12, 12),
            BorderStyle = BorderStyle.FixedSingle,
            Cursor = Cursors.Hand,
        };
        card.Controls.Add(new Label { Text = desc, Dock = DockStyle.Bottom, Height = 36, Padding = new Padding(8, 0, 8, 8) });
        card.Controls.Add(new Label { Text = title, Dock = DockStyle.Top, Height = 28, Font = new Font("Segoe UI", 10F, FontStyle.Bold), Padding = new Padding(8, 8, 8, 0) });
        card.Click += (_, _) => action();
        foreach (Control c in card.Controls)
            c.Click += (_, _) => action();
        return card;
    }

    private void ShowFiles(string category)
    {
        ClearContent();
        var title = new Label
        {
            Text = AppConstants.CategoryTitle(category),
            Dock = DockStyle.Top,
            Height = 36,
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
        };
        var lead = new Label
        {
            Text = category == "general"
                ? "Şantiye listesi aşağıda. Detay için Görüntüle düğmesine basın."
                : "Dosya listesi. Çift tıklayarak açabilirsiniz.",
            Dock = DockStyle.Top,
            Height = 28,
        };

        var files = new FileManagerControl(category, allowManage: false) { Dock = DockStyle.Fill, Height = 360 };

        _content.Controls.Add(files);
        if (category == "general")
        {
            var sitesPanel = BuildSitesPanel();
            sitesPanel.Dock = DockStyle.Top;
            sitesPanel.Height = 220;
            _content.Controls.Add(sitesPanel);
        }
        _content.Controls.Add(lead);
        _content.Controls.Add(title);
    }

    private Panel BuildSitesPanel()
    {
        var panel = new Panel { Padding = new Padding(0, 8, 0, 8) };
        panel.Controls.Add(new Label
        {
            Text = "Şantiyeler",
            Dock = DockStyle.Top,
            Height = 24,
            Font = new Font("Segoe UI", 11F, FontStyle.Bold),
        });

        var list = new ListView { Dock = DockStyle.Fill, View = View.Details, FullRowSelect = true };
        list.Columns.Add("Şantiye", 220);
        list.Columns.Add("Adres", 360);
        list.Columns.Add("", 100);

        foreach (var site in LocalDatabase.GetSites())
        {
            var item = new ListViewItem(site.Name) { Tag = site };
            item.SubItems.Add(site.Address);
            item.SubItems.Add("Görüntüle");
            list.Items.Add(item);
        }

        list.DoubleClick += (_, _) => OpenSite(list);
        list.Click += (_, e) =>
        {
            if (list.SelectedItems.Count == 0) return;
            var pt = list.PointToClient(Cursor.Position);
            var hit = list.HitTest(pt);
            if (hit.SubItem != null && hit.Item != null && hit.Item.SubItems.IndexOf(hit.SubItem) == 2)
                OpenSite(list);
        };

        panel.Controls.Add(list);
        return panel;
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
        var panel = BuildSitesPanel();
        panel.Dock = DockStyle.Fill;
        _content.Controls.Add(panel);
        _content.Controls.Add(new Label
        {
            Text = "Şantiyeler",
            Dock = DockStyle.Top,
            Height = 36,
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
        });
    }

    private void ShowMap()
    {
        ClearContent();
        var settings = LocalDatabase.GetSettings();
        var title = new Label { Text = "Konum Haritası", Dock = DockStyle.Top, Height = 36, Font = new Font("Segoe UI", 14F, FontStyle.Bold) };
        var info = new Label
        {
            Dock = DockStyle.Top,
            Height = 120,
            Text = $"Etiket: {settings.MapLabel}\r\nAdres: {settings.ContactAddress}\r\nEnlem: {settings.MapLat}\r\nBoylam: {settings.MapLng}\r\n\r\nHaritayı tarayıcıda açmak için aşağıdaki düğmeyi kullanın.",
        };
        var btn = UiFactory.CreateButton("Google Haritada Aç", 160);
        btn.Click += (_, _) =>
        {
            var url = $"https://www.google.com/maps?q={settings.MapLat},{settings.MapLng}";
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true });
        };
        var flow = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 44 };
        flow.Controls.Add(btn);

        _content.Controls.Add(flow);
        _content.Controls.Add(info);
        _content.Controls.Add(title);
    }

    private void ShowContact()
    {
        ClearContent();
        var settings = LocalDatabase.GetSettings();
        var title = new Label { Text = "İletişim", Dock = DockStyle.Top, Height = 36, Font = new Font("Segoe UI", 14F, FontStyle.Bold) };
        var info = new Label
        {
            Dock = DockStyle.Top,
            Height = 100,
            Text = $"E-posta: {settings.ContactEmail}\r\nTelefon: {settings.ContactPhone}\r\nAdres: {settings.ContactAddress}",
        };
        _content.Controls.Add(info);
        _content.Controls.Add(title);
    }
}
