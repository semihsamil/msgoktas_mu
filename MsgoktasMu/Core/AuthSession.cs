namespace MsgoktasMu.Core;

internal static class AuthSession
{
    public static UserSession? Current { get; private set; }

    public static void Set(UserSession session) => Current = session;

    public static void Clear() => Current = null;

    public static bool IsLoggedIn => Current != null;

    public static bool IsAdmin => Current?.Role == "admin";
}

internal sealed class UserSession
{
    public int Id { get; init; }
    public string Username { get; init; } = "";
    public string Role { get; init; } = "";
    public string FullName { get; init; } = "";
}

internal static class ValidationHelper
{
    public static string? ValidateUsername(string username)
    {
        username = username.Trim();
        if (username.Length < 3 || username.Length > 32)
            return "Kullanıcı adı 3-32 karakter olmalı";
        if (!System.Text.RegularExpressions.Regex.IsMatch(username, @"^[a-zA-Z0-9._-]+$"))
            return "Kullanıcı adında sadece harf, rakam, . _ - kullanılabilir";
        return null;
    }

    public static string? ValidatePassword(string password)
    {
        if (password.Length < 6)
            return "Şifre en az 6 karakter olmalı";
        return null;
    }

    public static string? ValidatePhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return "Telefon zorunlu";
        if (!System.Text.RegularExpressions.Regex.IsMatch(phone.Trim(), @"^\+90 5\d{9}$"))
            return "Telefon formatı +90 5XXXXXXXXX olmalı";
        return null;
    }

    public static string? ValidatePhoneOptional(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return null;
        return ValidatePhone(phone);
    }

    public static string? ValidateSite(ConstructionSiteInput input, bool isCreate)
    {
        var name = input.Name.Trim();
        if (isCreate && string.IsNullOrEmpty(name))
            return "Şantiye adı zorunlu";
        if (!string.IsNullOrEmpty(name) && (name.Length < 2 || name.Length > 100))
            return "Şantiye adı 2-100 karakter olmalı";
        if (input.Address.Length > 200)
            return "Adres en fazla 200 karakter olabilir";
        if (input.Phone.Length > 30)
            return "Telefon en fazla 30 karakter olabilir";
        if (input.Description.Length > 500)
            return "Açıklama en fazla 500 karakter olabilir";
        if (!string.IsNullOrWhiteSpace(input.Lat) && !double.TryParse(input.Lat.Replace(',', '.'), out _))
            return "Geçerli enlem girin";
        if (!string.IsNullOrWhiteSpace(input.Lng) && !double.TryParse(input.Lng.Replace(',', '.'), out _))
            return "Geçerli boylam girin";
        return null;
    }
}

internal sealed class ConstructionSiteInput
{
    public string Name { get; set; } = "";
    public string Address { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Lat { get; set; } = "";
    public string Lng { get; set; } = "";
    public string Description { get; set; } = "";
}
