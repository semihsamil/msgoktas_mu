using MsgoktasMu.Core;
using MsgoktasMu.Data;
using MsgoktasMu.Models;

namespace MsgoktasMu.Forms;

internal sealed class RegisterForm : Form
{
    private readonly ComboBox _role = new() { DropDownStyle = ComboBoxStyle.DropDownList, Font = AppTheme.BodyFont, Height = 30 };
    private readonly TextBox _username = AppTheme.CreateInput(placeholder: "örn: ahmet.yilmaz");
    private readonly TextBox _password = AppTheme.CreateInput(password: true, placeholder: "en az 6 karakter");
    private readonly TextBox _password2 = AppTheme.CreateInput(password: true, placeholder: "şifreyi tekrar yazın");
    private readonly TextBox _fullName = AppTheme.CreateInput(placeholder: "Adınız Soyadınız");
    private readonly TextBox _phone = AppTheme.CreateInput(placeholder: "+90 5XXXXXXXXX");
    private readonly TextBox _siteName = AppTheme.CreateInput(placeholder: "örn: Merkez Şantiye");
    private readonly TextBox _companyName = AppTheme.CreateInput(placeholder: "örn: ABC İnşaat Ltd.");
    private readonly GroupBox _customerPanel;
    private readonly Label _status = new()
    {
        Dock = DockStyle.Top,
        ForeColor = AppTheme.Error,
        Height = 36,
        Font = AppTheme.BodyFont,
        Padding = new Padding(0, 8, 0, 0),
    };

    public RegisterForm()
    {
        AppTheme.ApplyToForm(this);
        Text = "Kayıt Ol | Mimar Sinan Göktaş";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(560, 680);

        _role.Items.AddRange(["Personel", "Kullanıcı"]);
        _role.SelectedIndex = 0;
        _role.SelectedIndexChanged += (_, _) => UpdateCustomerVisibility();

        var root = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20), AutoScroll = true, BackColor = AppTheme.Background };

        var title = new Label
        {
            Text = "Yeni Hesap Oluştur",
            Dock = DockStyle.Top,
            Height = 36,
            Font = AppTheme.TitleFont,
            ForeColor = AppTheme.Accent,
        };
        var subtitle = new Label
        {
            Text = "Aşağıdaki alanları doldurun. Yıldızlı alanlar zorunludur.",
            Dock = DockStyle.Top,
            Height = 24,
            ForeColor = AppTheme.TextMuted,
            Font = AppTheme.BodyFont,
            Margin = new Padding(0, 0, 0, 12),
        };

        var groupRole = AppTheme.CreateGroupBox("1. Rol");
        groupRole.Dock = DockStyle.Top;
        groupRole.Height = 110;
        var roleGrid = AppTheme.CreateFormGrid(1);
        AppTheme.AddFormRow(roleGrid, 0, "Kayıt türü *", _role, "Personel: ofis çalışanı | Kullanıcı: iş yapılan taraf");
        groupRole.Controls.Add(roleGrid);

        var groupAccount = AppTheme.CreateGroupBox("2. Giriş Bilgileri");
        groupAccount.Dock = DockStyle.Top;
        groupAccount.Height = 210;
        var accountGrid = AppTheme.CreateFormGrid(3);
        AppTheme.AddFormRow(accountGrid, 0, "Kullanıcı adı *", _username, "3-32 karakter, harf/rakam/.-_");
        AppTheme.AddFormRow(accountGrid, 1, "Şifre *", _password);
        AppTheme.AddFormRow(accountGrid, 2, "Şifre tekrar *", _password2);
        groupAccount.Controls.Add(accountGrid);

        var groupPersonal = AppTheme.CreateGroupBox("3. Kişisel Bilgiler");
        groupPersonal.Dock = DockStyle.Top;
        groupPersonal.Height = 150;
        var personalGrid = AppTheme.CreateFormGrid(2);
        AppTheme.AddFormRow(personalGrid, 0, "Ad Soyad", _fullName);
        AppTheme.AddFormRow(personalGrid, 1, "Telefon *", _phone, "Format: +90 5XXXXXXXXX");
        groupPersonal.Controls.Add(personalGrid);

        _customerPanel = AppTheme.CreateGroupBox("4. Kullanıcı Bilgileri (Kullanıcı rolü için)");
        _customerPanel.Dock = DockStyle.Top;
        _customerPanel.Height = 150;
        var customerGrid = AppTheme.CreateFormGrid(2);
        AppTheme.AddFormRow(customerGrid, 0, "Şantiye adı", _siteName);
        AppTheme.AddFormRow(customerGrid, 1, "Kurum / Firma", _companyName);
        _customerPanel.Controls.Add(customerGrid);

        var btnSave = UiFactory.CreateButton("Kayıt Ol", primary: true, width: 130);
        btnSave.Click += (_, _) => DoRegister();
        var btnCancel = UiFactory.CreateButton("İptal", primary: false, width: 100);
        btnCancel.Click += (_, _) => Close();
        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 52,
            Padding = new Padding(0, 8, 0, 0),
            BackColor = AppTheme.Background,
        };
        buttons.Controls.Add(btnSave);
        buttons.Controls.Add(btnCancel);

        root.Controls.Add(buttons);
        root.Controls.Add(_status);
        root.Controls.Add(_customerPanel);
        root.Controls.Add(groupPersonal);
        root.Controls.Add(groupAccount);
        root.Controls.Add(groupRole);
        root.Controls.Add(subtitle);
        root.Controls.Add(title);

        Controls.Add(root);
        UpdateCustomerVisibility();
    }

    private void UpdateCustomerVisibility()
    {
        _customerPanel.Visible = _role.SelectedIndex == 1;
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
        var nameErr = ValidationHelper.ValidateFullNameOptional(_fullName.Text.Trim());
        if (userErr != null || phoneErr != null || nameErr != null)
        {
            _status.Text = userErr ?? phoneErr ?? nameErr ?? "Geçersiz bilgi";
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
