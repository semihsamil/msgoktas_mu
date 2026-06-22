using MsgoktasMu.Models;

namespace MsgoktasMu.Forms;

internal sealed class SiteDetailForm : Form
{
    public SiteDetailForm(ConstructionSite site)
    {
        AppTheme.ApplyToForm(this);
        Text = site.Name + " | Şantiye Detayı";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        ClientSize = new Size(540, 440);

        var outer = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20), BackColor = AppTheme.Background };
        var card = new Panel { Dock = DockStyle.Fill, BackColor = AppTheme.Surface, BorderStyle = BorderStyle.FixedSingle, Padding = new Padding(18) };

        var title = new Label
        {
            Text = site.Name,
            Dock = DockStyle.Top,
            Height = 34,
            Font = AppTheme.SectionFont,
            ForeColor = AppTheme.Accent,
        };

        var grid = AppTheme.CreateFormGrid(4);
        var address = string.IsNullOrWhiteSpace(site.Address) ? "Belirtilmemiş" : site.Address;
        var phone = string.IsNullOrWhiteSpace(site.Phone) ? "Belirtilmemiş" : site.Phone;
        grid.Controls.Add(AppTheme.CreateFieldLabel("Adres"), 0, 0);
        grid.Controls.Add(new Label { Text = address, Dock = DockStyle.Fill, ForeColor = AppTheme.Text, Padding = new Padding(0, 8, 0, 0) }, 1, 0);
        grid.Controls.Add(AppTheme.CreateFieldLabel("Telefon"), 0, 1);
        grid.Controls.Add(new Label { Text = phone, Dock = DockStyle.Fill, ForeColor = AppTheme.Text, Padding = new Padding(0, 8, 0, 0) }, 1, 1);
        grid.Controls.Add(AppTheme.CreateFieldLabel("Enlem / Boylam"), 0, 2);
        grid.Controls.Add(new Label { Text = $"{site.Lat} / {site.Lng}", Dock = DockStyle.Fill, ForeColor = AppTheme.Text, Padding = new Padding(0, 8, 0, 0) }, 1, 2);
        grid.Controls.Add(AppTheme.CreateFieldLabel("Açıklama"), 0, 3);
        grid.Controls.Add(new Label
        {
            Text = string.IsNullOrWhiteSpace(site.Description) ? "Yok" : site.Description,
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.Text,
            Padding = new Padding(0, 8, 0, 0),
        }, 1, 3);

        var btnMap = UiFactory.CreateButton("Haritada Aç", primary: true, width: 130);
        btnMap.Click += (_, _) =>
        {
            if (string.IsNullOrWhiteSpace(site.Lat) || string.IsNullOrWhiteSpace(site.Lng))
            {
                UiFactory.ShowError("Bu şantiye için koordinat girilmemiş.");
                return;
            }
            var url = $"https://www.google.com/maps?q={site.Lat},{site.Lng}";
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true });
        };
        var btnClose = UiFactory.CreateButton("Kapat", primary: false, width: 100);
        btnClose.Click += (_, _) => Close();
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 48, Padding = new Padding(0, 10, 0, 0) };
        buttons.Controls.Add(btnMap);
        buttons.Controls.Add(btnClose);

        card.Controls.Add(buttons);
        card.Controls.Add(grid);
        card.Controls.Add(title);
        outer.Controls.Add(card);
        Controls.Add(outer);
    }
}
