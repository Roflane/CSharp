using System.IO;
using System.IO.Compression;

namespace ZipArchiver.Core;

public static class ZipArchiverCore {
    public static async void ArchiveAsync(string sourceDir, string targetDir, IProgress<double> progress) {
        string[] files = Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories);
        decimal totalBytes = files.Sum(f => new FileInfo(f).Length);
        decimal processedBytes = 0;

        using (FileStream zipToCreate = new FileStream(targetDir, FileMode.Create))
        using (ZipArchive archive = new ZipArchive(zipToCreate, ZipArchiveMode.Create)) {
            foreach (string file in files) {
                string relativePath = Path.GetRelativePath(sourceDir, file);
                ZipArchiveEntry entry = archive.CreateEntry(relativePath, CompressionLevel.SmallestSize);

                using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                using (Stream entryStream = entry.Open()) {
                    byte[] buffer = new byte[4096 * 16];
                    decimal bytesRead;
                    while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0) {
                        await entryStream.WriteAsync(buffer, 0, (int)bytesRead);
                        processedBytes += bytesRead;

                        progress.Report((double)(processedBytes / totalBytes * (decimal)100.0));
                    }
                }
            }
        }
    }

    public static async void UnarchiveAsync(string sourceDir, string targetDir, IProgress<double> progress) {
        ZipArchive archive = ZipFile.OpenRead(sourceDir);
        decimal totalBytes = archive.Entries.Sum(e => e.Length);
        decimal processedBytes = 0;

        foreach (var entry in archive.Entries) {
            if (string.IsNullOrEmpty(entry.Name)) {
                Directory.CreateDirectory(Path.Combine(targetDir, entry.FullName));
                continue;
            }
            
            string destinationPath = Path.Combine(targetDir, entry.FullName);
            Directory.CreateDirectory(Path.GetDirectoryName(destinationPath) ?? string.Empty);

            using var entryStream = entry.Open();
            using var fileStream = File.Create(destinationPath);
            
            byte[] buffer = new byte[4096 * 16];
            int bytesRead;
            while ((bytesRead = entryStream.Read(buffer, 0, buffer.Length)) > 0) {
                await fileStream.WriteAsync(buffer, 0, bytesRead);
                processedBytes += bytesRead;
                
                progress.Report((double)(processedBytes / totalBytes * (decimal)100.0));
            }
        }
    }
}