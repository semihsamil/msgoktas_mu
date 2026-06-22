using MsgoktasMu.Core;
using MsgoktasMu.Data;
using MsgoktasMu.Models;

namespace MsgoktasMu.Forms;

internal sealed class AdminForm : Form
{
    private readonly TabControl _tabs = new() { Dock = DockStyle.Fill };
    private ListView? _sitesList;
    private TextBox? _siteName;
    private TextBox? _siteAddress;
    private TextBox? _sitePhone;
    private TextBox? _siteLat;
    private TextBox? _siteLng;
    private TextBox? _siteDescription;
    private int? _editingSiteId;
    private Label? _siteStatus;
    private DataGridView? _usersGrid;
    private TextBox? _contactEmail;
    private TextBox? _contactPhone;
    private TextBox? _contactAddress;
    private TextBox? _mapLat;
    private TextBox? _mapLng;
    private TextBox? _mapLabel;
    private Label? _settingsStatus;
    private Label? _usersStatus;

    public AdminForm()
    {
        Text = "Admin Paneli | Mimar Sinan Göktaş";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(980, 640);
        ClientSize = new Size(1100, 720);

        var top = new Panel { Dock = DockStyle.Top, Height = 52, Padding = new Padding(12, 10, 12, 0) };
        var title = new Label
        {
            Text = "Admin Paneli",
            Dock = DockStyle.Left,
            Width = 200,
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
        };
        var btnClose = UiFactory.CreateButton("Kapat", 100);
        btnClose.Click += (_, _) => Close();
        var actions = new FlowLayoutPanel { Dock = DockStyle.Right, Width = 120, FlowDirection = FlowDirection.RightToLeft };
        actions.Controls.Add(btnClose);
        top.Controls.Add(actions);
        top.Controls.Add(title);

        BuildTabs();

        Controls.Add(_tabs);
        Controls.Add(top);
    }

    private void BuildTabs()
    {
        _tabs.TabPages.Add(BuildGeneralTab());
        _tabs.TabPages.Add(BuildFileTab("Günlük Raporlar", "reports"));
        _tabs.TabPages.Add(BuildFileTab("Bilgi Notları", "notes"));
        _tabs.TabPages.Add(BuildFileTab("Çizelge", "schedule"));
        _tabs.TabPages.Add(BuildUsersTab());
        _tabs.TabPages.Add(BuildPreviewTab());
        _tabs.TabPages.Add(BuildSettingsTab());
    }

    private TabPage BuildGeneralTab()
    {
        var page = new TabPage("Genel Bilgiler");
        var split = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, SplitterDistance = 260 };

        _sitesList = new ListView { Dock = DockStyle.Fill, View = View.Details, FullRowSelect = true };
        _sitesList.Columns.Add("Ad", 180);
        _sitesList.Columns.Add("Adres", 260);
        _sitesList.SelectedIndexChanged += (_, _) => LoadSelectedSite();

        var siteButtons = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 40 };
        var btnNew = UiFactory.CreateButton("Yeni", 80);
        btnNew.Click += (_, _) => ClearSiteForm();
        var btnDelete = UiFactory.CreateButton("Sil", 80);
        btnDelete.Click += (_, _) => DeleteSite();
        siteButtons.Controls.Add(btnNew);
        siteButtons.Controls.Add(btnDelete);

        var siteTop = new Panel { Dock = DockStyle.Top, Height = 280, Padding = new Padding(8) };
        _siteName = UiFactory.CreateTextBox();
        _siteAddress = UiFactory.CreateTextBox();
        _sitePhone = UiFactory.CreateTextBox();
        _siteLat = UiFactory.CreateTextBox();
        _siteLng = UiFactory.CreateTextBox();
        _siteDescription = UiFactory.CreateTextBox(multiline: true, height: 60);
        _siteStatus = new Label { Dock = DockStyle.Top, Height = 20, ForeColor = Color.DarkGreen };

        var btnSaveSite = UiFactory.CreateButton("Şantiyeyi Kaydet", 130);
        btnSaveSite.Click += (_, _) => SaveSite();
        var siteFormButtons = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 40 };
        siteFormButtons.Controls.Add(btnSaveSite);

        siteTop.Controls.Add(_siteStatus);
        siteTop.Controls.Add(siteFormButtons);
        siteTop.Controls.Add(_siteDescription);
        siteTop.Controls.Add(UiFactory.CreateLabel("Açıklama"));
        siteTop.Controls.Add(_siteLng);
        siteTop.Controls.Add(UiFactory.CreateLabel("Boylam"));
        siteTop.Controls.Add(_siteLat);
        siteTop.Controls.Add(UiFactory.CreateLabel("Enlem"));
        siteTop.Controls.Add(_sitePhone);
        siteTop.Controls.Add(UiFactory.CreateLabel("Telefon"));
        siteTop.Controls.Add(_siteAddress);
        siteTop.Controls.Add(UiFactory.CreateLabel("Adres"));
        siteTop.Controls.Add(_siteName);
        siteTop.Controls.Add(UiFactory.CreateLabel("Şantiye adı *"));

        var sitePanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(8) };
        sitePanel.Controls.Add(_sitesList);
        sitePanel.Controls.Add(siteButtons);
        sitePanel.Controls.Add(new Label { Text = "Kayıtlı şantiyeler", Dock = DockStyle.Top, Height = 22, Font = new Font("Segoe UI", 9F, FontStyle.Bold) });

        split.Panel1.Controls.Add(sitePanel);
        split.Panel1.Controls.Add(siteTop);
        split.Panel2.Controls.Add(new FileManagerControl("general", allowManage: true) { Dock = DockStyle.Fill });
        split.Panel2.Controls.Add(new Label { Text = "Genel dosyalar", Dock = DockStyle.Top, Height = 22, Font = new Font("Segoe UI", 9F, FontStyle.Bold), Padding = new Padding(8, 8, 0, 0) });

        page.Controls.Add(split);
        ReloadSites();
        return page;
    }

    private TabPage BuildFileTab(string title, string category)
    {
        var page = new TabPage(title);
        page.Controls.Add(new FileManagerControl(category, allowManage: true) { Dock = DockStyle.Fill });
        return page;
    }

    private TabPage BuildUsersTab()
    {
        var page = new TabPage("Kullanıcılar");
        _usersGrid = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            AllowUserToAddRows = false,
            RowHeadersVisible = false,
        };

        _usersGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Username", HeaderText = "Kullanıcı adı" });
        _usersGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Password", HeaderText = "Şifre" });
        _usersGrid.Columns.Add(new DataGridViewComboBoxColumn
        {
            Name = "Role",
            HeaderText = "Rol",
            DataSource = AppConstants.ValidRoles.Select(r => new { Value = r, Text = AppConstants.RoleLabel(r) }).ToList(),
            DisplayMember = "Text",
            ValueMember = "Value",
        });
        _usersGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "FullName", HeaderText = "Ad Soyad" });
        _usersGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Phone", HeaderText = "Telefon" });
        _usersGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "SiteName", HeaderText = "Şantiye" });
        _usersGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "CompanyName", HeaderText = "Kurum" });
        _usersGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "ExtraNote", HeaderText = "Not" });
        _usersGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", Visible = false });

        var buttons = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 42, Padding = new Padding(8, 8, 8, 0) };
        var btnReload = UiFactory.CreateButton("Yenile", 90);
        btnReload.Click += (_, _) => ReloadUsers();
        var btnSave = UiFactory.CreateButton("Seçiliyi Kaydet", 120);
        btnSave.Click += (_, _) => SaveSelectedUser();
        var btnDelete = UiFactory.CreateButton("Seçiliyi Sil", 100);
        btnDelete.Click += (_, _) => DeleteSelectedUser();
        buttons.Controls.Add(btnReload);
        buttons.Controls.Add(btnSave);
        buttons.Controls.Add(btnDelete);

        _usersStatus = new Label { Dock = DockStyle.Bottom, Height = 22, Padding = new Padding(8, 0, 8, 4) };

        page.Controls.Add(_usersGrid);
        page.Controls.Add(_usersStatus);
        page.Controls.Add(buttons);
        ReloadUsers();
        return page;
    }

    private TabPage BuildPreviewTab()
    {
        var page = new TabPage("Site Önizleme");
        var flow = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(16), AutoScroll = true };
        flow.Controls.Add(new Label
        {
            Text = "Ana program ekranını ziyaretçi gözüyle incelemek için ana pencereyi kullanın.",
            AutoSize = true,
            MaximumSize = new Size(700, 0),
        });
        page.Controls.Add(flow);
        return page;
    }

    private TabPage BuildSettingsTab()
    {
        var page = new TabPage("İletişim & Harita");
        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(16), AutoScroll = true, Width = 520 };
        var settings = LocalDatabase.GetSettings();

        _contactEmail = UiFactory.CreateTextBox();
        _contactEmail.Text = settings.ContactEmail;
        _contactPhone = UiFactory.CreateTextBox();
        _contactPhone.Text = settings.ContactPhone;
        _contactAddress = UiFactory.CreateTextBox();
        _contactAddress.Text = settings.ContactAddress;
        _mapLat = UiFactory.CreateTextBox();
        _mapLat.Text = settings.MapLat;
        _mapLng = UiFactory.CreateTextBox();
        _mapLng.Text = settings.MapLng;
        _mapLabel = UiFactory.CreateTextBox();
        _mapLabel.Text = settings.MapLabel;
        _settingsStatus = new Label { Dock = DockStyle.Top, Height = 20, ForeColor = Color.DarkGreen };

        var btnSave = UiFactory.CreateButton("Ayarları Kaydet", 140);
        btnSave.Click += (_, _) => SaveSettings();
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 40 };
        buttons.Controls.Add(btnSave);

        panel.Controls.Add(_settingsStatus);
        panel.Controls.Add(buttons);
        panel.Controls.Add(_mapLabel);
        panel.Controls.Add(UiFactory.CreateLabel("Harita etiketi"));
        panel.Controls.Add(_mapLng);
        panel.Controls.Add(UiFactory.CreateLabel("Boylam"));
        panel.Controls.Add(_mapLat);
        panel.Controls.Add(UiFactory.CreateLabel("Enlem"));
        panel.Controls.Add(_contactAddress);
        panel.Controls.Add(UiFactory.CreateLabel("Adres"));
        panel.Controls.Add(_contactPhone);
        panel.Controls.Add(UiFactory.CreateLabel("Telefon"));
        panel.Controls.Add(_contactEmail);
        panel.Controls.Add(UiFactory.CreateLabel("E-posta"));

        page.Controls.Add(panel);
        return page;
    }

    private void ReloadSites()
    {
        if (_sitesList == null) return;
        _sitesList.Items.Clear();
        foreach (var site in LocalDatabase.GetSites())
        {
            var item = new ListViewItem(site.Name) { Tag = site.Id };
            item.SubItems.Add(site.Address);
            _sitesList.Items.Add(item);
        }
    }

    private void LoadSelectedSite()
    {
        if (_sitesList?.SelectedItems.Count != 1) return;
        var id = (int)_sitesList.SelectedItems[0].Tag!;
        var site = LocalDatabase.GetSite(id);
        if (site == null) return;
        _editingSiteId = site.Id;
        _siteName!.Text = site.Name;
        _siteAddress!.Text = site.Address;
        _sitePhone!.Text = site.Phone;
        _siteLat!.Text = site.Lat;
        _siteLng!.Text = site.Lng;
        _siteDescription!.Text = site.Description;
    }

    private void ClearSiteForm()
    {
        _editingSiteId = null;
        if (_sitesList != null)
            _sitesList.SelectedIndices.Clear();
        _siteName!.Text = "";
        _siteAddress!.Text = "";
        _sitePhone!.Text = "";
        _siteLat!.Text = "";
        _siteLng!.Text = "";
        _siteDescription!.Text = "";
        _siteStatus!.Text = "";
    }

    private void SaveSite()
    {
        var input = new ConstructionSiteInput
        {
            Name = _siteName!.Text,
            Address = _siteAddress!.Text,
            Phone = _sitePhone!.Text,
            Lat = _siteLat!.Text,
            Lng = _siteLng!.Text,
            Description = _siteDescription!.Text,
        };

        try
        {
            if (_editingSiteId.HasValue)
            {
                LocalDatabase.UpdateSite(_editingSiteId.Value, input);
                _siteStatus!.Text = "Şantiye güncellendi.";
            }
            else
            {
                var id = LocalDatabase.AddSite(input);
                _editingSiteId = id;
                _siteStatus!.Text = "Şantiye eklendi.";
            }
            ReloadSites();
        }
        catch (Exception ex)
        {
            UiFactory.ShowError(ex.Message);
        }
    }

    private void DeleteSite()
    {
        if (!_editingSiteId.HasValue)
        {
            UiFactory.ShowError("Silinecek şantiye seçin.");
            return;
        }
        if (!UiFactory.Confirm("Bu şantiyeyi silmek istiyor musunuz?")) return;
        try
        {
            LocalDatabase.DeleteSite(_editingSiteId.Value);
            ClearSiteForm();
            ReloadSites();
        }
        catch (Exception ex)
        {
            UiFactory.ShowError(ex.Message);
        }
    }

    private void ReloadUsers()
    {
        if (_usersGrid == null) return;
        _usersGrid.Rows.Clear();
        foreach (var user in LocalDatabase.GetUsers())
        {
            _usersGrid.Rows.Add(user.Username, user.Password, user.Role, user.FullName, user.Phone, user.SiteName, user.CompanyName, user.ExtraNote, user.Id);
        }
    }

    private void SaveSelectedUser()
    {
        if (_usersGrid?.CurrentRow == null) return;
        var row = _usersGrid.CurrentRow;
        var user = new UserRecord
        {
            Id = Convert.ToInt32(row.Cells["Id"].Value),
            Username = Convert.ToString(row.Cells["Username"].Value) ?? "",
            Password = Convert.ToString(row.Cells["Password"].Value) ?? "",
            Role = Convert.ToString(row.Cells["Role"].Value) ?? "personel",
            FullName = Convert.ToString(row.Cells["FullName"].Value) ?? "",
            Phone = Convert.ToString(row.Cells["Phone"].Value) ?? "",
            SiteName = Convert.ToString(row.Cells["SiteName"].Value) ?? "",
            CompanyName = Convert.ToString(row.Cells["CompanyName"].Value) ?? "",
            ExtraNote = Convert.ToString(row.Cells["ExtraNote"].Value) ?? "",
        };

        var userErr = ValidationHelper.ValidateUsername(user.Username) ?? ValidationHelper.ValidatePassword(user.Password);
        var phoneErr = ValidationHelper.ValidatePhoneOptional(user.Phone);
        if (userErr != null || phoneErr != null)
        {
            UiFactory.ShowError(userErr ?? phoneErr ?? "Geçersiz bilgi");
            return;
        }

        try
        {
            LocalDatabase.UpdateUser(user);
            _usersStatus!.Text = "Kullanıcı güncellendi.";
            ReloadUsers();
        }
        catch (Exception ex)
        {
            UiFactory.ShowError(ex.Message);
        }
    }

    private void DeleteSelectedUser()
    {
        if (_usersGrid?.CurrentRow == null) return;
        var id = Convert.ToInt32(_usersGrid.CurrentRow.Cells["Id"].Value);
        if (!UiFactory.Confirm("Bu kullanıcıyı silmek istiyor musunuz?")) return;
        try
        {
            LocalDatabase.DeleteUser(id, AuthSession.Current!.Id);
            _usersStatus!.Text = "Kullanıcı silindi.";
            ReloadUsers();
        }
        catch (Exception ex)
        {
            UiFactory.ShowError(ex.Message);
        }
    }

    private void SaveSettings()
    {
        try
        {
            LocalDatabase.SaveSettings(new AppSettings
            {
                ContactEmail = _contactEmail!.Text.Trim(),
                ContactPhone = _contactPhone!.Text.Trim(),
                ContactAddress = _contactAddress!.Text.Trim(),
                MapLat = _mapLat!.Text.Trim(),
                MapLng = _mapLng!.Text.Trim(),
                MapLabel = _mapLabel!.Text.Trim(),
            });
            _settingsStatus!.Text = "Ayarlar kaydedildi.";
        }
        catch (Exception ex)
        {
            UiFactory.ShowError(ex.Message);
        }
    }
}
