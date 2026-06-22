namespace MsgoktasMu.Core;

internal static class AppPaths
{
    public static string Root => AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

    public static string DatabaseFile => Path.Combine(Root, "msgoktas_mu.db");

    public static string UploadsDirectory => Path.Combine(Root, "uploads");

    public static void EnsureDirectories()
    {
        Directory.CreateDirectory(UploadsDirectory);
    }
}
