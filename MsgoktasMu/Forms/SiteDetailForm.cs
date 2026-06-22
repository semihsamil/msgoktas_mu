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
        ClientSize = new Size(520, 420);

        var outer = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20), BackColor = AppTheme.Background };

        var card = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.Surface,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(18),
        };

        var title = new Label
        {
            Text = site.Name,
            Dock = DockStyle.Top,
            Height = 36,
            Font = AppTheme.SectionFont,
            ForeColor = AppTheme.Accent,
        };

        var grid = AppTheme.CreateFormGrid(4);
        grid.Dock = DockStyle.Top;
        grid.Margin = new Padding(0, 8, 0, 0);
        AppTheme.AddDisplayRow(grid, 0, "Adres", string.IsNullOrWhiteSpace(site.Address) ? "Belirtilmemiş" : site.Address, 330);
        AppTheme.AddDisplayRow(grid, 1, "Telefon", string.IsNullOrWhiteSpace(site.Phone) ? "Belirtilmemiş" : site.Phone, 330);
        AppTheme.AddDisplayRow(grid, 2, "Enlem / Boylam", $"{site.Lat} / {site.Lng}", 330);
        AppTheme.AddDisplayRow(grid, 3, "Açıklama", string.IsNullOrWhiteSpace(site.Description) ? "Yok" : site.Description, 330);

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

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 50,
            Padding = new Padding(0, 12, 0, 0),
            FlowDirection = FlowDirection.LeftToRight,
        };
        buttons.Controls.Add(btnMap);
        buttons.Controls.Add(btnClose);

        var body = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
        body.Controls.Add(grid);

        card.Controls.Add(buttons);
        card.Controls.Add(body);
        card.Controls.Add(title);
        outer.Controls.Add(card);
        Controls.Add(outer);
    }
}
