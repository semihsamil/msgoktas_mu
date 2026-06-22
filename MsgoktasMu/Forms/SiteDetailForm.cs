using MsgoktasMu.Models;

namespace MsgoktasMu.Forms;

internal sealed class SiteDetailForm : Form
{
    public SiteDetailForm(ConstructionSite site)
    {
        Text = site.Name + " | Şantiye Detayı";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        ClientSize = new Size(520, 420);

        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20), AutoScroll = true };
        var text = new Label
        {
            Dock = DockStyle.Fill,
            Text = $"""
                Şantiye adı: {site.Name}
                Adres: {(string.IsNullOrWhiteSpace(site.Address) ? "Belirtilmemiş" : site.Address)}
                Telefon: {(string.IsNullOrWhiteSpace(site.Phone) ? "Belirtilmemiş" : site.Phone)}
                Enlem: {(string.IsNullOrWhiteSpace(site.Lat) ? "-" : site.Lat)}
                Boylam: {(string.IsNullOrWhiteSpace(site.Lng) ? "-" : site.Lng)}

                Açıklama:
                {(string.IsNullOrWhiteSpace(site.Description) ? "Yok" : site.Description)}
                """,
        };

        var btnMap = UiFactory.CreateButton("Haritada Aç", 120);
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
        var btnClose = UiFactory.CreateButton("Kapat", 100);
        btnClose.Click += (_, _) => Close();
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 44 };
        buttons.Controls.Add(btnMap);
        buttons.Controls.Add(btnClose);

        panel.Controls.Add(text);
        panel.Controls.Add(buttons);
        Controls.Add(panel);
    }
}
