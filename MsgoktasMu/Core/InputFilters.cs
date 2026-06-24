using System.Text.RegularExpressions;

namespace MsgoktasMu.Core;

internal static class InputFilters
{
    public const string MobilePhonePrefix = "+90 5";
    private const string MobilePhoneTag = "input:mobile-phone";
    private const string TextNameTag = "input:text-name";

    public static void AttachMobilePhone(TextBox box, bool optional = false)
    {
        if (box.Tag as string == MobilePhoneTag)
            return;

        box.Tag = MobilePhoneTag;
        box.MaxLength = 14;
        if (string.IsNullOrWhiteSpace(box.Text) || box.Text.Trim() == MobilePhonePrefix)
            box.Text = MobilePhonePrefix;
        else
            box.Text = ValidationHelper.ToMobilePhoneFieldValue(box.Text);

        box.TextChanged += (_, _) => ApplyMobilePhone(box);
        box.KeyPress += (_, e) =>
        {
            if (char.IsControl(e.KeyChar))
                return;
            if (!char.IsDigit(e.KeyChar))
                e.Handled = true;
        };
        box.Leave += (_, _) =>
        {
            ApplyMobilePhone(box);
            if (!optional && box.Text == MobilePhonePrefix)
                box.Text = MobilePhonePrefix;
        };
    }

    public static void AttachTextName(TextBox box)
    {
        if (box.Tag as string == TextNameTag)
            return;

        box.Tag = TextNameTag;
        box.TextChanged += (_, _) => ApplyTextName(box);
        box.KeyPress += (_, e) =>
        {
            if (char.IsControl(e.KeyChar))
                return;
            if (char.IsDigit(e.KeyChar))
                e.Handled = true;
        };
    }

    public static void ResetMobilePhone(TextBox box) => box.Text = MobilePhonePrefix;

    public static string ReadMobilePhone(TextBox box) => ValidationHelper.MobilePhoneForSave(box.Text);

    private static void ApplyMobilePhone(TextBox box)
    {
        if (box.Tag as string != MobilePhoneTag)
            return;

        var normalized = ValidationHelper.NormalizeMobilePhone(box.Text);
        if (box.Text == normalized)
        {
            if (box.SelectionStart < MobilePhonePrefix.Length)
                box.SelectionStart = MobilePhonePrefix.Length;
            return;
        }

        var cursorFromEnd = box.Text.Length - box.SelectionStart;
        box.Text = normalized;
        box.SelectionStart = Math.Clamp(normalized.Length - cursorFromEnd, MobilePhonePrefix.Length, normalized.Length);
    }

    private static void ApplyTextName(TextBox box)
    {
        if (box.Tag as string != TextNameTag)
            return;

        var cleaned = ValidationHelper.StripDigits(box.Text);
        if (cleaned == box.Text)
            return;

        var cursor = box.SelectionStart;
        box.Text = cleaned;
        box.SelectionStart = Math.Clamp(cursor - 1, 0, box.Text.Length);
    }
}
