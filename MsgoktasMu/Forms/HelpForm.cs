namespace MsgoktasMu.Forms;

internal sealed class HelpForm : Form
{
    public HelpForm(string? initialTab = null)
    {
        AppTheme.ApplyToForm(this);
        Text = "Yardım | Mimar Sinan Göktaş";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(720, 520);
        ClientSize = new Size(820, 580);

        var tabs = new TabControl { Dock = DockStyle.Fill, Font = AppTheme.BodyFont };
        tabs.TabPages.Add(CreateTab("Genel Bakış", HelpTexts.Genel));
        tabs.TabPages.Add(CreateTab("Ziyaretçi", HelpTexts.Ziyaretci));
        tabs.TabPages.Add(CreateTab("Admin", HelpTexts.Admin));
        tabs.TabPages.Add(CreateTab("Hata Mesajları", HelpTexts.Hatalar));

        if (!string.IsNullOrEmpty(initialTab))
        {
            foreach (TabPage page in tabs.TabPages)
            {
                if (page.Text == initialTab)
                {
                    tabs.SelectedTab = page;
                    break;
                }
            }
        }

        var btnClose = UiFactory.CreateButton("Kapat", primary: false, width: 100);
        btnClose.Click += (_, _) => Close();
        var bottom = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 48, Padding = new Padding(12, 8, 12, 8), BackColor = AppTheme.Background };
        bottom.Controls.Add(btnClose);

        Controls.Add(tabs);
        Controls.Add(bottom);
    }

    private static TabPage CreateTab(string title, string body)
    {
        var page = new TabPage(title) { Padding = new Padding(8), BackColor = AppTheme.Background };
        var box = new RichTextBox
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            BorderStyle = BorderStyle.None,
            BackColor = AppTheme.Surface,
            Font = AppTheme.BodyFont,
            Text = body,
        };
        page.Controls.Add(box);
        return page;
    }

    private static class HelpTexts
    {
        public const string Genel =
            """
            Mimar Sinan Göktaş — Şantiye Takip Programı

            Bu program mimarlık ofisinin şantiye bilgilerini, dosyalarını ve kullanıcılarını yönetmek için hazırlanmıştır.

            Roller:
            • Ziyaretçi — Giriş yapmadan dosya ve şantiye listesini görür
            • Personel — Giriş yaparak iş takibi ekranını kullanır
            • Kullanıcı — Kayıt sırasında kurum/şantiye bilgisi girer
            • Admin — Tüm kayıtları yönetir

            Admin giriş bilgileri sistem yöneticisi tarafından verilir.
            """;

        public const string Ziyaretci =
            """
            1. Üst menüden istediğiniz bölüme gidin.
            2. Genel Bilgiler — Şantiye listesi ve genel dosyalar
            3. Raporlar / Notlar / Çizelge — İlgili dosyaları çift tıklayarak açın
            4. Şantiyeler — Listeden satıra çift tıklayın, detay penceresi açılır
            5. Harita — Ofis koordinatlarını görür, Google Haritada Aç ile tarayıcıda açarsınız
            6. Giriş / Admin — Hesabınızla giriş yapın veya Kayıt Ol ile yeni hesap oluşturun

            Kayıt olurken:
            • Telefon: +90 5XXXXXXXXX formatında yazın
            • Kullanıcı adı: 3-32 karakter
            • Şifre: en az 6 karakter
            """;

        public const string Admin =
            """
            Admin paneli sekmeleri:

            Genel Bilgiler — Şantiye ekle/düzenle/sil, genel dosya yükle
            Günlük Raporlar — Rapor dosyası yükle ve sil
            Bilgi Notları — Not dosyası yükle ve sil
            Çizelge — Çizelge dosyası yükle ve sil
            Kullanıcılar — Kullanıcı adı, şifre, rol ve diğer alanları düzenle
            İletişim & Harita — Ana sayfa iletişim ve ofis konumu ayarları

            Dosya yükleme: Dosya Ekle → bilgisayardan seç → listede görünür
            Desteklenen türler: pdf, doc, docx, xlsx, xls, txt, png, jpg (en fazla 15 MB)
            """;

        public const string Hatalar =
            """
            Sık görülen uyarılar:

            • "Telefon formatı +90 5XXXXXXXXX olmalı" — Başında +90 ve 5 ile 10 hane olmalı
            • "Kullanıcı adı 3-32 karakter olmalı" — Kısa veya uzun kullanıcı adı
            • "Enlem sayı olmalı" — Koordinata harf yazmayın, 39.7477 gibi girin
            • "Bu kullanıcı adı zaten kayıtlı" — Farklı bir kullanıcı adı seçin
            • "Son admin kullanıcısı silinemez" — En az bir admin kalmalı

            Program hata verirse kapanmaz; uyarı penceresi gösterir. Girdiğiniz bilgiyi kontrol edip tekrar deneyin.
            """;
    }
}
