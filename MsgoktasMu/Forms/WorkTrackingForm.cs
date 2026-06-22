using MsgoktasMu.Core;
using MsgoktasMu.Data;

namespace MsgoktasMu.Forms;

internal sealed class WorkTrackingForm : Form
{
    public WorkTrackingForm()
    {
        var user = AuthSession.Current!;
        Text = "İş Takibi | Mimar Sinan Göktaş";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(760, 520);
        ClientSize = new Size(860, 580);

        var top = new Panel { Dock = DockStyle.Top, Height = 120, Padding = new Padding(16) };
        top.Controls.Add(new Label
        {
            Dock = DockStyle.Fill,
            Text = $"""
                Kullanıcı: {user.Username}
                Rol: {AppConstants.RoleLabel(user.Role)}
                Ad Soyad: {user.FullName}
                """,
        });

        var files = new FileManagerControl("reports", allowManage: false) { Dock = DockStyle.Fill };
        var title = new Label
        {
            Text = "Günlük Raporlar",
            Dock = DockStyle.Top,
            Height = 32,
            Font = new Font("Segoe UI", 12F, FontStyle.Bold),
            Padding = new Padding(16, 8, 0, 0),
        };

        var btnClose = UiFactory.CreateButton("Kapat", 100);
        btnClose.Click += (_, _) => Close();
        var bottom = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 44, Padding = new Padding(12, 6, 12, 6) };
        bottom.Controls.Add(btnClose);

        Controls.Add(files);
        Controls.Add(title);
        Controls.Add(top);
        Controls.Add(bottom);
    }
}
