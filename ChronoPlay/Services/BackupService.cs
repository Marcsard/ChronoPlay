using System.IO;
using ChronoPlay.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ChronoPlay.Services;

public class BackupService
{
    public string CrearRespaldo()
    {
        Directory.CreateDirectory(AppDataPaths.BackupsDirectory);
        string archivo = Path.Combine(
            AppDataPaths.BackupsDirectory,
            $"ChronoPlay_{DateTime.Now:yyyyMMdd_HHmmss_fff}.sqlite");

        using SqliteConnection source = new($"Data Source={AppDataPaths.DatabaseFile};Mode=ReadWrite;Foreign Keys=True");
        using SqliteConnection destination = new($"Data Source={archivo};Mode=ReadWriteCreate;Foreign Keys=True");
        source.Open();
        destination.Open();
        source.BackupDatabase(destination);

        return archivo;
    }
}