using Microsoft.Data.Sqlite;
using MsgoktasMu.Core;
using MsgoktasMu.Models;

namespace MsgoktasMu.Data;

internal static class LocalDatabase
{
    private static readonly Dictionary<string, string> DefaultSettings = new()
    {
        ["contact_email"] = "info@mimarsinangoktas.com",
        ["contact_phone"] = "+90 346 000 00 00",
        ["contact_address"] = "Sivas, Türkiye",
        ["map_lat"] = "39.7477",
        ["map_lng"] = "37.0179",
        ["map_label"] = "Mimar Sinan Göktaş — Sivas",
    };

    public static void Initialize()
    {
        AppPaths.EnsureDirectories();
        using var conn = OpenConnection();
        ExecuteNonQuery(conn, """
            CREATE TABLE IF NOT EXISTS files (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                filename TEXT,
                originalname TEXT,
                upload_date TEXT,
                category TEXT DEFAULT 'reports'
            )
            """);
        ExecuteNonQuery(conn, """
            CREATE TABLE IF NOT EXISTS settings (
                key TEXT PRIMARY KEY,
                value TEXT
            )
            """);
        ExecuteNonQuery(conn, """
            CREATE TABLE IF NOT EXISTS users (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                username TEXT UNIQUE,
                password TEXT,
                role TEXT DEFAULT 'personel',
                full_name TEXT,
                phone TEXT,
                site_name TEXT,
                company_name TEXT,
                extra_note TEXT,
                created_at TEXT
            )
            """);
        ExecuteNonQuery(conn, """
            CREATE TABLE IF NOT EXISTS construction_sites (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                name TEXT NOT NULL,
                address TEXT,
                phone TEXT,
                lat TEXT,
                lng TEXT,
                description TEXT,
                created_at TEXT
            )
            """);

        foreach (var pair in DefaultSettings)
        {
            ExecuteNonQuery(conn, "INSERT OR IGNORE INTO settings (key, value) VALUES (@k, @v)",
                new SqliteParameter("@k", pair.Key), new SqliteParameter("@v", pair.Value));
        }

        EnsureAdminUser(conn);
    }

    private static void EnsureAdminUser(SqliteConnection conn)
    {
        using var check = conn.CreateCommand();
        check.CommandText = "SELECT COUNT(*) FROM users WHERE username = @u";
        check.Parameters.AddWithValue("@u", AppConstants.AdminUsername);
        var count = Convert.ToInt64(check.ExecuteScalar());
        if (count > 0)
        {
            using var update = conn.CreateCommand();
            update.CommandText = """
                UPDATE users SET password=@p, role='admin', full_name='Sistem Yöneticisi'
                WHERE username=@u
                """;
            update.Parameters.AddWithValue("@p", AppConstants.AdminPassword);
            update.Parameters.AddWithValue("@u", AppConstants.AdminUsername);
            update.ExecuteNonQuery();
            return;
        }

        using var insert = conn.CreateCommand();
        insert.CommandText = """
            INSERT INTO users (username, password, role, full_name, created_at)
            VALUES (@u, @p, 'admin', 'Sistem Yöneticisi', @c)
            """;
        insert.Parameters.AddWithValue("@u", AppConstants.AdminUsername);
        insert.Parameters.AddWithValue("@p", AppConstants.AdminPassword);
        insert.Parameters.AddWithValue("@c", DateTime.UtcNow.ToString("o"));
        insert.ExecuteNonQuery();
    }

    public static UserRecord? Login(string username, string password)
    {
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT id, username, password, role, full_name, phone, site_name, company_name, extra_note, created_at
            FROM users WHERE username=@u AND password=@p
            """;
        cmd.Parameters.AddWithValue("@u", username.Trim());
        cmd.Parameters.AddWithValue("@p", password);
        using var reader = cmd.ExecuteReader();
        return reader.Read() ? ReadUser(reader) : null;
    }

    public static void Register(UserRecord user)
    {
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            INSERT INTO users (username, password, role, full_name, phone, site_name, company_name, extra_note, created_at)
            VALUES (@u, @p, @r, @fn, @ph, @sn, @cn, '', @c)
            """;
        cmd.Parameters.AddWithValue("@u", user.Username);
        cmd.Parameters.AddWithValue("@p", user.Password);
        cmd.Parameters.AddWithValue("@r", user.Role);
        cmd.Parameters.AddWithValue("@fn", user.FullName);
        cmd.Parameters.AddWithValue("@ph", user.Phone);
        cmd.Parameters.AddWithValue("@sn", user.SiteName);
        cmd.Parameters.AddWithValue("@cn", user.CompanyName);
        cmd.Parameters.AddWithValue("@c", DateTime.UtcNow.ToString("o"));
        try
        {
            cmd.ExecuteNonQuery();
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
        {
            throw new InvalidOperationException("Bu kullanıcı adı zaten kayıtlı");
        }
    }

    public static List<UserRecord> GetUsers()
    {
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT id, username, password, role, full_name, phone, site_name, company_name, extra_note, created_at
            FROM users ORDER BY datetime(created_at) DESC
            """;
        return ReadUsers(cmd);
    }

    public static void UpdateUser(UserRecord user)
    {
        using var conn = OpenConnection();
        var existing = GetUserById(conn, user.Id) ?? throw new InvalidOperationException("Kullanıcı bulunamadı");

        if (existing.Role == "admin" && user.Role != "admin" && CountAdmins(conn) <= 1)
            throw new InvalidOperationException("Son admin kullanıcısının rolü değiştirilemez");

        if (!string.Equals(existing.Username, user.Username, StringComparison.Ordinal))
        {
            if (UsernameExists(conn, user.Username, user.Id))
                throw new InvalidOperationException("Bu kullanıcı adı zaten kayıtlı");
        }

        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            UPDATE users SET username=@u, password=@p, role=@r, full_name=@fn, phone=@ph,
                site_name=@sn, company_name=@cn, extra_note=@en
            WHERE id=@id
            """;
        cmd.Parameters.AddWithValue("@u", user.Username.Trim());
        cmd.Parameters.AddWithValue("@p", user.Password);
        cmd.Parameters.AddWithValue("@r", user.Role);
        cmd.Parameters.AddWithValue("@fn", user.FullName);
        cmd.Parameters.AddWithValue("@ph", user.Phone);
        cmd.Parameters.AddWithValue("@sn", user.SiteName);
        cmd.Parameters.AddWithValue("@cn", user.CompanyName);
        cmd.Parameters.AddWithValue("@en", user.ExtraNote);
        cmd.Parameters.AddWithValue("@id", user.Id);
        cmd.ExecuteNonQuery();
    }

    public static void DeleteUser(int id, int currentUserId)
    {
        if (id == currentUserId)
            throw new InvalidOperationException("Oturum açtığınız kullanıcı silinemez");

        using var conn = OpenConnection();
        var user = GetUserById(conn, id) ?? throw new InvalidOperationException("Kullanıcı bulunamadı");
        if (user.Role == "admin" && CountAdmins(conn) <= 1)
            throw new InvalidOperationException("Son admin kullanıcısı silinemez");

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM users WHERE id=@id";
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    public static List<ConstructionSite> GetSites()
    {
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT * FROM construction_sites ORDER BY name COLLATE NOCASE";
        var list = new List<ConstructionSite>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(ReadSite(reader));
        return list;
    }

    public static ConstructionSite? GetSite(int id)
    {
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT * FROM construction_sites WHERE id=@id";
        cmd.Parameters.AddWithValue("@id", id);
        using var reader = cmd.ExecuteReader();
        return reader.Read() ? ReadSite(reader) : null;
    }

    public static int AddSite(ConstructionSiteInput input)
    {
        var err = ValidationHelper.ValidateSite(input, true);
        if (err != null) throw new InvalidOperationException(err);

        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            INSERT INTO construction_sites (name, address, phone, lat, lng, description, created_at)
            VALUES (@n, @a, @p, @lat, @lng, @d, @c)
            """;
        BindSite(cmd, input);
        cmd.ExecuteNonQuery();
        using var idCmd = conn.CreateCommand();
        idCmd.CommandText = "SELECT last_insert_rowid()";
        return Convert.ToInt32(idCmd.ExecuteScalar());
    }

    public static void UpdateSite(int id, ConstructionSiteInput input)
    {
        var err = ValidationHelper.ValidateSite(input, false);
        if (err != null) throw new InvalidOperationException(err);
        if (string.IsNullOrWhiteSpace(input.Name))
            throw new InvalidOperationException("Şantiye adı zorunlu");

        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            UPDATE construction_sites SET name=@n, address=@a, phone=@p, lat=@lat, lng=@lng, description=@d
            WHERE id=@id
            """;
        BindSite(cmd, input);
        cmd.Parameters.AddWithValue("@id", id);
        if (cmd.ExecuteNonQuery() == 0)
            throw new InvalidOperationException("Şantiye bulunamadı");
    }

    public static void DeleteSite(int id)
    {
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM construction_sites WHERE id=@id";
        cmd.Parameters.AddWithValue("@id", id);
        if (cmd.ExecuteNonQuery() == 0)
            throw new InvalidOperationException("Şantiye bulunamadı");
    }

    public static AppSettings GetSettings()
    {
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT key, value FROM settings";
        var map = new Dictionary<string, string>(StringComparer.Ordinal);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            map[reader.GetString(0)] = reader.IsDBNull(1) ? "" : reader.GetString(1);

        return new AppSettings
        {
            ContactEmail = map.GetValueOrDefault("contact_email", DefaultSettings["contact_email"]),
            ContactPhone = map.GetValueOrDefault("contact_phone", DefaultSettings["contact_phone"]),
            ContactAddress = map.GetValueOrDefault("contact_address", DefaultSettings["contact_address"]),
            MapLat = map.GetValueOrDefault("map_lat", DefaultSettings["map_lat"]),
            MapLng = map.GetValueOrDefault("map_lng", DefaultSettings["map_lng"]),
            MapLabel = map.GetValueOrDefault("map_label", DefaultSettings["map_label"]),
        };
    }

    public static void SaveSettings(AppSettings settings)
    {
        using var conn = OpenConnection();
        SaveSetting(conn, "contact_email", settings.ContactEmail);
        SaveSetting(conn, "contact_phone", settings.ContactPhone);
        SaveSetting(conn, "contact_address", settings.ContactAddress);
        SaveSetting(conn, "map_lat", settings.MapLat);
        SaveSetting(conn, "map_lng", settings.MapLng);
        SaveSetting(conn, "map_label", settings.MapLabel);
    }

    public static List<FileRecord> GetFiles(string? category = null)
    {
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        if (string.IsNullOrEmpty(category))
            cmd.CommandText = "SELECT * FROM files ORDER BY datetime(upload_date) DESC";
        else
        {
            cmd.CommandText = "SELECT * FROM files WHERE category=@c ORDER BY datetime(upload_date) DESC";
            cmd.Parameters.AddWithValue("@c", category);
        }

        var list = new List<FileRecord>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(ReadFile(reader));
        return list;
    }

    public static FileRecord AddFile(string sourcePath, string category)
    {
        if (!AppConstants.FileCategories.Contains(category))
            throw new InvalidOperationException("Geçersiz dosya kategorisi");

        var ext = Path.GetExtension(sourcePath).ToLowerInvariant();
        if (!AppConstants.AllowedExtensions.Contains(ext))
            throw new InvalidOperationException("Bu dosya türü desteklenmiyor");

        var info = new FileInfo(sourcePath);
        if (info.Length > AppConstants.MaxUploadBytes)
            throw new InvalidOperationException("Dosya en fazla 15 MB olabilir");

        var storedName = $"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}-{SanitizeFileName(info.Name)}";
        var destPath = Path.Combine(AppPaths.UploadsDirectory, storedName);
        File.Copy(sourcePath, destPath, overwrite: true);

        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            INSERT INTO files (filename, originalname, upload_date, category)
            VALUES (@f, @o, @d, @c)
            """;
        cmd.Parameters.AddWithValue("@f", storedName);
        cmd.Parameters.AddWithValue("@o", info.Name);
        cmd.Parameters.AddWithValue("@d", DateTime.UtcNow.ToString("o"));
        cmd.Parameters.AddWithValue("@c", category);
        cmd.ExecuteNonQuery();

        using var idCmd = conn.CreateCommand();
        idCmd.CommandText = "SELECT last_insert_rowid()";
        var id = Convert.ToInt32(idCmd.ExecuteScalar());
        return new FileRecord
        {
            Id = id,
            Filename = storedName,
            OriginalName = info.Name,
            UploadDate = DateTime.UtcNow.ToString("o"),
            Category = category
        };
    }

    public static void DeleteFile(int id)
    {
        using var conn = OpenConnection();
        using var get = conn.CreateCommand();
        get.CommandText = "SELECT filename FROM files WHERE id=@id";
        get.Parameters.AddWithValue("@id", id);
        var filename = get.ExecuteScalar() as string;
        if (filename == null)
            throw new InvalidOperationException("Dosya bulunamadı");

        using var del = conn.CreateCommand();
        del.CommandText = "DELETE FROM files WHERE id=@id";
        del.Parameters.AddWithValue("@id", id);
        del.ExecuteNonQuery();

        var path = Path.Combine(AppPaths.UploadsDirectory, filename);
        if (File.Exists(path))
            File.Delete(path);
    }

    public static string GetFilePath(FileRecord file) =>
        Path.Combine(AppPaths.UploadsDirectory, file.Filename);

    private static SqliteConnection OpenConnection()
    {
        var conn = new SqliteConnection($"Data Source={AppPaths.DatabaseFile}");
        conn.Open();
        return conn;
    }

    private static void ExecuteNonQuery(SqliteConnection conn, string sql, params SqliteParameter[] parameters)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        cmd.Parameters.AddRange(parameters);
        cmd.ExecuteNonQuery();
    }

    private static void SaveSetting(SqliteConnection conn, string key, string value)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            INSERT INTO settings (key, value) VALUES (@k, @v)
            ON CONFLICT(key) DO UPDATE SET value=excluded.value
            """;
        cmd.Parameters.AddWithValue("@k", key);
        cmd.Parameters.AddWithValue("@v", value);
        cmd.ExecuteNonQuery();
    }

    private static UserRecord? GetUserById(SqliteConnection conn, int id)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT id, username, password, role, full_name, phone, site_name, company_name, extra_note, created_at
            FROM users WHERE id=@id
            """;
        cmd.Parameters.AddWithValue("@id", id);
        using var reader = cmd.ExecuteReader();
        return reader.Read() ? ReadUser(reader) : null;
    }

    private static bool UsernameExists(SqliteConnection conn, string username, int excludeId)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT id FROM users WHERE username=@u AND id<>@id";
        cmd.Parameters.AddWithValue("@u", username.Trim());
        cmd.Parameters.AddWithValue("@id", excludeId);
        return cmd.ExecuteScalar() != null;
    }

    private static int CountAdmins(SqliteConnection conn)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM users WHERE role='admin'";
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    private static List<UserRecord> ReadUsers(SqliteCommand cmd)
    {
        var list = new List<UserRecord>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(ReadUser(reader));
        return list;
    }

    private static UserRecord ReadUser(SqliteDataReader reader) => new()
    {
        Id = reader.GetInt32(reader.GetOrdinal("id")),
        Username = reader.GetString(reader.GetOrdinal("username")),
        Password = reader.IsDBNull(reader.GetOrdinal("password")) ? "" : reader.GetString(reader.GetOrdinal("password")),
        Role = reader.IsDBNull(reader.GetOrdinal("role")) ? "personel" : reader.GetString(reader.GetOrdinal("role")),
        FullName = reader.IsDBNull(reader.GetOrdinal("full_name")) ? "" : reader.GetString(reader.GetOrdinal("full_name")),
        Phone = reader.IsDBNull(reader.GetOrdinal("phone")) ? "" : reader.GetString(reader.GetOrdinal("phone")),
        SiteName = reader.IsDBNull(reader.GetOrdinal("site_name")) ? "" : reader.GetString(reader.GetOrdinal("site_name")),
        CompanyName = reader.IsDBNull(reader.GetOrdinal("company_name")) ? "" : reader.GetString(reader.GetOrdinal("company_name")),
        ExtraNote = reader.IsDBNull(reader.GetOrdinal("extra_note")) ? "" : reader.GetString(reader.GetOrdinal("extra_note")),
        CreatedAt = reader.IsDBNull(reader.GetOrdinal("created_at")) ? "" : reader.GetString(reader.GetOrdinal("created_at")),
    };

    private static ConstructionSite ReadSite(SqliteDataReader reader) => new()
    {
        Id = reader.GetInt32(reader.GetOrdinal("id")),
        Name = reader.GetString(reader.GetOrdinal("name")),
        Address = reader.IsDBNull(reader.GetOrdinal("address")) ? "" : reader.GetString(reader.GetOrdinal("address")),
        Phone = reader.IsDBNull(reader.GetOrdinal("phone")) ? "" : reader.GetString(reader.GetOrdinal("phone")),
        Lat = reader.IsDBNull(reader.GetOrdinal("lat")) ? "" : reader.GetString(reader.GetOrdinal("lat")),
        Lng = reader.IsDBNull(reader.GetOrdinal("lng")) ? "" : reader.GetString(reader.GetOrdinal("lng")),
        Description = reader.IsDBNull(reader.GetOrdinal("description")) ? "" : reader.GetString(reader.GetOrdinal("description")),
        CreatedAt = reader.IsDBNull(reader.GetOrdinal("created_at")) ? "" : reader.GetString(reader.GetOrdinal("created_at")),
    };

    private static FileRecord ReadFile(SqliteDataReader reader) => new()
    {
        Id = reader.GetInt32(reader.GetOrdinal("id")),
        Filename = reader.GetString(reader.GetOrdinal("filename")),
        OriginalName = reader.GetString(reader.GetOrdinal("originalname")),
        UploadDate = reader.GetString(reader.GetOrdinal("upload_date")),
        Category = reader.IsDBNull(reader.GetOrdinal("category")) ? "reports" : reader.GetString(reader.GetOrdinal("category")),
    };

    private static void BindSite(SqliteCommand cmd, ConstructionSiteInput input)
    {
        cmd.Parameters.AddWithValue("@n", input.Name.Trim());
        cmd.Parameters.AddWithValue("@a", input.Address.Trim());
        cmd.Parameters.AddWithValue("@p", input.Phone.Trim());
        cmd.Parameters.AddWithValue("@lat", input.Lat.Trim().Replace(',', '.'));
        cmd.Parameters.AddWithValue("@lng", input.Lng.Trim().Replace(',', '.'));
        cmd.Parameters.AddWithValue("@d", input.Description.Trim());
        cmd.Parameters.AddWithValue("@c", DateTime.UtcNow.ToString("o"));
    }

    private static string SanitizeFileName(string name)
    {
        foreach (var c in Path.GetInvalidFileNameChars())
            name = name.Replace(c, '_');
        return name;
    }
}
