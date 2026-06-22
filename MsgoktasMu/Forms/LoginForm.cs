using MsgoktasMu.Core;
using MsgoktasMu.Data;

namespace MsgoktasMu.Forms;

internal sealed class LoginForm : Form
{
    private readonly TextBox _username = UiFactory.CreateTextBox();
    private readonly TextBox _password = new()
    {
        Dock = DockStyle.Top,
        Height = 28,
        UseSystemPasswordChar = true,
        Font = new Font("Segoe UI", 9F),
        Margin = new Padding(0, 0, 0, 8),
    };
    private readonly Label _status = new() { Dock = DockStyle.Top, ForeColor = Color.DarkRed, Height = 20 };

    public LoginForm()
    {
        Text = "Giriş | Mimar Sinan Göktaş";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(420, 260);

        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(24) };
        var title = new Label
        {
            Text = "Yönetici / Kullanıcı Girişi",
            Dock = DockStyle.Top,
            Height = 32,
            Font = new Font("Segoe UI", 12F, FontStyle.Bold),
        };

        var btnLogin = UiFactory.CreateButton("Giriş Yap", 120);
        btnLogin.Click += (_, _) => DoLogin();
        var btnRegister = UiFactory.CreateButton("Kayıt Ol", 120);
        btnRegister.Click += (_, _) =>
        {
            using var reg = new RegisterForm();
            reg.ShowDialog(this);
        };
        var btnCancel = UiFactory.CreateButton("İptal", 100);
        btnCancel.Click += (_, _) => Close();

        var buttons = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 44, FlowDirection = FlowDirection.LeftToRight };
        buttons.Controls.Add(btnLogin);
        buttons.Controls.Add(btnRegister);
        buttons.Controls.Add(btnCancel);

        panel.Controls.Add(buttons);
        panel.Controls.Add(_status);
        panel.Controls.Add(_password);
        panel.Controls.Add(UiFactory.CreateLabel("Şifre"));
        panel.Controls.Add(_username);
        panel.Controls.Add(UiFactory.CreateLabel("Kullanıcı adı"));
        panel.Controls.Add(title);

        Controls.Add(panel);
        AcceptButton = btnLogin;
    }

    private void DoLogin()
    {
        var username = _username.Text.Trim();
        var password = _password.Text;
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            _status.Text = "Kullanıcı adı ve şifre zorunlu.";
            return;
        }

        var user = LocalDatabase.Login(username, password);
        if (user == null)
        {
            _status.Text = "Kullanıcı adı veya şifre hatalı.";
            return;
        }

        AuthSession.Set(new UserSession
        {
            Id = user.Id,
            Username = user.Username,
            Role = user.Role,
            FullName = user.FullName,
        });

        DialogResult = DialogResult.OK;
        Close();
    }
}
