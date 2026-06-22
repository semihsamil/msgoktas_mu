namespace MsgoktasMu.Forms;

internal sealed class AboutForm : Form
{
    public AboutForm()
    {
        AppTheme.ApplyToForm(this);
        Text = "Hakkında";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(460, 260);

        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(24), BackColor = AppTheme.Surface, BorderStyle = BorderStyle.FixedSingle };
        panel.Controls.Add(new Label
        {
            Dock = DockStyle.Fill,
            Text =
                """
                Mimar Sinan Göktaş
                Şantiye Takip Programı (Masaüstü)

                Sürüm: 1.0
                Platform: Windows / .NET 8

                Şantiye bilgileri, dosya paylaşımı ve kullanıcı yönetimi için geliştirilmiştir.
                Veritabanı bu program klasöründe ayrı tutulur (msgoktas_mu.db).
                """,
            Font = AppTheme.BodyFont,
            ForeColor = AppTheme.Text,
        });

        var btnOk = UiFactory.CreateButton("Tamam", primary: true, width: 100);
        btnOk.Click += (_, _) => Close();
        var bottom = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 44, Padding = new Padding(12, 6, 12, 6), BackColor = AppTheme.Background };
        bottom.Controls.Add(btnOk);

        Controls.Add(panel);
        Controls.Add(bottom);
        AcceptButton = btnOk;
    }
}
