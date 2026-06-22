using MsgoktasMu.Core;
using MsgoktasMu.Data;

namespace MsgoktasMu.Forms;

internal sealed class WorkTrackingForm : Form
{
    public WorkTrackingForm()
    {
        AppTheme.ApplyToForm(this);
        var user = AuthSession.Current!;
        Text = "İş Takibi | Mimar Sinan Göktaş";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(820, 560);
        ClientSize = new Size(900, 620);

        var top = AppTheme.CreateGroupBox("Hesap Bilgileri");
        top.Dock = DockStyle.Top;
        top.Height = 100;
        top.Controls.Add(new Label
        {
            Dock = DockStyle.Fill,
            Font = AppTheme.BodyFont,
            ForeColor = AppTheme.Text,
            Text = $"Kullanıcı: {user.Username}\r\nRol: {AppConstants.RoleLabel(user.Role)}\r\nAd Soyad: {(string.IsNullOrWhiteSpace(user.FullName) ? "-" : user.FullName)}",
        });

        var filesBox = AppTheme.CreateGroupBox("Günlük Raporlar");
        filesBox.Dock = DockStyle.Fill;
        filesBox.Controls.Add(new FileManagerControl("reports", allowManage: false) { Dock = DockStyle.Fill });

        var btnClose = UiFactory.CreateButton("Kapat", primary: false, width: 100);
        btnClose.Click += (_, _) => Close();
        var bottom = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 48, Padding = new Padding(12, 8, 12, 8), BackColor = AppTheme.Background };
        bottom.Controls.Add(btnClose);

        Controls.Add(filesBox);
        Controls.Add(top);
        Controls.Add(bottom);
    }
}
