using System;
using Microsoft.Maui.Storage;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
#if ANDROID
using AndroidX.DocumentFile.Provider;
#endif

namespace MigraineTracker.Services
{
    public static class BackupService
    {
        public static async Task<string> ExportBackupAsync(string? directory = null)
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "migraine.db");

            if (!File.Exists(dbPath))
                throw new FileNotFoundException("Database not found", dbPath);

            directory ??= Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (string.IsNullOrWhiteSpace(directory))
                throw new ArgumentException("Destination directory is invalid", nameof(directory));

#if ANDROID
            if (directory.StartsWith("content://", StringComparison.OrdinalIgnoreCase))
            {
                Android.Net.Uri uri = Android.Net.Uri.Parse(directory);
                var folder = DocumentFile.FromTreeUri(Android.App.Application.Context, uri)
                             ?? throw new InvalidOperationException("Unable to access destination directory");

                string fileName = $"migraine_backup_{DateTime.Now:yyyyMMdd_HHmmss}.db";
                var destDoc = folder.CreateFile("application/octet-stream", fileName)
                             ?? throw new InvalidOperationException("Unable to create destination file");

                using FileStream source = File.Open(dbPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var destStream = Android.App.Application.Context.ContentResolver.OpenOutputStream(destDoc.Uri)
                                     ?? throw new InvalidOperationException("Unable to open destination stream");

                await source.CopyToAsync(destStream);
                return destDoc.Uri.ToString();
            }
#endif

            Directory.CreateDirectory(directory);
            string destPath = Path.Combine(directory, $"migraine_backup_{DateTime.Now:yyyyMMdd_HHmmss}.db");
            using FileStream sourceFile = File.Open(dbPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using FileStream destFile = File.Create(destPath);
            await sourceFile.CopyToAsync(destFile);
            return destPath;
        }

        public static async Task ImportBackupAsync(Stream sourceStream)
        {
            if (sourceStream == null)
                throw new ArgumentNullException(nameof(sourceStream));

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "migraine.db");

            // Ensure no DbContext instances hold the database file
            SqliteConnection.ClearAllPools();

            // Reset position in case the stream was previously read
            if (sourceStream.CanSeek)
                sourceStream.Position = 0;

            // Overwrite the existing database with the provided stream
            using FileStream dest = File.Create(dbPath);
            await sourceStream.CopyToAsync(dest);
        }
    }
}
