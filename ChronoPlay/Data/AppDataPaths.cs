using System.IO;

namespace ChronoPlay.Data;

public static class AppDataPaths
{
    public static string RootDirectory => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "ChronoPlay");

    public static string DatabaseDirectory => RootDirectory;

    public static string DatabaseFile => Path.Combine(DatabaseDirectory, "ChronoPlay.db");

    public static string BackupsDirectory => Path.Combine(RootDirectory, "Backups");

    public static string LogsDirectory => Path.Combine(RootDirectory, "Logs");

    public static void EnsureDirectories()
    {
        Directory.CreateDirectory(DatabaseDirectory);
        Directory.CreateDirectory(BackupsDirectory);
        Directory.CreateDirectory(LogsDirectory);
    }
}
