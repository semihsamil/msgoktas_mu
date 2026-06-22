using MsgoktasMu.Core;
using MsgoktasMu.Data;

namespace MsgoktasMu.Forms;

internal sealed class LoginForm : Form
{
    private readonly TextBox _username = AppTheme.CreateInput(placeholder: "Kullanıcı adınız");
    private readonly TextBox _password = AppTheme.CreateInput(password: true, placeholder: "Şifreniz");
    private readonly Label _status = new()
    {
        Dock = DockStyle.Top,
        ForeColor = AppTheme.Error,
        Height = 28,
        Font = AppTheme.BodyFont,
    };

    public LoginForm()
    {
        AppTheme.ApplyToForm(this);
        Text = "Giriş | Mimar Sinan Göktaş";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(480, 340);

        var card = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(24),
            BackColor = AppTheme.Surface,
            BorderStyle = BorderStyle.FixedSingle,
        };

        var title = new Label
        {
            Text = "Giriş Yap",
            Dock = DockStyle.Top,
            Height = 34,
            Font = AppTheme.TitleFont,
            ForeColor = AppTheme.Accent,
        };
        var subtitle = new Label
        {
            Text = "Admin, personel veya kullanıcı hesabınızla giriş yapın.",
            Dock = DockStyle.Top,
            Height = 22,
            ForeColor = AppTheme.TextMuted,
            Font = AppTheme.BodyFont,
            Margin = new Padding(0, 0, 0, 12),
        };

        var grid = AppTheme.CreateFormGrid(2);
        AppTheme.AddFormRow(grid, 0, "Kullanıcı adı *", _username);
        AppTheme.AddFormRow(grid, 1, "Şifre *", _password);

        var btnLogin = UiFactory.CreateButton("Giriş Yap", primary: true, width: 120);
        btnLogin.Click += (_, _) => DoLogin();
        var btnRegister = UiFactory.CreateButton("Kayıt Ol", primary: false, width: 110);
        btnRegister.Click += (_, _) =>
        {
            using var reg = new RegisterForm();
            reg.ShowDialog(this);
        };
        var btnCancel = UiFactory.CreateButton("İptal", primary: false, width: 90);
        btnCancel.Click += (_, _) => Close();

        var buttons = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 48, Padding = new Padding(0, 10, 0, 0) };
        buttons.Controls.Add(btnLogin);
        buttons.Controls.Add(btnRegister);
        buttons.Controls.Add(btnCancel);

        card.Controls.Add(buttons);
        card.Controls.Add(_status);
        card.Controls.Add(grid);
        card.Controls.Add(subtitle);
        card.Controls.Add(title);

        var outer = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20), BackColor = AppTheme.Background };
        outer.Controls.Add(card);
        Controls.Add(outer);
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
