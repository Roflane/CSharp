namespace XCloudClient.Core;

public static class XCloudClientCore {
    public static async Task<bool> CreateFile(string dir, string fileName, byte[] fileBuffer) {
        try {
            string targetDir = $"{Directory.GetCurrentDirectory()}/{dir}/{fileName}";
            if (string.IsNullOrEmpty(targetDir)) 
                return false;
            
            await File.WriteAllBytesAsync(targetDir, fileBuffer);
            return true;
        }
        catch { return false; }
    }
}