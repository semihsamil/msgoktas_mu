namespace MsgoktasMu.Forms;

internal static class AppTheme
{
    public static readonly Color Background = Color.FromArgb(246, 241, 235);
    public static readonly Color Surface = Color.White;
    public static readonly Color SurfaceMuted = Color.FromArgb(235, 228, 219);
    public static readonly Color Text = Color.FromArgb(26, 23, 20);
    public static readonly Color TextMuted = Color.FromArgb(92, 83, 76);
    public static readonly Color Accent = Color.FromArgb(139, 105, 20);
    public static readonly Color AccentHover = Color.FromArgb(111, 84, 16);
    public static readonly Color Border = Color.FromArgb(221, 212, 200);
    public static readonly Color Error = Color.FromArgb(168, 45, 45);
    public static readonly Color Success = Color.FromArgb(46, 125, 70);

    public static readonly Font TitleFont = new("Georgia", 16F, FontStyle.Bold);
    public static readonly Font SectionFont = new("Segoe UI", 13F, FontStyle.Bold);
    public static readonly Font BodyFont = new("Segoe UI", 10F, FontStyle.Regular);
    public static readonly Font LabelFont = new("Segoe UI", 9.5F, FontStyle.Bold);
    public static readonly Font HintFont = new("Segoe UI", 8.5F, FontStyle.Italic);

    public static void ApplyToForm(Form form)
    {
        form.BackColor = Background;
        form.Font = BodyFont;
        form.ForeColor = Text;
    }

    public static void StylePrimaryButton(Button button)
    {
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 0;
        button.BackColor = Accent;
        button.ForeColor = Color.White;
        button.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        button.Cursor = Cursors.Hand;
        button.Padding = new Padding(8, 4, 8, 4);
        button.FlatAppearance.MouseOverBackColor = AccentHover;
    }

    public static void StyleSecondaryButton(Button button)
    {
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderColor = Border;
        button.FlatAppearance.BorderSize = 1;
        button.BackColor = Surface;
        button.ForeColor = Text;
        button.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
        button.Cursor = Cursors.Hand;
        button.Padding = new Padding(8, 4, 8, 4);
        button.FlatAppearance.MouseOverBackColor = SurfaceMuted;
    }

    public static void StyleNavButton(Button button)
    {
        StyleSecondaryButton(button);
        button.Height = 34;
        button.Margin = new Padding(3, 0, 3, 0);
    }

    public static GroupBox CreateGroupBox(string title)
    {
        return new GroupBox
        {
            Text = "  " + title + "  ",
            ForeColor = Accent,
            Font = LabelFont,
            BackColor = Surface,
            Padding = new Padding(14, 18, 14, 12),
            Margin = new Padding(0, 0, 0, 14),
        };
    }

    public static Panel CreateCard(int width, int height)
    {
        return new Panel
        {
            Width = width,
            Height = height,
            BackColor = Surface,
            Margin = new Padding(8, 8, 16, 16),
            Padding = new Padding(14),
            BorderStyle = BorderStyle.FixedSingle,
            Cursor = Cursors.Hand,
        };
    }

    public static TextBox CreateMultilineInput(int height = 64)
    {
        return new TextBox
        {
            Font = BodyFont,
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Surface,
            ForeColor = Text,
            Multiline = true,
            Height = height,
            Dock = DockStyle.Top,
            ScrollBars = ScrollBars.Vertical,
            Margin = new Padding(0, 0, 0, 4),
        };
    }

    public static TextBox CreateInput(bool password = false, string placeholder = "")
    {
        var box = new TextBox
        {
            Font = BodyFont,
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Surface,
            ForeColor = Text,
            Height = 30,
            Dock = DockStyle.Top,
            Margin = new Padding(0, 0, 0, 4),
        };
        if (password) box.UseSystemPasswordChar = true;
        if (!string.IsNullOrEmpty(placeholder)) box.PlaceholderText = placeholder;
        return box;
    }

    public static Label CreateFieldLabel(string text)
    {
        return new Label
        {
            Text = text,
            Font = LabelFont,
            ForeColor = Text,
            AutoSize = true,
            Anchor = AnchorStyles.Top | AnchorStyles.Left,
            Margin = new Padding(0, 10, 16, 10),
            MaximumSize = new Size(132, 0),
        };
    }

    public static Label CreateValueLabel(string text)
    {
        return new Label
        {
            Text = text,
            Font = BodyFont,
            ForeColor = Text,
            AutoSize = true,
            Anchor = AnchorStyles.Top | AnchorStyles.Left,
            Margin = new Padding(0, 10, 0, 10),
        };
    }

    public static Label CreateHintLabel(string text)
    {
        return new Label
        {
            Text = text,
            Font = HintFont,
            ForeColor = TextMuted,
            AutoSize = true,
            Dock = DockStyle.Top,
            Margin = new Padding(0, 0, 0, 10),
        };
    }

    public static TableLayoutPanel CreateFormGrid(int rowCount)
    {
        var grid = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 2,
            RowCount = rowCount,
            BackColor = Color.Transparent,
        };
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 145));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        for (var i = 0; i < rowCount; i++)
            grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        return grid;
    }

    public static void AddDisplayRow(TableLayoutPanel grid, int row, string labelText, string value, int valueMaxWidth = 320)
    {
        grid.Controls.Add(CreateFieldLabel(labelText), 0, row);
        var valueLabel = CreateValueLabel(value);
        valueLabel.MaximumSize = new Size(valueMaxWidth, 0);
        grid.Controls.Add(valueLabel, 1, row);
    }

    public static void AddFormRow(TableLayoutPanel grid, int row, string labelText, Control input, string? hint = null)
    {
        grid.Controls.Add(CreateFieldLabel(labelText), 0, row);

        if (string.IsNullOrEmpty(hint))
        {
            input.Dock = DockStyle.None;
            input.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            input.Margin = new Padding(0, 6, 0, 12);
            input.MinimumSize = new Size(200, 30);
            grid.Controls.Add(input, 1, row);
            return;
        }

        var wrap = new Panel { AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
        input.Dock = DockStyle.None;
        input.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        input.Margin = new Padding(0, 6, 0, 0);
        input.MinimumSize = new Size(200, 30);
        wrap.Controls.Add(input);
        var hintLabel = CreateHintLabel(hint);
        hintLabel.Dock = DockStyle.None;
        hintLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
        hintLabel.Margin = new Padding(0, 4, 0, 8);
        wrap.Controls.Add(hintLabel);
        grid.Controls.Add(wrap, 1, row);
    }
}
