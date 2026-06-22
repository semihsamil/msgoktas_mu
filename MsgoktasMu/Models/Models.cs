namespace MsgoktasMu.Models;

internal sealed class UserRecord
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string Role { get; set; } = "personel";
    public string FullName { get; set; } = "";
    public string Phone { get; set; } = "";
    public string SiteName { get; set; } = "";
    public string CompanyName { get; set; } = "";
    public string ExtraNote { get; set; } = "";
    public string CreatedAt { get; set; } = "";
}

internal sealed class FileRecord
{
    public int Id { get; set; }
    public string Filename { get; set; } = "";
    public string OriginalName { get; set; } = "";
    public string UploadDate { get; set; } = "";
    public string Category { get; set; } = "reports";
}

internal sealed class ConstructionSite
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Address { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Lat { get; set; } = "";
    public string Lng { get; set; } = "";
    public string Description { get; set; } = "";
    public string CreatedAt { get; set; } = "";
}

internal sealed class AppSettings
{
    public string ContactEmail { get; set; } = "info@mimarsinangoktas.com";
    public string ContactPhone { get; set; } = "+90 346 000 00 00";
    public string ContactAddress { get; set; } = "Sivas, Türkiye";
    public string MapLat { get; set; } = "39.7477";
    public string MapLng { get; set; } = "37.0179";
    public string MapLabel { get; set; } = "Mimar Sinan Göktaş — Sivas";
}
