using System.IO;
using ChronoPlay.Data;

namespace ChronoPlay.Services;

public static class StartupLog
{
    public static void Write(Exception exception)
    {
        AppDataPaths.EnsureDirectories();
        string filePath = Path.Combine(AppDataPaths.LogsDirectory, "startup.log");
        string entry = $"[{DateTimeOffset.Now:O}]{Environment.NewLine}{exception}{Environment.NewLine}{Environment.NewLine}";
        File.AppendAllText(filePath, entry);
    }
}
