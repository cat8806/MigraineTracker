using System;
using Microsoft.Maui.Storage;
using System.IO;
using System.Threading.Tasks;

namespace MigraineTracker.Services
{
    public static class BackupService
    {
        public static async Task<string> ExportBackupAsync()
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "migraine.db");
            string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            Directory.CreateDirectory(documents);
            string destPath = Path.Combine(documents, $"migraine_backup_{DateTime.Now:yyyyMMdd_HHmmss}.db");
            using FileStream source = File.Open(dbPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using FileStream dest = File.Create(destPath);
            await source.CopyToAsync(dest);
            return destPath;
        }

        public static async Task ImportBackupAsync(string sourcePath)
        {
            if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
                throw new FileNotFoundException("Backup file not found", sourcePath);

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "migraine.db");

            // Overwrite the existing database with the selected backup
            using FileStream source = File.OpenRead(sourcePath);
            using FileStream dest = File.Create(dbPath);
            await source.CopyToAsync(dest);
        }
    }
}
