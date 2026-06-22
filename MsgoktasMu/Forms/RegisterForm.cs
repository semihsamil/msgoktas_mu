using MsgoktasMu.Core;
using MsgoktasMu.Data;
using MsgoktasMu.Models;

namespace MsgoktasMu.Forms;

internal sealed class RegisterForm : Form
{
    private readonly ComboBox _role = new() { Dock = DockStyle.Top, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly TextBox _username = UiFactory.CreateTextBox();
    private readonly TextBox _password = new() { Dock = DockStyle.Top, Height = 28, UseSystemPasswordChar = true, Margin = new Padding(0, 0, 0, 8) };
    private readonly TextBox _password2 = new() { Dock = DockStyle.Top, Height = 28, UseSystemPasswordChar = true, Margin = new Padding(0, 0, 0, 8) };
    private readonly TextBox _fullName = UiFactory.CreateTextBox();
    private readonly TextBox _phone = UiFactory.CreateTextBox();
    private readonly TextBox _siteName = UiFactory.CreateTextBox();
    private readonly TextBox _companyName = UiFactory.CreateTextBox();
    private readonly Label _lblSite = UiFactory.CreateLabel("Şantiye Adı (varsa)");
    private readonly Label _lblCompany = UiFactory.CreateLabel("Kurum / Firma Adı");
    private readonly Label _status = new() { Dock = DockStyle.Top, ForeColor = Color.DarkRed, Height = 36 };

    public RegisterForm()
    {
        Text = "Kayıt Ol | Mimar Sinan Göktaş";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(460, 520);

        _role.Items.AddRange(["Personel", "Kullanıcı"]);
        _role.SelectedIndex = 0;
        _role.SelectedIndexChanged += (_, _) => ToggleCustomerFields();

        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(24), AutoScroll = true };
        var title = new Label
        {
            Text = "Yeni Hesap",
            Dock = DockStyle.Top,
            Height = 30,
            Font = new Font("Segoe UI", 12F, FontStyle.Bold),
        };

        var btnSave = UiFactory.CreateButton("Kayıt Ol", 120);
        btnSave.Click += (_, _) => DoRegister();
        var btnCancel = UiFactory.CreateButton("İptal", 100);
        btnCancel.Click += (_, _) => Close();
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 44 };
        buttons.Controls.Add(btnSave);
        buttons.Controls.Add(btnCancel);

        panel.Controls.Add(buttons);
        panel.Controls.Add(_status);
        panel.Controls.Add(_companyName);
        panel.Controls.Add(_lblCompany);
        panel.Controls.Add(_siteName);
        panel.Controls.Add(_lblSite);
        panel.Controls.Add(_phone);
        panel.Controls.Add(UiFactory.CreateLabel("Telefon (+90 5...)"));
        panel.Controls.Add(_fullName);
        panel.Controls.Add(UiFactory.CreateLabel("Ad Soyad"));
        panel.Controls.Add(_password2);
        panel.Controls.Add(UiFactory.CreateLabel("Şifre (Tekrar)"));
        panel.Controls.Add(_password);
        panel.Controls.Add(UiFactory.CreateLabel("Şifre"));
        panel.Controls.Add(_username);
        panel.Controls.Add(UiFactory.CreateLabel("Kullanıcı adı"));
        panel.Controls.Add(_role);
        panel.Controls.Add(UiFactory.CreateLabel("Rol"));
        panel.Controls.Add(title);

        Controls.Add(panel);
        ToggleCustomerFields();
    }

    private void ToggleCustomerFields()
    {
        var customer = _role.SelectedIndex == 1;
        _lblSite.Visible = customer;
        _siteName.Visible = customer;
        _lblCompany.Visible = customer;
        _companyName.Visible = customer;
    }

    private void DoRegister()
    {
        var role = _role.SelectedIndex == 0 ? "personel" : "is_yapilan";
        var username = _username.Text.Trim();
        var password = _password.Text;
        var password2 = _password2.Text;
        var phone = _phone.Text.Trim();

        if (password != password2)
        {
            _status.Text = "Şifreler eşleşmiyor.";
            return;
        }

        var userErr = ValidationHelper.ValidateUsername(username) ?? ValidationHelper.ValidatePassword(password);
        var phoneErr = ValidationHelper.ValidatePhone(phone);
        if (userErr != null || phoneErr != null)
        {
            _status.Text = userErr ?? phoneErr ?? "Geçersiz bilgi";
            return;
        }

        try
        {
            LocalDatabase.Register(new UserRecord
            {
                Username = username,
                Password = password,
                Role = role,
                FullName = _fullName.Text.Trim(),
                Phone = phone,
                SiteName = role == "is_yapilan" ? _siteName.Text.Trim() : "",
                CompanyName = role == "is_yapilan" ? _companyName.Text.Trim() : "",
            });
            UiFactory.ShowInfo("Kayıt başarılı. Giriş yapabilirsiniz.");
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            _status.Text = ex.Message;
        }
    }
}
