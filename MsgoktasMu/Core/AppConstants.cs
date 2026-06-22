namespace MsgoktasMu.Core;

internal static class AppConstants
{
    public const string AdminUsername = "admin";
    public const string AdminPassword = "admin123";

    public static readonly string[] ValidRoles = ["admin", "personel", "is_yapilan"];
    public static readonly string[] RegisterRoles = ["personel", "is_yapilan"];
    public static readonly string[] FileCategories = ["general", "reports", "notes", "schedule"];

    public static readonly string[] AllowedExtensions =
    [
        ".pdf", ".doc", ".docx", ".xlsx", ".xls", ".txt", ".png", ".jpg", ".jpeg"
    ];

    public const long MaxUploadBytes = 15 * 1024 * 1024;

    public static string RoleLabel(string role) => role switch
    {
        "admin" => "Admin",
        "personel" => "Personel",
        "is_yapilan" => "Kullanıcı",
        _ => role
    };

    public static string CategoryTitle(string category) => category switch
    {
        "general" => "Genel Şantiye Bilgileri",
        "reports" => "Günlük Raporlar",
        "notes" => "Bilgi Notları",
        "schedule" => "Şantiye Çizelgesi",
        _ => category
    };
}
