namespace MsgoktasMu.Core;

using System.Text.RegularExpressions;

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
        phone = phone.Trim();
        if (string.IsNullOrWhiteSpace(phone) || phone == InputFilters.MobilePhonePrefix)
            return "Telefon zorunlu";
        if (!System.Text.RegularExpressions.Regex.IsMatch(phone, @"^\+90 5\d{9}$"))
            return "Telefon formatı +90 5XXXXXXXXX olmalı";
        return null;
    }

    public static string? ValidatePhoneOptional(string phone)
    {
        phone = phone.Trim();
        if (string.IsNullOrWhiteSpace(phone) || phone == InputFilters.MobilePhonePrefix)
            return null;
        return ValidatePhone(phone);
    }

    public static string NormalizeMobilePhone(string? raw)
    {
        const string prefix = InputFilters.MobilePhonePrefix;
        var digitsOnly = Regex.Replace(raw ?? "", @"\D", "");
        string rest;
        if (digitsOnly.StartsWith("905", StringComparison.Ordinal))
            rest = digitsOnly[3..];
        else if (digitsOnly.StartsWith('5'))
            rest = digitsOnly[1..];
        else
            rest = digitsOnly;

        if (rest.Length > 9)
            rest = rest[..9];
        return prefix + rest;
    }

    public static string MobilePhoneForSave(string? raw)
    {
        var normalized = NormalizeMobilePhone(raw);
        return normalized == InputFilters.MobilePhonePrefix ? "" : normalized;
    }

    public static string ToMobilePhoneFieldValue(string? stored)
    {
        if (string.IsNullOrWhiteSpace(stored))
            return InputFilters.MobilePhonePrefix;
        var trimmed = stored.Trim();
        if (Regex.IsMatch(trimmed, @"^\+90 5\d{9}$"))
            return trimmed;
        return NormalizeMobilePhone(trimmed);
    }

    public static string StripDigits(string value) => Regex.Replace(value, @"\d", "");

    public static string? ValidateSite(ConstructionSiteInput input, bool isCreate)
    {
        var name = input.Name.Trim();
        if (isCreate && string.IsNullOrEmpty(name))
            return "Şantiye adı zorunlu";
        if (!string.IsNullOrEmpty(name) && (name.Length < 2 || name.Length > 100))
            return "Şantiye adı 2-100 karakter olmalı";
        var nameTextErr = ValidateTextNameOptional(name, "Şantiye adı");
        if (nameTextErr != null)
            return nameTextErr;
        if (input.Address.Length > 200)
            return "Adres en fazla 200 karakter olabilir";
        var phoneErr = ValidatePhoneOptional(input.Phone);
        if (phoneErr != null)
            return phoneErr;
        if (input.Phone.Length > 30)
            return "Telefon en fazla 30 karakter olabilir";
        if (input.Description.Length > 500)
            return "Açıklama en fazla 500 karakter olabilir";
        if (!string.IsNullOrWhiteSpace(input.Lat) && !double.TryParse(input.Lat.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out _))
            return "Enlem sayı olmalı (ör. 39.7477)";
        if (!string.IsNullOrWhiteSpace(input.Lng) && !double.TryParse(input.Lng.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out _))
            return "Boylam sayı olmalı (ör. 37.0179)";
        return null;
    }

    public static string? ValidateFullNameOptional(string fullName)
    {
        return ValidateTextNameOptional(fullName, "Ad soyad");
    }

    public static string? ValidateTextNameOptional(string value, string fieldLabel)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        value = value.Trim();
        if (value.Length < 2 || value.Length > 80)
            return $"{fieldLabel} 2-80 karakter olmalı";
        if (Regex.IsMatch(value, @"\d"))
            return $"{fieldLabel} alanına rakam yazılamaz";
        return null;
    }

    public static string? ValidateSettings(Models.AppSettings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.ContactEmail))
            return "E-posta zorunlu";
        if (!settings.ContactEmail.Contains('@') || settings.ContactEmail.Length > 120)
            return "Geçerli bir e-posta adresi girin";
        if (string.IsNullOrWhiteSpace(settings.ContactPhone))
            return "Telefon zorunlu";
        var phoneErr = ValidatePhone(settings.ContactPhone);
        if (phoneErr != null)
            return phoneErr;
        if (settings.ContactAddress.Length > 200)
            return "Adres en fazla 200 karakter olabilir";
        if (string.IsNullOrWhiteSpace(settings.MapLat) || !double.TryParse(settings.MapLat.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out _))
            return "Harita enlemi sayı olmalı";
        if (string.IsNullOrWhiteSpace(settings.MapLng) || !double.TryParse(settings.MapLng.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out _))
            return "Harita boylamı sayı olmalı";
        if (settings.MapLabel.Length > 100)
            return "Harita etiketi en fazla 100 karakter olabilir";
        return null;
    }

    public static string NormalizeRole(string role)
    {
        return role.Trim() switch
        {
            "Admin" => "admin",
            "Personel" => "personel",
            "Kullanıcı" => "is_yapilan",
            _ => role.Trim(),
        };
    }

    public static string? ValidateRole(string role)
    {
        var normalized = NormalizeRole(role);
        if (!AppConstants.ValidRoles.Contains(normalized))
            return "Geçersiz rol seçimi";
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
