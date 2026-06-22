namespace MsgoktasMu.Forms;

internal static class UiFactory
{
    public static Button CreateButton(string text, bool primary = true, int width = 120, int height = 36)
    {
        var button = new Button
        {
            Text = text,
            Width = width,
            Height = height,
            Margin = new Padding(6, 4, 6, 4),
        };
        if (primary) AppTheme.StylePrimaryButton(button);
        else AppTheme.StyleSecondaryButton(button);
        return button;
    }

    public static void ShowError(string message) =>
        MessageBox.Show(message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);

    public static void ShowInfo(string message) =>
        MessageBox.Show(message, "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

    public static bool Confirm(string message) =>
        MessageBox.Show(message, "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
}

internal sealed class FileManagerControl : UserControl
{
    private readonly string _category;
    private readonly bool _allowManage;
    private readonly ListView _list = new() { Dock = DockStyle.Fill, View = View.Details, FullRowSelect = true, BorderStyle = BorderStyle.FixedSingle, Font = AppTheme.BodyFont };
    private readonly Button _btnAdd = UiFactory.CreateButton("Dosya Ekle", primary: false, width: 110);
    private readonly Button _btnOpen = UiFactory.CreateButton("Aç", primary: false, width: 80);
    private readonly Button _btnDelete = UiFactory.CreateButton("Sil", primary: false, width: 80);

    public FileManagerControl(string category, bool allowManage)
    {
        _category = category;
        _allowManage = allowManage;
        BackColor = AppTheme.Surface;
        Padding = new Padding(12);

        _list.Columns.Add("Dosya", 320);
        _list.Columns.Add("Tarih", 180);
        _list.BackColor = AppTheme.Surface;

        var top = new Panel
        {
            Dock = DockStyle.Top,
            Height = 48,
            Padding = new Padding(0, 0, 0, 8),
            BackColor = AppTheme.Surface,
        };
        var flow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false };
        flow.Controls.Add(_btnOpen);
        if (_allowManage)
        {
            flow.Controls.Add(_btnAdd);
            flow.Controls.Add(_btnDelete);
        }
        top.Controls.Add(flow);

        Controls.Add(_list);
        Controls.Add(top);

        _btnAdd.Click += (_, _) => AddFile();
        _btnOpen.Click += (_, _) => OpenSelected();
        _btnDelete.Click += (_, _) => DeleteSelected();
        _list.DoubleClick += (_, _) => OpenSelected();

        LoadFiles();
    }

    public void RefreshFiles() => LoadFiles();

    private void LoadFiles()
    {
        _list.Items.Clear();
        foreach (var file in Data.LocalDatabase.GetFiles(_category))
        {
            var item = new ListViewItem(file.OriginalName) { Tag = file };
            item.SubItems.Add(ParseDate(file.UploadDate).ToLocalTime().ToString("g"));
            _list.Items.Add(item);
        }
    }

    private static DateTime ParseDate(string value) =>
        DateTime.TryParse(value, out var dt) ? dt : DateTime.MinValue;

    private void AddFile()
    {
        using var dialog = new OpenFileDialog
        {
            Filter = "Desteklenen dosyalar|*.pdf;*.doc;*.docx;*.xlsx;*.xls;*.txt;*.png;*.jpg;*.jpeg",
        };
        if (dialog.ShowDialog() != DialogResult.OK)
            return;

        try
        {
            Data.LocalDatabase.AddFile(dialog.FileName, _category);
            LoadFiles();
        }
        catch (Exception ex)
        {
            UiFactory.ShowError(ex.Message);
        }
    }

    private void OpenSelected()
    {
        if (_list.SelectedItems.Count == 0)
            return;
        var file = (Models.FileRecord)_list.SelectedItems[0].Tag!;
        var path = Data.LocalDatabase.GetFilePath(file);
        if (!File.Exists(path))
        {
            UiFactory.ShowError("Dosya diskte bulunamadı.");
            return;
        }

        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(path) { UseShellExecute = true });
    }

    private void DeleteSelected()
    {
        if (_list.SelectedItems.Count == 0)
            return;
        if (!UiFactory.Confirm("Seçili dosyayı silmek istiyor musunuz?"))
            return;

        var file = (Models.FileRecord)_list.SelectedItems[0].Tag!;
        try
        {
            Data.LocalDatabase.DeleteFile(file.Id);
            LoadFiles();
        }
        catch (Exception ex)
        {
            UiFactory.ShowError(ex.Message);
        }
    }
}
